using UnityEngine;

public class selfstart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("selfEnable", 0.1f);
    }
    void selfEnable() 
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
