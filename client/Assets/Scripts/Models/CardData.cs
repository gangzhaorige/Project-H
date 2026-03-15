namespace ProjectH.Models
{
    [System.Serializable]
    public class CardData
    {
        public int Id;
        public int Suit; // 0=Spade, 1=Diamond, 2=Heart, 3=Club
        public int Value; // 1-13
        public string Type; // "Attack", "Dodge", "Heal", etc.
    }
}
