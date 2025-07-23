using UnityEngine;

public class InterceptorMove : MonoBehaviour
{
    private bool hasSelectedFirstRoot = false;
    private bool firstRootIsT1 = false;

    public GameObject central;
    public Missile trackedMissile;
    public float interceptorSpeed = 300f;
    private float interceptorRealSpeed = 300f;
    public float turnSpeed = 3f;
    public float minInterceptDistance = 2f;
    public float maxInterceptTime = 50f;

    private Vector3 targetInterceptPoint;
    private bool hasValidIntercept = false;
    private int consecutiveFailures = 0;

    public float recalculateInterval = 1f;
    private float lastRecalculateTime = 0f;

    private Vector3 lastPosition;

    private bool isChasing = false;
    public float chaseDistanceThreshold = 200f;

    public Vector3 missileTargetPoint = Vector3.zero;

    public bool showDebugInfo = true;

    void Update()
    {
        if (central != null)
            central.GetComponent<RadarCentral>().TryGetMissileData(trackedMissile.ID, out trackedMissile);

        float distanceToTarget = Vector3.Distance(transform.position, trackedMissile.position);
        if (distanceToTarget <= chaseDistanceThreshold)
        {
            isChasing = true;
        }

        if (trackedMissile.position == Vector3.zero)
        {
            gameObject.SetActive(false);
        }

        if (!isChasing)
        {
            if (Time.time - lastRecalculateTime > recalculateInterval || consecutiveFailures > 3)
            {
                float deltaTime = Time.time - lastRecalculateTime;
                if (deltaTime > 0f)
                    interceptorRealSpeed = Vector3.Distance(lastPosition, transform.position) / deltaTime;
                else
                    interceptorRealSpeed = interceptorSpeed;

                lastPosition = transform.position;
                CalculateIntercept();
                lastRecalculateTime = Time.time;
            }
        }

        MoveInterceptor();
    }

    void CalculateIntercept()
    {
        Vector3 targetPos = trackedMissile.position;
        Vector3 targetVel = -trackedMissile.direction * trackedMissile.speed;
        Vector3 interceptorPos = transform.position;

        if (showDebugInfo)
        {
            Debug.DrawLine(targetPos, targetPos + targetVel.normalized * 10f, Color.blue, 0.1f);
        }

        if (TryCalculateIntercept(targetPos, targetVel, interceptorPos, interceptorRealSpeed, out Vector3 interceptPoint, out float interceptTime)
            && interceptTime > 0.01f && interceptTime < maxInterceptTime)
        {
            targetInterceptPoint = interceptPoint;
            hasValidIntercept = true;
            consecutiveFailures = 0;

            if (showDebugInfo)
            {
                Debug.DrawLine(interceptorPos, interceptPoint, Color.green, 0.1f);
            }
        }
        else
        {
            targetInterceptPoint = targetPos + targetVel.normalized * 10f;
            hasValidIntercept = true;
            consecutiveFailures++;

            if (showDebugInfo)
            {
                Debug.DrawLine(interceptorPos, targetInterceptPoint, Color.red, 0.1f);
            }
        }
    }

    void MoveInterceptor()
    {
        transform.position += transform.forward * interceptorSpeed * Time.deltaTime;

        if (isChasing)
        {
            Vector3 chaseDir = (trackedMissile.position - transform.position).normalized;
            Quaternion chaseRot = Quaternion.LookRotation(chaseDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, chaseRot, turnSpeed * Time.deltaTime);
        }
        else if (hasValidIntercept && Vector3.Distance(transform.position, targetInterceptPoint) > minInterceptDistance)
        {
            Vector3 dir = (targetInterceptPoint - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    bool TryCalculateIntercept(Vector3 targetPos, Vector3 targetVel, Vector3 interceptorPos, float interceptorSpeed,
                               out Vector3 interceptPoint, out float interceptTime)
    {
        interceptPoint = Vector3.zero;
        interceptTime = 0f;

        Vector3 toTarget = targetPos - interceptorPos;

        if (interceptorSpeed <= targetVel.magnitude)
        {
            interceptTime = toTarget.magnitude / interceptorSpeed;
            if (MissileWillReachTargetBefore(targetPos, targetVel, interceptTime))
                return false;

            interceptPoint = targetPos + targetVel * interceptTime;

            if (!hasSelectedFirstRoot)
            {
                hasSelectedFirstRoot = true;
            }

            return true;
        }

        float a = targetVel.sqrMagnitude - interceptorSpeed * interceptorSpeed;
        float b = 2f * Vector3.Dot(toTarget, targetVel);
        float c = toTarget.sqrMagnitude;

        float discriminant = b * b - 4f * a * c;

        if (discriminant < 0f) return false;

        float sqrtDisc = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);

        bool t1Valid = t1 > 0f && !MissileWillReachTargetBefore(targetPos, targetVel, t1);
        bool t2Valid = t2 > 0f && !MissileWillReachTargetBefore(targetPos, targetVel, t2);

        if (t1Valid && t2Valid)
        {
            if (!hasSelectedFirstRoot)
            {
                interceptTime = Mathf.Min(t1, t2);
                firstRootIsT1 = (interceptTime == t1);
                hasSelectedFirstRoot = true;
            }
            else
            {
                interceptTime = firstRootIsT1 ? t1 : t2;
            }
        }
        else if (t1Valid)
        {
            interceptTime = t1;
            if (!hasSelectedFirstRoot)
            {
                firstRootIsT1 = true;
                hasSelectedFirstRoot = true;
            }
        }
        else if (t2Valid)
        {
            interceptTime = t2;
            if (!hasSelectedFirstRoot)
            {
                firstRootIsT1 = false;
                hasSelectedFirstRoot = true;
            }
        }
        else
        {
            return false;
        }

        interceptPoint = targetPos + targetVel * interceptTime;
        return true;
    }

    bool MissileWillReachTargetBefore(Vector3 missilePos, Vector3 missileVel, float interceptTime)
    {
        Vector3 toTarget = missileTargetPoint - missilePos;
        float speed = missileVel.magnitude;
        if (speed <= 0.01f) return false; // Missile is stationary

        float timeToTarget = Vector3.Dot(toTarget, missileVel.normalized) / speed;
        return timeToTarget >= 0f && timeToTarget < interceptTime;
    }
}
