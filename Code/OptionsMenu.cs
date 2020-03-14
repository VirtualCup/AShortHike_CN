// OptionsMenu
public class OptionsMenu : UnityEngine.MonoBehaviour
{
	private struct MenuItem
	{
		public string name;

		public System.Action<BasicMenuItem> action;

		public MenuItem(string name, System.Action action)
		{
			this.name = name;
			this.action = delegate
			{
				action();
			};
		}

		public MenuItem(string name, System.Action<BasicMenuItem> action)
		{
			this.name = name;
			this.action = action;
		}
	}

	public const string DATA_FOLDER = "/Resources/Data/";

	public const string LICENSE_FILE = "SoftwareLicenses.txt";

	private const string FPS_PREF = "FPS_";

	private const string JITTER_PREF = "JITTER_";

	private const string SCALE_PREF = "SCALE_";

	private static UnityEngine.Resolution[] REQUIRED_RESOLUTIONS = new UnityEngine.Resolution[5]
	{
		new UnityEngine.Resolution
		{
			width = 384,
			height = 216
		},
		new UnityEngine.Resolution
		{
			width = 768,
			height = 432
		},
		new UnityEngine.Resolution
		{
			width = 1152,
			height = 648
		},
		new UnityEngine.Resolution
		{
			width = 1280,
			height = 720
		},
		new UnityEngine.Resolution
		{
			width = 1980,
			height = 1080
		}
	};

	private static System.Collections.Generic.Dictionary<UnityEngine.FullScreenMode, string> SCREEN_MODES = new System.Collections.Generic.Dictionary<UnityEngine.FullScreenMode, string>
	{
		{
			UnityEngine.FullScreenMode.Windowed,
			"窗口"
		},
		{
			UnityEngine.FullScreenMode.FullScreenWindow,
			"无边框窗口"
		},
		{
			UnityEngine.FullScreenMode.ExclusiveFullScreen,
			"真全屏"
		}
	};

	private static System.Collections.Generic.Dictionary<Volume.Channel, string> VOLUME_CHANNELS = new System.Collections.Generic.Dictionary<Volume.Channel, string>
	{
		{
			Volume.Channel.Master,
			"主音量"
		},
		{
			Volume.Channel.Music,
			"音乐"
		},
		{
			Volume.Channel.SoundEffects,
			"音效"
		},
		{
			Volume.Channel.Ambience,
			"环境音"
		}
	};

	private static System.Collections.Generic.List<float> RENDER_SCALES = new System.Collections.Generic.List<float>
	{
		1f,
		1.25f,
		1.5f
	};

	private static System.Collections.Generic.List<float> EXTRA_RENDER_SCALES = new System.Collections.Generic.List<float>
	{
		2f,
		2.5f,
		-1f
	};

	public static string OPTIONS_RED_HEX;

	public UnityEngine.Color highlightColor = UnityEngine.Color.red;

	public UnityEngine.GameObject dialogBoxPrefab;

	public UnityEngine.GameObject creditsPrefab;

	public UnityEngine.GameObject messagePrefab;

	public UnityEngine.GameObject gamepadControlsPrefab;

	public UnityEngine.GameObject keyboardControlsPrefab;

	public UnityEngine.GameObject rebindPrefab;

	public UnityEngine.GameObject achievementsMenuPrefab;

	public UnityEngine.GameObject controllerRemapperPrefab;

	private UI ui;

	private LinearMenu optionsMenu;

	private void Awake()
	{
		ui = Singleton<ServiceLocator>.instance.Locate<UI>();
	}

	private void Start()
	{
		OPTIONS_RED_HEX = highlightColor.ToHexString();
		BuildMainMenu();
	}

	private void BuildMainMenu()
	{
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = new System.Collections.Generic.List<OptionsMenu.MenuItem>();
		list.Add(new OptionsMenu.MenuItem("图像", ShowGraphicsMenu));
		list.Add(new OptionsMenu.MenuItem("音频", ShowAudioMenu));
		list.Add(new OptionsMenu.MenuItem("控制", ShowControlsMenu));
		list.Add(new OptionsMenu.MenuItem("信息", ShowAboutMenu));
		optionsMenu = BuildSimpleMenu(list);
		optionsMenu.onKill += delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		};
	}

	private LinearMenu BuildSimpleMenu(System.Collections.Generic.List<OptionsMenu.MenuItem> menuItems)
	{
		LinearMenu linearMenu = ui.CreateSimpleMenu(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(menuItems, (OptionsMenu.MenuItem i) => i.name)), System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(menuItems, (OptionsMenu.MenuItem i) => i.action)));
		PositionMenu(linearMenu.transform);
		return linearMenu;
	}

	public static void PositionMenu(UnityEngine.Transform menu)
	{
		UnityEngine.RectTransform rectTransform = menu.transform as UnityEngine.RectTransform;
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		rectTransform.CenterWithinParent();
		UnityEngine.RectTransform rectTransform2 = rectTransform.parent as UnityEngine.RectTransform;
		float num = rectTransform2.rect.width * 0.25f;
		num += (float)((Singleton<QuickUnityTools.Input.UserInputManager>.instance.inputStackCount - 3) * 20);
		num = UnityEngine.Mathf.RoundToInt(num);
		rectTransform.SetInsetAndSizeFromParentEdge(UnityEngine.RectTransform.Edge.Left, num, rectTransform.rect.size.x);
		if (rectTransform.localPosition.x + rectTransform.rect.size.x / 2f > rectTransform2.rect.max.x)
		{
			rectTransform.SetInsetAndSizeFromParentEdge(UnityEngine.RectTransform.Edge.Right, 0f, rectTransform.rect.size.x);
		}
	}

	private string HighlightText(string text)
	{
		int num = text.IndexOf(':');
		if (num == -1)
		{
			return text;
		}
		text = text.Insert(text.Length, "</color>");
		text = text.Insert(num + 1, "<color=" + OPTIONS_RED_HEX + ">");
		return text;
	}

	private string GetVSyncText()
	{
		return HighlightText("垂直同步: " + ((UnityEngine.QualitySettings.vSyncCount == 0) ? "已禁用" : "已启用"));
	}

	private string GetShadowsText()
	{
		return HighlightText("阴影: " + ((UnityEngine.QualitySettings.shadows == UnityEngine.ShadowQuality.Disable) ? "已禁用" : "已启用"));
	}

	private string GetEdgesText()
	{
		return HighlightText("边缘: " + ((!ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.EdgeDetection)) ? "已禁用" : "已启用"));
	}

	private string GetColorCorrectionText()
	{
		return HighlightText("颜色校正: " + ((!ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.ColorCorrection)) ? "已禁用" : "已启用"));
	}

	private string GetJitterFixText()
	{
		return HighlightText("抖动修正: " + Player.jitterFixConfiguration.ToString().ToLower());
	}

	private string GetRenderResolutionText()
	{
		return HighlightText("像素度: " + PixelFilterAdjuster.GetScaleString(PixelFilterAdjuster.scale));
	}

	private void ShowGraphicsMenu()
	{
		LinearMenu menu = null;
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = new System.Collections.Generic.List<OptionsMenu.MenuItem>();
		list.Add(new OptionsMenu.MenuItem(GetResolutionText(), ShowResolutionMenu));
		list.Add(new OptionsMenu.MenuItem(GetWindowModeText(), ShowWindowModeMenu));
		list.Add(new OptionsMenu.MenuItem(GetImageQualityText(), delegate(BasicMenuItem item)
		{
			ShowImageQualityMenu(menu, item);
		}));
		list.Add(new OptionsMenu.MenuItem(GetShadowsText(), delegate(BasicMenuItem item)
		{
			UnityEngine.QualitySettings.shadows = ((UnityEngine.QualitySettings.shadows == UnityEngine.ShadowQuality.Disable) ? UnityEngine.ShadowQuality.HardOnly : UnityEngine.ShadowQuality.Disable);
			UI.SetGenericText(item.gameObject, GetShadowsText());
		}));
		list.Add(new OptionsMenu.MenuItem(GetEdgesText(), delegate(BasicMenuItem item)
		{
			ImageEffectsSettingsController.allowedEffects = (ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.EdgeDetection) ? (ImageEffectsSettingsController.allowedEffects & ~ImageEffectsSettingsController.Effect.EdgeDetection) : (ImageEffectsSettingsController.allowedEffects | ImageEffectsSettingsController.Effect.EdgeDetection));
			UI.SetGenericText(item.gameObject, GetEdgesText());
		}));
		list.Add(new OptionsMenu.MenuItem(GetColorCorrectionText(), delegate(BasicMenuItem item)
		{
			ImageEffectsSettingsController.allowedEffects = (ImageEffectsSettingsController.IsEffectAllowed(ImageEffectsSettingsController.Effect.ColorCorrection) ? (ImageEffectsSettingsController.allowedEffects & ~ImageEffectsSettingsController.Effect.ColorCorrection) : (ImageEffectsSettingsController.allowedEffects | ImageEffectsSettingsController.Effect.ColorCorrection));
			UI.SetGenericText(item.gameObject, GetColorCorrectionText());
		}));
		System.Func<string> targetFPSText = () => HighlightText("目标FPS: " + ((UnityEngine.Application.targetFrameRate == -1) ? "无限制" : UnityEngine.Application.targetFrameRate.ToString()));
		list.Add(new OptionsMenu.MenuItem(GetVSyncText(), delegate(BasicMenuItem item)
		{
			UnityEngine.QualitySettings.vSyncCount = ((UnityEngine.QualitySettings.vSyncCount == 0) ? 1 : 0);
			UI.SetGenericText(item.gameObject, GetVSyncText());
			UnityEngine.Application.targetFrameRate = -1;
			UpdateItemStartingWith(menu, "目标FPS", targetFPSText());
		}));
		list.Add(new OptionsMenu.MenuItem(targetFPSText(), delegate(BasicMenuItem item)
		{
			if (UnityEngine.Application.targetFrameRate == -1)
			{
				UnityEngine.Application.targetFrameRate = 30;
			}
			else if (UnityEngine.Application.targetFrameRate == 30)
			{
				UnityEngine.Application.targetFrameRate = 60;
			}
			else
			{
				UnityEngine.Application.targetFrameRate = -1;
			}
			UnityEngine.QualitySettings.vSyncCount = 0;
			UpdateItemStartingWith(menu, "垂直同步", GetVSyncText());
			UI.SetGenericText(item.gameObject, targetFPSText());
		}));
		System.Func<string> showFPSText = () => HighlightText("显示FPS: " + ((!LevelUI.enableFPSCounter) ? "已禁用" : "已启用"));
		list.Add(new OptionsMenu.MenuItem(showFPSText(), delegate(BasicMenuItem item)
		{
			LevelUI.enableFPSCounter = !LevelUI.enableFPSCounter;
			UnityEngine.PlayerPrefs.SetInt("FPS_", LevelUI.enableFPSCounter ? 1 : 0);
			UI.SetGenericText(item.gameObject, showFPSText());
		}));
		list.Add(new OptionsMenu.MenuItem(GetJitterFixText(), delegate(BasicMenuItem item)
		{
			string warningText = "<sprite name=\"Warning\" tint=1> 该选项会通过降低画面整体平滑度来\n消除可能由不稳定帧数导致的镜头抖动。\n<size=8>\n</size>默认情况下，该功能会在帧数不稳定时\n自动启用。";
			System.Collections.Generic.IEnumerable<Player.JitterFixConfiguration> source = System.Linq.Enumerable.Cast<Player.JitterFixConfiguration>(System.Enum.GetValues(typeof(Player.JitterFixConfiguration)));
			CreateSubmenuPrompt(warningText, new string[3]
			{
				"自动<color=#AAA>（推荐）</color>",
				"总是禁用",
				"总是启用"
			}, System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(source, (System.Func<Player.JitterFixConfiguration, System.Action>)((Player.JitterFixConfiguration e) => delegate
			{
				Player.jitterFixConfiguration = e;
				UnityEngine.PlayerPrefs.SetInt("JITTER_", (int)Player.jitterFixConfiguration);
				UI.SetGenericText(item.gameObject, GetJitterFixText());
			}))));
		}));
		list.Add(new OptionsMenu.MenuItem(GetRenderResolutionText(), ShowRenderResolutionMenu));
		menu = BuildSimpleMenu(list);
	}

	private string GetImageQualityText()
	{
		return HighlightText("图像质量: " + UnityEngine.QualitySettings.names[UnityEngine.QualitySettings.GetQualityLevel()]);
	}

	private void ShowImageQualityMenu(LinearMenu menu, BasicMenuItem menuItem)
	{
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = new System.Collections.Generic.List<OptionsMenu.MenuItem>();
		for (int i = 0; i < UnityEngine.QualitySettings.names.Length; i++)
		{
			int index = i;
			list.Add(new OptionsMenu.MenuItem(UnityEngine.QualitySettings.names[i], (System.Action)delegate
			{
				UnityEngine.QualitySettings.SetQualityLevel(index);
				UI.SetGenericText(menuItem.gameObject, GetImageQualityText());
				ImageEffectsSettingsController.SetEffectsToMatchQualitySettings();
				UpdateItemStartingWith(menu, "垂直同步", GetVSyncText());
				UpdateItemStartingWith(menu, "阴影", GetShadowsText());
				UpdateItemStartingWith(menu, "颜色校正", GetColorCorrectionText());
				UpdateItemStartingWith(menu, "边缘", GetEdgesText());
			}));
		}
		BuildSimpleMenu(list);
	}

	private string GetResolutionText()
	{
		return HighlightText("分辨率: " + UnityEngine.Screen.width + "x" + UnityEngine.Screen.height);
	}

	private void ShowResolutionMenu(BasicMenuItem resolutionItem)
	{
		System.Collections.Generic.List<OptionsMenu.MenuItem> menuItems = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(System.Linq.Enumerable.Select(System.Linq.Enumerable.GroupBy(System.Linq.Enumerable.OrderBy(System.Linq.Enumerable.Concat(UnityEngine.Screen.resolutions, REQUIRED_RESOLUTIONS), (UnityEngine.Resolution r) => r.width), (UnityEngine.Resolution r) => (r.width + r.height).GetHashCode()), (System.Linq.IGrouping<int, UnityEngine.Resolution> g) => System.Linq.Enumerable.First(g)), (UnityEngine.Resolution r) => new OptionsMenu.MenuItem(r.width + " X " + r.height, (System.Action)delegate
		{
			UnityEngine.Screen.SetResolution(r.width, r.height, UnityEngine.Screen.fullScreenMode);
			this.RegisterTimer(0.01f, delegate
			{
				UI.SetGenericText(resolutionItem.gameObject, GetResolutionText());
			});
		})));
		BuildSimpleMenu(menuItems);
	}

	private void ShowRenderResolutionMenu(BasicMenuItem menuItem)
	{
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(RENDER_SCALES, (float s) => new OptionsMenu.MenuItem(PixelFilterAdjuster.GetScaleString(s), (System.Action)delegate
		{
			PixelFilterAdjuster.scale = s;
			UnityEngine.PlayerPrefs.SetFloat("SCALE_", s);
			UI.SetGenericText(menuItem.gameObject, GetRenderResolutionText());
		})));
		list.Add(new OptionsMenu.MenuItem("高级", (System.Action)delegate
		{
			System.Collections.Generic.List<OptionsMenu.MenuItem> menuItems = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(EXTRA_RENDER_SCALES, (float s) => new OptionsMenu.MenuItem(PixelFilterAdjuster.GetScaleString(s), (System.Action)delegate
			{
				PixelFilterAdjuster.scale = s;
				UnityEngine.PlayerPrefs.SetFloat("SCALE_", s);
				UI.SetGenericText(menuItem.gameObject, GetRenderResolutionText());
			})));
			BuildSimpleMenu(menuItems);
			ShowResolutionWarning();
		}));
		BuildSimpleMenu(list);
	}

	private void ShowResolutionWarning()
	{
		UI.SetGenericText(ui.AddUI(dialogBoxPrefab.Clone()), "<sprite name=\"Warning\" tint=1> 本游戏的画面效果是以\n默认像素度来设计的。\n因此这些功能是\n以能用为目的加进来的。");
	}

	private string GetWindowModeText()
	{
		if (!SCREEN_MODES.ContainsKey(UnityEngine.Screen.fullScreenMode))
		{
			return "screen mode";
		}
		return HighlightText("屏幕: " + SCREEN_MODES[UnityEngine.Screen.fullScreenMode]);
	}

	private void ShowWindowModeMenu(BasicMenuItem windowItem)
	{
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = new System.Collections.Generic.List<OptionsMenu.MenuItem>();
		foreach (UnityEngine.FullScreenMode mode in SCREEN_MODES.Keys)
		{
			list.Add(new OptionsMenu.MenuItem(SCREEN_MODES[mode], (System.Action)delegate
			{
				UnityEngine.Screen.SetResolution(UnityEngine.Screen.width, UnityEngine.Screen.height, mode);
				this.RegisterTimer(0.01f, delegate
				{
					UI.SetGenericText(windowItem.gameObject, GetWindowModeText());
				});
			}));
		}
		BuildSimpleMenu(list);
	}

	private string GetVolumeText(Volume.Channel channel)
	{
		return HighlightText(VOLUME_CHANNELS[channel] + ": " + UnityEngine.Mathf.Round(Volume.GetVolume(channel) * 10f));
	}

	private void ShowAudioMenu()
	{
		LinearMenu linearMenu = ui.CreateSimpleMenu();
		System.Collections.Generic.List<UnityEngine.GameObject> list = new System.Collections.Generic.List<UnityEngine.GameObject>();
		foreach (Volume.Channel channel in VOLUME_CHANNELS.Keys)
		{
			UnityEngine.GameObject gameObject = ui.CreateScrollMenuItem(GetVolumeText(channel), delegate
			{
			}, delegate(int scrollValue, ScrollMenuItem item)
			{
				float volume = Volume.GetVolume(channel);
				volume += (float)scrollValue * 0.1f;
				volume = UnityEngine.Mathf.Clamp(UnityEngine.Mathf.Round(volume * 10f) / 10f, 0f, 2f);
				Volume.SetVolume(channel, volume);
				UI.SetGenericText(item.gameObject, GetVolumeText(channel));
			});
			gameObject.transform.SetParent(linearMenu.transform, worldPositionStays: false);
			list.Add(gameObject);
		}
		linearMenu.SetMenuObjects(list);
		PositionMenu(linearMenu.transform);
	}

	private void ShowAchievementsMenu()
	{
		UnityEngine.GameObject gameObject = achievementsMenuPrefab.Clone();
		Singleton<GameServiceLocator>.instance.ui.AddUI(gameObject);
		gameObject.GetComponent<CollectionListUI>().Setup((Achievement[])System.Enum.GetValues(typeof(Achievement)), delegate(Achievement data, CollectionListUIElement element)
		{
			AchievementData data2 = data.GetData();
			bool flag = Singleton<GameServiceLocator>.instance.achievements.HasAchievement(data);
			element.image.sprite = (flag ? data2.sprite : data2.lockedSprite);
			string text = string.Format("<color={2}>{0}</color>\n{1}", data2.title, data2.description, flag ? "#FC4" : "#863");
			text = ((!flag && data2.secret) ? "???" : text);
			element.text.text = text;
		});
	}

	private void ShowAboutMenu()
	{
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = new System.Collections.Generic.List<OptionsMenu.MenuItem>();
		list.Add(new OptionsMenu.MenuItem("成就", ShowAchievementsMenu));
		list.Add(new OptionsMenu.MenuItem("制作名单", (System.Action)delegate
		{
			ui.AddUI(creditsPrefab.Clone());
		}));
		list.Add(new OptionsMenu.MenuItem("第三方许可证", (System.Action)delegate
		{
			CreateSubmenuPrompt("<sprite name=\"Warning\" tint=1> 此举会开启新的窗口", "来吧", "算了", delegate
			{
				UnityEngine.Application.OpenURL((UnityEngine.Application.isEditor ? (UnityEngine.Application.dataPath + "/Resources/Data/") : (UnityEngine.Application.dataPath + "/")) + "SoftwareLicenses.txt");
			}, delegate
			{
			});
		}));
		list.Add(new OptionsMenu.MenuItem("私人讯息", (System.Action)delegate
		{
			ui.AddUI(messagePrefab.Clone());
		}));
		list.Add(new OptionsMenu.MenuItem("版本", (System.Action)delegate
		{
			UnityEngine.GameObject gameObject = ui.AddUI(dialogBoxPrefab.Clone());
			BuildInfo buildInfo = BuildInfo.Load();
			UI.SetGenericText(gameObject, "版本: " + buildInfo.version + "版本号: " + buildInfo.buildNumber);
		}));
		if (UnityEngine.PlayerPrefs.GetInt("BeatGame") != 0 || UnityEngine.Application.isEditor)
		{
			System.Func<string> speedrunText = () => HighlightText("竞速时钟: " + ((!LevelController.speedrunClockActive) ? "已禁用" : "已启用"));
			list.Add(new OptionsMenu.MenuItem(speedrunText(), delegate(BasicMenuItem element)
			{
				LevelController.speedrunClockActive = !LevelController.speedrunClockActive;
				UI.SetGenericText(element.gameObject, speedrunText());
			}));
		}
		BuildSimpleMenu(list);
	}

	private void CreateSubmenuPrompt(string warningText, string[] options, System.Action[] actions)
	{
		LinearMenu submenu = null;
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = new System.Collections.Generic.List<OptionsMenu.MenuItem>();
		for (int i = 0; i < options.Length; i++)
		{
			System.Action action = actions[i];
			list.Add(new OptionsMenu.MenuItem(options[i], (System.Action)delegate
			{
				action();
				submenu.Kill();
			}));
		}
		submenu = BuildSimpleMenu(list);
		UnityEngine.GameObject gameObject = ui.CreateTextMenuItem(warningText);
		gameObject.transform.SetParent(submenu.transform, worldPositionStays: false);
		gameObject.transform.SetAsFirstSibling();
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(submenu.transform as UnityEngine.RectTransform);
		(submenu.transform as UnityEngine.RectTransform).CenterWithinParent();
	}

	private void CreateSubmenuPrompt(string warningText, string confirmText, string cancelText, System.Action confirmAction, System.Action cancelAction)
	{
		CreateSubmenuPrompt(warningText, new string[2]
		{
			confirmText,
			cancelText
		}, new System.Action[2]
		{
			confirmAction,
			cancelAction
		});
	}

	private void ShowControlsMenu()
	{
		System.Collections.Generic.List<OptionsMenu.MenuItem> list = new System.Collections.Generic.List<OptionsMenu.MenuItem>();
		list.Add(new OptionsMenu.MenuItem("手柄", (System.Action)delegate
		{
			ui.AddUI(gamepadControlsPrefab.Clone());
		}));
		list.Add(new OptionsMenu.MenuItem("键盘", (System.Action)delegate
		{
			ui.AddUI(keyboardControlsPrefab.Clone());
		}));
		list.Add(new OptionsMenu.MenuItem("重新绑定键盘", (System.Action)delegate
		{
			ui.AddUI(rebindPrefab.Clone());
		}));
		list.Add(new OptionsMenu.MenuItem("重置键盘绑定", (System.Action)delegate
		{
			KeyboardRemapper.ResetKeyboardBindings();
			this.RegisterTimer(0.01f, delegate
			{
				UnityEngine.GameObject dialogue = ui.AddUI(dialogBoxPrefab.Clone());
				UI.SetGenericText(dialogue, "按键绑定已重置");
				this.RegisterTimer(1f, delegate
				{
					if (dialogue != null)
					{
						UnityEngine.Object.Destroy(dialogue);
					}
				});
			});
		}));
		list.Add(new OptionsMenu.MenuItem("高级控制器设置", (System.Action)delegate
		{
			controllerRemapperPrefab.Clone();
		}));
		BuildSimpleMenu(list);
	}

	private void UpdateItemStartingWith(LinearMenu menu, string startsWith, string text)
	{
		UnityEngine.GameObject gameObject = System.Linq.Enumerable.FirstOrDefault(menu.GetMenuObjects(), delegate(UnityEngine.GameObject item)
		{
			TMPro.TMP_Text componentInChildren = item.GetComponentInChildren<TMPro.TMP_Text>();
			if (componentInChildren != null)
			{
				return componentInChildren.text.Contains(startsWith);
			}
			return false;
		});
		if (!(bool)gameObject)
		{
			UnityEngine.Debug.LogWarning("Could not find menu item with " + startsWith);
		}
		else
		{
			gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = text;
		}
	}

	public static void LoadGraphicsPlayerPrefs()
	{
		Player.jitterFixConfiguration = (Player.JitterFixConfiguration)UnityEngine.PlayerPrefs.GetInt("JITTER_", 0);
		LevelUI.enableFPSCounter = ((UnityEngine.PlayerPrefs.GetInt("FPS_", 0) == 1) ? true : false);
		PixelFilterAdjuster.scale = UnityEngine.PlayerPrefs.GetFloat("SCALE_", 1f);
	}
}
