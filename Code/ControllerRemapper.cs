// ControllerRemapper
public class ControllerRemapper : UnityEngine.MonoBehaviour
{
	public static bool remapWarning;

	public UnityEngine.GameObject dialogBoxPrefab;

	public UnityEngine.GameObject testInputPrefab;

	public UnityEngine.GameObject collectionListPrefab;

	public UnityEngine.GameObject detectInputPrefab;

	public UnityEngine.GameObject textInputPrefab;

	private LinearMenu mainMenu;

	private UI ui;

	private void Start()
	{
		ui = Singleton<ServiceLocator>.instance.Locate<UI>();
		BuildMainMenu();
	}

	private void BuildMainMenu()
	{
		System.Collections.Generic.IEnumerable<InControl.UnityInputDevice> source = System.Linq.Enumerable.Cast<InControl.UnityInputDevice>(System.Linq.Enumerable.Where(InControl.InputManager.Devices, delegate(InControl.InputDevice device)
		{
			if (device.Name != "Keyboard")
			{
				return device is InControl.UnityInputDevice;
			}
			return false;
		}));
		bool flag = System.Linq.Enumerable.Any(Singleton<CustomControllerManager>.instance.disabledDevices);
		bool flag2 = !System.Linq.Enumerable.Any(source);
		System.Collections.Generic.IEnumerable<InControl.UnityInputDevice> ignoredDevices = Singleton<CustomControllerManager>.instance.ignoredDevices;
		int num = System.Linq.Enumerable.Count(ignoredDevices);
		if (flag2 && !flag && num == 0)
		{
			UnityEngine.GameObject gameObject = ui.AddUI(dialogBoxPrefab.Clone());
			UI.SetGenericText(gameObject, "没检测到控制器！");
			gameObject.RegisterOnDestroyCallback(delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			return;
		}
		System.Collections.Generic.List<string> list = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(source, (InControl.UnityInputDevice d) => d.JoystickId + ": " + TrimName(d.Name)));
		System.Collections.Generic.List<System.Action> list2 = System.Linq.Enumerable.ToList(System.Linq.Enumerable.Select(source, (System.Func<InControl.UnityInputDevice, System.Action>)delegate(InControl.UnityInputDevice device)
		{
			ControllerRemapper controllerRemapper3 = this;
			return delegate
			{
				controllerRemapper3.ConfigureDevice(device);
			};
		}));
		if (num > 0)
		{
			list.AddRange(System.Linq.Enumerable.Select(ignoredDevices, (InControl.UnityInputDevice n) => n.JoystickId + ": " + TrimName(n.Name)));
			list2.AddRange(System.Linq.Enumerable.Select(ignoredDevices, (System.Func<InControl.UnityInputDevice, System.Action>)delegate(InControl.UnityInputDevice name)
			{
				ControllerRemapper controllerRemapper2 = this;
				return delegate
				{
					LinearMenu submenu2 = null;
					submenu2 = controllerRemapper2.ShowMenu(new string[1]
					{
						"取消忽略"
					}, new System.Action[1]
					{
						delegate
						{
							Singleton<CustomControllerManager>.instance.IgnoreDevice(name, ignore: false);
							submenu2.Kill();
							controllerRemapper2.RefreshMainMenu();
						}
					});
				};
			}));
		}
		if (flag)
		{
			System.Collections.Generic.IEnumerable<string> disabledDevices = Singleton<CustomControllerManager>.instance.disabledDevices;
			list.AddRange(System.Linq.Enumerable.Select(disabledDevices, (string n) => TrimName(n)));
			list2.AddRange(System.Linq.Enumerable.Select(disabledDevices, (System.Func<string, System.Action>)delegate(string name)
			{
				ControllerRemapper controllerRemapper = this;
				return delegate
				{
					LinearMenu submenu = null;
					submenu = controllerRemapper.ShowMenu(new string[1]
					{
						"重新启用"
					}, new System.Action[1]
					{
						delegate
						{
							Singleton<CustomControllerManager>.instance.EnableDeviceName(name);
							submenu.Kill();
							controllerRemapper.RefreshMainMenu();
						}
					});
				};
			}));
		}
		LinearMenu linearMenu = ShowMenu(list, list2);
		linearMenu.onKill += delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		};
		mainMenu = linearMenu;
		if (!flag2)
		{
			UnityEngine.GameObject gameObject2 = ui.CreateTextMenuItem("检测到的设备");
			gameObject2.transform.SetParent(linearMenu.transform, worldPositionStays: false);
			gameObject2.transform.SetAsFirstSibling();
		}
		if (num > 0)
		{
			UnityEngine.GameObject gameObject3 = ui.CreateTextMenuItem("忽略的设备");
			gameObject3.transform.SetParent(linearMenu.transform, worldPositionStays: false);
			gameObject3.transform.SetSiblingIndex(gameObject3.transform.parent.childCount - 1 - System.Linq.Enumerable.Count(Singleton<CustomControllerManager>.instance.disabledDevices) - num);
		}
		if (flag)
		{
			UnityEngine.GameObject gameObject4 = ui.CreateTextMenuItem("禁用的设备类型");
			gameObject4.transform.SetParent(linearMenu.transform, worldPositionStays: false);
			gameObject4.transform.SetSiblingIndex(gameObject4.transform.parent.childCount - 1 - System.Linq.Enumerable.Count(Singleton<CustomControllerManager>.instance.disabledDevices));
		}
	}

	private string TrimName(string name)
	{
		System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(name, "(.+)(（自定义）)");
		if (match.Groups.Count == 3)
		{
			string value = match.Groups[1].Value;
			return Ellipsis(value, 21) + match.Groups[2].Value;
		}
		return Ellipsis(name, 21);
	}

	private string Ellipsis(string name, int maxLength)
	{
		if (name.Length > maxLength)
		{
			return name.Substring(0, maxLength) + "...";
		}
		return name;
	}

	private void ConfigureDevice(InControl.UnityInputDevice device)
	{
		UI ui = Singleton<ServiceLocator>.instance.Locate<UI>();
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		System.Collections.Generic.List<System.Action> list2 = new System.Collections.Generic.List<System.Action>();
		LinearMenu menu = null;
		CustomControllerProfile customControllerProfile = device.Profile as CustomControllerProfile;
		string originalDeviceName = (customControllerProfile != null) ? customControllerProfile.originalName : device.Name;
		list.Add("自定义");
		list2.Add(delegate
		{
			CustomizeDevice(device);
			menu.Kill();
		});
		list.Add("测试输入");
		list2.Add(delegate
		{
			ui.AddUI(testInputPrefab.Clone()).GetComponent<InputTestBox>().Setup(device);
			menu.Kill();
		});
		if (customControllerProfile != null)
		{
			list.Add("还原自定义设置");
			list2.Add(delegate
			{
				Singleton<CustomControllerManager>.instance.SetReplacementConfiguration(originalDeviceName, null);
				menu.Kill();
				RefreshMainMenu();
			});
		}
		list.Add("忽略");
		list2.Add(delegate
		{
			Singleton<CustomControllerManager>.instance.IgnoreDevice(device);
			menu.Kill();
			RefreshMainMenu();
		});
		list.Add("禁用设备类型");
		list2.Add(delegate
		{
			Singleton<CustomControllerManager>.instance.DisableDeviceName(originalDeviceName);
			menu.Kill();
			RefreshMainMenu();
		});
		menu = ShowMenu(list, list2);
	}

	private void RefreshMainMenu()
	{
		UnityEngine.Object.Destroy(mainMenu.gameObject);
		BuildMainMenu();
	}

	public void CustomizeDevice(InControl.UnityInputDevice device)
	{
		UI uI = Singleton<ServiceLocator>.instance.Locate<UI>();
		CustomControllerConfiguration config = null;
		CustomControllerProfile customProfile = device.Profile as CustomControllerProfile;
		if (customProfile != null)
		{
			config = customProfile.configuration;
		}
		else
		{
			config = new CustomControllerConfiguration(device.Profile);
			Singleton<CustomControllerManager>.instance.SetReplacementConfiguration(device.Name, config);
		}
		InControl.UnityInputDevice customDevice = (device.Profile is CustomControllerProfile) ? device : Singleton<CustomControllerManager>.instance.FindReplacementDevice(device);
		CollectionListUI component = uI.AddUI(collectionListPrefab.Clone()).GetComponent<CollectionListUI>();
		component.Setup(config.mappings, delegate(CustomControllerConfiguration.InputMapping data, CollectionListUIElement element)
		{
			StylizeControlElement(data, element);
			element.onConfirm += delegate
			{
				OnControlSelected(customDevice, data, element);
			};
		});
		component.gameObject.RegisterOnDestroyCallback(delegate
		{
			string deviceName = (customProfile != null) ? customProfile.originalName : device.Name;
			Singleton<CustomControllerManager>.instance.SetReplacementConfiguration(deviceName, config);
			RefreshMainMenu();
		});
		if (!remapWarning)
		{
			remapWarning = true;
			UI.SetGenericText(uI.AddUI(dialogBoxPrefab.Clone()), "变更会在该菜单关闭后生效。");
		}
	}

	private static void StylizeControlElement(CustomControllerConfiguration.InputMapping data, CollectionListUIElement element)
	{
		string inputTargetName = GetInputTargetName(data.target);
		string str = "空";
		if (data.sourceIndex.HasValue)
		{
			str = ((data.sourceType == CustomControllerConfiguration.InputType.Analog) ? "手柄轴 " : "按键 ") + data.sourceIndex.Value;
		}
		str += "<color=#BC6320>";
		if (data.inverted)
		{
			str += "（反转）";
		}
		if ((double)data.maxValue < 0.9 || data.minValue > -0.9f)
		{
			str += $" ({UnityEngine.Mathf.Max(UnityEngine.Mathf.Abs(data.minValue), data.maxValue):0.0})";
		}
		if (data.targetInputType == CustomControllerConfiguration.InputType.Button)
		{
			str = str + "（" + QuickUnityTools.Input.GameUserInput.WrapButtonNameForText(null, data.handle) + "）";
		}
		str += "</color>";
		string text = $"<color=yellow>{inputTargetName}:</color> {str}";
		element.text.text = text;
		element.image.gameObject.SetActive(value: false);
	}

	private static string GetInputTargetName(InControl.InputControlType target)
	{
		switch (target)
		{
		case InControl.InputControlType.Action1:
			return "跳跃/选择";
		case InControl.InputControlType.Action2:
			return "使用道具/返回";
		case InControl.InputControlType.Action3:
			return "奔跑";
		case InControl.InputControlType.Action4:
			return "快速菜单";
		case InControl.InputControlType.Menu:
			return "开始";
		default:
			return System.Text.RegularExpressions.Regex.Replace(target.ToString(), "([a-z])([A-Z0-9])", "$1 $2").Trim();
		}
	}

	private void OnControlSelected(InControl.UnityInputDevice device, CustomControllerConfiguration.InputMapping data, CollectionListUIElement element)
	{
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		System.Collections.Generic.List<System.Action> list2 = new System.Collections.Generic.List<System.Action>();
		LinearMenu menu = null;
		list.Add("设置");
		list2.Add(delegate
		{
			ControllerButtonDetector component = ui.AddUI(detectInputPrefab.Clone()).GetComponent<ControllerButtonDetector>();
			component.Setup(device, data);
			component.gameObject.RegisterOnDestroyCallback(delegate
			{
				StylizeControlElement(data, element);
			});
			menu.Kill();
		});
		if (data.targetInputType == CustomControllerConfiguration.InputType.Analog)
		{
			list.Add("反转");
			list2.Add(delegate
			{
				data.inverted = !data.inverted;
				StylizeControlElement(data, element);
				menu.Kill();
			});
		}
		if (data.targetInputType == CustomControllerConfiguration.InputType.Button)
		{
			list.Add("重命名");
			list2.Add(delegate
			{
				ShowRenameMenu(data, element, menu);
			});
		}
		if (data.sourceIndex.HasValue)
		{
			list.Add("取消绑定");
			list2.Add(delegate
			{
				data.sourceIndex = null;
				StylizeControlElement(data, element);
				menu.Kill();
			});
		}
		menu = ShowMenu(list, list2);
	}

	private void ShowRenameMenu(CustomControllerConfiguration.InputMapping data, CollectionListUIElement element, LinearMenu parentMenu)
	{
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		System.Collections.Generic.List<System.Action> list2 = new System.Collections.Generic.List<System.Action>();
		LinearMenu menu = null;
		list.Add("选择图标");
		list2.Add(delegate
		{
			ShowIconMenu(data, element);
			menu.Kill();
			parentMenu.Kill();
		});
		list.Add("输入名称");
		list2.Add(delegate
		{
			ui.AddUI(textInputPrefab.Clone()).GetComponent<TextInput>().Setup("输入按键首字母并按回车键", 8, delegate(string label)
			{
				if (!string.IsNullOrEmpty(label))
				{
					data.handle = label;
					StylizeControlElement(data, element);
				}
			});
			menu.Kill();
			parentMenu.Kill();
		});
		menu = ShowMenu(list, list2);
	}

	private void ShowIconMenu(CustomControllerConfiguration.InputMapping data, CollectionListUIElement element)
	{
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		System.Collections.Generic.List<System.Action> list2 = new System.Collections.Generic.List<System.Action>();
		LinearMenu menu = null;
		foreach (string icon in new System.Collections.Generic.List<string>
		{
			"A",
			"B",
			"X",
			"Y",
			"Square",
			"Triangle",
			"Circle"
		})
		{
			list.Add(QuickUnityTools.Input.GameUserInput.WrapButtonNameForText(null, icon));
			list2.Add(delegate
			{
				data.handle = icon;
				StylizeControlElement(data, element);
				menu.Kill();
			});
		}
		menu = ShowMenu(list, list2);
	}

	private LinearMenu ShowMenu(System.Collections.Generic.IEnumerable<string> names, System.Collections.Generic.IEnumerable<System.Action> actions)
	{
		LinearMenu linearMenu = Singleton<ServiceLocator>.instance.Locate<UI>().CreateSimpleMenu(System.Linq.Enumerable.ToArray(names), System.Linq.Enumerable.ToArray(actions));
		OptionsMenu.PositionMenu(linearMenu.transform);
		return linearMenu;
	}
}
