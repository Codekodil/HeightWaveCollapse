#pragma once

#include <unordered_set>
#include <set>
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

	struct PointCompair {
		static unsigned __int64 shift(__int32 f, __int32 s)
		{
			unsigned __int64 shifted = 0;

			for (auto i = 0; i < 32; ++i)
			{
				shifted = shifted << 1;
				shifted |= f & 0x00000001;
				shifted = shifted << 1;
				shifted |= s & 0x00000001;

				f = f >> 1;
				s = s >> 1;
			}
			return shifted;
		}
		bool operator() (std::pair<__int32, __int32> a, std::pair<__int32, __int32> b) const {
			return shift(a.first, a.second) < shift(b.first, b.second);
		}
	};

	typedef std::pair<__int32, __int32> Point;
	typedef std::unordered_set<Point, PairHash> PointSet;
	typedef std::set<Point, PointCompair> OrderedPointSet;
	typedef std::vector<Point> PointVector;
	typedef std::list<Point> PointList;
}