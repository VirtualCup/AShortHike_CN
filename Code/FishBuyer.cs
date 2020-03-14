// FishBuyer
public class FishBuyer : UnityEngine.MonoBehaviour
{
	private const string SOLD_FISH = "Sold_";

	public UnityEngine.GameObject collectionUIPrefab;

	public CollectableItem baitItem;

	public CollectableItem coinItem;

	public UnityEngine.AudioClip saleSoundEffect;

	[UnityEngine.Header("Yarn Nodes")]
	public string sellNode = "BuyFish";

	public string cancelNode = "BuyFishCancel";

	[UnityEngine.Header("Yarn Read Tags")]
	public string alreadySoldTag = "FishAlreadySold";

	public string nameTag = "FishName";

	public string priceTag = "FishPrice";

	public string rareTag = "FishRare";

	[UnityEngine.Header("Yarn Write Tags")]
	public string soldBaitTag = "BaitSold";

	public void SellFish()
	{
		FishItemActions.CreateFishMenu(collectionUIPrefab, ShowSubmenu).GetComponent<KillOnBackButton>().onKill += delegate
		{
			Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(cancelNode, base.transform);
		};
	}

	public static string GetFishSoldTag(FishSpecies species, bool rare)
	{
		return "Sold_" + species.name + (rare ? "_Rare" : "");
	}

	private void ShowSubmenu(Fish fish, CollectionListUIElement element, UnityEngine.GameObject fishMenu)
	{
		LinearMenu menu = null;
		GlobalData.GameData data = Singleton<GlobalData>.instance.gameData;
		string specificFishAlreadySoldTag = GetFishSoldTag(fish.species, fish.rare);
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		System.Collections.Generic.List<System.Action> list2 = new System.Collections.Generic.List<System.Action>();
		int baitWorth = 1 + ((fish.sizeCategory != Fish.SizeCategory.Normal) ? 1 : 0) + (fish.rare ? 2 : 0);
		if (data.tags.GetBool(soldBaitTag) && data.tags.GetBool(specificFishAlreadySoldTag))
		{
			list.Add("卖出去换" + baitWorth + "鱼饵");
			list2.Add(delegate
			{
				data.inventory.RemoveFish(fish);
				data.AddCollected(baitItem, baitWorth);
				menu.Kill();
				fishMenu.GetComponent<CollectionListUI>().RemoveElement(element.gameObject);
				saleSoundEffect.Play();
			});
		}
		else
		{
			list.Add("卖");
			list2.Add(delegate
			{
				data.inventory.RemoveFish(fish);
				menu.Kill();
				UnityEngine.Object.Destroy(fishMenu);
				data.tags.SetBool(alreadySoldTag, data.tags.GetBool(specificFishAlreadySoldTag));
				string text = (fish.rare ? (fish.species.rarePrefix + "") : "") + fish.species.readableName;
				text = text.ToLower();
				data.tags.SetString(nameTag, text);
				data.tags.SetFloat(priceTag, fish.species.price * ((!fish.rare) ? 1 : 4));
				data.tags.SetBool(rareTag, fish.rare);
				Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(sellNode, base.transform);
				data.tags.SetBool(specificFishAlreadySoldTag);
			});
		}
		menu = Singleton<GameServiceLocator>.instance.ui.CreateSimpleMenu(list.ToArray(), list2.ToArray());
		element.PositionSimpleMenuAbove(menu.gameObject);
	}

	private void PayCoins()
	{
		GlobalData.GameData gameData = Singleton<GlobalData>.instance.gameData;
		gameData.AddCollected(coinItem, (int)gameData.tags.GetFloat(priceTag));
	}
}
