using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
	#region Variables
	[SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remotePlayerLayer = "RemotePlayerLayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
	[HideInInspector]
	public GameObject playerUIInstance;
	#endregion

	#region Methods
	// Start is called before the first frame update
	void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignLayer();
        } else
        {
            // Disable Player Graphics for local Player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create PlayerUI
            playerUIInstance =  Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

			// Configure PlayerUI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if(ui == null)
				Debug.LogError("No PlayerUI component on PlayerUI Prefab");
			ui.SetController(GetComponent<PlayerController>());

			GetComponent<Player>().PlayerSetup();
        }
    }

    void SetLayerRecursively(GameObject _obj, int _newLayer)
    {
        _obj.layer = _newLayer;
        foreach(Transform child in _obj.transform)
        {
            SetLayerRecursively(child.gameObject, _newLayer);
        }
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netId, _player);
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);
		if (isLocalPlayer)
		{
			GameManager.instance.SetOverviewCameraActive(true);
		}

        GameManager.UnRegisterPlayer(transform.name);
    }

    void DisableComponents()
    {
        foreach (Behaviour item in componentsToDisable)
        {
            item.enabled = false;
        }
    }

    void AssignLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remotePlayerLayer);
    }
	#endregion
}
