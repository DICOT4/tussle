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
        if (Input.GetButton ("Jump")) {
            thrusterForce = Vector3.up * this.thrusterForce;
            SetJointSettings (0f);
        } else {
            SetJointSettings (this.jointSpring);
        }

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
