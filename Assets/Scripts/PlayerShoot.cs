using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

	#region Variables
	private const string PLAYER_TAG = "Player";

    [SerializeField]
	[Tooltip("Mask which Layer to raycast on")]
    private LayerMask mask;

    [SerializeField]
    private Camera cam;

	[Header("Weapon Settings")]
	private Weapon currentWeapon;

	[SerializeField]
	private WeaponManager weaponManager;
	#endregion

	#region Methods

	// Start is called before the first frame update
	void Start()
    {
        if(cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
		weaponManager = GetComponent<WeaponManager>();
	}

    // Update is called once per frame
    void Update()
    {
		currentWeapon = weaponManager.GetCurrentWeapon();
		if (currentWeapon.fireRate <= 0f)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				Shoot();
			}
		}
		else
		{
			if (Input.GetButtonDown("Fire1"))
			{
				// Invoke Repeating (which Method should be repeated, when to start in sec., repeatRate)
				InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
			}
			else if (Input.GetButtonUp("Fire1"))
			{
				CancelInvoke("Shoot");
			}
		}


    }

    [Client]
    void Shoot()
    {
		if (!isLocalPlayer)
		{
			return;
		}

		// We are shooting call the method CmdOnShoot on the server
		CmdOnShoot();

        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {

			if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
			// We hit something: Call the OnHit Method on the server
			CmdOnHit(_hit.point, _hit.normal);
        }
    }

	// Is called on alle clients when we need to do a shoot effect
	[ClientRpc]
	void RpcDoShootEffect()
	{
		weaponManager.GetCurrentWeaponGraphics().muzzleFlash.Play();
	}

	// Is called on all clients 
	// Here we can spawn cool Hiteffects
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
	{
		Debug.Log("Effect");
		GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentWeaponGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
		Destroy(_hitEffect, 2f);
	}


	// Is called on the server if we hit something
	[Command]
	void CmdOnHit(Vector3 _pos, Vector3 _normal)
	{
		Debug.Log("Shoot");
		RpcDoHitEffect(_pos, _normal);
	}


	// This method is called whenever a player shoots
    [Command]
	void CmdOnShoot()
	{
		RpcDoShootEffect();
	}

    [Command]
    void CmdPlayerShot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + " has been shot.");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
	#endregion
}
