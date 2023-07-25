using System.Collections;

namespace HeightWaveCollapse
{
	public sealed class WaveList<TCell> : IEnumerable<(TCell Value, int Height)> where TCell : notnull
	{
		public int Size { get; }
		public WaveFunction<TCell> WaveFunction { get; }
		public bool InUse => !_diposeNative;

		internal readonly HeightWaveCollapseBase.HeightWaveCollapseBase.WaveList _nativeList;
		private bool _diposeNative = true;
		private readonly object _locker = new object();

		public WaveList(WaveFunction<TCell> waveFunction, int size)
		{
			if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
			Size = size;
			WaveFunction = waveFunction;
			_nativeList = new HeightWaveCollapseBase.HeightWaveCollapseBase.WaveList(Size);
		}

		public WaveList(WaveFunction<TCell> waveFunction, IEnumerable<(TCell Value, int Height)> values)
		{
			var list = values.ToList();
			Size = list.Count;
			WaveFunction = waveFunction;
			_nativeList = new HeightWaveCollapseBase.HeightWaveCollapseBase.WaveList(Size);
			for (int i = 0; i < list.Count; i++)
				this[i] = list[i];
		}

		internal WaveList(WaveFunction<TCell> waveFunction, HeightWaveCollapseBase.HeightWaveCollapseBase.WaveList native)
		{
			_nativeList = native;
			Size = _nativeList.GetSize();
			WaveFunction = waveFunction;
		}

		~WaveList()
		{
			if (_diposeNative)
				_nativeList.Dispose();
		}

		internal bool SetInUse()
		{
			if (InUse) return false;
			lock (_locker)
			{
				if (InUse) return false;
				_diposeNative = false;
				return true;
			}
		}
		internal void Free() => _diposeNative = true;

		public IEnumerator<(TCell Value, int Height)> GetEnumerator()
		{
			for (var i = 0; i < Size; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public (TCell Value, int Height) this[int index]
		{
			get
			{
				lock (_locker)
				{
					if (InUse)
						throw new InvalidOperationException("can not access lists that are in use");
					int i = default, h = default;
					if (!_nativeList.Get(index, ref i, ref h))
						throw new IndexOutOfRangeException();
					if (!WaveFunction.TryFromIndex(i, out var v))
						throw new KeyNotFoundException($"Invalid cell id {i}");
					return (v, h);
				}
			}
			set
			{
				lock (_locker)
				{
					if (InUse)
						throw new InvalidOperationException("can not access lists that are in use");
					if (!WaveFunction.TryGetIndex(value.Value, out var i))
						throw new KeyNotFoundException($"Invalid cell value [{value.Value}]");
					if (!_nativeList.Set(index, i, value.Height))
						throw new IndexOutOfRangeException();
				}
			}
		}
	}
}