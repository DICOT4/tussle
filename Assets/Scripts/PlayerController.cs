using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;


    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;


    public float getThrusterFuelAmount() {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header ("Spring Settings")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private void Start () {
        motor = GetComponent<PlayerMotor> ();
        joint = GetComponent<ConfigurableJoint> ();
        animator = GetComponent<Animator> ();

        SetJointSettings (this.jointSpring);
    }

    private void Update () {

        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else {

            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        
        // Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxis ("Horizontal");
        float zMov = Input.GetAxis ("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        // final movement vector
        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        // Animate movement
        animator.SetFloat ("ForwardVelocity", zMov);

        // apply movement
        motor.Move (velocity);

        // Calculate rotation as a 3D vector (turning around)
        float yRot = Input.GetAxisRaw ("Mouse X");
        Vector3 rotation = new Vector3 (0f, yRot, 0f) * lookSensitivity;
        // Apply rotation
        motor.Rotate (rotation);
        
        // Calculate camera rotation as a 3D vector (look up and down)
        float xRot = Input.GetAxisRaw ("Mouse Y");
        float cameraRotationX = xRot * lookSensitivity;
        // Apply camera rotation
        motor.RotateCamera (cameraRotationX);

        // Calculate thruster force based on player input
        Vector3 thrusterForce = Vector3.zero;
        if (Input.GetButton ("Jump") && thrusterFuelAmount > 0f) {

               thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
                thrusterForce = Vector3.up * this.thrusterForce;
                SetJointSettings(0f);
            }
        } else {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings (this.jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        // Apply thruster force
        motor.ApplyThruster (thrusterForce);

    }

    private void SetJointSettings (float jointSpring) {
        joint.yDrive = new JointDrive {
            positionSpring = jointSpring,
            maximumForce = jointMaxForce
        };
    }

}
