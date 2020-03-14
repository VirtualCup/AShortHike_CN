// FishCollectPrompt
public class FishCollectPrompt : UnityEngine.MonoBehaviour
{
	public UnityEngine.UI.Image icon;

	public UnityEngine.UI.Image fishPicture;

	public TMPro.TMP_Text fishTypeName;

	public TMPro.TMP_Text fishTitle;

	public TMPro.TMP_Text fishInfo;

	public UnityEngine.UI.Image newPill;

	public UnityEngine.UI.Image newRecordPill;

	private QuickUnityTools.Input.FocusableUserInput input;

	private void Start()
	{
		input = QuickUnityTools.Input.GameUserInput.CreateInput(base.gameObject);
	}

	private void Update()
	{
		if (input.WasDismissPressed())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Setup(Fish fish)
	{
		fishTypeName.text = fish.species.typeName;
		fishTitle.text = fish.GetTitle();
		fishInfo.text = string.Format("{0}厘米", fish.size.ToString("0.0"));
		newPill.enabled = false;
		newRecordPill.enabled = false;
		if (Singleton<GlobalData>.instance.gameData.inventory.GetCatchCount(fish.species) == 1)
		{
			newPill.enabled = true;
		}
		else if (Singleton<GlobalData>.instance.gameData.inventory.GetBiggestFishRecord(fish.species, fish.rare) == fish)
		{
			newRecordPill.enabled = true;
		}
		FishItemActions.SetupFishSprite(fish, fishPicture);
		if ((bool)fish.species.customIcon)
		{
			icon.sprite = fish.species.customIcon;
		}
	}
}
