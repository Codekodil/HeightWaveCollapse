#pragma once

#include "CodeGen.h"

namespace HeightWaveCollapseBase
{
	class Wrapper_Generate Area
	{
	public:
		int X, Y, Width, Height;

		Area(int x, int y, int width, int height);

		bool Inside(int x, int y);

		int GetX() { return X; }
		int GetY() { return Y; }
		int GetWidth() { return Width; }
		int GetHeight() { return Height; }

	private:
	};
}