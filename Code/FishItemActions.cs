// FishItemActions
public class FishItemActions : UnityEngine.MonoBehaviour, IActionableItem
{
	public delegate void FishMenuCallback(Fish fish, CollectionListUIElement element, UnityEngine.GameObject fishMenu);

	public UnityEngine.GameObject collectionUIPrefab;

	public System.Collections.Generic.List<ItemAction> GetMenuActions(bool held)
	{
		ItemAction item = new ItemAction("浏览收藏", delegate
		{
			CreateFishMenu(collectionUIPrefab, ShowFishInventoryMenu);
			return true;
		});
		return new System.Collections.Generic.List<ItemAction>
		{
			item
		};
	}

	public static UnityEngine.GameObject CreateFishMenu(UnityEngine.GameObject menuPrefab, FishItemActions.FishMenuCallback onSelect)
	{
		UnityEngine.GameObject ui = menuPrefab.Clone();
		ui.GetComponent<CollectionListUI>().Setup(Singleton<GlobalData>.instance.gameData.inventory.GetAllFish(), delegate(Fish fish, CollectionListUIElement element)
		{
			element.text.text = string.Format("{0}({1}厘米)", fish.GetTitle(), fish.size.ToString("0.0"));
			SetupFishSprite(fish, element.image);
			element.onConfirm += delegate
			{
				onSelect(fish, element, ui);
			};
		});
		Singleton<GameServiceLocator>.instance.ui.AddUI(ui);
		return ui;
	}

	public static void SetupFishSprite(Fish fish, UnityEngine.UI.Image image)
	{
		UnityEngine.UI.LayoutElement component = image.GetComponent<UnityEngine.UI.LayoutElement>();
		UnityEngine.Vector2 vector = (component != null) ? new UnityEngine.Vector2(component.preferredWidth, component.preferredHeight) : image.rectTransform.sizeDelta;
		UnityEngine.Sprite sprite = fish.GetSprite();
		float num = vector.y * sprite.rect.width / sprite.rect.height;
		num *= UnityEngine.Mathf.Lerp(0.8f, 1.2f, UnityEngine.Mathf.InverseLerp(fish.species.size.min, fish.species.size.max, fish.size));
		image.sprite = sprite;
		UnityEngine.Vector2 vector2 = new UnityEngine.Vector2(num, vector.y);
		if ((bool)component)
		{
			component.preferredWidth = vector2.x;
			component.preferredHeight = vector2.y;
		}
		else
		{
			image.rectTransform.sizeDelta = vector2;
		}
	}

	private void ShowFishInventoryMenu(Fish fish, CollectionListUIElement element, UnityEngine.GameObject fishMenu)
	{
		LinearMenu menu = null;
		menu = Singleton<GameServiceLocator>.instance.ui.CreateSimpleMenu(new string[1]
		{
			"释放"
		}, new System.Action[1]
		{
			delegate
			{
				Singleton<GlobalData>.instance.gameData.inventory.RemoveFish(fish);
				menu.Kill();
				UnityEngine.Object.Destroy(fishMenu);
			}
		});
		element.PositionSimpleMenuAbove(menu.gameObject);
	}
}
