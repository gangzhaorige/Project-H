using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectH.UI
{
    /// <summary>
    /// Holds references to the UI components for the card selection panel (e.g., stealing from opponent).
    /// Attached to the SelectPanel in the Canvas.
    /// </summary>
    public class SelectPanelView : MonoBehaviour
    {
        [Tooltip("The container where selectable card items will be spawned.")]
        public Transform cardsContainer;
        
        [Tooltip("The prefab used for cards in the selection panel.")]
        public GameObject cardPrefab;
        
        [Tooltip("The button to confirm the selection.")]
        public Button confirmButton;
        
        [Tooltip("Optional text field to display instructions or messages.")]
        public TextMeshProUGUI messageText;
    }
}
