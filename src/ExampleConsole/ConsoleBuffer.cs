namespace ExampleConsole
{
	public class ConsoleBuffer
	{
		public struct Color
		{
			public Color(int r, int g, int b)
			{
				R = r;
				G = g;
				B = b;
			}
			public int R, G, B;
		}

		public int Width { get; }
		public int Height { get; }
		private readonly char[,] _symbols;
		private readonly Color[,] _fg;
		private readonly Color[,] _bg;
		private readonly HashSet<(int X, int Y)> _needUpdate = new HashSet<(int X, int Y)>();
		public ConsoleBuffer(int width, int height)
		{
			Width = width;
			Height = height;
			_symbols = new char[Width, Height];
			_fg = new Color[Width, Height];
			_bg = new Color[Width, Height];
			for (var y = 0; y < Height; y++)
				for (var x = 0; x < Width; x++)
					_symbols[x, y] = ' ';
		}

		public (char Symbol, Color Fg, Color Bg) this[int x, int y]
		{
			get => (_symbols[x, y], _fg[x, y], _bg[x, y]);
			set
			{
				if (_symbols[x, y] == value.Symbol &&
					_fg[x, y].Equals(value.Fg) &&
					_bg[x, y].Equals(value.Bg))
					return;
				_needUpdate.Add((x, y));
				_symbols[x, y] = value.Symbol;
				_fg[x, y] = value.Fg;
				_bg[x, y] = value.Bg;
			}
		}

		public void Draw()
		{
			var instructions = "";
			Color? lastFg = null, lastBg = null;
			var lastCellUpdated = false;
			for (var y = Height - 1; y >= 0; y--)
			{
				lastCellUpdated = false;
				for (var x = 0; x < Width; x++)
				{
					if (!_needUpdate.Contains((x, y)))
					{
						lastCellUpdated = false;
						continue;
					}
					if (!lastCellUpdated)
					{
						instructions += $"\u001b[{Height - y};{x + 1}H";
					}
					lastCellUpdated = true;

					var fg = _fg[x, y];
					var bg = _bg[x, y];
					if (!(lastFg?.Equals(fg) ?? false))
					{
						lastFg = fg;
						instructions += $"\u001b[38;2;{fg.R};{fg.G};{fg.B}m";
					}
					if (!(lastBg?.Equals(bg) ?? false))
					{
						lastBg = bg;
						instructions += $"\u001b[48;2;{bg.R};{bg.G};{bg.B}m";
					}
					instructions += _symbols[x, y];
				}
			}
			Console.Write(instructions);
			_needUpdate.Clear();
		}
	}
}
