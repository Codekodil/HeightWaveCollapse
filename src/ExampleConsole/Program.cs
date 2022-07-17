using HeightWaveCollapse;

Console.WriteLine("╔╦═╗");
Console.WriteLine("╠╬═╣");
Console.WriteLine("║║ ║");
Console.WriteLine("╚╩═╝");

var wave = new HeightWave<char, bool>();

wave.AddEdgePairing(false);
wave.AddEdgePairing(true);

wave.AddCell(' ', false, false, false, false);
wave.AddCell('╡', '╨', '╞', '╥', true, false, false, false);
wave.AddCell('═', true, false, true, false);
wave.AddCell('║', false, true, false, true);
wave.AddCell('╔', '╗', '╝', '╚', false, false, true, true);
wave.AddCell('╠', '╦', '╣', '╩', false, true, true, true);
wave.AddCell('╬', true, true, true, true);

var wf = wave.Bake();

var field = new WaveField<char>(wf, 10, 10, (x, y) =>
{
	var list = new WaveList<char>(wf, wf.CellValues.Count);
	for (int i = 0; i < list.Size; i++)
		list[i] = (wf.CellValues[i], 0);
	return list;
});
field.AddChunk(0, 0);
field.AddChunk(1, 0);

var list = new WaveList<char>(wf, 10);
list[2] = ('╠', 3);
list[3] = ('═', 4);
Console.WriteLine(list[2]);
Console.WriteLine(list[3]);