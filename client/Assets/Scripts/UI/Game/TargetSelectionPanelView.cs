using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectH.UI
{
    /// <summary>
    /// Holds references to the UI components for the target selection screen.
    /// Attached to the TargetSelectionPanel in the Canvas.
    /// </summary>
    public class TargetSelectionPanelView : MonoBehaviour
    {
        public Transform container;
        public Button confirmButton;
        public Button cancelButton;
        public TextMeshProUGUI instructionText;
    }
}
