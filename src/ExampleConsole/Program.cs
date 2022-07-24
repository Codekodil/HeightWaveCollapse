using ExampleConsole;

var newWave = new TileWave();

newWave.AddEdgePairing(EEdge.None);
newWave.AddTile(ETileType.Flat, EEdge.None, EEdge.None, EEdge.None, EEdge.None);
newWave.AddTile(ETileType.Shrub, EEdge.None, EEdge.None, EEdge.None, EEdge.None);

newWave.AddEdgePairing(EEdge.TreeCW, EEdge.TreeCCW);
newWave.AddCorner(ETileType.Tree, EEdge.None, EEdge.TreeCW, EEdge.TreeCCW, EEdge.None);

newWave.AddEdgePairing(EEdge.CliffCW, EEdge.CliffCCW);
newWave.AddCorner(ETileType.Cliff, EEdge.None, EEdge.CliffCW, EEdge.CliffCCW, EEdge.None);
newWave.AddEdgePairing(EEdge.CliffTransition);
newWave.AddEdgePairing(EEdge.CliffTransition, EEdge.None, 1);
newWave.AddTransition(ETileType.Cliff, EEdge.CliffCW, EEdge.CliffTransition, EEdge.CliffCCW, EEdge.None);
newWave.AddInnerCorner(ETileType.Cliff, EEdge.CliffTransition, EEdge.CliffCCW, EEdge.CliffCW, EEdge.CliffTransition);

newWave.AddEdgePairing(EEdge.StairTransition, EEdge.None, 1);
newWave.AddTransition(ETileType.Stairs, EEdge.CliffCW, EEdge.StairTransition, EEdge.CliffCCW, EEdge.None);

var newWf = newWave.Bake();

var newField = new TileField(newWf, Console.WindowWidth / 3, Console.WindowHeight / 2);//Dividing is only there for testing multiple chunks
newField.AddChunk(0, 0);
newField.AddChunk(0, 1);
newField.AddChunk(1, 0);
newField.AddChunk(1, 1);
newField.AddChunk(2, 0);
newField.AddChunk(2, 1);

var displayCache = new Dictionary<(int x, int y), (ConsoleColor bg, ConsoleColor fg, char c)>();
Task? displayRunning = null;
var forceDisplayCounter = 0;
newField.AfterCollapseCell += Display;

Display();
newField.Collapse();
while (displayRunning != null)
	Thread.Sleep(100);
Display();
displayRunning?.GetAwaiter().GetResult();

void Display()
{
	if (forceDisplayCounter++ > 1000)
	{
		forceDisplayCounter = 0;
		while (displayRunning != null)
			Thread.Sleep(10);
	}
	if (displayRunning != null) return;

	var cells = newField.GetCells().Select(c => (c, newField.PossibilitiesAt(c.X, c.Y))).ToList();

	displayRunning = Task.Run(() =>
	{
		try
		{
			var lastBg = Console.BackgroundColor;
			var lastFg = Console.ForegroundColor;
			foreach (var (c, p) in cells)
			{
				(ConsoleColor bg, ConsoleColor fg, char c) data;

				if (p == null || p.Size == 0)
					data = (ConsoleColor.Black, ConsoleColor.Red, '0');
				else if (p.Size == 1)
					data = DrawingData(p[0].Value, p[0].Height);
				else
					data = (ConsoleColor.Black, p.Size <= 9 ? ConsoleColor.White : ConsoleColor.Gray, Math.Min(p.Size, 9).ToString()[0]);

				if (displayCache.TryGetValue(c, out var lastData) && lastData == data)
					continue;

				displayCache[c] = data;
				Console.SetCursorPosition(c.X, Console.WindowHeight - 1 - c.Y);
				if (lastBg != data.bg)
					Console.BackgroundColor = lastBg = data.bg;
				if (lastFg != data.fg)
					Console.ForegroundColor = lastFg = data.fg;
				Console.Write(data.c);
			}
		}
		finally
		{
			displayRunning = null;
		}
	});
}

(ConsoleColor bg, ConsoleColor fg, char c) DrawingData((ETileType Tile, EOrientation Orientation) cell, int height)
{
	var heightBg = height == 0 ?
		ConsoleColor.DarkGreen :
		height == 1 ?
		ConsoleColor.Green :
		height == 2 ?
		ConsoleColor.DarkYellow :
		ConsoleColor.DarkGray;
	switch (cell.Tile)
	{
		case ETileType.Flat: return (heightBg, heightBg, ' ');
		case ETileType.Shrub: return (heightBg, ConsoleColor.DarkCyan, '#');
		case ETileType.Tree:
			switch (cell.Orientation)
			{
				case EOrientation.CornerUpRight: return (heightBg, ConsoleColor.Cyan, '╚');
				case EOrientation.CornerDownRight: return (heightBg, ConsoleColor.Cyan, '╔');
				case EOrientation.CornerDownLeft: return (heightBg, ConsoleColor.Cyan, '╗');
				case EOrientation.CornerUpLeft: return (heightBg, ConsoleColor.Cyan, '╝');
			}
			break;
		case ETileType.Cliff:
			switch (cell.Orientation)
			{
				case EOrientation.CornerUpRight: return (heightBg, ConsoleColor.Black, '╚');
				case EOrientation.CornerDownRight: return (heightBg, ConsoleColor.Black, '╔');
				case EOrientation.CornerDownLeft: return (heightBg, ConsoleColor.Black, '╗');
				case EOrientation.CornerUpLeft: return (heightBg, ConsoleColor.Black, '╝');

				case EOrientation.TransitionUp: return (heightBg, ConsoleColor.Black, '╩');
				case EOrientation.TransitionRight: return (heightBg, ConsoleColor.Black, '╠');
				case EOrientation.TransitionDown: return (heightBg, ConsoleColor.Black, '╦');
				case EOrientation.TransitionLeft: return (heightBg, ConsoleColor.Black, '╣');

				case EOrientation.InnerCornerUpRight: return (heightBg, ConsoleColor.Black, '└');
				case EOrientation.InnerCornerDownRight: return (heightBg, ConsoleColor.Black, '┌');
				case EOrientation.InnerCornerDownLeft: return (heightBg, ConsoleColor.Black, '┐');
				case EOrientation.InnerCornerUpLeft: return (heightBg, ConsoleColor.Black, '┘');
			}
			break;
		case ETileType.Stairs:
			switch (cell.Orientation)
			{
				case EOrientation.TransitionUp: return (heightBg, ConsoleColor.Gray, '╩');
				case EOrientation.TransitionRight: return (heightBg, ConsoleColor.Gray, '╠');
				case EOrientation.TransitionDown: return (heightBg, ConsoleColor.Gray, '╦');
				case EOrientation.TransitionLeft: return (heightBg, ConsoleColor.Gray, '╣');
			}
			break;
	}
	return (ConsoleColor.Black, ConsoleColor.Red, '?');
}

Console.SetCursorPosition(0, Console.WindowHeight);