#pragma once

#include <unordered_set>
#include <memory>
#include "Point.h"

namespace HeightWaveCollapseBase
{
	class WaveChunk
	{
	public:
		WaveChunk(int width, int height);
		PointList& At(int x, int y);
	private:
		int _width, _height;
		std::unique_ptr<PointList[]> _possibilities;
	};
}