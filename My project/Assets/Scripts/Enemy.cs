using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private GameObject player;
    private Camera cam;
    private Vector3 viewPos;

    float timer;
    [SerializeField] int waitingTime = 5;
    [SerializeField] int bulletSpeed = 25;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");   
        cam = Camera.main;
        viewPos = cam.WorldToViewportPoint(player.transform.position);
    }

    void Shoot()
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        bullet.transform.position = transform.position + (-transform.up);
        Rigidbody b = bullet.AddComponent<Rigidbody>();
        b.useGravity = false;
        b.AddForce((player.transform.position - bullet.transform.position) * bulletSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > waitingTime)
        {
            Shoot();
            timer = 0;
        }
    }
}
