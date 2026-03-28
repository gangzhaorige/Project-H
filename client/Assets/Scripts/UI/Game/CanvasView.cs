using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectH.UI
{
    /// <summary>
    /// Acts as a central repository for all 2D UI elements within the main screen-space Canvas.
    /// Used by the GameInitiator to inject UI references into the UIController logic.
    /// </summary>
    public class CanvasView : MonoBehaviour
    {
        [Header("Turn Controls")]
        public Button endTurnButton;
        public Button passButton;
        public Button confirmPlayButton;

        [Header("Status Displays")]
        public TextMeshProUGUI turnStatusText;
        public TextMeshProUGUI instructionText;
        public TextMeshProUGUI stateText;

        [Header("Timer UI")]
        public GameObject timerUI;
        public Slider timerSlider;

        [Header("Major Panels")]
        public TargetSelectionPanelView targetSelectionPanelView;
        public SelectPanelView selectPanelView;
        public SkillResponsePanelView skillResponsePanelView;
        public RectTransform skillActivationPanel;
        public RectTransform skillContainer;
    }
}
