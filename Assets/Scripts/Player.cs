using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    private bool _isDead = false;

    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [Header("Death Behaviour")]

    [SerializeField]
    private Behaviour[] behavioursToDisableOnDeath;
    private bool[] wasEnabled;

	[SerializeField]
	GameObject[] disableGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;


    public void PlayerSetup()
    {
		// Switch camera
		GameManager.instance.SetOverviewCameraActive(false);
		GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);

		CmdBroadCastNewPlayerSetup();

	}

	[Command]
	private void CmdBroadCastNewPlayerSetup()
	{
		RpcSetupPlayerOnAllClients();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients()
	{
		wasEnabled = new bool[behavioursToDisableOnDeath.Length];
		for (int i = 0; i < wasEnabled.Length; i++)
		{
			wasEnabled[i] = behavioursToDisableOnDeath[i].enabled;
		}
		SetDefaults();
	}

    public void SetDefaults()
    {
        currentHealth = maxHealth;
		_isDead = false;

		for (int i = 0; i < behavioursToDisableOnDeath.Length; i++)
        {
            // Alle Behaviour außer Collider -> Collider erben nicht von Behaviour
            behavioursToDisableOnDeath[i].enabled = wasEnabled[i];
        }

        // Collider erben von Component nicht von Behaviour, können aber disabled werden
        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true;
        }

		// Enable GameObjects on Death
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
		{
			disableGameObjectsOnDeath[i].SetActive(true);
		}

		//Spawn Effect
		GameObject _gfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);

	}

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= _amount;
		
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
	
        // Disable Components
        for (int i = 0; i < behavioursToDisableOnDeath.Length; i++)
        {
            behavioursToDisableOnDeath[i].enabled = false;
        } 
		
		// Disable GameObjects on Death
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
			disableGameObjectsOnDeath[i].SetActive(false);
        }

		//Disable Collider
		Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

		//Dead Effect
		GameObject _gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 3f);

		// If we are the local player - switch camera to overviewCamera
		if (isLocalPlayer)
		{
			GameManager.instance.SetOverviewCameraActive(true);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
		}

		Debug.Log(transform.name + " is DEAD!");

        // Call respawn method
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

		// Switch cameras
		GameManager.instance.SetOverviewCameraActive(false);
		GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);

		SetDefaults();

		Debug.Log(transform.name + " respawned");
    }


	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.K))
		{
			RpcTakeDamage(9999);
		}
	}
}
