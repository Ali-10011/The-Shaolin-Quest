using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody _rb;
    int bulletSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
