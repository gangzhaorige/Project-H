using System.Collections.Generic;
using ProjectH.Models;

namespace ProjectH.Rules
{
    public interface ICardRule
    {
        bool CanPlay(CardData card, PlayerData caster);
        int GetMaxTargets(CardData card);
        bool CanTarget(CardData card, PlayerData caster, PlayerData target);
    }

    public abstract class BaseCardRule : ICardRule
    {
        public virtual bool CanPlay(CardData card, PlayerData caster)
        {
            return caster != null && caster.Champion != null;
        }

        public abstract int GetMaxTargets(CardData card);

        public virtual bool CanTarget(CardData card, PlayerData caster, PlayerData target)
        {
            return true;
        }
    }
}
