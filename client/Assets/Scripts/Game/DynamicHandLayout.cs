using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DynamicHandLayout : MonoBehaviour
{
    [Header("Components")]
    public HandManager handManager;

    [Header("Layout Configuration")]
    public float cardWidth = 200f;
    [Tooltip("Default spacing. Set equal to cardWidth for no gaps.")]
    public float cardSpacing = 200f;

    private bool needsUpdate = false;

    private void OnValidate()
    {
        needsUpdate = true;
    }

    private void Update()
    {
        if (needsUpdate)
        {
            if (handManager != null) handManager.ReorganizeHand();
            needsUpdate = false;
        }
    }

    /// <summary>
    /// Calculates the local position for a card at a specific index within a total count.
    /// </summary>
    public Vector3 GetLocalPosition(int index, int totalCount, float panelWidth)
    {
        if (totalCount <= 0) return Vector3.zero;

        // 1. Determine effective spacing (squeeze if hand exceeds panel width)
        float totalWidthRequired = (totalCount - 1) * cardSpacing + cardWidth;
        float actualSpacing = (totalWidthRequired > panelWidth && totalCount > 1) 
            ? (panelWidth - cardWidth) / (totalCount - 1) 
            : cardSpacing;

        // 2. Calculate centering offset
        float handWidth = (totalCount - 1) * actualSpacing;
        float startX = -handWidth / 2f;

        return new Vector3(startX + (index * actualSpacing), 0, 0);
    }

    public List<(Vector3 pos, Quaternion rot)> GetCardTransforms(int count, float panelWidth)
    {
        var transforms = new List<(Vector3, Quaternion)>();
        for (int i = 0; i < count; i++)
        {
            transforms.Add((GetLocalPosition(i, count, panelWidth), Quaternion.identity));
        }
        return transforms;
    }
}
