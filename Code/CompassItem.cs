// CompassItem
public class CompassItem : UnityEngine.MonoBehaviour, IActionableItem
{
	public const string SHOW_COMPASS_TAG = "ShowCompass";

	public System.Collections.Generic.List<ItemAction> GetMenuActions(bool held)
	{
		bool shown = Singleton<GlobalData>.instance.gameData.tags.GetBool("ShowCompass");
		string name = shown ? "隐藏" : "显示";
		return new System.Collections.Generic.List<ItemAction>
		{
			new ItemAction(name, delegate
			{
				Singleton<GlobalData>.instance.gameData.tags.SetBool("ShowCompass", !shown);
				return true;
			})
		};
	}
}
