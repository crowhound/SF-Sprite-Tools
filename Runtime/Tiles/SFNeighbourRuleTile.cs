using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SF.TileModule
{
    [CreateAssetMenu(menuName = "2D/SF/Tiles/SFNeighbour Rule Tile", fileName = "Neighbor Rule Tile")]
    public class SFNeighbourRuleTile : RuleTile<SFNeighbourRuleTile.Neighbor> {

        public class Neighbor : RuleTile.TilingRuleOutput.Neighbor {
            public const int Null = 3;
            public const int NotNull = 4;
        }
        
        public override bool RuleMatch(int neighbor, TileBase tile) 
        {
            if(tile is RuleOverrideTile overrideTile)
                tile = overrideTile.m_InstanceTile;
 
            switch (neighbor) {
                case TilingRuleOutput.Neighbor.This:
                {
                    return tile == this;
                }
                case TilingRuleOutput.Neighbor.NotThis:
                {
                    return tile != this;
                } 
                
                case Neighbor.Null: return tile == null;
                case Neighbor.NotNull: return tile != null;
            }

            return true;
        }
    }
}