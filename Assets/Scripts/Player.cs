using UnityEngine.Networking;
using UnityEngine;

public class Player : NetworkBehaviour
{

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    private void Awake () {
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
        currentHealth = maxHealth;
    }

    public void TakeDamage (int amount) {
        currentHealth -= amount;

        Debug.Log (transform.name + " now has " + currentHealth + " health.");
    }
}
