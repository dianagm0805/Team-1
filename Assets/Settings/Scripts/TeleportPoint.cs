using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class TeleportPoint : MonoBehaviour
{
    private float dimmingSpeed = 16;
    private float fullIntensity = 4;
    private float lowIntensity = 0.5f;
    private float lastLookAtTime = 0;

    private float defaultHeight = 0.25f;
    private float fadeOutDuration = 1f;

    public Transform teleportTransform;

    [SerializeField] private GameObject rainParticle;

#if UNITY_EDITOR
    [SerializeField] private SceneAsset targetScene;
#endif
    [SerializeField] private string targetSceneName = "";

    private bool isGlowing = false;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public Vector3 GetTeleportPosition()
    {
        return teleportTransform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportWithFade());
        }
    }

    private System.Collections.IEnumerator TeleportWithFade()
    {
        yield return StartCoroutine(FadeToBlack(fadeOutDuration));
        LoadTargetScene();
    }

    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("TeleportPoint: No target scene assigned!", this);
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetScene != null)
        {
            targetSceneName = targetScene.name;
        }
    }
#endif

    private void Update()
    {
        if (meshRenderer == null)
            return;

        float intensity = Mathf.SmoothStep(fullIntensity, lowIntensity, (Time.time - lastLookAtTime) * dimmingSpeed);
        meshRenderer.material.SetFloat("_Intensity", intensity);
    }

    public void OnLookAt(bool isValid)
    {
        if (isGlowing == false)
        {
            isGlowing = true;
            rainParticle.SetActive(true);
            StartCoroutine(ScaleYCoroutine(1f, 0.5f));
        }
    }

    public void Reset()
    {
        if (isGlowing)
        {
            isGlowing = false;
            rainParticle.SetActive(false);
            StartCoroutine(ScaleYCoroutine(defaultHeight, 0.5f));
        }
    }

    private System.Collections.IEnumerator ScaleYCoroutine(float targetScale, float duration)
    {
        Vector3 startScale = teleportTransform.localScale;
        Vector3 endScale = startScale;
        endScale.y = targetScale;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            teleportTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        teleportTransform.localScale = endScale;
    }

    private System.Collections.IEnumerator FadeToBlack(float duration)
    {
        // Find or create fade canvas
        Canvas fadeCanvas = FindObjectOfType<Canvas>();
        if (fadeCanvas == null)
        {
            GameObject canvasGO = new GameObject("FadeCanvas");
            fadeCanvas = canvasGO.AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        // Create fade image
        GameObject fadeImageGO = new GameObject("FadeImage");
        fadeImageGO.transform.SetParent(fadeCanvas.transform, false);
        Image fadeImage = fadeImageGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);

        RectTransform rectTransform = fadeImageGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // Fade in to black
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }
}
