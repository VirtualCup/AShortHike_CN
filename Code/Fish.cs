// Fish
[System.Serializable]
public class Fish
{
	public enum SizeCategory
	{
		Tiny,
		Normal,
		Big
	}

	public static readonly System.Collections.Generic.Dictionary<Fish.SizeCategory, string> SIZE_TEXT_COLOR = new System.Collections.Generic.Dictionary<Fish.SizeCategory, string>
	{
		{
			Fish.SizeCategory.Tiny,
			"<color=#FFC0CB>小的</color> "
		},
		{
			Fish.SizeCategory.Normal,
			""
		},
		{
			Fish.SizeCategory.Big,
			"<color=yellow>大的</color> "
		}
	};

	public static System.Action<Fish> onFishCaught;

	[System.NonSerialized]
	private FishSpecies _species;

	public float size;

	public bool rare;

	private string speciesName;

	public FishSpecies species
	{
		get
		{
			if (_species == null)
			{
				_species = FishSpecies.Load(speciesName);
			}
			return _species;
		}
		set
		{
			_species = value;
		}
	}

	public Fish.SizeCategory sizeCategory
	{
		get
		{
			if (size > UnityEngine.Mathf.Lerp(species.size.min, species.size.max, 0.8f))
			{
				return Fish.SizeCategory.Big;
			}
			if (size < UnityEngine.Mathf.Lerp(species.size.min, species.size.max, 0.2f))
			{
				return Fish.SizeCategory.Tiny;
			}
			return Fish.SizeCategory.Normal;
		}
	}

	public Fish(FishSpecies fishSpecies, bool rare)
	{
		species = fishSpecies;
		size = fishSpecies.size.Random();
		speciesName = fishSpecies.name;
		this.rare = rare;
	}

	public System.Collections.IEnumerator CatchFishRoutine()
	{
		Singleton<GlobalData>.instance.gameData.inventory.AddFish(this);
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		player.TurnToFace(UnityEngine.Camera.main.transform);
		StackResourceSortingKey emotionKey = player.ikAnimator.ShowEmotion(Emotion.Happy);
		System.Action undoPose = player.ikAnimator.Pose(Pose.RaiseArms);
		FishCollectPrompt prompt = Singleton<GameServiceLocator>.instance.ui.CreateFishCatchPrompt(this);
		yield return new UnityEngine.WaitUntil(() => prompt == null);
		emotionKey.ReleaseResource();
		undoPose();
		onFishCaught?.Invoke(this);
	}

	public string GetTitle()
	{
		return SIZE_TEXT_COLOR[sizeCategory] + (rare ? (species.GetRarePrefix() + "") : "") + species.readableName;
	}

	public UnityEngine.Sprite GetSprite()
	{
		if (!rare)
		{
			return species.sprite;
		}
		if (!(bool)species.rareSprite)
		{
			return species.sprite;
		}
		return species.rareSprite;
	}
}
