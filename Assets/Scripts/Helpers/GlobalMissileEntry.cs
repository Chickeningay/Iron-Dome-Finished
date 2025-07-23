using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMissileEntry : MonoBehaviour
{
    public List<GameObject> missileRegistry;

    private void Start()
    {
        missileRegistry = new List<GameObject>();
    }
    public void Register(GameObject Missile) 
    {
        missileRegistry.Add(Missile);
    }
}
