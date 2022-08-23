using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject bulletPrefab = null;
    private Camera cam;
    private Vector3 viewPos;

    float timer;
    [SerializeField] int waitingTime = 5;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");   
        cam = Camera.main;
        viewPos = cam.WorldToViewportPoint(player.transform.position);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
        Bullet _bltScript= bullet.GetComponent<Bullet>();
        _bltScript.Shoot(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
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
                    Shoot();
                    timer = 0;
                }
            }
        }
        catch
        {
            timer += Time.deltaTime;

                if (timer > waitingTime)
                {
                    Shoot();
                    timer = 0;
                }
        }
    }
}
