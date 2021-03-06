using System.Runtime.InteropServices;

namespace HeightWaveCollapse
{
	public abstract class WaveField<TCell> where TCell : notnull
	{
		public int ChunkHeight { get; }
		public int ChunkWidth { get; }
		public WaveFunction<TCell> WaveFunction { get; }
		private readonly IntPtr _nativeField;
		private readonly object _locker = new object();
		private readonly HashSet<(int X, int Y)> _chunks = new HashSet<(int X, int Y)>();
		public WaveField(WaveFunction<TCell> waveFunction, int chunkWidth, int chunkHeight)
		{
			if (chunkWidth <= 0) throw new ArgumentOutOfRangeException(nameof(chunkWidth));
			if (chunkHeight <= 0) throw new ArgumentOutOfRangeException(nameof(chunkHeight));
			WaveFunction = waveFunction;
			ChunkWidth = chunkWidth;
			ChunkHeight = chunkHeight;
			_nativeField = NativeWaveField.NewWaveField(ChunkWidth, ChunkHeight);
		}

		~WaveField()
		{
			NativeWaveField.DeleteWaveField(_nativeField);
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
				result = NativeWaveField.WaveFieldAddChunk(_nativeField, chunkX, chunkY, Initialize);
				_chunks.Add((chunkX, chunkY));
			}
			if (exceptions.Count > 0)
				throw new AggregateException(exceptions);
			return result;

			IntPtr Initialize(int x, int y)
			{
				try
				{
					var list = CellInitializer(x, y);
					if (list.WaveFunction != WaveFunction)
						throw new InvalidOperationException($"list of size [{list.Size}] originates from a different WaveFunction");
					if (!list.SetInUse())
						throw new InvalidOperationException($"list of size [{list.Size}] is already in use");
					return list._nativeList;
				}
				catch (Exception ex)
				{
					exceptions.Add(new Exception($"Initialize Exception at {x}/{y}", ex));
				}
				var emptyList = new WaveList<TCell>(WaveFunction, 0);
				emptyList.SetInUse();
				return emptyList._nativeList;
			}
		}

		public List<(int ChunkX, int ChunkY)> GetChunks()
		{
			lock (_locker)
				return _chunks.ToList();
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
				var nativeList = NativeWaveField.WaveFieldListAt(_nativeField, x, y);
				if (nativeList == IntPtr.Zero)
					return null;
				return new WaveList<TCell>(WaveFunction, nativeList);
			}
		}

		public void Collapse()
		{
			var exceptions = new List<Exception>();
			lock (_locker)
				NativeWaveField.WaveFieldCollapse(_nativeField, WaveFunction._nativeFunction, CollapseField);
			if (exceptions.Count > 0)
				throw new AggregateException(exceptions);

			void CollapseField(int x, int y, out int id, out int height)
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
		}
	}

	internal static class NativeWaveField
	{
		[DllImport("HeightWaveCollapseBase")]
		internal static extern IntPtr NewWaveField(int chunkWidth, int chunkHeight);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern void DeleteWaveField(IntPtr field);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate IntPtr InitCellDelegate(int x, int y);
		[DllImport("HeightWaveCollapseBase")]
		internal static extern bool WaveFieldAddChunk(IntPtr field, int chunkX, int chunkY, [MarshalAs(UnmanagedType.FunctionPtr)] InitCellDelegate initCell);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern IntPtr WaveFieldListAt(IntPtr field, int x, int y);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CollapseFieldDelegate(int x, int y, out int id, out int height);
		[DllImport("HeightWaveCollapseBase")]
		internal static extern void WaveFieldCollapse(IntPtr field, IntPtr func, [MarshalAs(UnmanagedType.FunctionPtr)] CollapseFieldDelegate ccollapseField);
	}
}