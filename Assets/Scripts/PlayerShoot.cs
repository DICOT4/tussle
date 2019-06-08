using UnityEngine.Networking;
using UnityEngine;

[RequireComponent (typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";
    
    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

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
        weaponManager = GetComponent<WeaponManager> ();
    }

    // Update is called once per frame
    void Update () {
        currentWeapon = weaponManager.getCurrentWeapon ();
        if (currentWeapon.fireRate <= 0f) {
            if ( Input.GetButtonDown ("Fire1") )
                Shoot ();
        }
        else {
            if ( Input.GetButtonDown ("Fire1") )
                InvokeRepeating ("Shoot", 0f, 1f / currentWeapon.fireRate);
            else if (Input.GetButtonUp("Fire1"))
                CancelInvoke ("Shoot");
        }
    }

    [Client]
    private void Shoot () {
        Debug.Log ("Shoot");

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)) {
            // We hit something
            if (hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot (hit.collider.name, currentWeapon.damage);
            }
        }

    }

    [Command]
    private void CmdPlayerShot (string playerID, int damage) {
        Debug.Log (playerID + " has been shot.");

        Player player = GameManager.GetPlayer (playerID);
        player.RpcTakeDamage (damage);
    }
}
