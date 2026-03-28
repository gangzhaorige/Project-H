using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace ProjectH.UI
{
    public class SkillResponsePanelView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        public void Show(string message, UnityAction onYes, UnityAction onNo)
        {
            if (_messageText != null) _messageText.text = message;

            _yesButton.onClick.RemoveAllListeners();
            _yesButton.onClick.AddListener(() => {
                onYes?.Invoke();
                Hide();
            });

            _noButton.onClick.RemoveAllListeners();
            _noButton.onClick.AddListener(() => {
                onNo?.Invoke();
                Hide();
            });

            if (_panel != null) _panel.SetActive(true);
        }

        public void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
        }

        private void Awake()
        {
            Hide();
        }
    }
}
