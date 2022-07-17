using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HeightWaveCollapse
{
	public class WaveFunction<TCell> where TCell : notnull
	{
		private readonly List<TCell> _cells;
		private readonly Dictionary<TCell, int> _indices;
		private readonly IntPtr _nativeFunction;
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
			foreach (var cell in _cells)
				if (!connections.ContainsKey(cell))
					throw new Exception($"missing connections for [{cell}]");
			_nativeFunction = NativeWaveFunction.NewWaveFunction(_cells.Count);
			for (var i = 0; i < _cells.Count; i++)
			{
				var connection = connections[_cells[i]];
				if(!NativeWaveFunction.WaveFunctionSetPossibilities(_nativeFunction, i,
					ListFromSet(connection.left)._nativeList,
					ListFromSet(connection.up)._nativeList,
					ListFromSet(connection.right)._nativeList,
					ListFromSet(connection.down)._nativeList))
					throw new Exception($"internal error with possible memory leak (SetPossibilities)");
			}
			WaveList<TCell> ListFromSet(HashSet<(TCell cell, int height)> set)
			{
				var list = new WaveList<TCell>(this, set.Count);
				var i = 0;
				foreach (var cell in set)
					list[i++] = (cell.cell, cell.height);
				list._diposeNative = false;
				return list;
			}
		}

		~WaveFunction()
		{
			NativeWaveFunction.DeleteWaveFunction(_nativeFunction);
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
	}

	internal static class NativeWaveFunction
	{
		[DllImport("HeightWaveCollapseBase")]
		internal static extern IntPtr NewWaveFunction(int possibilities);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern void DeleteWaveFunction(IntPtr func);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern bool WaveFunctionSetPossibilities(IntPtr func, int index, IntPtr left, IntPtr up, IntPtr right, IntPtr down);
	}
}