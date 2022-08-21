using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float PlayerSpeed = 1f;
    private Vector3 viewPos;
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

    private bool Spawn()
    {
        for (int i = 0; i < 10; i++)
        {
            // A random position in viewport
            Vector3 viewPortPos = new Vector3(Random.Range(0.1f, 0.9f), Random.Range(0.6f, 0.8f), viewPos.z);
            Vector3 worldPos = cam.ViewportToWorldPoint(viewPortPos);

            if (!Physics.CheckSphere(worldPos, 1))
            {
                GameObject self = GameObject.CreatePrimitive(PrimitiveType.Cube);
                self.transform.localScale = transform.localScale;
                self.transform.position = worldPos;
                self.AddComponent<Enemy>();
                return true;
            }
        }   
        return false;
    }

    // Update is called once per frame
    // TO DO: Create a better mechanic after finalizing mechanics,
    // Current impl only works for slow character speeds
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Spawn();
        }
        // Get Poisition of object relative to camera's viewport Range is bw 0 and 1
        // 0 means extreme left on x axis, 1 means extreme right on x-axis
        // 0 means bottom on y axis, 1 means top on y-axis
        viewPos = cam.WorldToViewportPoint(transform.position);
        
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
