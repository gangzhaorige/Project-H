using ProjectH.Models;

namespace ProjectH.Rules
{
    public class DefaultCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            switch (card.Type)
            {
                case "DUEL": return 1;
                default: return 0;
            }
        }
    }
}
