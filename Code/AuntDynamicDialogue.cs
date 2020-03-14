// AuntDynamicDialogue
public class AuntDynamicDialogue : UnityEngine.MonoBehaviour
{
	public string talkPointsTag = "$TalkPoints";

	public string transitionTag = "Transition";

	private void Start()
	{
		Singleton<GlobalData>.instance.gameData.tags.WatchFloat(talkPointsTag, OnTalkPointsChanged);
	}

	private void OnDestroy()
	{
		if (Singleton<GlobalData>.instance != null)
		{
			Singleton<GlobalData>.instance.gameData.tags.UnwatchFloat(talkPointsTag, OnTalkPointsChanged);
		}
	}

	private void OnTalkPointsChanged(float number)
	{
		string value;
		switch ((int)number % 4)
		{
		case 0:
			value = "";
			break;
		case 1:
			value = "还有，";
			break;
		case 2:
			value = "哦！";
			break;
		case 3:
			value = "我还做了什么……";
			break;
		default:
			value = "";
			break;
		}
		Singleton<GlobalData>.instance.gameData.tags.SetString(transitionTag, value);
	}
}
