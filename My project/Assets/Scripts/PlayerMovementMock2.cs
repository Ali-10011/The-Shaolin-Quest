using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementMock2 : MonoBehaviour
{
    GameObject pivot;
    public bool isMoving = false;
    float oldRotation;
    [SerializeField] float rotationAngle = 45f;
    [SerializeField] float rotateSpeed = 1f;
    SphereCollider deflectArea;

    public static List<GameObject> bulletsList = new List<GameObject>();
    public static List<GameObject> bulletsToDelete = new List<GameObject>();
    private void Awake() 
    {
        pivot = GameObject.FindGameObjectWithTag("Pivot"); 
        oldRotation = transform.rotation.y;
        deflectArea = GetComponent<SphereCollider>();
    }

    private IEnumerator Rotate(bool isLeft)
    {
        Vector3 rotateDirection = isLeft ? Vector3.up : - Vector3.up; 
        isMoving = true;

        Quaternion startRotation = transform.rotation;
        Vector3 startPosition = transform.position;
        // Get end position;
        transform.RotateAround(pivot.transform.position, rotateDirection, rotationAngle);
        Quaternion endRotation = transform.rotation;
        Vector3 endPosition = transform.position;
        transform.rotation = startRotation;
        transform.position = startPosition;
 
        float rate = rotationAngle / rotateSpeed;
        //Start Rotate
        for (float i = 0.0f; Mathf.Abs(i) < Mathf.Abs(rotationAngle); i += Time.deltaTime * rate)
        {
            transform.RotateAround(pivot.transform.position, rotateDirection, Time.deltaTime * rate);
            yield return null;
        }
 
        transform.rotation = endRotation;
        transform.position = endPosition;
        isMoving = false;
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        bulletsList.Add(other.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && !isMoving)
        {
            StartCoroutine(Rotate(false));   
        }
        else if (Input.GetKeyDown(KeyCode.A) && !isMoving)
        {
            StartCoroutine(Rotate(true));   
        }

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bulletsToDelete.Clear();

            foreach (GameObject bullet in bulletsList)
            {
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.velocity = -rb.velocity;
            }

            foreach (GameObject bullet in bulletsToDelete)
            {
                bulletsList.Remove(bullet);
            }
        }
        
    }
}
