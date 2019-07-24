using UnityEngine;

[System.Serializable]
public class Weapon
{
    public string name = "Glock";

    public int damage = 10;
    public float range = 100f;

	public GameObject graphics;

	public float fireRate = 0f;

}
