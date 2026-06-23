using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float maxOffset = 0.18f;   // limite máximo de deslocamento
    [SerializeField] private float damping = 0.9f;      // reduz intensidade ao longo do tempo

    private Coroutine shakeCoroutine;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        float intensidadeAtual = magnitude;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * intensidadeAtual;
            float offsetY = Random.Range(-1f, 1f) * intensidadeAtual;

            offsetX = Mathf.Clamp(offsetX, -maxOffset, maxOffset);
            offsetY = Mathf.Clamp(offsetY, -maxOffset, maxOffset);

            transform.localPosition = originalLocalPosition + new Vector3(offsetX, offsetY, 0f);

            intensidadeAtual *= damping;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        shakeCoroutine = null;
    }
}
