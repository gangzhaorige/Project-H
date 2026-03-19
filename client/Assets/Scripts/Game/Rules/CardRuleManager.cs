using System.Collections.Generic;
using ProjectH.Models;

namespace ProjectH.Rules
{
    public static class CardRuleManager
    {
        private static readonly Dictionary<string, ICardRule> Rules = new Dictionary<string, ICardRule>();
        private static readonly ICardRule DefaultRule = new DefaultCardRule();

        static CardRuleManager()
        {
            Rules.Add("ATTACK", new AttackCardRule());
            Rules.Add("DODGE", new DodgeCardRule());
            // Add other rules as needed
        }

        public static ICardRule GetRule(string cardType)
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
