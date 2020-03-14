// FishingBait
public class FishingBait : UnityEngine.MonoBehaviour, IActionableItem
{
	public const string ACTIVE_TAG = "BaitActive";

	public static bool isBaitActive
	{
		get
		{
			Tags tags = Singleton<GlobalData>.instance.gameData.tags;
			if (!tags.HasBool("BaitActive"))
			{
				tags.SetBool("BaitActive");
			}
			return tags.GetBool("BaitActive");
		}
		set
		{
			Singleton<GlobalData>.instance.gameData.tags.SetBool("BaitActive", value);
		}
	}

	public System.Collections.Generic.List<ItemAction> GetMenuActions(bool held)
	{
		System.Collections.Generic.List<ItemAction> list = new System.Collections.Generic.List<ItemAction>();
		if (isBaitActive)
		{
			list.Add(new ItemAction("停用鱼饵", delegate
			{
				isBaitActive = !isBaitActive;
				return false;
			}));
			list.Add(new ItemAction("保持使用鱼饵", () => false));
		}
		else
		{
			list.Add(new ItemAction("使用鱼饵", delegate
			{
				isBaitActive = !isBaitActive;
				return false;
			}));
			list.Add(new ItemAction("保持停用鱼饵", () => false));
		}
		return list;
	}
}
