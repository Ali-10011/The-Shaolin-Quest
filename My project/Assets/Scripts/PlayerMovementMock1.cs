using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementMock1 : MonoBehaviour
{
    public float PlayerSpeed = 1f;
    [SerializeField] GameObject enemyPrefab;
    private Vector3 viewPos;
    public FixedJoystick FixedJoystick;
    private Camera cam;
    SphereCollider deflectArea;

    public static List<GameObject> bulletsList = new List<GameObject>();
    public static List<GameObject> bulletsToDelete = new List<GameObject>();
    [SerializeField] float spawnAfter = 5f;
    // Start is called before the first frame update
    private void Awake() 
    {
        cam = Camera.main;
        viewPos = cam.WorldToViewportPoint(transform.position);
        deflectArea = GetComponent<SphereCollider>();
        StartCoroutine(AutoSpawnEnemies());
    }

    private bool Spawn()
    {
        for (int i = 0; i < 10; i++)
        {
            // A random position in viewport
            Vector3 viewPortPos = new Vector3(Random.Range(0.1f, 0.9f), Random.Range(0.6f, 0.8f), viewPos.z + 43);
            Vector3 worldPos = cam.ViewportToWorldPoint(viewPortPos);
            Vector3 collisionCircle = new Vector3(worldPos.x, worldPos.y+5.6f, worldPos.z);
            if (!Physics.CheckSphere(worldPos, 12))
            {
                GameObject enemy = Instantiate(enemyPrefab, worldPos, transform.rotation);
                return true;
            }
        }   
        return false;
    }

     IEnumerator AutoSpawnEnemies()
     {
         while(true) 
         {
            Spawn();
            yield return new WaitForSeconds(spawnAfter);
         }
     }

    public void Deflect()
    {
        Collider[] bullets = Physics.OverlapSphere(transform.position, 20f);

        foreach (Collider bullet in bullets)
        {
            if (bullet.tag.Equals("Bullet"))
            {
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                bullet.transform.Find("kunai1").Rotate(180, 0, 0);
                bullet.transform.Find("kunai2").Rotate(180, 0, 0);
                rb.velocity = -rb.velocity;
                bullet.tag = "DeflectedBullet";
            }
        }
    }

    // Update is called once per frame
    // TO DO: Create a better mechanic after finalizing mechanics,
    // Current impl only works for slow character speeds
    void Update()
    {
        
        // Get Poisition of object relative to camera's viewport Range is bw 0 and 1
        // 0 means extreme left on x axis, 1 means extreme right on x-axis
        // 0 means bottom on y axis, 1 means top on y-axis
        viewPos = cam.WorldToViewportPoint(transform.position);

        // Only move object when within the given bounds (viewport of the camera)
        // To prevent a part of object going out of viewport, a larger/smaller value is chosen
        // instead of 0 and 1.
        float xMov = FixedJoystick.Horizontal * PlayerSpeed;
        float yMov = FixedJoystick.Vertical * PlayerSpeed;

        // Calculate final position in advance before movement
        Vector3 newPos = transform.position + transform.TransformDirection(new Vector3(FixedJoystick.Horizontal * PlayerSpeed, y: FixedJoystick.Vertical * PlayerSpeed, z: 0));
        newPos = cam.WorldToViewportPoint(newPos);

        // If final position is inside movement, move, otherwise do not
        if ((0.07f < newPos.x && newPos.x < 0.93f) && (0.07f < newPos.y && newPos.y < 0.53f))
        {
            transform.Translate(translation: new Vector3(FixedJoystick.Horizontal*PlayerSpeed,y:FixedJoystick.Vertical*PlayerSpeed,z:0));
        }
    }
}
