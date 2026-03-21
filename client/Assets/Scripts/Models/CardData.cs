namespace ProjectH.Models
{
    [System.Serializable]
    public class CardData
    {
        public int Id;
        public int Suit; // 0=Spade, 1=Heart, 2=Club, 3=Diamond
        public int Value; // 1-13
        public string Type; // "Attack", "Dodge", "Heal", etc.
    }
}
