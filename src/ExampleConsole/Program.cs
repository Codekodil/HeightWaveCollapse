using HeightWaveCollapse;

var wave = new HeightWave<char, bool>();

wave.AddEdgePairing(false);
wave.AddEdgePairing(true);

wave.AddCell(' ', false, false, false, false);
//wave.AddCell('╡', '╨', '╞', '╥', true, false, false, false);
//wave.AddCell('┤', '┴', '├', '┬', true, false, false, false);
wave.AddCell('═', true, false, true, false);
wave.AddCell('║', false, true, false, true);
//wave.AddCell('╔', '╗', '╝', '╚', false, false, true, true);
wave.AddCell('╠', '╦', '╣', '╩', false, true, true, true);
//wave.AddCell('╬', true, true, true, true);

var wf = wave.Bake();

var rand = new Random();
var field = new DelegateWaveField<char>(wf, 80, 80, (x, y) =>
	new WaveList<char>(wf, wf.CellValues.OrderBy(_ => rand.Next()).Select(v => (v, 0))));
field.AddChunk(0, 0);
field.AddChunk(1, 0);
field.AddChunk(2, 0);

field.Collapse();

Display();

void Display()
{
	for (var y = 79; y >= 0; y--)
	{
		for (var x = 0; x < 240; x++)
		{
			var possibilities = field.PossibilitiesAt(x, y);
			if (possibilities == null || possibilities.Size == 0)
				Console.Write("0");
			else if (possibilities.Size == 1)
				Console.Write(possibilities[0].value);
			else if (possibilities.Size < 9)
				Console.Write(possibilities.Size);
			else
				Console.Write("*");
		}
		Console.WriteLine("");
	}
}