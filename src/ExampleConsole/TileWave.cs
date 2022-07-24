using HeightWaveCollapse;

namespace ExampleConsole
{
	public class TileWave : HeightWave<(ETileType Tile, EOrientation Orientation), EEdge>
	{
		public void AddTile(ETileType tile, EEdge left, EEdge up, EEdge right, EEdge down) =>
			AddCell(
				(tile, EOrientation.None),
				left, up, right, down);
		public void AddCorner(ETileType tile, EEdge leftOuter, EEdge up, EEdge right, EEdge downOuter) =>
			AddCell(
				(tile, EOrientation.CornerUpRight),
				(tile, EOrientation.CornerDownRight),
				(tile, EOrientation.CornerDownLeft),
				(tile, EOrientation.CornerUpLeft),
				leftOuter, up, right, downOuter);
		public void AddTransition(ETileType tile, EEdge left, EEdge upInner, EEdge right, EEdge downOuter) =>
			AddCell(
				(tile, EOrientation.TransitionUp),
				(tile, EOrientation.TransitionRight),
				(tile, EOrientation.TransitionDown),
				(tile, EOrientation.TransitionLeft),
				left, upInner, right, downOuter);
		public void AddInnerCorner(ETileType tile, EEdge leftInner, EEdge up, EEdge right, EEdge downInner) =>
			AddCell(
				(tile, EOrientation.InnerCornerUpRight),
				(tile, EOrientation.InnerCornerDownRight),
				(tile, EOrientation.InnerCornerDownLeft),
				(tile, EOrientation.InnerCornerUpLeft),
				leftInner, up, right, downInner);
	}
}
