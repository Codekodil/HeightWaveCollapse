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