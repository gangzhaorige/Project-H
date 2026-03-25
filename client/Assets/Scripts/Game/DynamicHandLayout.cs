using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DynamicHandLayout : MonoBehaviour
{
    [Header("Layout Configuration")]
    public float cardWidth = 200f;
    [Tooltip("Default spacing. Set equal to cardWidth for no gaps.")]
    public float cardSpacing = 200f;

    /// <summary>
    /// Calculates the local position for a card at a specific index within a total count.
    /// </summary>
    public Vector3 GetLocalPosition(int index, int totalCount, float panelWidth)
    {
        if (totalCount <= 0) return Vector3.zero;

        // Use a fallback width if the panel hasn't initialized yet to prevent stacking at (0,0,0)
        float effectiveWidth = panelWidth > 0 ? panelWidth : 1000f;

        // 1. Determine effective spacing (squeeze if hand exceeds panel width)
        float totalWidthRequired = (totalCount - 1) * cardSpacing + cardWidth;
        float actualSpacing = (totalWidthRequired > effectiveWidth && totalCount > 1) 
            ? (effectiveWidth - cardWidth) / (totalCount - 1) 
            : cardSpacing;

        // Prevent complete stacking: ensure cards are always offset by at least 15% of their width
        float minSpacing = cardWidth * 0.15f;
        actualSpacing = Mathf.Max(actualSpacing, minSpacing);

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
