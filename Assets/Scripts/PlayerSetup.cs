using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";
    [SerializeField]
    private string dontDrawLayerName = "DontDraw";
    [SerializeField]
    private GameObject playerGraphics;
    [SerializeField]
    private GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    Camera sceneCamera;

    private void Start () {
        // Disable components that should only be
        // active on the player that we control
        if (!isLocalPlayer) {
            DisableComponents ();
            AssignRemoteLayer ();
        } else {
            // We are local player: disable scene camera
            sceneCamera = Camera.main;
            if (sceneCamera != null)
                sceneCamera.gameObject.SetActive (false);
            // Disable Player graphics for local player
            SetLayerRecursively (playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create PlayerUI
            playerUIInstance = Instantiate (playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }

        GetComponent<Player> ().Setup ();
    }

    private void SetLayerRecursively (GameObject obj, int newlayer) {
        obj.layer = newlayer;

        foreach ( Transform child in obj.transform )
            SetLayerRecursively (child.gameObject, newlayer);
    }

    public override void OnStartClient () {
        base.OnStartClient ();

        string netID = GetComponent<NetworkIdentity> ().netId.ToString();
        Player player = GetComponent<Player> ();
        GameManager.RegisterPlayer (netID, player);
    }

    private void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
    }

    private void DisableComponents () {
        for ( int i = 0; i < componentsToDisable.Length; i++ ) {
            componentsToDisable[i].enabled = false;
        }
    }

    // Called when the player is destroyed
    private void OnDisable () {
        Destroy (playerUIInstance);

        // Re-enable the scene camera
        if ( sceneCamera != null )
            sceneCamera.gameObject.SetActive (true);

        GameManager.UnRegisterPlayer (transform.name);
    }

}
