using UnityEngine;
using UnityEngine.UI;

public class spawnCooldownCircle : MonoBehaviour
{
    private Image cooldownImage;
    public float cooldownTime = 0.2f;
    private float timer = 0f;
    private bool isCoolingDown = false;
    private void Start()
    {
        cooldownImage = GetComponent<Image>();
    }
    void Update()
    {
        if (isCoolingDown)
        {
            timer -= Time.deltaTime;
            cooldownImage.fillAmount = timer / cooldownTime;

            if (timer <= 0f)
            {
                isCoolingDown = false;
                cooldownImage.fillAmount = 0f;
            }
        }
    }

    public void StartCooldown()
    {
        isCoolingDown = true;
        timer = cooldownTime;
        cooldownImage.fillAmount = 1f;
    }
}