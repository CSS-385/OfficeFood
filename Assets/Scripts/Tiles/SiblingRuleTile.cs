using UnityEngine;
using UnityEngine.Tilemaps;

namespace OfficeFood.Tiles
{
    [CreateAssetMenu(fileName = "GroupRuleTile", menuName = "2D/Tiles/Group Rule Tile")]
    public class GroupRuleTile : RuleTile
    {
        public string Group = "";

        public override bool RuleMatch(int neighbor, TileBase other)
        {
            GroupRuleTile groupRuleTile = other as GroupRuleTile;
            switch (neighbor)
            {
                case TilingRuleOutput.Neighbor.This:
                    {
                        return groupRuleTile != null && groupRuleTile.Group == Group;
                    }
                case TilingRuleOutput.Neighbor.NotThis:
                    {
                        return groupRuleTile == null || groupRuleTile.Group != Group;
                        //return !(other is SiblingRuleTile && (other as SiblingRuleTile).sibingGroup == this.sibingGroup);
                    }
            }
            return base.RuleMatch(neighbor, other);
        }
    }
}
