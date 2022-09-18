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
    [SerializeField] GameObject deflectPrefab;
    public Animator playerAnim;

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

    public float totalEnemies = -1;
    public float hitEnemies = 0;
    public float missedEnemies = 0;
    public int gotHit = 0;
    public float score = 0;

    
    // Start is called before the first frame update
    private void Awake() 
    {
        cam = Camera.main;

        playerAnim = transform.Find("panda").GetComponent<Animator>();
        playScreen = GameObject.Find("Canvas").transform.Find("PlayScreen").gameObject;

        timeTxt = playScreen.transform.Find("TimeTxt").GetComponent<TextMeshProUGUI>();
        scoreTxt = playScreen.transform.Find("ScoreTxt").GetComponent<TextMeshProUGUI>();

        FixedJoystick = playScreen.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        defBtn = playScreen.transform.Find("Deflect Btn").GetComponent<Button>();
        defBtn.onClick.AddListener(Deflect);

        viewPos = cam.WorldToViewportPoint(transform.position);
        maxEnemies = PlayerPrefs.GetInt("currentLevel") + 2;
        spawnAfter = 7 - PlayerPrefs.GetInt("currentLevel");
        StartCoroutine(AutoSpawnEnemies());
    }

 
    private bool Spawn()
    {
        for (int i = 0; i < 10; i++)
        {
            // A random position in viewport
            Vector3 viewPortPos = new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.6f, 0.8f), 73);
            Vector3 worldPos = cam.ViewportToWorldPoint(viewPortPos);

            if (!Physics.CheckSphere(worldPos, 11) && totalEnemies < maxEnemies)
            {
                GameObject enemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
                Transform smoke = enemy.transform.Find("smoke");
                smoke.transform.rotation = new Quaternion(0.0f, cam.transform.rotation.y, 0.0f, cam.transform.rotation.w);
                smoke.gameObject.SetActive(true);
                enemy.gameObject.SetActive(true);
                totalEnemies++;
                if (totalEnemies == 0)
                {
                    Destroy(enemy);
                    totalEnemies--;
                }
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

        ParticleSystem defPart = Instantiate(deflectPrefab, transform).GetComponent<ParticleSystem>();
        defPart.Play();

        canDeflect = false;
        defBtn.interactable = false;
        currDeflectCD = deflectCD;

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

    void DisplayScore()
    {
        score = (hitEnemies / totalEnemies) * 100;
        score -= (missedEnemies * 5);
        score -= (gotHit * 5);
        score = Mathf.Clamp(score, 0, 100);
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

        if (timeToDisplay < 1 || missedEnemies + hitEnemies == maxEnemies)
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

        // Calculate final position in advance before movement
        Vector3 newPos = transform.position + transform.TransformDirection(new Vector3(x: FixedJoystick.Horizontal * PlayerSpeed, y: 0, z: 0));
        newPos = cam.WorldToViewportPoint(newPos);
        
        // If final position is inside movement, move, otherwise do not
        if ((0.07f < newPos.x && newPos.x < 0.93f))
        {
            float mov = FixedJoystick.Horizontal * PlayerSpeed;
            
            if (mov < 0.0f)
            {
                transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            }
            else if (mov > 0.0f)
            {
                transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            playerAnim.SetBool("isRunning", mov != 0);
            transform.Translate(translation: new Vector3(x:mov, y:0, z:0), Space.World);
        }
    }
}
