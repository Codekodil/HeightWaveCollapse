#pragma once

#include <memory>
#include "WaveList.h"
#include "Point.h"

namespace HeightWaveCollapseBase
{
	class WaveFunction
	{
	public:
		WaveFunction(int possibilities);
		bool SetPossibilities(int index, WaveList* left, WaveList* up, WaveList* right, WaveList* down);
		bool DirectionContains(int direction, int index, Point value);
	private:
		int _possibilities;
		std::unique_ptr<PointSet[]>_sets[4];
	};
}