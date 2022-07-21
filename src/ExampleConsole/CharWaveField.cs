using HeightWaveCollapse;

namespace ExampleConsole
{
	public class CharWaveField : WaveField<char>
	{
		public CharWaveField(WaveFunction<char> waveFunction, int chunkWidth, int chunkHeight) : base(waveFunction, chunkWidth, chunkHeight) { }

		private readonly Random _rand = new Random();
		protected override WaveList<char> CellInitializer(int x, int y)
		{
			if (Math.Abs(x - 100) + Math.Abs(y - 40) < 25)
				return new WaveList<char>(WaveFunction, WaveFunction.CellValues.OrderBy(_ => _rand.Next()).Select(v => (v, 1)));
			if (Math.Abs(x - 120) > 80)
				return new WaveList<char>(WaveFunction, WaveFunction.CellValues.OrderBy(_ => _rand.Next()).Select(v => (v, 0)));
			return new WaveList<char>(WaveFunction, WaveFunction.CellValues.OrderBy(_ => _rand.Next()).SelectMany(v => new[] { (v, 0), (v, 1) }));
		}

		protected override (char value, int height) CollapseCell(int x, int y, WaveList<char> possibilities)
		{
			var buckets = possibilities.GroupBy(v => v.height).ToDictionary(g => g.Key, g => g.ToList());

			buckets.TryGetValue(0, out var b0);
			buckets.TryGetValue(1, out var b1);

			//var desired = Math.Abs(x - 100) + Math.Abs(y - 40) > 10 ? (b0 ?? b1) : (b1 ?? b0);
			var desired = y % 2 == 0 ? b0 ?? b1 : b1 ?? b0;
			if (desired == null)
				return possibilities[0];

			return desired[_rand.Next(desired.Count)];
		}
	}
}
