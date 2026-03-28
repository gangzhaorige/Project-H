using ProjectH.Models;

namespace ProjectH.Rules
{
    public class HealCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 0; // Self-targeted
        }

        public override bool CanPlay(CardData card, PlayerData caster)
        {
            if (!base.CanPlay(card, caster)) return false;

            // Caster's HP is not full
            return caster.Champion.CurHP < caster.Champion.MaxHP;
        }
    }
}
