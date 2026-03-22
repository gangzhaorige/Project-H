using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance { get; private set; }

    private Queue<IEnumerator> animationQueue = new Queue<IEnumerator>();
    private bool isProcessing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // If we are not currently playing an animation and there are more in the queue, start the next one
        if (!isProcessing && animationQueue.Count > 0)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        // Get the next animation coroutine
        IEnumerator nextAnim = animationQueue.Dequeue();
        
        // Execute it and wait until it is completely finished
        yield return StartCoroutine(nextAnim);

        isProcessing = false;
    }

    /// <summary>
    /// Adds an animation task to the global sequential queue.
    /// Example: AnimationController.Instance.AddAnimation(MyCoroutine());
    /// </summary>
    public void AddAnimation(IEnumerator anim)
    {
        animationQueue.Enqueue(anim);
    }

    /// <summary>
    /// Simple utility to add a delay between other animations in the queue.
    /// </summary>
    public void AddDelay(float duration)
    {
        AddAnimation(WaitCoroutine(duration));
    }

    private IEnumerator WaitCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    public bool IsBusy()
    {
        return isProcessing || animationQueue.Count > 0;
    }
}
