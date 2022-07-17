#pragma once

#include <memory>
#include <unordered_map>
#include "WaveChunk.h"
#include "WaveList.h"
#include "PairHash.h"

namespace HeightWaveCollapseBase
{
	class WaveField
	{
	public:
		WaveField(int chunkWidth, int chunkHeight);
		bool AddChunk(int chunkX, int chunkY, WaveList* (*initCell)(int x, int y));
	private:
		int _chunkWidth, _chunkHeight;
		std::unordered_map<std::pair<int, int>, std::unique_ptr<WaveChunk>, PairHash> _chunks;
	};
}