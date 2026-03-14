using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChampionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerInfoText; // Username + ID
    [SerializeField] private TextMeshProUGUI championNameText;
    [SerializeField] private Image championImage;
    [SerializeField] private Image pathImage;

    public int Team { get; set; }

    public void Init(int playerId, string username, int team)
    {
        this.Team = team;
        if (playerInfoText != null)
        {
            playerInfoText.text = $"{username} ({playerId})";
        }
        
        if (championNameText != null)
        {
            championNameText.text = "Waiting...";
        }

        if (championImage != null)
        {
            // Flip 180 degrees on Y axis if Blue Team
            if (Team == Constants.TEAM_BLUE)
            {
                championImage.rectTransform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                championImage.rectTransform.localEulerAngles = Vector3.zero;
            }
            championImage.gameObject.SetActive(false);
        }

        if (pathImage != null)
        {
            pathImage.gameObject.SetActive(false);
        }
    }

    public void UpdateChampion(string displayName, Sprite portrait = null, Sprite pathIcon = null, string element = "")
    {
        if (championNameText != null)
        {
            championNameText.text = displayName;
        }

        if (championImage != null)
        {
            if (portrait != null)
            {
                championImage.sprite = portrait;
                championImage.color = Color.white;
                championImage.gameObject.SetActive(true);
            }
            else
            {
                championImage.gameObject.SetActive(false);
            }
        }

        if (pathImage != null)
        {
            if (pathIcon != null)
            {
                pathImage.sprite = pathIcon;
                pathImage.color = GetElementColor(element);
                pathImage.gameObject.SetActive(true);
            }
            else
            {
                pathImage.gameObject.SetActive(false);
            }
        }
    }

    private Color GetElementColor(string element)
    {
        switch (element.ToLower())
        {
            case "physical": return new Color(0.7f, 0.7f, 0.7f); // Silver/Gray
            case "fire": return new Color(0.9f, 0.2f, 0.2f); // Red
            case "ice": return new Color(0.2f, 0.6f, 0.9f); // Light Blue
            case "lightning": return new Color(0.7f, 0.3f, 0.9f); // Purple
            case "wind": return new Color(0.3f, 0.8f, 0.5f); // Green
            case "quantum": return new Color(0.3f, 0.3f, 0.8f); // Indigo/Dark Blue
            case "imaginary": return new Color(0.9f, 0.8f, 0.2f); // Gold/Yellow
            default: return Color.white;
        }
    }
    
    public void SetLockedIn(bool locked)
    {
        if (championImage != null)
        {
            // Visual feedback for locking in, e.g., changing alpha or adding a border
            // For now, let's just ensure it's fully visible
            championImage.color = locked ? Color.white : new Color(1, 1, 1, 0.5f);
        }
    }
}
