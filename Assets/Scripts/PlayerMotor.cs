using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationOnX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per phyiscs iteration
    public void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    // Gets a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;    
    }    
    
    // Gets a rotation vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;    
    }
        
    // Gets a Camera rotation vector
    public void CameraRotate(float _cameraRotation)
    {
        cameraRotationX = _cameraRotation;    
    }


    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
        }
        if (thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.deltaTime, ForceMode.Acceleration);
        }


    }

    void PerformRotation()
    {
        rb.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
        if(cam != null)
        {
            // Set our rotation and clamp it
            currentCameraRotationOnX -= cameraRotationX;
            currentCameraRotationOnX = Mathf.Clamp(currentCameraRotationOnX, -cameraRotationLimit, cameraRotationLimit);

            // Apply the rotation to the camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationOnX, 0f, 0f);
        }
            
    }

    //Get the Thruster Force
    public void ApplyThrusterForce(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;

    }
}
