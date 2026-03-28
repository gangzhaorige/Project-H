using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardAnimationManager : MonoBehaviour
{
    [Header("Animation Settings")]
    public float defaultDuration = 0.5f;

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

    public void Init()
    {
        // Initialization if needed
    }

    /// <summary>
    /// Smoothly moves a transform to a target local position and rotation over time.
    /// Cancels any existing movement coroutine for this object.
    /// </summary>
    public IEnumerator SmoothMoveLocal(Transform tr, Vector3 targetLocalPos, Quaternion targetLocalRot, float duration)
    {
        if (tr == null) yield break;

        GameObject owner = tr.gameObject;

        // Cancel existing
        if (activeCoroutines.TryGetValue(owner, out Coroutine existingCo))
        {
            if (existingCo != null) StopCoroutine(existingCo);
            activeCoroutines.Remove(owner);
        }

        // Start new tracking
        Coroutine newCo = StartCoroutine(SmoothMoveLocalRoutine(tr, targetLocalPos, targetLocalRot, duration, owner));
        activeCoroutines[owner] = newCo;
        
        yield return newCo;
    }

    private IEnumerator SmoothMoveLocalRoutine(Transform tr, Vector3 targetLocalPos, Quaternion targetLocalRot, float duration, GameObject owner)
    {
        Vector3 startPos = tr.localPosition;
        Quaternion startRot = tr.localRotation;
        float elapsed = 0;

        while (elapsed < duration)
        {
            if (tr == null)
            {
                activeCoroutines.Remove(owner);
                yield break;
            }
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t); // Smooth step

            tr.localPosition = Vector3.Lerp(startPos, targetLocalPos, t);
            tr.localRotation = Quaternion.Slerp(startRot, targetLocalRot, t);
            yield return null;
        }

        if (tr != null)
        {
            tr.localPosition = targetLocalPos;
            tr.localRotation = targetLocalRot;
        }

        activeCoroutines.Remove(owner);
    }

    /// <summary>
    /// Smoothly moves a transform to a target world position over time.
    /// Cancels any existing movement coroutine for this object.
    /// </summary>
    public IEnumerator SmoothMoveWorld(Transform tr, Vector3 targetWorldPos, float duration)
    {
        if (tr == null) yield break;

        GameObject owner = tr.gameObject;

        // Cancel existing
        if (activeCoroutines.TryGetValue(owner, out Coroutine existingCo))
        {
            if (existingCo != null) StopCoroutine(existingCo);
            activeCoroutines.Remove(owner);
        }

        // Start new tracking
        Coroutine newCo = StartCoroutine(SmoothMoveWorldRoutine(tr, targetWorldPos, duration, owner));
        activeCoroutines[owner] = newCo;
        
        yield return newCo;
    }

    private IEnumerator SmoothMoveWorldRoutine(Transform tr, Vector3 targetWorldPos, float duration, GameObject owner)
    {
        Vector3 startPos = tr.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            if (tr == null)
            {
                activeCoroutines.Remove(owner);
                yield break;
            }
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t); // Smooth step

            tr.position = Vector3.Lerp(startPos, targetWorldPos, t);
            yield return null;
        }

        if (tr != null)
        {
            tr.position = targetWorldPos;
        }

        activeCoroutines.Remove(owner);
    }

    public void StopAllAnimationsFor(GameObject owner)
    {
        if (owner != null && activeCoroutines.TryGetValue(owner, out Coroutine co))
        {
            if (co != null) StopCoroutine(co);
            activeCoroutines.Remove(owner);
        }
    }
}
