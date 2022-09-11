using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerMovementMock1 : MonoBehaviour
{
    public float PlayerSpeed = 1f;
    [SerializeField] GameObject enemyPrefab;
    private Vector3 viewPos;
    public FixedJoystick FixedJoystick;
    private Camera cam;
    SphereCollider deflectArea;

    bool canDeflect;
    [SerializeField] float deflectCD;
    [SerializeField] Button defBtn;

    [SerializeField] TextMeshProUGUI timeTxt;
    [SerializeField] float timeToDisplay;


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
            Vector3 viewPortPos = new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.6f, 0.8f), viewPos.z + 43);
            Vector3 worldPos = cam.ViewportToWorldPoint(viewPortPos);

            if (!Physics.CheckSphere(worldPos, 11))
            {
                GameObject enemy = Instantiate(enemyPrefab, worldPos, transform.rotation);
                Transform smoke = enemy.transform.Find("smoke");
                smoke.transform.rotation = new Quaternion(0.0f, cam.transform.rotation.y, 0.0f, cam.transform.rotation.w);
                smoke.gameObject.SetActive(true);
                enemy.gameObject.SetActive(true);
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
        if (!canDeflect) return;

        canDeflect = false;
        defBtn.interactable = false;
        deflectCD = 10;

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

    void DisplayTime()
    {
        if (timeToDisplay < 1)
            return;

        timeToDisplay -= Time.deltaTime;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Update is called once per frame
    // TO DO: Create a better mechanic after finalizing mechanics,
    // Current impl only works for slow character speeds
    void Update()
    {

        DisplayTime();

        if (!canDeflect)
        {
            if (deflectCD > 0)
            {
                deflectCD -= Time.deltaTime;
            }
            else
            {
                canDeflect = true;
                deflectCD = 0;
                defBtn.interactable = true;
            }
        }

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
