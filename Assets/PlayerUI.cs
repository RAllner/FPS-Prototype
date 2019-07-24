/*
* Copyright (c) Raphael Allner
* www.raphaelallner.de
*/

using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	#region Variables
	[SerializeField]
	RectTransform thrusterFuelFill;

	[SerializeField]
	RectTransform currentHealth;

	private PlayerController controller;
	#endregion

	#region Methods


	public void SetController(PlayerController _controller)
	{
		controller = _controller;
	}

	void SetFuelAmount(float _amount)
	{
		thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
	}

	void SetCurrentHealth(float _amount)
	{
		thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
	}

	void Update()
	{
		SetFuelAmount(controller.GetThrusterFuelFill());
		
	}
	#endregion
}
