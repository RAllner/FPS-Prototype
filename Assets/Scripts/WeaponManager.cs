/*
* Copyright (c) Raphael Allner
* www.raphaelallner.de
*/

using System;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
	#region Variables
	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponHolder;

	[SerializeField]
	private Weapon primaryWeapon;

	private Weapon currentWeapon;

	private WeaponGraphics currentWeaponGraphics;
	#endregion
	
	#region Methods
    void Start()
    {
		EquipWeapon(primaryWeapon);
		
	}

    void Update()
    {
        
    }
	public Weapon GetCurrentWeapon()
	{
		return currentWeapon;
	}
	public WeaponGraphics GetCurrentWeaponGraphics()
	{
		return currentWeaponGraphics;
	}
	void EquipWeapon(Weapon _weapon)
	{
		currentWeapon = _weapon;

		// Instantiate a new weapon with the weaponHolder as a parrent Object
		GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation, weaponHolder);

		currentWeaponGraphics = _weaponIns.GetComponent<WeaponGraphics>();

		if (currentWeaponGraphics == null)
		{
			Debug.LogError("No WeaponGraphics component on the weapin object: " + _weaponIns.name);
		}

		if (isLocalPlayer)
			Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
		
	}


	#endregion
}
