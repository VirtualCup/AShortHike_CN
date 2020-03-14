// KeyboardRemapper
public class KeyboardRemapper : UnityEngine.MonoBehaviour
{
	public static readonly System.Collections.Generic.List<string> KEY_PROMPTS = new System.Collections.Generic.List<string>
	{
		"UP",
		"DOWN",
		"LEFT",
		"RIGHT",
		"JUMP/INTERACT",
		"USE ITEM/BACK",
		"RUN",
		"MENU"
	};

	public TMPro.TMP_Text text;

	private void Start()
	{
		StartCoroutine(DetectKeys());
	}

	private System.Collections.IEnumerator DetectKeys()
	{
		UnityEngine.KeyCode[] keys = System.Linq.Enumerable.ToArray((System.Collections.Generic.IEnumerable<UnityEngine.KeyCode>)System.Enum.GetValues(typeof(UnityEngine.KeyCode)));
		System.Collections.Generic.List<UnityEngine.KeyCode> pickedKeys = new System.Collections.Generic.List<UnityEngine.KeyCode>();
		for (int index = 0; index < KEY_PROMPTS.Count; index++)
		{
			text.text = "按下<color=red>" + KEY_PROMPTS[index] + "</color>键";
			UnityEngine.KeyCode foundKey = (UnityEngine.KeyCode)(-1);
			while (foundKey == (UnityEngine.KeyCode)(-1))
			{
				yield return new UnityEngine.WaitUntil(() => UnityEngine.Input.anyKeyDown);
				UnityEngine.KeyCode[] array = keys;
				foreach (UnityEngine.KeyCode keyCode in array)
				{
					if (UnityEngine.Input.GetKeyDown(keyCode) && !pickedKeys.Contains(keyCode))
					{
						foundKey = keyCode;
						break;
					}
				}
			}
			text.text = "你选择了：<color=red>" + foundKey.ToString() + "</color>";
			yield return new UnityEngine.WaitForSeconds(0.5f);
			pickedKeys.Add(foundKey);
		}
		SetKeyboardMapping(pickedKeys);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public static System.Collections.Generic.IList<UnityEngine.KeyCode> LoadPlayerPrefsKeyboardMapping()
	{
		System.Collections.Generic.List<UnityEngine.KeyCode> list = new System.Collections.Generic.List<UnityEngine.KeyCode>();
		for (int i = 0; i < KEY_PROMPTS.Count; i++)
		{
			int @int = UnityEngine.PlayerPrefs.GetInt("KEY_" + i, -1);
			if (@int == -1)
			{
				return null;
			}
			list.Add((UnityEngine.KeyCode)@int);
		}
		return list;
	}

	public static void SetKeyboardMapping(System.Collections.Generic.IList<UnityEngine.KeyCode> newKeys)
	{
		InControl.InputManager.DetachDevice(System.Linq.Enumerable.First(InControl.InputManager.Devices, (InControl.InputDevice d) => d.Name == "Keyboard"));
		InControl.InputManager.AttachDevice(new InControl.UnityInputDevice(new KeyboardAndMouseProfile(newKeys[0], newKeys[1], newKeys[2], newKeys[3], newKeys[4], newKeys[5], newKeys[6], newKeys[7])));
		for (int i = 0; i < newKeys.Count; i++)
		{
			UnityEngine.PlayerPrefs.SetInt("KEY_" + i, (int)newKeys[i]);
		}
	}

	public static void ResetKeyboardBindings()
	{
		for (int i = 0; i < KEY_PROMPTS.Count; i++)
		{
			UnityEngine.PlayerPrefs.SetInt("KEY_" + i, -1);
		}
		InControl.InputManager.DetachDevice(System.Linq.Enumerable.First(InControl.InputManager.Devices, (InControl.InputDevice d) => d.Name == "Keyboard"));
		InControl.InputManager.AttachDevice(new InControl.UnityInputDevice(new KeyboardAndMouseProfile()));
	}
}
