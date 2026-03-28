using ProjectH.Models;
using System.Linq;

namespace ProjectH.Rules
{
    public class HealAllCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 0; // Affects everyone alive
        }

        public override bool CanPlay(CardData card, PlayerData caster)
        {
            if (!base.CanPlay(card, caster)) return false;

            // Everyone that is alive, have at least one person hp is not full
            return GameSession.Instance.Players.Values
                .Any(p => p.IsAlive && p.Champion.CurHP < p.Champion.MaxHP);
        }
    }
}
