using System.Runtime.InteropServices;

namespace HeightWaveCollapse
{
	public class WaveList<TCell> where TCell : notnull
	{
		public int Size { get; }
		public WaveFunction<TCell> WaveFunction { get; }
		internal readonly IntPtr _nativeList;
		internal bool _diposeNative = true;
		public bool InUse => !_diposeNative;
		public WaveList(WaveFunction<TCell> waveFunction, int size)
		{
			if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
			Size = size;
			WaveFunction = waveFunction;
			_nativeList = NativeWaveList.NewWaveList(Size);
		}

		~WaveList()
		{
			if (_diposeNative)
				NativeWaveList.DeleteWaveList(_nativeList);
		}

		public (TCell value, int height) this[int index]
		{
			get
			{
				if (InUse)
					throw new InvalidOperationException("can not access lists that are in use");
				if (!NativeWaveList.WaveListGet(_nativeList, 2, out var i, out var h))
					throw new IndexOutOfRangeException();
				if (!WaveFunction.TryFromIndex(i, out var v))
					throw new KeyNotFoundException($"Invalid cell id {i}");
				return (v, h);
			}
			set
			{
				if (InUse)
					throw new InvalidOperationException("can not access lists that are in use");
				if (!WaveFunction.TryGetIndex(value.value, out var i))
					throw new KeyNotFoundException($"Invalid cell value [{value.value}]");
				if (!NativeWaveList.WaveListSet(_nativeList, 2, i, value.height))
					throw new IndexOutOfRangeException();
			}
		}
	}

	internal static class NativeWaveList
	{
		[DllImport("HeightWaveCollapseBase")]
		internal static extern IntPtr NewWaveList(int size);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern void DeleteWaveList(IntPtr list);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern bool WaveListGet(IntPtr list, int index, out int id, out int height);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern bool WaveListSet(IntPtr list, int index, int id, int height);
	}
}