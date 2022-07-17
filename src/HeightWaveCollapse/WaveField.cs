using System.Runtime.InteropServices;

namespace HeightWaveCollapse
{
	public class WaveField<TCell> where TCell : notnull
	{
		public int ChunkHeight { get; }
		public int ChunkWidth { get; }
		public WaveFunction<TCell> WaveFunction { get; }
		private readonly IntPtr _nativeField;
		public WaveField(WaveFunction<TCell> waveFunction, int chunkWidth, int chunkHeight)
		{
			if (chunkWidth <= 0) throw new ArgumentOutOfRangeException(nameof(chunkWidth));
			if (chunkHeight <= 0) throw new ArgumentOutOfRangeException(nameof(chunkHeight));
			WaveFunction = waveFunction;
			ChunkWidth = chunkWidth;
			ChunkHeight = chunkHeight;
			_nativeField = NativeWaveField.NewWaveField(ChunkWidth, ChunkHeight);
		}

		~WaveField()
		{
			NativeWaveField.DeleteWaveField(_nativeField);
		}
	}

	internal static class NativeWaveField
	{
		[DllImport("HeightWaveCollapseBase")]
		internal static extern IntPtr NewWaveField(int chunkWidth, int chunkHeight);

		[DllImport("HeightWaveCollapseBase")]
		internal static extern void DeleteWaveField(IntPtr field);
	}
}
