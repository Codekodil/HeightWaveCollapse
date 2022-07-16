namespace HeightWaveCollapse
{
	public class WaveFunction<TCell> where TCell : notnull
	{
		internal WaveFunction(Dictionary<TCell, (
						HashSet<(TCell cell, int height)> left,
						HashSet<(TCell cell, int height)> up,
						HashSet<(TCell cell, int height)> right,
						HashSet<(TCell cell, int height)> down)> connections)
		{
			var cells = new List<TCell>();
			var indices = new Dictionary<TCell, int>();
			int ToIndex(TCell cell)
			{
				if (indices.TryGetValue(cell, out var i))
					return i;
				i = cells.Count;
				cells.Add(cell);
				return indices[cell] = i;
			}
			foreach (var cell in connections)
				ToIndex(cell.Key);
			for (int i = 0; i < cells.Count; i++)
				Console.WriteLine($"{i}:\t[{cells[i]}]");
		}
	}
}
