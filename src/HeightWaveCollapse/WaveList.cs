namespace HeightWaveCollapse
{
	public class WaveList<TCell> where TCell : notnull
	{
		public int Size { get; }
		public WaveFunction<TCell> WaveFunction { get; }
		private readonly IntPtr _nativeList;
		private bool _diposeNative = true;
		internal WaveList(WaveFunction<TCell> waveFunction, int size)
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
				if (!NativeWaveList.WaveListGet(_nativeList, 2, out var i, out var h))
					throw new IndexOutOfRangeException();
				if (!WaveFunction.TryFromIndex(i, out var v))
					throw new KeyNotFoundException($"Invalid cell id {i}");
				return (v, h);
			}
			set
			{
				if (!WaveFunction.TryGetIndex(value.value, out var i))
					throw new KeyNotFoundException($"Invalid cell value [{value.value}]");
				if (!NativeWaveList.WaveListSet(_nativeList, 2, i, value.height))
					throw new IndexOutOfRangeException();
			}
		}
	}
}