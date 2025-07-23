using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissileMove : MonoBehaviour
{
    public GameObject Target;
    public GameObject GlobalRegistry;
    public float speed = 5f;
    float speedOfLight = 300000000;
    void OnEnable()
    {
        Target = GameObject.Find("Target"); GlobalRegistry = GameObject.Find("GlobalMissileEntry"); GlobalRegistry.GetComponent<GlobalMissileEntry>().Register(gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null) { 
        gameObject.transform.position = Vector3.MoveTowards(
            gameObject.transform.position,
            Target.transform.position,
            speed * Time.deltaTime
        );
        transform.LookAt(Target.transform);
        }
    }

    public void MissileProps(Transform Radar, out float duration, out Vector3 direction)
    {
        Vector3 toRadar = transform.position- Radar.position;
        float distance = toRadar.magnitude;
        direction = toRadar.normalized;
        duration = distance / speedOfLight;      
    }

}
