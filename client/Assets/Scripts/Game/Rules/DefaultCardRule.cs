using ProjectH.Models;

namespace ProjectH.Rules
{
    public class DefaultCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 0;
        }
    }
}
