using ProjectH.Models;
using System.Linq;

namespace ProjectH.Rules
{
    public class PrisonCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 1;
        }

        public override bool CanTarget(CardData card, PlayerData caster, PlayerData target)
        {
            if (caster.PlayerId == target.PlayerId) return false;

            // Cannot imprison someone who already has prison.
            // Note: Since ActiveEffects is not yet in PlayerData on client, 
            // we might need to add it or skip this check on client for now.
            // However, to be consistent with the server, let's assume we'll add it.
            
            // For now, let's just return true and we can add the effect check if we update PlayerData.
            return true;
        }
    }
}
