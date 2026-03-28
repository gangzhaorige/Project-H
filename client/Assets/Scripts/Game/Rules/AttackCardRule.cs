using UnityEngine;
using System.Linq;
using ProjectH.Models;

namespace ProjectH.Rules
{
    public class AttackCardRule : BaseCardRule
    {
        public override bool CanPlay(CardData card, PlayerData caster)
        {
            if (!base.CanPlay(card, caster)) return false;

            // Follow server-side logic: check if attack limit has been reached
            if (caster.Champion.CurNumOfAttack >= caster.Champion.MaxNumOfAttack) return false;

            // Check if there is at least one valid target in range
            return GameSession.Instance.Players.Values.Any(p => p.IsAlive && CanTarget(card, caster, p));
        }

        public override int GetMaxTargets(CardData card)
        {
            var localPlayer = GameSession.Instance.GetLocalPlayer();
            int additional = (localPlayer != null && localPlayer.Champion != null) 
                ? localPlayer.Champion.AdditionalTargetForAttack 
                : 0;
            return 1 + additional;
        }

        public override bool CanTarget(CardData card, PlayerData caster, PlayerData target)
        {
            if (caster.PlayerId == target.PlayerId) return false;

            // Range check logic from CardTargetSelector
            int distance = GetDistance(caster.PlayerId, target.PlayerId);
            int effectiveDistance = distance + target.Champion.SpecialDefense;

            return effectiveDistance <= caster.Champion.AttackRange;
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
