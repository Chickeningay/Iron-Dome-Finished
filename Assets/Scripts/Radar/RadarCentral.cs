using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Missile
{
    public int ID;
    public float speed;
    public Vector3 position;
    public Vector3 direction;
    public bool passed;
    public bool init;
}

public class RadarCentral : MonoBehaviour
{
    [Header("Radar Detection Fusion")]
    public List<GameObject> radars = new List<GameObject>();
    public List<Missile> trackedMissiles = new List<Missile>();
    public float defaultThreshold = 100f;
    public float speedMultiplier = 2f;
    int nextMissileID = 0;

    [Header("Launchers")]
    public List<GameObject> launchers = new List<GameObject>();

    public List<InterceptorMove> interceptors = new List<InterceptorMove>();

    private void Start()
    {
        launchers = GameObject.FindGameObjectsWithTag("Launcher").ToList<GameObject>();
        radars = GameObject.FindGameObjectsWithTag("Radar").ToList<GameObject>();
    }

    void Update()
    {
        for (int i = 0; i < trackedMissiles.Count; i++)
        {
            Missile m = trackedMissiles[i];
            m.passed = false;
            trackedMissiles[i] = m;
        }

        List<Missile> combinedDetections = new List<Missile>();
        foreach (var radar in radars)
        {
            if (radar == null) continue;
            combinedDetections.AddRange(radar.GetComponent<RadarDetect>().visibleMissiles);
        }

        trackedMissiles = FuseRadarReports(combinedDetections, trackedMissiles);

        for (int i = trackedMissiles.Count - 1; i >= 0; i--)
        {
            if (!trackedMissiles[i].passed)
            {
                trackedMissiles.RemoveAt(i);
            }
        }

       /*
        // Optional visualization
        foreach (var m in trackedMissiles)
        {
            Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), m.position, Quaternion.identity);
        }
       */
        foreach (var Missile in trackedMissiles)
        {

            bool matched = false;
            foreach (var inteceptor in interceptors)
            {
                if (inteceptor.trackedMissile.ID == Missile.ID)
                {
                    matched = true;

                }

            }

            if (!matched)
            {

                GameObject selectedLauncher = SelectClosestLaunchPosition(Missile.position, launchers);
                selectedLauncher.GetComponent<LaunchInterceptor>().central = gameObject;
                selectedLauncher.GetComponent<LaunchInterceptor>().target = Missile;
                selectedLauncher.GetComponent<LaunchInterceptor>().launch = true;
                selectedLauncher.GetComponent<LaunchInterceptor>().LaunchIntercept();


            }

        }
    }

    public void returnInterceptor(GameObject Interceptor)
    {

        interceptors.Add(Interceptor.GetComponent<InterceptorMove>());

    }

    public bool TryGetMissileData(int missileID, out Missile missileData)
    {
        foreach (var missile in trackedMissiles)
        {
            if (missile.ID == missileID)
            {
                missileData = missile;
                return true;
            }
        }
        missileData = default;
        return false;
    }

    List<Missile> FuseRadarReports(List<Missile> radarReports, List<Missile> trackedMissiles)
    {
        foreach (var report in radarReports)
        {
            bool matched = false;

            for (int i = 0; i < trackedMissiles.Count; i++)
            {
                Missile m = trackedMissiles[i];

                float dynamicThreshold = m.speed * Time.deltaTime * speedMultiplier;
                float threshold = Mathf.Max(dynamicThreshold, defaultThreshold);

                if (Vector3.Distance(report.position, m.position) <= threshold)
                {
                    float distanceMoved = Vector3.Distance(m.position, report.position);
                    m.position = report.position;
                    m.direction = report.direction;
                    m.passed = true;

                    if (!m.init)
                    {
                        m.speed = distanceMoved / Time.deltaTime;
                        m.init = true;
                    }
                    else
                    {
                        m.speed = distanceMoved / Time.deltaTime;
                    }

                    trackedMissiles[i] = m;
                    matched = true;
                    break;  // Stop checking after first match
                }
            }

            if (!matched)
            {
                Missile newMissile = new Missile
                {
                    ID = nextMissileID++,
                    position = report.position,
                    direction = report.direction.normalized,
                    speed = 0f,
                    passed = true,
                    init = false
                };

                trackedMissiles.Add(newMissile);
            }
        }

        return trackedMissiles;
    }


    public GameObject SelectClosestLaunchPosition(Vector3 targetPos, List<GameObject> launchPositions)
    {
        GameObject closest = null;
        float minDistSqr = float.MaxValue;

        foreach (var pos in launchPositions)
        {
            if (pos == null) continue;

            float distSqr = (pos.transform.position - targetPos).sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                closest = pos;
            }
        }
        return closest;
    }
}