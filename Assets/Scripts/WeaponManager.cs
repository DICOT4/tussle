using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private PlayerWeapon primaryWeapon;
    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    private PlayerWeapon currentWeapon;
    private WeaponGraphics weaponGraphics;

    // Start is called before the first frame update
    void Start() {
        EquipWeapon (primaryWeapon);
    }

    // Update is called once per frame
    void Update() {
        
    }

    void EquipWeapon (PlayerWeapon weapon) {
        currentWeapon = weapon;
        GameObject weaponIns = Instantiate (weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent (weaponHolder);

        weaponGraphics = weaponIns.GetComponent<WeaponGraphics> ();
        if ( weaponGraphics == null )
            Debug.Log ("No weapon graphics available on the weapon object: " + weaponIns.name);

        if (isLocalPlayer) {
            weaponIns.layer = LayerMask.NameToLayer (weaponLayerName);
        }
    }

    public PlayerWeapon getCurrentWeapon () {
        return currentWeapon;
    }

    public WeaponGraphics getWeaponGraphics () {
        return weaponGraphics;
    }
}
