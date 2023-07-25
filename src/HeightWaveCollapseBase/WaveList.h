#pragma once

#include "CodeGen.h"
#include <memory>
#include <span>

namespace HeightWaveCollapseBase
{
	class Wrapper_Lookup WaveList
	{
	public:
		WaveList(int size);
		int GetSize();
		bool Get(int index, int* id, int* height);
		bool Set(int index, int id, int height);
	private:
		int _size;
		std::unique_ptr<int[]> _ids;
		std::unique_ptr<int[]> _heights;
	};
}