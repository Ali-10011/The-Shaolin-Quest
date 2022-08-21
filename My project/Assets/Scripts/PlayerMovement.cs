using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float PlayerSpeed = 1f;
    public FixedJoystick FixedJoystick;

    private Camera cam;

    // Start is called before the first frame update
    private void Awake() 
    {
        cam = Camera.main;
    }
    void Start()
    {
        
    }

    void SpawnCube()
    {
        bool spawned = false;
        int tries = 0;

        while (!spawned || tries != 10)
        {
            // A random position in viewport
            Vector3 viewPortPos = new Vector3(Random.Range(0.1f, 0.4f), Random.Range(0.1f, 0.4f), transform.position.z);
            Vector3 worldPos = cam.ViewportToWorldPoint(viewPortPos);

            tries++;
            if (!Physics.CheckSphere(worldPos, 1))
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = worldPos;
                spawned = true;
            }
        }
    }
    // Update is called once per frame
    // TO DO: Create a better mechanic after finalizing mechanics,
    // Current impl only works for slow character speeds
    void Update()
    {
        /*if (Input.GetKey(KeyCode.T))
        {
            SpawnCube();
        }*/
        // Get Poisition of object relative to camera's viewport Range is bw 0 and 1
        // 0 means extreme left on x axis, 1 means extreme right on x-axis
        // 0 means bottom on y axis, 1 means top on y-axis
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        
        Debug.Log(FixedJoystick.Horizontal*PlayerSpeed);
        // Only move object when within the given bounds (viewport of the camera)
        // To prevent a part of object going out of viewport, a larger/smaller value is chosen
        // instead of 0 and 1.
        if ((0.07f < viewPos.x && viewPos.x < 0.93f) && (0.07f < viewPos.y && viewPos.y < 0.93f))
        {
            transform.Translate(translation: new Vector3(x:FixedJoystick.Horizontal*PlayerSpeed,y:FixedJoystick.Vertical*PlayerSpeed,z:0));
        }

        // If not within the bounds of viewport, move it back inside viewport 
        else 
        {
            // Clamp back the value between the ranges
            viewPos.x = Mathf.Clamp(viewPos.x, 0.08f, 0.92f);
            viewPos.y = Mathf.Clamp(viewPos.y, 0.08f, 0.92f);

            Vector3 worldPos = cam.ViewportToWorldPoint(viewPos);

            // Move back smoothly inside the viewport
            transform.position = Vector3.MoveTowards(current: transform.position, target: new Vector3(worldPos.x, worldPos.y, transform.position.z), maxDistanceDelta: 0.009f);
        }
    }
}
