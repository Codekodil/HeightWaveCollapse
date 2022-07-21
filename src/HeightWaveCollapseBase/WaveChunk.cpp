#include "WaveChunk.h"

using namespace HeightWaveCollapseBase;
using namespace std;

WaveChunk::WaveChunk(int width, int height)
{
	_width = width;
	_height = height;
	_possibilities = make_unique<PointList[]>(_width * _height);
}

PointList& WaveChunk::At(int x, int y)
{
	return _possibilities[x + y * _width];
}
