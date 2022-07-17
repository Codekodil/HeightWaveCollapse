#include "WaveField.h"

using namespace HeightWaveCollapseBase;

WaveField::WaveField(int chunkWidth, int chunkHeight)
{

}

extern "C"
{
	__declspec(dllexport) WaveField* __stdcall NewWaveField(int chunkWidth, int chunkHeight);
	__declspec(dllexport) void __stdcall DeleteWaveField(WaveField* field);
}

WaveField* __stdcall NewWaveField(int chunkWidth, int chunkHeight)
{
	return new WaveField(chunkWidth, chunkHeight);
}

void __stdcall DeleteWaveField(WaveField* field)
{
	delete field;
}