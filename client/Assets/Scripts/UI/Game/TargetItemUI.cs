using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectH.Models;
using System;

public class TargetItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image portraitImage;
    public Toggle selectionToggle;
    public CanvasGroup canvasGroup;

    public int PlayerId { get; private set; }
    private Action<int, bool> onToggled;

    public void Setup(PlayerData data, bool inRange, Action<int, bool> onToggled)
    {
        this.PlayerId = data.PlayerId;
        this.onToggled = onToggled;

        if (nameText != null) nameText.text = data.Username;
        if (portraitImage != null && data.Champion != null)
        {
            portraitImage.sprite = Resources.Load<Sprite>($"Images/Characters/Portraits/InGame/{data.Champion.Id}");
        }

        if (selectionToggle != null)
        {
            selectionToggle.interactable = inRange;
            selectionToggle.onValueChanged.RemoveAllListeners();
            selectionToggle.onValueChanged.AddListener((val) => onToggled?.Invoke(PlayerId, val));
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = inRange ? 1f : 0.5f;
            canvasGroup.interactable = inRange;
            canvasGroup.blocksRaycasts = inRange;
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectionToggle != null)
        {
            selectionToggle.SetIsOnWithoutNotify(selected);
        }
    }
}
