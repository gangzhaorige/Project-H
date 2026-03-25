using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChampionSkillPanel : MonoBehaviour
{
    [SerializeField] private Image charImage;
    [SerializeField] private TextMeshProUGUI skillName;

    [Header("Color Scheme Elements")]
    [SerializeField] private Image background;
    [SerializeField] private Image frame;
    [SerializeField] private Image textBackground;

    private struct ElementTheme
    {
        public Color bgColor;
        public Color frameColor;
        public Color textBgColor;

        public ElementTheme(string bgHex, string frameHex, string textBgHex)
        {
            ColorUtility.TryParseHtmlString("#" + bgHex, out bgColor);
            ColorUtility.TryParseHtmlString("#" + frameHex, out frameColor);
            ColorUtility.TryParseHtmlString("#" + textBgHex, out textBgColor);
        }
    }

    private Dictionary<int, ElementTheme> themes = new Dictionary<int, ElementTheme>()
    {
        { 1, new ElementTheme("D3D3D3", "A9A9A9", "2F4F4F") }, // Physical
        { 2, new ElementTheme("FF8C00", "FF4500", "8B0000") }, // Fire
        { 3, new ElementTheme("B0E0E6", "00BFFF", "00008B") }, // Ice
        { 4, new ElementTheme("E6E6FA", "9370DB", "4B0082") }, // Lightning
        { 5, new ElementTheme("79D185", "73EE66", "17603B") }, // Wind (Provided)
        { 6, new ElementTheme("9370DB", "8A2BE2", "191970") }, // Quantum
        { 7, new ElementTheme("FFFACD", "FFD700", "8B4513") }  // Imaginary
    };

    public void SetElementTheme(int elementId)
    {
        if (themes.TryGetValue(elementId, out ElementTheme theme))
        {
            if (background != null) background.color = theme.bgColor;
            if (frame != null) frame.color = theme.frameColor;
            if (textBackground != null) textBackground.color = theme.textBgColor;
        }
    }

    public void UpdatePanelInfo(Sprite characterSprite)
    {
        if (characterSprite != null)
        {
            charImage.sprite = characterSprite;
        }
    }

    public void UpdateText(string skillName)
    {
        if (skillName != null)
        {
            this.skillName.text = skillName;
        }
    }
}
