using UnityEngine;
using ProjectH.Models;

namespace ProjectH.Rules
{
    public class StealCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 1;
        }

        public override bool CanTarget(CardData card, PlayerData caster, PlayerData target)
        {
            if (caster.PlayerId == target.PlayerId) return false;

            // Target must have cards in hand
            // Wait, we don't always know target hand size precisely in PlayerData unless we track it
            // Assuming target.Hand.Count or another metric is updated, or we just rely on range check
            // and let server reject if hand is empty.

            int distance = GetDistance(caster.PlayerId, target.PlayerId);
            int effectiveDistance = distance + target.Champion.SpecialDefense;

            // StealCard has an inherent +1 range
            return effectiveDistance <= caster.Champion.SpecialRange + 1;
        }

        private int GetDistance(int p1Id, int p2Id)
        {
            var order = GameSession.Instance.PlayerOrder;
            int idx1 = order.IndexOf(p1Id);
            int idx2 = order.IndexOf(p2Id);
            if (idx1 == -1 || idx2 == -1) return 999;
            int n = order.Count;
            int diff = Mathf.Abs(idx1 - idx2);
            return Mathf.Min(diff, n - diff);
        }
    }
}
