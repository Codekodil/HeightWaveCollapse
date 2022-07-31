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

Task? displayRunning = null;
var forceDisplayCounter = 0;
var buffer = new ConsoleBuffer(Console.WindowWidth, Console.WindowHeight); ;
newField.AfterCollapseCell += Display;

Display();
newField.Collapse();
while (displayRunning != null)
	Thread.Sleep(100);
Display();
displayRunning?.GetAwaiter().GetResult();
Thread.Sleep(1000);

void Display()
{
	if (forceDisplayCounter++ > 200)
	{
		while (displayRunning != null)
			Thread.Sleep(10);
	}
	if (displayRunning != null) return;
	var iterations = forceDisplayCounter;
	forceDisplayCounter = 0;

	var cells = newField.GetCells().Select(c => (c, newField.PossibilitiesAt(c.X, c.Y))).ToList();

	displayRunning = Task.Run(() =>
	{
		try
		{
			foreach (var (c, p) in cells)
			{
				(ConsoleBuffer.Color bg, ConsoleBuffer.Color fg, char c) data;

				if (p == null || p.Size == 0)
					data = (new ConsoleBuffer.Color(0, 0, 0), new ConsoleBuffer.Color(255, 0, 0), '0');
				else if (p.Size == 1)
					data = DrawingData(p[0].Value, p[0].Height);
				else
					data = (new ConsoleBuffer.Color(0, 0, 0), p.Size <= 9 ? new ConsoleBuffer.Color(255, 255, 255) : new ConsoleBuffer.Color(64, 64, 64), Math.Min(p.Size, 9).ToString()[0]);

				buffer[c.X, c.Y] = (data.c, data.fg, data.bg);
			}
			buffer.Draw();
		}
		finally
		{
			displayRunning = null;
		}
	});
}

(ConsoleBuffer.Color bg, ConsoleBuffer.Color fg, char c) DrawingData((ETileType Tile, EOrientation Orientation) cell, int height)
{
	var heightBg = height == 0 ?
		new ConsoleBuffer.Color(0, 100, 0) :
		height == 1 ?
		new ConsoleBuffer.Color(0, 130, 0) :
		height == 2 ?
		new ConsoleBuffer.Color(0, 160, 0) :
		new ConsoleBuffer.Color(0, 190, 0);
	ConsoleBuffer.Color fg;
	switch (cell.Tile)
	{
		case ETileType.Flat: return (heightBg, heightBg, ' ');
		case ETileType.Shrub: return (heightBg, new ConsoleBuffer.Color(40, 120, 40), '#');
		case ETileType.Tree:
			fg = new ConsoleBuffer.Color(190, 90, 60);
			switch (cell.Orientation)
			{
				case EOrientation.CornerUpRight: return (heightBg, fg, '╚');
				case EOrientation.CornerDownRight: return (heightBg, fg, '╔');
				case EOrientation.CornerDownLeft: return (heightBg, fg, '╗');
				case EOrientation.CornerUpLeft: return (heightBg, fg, '╝');
			}
			break;
		case ETileType.Cliff:
			fg = new ConsoleBuffer.Color(0, 0, 0);
			switch (cell.Orientation)
			{
				case EOrientation.CornerUpRight: return (heightBg, fg, '╚');
				case EOrientation.CornerDownRight: return (heightBg, fg, '╔');
				case EOrientation.CornerDownLeft: return (heightBg, fg, '╗');
				case EOrientation.CornerUpLeft: return (heightBg, fg, '╝');

				case EOrientation.TransitionUp: return (heightBg, fg, '╩');
				case EOrientation.TransitionRight: return (heightBg, fg, '╠');
				case EOrientation.TransitionDown: return (heightBg, fg, '╦');
				case EOrientation.TransitionLeft: return (heightBg, fg, '╣');

				case EOrientation.InnerCornerUpRight: return (heightBg, fg, '└');
				case EOrientation.InnerCornerDownRight: return (heightBg, fg, '┌');
				case EOrientation.InnerCornerDownLeft: return (heightBg, fg, '┐');
				case EOrientation.InnerCornerUpLeft: return (heightBg, fg, '┘');
			}
			break;
		case ETileType.Stairs:
			fg = new ConsoleBuffer.Color(160, 160, 160);
			switch (cell.Orientation)
			{
				case EOrientation.TransitionUp: return (heightBg, fg, '╩');
				case EOrientation.TransitionRight: return (heightBg, fg, '╠');
				case EOrientation.TransitionDown: return (heightBg, fg, '╦');
				case EOrientation.TransitionLeft: return (heightBg, fg, '╣');
			}
			break;
	}
	return (new ConsoleBuffer.Color(0, 0, 0), new ConsoleBuffer.Color(255, 0, 0), '?');
}

Console.SetCursorPosition(0, Console.WindowHeight);