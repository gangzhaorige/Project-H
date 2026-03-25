using System.Collections.Generic;
using ProjectH.Models;

namespace ProjectH.Rules
{
    public static class CardRuleManager
    {
        private static readonly Dictionary<int, ICardRule> Rules = new Dictionary<int, ICardRule>();
        private static readonly ICardRule DefaultRule = new DefaultCardRule();

        static CardRuleManager()
        {
            Rules.Add((int)CardData.NormalType.ATTACK, new AttackCardRule());
            Rules.Add((int)CardData.NormalType.DODGE, new DodgeCardRule());
            Rules.Add((int)CardData.SpecialType.PRISON, new PrisonCardRule());
            Rules.Add((int)CardData.SpecialType.STEAL, new StealCardRule());
            Rules.Add((int)CardData.SpecialType.DISMANTLE, new DismantleCardRule());
            // Add other rules as needed
        }

        public static ICardRule GetRule(int cardType)
        {
            if (Rules.TryGetValue(cardType, out var rule))
            {
                return rule;
            }
            return DefaultRule;
        }

        public static bool CanPlay(CardData card, PlayerData caster)
        {
            return GetRule(card.Type).CanPlay(card, caster);
        }

        public static int GetMaxTargets(CardData card)
        {
            return GetRule(card.Type).GetMaxTargets(card);
        }

        public static bool CanTarget(CardData card, PlayerData caster, PlayerData target)
        {
            return GetRule(card.Type).CanTarget(card, caster, target);
        }
    }
}
