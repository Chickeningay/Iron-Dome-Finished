using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarDetect : MonoBehaviour
{
    public GameObject GlobalMissileEntry;
    public List<GameObject> Missiles = new List<GameObject>();

    // This will hold detected missiles as Missile structs (position, direction, speed etc.)
    public List<Missile> visibleMissiles = new List<Missile>();

    public float RadarRange = 5000f;
    float speedOfLight = 300000000f;

    void Start()
    {
        GlobalMissileEntry = GameObject.Find("GlobalMissileEntry");
        Missiles = GlobalMissileEntry.GetComponent<GlobalMissileEntry>().missileRegistry;
    }

    void Update()
    {
        Missiles = GlobalMissileEntry.GetComponent<GlobalMissileEntry>().missileRegistry;

        visibleMissiles.Clear();

        foreach (var missileGO in Missiles)
        {
            if (missileGO == null) continue;

            // Check if missile within radar range
            if (Vector3.Distance(missileGO.transform.position, transform.position) < RadarRange)
            {
                MissileMove missileMove = missileGO.GetComponent<MissileMove>();
                if (missileMove == null) continue;  // Skip if missing component

                // Use MissileProps method to get info about missile
                missileMove.MissileProps(transform, out float duration, out Vector3 direction);

                // Predicted missile position relative to radar (similar to main)
                Vector3 predictedPos = transform.position + direction * duration * speedOfLight;

                // Construct Missile struct with the detected info
                Missile m = new Missile
                {
                    position = predictedPos,
                    direction = direction,
                    speed = 0f,  // Speed is unknown here; central tracker will calculate
                    init = false,
                    passed = true  // Mark true for now; central uses this flag each frame
                };

                visibleMissiles.Add(m);
            }
        }
    }
}
