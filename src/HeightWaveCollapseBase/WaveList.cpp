#include "WaveList.h"

using namespace HeightWaveCollapseBase;
using namespace std;

WaveList::WaveList(int size)
{
	_size = max(size, 0);
	if (_size == 0)
		return;
	_ids = make_unique<int[]>(_size);
	_heights = make_unique<int[]>(_size);
}

int WaveList::GetSize() { return _size; }

bool WaveList::Get(int index, int& id, int& height)
{
	if (index < 0 || index >= _size)return false;

	id = _ids[index];
	height = _heights[index];

	return true;
}

bool WaveList::Set(int index, int id, int height)
{
	if (index < 0 || index >= _size)return false;

	_ids[index] = id;
	_heights[index] = height;

	return true;
}

extern "C"
{
	__declspec(dllexport) WaveList* __stdcall NewWaveList(int size);
	__declspec(dllexport) void __stdcall DeleteWaveList(WaveList* list);
	__declspec(dllexport) int __stdcall WaveListGetSize(WaveList* list);
	__declspec(dllexport) bool __stdcall WaveListGet(WaveList* list, int index, int& id, int& height);
	__declspec(dllexport) bool __stdcall WaveListSet(WaveList* list, int index, int id, int height);
}

WaveList* __stdcall NewWaveList(int size)
{
	return new WaveList(size);
}

void __stdcall DeleteWaveList(WaveList* list)
{
	delete list;
}

int __stdcall WaveListGetSize(WaveList* list)
{
	return list->GetSize();
}

bool __stdcall WaveListGet(WaveList* list, int index, int& id, int& height)
{
	return list->Get(index, id, height);
}

bool __stdcall WaveListSet(WaveList* list, int index, int id, int height)
{
	return list->Set(index, id, height);
}