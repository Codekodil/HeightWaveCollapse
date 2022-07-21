#pragma once

#include <memory>
#include <unordered_map>
#include "Point.h"

namespace HeightWaveCollapseBase
{
	class WaveField;
	class CollapsingOrder
	{
	public:
		CollapsingOrder(PointSet& allFields, WaveField* field);
		void Update(Point value, int newSize);
		bool FindNext(Point& next);
	private:
		int _setCount;
		std::unordered_map<Point, int, PairHash> _lastSize;
		std::unique_ptr<PointSet[]> _orderedSets;
	};
}