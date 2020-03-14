// ControllerButtonDetector
public class ControllerButtonDetector : UnityEngine.MonoBehaviour
{
	public TMPro.TMP_Text text;

	public float totalButtonDiff = 3f;

	public float totalAnalogDiff = 6f;

	public float timeout = 5f;

	private InControl.UnityInputDevice device;

	private CustomControllerConfiguration.InputMapping mapping;

	private System.Collections.Generic.Dictionary<InControl.InputControlType, float> totalInputDiff = new System.Collections.Generic.Dictionary<InControl.InputControlType, float>();

	private System.Collections.Generic.Dictionary<InControl.InputControlType, float> analogMin = new System.Collections.Generic.Dictionary<InControl.InputControlType, float>();

	private System.Collections.Generic.Dictionary<InControl.InputControlType, float> analogMax = new System.Collections.Generic.Dictionary<InControl.InputControlType, float>();

	private System.Collections.Generic.Dictionary<InControl.InputControlType, float> analogAverage = new System.Collections.Generic.Dictionary<InControl.InputControlType, float>();

	private float timeoutTime;

	private string instructions;

	private int lastTimeLeft;

	public void Setup(InControl.UnityInputDevice device, CustomControllerConfiguration.InputMapping data)
	{
		GetComponent<InputTestBox>().Setup(device);
		this.device = device;
		mapping = data;
		timeoutTime = UnityEngine.Time.time + timeout;
		instructions = "按下按键3次";
		if (data.targetInputType == CustomControllerConfiguration.InputType.Analog)
		{
			if (data.target.ToString().Contains("X"))
			{
				instructions = "wiggle the stick left and right as far as it will go";
			}
			else if (data.target.ToString().Contains("Y"))
			{
				instructions = "wiggle the stick up and down as far as it will go";
			}
			else
			{
				instructions = "wiggle the stick back and forth as far as it will go";
			}
		}
		text.text = instructions;
	}

	private void Update()
	{
		int num = (int)(timeoutTime - UnityEngine.Time.time);
		if (num != lastTimeLeft)
		{
			text.text = instructions + " (" + num + ")";
		}
		lastTimeLeft = num;
		if ((mapping.targetInputType & CustomControllerConfiguration.InputType.Analog) == CustomControllerConfiguration.InputType.Analog)
		{
			for (int i = 0; i < 20; i++)
			{
				InControl.InputControlType inputControlType = (InControl.InputControlType)(35 + i);
				InControl.InputControl control = device.GetControl(inputControlType);
				float num2 = control.Value - control.LastValue;
				if (num2 == 0f)
				{
					continue;
				}
				if (!totalInputDiff.ContainsKey(inputControlType))
				{
					totalInputDiff.Add(inputControlType, 0f);
					analogMax.Add(inputControlType, 0.2f);
					analogMin.Add(inputControlType, -0.2f);
					analogAverage.Add(inputControlType, 0f);
				}
				analogMax[inputControlType] = UnityEngine.Mathf.Max(analogMax[inputControlType], control.Value);
				analogMin[inputControlType] = UnityEngine.Mathf.Min(analogMin[inputControlType], control.Value);
				System.Collections.Generic.Dictionary<InControl.InputControlType, float> dictionary = totalInputDiff;
				InControl.InputControlType key = inputControlType;
				dictionary[key] += UnityEngine.Mathf.Abs(num2);
				dictionary = analogAverage;
				key = inputControlType;
				dictionary[key] += control.Value;
				if (totalInputDiff[inputControlType] >= totalAnalogDiff)
				{
					if (mapping.targetInputType == CustomControllerConfiguration.InputType.Both)
					{
						float v = (analogAverage[inputControlType] < 0f) ? (-1) : 0;
						float v2 = (!(analogAverage[inputControlType] < 0f)) ? 1 : 0;
						mapping.inverted = (analogAverage[inputControlType] < 0f);
						mapping.matchDestinationToSourceRange = true;
						SelectAxis(i, v, v2);
					}
					else
					{
						float num3 = UnityEngine.Mathf.Min(UnityEngine.Mathf.Abs(analogMin[inputControlType]), analogMax[inputControlType]);
						SelectAxis(i, 0f - num3, num3);
					}
				}
			}
		}
		if ((mapping.targetInputType & CustomControllerConfiguration.InputType.Button) == CustomControllerConfiguration.InputType.Button)
		{
			for (int j = 0; j < 20; j++)
			{
				InControl.InputControlType inputControlType2 = (InControl.InputControlType)(55 + j);
				InControl.InputControl control2 = device.GetControl(inputControlType2);
				float num4 = control2.Value - control2.LastValue;
				if (num4 > 0f)
				{
					if (!totalInputDiff.ContainsKey(inputControlType2))
					{
						totalInputDiff.Add(inputControlType2, 0f);
					}
					System.Collections.Generic.Dictionary<InControl.InputControlType, float> dictionary = totalInputDiff;
					InControl.InputControlType key = inputControlType2;
					dictionary[key] += UnityEngine.Mathf.Abs(num4);
					if (totalInputDiff[inputControlType2] >= totalButtonDiff)
					{
						SelectButton(j);
					}
				}
			}
		}
		if (UnityEngine.Time.time > timeoutTime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void SelectAxis(int i, float v1, float v2)
	{
		mapping.sourceIndex = i;
		mapping.minValue = v1;
		mapping.maxValue = v2;
		mapping.sourceType = CustomControllerConfiguration.InputType.Analog;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void SelectButton(int i)
	{
		mapping.sourceIndex = i;
		mapping.sourceType = CustomControllerConfiguration.InputType.Button;
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
