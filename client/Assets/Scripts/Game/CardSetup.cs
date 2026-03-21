using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSetup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image cardMainImage;
    [SerializeField] private Image suitTopImage;
    [SerializeField] private Image suitBotImage;
    [SerializeField] private TextMeshProUGUI topValueText;
    [SerializeField] private TextMeshProUGUI botValueText;
    [SerializeField] private TextMeshProUGUI cardTypeText;

    public void Init(string cardType, int suit, int value)
    {
        // 1. Set Card Type Text
        if (cardTypeText != null) cardTypeText.text = cardType;

        // 2. Set Value Text (1=A, 11=J, 12=Q, 13=K)
        string valueStr = GetValueString(value);
        if (topValueText != null) topValueText.text = valueStr;
        if (botValueText != null) botValueText.text = valueStr;

        // 3. Set Value/Suit Color (Hearts and Diamonds are red)
        // 0=Spade (Dark), 1=Heart (Red), 2=Club (Dark), 3=Diamond (Red)
        Color cardColor = (suit == 1 || suit == 3) ? Color.red : Color.black;
        if (topValueText != null) topValueText.color = cardColor;
        if (botValueText != null) botValueText.color = cardColor;

        // 4. Load Suit Images
        Sprite suitSprite = Resources.Load<Sprite>($"Images/cards/CardSuits/{suit}");
        if (suitTopImage != null) {
            suitTopImage.sprite = suitSprite;
            suitTopImage.color = cardColor;
        }
        if (suitBotImage != null) {
            suitBotImage.sprite = suitSprite;
            suitBotImage.color = cardColor;
        }

        // 5. Load Main Card Illustration based on type
        // Normalizing type name to match file (e.g. "Attack" -> "attack")
        string resourcePath = $"Images/cards/{cardType.ToLower()}";
        Sprite mainSprite = Resources.Load<Sprite>(resourcePath);
        if (cardMainImage != null && mainSprite != null)
        {
            cardMainImage.sprite = mainSprite;
        }
    }

    private string GetValueString(int value)
    {
        switch (value)
        {
            case 1: return "A";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
            default: return value.ToString();
        }
    }
}
