using UnityEngine;
using System.Collections;


public class Enemy : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject bulletPrefab = null;
    float timer;
    [SerializeField] int waitingTime = 5;
    [SerializeField] int MaxMissedBullets = 3;
    public int missedBullets = 0;
    public Animator animator;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player = player.transform.Find("panda").gameObject;
    }

    public void OnHit()
    {
        Destroy(transform.parent.gameObject);
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
    void Update()
    {
        if (missedBullets > MaxMissedBullets)
        {
            Destroy(gameObject);
        }
        try 
        {
            PlayerMovementMock2 playerScript = player.GetComponent<PlayerMovementMock2>();
            if (playerScript.isMoving)
                {
                    transform.LookAt(player.transform.position);
                }
            else 
            {
                timer += Time.deltaTime;

                if (timer > waitingTime)
                {
                    animator.Play("Base Layer.Throw", -1);
                    timer = 0;
                }
            }
        }
        catch
        {
            timer += Time.deltaTime;

                if (timer > waitingTime)
                {
                    animator.Play("Base Layer.Throw", -1);
                    timer = 0;
                }
        }
    }
}
