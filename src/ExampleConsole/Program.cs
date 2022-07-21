using ExampleConsole;
using HeightWaveCollapse;

var wave = new HeightWave<char, EEdge>();

wave.AddEdgePairing(EEdge.None);
wave.AddEdgePairing(EEdge.Connect);
wave.AddEdgePairing(EEdge.MaybeDown);
wave.AddEdgePairing(EEdge.MaybeDown, EEdge.None, 0);
wave.AddEdgePairing(EEdge.Down, EEdge.None, -1);

wave.AddEdgePairing(EEdge.Down, EEdge.MaybeDown, -1);
wave.AddEdgePairing(EEdge.MaybeDown, EEdge.None, -1);
wave.AddEdgePairing(EEdge.MaybeDown, EEdge.MaybeDown, -1);

wave.AddCell(' ', EEdge.None, EEdge.None, EEdge.None, EEdge.None);
//wave.AddCell('╡', '╨', '╞', '╥', true, false, false, false);
//wave.AddCell('┤', '┴', '├', '┬', true, false, false, false);
wave.AddCell('═', EEdge.Connect, EEdge.MaybeDown, EEdge.Connect, EEdge.MaybeDown);
wave.AddCell('║', EEdge.MaybeDown, EEdge.Connect, EEdge.MaybeDown, EEdge.Connect);
wave.AddCell('╔', '╗', '╝', '╚', EEdge.Down, EEdge.Down, EEdge.Connect, EEdge.Connect);
wave.AddCell('╠', '╦', '╣', '╩', EEdge.MaybeDown, EEdge.Connect, EEdge.Connect, EEdge.Connect);
//wave.AddCell('╬', true, true, true, true);

var wf = wave.Bake();

var rand = new Random();
var field = new CharWaveField(wf, 80, 80);
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
			Console.ForegroundColor = ConsoleColor.White;
			var possibilities = field.PossibilitiesAt(x, y);
			if (possibilities == null || possibilities.Size == 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("0");
			}
			else if (possibilities.Size == 1)
			{
				if (possibilities[0].height == 1)
					Console.ForegroundColor = ConsoleColor.Green;
				Console.Write(possibilities[0].value);
			}
			else if (possibilities.Size < 9)
				Console.Write(possibilities.Size);
			else
				Console.Write("*");
		}
		Console.WriteLine("");
	}
}