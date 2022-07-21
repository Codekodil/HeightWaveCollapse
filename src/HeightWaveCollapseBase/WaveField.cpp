#include "WaveField.h"

#include "WaveFunction.h"

using namespace HeightWaveCollapseBase;
using namespace std;

WaveField::WaveField(int chunkWidth, int chunkHeight)
{
	_chunkWidth = max(chunkWidth, 1);
	_chunkHeight = max(chunkHeight, 1);
}

bool WaveField::AddChunk(int chunkX, int chunkY, WaveList* (*initCell)(int x, int y))
{
	pair<int, int> key = { chunkX, chunkY };
	if (_chunks.find(key) != _chunks.end())
		return false;
	auto chunk = make_unique<WaveChunk>(_chunkWidth, _chunkHeight);
	for (int x = 0; x < _chunkWidth; x++)
		for (int y = 0; y < _chunkHeight; y++)
		{
			auto list = unique_ptr<WaveList>(initCell(_chunkWidth * chunkX + x, _chunkHeight * chunkY + y));
			int id, height;
			for (int i = 0; i < list->GetSize(); i++)
			{
				list->Get(i, id, height);
				chunk->At(x, y).push_back({ id,height });
			}
		}
	_chunks[key] = move(chunk);
	return true;
}

PointList* WaveField::At(int x, int y)
{
	int chunkX = x / _chunkWidth;
	int innerX = x % _chunkWidth;
	if (innerX < 0) { innerX += _chunkWidth; --chunkX; }

	int chunkY = y / _chunkHeight;
	int innerY = y % _chunkHeight;
	if (innerY < 0) { innerY += _chunkHeight; --chunkY; }

	auto chunk = _chunks.find({ chunkX, chunkY });
	if (chunk == _chunks.end())
		return nullptr;

	return &chunk->second->At(innerX, innerY);
}

WaveList* WaveField::ListAt(int x, int y)
{
	auto set = At(x, y);
	if (!set)return nullptr;

	auto list = new WaveList(static_cast<int>(set->size()));
	int i = 0;
	for (auto& cell : *set)
		list->Set(i++, cell.first, cell.second);
	return list;
}

void WaveField::Collapse(WaveFunction* func, void (*collapseField)(int x, int y, int& id, int& height))
{

	PointSet allFields;
	for (auto& chunk : _chunks)
		for (int x = 0; x < _chunkWidth; ++x)
			for (int y = 0; y < _chunkHeight; ++y)
				allFields.insert({ x + chunk.first.first * _chunkWidth,y + chunk.first.second * _chunkHeight });

	CollapsingOrder order(allFields, this);

	ReducePossibilities(func, allFields, order);

	Point next;
	while (order.FindNext(next))
	{
		auto possibilities = At(next.first, next.second);
		if (!possibilities || possibilities->size() <= 1)
			continue;

		int id, height;
		collapseField(next.first, next.second, id, height);
		possibilities->erase(possibilities->begin(), possibilities->end());
		if (id >= 0)
			possibilities->push_back({ id, height });
		PointSet sides;
		sides.insert({ next.first - 1, next.second });
		sides.insert({ next.first, next.second + 1 });
		sides.insert({ next.first + 1, next.second });
		sides.insert({ next.first, next.second - 1 });

		ReducePossibilities(func, sides, order);
	}
}

bool ShouldRemove(WaveFunction* func, int direction, Point toTest, PointList* neighbor)
{
	if (!neighbor || neighbor->size() == 0)return false;
	for (auto& i : *neighbor)
	{
		auto deltaHeight = toTest.second - i.second;
		if (func->DirectionContains(direction, i.first, { toTest.first, deltaHeight }))
			return false;
	}
	return true;
}
void WaveField::ReducePossibilities(WaveFunction* func, PointSet origins, CollapsingOrder& order)
{
	while (origins.size() > 0)
	{
		auto next = *origins.begin();
		origins.erase(next);
		auto current = At(next.first, next.second);
		if (!current)continue;
		for (auto i = current->begin(); i != current->end();)
		{
			auto toTest = (*i);
			auto left = At(next.first - 1, next.second);
			auto up = At(next.first, next.second + 1);
			auto right = At(next.first + 1, next.second);
			auto down = At(next.first, next.second - 1);

			if (ShouldRemove(func, 2, toTest, left) ||
				ShouldRemove(func, 3, toTest, up) ||
				ShouldRemove(func, 0, toTest, right) ||
				ShouldRemove(func, 1, toTest, down))
			{
				origins.insert({ next.first - 1, next.second });
				origins.insert({ next.first , next.second + 1 });
				origins.insert({ next.first + 1, next.second });
				origins.insert({ next.first , next.second - 1 });

				if (i == current->begin())
				{
					current->erase(i);
					i = current->begin();
				}
				else
				{
					auto remove = i;
					--i;
					current->erase(remove);
					++i;
				}
			}
			else
				++i;
		}
		order.Update(next, current->size());
	}
}

extern "C"
{
	__declspec(dllexport) WaveField* __stdcall NewWaveField(int chunkWidth, int chunkHeight);
	__declspec(dllexport) void __stdcall DeleteWaveField(WaveField* field);
	__declspec(dllexport) bool __stdcall WaveFieldAddChunk(WaveField* field, int chunkX, int chunkY, WaveList* (*initCell)(int x, int y));
	__declspec(dllexport) WaveList* __stdcall WaveFieldListAt(WaveField* field, int x, int y);
	__declspec(dllexport) void __stdcall WaveFieldCollapse(WaveField* field, WaveFunction* func, void (*collapseField)(int x, int y, int& id, int& height));
}

WaveField* __stdcall NewWaveField(int chunkWidth, int chunkHeight)
{
	return new WaveField(chunkWidth, chunkHeight);
}

void __stdcall DeleteWaveField(WaveField* field)
{
	delete field;
}

bool __stdcall WaveFieldAddChunk(WaveField* field, int chunkX, int chunkY, WaveList* (*initCell)(int x, int y))
{
	return field->AddChunk(chunkX, chunkY, initCell);
}

WaveList* __stdcall WaveFieldListAt(WaveField* field, int x, int y)
{
	return field->ListAt(x, y);
}

void __stdcall WaveFieldCollapse(WaveField* field, WaveFunction* func, void (*collapseField)(int x, int y, int& id, int& height))
{
	field->Collapse(func, collapseField);
}