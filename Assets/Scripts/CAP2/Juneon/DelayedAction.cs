using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedAction : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    public UnityEvent onDelayComplete;

    public void TriggerDelay()
    {
        StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        yield return new WaitForSeconds(delay);
        onDelayComplete?.Invoke();
    }
}