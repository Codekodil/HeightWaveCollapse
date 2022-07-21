namespace HeightWaveCollapse
{
	public class DelegateWaveField<TCell> : WaveField<TCell> where TCell : notnull
	{
		public delegate WaveList<TCell> CellInitializerDelegate(int x, int y);
		private readonly CellInitializerDelegate _cellInitializer;
		public DelegateWaveField(WaveFunction<TCell> waveFunction, int chunkWidth, int chunkHeight, CellInitializerDelegate cellInitializer) :
			base(waveFunction, chunkWidth, chunkHeight)
		{
			_cellInitializer = cellInitializer;
		}

		protected override WaveList<TCell> CellInitializer(int x, int y) =>
			_cellInitializer(x, y);
	}
}
