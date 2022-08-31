using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody _rb;
    Camera _cam;
    int bulletSpeed;
    public Enemy _enemy;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        bulletSpeed = 40;
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (!other.collider.isTrigger)
        {
            Destroy(gameObject);
            PlayerMovementMock2.bulletsList.Remove(gameObject);
        }    
    }
    public void Shoot(Vector3 target)
    {
        _rb.AddForce((target - transform.position) * bulletSpeed);
    }

    private bool CheckInCam()
    {
        Vector3 pos = gameObject.transform.position;
        Vector3 posInViewport = _cam.WorldToViewportPoint(pos);

        if ((posInViewport.y < 0 || posInViewport.y > 1) || (posInViewport.x < 0 || posInViewport.x > 1))
        {
            _enemy.missedBullets++;
            return false;
        }
        else
            return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckInCam())
        {
            Destroy(gameObject);
        }
    }
}
