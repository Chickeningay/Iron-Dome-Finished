using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class spawnXtra : MonoBehaviour
{

    Transform[] camPos;
    public GameObject camPositionParent;
    Camera mainCamera;
    public GameObject missile;
    bool dontSpawn = false;
    public GameObject FPS;
    bool spawnCooldownActive=false;
    int perspective=0;
    public GameObject spawnCooldownCircle;
    void Start()
    {
        mainCamera = gameObject.GetComponent<Camera>();
        Transform[] rawChildren = camPositionParent.GetComponentsInChildren<Transform>();
        camPos = new Transform[rawChildren.Length - 1];

        int index = 0;
        for (int i = 0; i < rawChildren.Length; i++)
        {
            if (rawChildren[i] != camPositionParent.transform)
                camPos[index++] = rawChildren[i];
        }

    }
    IEnumerator spawnCooldown()
    {
        spawnCooldownActive = true;
        spawnCooldownCircle.GetComponent<spawnCooldownCircle>().StartCooldown();
        yield return new WaitForSeconds(0.2f);
        spawnCooldownActive = false;
    }
    void Update()
    {
        spawnCooldownCircle.GetComponent<Image>().enabled = !dontSpawn;
        if (mainCamera.enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown(KeyCode.R)&& mainCamera.enabled) 
        {
            perspective++;
            if (perspective >= camPos.Length) { perspective = 0; }
            gameObject.transform.position = camPos[perspective].position;
            gameObject.transform.rotation = camPos[perspective].rotation;

        }
        if (Input.GetMouseButtonDown(1))
        {
            mainCamera.enabled = !mainCamera.enabled;
            dontSpawn = !dontSpawn;
            FPS.SetActive(!FPS.activeSelf);

        }
        if (Input.GetMouseButtonDown(0)&&!dontSpawn&& !spawnCooldownActive)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if we hit anything
            if (!Physics.Raycast(ray, out hit, 10000f))
            {
                float randomDistance = Random.Range(-1500f, 3000f);
                Vector3 pointInSky = ray.origin + ray.direction * randomDistance;
                Instantiate(missile, pointInSky, Quaternion.identity);
                StartCoroutine(spawnCooldown());
            }
        }
    }
}
