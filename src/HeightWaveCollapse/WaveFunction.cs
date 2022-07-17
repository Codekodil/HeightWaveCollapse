using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HeightWaveCollapse
{
	public class WaveFunction<TCell> where TCell : notnull
	{
		public IReadOnlyList<TCell> CellValues { get; }
		private readonly Dictionary<TCell, int> _indices;
		private readonly IntPtr _nativeFunction;
		internal WaveFunction(Dictionary<TCell, (
						HashSet<(TCell cell, int height)> left,
						HashSet<(TCell cell, int height)> up,
						HashSet<(TCell cell, int height)> right,
						HashSet<(TCell cell, int height)> down)> connections)
		{
			var cells = new List<TCell>();
			_indices = new Dictionary<TCell, int>();
			foreach (var cell in connections)
			{
				_indices[cell.Key] = cells.Count;
				cells.Add(cell.Key);
			}
			foreach (var cell in cells)
				if (!connections.ContainsKey(cell))
					throw new Exception($"missing connections for [{cell}]");
			_nativeFunction = NativeWaveFunction.NewWaveFunction(cells.Count);
			for (var i = 0; i < cells.Count; i++)
			{
				var connection = connections[cells[i]];
				if (!NativeWaveFunction.WaveFunctionSetPossibilities(_nativeFunction, i,
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
				list.SetInUse();
				return list;
			}
			CellValues = cells.AsReadOnly();
		}

		~WaveFunction()
		{
			NativeWaveFunction.DeleteWaveFunction(_nativeFunction);
		}

		internal bool TryFromIndex(int index, [MaybeNullWhen(false)] out TCell value)
		{
			if (index < 0 || index >= CellValues.Count)
			{
				value = default;
				return false;
			}
			value = CellValues[index];
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