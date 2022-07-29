#include "WaveFunction.h"

using namespace HeightWaveCollapseBase;
using namespace std;

WaveFunction::WaveFunction(int possibilities)
{
	_possibilities = max(possibilities, 1);
	_sets[0] = make_unique<PointSet[]>(_possibilities);
	_sets[1] = make_unique<PointSet[]>(_possibilities);
	_sets[2] = make_unique<PointSet[]>(_possibilities);
	_sets[3] = make_unique<PointSet[]>(_possibilities);
}

void AddWaveListToSet(PointSet& set, unique_ptr<WaveList>& list)
{
	for (int i = 0; i < list->GetSize(); i++)
	{
		int id, height;
		list->Get(i, id, height);
		set.insert({ id,height });
	}
}
bool WaveFunction::SetPossibilities(int index, WaveList* left, WaveList* up, WaveList* right, WaveList* down)
{
	if (index < 0 || index >= _possibilities)return false;

	auto l = unique_ptr<WaveList>(left);
	auto u = unique_ptr<WaveList>(up);
	auto r = unique_ptr<WaveList>(right);
	auto d = unique_ptr<WaveList>(down);

	AddWaveListToSet(_sets[0][index], l);
	AddWaveListToSet(_sets[1][index], u);
	AddWaveListToSet(_sets[2][index], r);
	AddWaveListToSet(_sets[3][index], d);

	return true;
}


bool WaveFunction::DirectionContains(int direction, int index, Point value)
{
	if (index < 0 || index >= _possibilities)
		return false;
	auto& set = _sets[direction];
	return set[index].contains(value);
}

extern "C"
{
	__declspec(dllexport) WaveFunction* __stdcall NewWaveFunction(int possibilities);
	__declspec(dllexport) void __stdcall DeleteWaveFunction(WaveFunction* func);
	__declspec(dllexport) bool __stdcall WaveFunctionSetPossibilities(WaveFunction* func, int index, WaveList* left, WaveList* up, WaveList* right, WaveList* down);
}

WaveFunction* __stdcall NewWaveFunction(int possibilities)
{
	return new WaveFunction(possibilities);
}

void __stdcall DeleteWaveFunction(WaveFunction* func)
{
	delete func;
}

bool __stdcall WaveFunctionSetPossibilities(WaveFunction* func, int index, WaveList* left, WaveList* up, WaveList* right, WaveList* down)
{
	return func->SetPossibilities(index, left, up, right, down);
}