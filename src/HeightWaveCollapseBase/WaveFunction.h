#pragma once

#include <memory>
#include "WaveList.h"

namespace HeightWaveCollapseBase
{
	class WaveFunction
	{
	public:
		WaveFunction(int possibilities);
		bool SetPossibilities(int index, WaveList* left, WaveList* up, WaveList* right, WaveList* down);
	private:
		int _possibilities;
		std::unique_ptr<std::unique_ptr<WaveList>[]>_lefts, _ups, _rights, _downs;
	};
}