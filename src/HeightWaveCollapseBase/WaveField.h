#pragma once

#include "CodeGen.h"
#include "WaveChunk.h"
#include "WaveList.h"
#include "Point.h"
#include "CollapsingOrder.h"
#include <memory>
#include <unordered_map>

namespace HeightWaveCollapseBase
{
	class WaveFunction;

	class Wrapper_Generate CellInitializer
	{
	public:
		CellInitializer() {}
		void* (__stdcall* InitCell)(int x, int y) = nullptr;
		WaveList* Generate(int x, int y);
	};

	class Wrapper_Generate CellCollapse
	{
	public:
		CellCollapse() {}
		void(__stdcall* CollapseCell)(int x, int y, int* id, int* height) = nullptr;
		void Invoke(int x, int y, int& id, int& height);
	};

	class Wrapper_Lookup WaveField
	{
	public:
		WaveField(int chunkWidth, int chunkHeight);
		bool AddChunk(int chunkX, int chunkY, CellInitializer* initializer);
		std::list<std::pair<int, int>>* At(int x, int y);
		WaveList* ListAt(int x, int y);
		void Collapse(WaveFunction* func, CellCollapse* collapse);
	private:
		int _chunkWidth, _chunkHeight;
		std::unordered_map<Point, std::unique_ptr<WaveChunk>, PairHash> _chunks;
		void ReducePossibilities(WaveFunction* func, PointSet origins, CollapsingOrder& order);
	};
}