#pragma once

#include <memory>
#include <unordered_map>
#include "WaveChunk.h"
#include "WaveList.h"
#include "Point.h"
#include "CollapsingOrder.h"

namespace HeightWaveCollapseBase
{
	class WaveFunction;
	class WaveField
	{
	public:
		WaveField(int chunkWidth, int chunkHeight);
		bool AddChunk(int chunkX, int chunkY, WaveList* (*initCell)(int x, int y));
		std::list<std::pair<int, int>>* At(int x, int y);
		WaveList* ListAt(int x, int y);
		void Collapse(WaveFunction* func);
	private:
		int _chunkWidth, _chunkHeight;
		std::unordered_map<Point, std::unique_ptr<WaveChunk>, PairHash> _chunks;
		void ReducePossibilities(WaveFunction* func, PointSet origins, CollapsingOrder& order);
	};
}