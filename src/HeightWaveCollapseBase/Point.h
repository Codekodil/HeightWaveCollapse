#pragma once

#include <unordered_set>
#include <vector>
#include <list>

namespace HeightWaveCollapseBase
{
	struct PairHash {
		template <class T1, class T2>
		std::size_t operator () (const std::pair<T1, T2>& p) const {
			auto h1 = std::hash<T1>{}(p.first);
			auto h2 = std::hash<T2>{}(p.second);
			return (h1 << 16) ^ (h1 >> 16) ^ h2;
		}
	};

	typedef std::pair<int, int> Point;
	typedef std::unordered_set<Point, PairHash> PointSet;
	typedef std::vector<Point> PointVector;
	typedef std::list<Point> PointList;
}