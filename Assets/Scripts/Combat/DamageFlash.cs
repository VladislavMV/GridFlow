using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Advanced (For Solid White Shader)")]
    [Tooltip("Увімкніть, якщо використовуєте кастомний шейдер зі змінною _FlashAmount")]
    [SerializeField] private bool useShaderFlash = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Material material;
    private Coroutine flashCoroutine;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            material = spriteRenderer.material;
        }
    }

    public void PlayFlash()
    {
        if (spriteRenderer == null || !gameObject.activeInHierarchy) return;

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (useShaderFlash)
        {
            material.SetFloat("_FlashAmount", 1f);
            material.SetColor("_FlashColor", flashColor);
        }
        else
        {
            spriteRenderer.color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        if (useShaderFlash)
        {
            material.SetFloat("_FlashAmount", 0f);
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
    }
}