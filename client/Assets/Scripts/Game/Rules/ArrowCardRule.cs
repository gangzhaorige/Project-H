using ProjectH.Models;
using System.Linq;

namespace ProjectH.Rules
{
    public class ArrowCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 0; // Affects all other players
        }

        public override bool CanPlay(CardData card, PlayerData caster)
        {
            if (!base.CanPlay(card, caster)) return false;

            // Must have at least 1 alive champion beside caster
            int aliveOthers = GameSession.Instance.Players.Values
                .Count(p => p.PlayerId != caster.PlayerId && p.IsAlive);

            return aliveOthers >= 1;
        }
    }
}
