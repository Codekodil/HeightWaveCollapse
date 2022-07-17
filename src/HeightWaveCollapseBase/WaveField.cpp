#include "WaveField.h"

using namespace HeightWaveCollapseBase;
using namespace std;

WaveField::WaveField(int chunkWidth, int chunkHeight)
{
	_chunkWidth = max(chunkWidth, 1);
	_chunkHeight = max(chunkHeight, 1);
}

bool HeightWaveCollapseBase::WaveField::AddChunk(int chunkX, int chunkY, WaveList* (*initCell)(int x, int y))
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
				chunk->At(x, y).insert({ id,height });
			}
		}
	_chunks[key] = move(chunk);
	return true;
}

extern "C"
{
	__declspec(dllexport) WaveField* __stdcall NewWaveField(int chunkWidth, int chunkHeight);
	__declspec(dllexport) void __stdcall DeleteWaveField(WaveField* field);
	__declspec(dllexport) bool __stdcall WaveFieldAddChunk(WaveField* field, int chunkX, int chunkY, WaveList* (*initCell)(int x, int y));
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
