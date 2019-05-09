using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    private Rigidbody rb;

    private void Start () {
        rb = GetComponent<Rigidbody> ();
    }

    // Gets a movement vector
    public void Move (Vector3 velocity) {
        this.velocity = velocity;
    }

    // Gets a rotational vector
    public void Rotate (Vector3 rotation) {
        this.rotation = rotation;
    }
    
    // Gets a rotational vector for camera
    public void RotateCamera (float cameraRotationX) {
        this.cameraRotationX = cameraRotationX;
    }

    // Get a force vector for our thruster
    public void ApplyThruster (Vector3 thrusterForce) {
        this.thrusterForce = thrusterForce;
    }

    private void FixedUpdate () {
        PerformMovement ();
        PerformRotation ();
    }

    private void PerformMovement () {
        if (velocity != Vector3.zero) {
            rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero) {
            rb.AddForce (thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    private void PerformRotation () {
        rb.MoveRotation (rb.rotation * Quaternion.Euler (this.rotation));

        if (cam != null) {
            // Set our rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            // Set our rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0f, 0f);
        }
    }

}
