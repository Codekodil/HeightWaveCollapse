namespace HeightWaveCollapse
{
	public abstract class WaveField<TCell> where TCell : notnull
	{
		public int ChunkHeight { get; }
		public int ChunkWidth { get; }
		public WaveFunction<TCell> WaveFunction { get; }
		private readonly HeightWaveCollapseBase.HeightWaveCollapseBase.WaveField _nativeField;
		private int _pendingCollapses = 0;
		private readonly object _locker = new object();
		private readonly Dictionary<(int X, int Y), Task> _chunks = new Dictionary<(int X, int Y), Task>();
		public WaveField(WaveFunction<TCell> waveFunction, int chunkWidth, int chunkHeight)
		{
			if (chunkWidth <= 0) throw new ArgumentOutOfRangeException(nameof(chunkWidth));
			if (chunkHeight <= 0) throw new ArgumentOutOfRangeException(nameof(chunkHeight));
			WaveFunction = waveFunction;
			ChunkWidth = chunkWidth;
			ChunkHeight = chunkHeight;
			_nativeField = new HeightWaveCollapseBase.HeightWaveCollapseBase.WaveField(ChunkWidth, ChunkHeight);
		}

		protected abstract WaveList<TCell> CellInitializer(int x, int y);
		protected virtual (TCell Value, int Height) CollapseCell(int x, int y, WaveList<TCell> possibilities)
		{
			return possibilities[Math.Abs(Tuple.Create(x, y).GetHashCode()) % possibilities.Size];
		}

		public bool AddChunk(int chunkX, int chunkY)
		{
			var exceptions = new List<Exception>();
			bool result;

			lock (_locker)
			{
				if (_pendingCollapses > 0)
					throw new InvalidOperationException("pending collapse");

				var listReferences = new List<HeightWaveCollapseBase.HeightWaveCollapseBase.WaveList>();
				using var initializer = new HeightWaveCollapseBase.HeightWaveCollapseBase.CellInitializer();
				initializer.InitCell += (x, y) =>
				{
					var list = Initialize(x, y);
					listReferences.Add(list);
					return list.Native!.Value;
				};

				result = _nativeField.AddChunk(chunkX, chunkY, initializer);
				_chunks[(chunkX, chunkY)] = Task.CompletedTask;
			}
			if (exceptions.Count > 0)
				throw new AggregateException(exceptions);
			return result;

			HeightWaveCollapseBase.HeightWaveCollapseBase.WaveList Initialize(int x, int y)
			{
				try
				{
					var list = CellInitializer(x, y);
					if (list.WaveFunction != WaveFunction)
						throw new InvalidOperationException($"list of size [{list.Size}] originates from a different WaveFunction");
					return list._nativeList;
				}
				catch (Exception ex)
				{
					exceptions.Add(new Exception($"Initialize Exception at {x}/{y}", ex));
				}
				var emptyList = new WaveList<TCell>(WaveFunction, 0);
				return emptyList._nativeList;
			}
		}

		public List<(int ChunkX, int ChunkY)> GetChunks()
		{
			lock (_locker)
				return _chunks.Keys.ToList();
		}
		public IEnumerable<(int X, int Y)> GetCells()
		{
			foreach (var (chunkX, chunkY) in GetChunks())
			{
				for (var x = 0; x < ChunkWidth; x++)
					for (var y = 0; y < ChunkHeight; y++)
						yield return (chunkX * ChunkWidth + x, chunkY * ChunkHeight + y);
			}
		}

		public WaveList<TCell>? PossibilitiesAt(int x, int y)
		{
			lock (_locker)
			{
				var nativeList = _nativeField.ListAt(x, y);
				if (nativeList is null)
					return null;
				return new WaveList<TCell>(WaveFunction, nativeList);
			}
		}

		public Task CollapseAsync()
		{
			var minX = int.MaxValue;
			var minY = int.MaxValue;
			var maxX = int.MinValue;
			var maxY = int.MinValue;
			lock (_locker)
			{
				foreach (var chunk in _chunks)
				{
					minX = Math.Min(minX, chunk.Key.X);
					minY = Math.Min(minY, chunk.Key.Y);
					maxX = Math.Max(maxX, chunk.Key.X);
					maxY = Math.Max(maxY, chunk.Key.Y);
				}
			}
			minX *= ChunkWidth; minY *= ChunkHeight;
			maxX *= ChunkWidth; maxY *= ChunkHeight;
			return CollapseAsync(minX, minY, maxX - minX + ChunkWidth, maxY - minY + ChunkHeight, 1);
		}

		public async Task CollapseAsync(int x, int y, int width, int height, int reduceBorder)
		{
			var borderChunkMinX = x - reduceBorder; borderChunkMinX = borderChunkMinX < 0 ? (borderChunkMinX - ChunkWidth + 1) / ChunkWidth : borderChunkMinX / ChunkWidth;
			var borderChunkMinY = y - reduceBorder; borderChunkMinY = borderChunkMinY < 0 ? (borderChunkMinY - ChunkHeight + 1) / ChunkHeight : borderChunkMinY / ChunkHeight;
			var borderChunkMaxX = x + width + reduceBorder - 1; borderChunkMaxX = borderChunkMaxX < 0 ? (borderChunkMaxX - ChunkWidth + 1) / ChunkWidth : borderChunkMaxX / ChunkWidth;
			var borderChunkMaxY = y + height + reduceBorder - 1; borderChunkMaxY = borderChunkMaxY < 0 ? (borderChunkMaxY - ChunkHeight + 1) / ChunkHeight : borderChunkMaxY / ChunkHeight;

			var completionSource = new TaskCompletionSource();

			lock (_locker)
			{
				_pendingCollapses++;
			}
			try
			{
				Task? waitBeforeNextTryReserve = Task.CompletedTask;
				while (waitBeforeNextTryReserve is not null)
				{
					await waitBeforeNextTryReserve.ConfigureAwait(false);
					lock (_locker)
					{
						var requiredChunks = _chunks.Where(kv =>
							kv.Key.X >= borderChunkMinX &&
							kv.Key.Y >= borderChunkMinY &&
							kv.Key.X <= borderChunkMaxX &&
							kv.Key.Y <= borderChunkMaxY).ToList();

						waitBeforeNextTryReserve = Task.WhenAll(requiredChunks.Select(kv => kv.Value));

						if (waitBeforeNextTryReserve.IsCompleted)
						{
							waitBeforeNextTryReserve = null;
							foreach (var chunk in requiredChunks)
								_chunks[chunk.Key] = completionSource.Task;
						}
					}
				}

				var threadWait = new TaskCompletionSource();
				var thread = new Thread(() =>
				{
					try
					{
						List<Exception> exceptions = new List<Exception>();

						using var collapse = new HeightWaveCollapseBase.HeightWaveCollapseBase.CellCollapse();
						collapse.CollapseCell += CollapseField;

						using var reduceChunks = new HeightWaveCollapseBase.HeightWaveCollapseBase.Area(borderChunkMinX, borderChunkMinY, borderChunkMaxX - borderChunkMinX + 1, borderChunkMaxY - borderChunkMinY + 1);
						using var collapseArea = new HeightWaveCollapseBase.HeightWaveCollapseBase.Area(x, y, width, height);

						_nativeField.Collapse(WaveFunction._nativeFunction, reduceChunks, collapseArea, collapse);

						if (exceptions.Count > 0)
							throw new AggregateException(exceptions);

						void CollapseField(int x, int y, ref int id, ref int height)
						{
							try
							{
								var possibilities = PossibilitiesAt(x, y);
								if (possibilities == null)
								{
									id = -1;
									height = 0;
									return;
								}
								var result = CollapseCell(x, y, possibilities);
								if (!WaveFunction.TryGetIndex(result.Value, out var index))
									throw new InvalidOperationException($"Invalid cell id {result.Value}");
								id = index;
								height = result.Height;
							}
							catch (Exception ex)
							{
								exceptions.Add(new Exception($"Collapse Exception at {x}/{y}", ex));
								id = -1;
								height = 0;
							}
						}

						threadWait.SetResult();
					}
					catch (Exception ex)
					{
						threadWait.SetException(ex);
					}
				});
				thread.IsBackground = true;
				thread.Start();

				await threadWait.Task.ConfigureAwait(false);
			}
			finally
			{
				lock (_locker)
				{
					_pendingCollapses--;
				}
				completionSource.SetResult();
			}
		}
	}
}