#pragma once

#include <unordered_set>
#include <memory>
#include "PairHash.h"

namespace HeightWaveCollapseBase
{
	class WaveChunk
	{
	public:
		WaveChunk(int width, int height);
		std::unordered_set<std::pair<int, int>, PairHash>& At(int x, int y);
	private:
		int _width, _height;
		std::unique_ptr<std::unordered_set<std::pair<int, int>, PairHash>[]> _possibilities;
	};
}