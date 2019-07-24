using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
	[Header("Controls:")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

	[Space]

	[Header("Thruster Settings:")]
	[SerializeField]
    private float thrusterForce = 1300f;
	[SerializeField]
	private float thrusterFuelBurnSpeed = 0.8f;
	[SerializeField]
	private float thrusterFuelRegenSpeed = 0.3f;
	[SerializeField]
	private float thrusterFuelFill = 1f;

    [Header("Spring settings:")]

    [SerializeField] 
    private JointProjectionMode jointMode = JointProjectionMode.PositionAndRotation;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;
	[SerializeField]
	private LayerMask environmentMask;

    // Caching Components
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    // Update is called once per frame
    void Update()
    {
		// Setting target position for spring
		// This makes the physics act right when it comes to
		// applying gravity when flying over objects
		//RaycastHit _hitUnderThePlayer;
		//if (Physics.Raycast(transform.position, Vector3.down, out _hitUnderThePlayer, 100f, environmentMask))
		//{
		//	joint.targetPosition = new Vector3(0f, -_hitUnderThePlayer.point.y, 0f);
		//} else
		//{ 
		//	joint.targetPosition = new Vector3(0f, 0f, 0f);
		//}

        //calculate movement velocity as a 3D Vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        // Final movement Vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        // Animate Movement
        animator.SetFloat("ForwardVelocity", _zMov);

        motor.Move(_velocity);
        
        // Calculate rotation as a 3D Vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;
        motor.Rotate(_rotation);


        // Calculate camera roation as 3D Vector
        float _xRot = Input.GetAxisRaw("Mouse Y");
        float _cameraRotation = _xRot * lookSensitivity;
        motor.CameraRotate(_cameraRotation);


        // Calculate the Thruster Force depending on player input
        Vector3 _thrusterForce = Vector3.zero;

        if (Input.GetButton("Jump") && thrusterFuelFill > 0f)
        {
			thrusterFuelFill -= thrusterFuelBurnSpeed * Time.deltaTime;
			if(thrusterFuelFill >= 0.1f) { 
				_thrusterForce = Vector3.up * thrusterForce;
				SetJointSettings(0f);
			}
		}
        else
        {
			thrusterFuelFill += thrusterFuelRegenSpeed * Time.deltaTime;
			SetJointSettings(jointSpring);
        }

		thrusterFuelFill = Mathf.Clamp(thrusterFuelFill, 0f, 1f);
        // Apply the thruster force
        motor.ApplyThrusterForce(_thrusterForce);

    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            mode = (UnityEngine.JointDriveMode)jointMode,
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }

	public float GetThrusterFuelFill()
	{
		return thrusterFuelFill;
	}

	//public float GetCurrentHealth()
	//{
	//	return ;
	//}
}
