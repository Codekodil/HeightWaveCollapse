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

bool WaveList::Get(int index, int* id, int* height)
{
	if (index < 0 || index >= _size)return false;

	*id = _ids[index];
	*height = _heights[index];

	return true;
}

bool WaveList::Set(int index, int id, int height)
{
	if (index < 0 || index >= _size)return false;

	_ids[index] = id;
	_heights[index] = height;

	return true;
}