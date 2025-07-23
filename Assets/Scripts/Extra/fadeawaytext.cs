using TMPro;
using UnityEngine;

public class fadeawaytext : MonoBehaviour
{
     TextMeshProUGUI textToFade;
    public float fadeDuration = 2f;

    private float fadeTimer;

    void Start()
    {
        textToFade = GetComponent<TextMeshProUGUI>();
        fadeTimer = fadeDuration;
    }

    void Update()
    {
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            Color color = textToFade.color;
            color.a = alpha;
            textToFade.color = color;
        }
    }
}
