using UnityEngine;
using System.Collections;


public class Enemy : MonoBehaviour
{
    private GameObject player;
    private PlayerMovementMock1 playerScript;
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private GameObject smoke = null;
    float timer;
    [SerializeField] int waitingTime;
    [SerializeField] int MaxMissedBullets;
    public int missedBullets = 0;
    public Animator animator;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerMovementMock1>();
        player = player.transform.Find("panda").gameObject;
        MaxMissedBullets = 7 - PlayerPrefs.GetInt("currentLevel");
        waitingTime = 5;
    }

    public void OnHit()
    {
        GameObject smok = Instantiate(smoke);
        smok.transform.parent = null;
        smok.transform.position = transform.position;
        smok.GetComponent<ParticleSystem>().Play();
        Destroy(transform.parent.gameObject);
        playerScript.hitEnemies++;
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.parent.Find("shoot").position, transform.rotation);
        bullet.transform.SetParent(transform);
        Bullet _bltScript= bullet.GetComponent<Bullet>();
        _bltScript._enemy = this;
        _bltScript.transform.LookAt(player.transform);
        _bltScript.Shoot(player.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
            GameObject.FindGameObjectWithTag("Player");

        if (missedBullets > MaxMissedBullets)
        {
            Destroy(transform.parent.gameObject);
            playerScript.missedEnemies++;
        }
        
        timer += Time.deltaTime;

        if (timer > waitingTime)
        {
            animator.Play("Base Layer.Throw", -1);
            timer = 0;
        }
    }
}
