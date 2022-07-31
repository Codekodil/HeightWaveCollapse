#include "CollapsingOrder.h"

#include "WaveField.h"

using namespace HeightWaveCollapseBase;
using namespace std;

CollapsingOrder::CollapsingOrder(PointSet& allFields, WaveField* field)
{
	int maxSize = -1;
	for (auto& i : allFields)
	{
		auto possibilities = field->At(i.first, i.second);
		int index = possibilities ? static_cast<int>(possibilities->size() - 2) : -1;
		maxSize = max(maxSize, index);
		if (index >= 0)
			_lastSize[i] = index;
	}

	if (maxSize < 0)
	{
		_setCount = 0;
		return;
	}

	_setCount = maxSize + 1;
	_orderedSets = make_unique<OrderedPointSet[]>(_setCount);
	for (auto& i : _lastSize)
		_orderedSets[i.second].insert(i.first);
}

void CollapsingOrder::Update(Point value, int newSize)
{
	if (!_lastSize.contains(value))
		return;
	auto last = _lastSize[value];
	auto next = newSize - 2;
	if (next < last)
	{
		_orderedSets[last].erase(value);
		if (next >= 0)
		{
			_lastSize[value] = next;
			_orderedSets[next].insert(value);
		}
		else
			_lastSize.erase(value);
	}
}

bool CollapsingOrder::FindNext(Point& next)
{
	for (int i = 0; i < _setCount; ++i)
	{
		if (_orderedSets[i].size() > 0)
		{
			next = *_orderedSets[i].begin();
			_orderedSets[i].erase(next);
			return true;
		}
	}
	return false;
}