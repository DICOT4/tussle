using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

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
    private bool[] wasEnabled;

    public void Setup () {
        wasEnabled = new bool[disableOnDeath.Length];
        for ( int i = 0; i < wasEnabled.Length; i++ ) {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults ();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDefaults () {
        isDead = false;
        currentHealth = maxHealth;
        for ( int i = 0; i < disableOnDeath.Length; i++ ) {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider col = GetComponent<Collider> ();
        if (col != null)
            col.enabled = true;
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

        Collider col = GetComponent<Collider> ();
        if ( col != null )
            col.enabled = false;

        Debug.Log (transform.name + " is dead.");

        StartCoroutine (Respawn());

    }

    private IEnumerator Respawn () {
        yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);
        SetDefaults ();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition ();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        Debug.Log (transform.name + " respawned.");
    }
}
