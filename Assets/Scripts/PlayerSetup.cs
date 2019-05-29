using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

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
        }

        GetComponent<Player> ().Setup ();
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
        // Re-enable the scene camera
        if ( sceneCamera != null )
            sceneCamera.gameObject.SetActive (true);

        GameManager.UnRegisterPlayer (transform.name);
    }

}
