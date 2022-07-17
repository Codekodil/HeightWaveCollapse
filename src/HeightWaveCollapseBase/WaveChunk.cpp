#include "WaveChunk.h"

using namespace HeightWaveCollapseBase;
using namespace std;

WaveChunk::WaveChunk(int width, int height)
{
	_width = width;
	_height = height;
	_possibilities = make_unique<std::unordered_set<std::pair<int, int>, PairHash>[]>(_width * _height);
}

unordered_set<pair<int, int>, PairHash>& WaveChunk::At(int x, int y)
{
	return _possibilities[x + y * _width];
}
