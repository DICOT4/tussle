using UnityEngine.Networking;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    // Start is called before the first frame update
    void Start () {
        if ( cam == null ) {
            Debug.LogError ("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Fire1")) {
            Shoot ();
        }
    }

    [Client]
    private void Shoot () {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask)) {
            // We hit something
            if (hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot (hit.collider.name, weapon.damage);
            }
        }

    }

    [Command]
    private void CmdPlayerShot (string playerID, int damage) {
        Debug.Log (playerID + " has been shot.");

        Player player = GameManager.GetPlayer (playerID);
        player.TakeDamage (damage);
    }
}
