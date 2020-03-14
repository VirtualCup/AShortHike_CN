// TitleScreen
public class TitleScreen : UnityEngine.MonoBehaviour
{
	public static int DEMO_MODE = -1;

	public string titleScreenIntroStart = "TitleScreenIntroStart";

	public float loadTickFrequency = 0.5f;

	public GridMenu menu;

	public UnityEngine.GameObject newGameItem;

	public UnityEngine.GameObject continueItem;

	public UnityEngine.GameObject optionsMenuPrefab;

	public TMPro.TMP_Text pressToStart;

	public TMPro.TMP_Text loadingText;

	public TMPro.TMP_Text demoText;

	public UnityEngine.Transform introSpeaker;

	public UnityEngine.GameObject transitionAnimation;

	public UnityEngine.AudioClip driveAwayClip;

	public UnityEngine.Audio.AudioMixerSnapshot quietSnapshot;

	public UnityEngine.Audio.AudioMixerSnapshot defaultSnapshot;

	private DialogueController dialogue;

	private UnityEngine.AsyncOperation loadingOperation;

	private QuickUnityTools.Input.GameUserInput input;

	private float loadTimer;

	private IConversation introConversastion;

	private bool startedTransition;

	private void Start()
	{
		Volume.LoadVolumeLevels();
		OptionsMenu.LoadGraphicsPlayerPrefs();
		if (!Singleton<GlobalData>.instance.DoesSaveExist(GlobalData.DEFAULT_SAVE_FILE))
		{
			newGameItem.transform.SetAsFirstSibling();
			menu.SetMenuItem(0, 0, newGameItem);
			menu.SetMenuItem(1, 0, continueItem);
			continueItem.GetComponent<IMenuItem>().enabled = false;
		}
		dialogue = Singleton<ServiceLocator>.instance.Locate<DialogueController>();
		UnityEngine.GameObject gameObject = new UnityEngine.GameObject("InputHolder");
		gameObject.gameObject.SetActive(value: false);
		input = QuickUnityTools.Input.GameUserInput.CreateInput(gameObject);
		input.priority = -10;
		gameObject.gameObject.SetActive(value: true);
		pressToStart.text = TextReplacer.ReplaceVariables(pressToStart.text);
		pressToStart.gameObject.SetActive(value: true);
		menu.gameObject.SetActive(value: false);
		loadingText.gameObject.SetActive(value: false);
		SetDemoText(DEMO_MODE);
	}

	private void Update()
	{
		if (pressToStart.gameObject.activeSelf && input.GetConfirmButton().ConsumePress())
		{
			pressToStart.gameObject.SetActive(value: false);
			this.RegisterTimer(0.5f, delegate
			{
				menu.gameObject.SetActive(value: true);
			});
		}
		if (loadingOperation != null)
		{
			loadTimer += UnityEngine.Time.deltaTime;
			loadingText.enabled = (loadingOperation.progress < 0.9f);
			if (loadTimer > loadTickFrequency)
			{
				loadTimer = 0f;
				loadingText.text += ".";
				if (loadingText.text.Length > 3)
				{
					loadingText.text = "";
				}
			}
			if (loadingOperation.progress >= 0.9f && (introConversastion == null || !introConversastion.isAlive))
			{
				PlayTransitionAnimation();
			}
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) && UnityEngine.Input.GetKey(UnityEngine.KeyCode.F1) && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Backspace))
		{
			EnableDemoMode(0);
		}
		if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D) && UnityEngine.Input.GetKey(UnityEngine.KeyCode.F2) && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Backspace))
		{
			EnableDemoMode(1);
		}
	}

	private void PlayTransitionAnimation()
	{
		if (!startedTransition)
		{
			startedTransition = true;
			transitionAnimation.SetActive(value: true);
			UnityEngine.Object.DontDestroyOnLoad(driveAwayClip.Play().gameObject);
			quietSnapshot.TransitionTo(1f);
			this.RegisterTimer(1.4f, delegate
			{
				loadingOperation.allowSceneActivation = true;
			});
		}
	}

	public void StartNewGame()
	{
		if (Singleton<GlobalData>.instance.DoesSaveExist(GlobalData.DEFAULT_SAVE_FILE))
		{
			UI uI = Singleton<ServiceLocator>.instance.Locate<UI>();
			LinearMenu submenu = null;
			submenu = uI.CreateSimpleMenu(new string[2]
			{
				"继续",
				"等会！"
			}, new System.Action[2]
			{
				delegate
				{
					submenu.Kill();
					BeginLoadingNewGame();
				},
				delegate
				{
					submenu.Kill();
				}
			});
			UnityEngine.GameObject gameObject = uI.CreateTextMenuItem("<sprite name=\"Warning\" tint=1> 此举会覆盖掉当前存档");
			gameObject.transform.SetParent(submenu.transform, worldPositionStays: false);
			gameObject.transform.SetAsFirstSibling();
			UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as UnityEngine.RectTransform);
			(submenu.transform as UnityEngine.RectTransform).CenterWithinParent();
		}
		else
		{
			BeginLoadingNewGame();
		}
	}

	private void BeginLoadingNewGame()
	{
		LoadGame(Singleton<GlobalData>.instance.NewGame());
		introConversastion = dialogue.StartConversation(titleScreenIntroStart, introSpeaker);
	}

	public void ContinueGame()
	{
		LoadGame(Singleton<GlobalData>.instance.LoadGame(GlobalData.DEFAULT_SAVE_FILE));
	}

	private void LoadGame(UnityEngine.AsyncOperation loading)
	{
		loadingOperation = loading;
		menu.gameObject.SetActive(value: false);
		loadingText.gameObject.SetActive(value: true);
		loadingText.text = "";
		loadingOperation.allowSceneActivation = false;
	}

	private void EnableDemoMode(int demo)
	{
		SetDemoText(demo);
		UnityEngine.Object.FindObjectOfType<InControlSpawner>().EnableXInput();
		QuickUnityTools.Input.GameUserInput.FORCE_DEVICE = demo;
		Singleton<GlobalData>.instance.SetNewFilename("demosave" + demo);
		SetWindowText(FindWindow(null, "A Short Hike"), "A Short Hike - Demo " + (demo + 1).ToString());
	}

	private void SetDemoText(int demo)
	{
		if (demo == -1)
		{
			demoText.gameObject.SetActive(value: false);
			return;
		}
		demoText.gameObject.SetActive(value: true);
		demoText.text = "demo " + (demo + 1).ToString();
	}

	public void Quit()
	{
		if (DEMO_MODE == -1)
		{
			UnityEngine.PlayerPrefs.Save();
			UnityEngine.Application.Quit();
		}
	}

	public void ShowOptions()
	{
		optionsMenuPrefab.Clone();
	}

	[System.Runtime.InteropServices.DllImport("user32.dll")]
	public static extern bool SetWindowText(System.IntPtr hwnd, string lpString);

	[System.Runtime.InteropServices.DllImport("user32.dll")]
	public static extern System.IntPtr FindWindow(string className, string windowName);
}
