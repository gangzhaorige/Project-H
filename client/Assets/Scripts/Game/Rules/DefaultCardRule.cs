using ProjectH.Models;

namespace ProjectH.Rules
{
    public class DefaultCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            if (card.Type == (int)CardData.SpecialType.DUEL) return 1;
            return 0;
        }
    }
}
