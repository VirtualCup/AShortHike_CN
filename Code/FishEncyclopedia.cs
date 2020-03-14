// FishEncyclopedia
public class FishEncyclopedia : UnityEngine.MonoBehaviour, IActionableItem
{
	public UnityEngine.GameObject collectionUIPrefab;

	public UnityEngine.GameObject dialogueBoxPrefab;

	public System.Collections.Generic.List<ItemAction> GetMenuActions(bool held)
	{
		ItemAction item = new ItemAction("查看记录", delegate
		{
			FishEncyclopedia fishEncyclopedia = this;
			GlobalData.CollectionInventory inventory = Singleton<GlobalData>.instance.gameData.inventory;
			UnityEngine.GameObject ui = collectionUIPrefab.Clone();
			CollectionListUI component = ui.GetComponent<CollectionListUI>();
			FishSpecies[] source = FishSpecies.LoadAll();
			System.Linq.IOrderedEnumerable<(FishSpecies, bool)> data = System.Linq.Enumerable.ThenBy(System.Linq.Enumerable.OrderByDescending(System.Linq.Enumerable.Concat(second: System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(System.Linq.Enumerable.Where(source, (FishSpecies s) => inventory.GetBiggestFishRecord(s, rare: true) != null), (System.Func<FishSpecies, (FishSpecies, bool)>)((FishSpecies species) => (species, true)))), first: System.Linq.Enumerable.Select((System.Collections.Generic.IEnumerable<FishSpecies>)source, (System.Func<FishSpecies, (FishSpecies, bool)>)((FishSpecies species) => (species, false)))), (System.Func<(FishSpecies, bool), int>)(((FishSpecies species, bool) t) => inventory.GetCatchCount(t.species))), (System.Func<(FishSpecies, bool), int>)delegate((FishSpecies species, bool) t)
			{
				if (!t.Item2)
				{
					return 0;
				}
				return 1;
			});
			component.Setup((System.Collections.Generic.IEnumerable<(FishSpecies, bool)>)data, (System.Action<(FishSpecies, bool), CollectionListUIElement>)delegate((FishSpecies species, bool) tuple, CollectionListUIElement element)
			{
				Fish biggestFishRecord = Singleton<GlobalData>.instance.gameData.inventory.GetBiggestFishRecord(tuple.species, tuple.Item2);
				if (biggestFishRecord != null)
				{
					string arg = (biggestFishRecord.rare ? (biggestFishRecord.species.GetRarePrefix() + "") : "") + biggestFishRecord.species.readableName;
					element.text.text = string.Format("{0}<color=#BA8>({1}厘米)</color>", arg, biggestFishRecord.size.ToString("0.0"));
					FishItemActions.SetupFishSprite(biggestFishRecord, element.image);
				}
				else
				{
					element.Setup(tuple.species.sprite, "???");
					element.image.color = UnityEngine.Color.black;
				}
				element.onConfirm += delegate
				{
					fishEncyclopedia.ShowFishInventoryMenu(tuple.species, element, ui);
				};
			});
			Singleton<GameServiceLocator>.instance.ui.AddUI(ui);
			return true;
		});
		return new System.Collections.Generic.List<ItemAction>
		{
			item
		};
	}

	private void ShowFishInventoryMenu(FishSpecies fish, CollectionListUIElement element, UnityEngine.GameObject fishMenu)
	{
		LinearMenu menu = null;
		menu = Singleton<GameServiceLocator>.instance.ui.CreateSimpleMenu(new string[1]
		{
			"查看说明"
		}, new System.Action[1]
		{
			delegate
			{
				UnityEngine.GameObject gameObject = dialogueBoxPrefab.Clone();
				UI.SetGenericText(gameObject, fish.journalInfo);
				Singleton<GameServiceLocator>.instance.ui.AddUI(gameObject);
				menu.Kill();
			}
		});
		element.PositionSimpleMenuAbove(menu.gameObject);
	}
}
