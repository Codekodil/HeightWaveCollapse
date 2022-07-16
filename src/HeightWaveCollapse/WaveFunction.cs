using System.Diagnostics.CodeAnalysis;

namespace HeightWaveCollapse
{
	public class WaveFunction<TCell> where TCell : notnull
	{
		private readonly List<TCell> _cells;
		private readonly Dictionary<TCell, int> _indices;
		internal WaveFunction(Dictionary<TCell, (
						HashSet<(TCell cell, int height)> left,
						HashSet<(TCell cell, int height)> up,
						HashSet<(TCell cell, int height)> right,
						HashSet<(TCell cell, int height)> down)> connections)
		{
			_cells = new List<TCell>();
			_indices = new Dictionary<TCell, int>();
			int ToIndex(TCell cell)
			{
				if (_indices.TryGetValue(cell, out var i))
					return i;
				i = _cells.Count;
				_cells.Add(cell);
				return _indices[cell] = i;
			}
			foreach (var cell in connections)
				ToIndex(cell.Key);
		}

		internal bool TryFromIndex(int index, [MaybeNullWhen(false)] out TCell value)
		{
			if (index < 0 || index >= _cells.Count)
			{
				value = default;
				return false;
			}
			value = _cells[index];
			return true;
		}
		internal bool TryGetIndex(TCell value, out int index) => _indices.TryGetValue(value, out index);

		public WaveList<TCell> NewWaveList(int size) => new WaveList<TCell>(this, size);
	}
}