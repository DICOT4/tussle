using UnityEngine.Networking;
using UnityEngine;



[RequireComponent (typeof(weaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";


    private PlayerWeapon currentWeapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private weaponManager weaponManager;

    // Start is called before the first frame update
    void Start () {
        if ( cam == null ) {
            Debug.LogError ("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }

        weaponManager = GetComponent<weaponManager>();
    }

    // Update is called once per frame
    void Update () {
        currentWeapon = weaponManager.getCurrWeapon();
        if (currentWeapon.fireRate <= 0f) {
            if (Input.GetButtonDown("Fire1")){
                Shoot();
            }
        }
        else {
            if (Input.GetButtonDown("Fire1")){
                InvokeRepeating("shoot", 0f, 1f/ currentWeapon.fireRate);
            } else if (Input.GetButtonUp("Fire1")) {
                CancelInvoke("Shoot");
            } 
        }   
    }

    [Command]
   void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    [Command]

    void CmdOnHit (Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    [ClientRpc]

    void RpcDoShootEffect()
    {
//        Debug.Log("Init");
        weaponManager.getCurrGraphics().MuzzleFlash.Play();
    }

    [ClientRpc]

    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
         GameObject _hitEffect = (GameObject)Instantiate(weaponManager.getCurrGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
         Destroy(_hitEffect, 2f);
    }
    [Client]
    private void Shoot () {

        if (!isLocalPlayer) { 
        return; 
        }

        CmdOnShoot();

        Debug.Log("Shoot!");
        RaycastHit hit;
        currentWeapon = weaponManager.getCurrWeapon();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)) {
            // We hit something
            if (hit.collider.tag == PLAYER_TAG) {
                CmdPlayerShot (hit.collider.name, currentWeapon.damage);
            }

            CmdOnHit(hit.point, hit.normal); 
       }

    }

    [Command]
    private void CmdPlayerShot (string playerID, int damage) {
        Debug.Log (playerID + " has been shot.");

        Player player = GameManager.GetPlayer (playerID);
        player.RpcTakeDamage (damage);
    }



}
