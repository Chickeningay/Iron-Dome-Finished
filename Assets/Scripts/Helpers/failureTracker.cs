using UnityEngine;

public class failureTracker : MonoBehaviour
{
    public int hitCount = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Missile") 
        {
            Destroy(other.gameObject);
            hitCount++;
            if (hitCount > 1) { print(hitCount + " Missiles have hit their target"); }
            else { print(hitCount + " Missile has hit their target"); }
        }
    }
}
