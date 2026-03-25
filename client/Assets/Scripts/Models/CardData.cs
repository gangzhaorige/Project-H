namespace ProjectH.Models
{
    [System.Serializable]
    public class CardData
    {
        public enum NormalType
        {
            ATTACK = 1,
            DODGE = 2,
            HEAL = 3
        }

        public enum SpecialType
        {
            ARROW = 101,
            DUEL = 102,
            DRAW = 103,
            STEAL = 104,
            DISMANTLE = 105,
            HEAL_ALL = 106,
            FIRE = 107,
            NEGATE = 108,
            PRISON = 109,
            DRAW_SKIP = 110
        }

        public int Id;
        public int Suit; // 0=Spade, 1=Heart, 2=Club, 3=Diamond
        public int Value; // 1-13
        public int Type; // EnumId
    }
}
