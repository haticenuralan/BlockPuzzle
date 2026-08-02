using System.Collections;
using UnityEngine;

public class BlockAnimator : MonoBehaviour
{
    [Tooltip("Yerleşince ne kadar büyüyüp küçülsün")]
    public float punchScale = 1.15f;

    [Tooltip("Animasyon süresi (saniye)")]
    public float duration = 0.15f;

    // Bloğu yerleştirdiğimizde çağrılır: hızlı bir "punch" efekti oynatır
    public void PlayPlaceAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = originalScale * punchScale;

        float half = duration / 2f;
        float t = 0f;

        // Büyüme aşaması
        while (t < half)
        {
            t += Time.deltaTime;
            float progress = t / half;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // Küçülme aşaması (normale dönüş)
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float progress = t / half;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}