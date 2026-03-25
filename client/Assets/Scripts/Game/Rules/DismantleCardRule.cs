using UnityEngine;
using ProjectH.Models;

namespace ProjectH.Rules
{
    public class DismantleCardRule : BaseCardRule
    {
        public override int GetMaxTargets(CardData card)
        {
            return 1;
        }

        public override bool CanTarget(CardData card, PlayerData caster, PlayerData target)
        {
            // Dismantle can target anyone except self
            if (caster.PlayerId == target.PlayerId) return false;

            // Optional: check if target has cards (though server validates this too)
            return true;
        }
    }
}
