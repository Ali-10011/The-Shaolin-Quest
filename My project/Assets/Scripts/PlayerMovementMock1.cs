using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Cinemachine.Utility;

public class PlayerMovementMock1 : MonoBehaviour
{
    public float PlayerSpeed = 1f;
    [SerializeField] GameObject enemyPrefab;
    Rigidbody rigidbody;
    private Vector3 viewPos;
    FixedJoystick FixedJoystick;
    private Camera cam;
    GameObject playScreen;
    
    bool canDeflect;
    [SerializeField] float deflectCD;
    [SerializeField] float currDeflectCD;
    Button defBtn;

    TextMeshProUGUI timeTxt;
    TextMeshProUGUI scoreTxt;
    [SerializeField] float timeToDisplay;


    public static List<GameObject> bulletsList = new List<GameObject>();
    public static List<GameObject> bulletsToDelete = new List<GameObject>();
    [SerializeField] float spawnAfter;
    [SerializeField] float maxEnemies;

    public float totalEnemies = 0;
    public float hitEnemies = 0;
    public float missedEnemies = 0;

    public float score = 0;

    
    // Start is called before the first frame update
    private void Awake() 
    {
        cam = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        playScreen = GameObject.Find("Canvas").transform.Find("PlayScreen").gameObject;

        timeTxt = playScreen.transform.Find("TimeTxt").GetComponent<TextMeshProUGUI>();
        scoreTxt = playScreen.transform.Find("ScoreTxt").GetComponent<TextMeshProUGUI>();

        FixedJoystick = playScreen.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        defBtn = playScreen.transform.Find("Deflect Btn").GetComponent<Button>();
        defBtn.onClick.AddListener(Deflect);

        viewPos = cam.WorldToViewportPoint(transform.position);
        maxEnemies = PlayerPrefs.GetInt("currentLevel");
        spawnAfter = 7 - PlayerPrefs.GetInt("currentLevel");
        StartCoroutine(AutoSpawnEnemies());
    }

 
    private bool Spawn()
    {
        for (int i = 0; i < 10; i++)
        {
            // A random position in viewport
            Vector3 viewPortPos = new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.6f, 0.8f), viewPos.z + 43);
            Vector3 worldPos = cam.ViewportToWorldPoint(viewPortPos);

            if (!Physics.CheckSphere(worldPos, 11) && totalEnemies < maxEnemies)
            {
                GameObject enemy = Instantiate(enemyPrefab, worldPos, transform.rotation);
                Transform smoke = enemy.transform.Find("smoke");
                smoke.transform.rotation = new Quaternion(0.0f, cam.transform.rotation.y, 0.0f, cam.transform.rotation.w);
                smoke.gameObject.SetActive(true);
                enemy.gameObject.SetActive(true);
                totalEnemies++;
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
        currDeflectCD = deflectCD;

        Collider[] bullets = Physics.OverlapSphere(transform.position, 20f);

        foreach (Collider bullet in bullets)
        {
            if (bullet.tag.Equals("Bullet"))
            {
                print(bullet);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                bullet.transform.Find("kunai1").Rotate(180, 0, 0);
                bullet.transform.Find("kunai2").Rotate(180, 0, 0);
                rb.velocity = -rb.velocity;
                bullet.tag = "DeflectedBullet";
            }
        }


    }

    void DisplayScore()
    {
        score = (hitEnemies / totalEnemies) * 100;
        score -= (missedEnemies * 5);
        scoreTxt.text = "Score: " + (int) score;
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

    void EndLevel()
    {
        GameObject canvas = GameObject.Find("Canvas");
        UI uiScript = canvas.GetComponent<UI>();

        Transform resultScreen = canvas.transform.Find("Score").transform.Find("ScorePanel");

        if (score >= 25)
        {
            int level = PlayerPrefs.GetInt("currentLevel");
            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevels");
            print("Ubl " + unlockedLevel);
            if ((unlockedLevel < 6) && (level == unlockedLevel))
            PlayerPrefs.SetInt("UnlockedLevels", ++unlockedLevel);
        }

        resultScreen.Find("NextLvl").gameObject.SetActive(score >= 25);
        resultScreen.Find("leftStar").gameObject.SetActive(score >= 25);
        resultScreen.Find("middleStar").gameObject.SetActive(score >= 50);
        resultScreen.Find("rightStar").gameObject.SetActive(score == 100);

        uiScript.OnClickMenuBtn(4);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        Destroy(gameObject);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {  
        DisplayTime();
        DisplayScore();

        if (timeToDisplay <= 0 || missedEnemies + hitEnemies == maxEnemies)
        {
            EndLevel();
        }

        if (!canDeflect)
        {
            if (currDeflectCD > 0)
            {
                currDeflectCD -= Time.deltaTime;
            }
            else
            {
                canDeflect = true;
                currDeflectCD = 0;
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
