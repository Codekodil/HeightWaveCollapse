using System.Runtime.InteropServices;

namespace HeightWaveCollapse
{
	internal static class NativeWaveList
	{
		[DllImport("HeightWaveCollapseBase")]
		internal static extern IntPtr NewWaveList(int size);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern void DeleteWaveList(IntPtr list);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern bool WaveListGet(IntPtr list, int index, out int id, out int height);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern bool WaveListSet(IntPtr list, int index, int id, int height);
	}
}