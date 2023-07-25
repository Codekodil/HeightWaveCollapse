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

void AddWaveListToSet(PointSet& set, WaveList* list)
{
	for (int i = 0; i < list->GetSize(); i++)
	{
		int id, height;
		list->Get(i, &id, &height);
		set.insert({ id,height });
	}
}

bool WaveFunction::SetPossibilities(int index, WaveList* left, WaveList* up, WaveList* right, WaveList* down)
{
	if (index < 0 || index >= _possibilities)return false;

	AddWaveListToSet(_sets[0][index], left);
	AddWaveListToSet(_sets[1][index], up);
	AddWaveListToSet(_sets[2][index], right);
	AddWaveListToSet(_sets[3][index], down);

	return true;
}

bool WaveFunction::DirectionContains(int direction, int index, Point value)
{
	if (index < 0 || index >= _possibilities)
		return false;
	auto& set = _sets[direction];
	return set[index].contains(value);
}