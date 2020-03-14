// Holdable
public class Holdable : UnityEngine.MonoBehaviour, IInteractableComponent, IActionableItem
{
	public enum UseAction
	{
		Swing,
		Dig,
		Bucket
	}

	public Holdable.UseAction useAction;

	public CollectableItem associatedItem;

	public bool canUseWhileJumping;

	private UnityEngine.Collider[] _colliders;

	public bool cannotDrop => associatedItem.cannotDrop;

	public bool cannotStash => associatedItem.cannotStash;

	public Player anchoredTo
	{
		get;
		private set;
	}

	private UnityEngine.Collider[] colliders
	{
		get
		{
			if (_colliders == null)
			{
				_colliders = GetComponentsInChildren<UnityEngine.Collider>();
			}
			return _colliders;
		}
	}

	public event System.Action onReleased;

	public event System.Action onPickedUp;

	public void Interact()
	{
		Singleton<GameServiceLocator>.instance.levelController.player.PickUp(this);
		StatusBarUI statusBar = Singleton<GameServiceLocator>.instance.levelUI.statusBar;
		int collected = Singleton<GlobalData>.instance.gameData.GetCollected(associatedItem);
		statusBar.ShowStatusBox(associatedItem, collected + 1).HideAndKill(statusBar.statusBoxTime);
	}

	private void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (anchoredTo != null)
		{
			base.transform.position = anchoredTo.handTransform.position;
			base.transform.rotation = anchoredTo.handTransform.rotation;
		}
	}

	public void ParentToPlayer(Player player)
	{
		if (this == null)
		{
			UnityEngine.Debug.LogError("Trying to parent a destroyed object!", this);
			return;
		}
		UnityEngine.Collider[] colliders = this.colliders;
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
		GetComponent<UnityEngine.Rigidbody>().isKinematic = true;
		GetComponent<RangedInteractable>().enabled = false;
		anchoredTo = player;
		UpdatePosition();
		this.onPickedUp?.Invoke();
	}

	public void ReleaseFromPlayer()
	{
		if (anchoredTo == null)
		{
			UnityEngine.Debug.LogWarning("Cannot release from player since it is not held.");
			return;
		}
		UnityEngine.Collider[] colliders = this.colliders;
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = true;
		}
		GetComponent<UnityEngine.Rigidbody>().isKinematic = false;
		GetComponent<RangedInteractable>().enabled = true;
		anchoredTo = null;
		this.onReleased?.Invoke();
	}

	public System.Collections.Generic.List<ItemAction> GetMenuActions(bool held)
	{
		System.Collections.Generic.List<ItemAction> list = new System.Collections.Generic.List<ItemAction>();
		if (!(bool)associatedItem)
		{
			return list;
		}
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		if (!held)
		{
			list.Add(new ItemAction("装备", delegate
			{
				EquipFromInventory(player, associatedItem);
				return true;
			}));
		}
		else
		{
			if (!associatedItem.cannotDrop)
			{
				list.Add(new ItemAction("丢弃", delegate
				{
					player.DropItem();
					return true;
				}));
			}
			list.Add(new ItemAction("存贮", delegate
			{
				player.StashHeldItem();
				return false;
			}));
		}
		return list;
	}

	public static void EquipFromInventory(Player player, CollectableItem item)
	{
		Singleton<GlobalData>.instance.gameData.AddCollected(item, -1, equipAction: true);
		UnityEngine.GameObject gameObject = item.worldPrefab.Clone();
		player.PickUp(gameObject.GetComponent<Holdable>());
	}

	bool IInteractableComponent.get_enabled()
	{
		return base.enabled;
	}

	void IInteractableComponent.set_enabled(bool value)
	{
		base.enabled = value;
	}
}
