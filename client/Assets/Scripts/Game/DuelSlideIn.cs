using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class DualUISlideIn : MonoBehaviour
{
    [Header("UI Elements to Animate")]
    public RectTransform leftToRightObject;
    public RectTransform rightToLeftObject;
    public RectTransform topToBottomObject;
    public RectTransform bottomToTopObject; // New: Typically for the skill text

    [Header("Animation Settings")]
    public float slideInDuration = 1.0f;
    public float fadeOutDuration = 0.5f;
    public float horizontalSlideDistance = 200f;
    public float verticalSlideDistance = 200f;
    public Ease easeType = Ease.OutExpo;

    private CanvasGroup canvasGroup;

    // Positions for horizontal objects
    private Vector2 leftObjectOnScreenPos, leftObjectOffScreenPos;
    private Vector2 rightObjectOnScreenPos, rightObjectOffScreenPos;
    // Positions for vertical objects
    private Vector2 topObjectOnScreenPos, topObjectOffScreenPos;
    private Vector2 bottomObjectOnScreenPos, bottomObjectOffScreenPos; // New
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Store positions for horizontal objects
        if (leftToRightObject != null)
        {
            leftObjectOnScreenPos = leftToRightObject.anchoredPosition;
            leftObjectOffScreenPos = new Vector2(leftObjectOnScreenPos.x - horizontalSlideDistance, leftObjectOnScreenPos.y);
        }
        if (rightToLeftObject != null)
        {
            rightObjectOnScreenPos = rightToLeftObject.anchoredPosition;
            rightObjectOffScreenPos = new Vector2(rightObjectOnScreenPos.x + horizontalSlideDistance, rightObjectOnScreenPos.y);
        }
        
        // Store positions for vertical objects
        if (topToBottomObject != null)
        {
            topObjectOnScreenPos = topToBottomObject.anchoredPosition;
            topObjectOffScreenPos = new Vector2(topObjectOnScreenPos.x, topObjectOnScreenPos.y + verticalSlideDistance);
        }
        if (bottomToTopObject != null)
        {
            bottomObjectOnScreenPos = bottomToTopObject.anchoredPosition;
            bottomObjectOffScreenPos = new Vector2(bottomObjectOnScreenPos.x, bottomObjectOnScreenPos.y - verticalSlideDistance);
        }
    }

    private void ResetToStartPosition()
    {
        // Stop any running animations
        if (leftToRightObject != null) leftToRightObject.DOKill();
        if (rightToLeftObject != null) rightToLeftObject.DOKill();
        if (topToBottomObject != null) topToBottomObject.DOKill();
        if (bottomToTopObject != null) bottomToTopObject.DOKill();
        canvasGroup.DOKill();

        // Instantly move panels to their off-screen positions
        if (leftToRightObject != null) leftToRightObject.anchoredPosition = leftObjectOffScreenPos;
        if (rightToLeftObject != null) rightToLeftObject.anchoredPosition = rightObjectOffScreenPos;
        if (topToBottomObject != null) topToBottomObject.anchoredPosition = topObjectOffScreenPos;
        if (bottomToTopObject != null) bottomToTopObject.anchoredPosition = bottomObjectOffScreenPos;

        // Reset alpha
        canvasGroup.alpha = 1f;
    }

    public void PlayAnimationAndFadeOut(Action onFinished)
    {
        ResetToStartPosition();
        this.gameObject.SetActive(true);
        
        Sequence mySequence = DOTween.Sequence();

        if (leftToRightObject != null) 
            mySequence.Join(leftToRightObject.DOAnchorPos(leftObjectOnScreenPos, slideInDuration).SetEase(easeType));
        if (rightToLeftObject != null) 
            mySequence.Join(rightToLeftObject.DOAnchorPos(rightObjectOnScreenPos, slideInDuration).SetEase(easeType));
        if (topToBottomObject != null) 
            mySequence.Join(topToBottomObject.DOAnchorPos(topObjectOnScreenPos, slideInDuration).SetEase(easeType));
        if (bottomToTopObject != null) 
            mySequence.Join(bottomToTopObject.DOAnchorPos(bottomObjectOnScreenPos, slideInDuration).SetEase(easeType));

        mySequence.Append(canvasGroup.DOFade(0, fadeOutDuration));

        // When the sequence completes, hide the object AND invoke the callback we received.
        mySequence.OnComplete(() =>
        {
            this.gameObject.SetActive(false);
            onFinished?.Invoke(); // Signals completion to the manager
        });
    }
}
