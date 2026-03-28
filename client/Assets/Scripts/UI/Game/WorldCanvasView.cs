using UnityEngine;

namespace ProjectH.UI
{
    /// <summary>
    /// Acts as a central repository for all critical UI RectTransforms within the WorldCanvas.
    /// Used by the GameInitiator to inject these UI references into logic managers (like HandManager).
    /// </summary>
    public class WorldCanvasView : MonoBehaviour
    {
        [Header("Card Positions")]
        [Tooltip("The position where the deck is physically located on the board.")]
        public RectTransform deckPosition;
        
        [Tooltip("The position where discarded cards are sent.")]
        public RectTransform discardPilePosition;
        
        [Tooltip("The general area where cards are placed when played onto the field.")]
        public RectTransform playFieldPanel;

        [Header("Player Hand")]
        [Tooltip("The container that holds the local player's current hand of cards.")]
        public RectTransform handPanel;
        
        [Tooltip("The position used to display a specific card prominently (e.g., during targeting or skill resolution).")]
        public RectTransform displayCardPosition;

        [Header("Components")]
        [Tooltip("The layout handler for the player's hand.")]
        public DynamicHandLayout layoutHandHandler;
    }
}
