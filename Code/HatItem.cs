// HatItem
public class HatItem : UnityEngine.MonoBehaviour, IActionableItem
{
	public const string HAT_TAG = "WornHat";

	public CollectableItem associatedItem;

	public System.Collections.Generic.List<ItemAction> GetMenuActions(bool held)
	{
		System.Collections.Generic.List<ItemAction> list = new System.Collections.Generic.List<ItemAction>();
		Tags tags = Singleton<GlobalData>.instance.gameData.tags;
		if (tags.GetString("WornHat") == associatedItem.name)
		{
			System.Func<bool> action = delegate
			{
				tags.SetString("WornHat", null);
				return true;
			};
			list.Add(new ItemAction("取下", action));
		}
		else
		{
			System.Func<bool> action2 = delegate
			{
				tags.SetString("WornHat", associatedItem.name);
				return true;
			};
			list.Add(new ItemAction("戴上", action2));
		}
		return list;
	}
}
