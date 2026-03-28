using System.Linq;
using ProjectH.Models;

namespace ProjectH.Rules
{
    public class DuelCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 1;
        }

        public override bool CanPlay(CardData card, PlayerData caster)
        {
            if (!base.CanPlay(card, caster)) return false;

            // Must have at least one valid target
            return GameSession.Instance.Players.Values.Any(p => CanTarget(card, caster, p));
        }

        public override bool CanTarget(CardData card, PlayerData caster, PlayerData target)
        {
            // Cannot duel self
            if (caster.PlayerId == target.PlayerId) return false;

            // Target must be alive
            return target.IsAlive;
        }
    }
}
