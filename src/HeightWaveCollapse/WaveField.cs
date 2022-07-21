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

		public bool AddChunk(int chunkX, int chunkY)
		{
			var exceptions = new List<Exception>();
			bool result;
			lock (_locker)
				result = NativeWaveField.WaveFieldAddChunk(_nativeField, chunkX, chunkY, Initialize);
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
			lock (_locker)
				NativeWaveField.WaveFieldCollapse(_nativeField, WaveFunction._nativeFunction);
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

		[DllImport("HeightWaveCollapseBase")]
		internal static extern void WaveFieldCollapse(IntPtr field, IntPtr func);
	}
}