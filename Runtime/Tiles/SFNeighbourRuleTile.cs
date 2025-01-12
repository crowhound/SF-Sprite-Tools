using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/SF/Tiles/SFNeighbour Rule Tile", fileName = "Neighbor Rule Tile")]
public class SFNeighbourRuleTile : RuleTile<SFNeighbourRuleTile.Neighbor> {

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
		if(tile is TileBase && tile != null)
			return true;

        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}