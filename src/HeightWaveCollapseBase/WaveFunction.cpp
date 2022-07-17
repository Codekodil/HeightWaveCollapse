#include "WaveFunction.h"

using namespace HeightWaveCollapseBase;
using namespace std;

WaveFunction::WaveFunction(int possibilities)
{
	_possibilities = max(possibilities, 1);
	_lefts = make_unique<unique_ptr<WaveList>[]>(_possibilities);
	_ups = make_unique<unique_ptr<WaveList>[]>(_possibilities);
	_rights = make_unique<unique_ptr<WaveList>[]>(_possibilities);
	_downs = make_unique<unique_ptr<WaveList>[]>(_possibilities);
}

bool WaveFunction::SetPossibilities(int index, WaveList* left, WaveList* up, WaveList* right, WaveList* down)
{
	if (index < 0 || index >= _possibilities)return false;

	_lefts[index] = unique_ptr<WaveList>(left);
	_ups[index] = unique_ptr<WaveList>(up);
	_rights[index] = unique_ptr<WaveList>(right);
	_downs[index] = unique_ptr<WaveList>(down);

	return true;
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