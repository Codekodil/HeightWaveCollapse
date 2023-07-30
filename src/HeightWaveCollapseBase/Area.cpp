#include "Area.h"

using namespace HeightWaveCollapseBase;

Area::Area(int x, int y, int width, int height)
{
	X = x;
	Y = y;
	Width = width;
	Height = height;
}

bool Area::Inside(int x, int y)
{
	return x >= X && y >= Y && x < X + Width && y < Y + Height;
}
