using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody _rb;
    Camera _cam;
    [SerializeField] float bulletSpeed;
    public Enemy _enemy;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
        bulletSpeed = 30 * (PlayerPrefs.GetInt("currentLevel") / 10);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.collider.transform.root.tag.Contains("Player"))
        {
            Destroy(gameObject);
        }
        else if (other.collider.tag.Contains("Bullet"))
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), other.collider);
        }
        else if (!other.collider.isTrigger)
        {
            Destroy(gameObject);
            other.transform.GetComponentInChildren<Enemy>().animator.Play("Base Layer.Hit", -1);
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
    void FixedUpdate()
    {
        if (!CheckInCam())
        {
            Destroy(gameObject);
        }
    }
}
