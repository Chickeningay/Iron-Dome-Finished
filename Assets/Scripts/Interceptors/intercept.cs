using UnityEngine;

public class intercept : MonoBehaviour
{
    public GameObject explode;
    public bool leftBase=false;
    private void Update()
    {
        if(Vector3.Distance(gameObject.transform.position, new Vector3(0, 0, 0)) > 700f) 
        {
            leftBase = true;
        }
     if(Vector3.Distance(gameObject.transform.position,new Vector3(0, 0, 0)) < 120f && leftBase) 
        {
            Instantiate(explode, transform.position, Quaternion.identity); Destroy(gameObject);
        }   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Missile") 
        {
            Instantiate(explode, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
