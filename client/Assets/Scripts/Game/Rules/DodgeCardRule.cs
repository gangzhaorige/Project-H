using ProjectH.Models;

namespace ProjectH.Rules
{
    public class DodgeCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 0;
        }
    }
}
