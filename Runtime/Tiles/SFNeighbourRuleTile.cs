using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/SF/Tiles/SFNeighbour Rule Tile", fileName = "Neighbor Rule Tile")]
public class SFNeighbourRuleTile : RuleTile<SFNeighbourRuleTile.Neighbor> {

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) 
    {
        switch (neighbor)
        {
            case TilingRuleOutput.Neighbor.This: return tile == this;
            case TilingRuleOutput.Neighbor.NotThis: return tile != this;
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }

        return true;
    }
}

/*
 Error running GetTileData for new TileNullReferenceException: Object reference not set to an instance of an object
UnityEngine.Object.IsNativeObjectAlive (UnityEngine.Object o) (at <c261e0baa24e41a1b3bced11f12a19c6>:0)
UnityEngine.Object.CompareBaseObjects (UnityEngine.Object lhs, UnityEngine.Object rhs) (at <c261e0baa24e41a1b3bced11f12a19c6>:0)
UnityEngine.Object.op_Equality (UnityEngine.Object x, UnityEngine.Object y) (at <c261e0baa24e41a1b3bced11f12a19c6>:0)
SFNeighbourRuleTile.RuleMatch (System.Int32 neighbor, UnityEngine.Tilemaps.TileBase tile) (at F:/Unity Packages/SF Sprite Tools/Runtime/Tiles/SFNeighbourRuleTile.cs:15)
UnityEngine.RuleTile.RuleMatches (UnityEngine.RuleTile+TilingRule rule, UnityEngine.Vector3Int position, UnityEngine.Tilemaps.ITilemap tilemap, System.Int32 angle, System.Boolean mirrorX) (at ./Library/PackageCache/com.unity.2d.tilemap.extras@05b20556f4e5/Runtime/Tiles/RuleTile/RuleTile.cs:578)
UnityEngine.RuleTile.RuleMatches (UnityEngine.RuleTile+TilingRule rule, UnityEngine.Vector3Int position, UnityEngine.Tilemaps.ITilemap tilemap, UnityEngine.Matrix4x4& transform) (at ./Library/PackageCache/com.unity.2d.tilemap.extras@05b20556f4e5/Runtime/Tiles/RuleTile/RuleTile.cs:403)
UnityEngine.RuleTile.GetTileData (UnityEngine.Vector3Int position, UnityEngine.Tilemaps.ITilemap tilemap, UnityEngine.Tilemaps.TileData& tileData) (at ./Library/PackageCache/com.unity.2d.tilemap.extras@05b20556f4e5/Runtime/Tiles/RuleTile/RuleTile.cs:213)

*/

/*
NullReferenceException: Object reference not set to an instance of an object
UnityEngine.RuleTile.RuleMatch (System.Int32 neighbor, UnityEngine.Tilemaps.TileBase other) (at ./Library/PackageCache/com.unity.2d.tilemap.extras@05b20556f4e5/Runtime/Tiles/RuleTile/RuleTile.cs:546)
SFNeighbourRuleTile.RuleMatch (System.Int32 neighbor, UnityEngine.Tilemaps.TileBase tile) (at F:/Unity Packages/SF Sprite Tools/Runtime/Tiles/SFNeighbourRuleTile.cs:18)
UnityEngine.RuleTile.RuleMatches (UnityEngine.RuleTile+TilingRule rule, UnityEngine.Vector3Int position, UnityEngine.Tilemaps.ITilemap tilemap, System.Int32 angle, System.Boolean mirrorX) (at ./Library/PackageCache/com.unity.2d.tilemap.extras@05b20556f4e5/Runtime/Tiles/RuleTile/RuleTile.cs:578)
UnityEngine.RuleTile.RuleMatches (UnityEngine.RuleTile+TilingRule rule, UnityEngine.Vector3Int position, UnityEngine.Tilemaps.ITilemap tilemap, UnityEngine.Matrix4x4& transform) (at ./Library/PackageCache/com.unity.2d.tilemap.extras@05b20556f4e5/Runtime/Tiles/RuleTile/RuleTile.cs:403)
UnityEngine.RuleTile.GetTileData (UnityEngine.Vector3Int position, UnityEngine.Tilemaps.ITilemap tilemap, UnityEngine.Tilemaps.TileData& tileData) (at ./Library/PackageCache/com.unity.2d.tilemap.extras@05b20556f4e5/Runtime/Tiles/RuleTile/RuleTile.cs:213)

*/