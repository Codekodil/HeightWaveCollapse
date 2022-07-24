using HeightWaveCollapse;

namespace ExampleConsole
{
	public class TileField : WaveField<(ETileType Tile, EOrientation Orientation)>
	{
		public TileField(WaveFunction<(ETileType Tile, EOrientation Orientation)> waveFunction, int chunkWidth, int chunkHeight) : base(waveFunction, chunkWidth, chunkHeight) { }

		int Distance(int x, int y)
		{
			return new[] {
				Rect(39, 10, 42, 93),
				Rect(197, 10, 200, 130),
				Rect(547, 50, 550, 130),
				Rect(197, 127, 550, 130),
				Rect(197, 50, 550, 53),
				Rect(40, 90, 200, 93),
				Rect(40, 10, 200, 13) }.Min();

			int Rect(int left, int down, int right, int up)
			{
				return Math.Max(
					x < left ? left - x : right < x ? x - right : 0,
					y < down ? down - y : up < y ? y - up : 0);
			}
		}

		protected override WaveList<(ETileType Tile, EOrientation Orientation)> CellInitializer(int x, int y)
		{
			IEnumerable<(ETileType Tile, EOrientation Orientation)> validCells = WaveFunction.CellValues;

			var d = Distance(x, y);
			if (d == 0)
				validCells = validCells.Where(c => c.Tile == ETileType.Flat || c.Tile == ETileType.Stairs);
			else if (d > 10)
				validCells = validCells.Where(c => c.Tile != ETileType.Flat && c.Tile != ETileType.Stairs);
			else
				validCells = validCells.Where(c => c.Tile != ETileType.Stairs && c.Tile != ETileType.Shrub);


			var (_, min, max) = HeightTarget(x, y);
			if (min == max)
				return new WaveList<(ETileType Tile, EOrientation Orientation)>(WaveFunction, validCells.SelectMany(v => new[] { (v, min) }));
			return new WaveList<(ETileType Tile, EOrientation Orientation)>(WaveFunction, validCells.SelectMany(v => new[] { (v, min), (v, max) }));
		}

		(int Desired, int Min, int Max) HeightTarget(int x, int y)
		{
			int min, max;

			if (x < 100) max = 0;
			else if (x < 300) max = 1;
			else if (x < 500) max = 2;
			else max = 3;

			if (x < 200) min = 0;
			else if (x < 400) min = 1;
			else if (x < 600) min = 2;
			else min = 3;

			if (min == max) return (min, min, min);
			return (Math.Abs(Tuple.Create(Math.Sqrt(x / 3), Math.Sqrt(y / 3)).GetHashCode()) % (max - min + 1) + min, min, max);
		}

		private readonly Random _rand = new Random();

		public event Action? AfterCollapseCell;
		protected override ((ETileType Tile, EOrientation Orientation) Value, int Height) CollapseCell(int x, int y, WaveList<(ETileType Tile, EOrientation Orientation)> possibilities)
		{
			try
			{
				var (desired, _, _) = HeightTarget(x, y);

				var heighWithCliff = new List<((ETileType Tile, EOrientation Orientation) Value, int Height)>();
				var heigh = new List<((ETileType Tile, EOrientation Orientation) Value, int Height)>();
				var goodWithCliff = new List<((ETileType Tile, EOrientation Orientation) Value, int Height)>();
				var good = new List<((ETileType Tile, EOrientation Orientation) Value, int Height)>();
				var lowWithCliff = new List<((ETileType Tile, EOrientation Orientation) Value, int Height)>();
				var low = new List<((ETileType Tile, EOrientation Orientation) Value, int Height)>();

				foreach (var p in possibilities)
				{
					var isCliff = p.Value.Tile == ETileType.Cliff || p.Value.Tile == ETileType.Stairs;
					if (p.Height == desired)
					{
						if (isCliff)
							goodWithCliff.Add(p);
						else
							good.Add(p);
					}
					else if (p.Height > desired)
					{
						if (isCliff)
							heighWithCliff.Add(p);
						else
							heigh.Add(p);
					}
					else
					{
						if (isCliff)
							lowWithCliff.Add(p);
						else
							low.Add(p);
					}
				}

				var filtered = good.Count > 0 ? good :
					goodWithCliff.Count > 0 ? goodWithCliff :
					lowWithCliff.Count > 0 ? lowWithCliff :
					heigh.Count > 0 ? heigh :
					low.Count > 0 ? low :
					heighWithCliff;

				foreach (var item in filtered)
					if (item.Value.Tile == ETileType.Tree)
						switch (item.Value.Orientation)
						{
							case EOrientation.CornerUpRight: if ((x + item.Height) % 2 == 0 && (y + item.Height) % 2 == 0) return item; break;
							case EOrientation.CornerDownRight: if ((x + item.Height) % 2 == 0 && (y + item.Height + 1) % 2 == 0) return item; break;
							case EOrientation.CornerDownLeft: if ((x + item.Height + 1) % 2 == 0 && (y + item.Height + 1) % 2 == 0) return item; break;
							case EOrientation.CornerUpLeft: if ((x + item.Height + 1) % 2 == 0 && (y + item.Height) % 2 == 0) return item; break;
						}

				return filtered[_rand.Next(filtered.Count)];
			}
			finally
			{
				AfterCollapseCell?.Invoke();
			}
		}
	}
}
