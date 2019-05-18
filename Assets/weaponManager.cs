using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class weaponManager : NetworkBehaviour
{


    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private string weaponLayerName = "Weapon";

   


    [SerializeField]
    private PlayerWeapon primaryWeapon;
    private WeaponGraphics currentGraphics;

   

    private PlayerWeapon currentWeapon;
   
    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerWeapon getCurrWeapon() {

        return currentWeapon;
    }


    public WeaponGraphics getCurrGraphics()
    {

        return currentGraphics;
    }

    void EquipWeapon(PlayerWeapon _weapon) {
        currentWeapon = _weapon;

       GameObject _weaponIns =  (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();

        if (currentGraphics == null){

            Debug.LogError("Graphics not present");
        }

        if (isLocalPlayer)
            Util.setLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }
}
