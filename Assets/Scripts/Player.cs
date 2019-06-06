using UnityEngine.Networking;
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SyncVar]
    private bool isDead = false;
    public bool IsDead { get => isDead; protected set => isDead = value; }

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    private bool firstSetup = true;

    private bool[] wasEnabled;
    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private GameObject respawnEffect;

    public void SetupPlayer () {
        if (isLocalPlayer)
        {
            GameManager.instance.setSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
            CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup(){

        RpcStupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcStupPlayerOnAllClients(){

        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }

        SetDefaults();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
 /*   void Update()
    {
        if (!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(99999);
        }
    }
    */
    public void SetDefaults () {
        isDead = false;
        currentHealth = maxHealth;
        for ( int i = 0; i < disableOnDeath.Length; i++ ) {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

       

        Collider col = GetComponent<Collider> ();
        if (col != null)
            col.enabled = true;


        GameObject gfxIns = (GameObject)Instantiate(respawnEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 1f);
    }

    [ClientRpc]
    public void RpcTakeDamage (int amount) {
        if ( !isDead ) {
            currentHealth -= amount;
            Debug.Log (transform.name + " now has " + currentHealth + " health.");
            if (currentHealth <= 0) {
                Die ();
            }
        }
    }

    private void Die () {
        isDead = true;

        for ( int i = 0; i < disableOnDeath.Length; i++ ) {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        Collider col = GetComponent<Collider> ();
        if ( col != null )
            col.enabled = false;

        GameObject gfxIns = (GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gfxIns, 3f);

        if (isLocalPlayer) {
            GameManager.instance.setSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log (transform.name + " is dead.");

        StartCoroutine (Respawn());

    }

    private IEnumerator Respawn () {
        yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);
       
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition ();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);
        SetupPlayer();

        Debug.Log (transform.name + " respawned.");
    }
}
