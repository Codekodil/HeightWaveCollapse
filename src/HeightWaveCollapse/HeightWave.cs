namespace HeightWaveCollapse
{
	public class HeightWave<TCell, TEdge> where TCell : notnull where TEdge : notnull
	{
		private readonly Dictionary<(TEdge left, TEdge right), HashSet<int>> _edgePairings = new Dictionary<(TEdge left, TEdge right), HashSet<int>>();
		private readonly Dictionary<TCell, (TEdge left, TEdge up, TEdge right, TEdge down)> _cells = new Dictionary<TCell, (TEdge left, TEdge up, TEdge right, TEdge down)>();

		public void AddCell(TCell value, TCell clockwise90, TCell clockwise180, TCell clockwise270, TEdge left, TEdge up, TEdge right, TEdge down)
		{
			AddCell(value, left, up, right, down);
			AddCell(clockwise90, down, left, up, right);
			AddCell(clockwise180, right, down, left, up);
			AddCell(clockwise270, up, right, down, left);
		}
		public void AddCell(TCell value, TEdge left, TEdge up, TEdge right, TEdge down)
		{
			lock (_cells)
				_cells[value] = (left, up, right, down);
		}
		public void AddEdgePairing(TEdge same, int deltaHeight = 0)
		{
			lock (_edgePairings)
			{
				var set = _edgePairings.ForceGet((same, same), () => new HashSet<int>());
				set.Add(deltaHeight);
				set.Add(-deltaHeight);
			}
		}
		public void AddEdgePairing(TEdge left, TEdge right, int deltaHeight = 0)
		{
			lock (_edgePairings)
			{
				_edgePairings.ForceGet((left, right), () => new HashSet<int>()).Add(deltaHeight);
				_edgePairings.ForceGet((right, left), () => new HashSet<int>()).Add(-deltaHeight);
			}
		}


		public WaveFunction<TCell> Bake()
		{
			lock (_cells)
				lock (_edgePairings)
				{
					var leftEdge = new Dictionary<TEdge, List<TCell>>();
					var upEdge = new Dictionary<TEdge, List<TCell>>();
					var rightEdge = new Dictionary<TEdge, List<TCell>>();
					var downEdge = new Dictionary<TEdge, List<TCell>>();
					foreach (var cell in _cells)
					{
						leftEdge.ForceGet(cell.Value.left, () => new List<TCell>()).Add(cell.Key);
						upEdge.ForceGet(cell.Value.up, () => new List<TCell>()).Add(cell.Key);
						rightEdge.ForceGet(cell.Value.right, () => new List<TCell>()).Add(cell.Key);
						downEdge.ForceGet(cell.Value.down, () => new List<TCell>()).Add(cell.Key);
					}

					var edgeMapping = new Dictionary<TEdge, List<(TEdge edge, HashSet<int> heights)>>();
					foreach (var edge in _edgePairings)
					{
						edgeMapping.ForceGet(edge.Key.right, () => new List<(TEdge edge, HashSet<int> heights)>())
							.Add((edge.Key.left, edge.Value));
					}

					var connectedRightEdge = ConnectedEdges(leftEdge);
					var connectedDownEdge = ConnectedEdges(upEdge);
					var connectedLeftEdge = ConnectedEdges(rightEdge);
					var connectedUpEdge = ConnectedEdges(downEdge);

					Dictionary<TEdge, HashSet<(TCell cell, int height)>> ConnectedEdges(Dictionary<TEdge, List<TCell>> edges)
					{
						var connected = new Dictionary<TEdge, HashSet<(TCell cell, int height)>>();
						foreach (var edge in edges)
							if (edgeMapping.TryGetValue(edge.Key, out var pairs))
								foreach (var pair in edgeMapping[edge.Key])
								{
									if (!connected.TryGetValue(pair.edge, out var e))
										connected[pair.edge] = e = new HashSet<(TCell cell, int height)>();
									foreach (var cell in edge.Value)
										foreach (var height in pair.heights)
											e.Add((cell, height));
								}
						return connected;
					}

					var connectedCells = new Dictionary<TCell, (
						HashSet<(TCell cell, int height)> left,
						HashSet<(TCell cell, int height)> up,
						HashSet<(TCell cell, int height)> right,
						HashSet<(TCell cell, int height)> down)>();
					foreach (var cell in _cells)
					{
						if (!connectedLeftEdge.TryGetValue(cell.Value.left, out var l) || !l.Any())
							throw new Exception($"[{cell.Key}] has no left connections");
						if (!connectedUpEdge.TryGetValue(cell.Value.up, out var u) || !u.Any())
							throw new Exception($"[{cell.Key}] has no up connections");
						if (!connectedRightEdge.TryGetValue(cell.Value.right, out var r) || !r.Any())
							throw new Exception($"[{cell.Key}] has no right connections");
						if (!connectedDownEdge.TryGetValue(cell.Value.down, out var d) || !d.Any())
							throw new Exception($"[{cell.Key}] has no down connections");
						connectedCells[cell.Key] = (l, u, r, d);
					}

					return new WaveFunction<TCell>(connectedCells);
				}
		}
	}
}
