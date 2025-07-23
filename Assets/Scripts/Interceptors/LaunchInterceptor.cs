using Unity.VisualScripting;
using UnityEngine;

public class LaunchInterceptor : MonoBehaviour
{
    [SerializeField]
    GameObject interceptor;
    public Missile target;
    public bool launch;
    public GameObject central;
    public void LaunchIntercept()
    {
        if (launch) 
        {
            GameObject tempIntercept = Instantiate(interceptor, gameObject.transform.position, Quaternion.identity);
            tempIntercept.GetComponent<InterceptorMove>().trackedMissile = target;
            tempIntercept.GetComponent<InterceptorMove>().central = central;
            launch = false;
            target = new Missile();
            central.GetComponent<RadarCentral>().returnInterceptor(tempIntercept);
        }
    }
}
