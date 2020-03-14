// CustomControllerProfile
public class CustomControllerProfile : InControl.UnityInputDeviceProfile
{
	public const string CUSTOM_SUFFIX = " (custom)";

	public CustomControllerConfiguration configuration;

	public string originalName => configuration.originalName;

	public CustomControllerProfile(CustomControllerConfiguration configuration)
	{
		this.configuration = configuration;
		base.Name = configuration.originalName + "（自定义）";
		base.Meta = "Custom Configuration Meta Text";
		base.Sensitivity = 1f;
		base.LowerDeadZone = 0.2f;
		SupportedPlatforms = null;
		JoystickNames = new string[0];
		System.Collections.Generic.List<InControl.InputControlMapping> list = new System.Collections.Generic.List<InControl.InputControlMapping>();
		System.Collections.Generic.List<InControl.InputControlMapping> list2 = new System.Collections.Generic.List<InControl.InputControlMapping>();
		foreach (CustomControllerConfiguration.InputMapping mapping in configuration.mappings)
		{
			if (mapping.sourceIndex.HasValue)
			{
				if (mapping.sourceType == CustomControllerConfiguration.InputType.Both)
				{
					UnityEngine.Debug.LogWarning("Source input types should not come from both! There is only one source!");
				}
				InControl.InputControlMapping inputControlMapping = new InControl.InputControlMapping
				{
					Handle = mapping.handle,
					Target = mapping.target,
					Invert = mapping.inverted,
					Source = ((mapping.sourceType == CustomControllerConfiguration.InputType.Analog) ? InControl.UnityInputDeviceProfile.Analog(mapping.sourceIndex.Value) : InControl.UnityInputDeviceProfile.Button(mapping.sourceIndex.Value)),
					SourceRange = ((mapping.sourceType == CustomControllerConfiguration.InputType.Button) ? InControl.InputControlMapping.Range.Complete : new InControl.InputControlMapping.Range
					{
						Minimum = mapping.minValue,
						Maximum = mapping.maxValue
					})
				};
				if (mapping.matchDestinationToSourceRange)
				{
					inputControlMapping.TargetRange = inputControlMapping.SourceRange;
				}
				if (mapping.sourceType == CustomControllerConfiguration.InputType.Button)
				{
					list.Add(inputControlMapping);
				}
				else
				{
					list2.Add(inputControlMapping);
				}
			}
		}
		for (int i = 0; i < 20; i++)
		{
			list2.Add(new InControl.InputControlMapping
			{
				Handle = "Analog " + i,
				Source = InControl.UnityInputDeviceProfile.Analog(i),
				Target = (InControl.InputControlType)(35 + i)
			});
		}
		base.ButtonMappings = new InControl.InputControlMapping[20];
		for (int j = 0; j < 20; j++)
		{
			list.Add(new InControl.InputControlMapping
			{
				Handle = "Button " + j,
				Source = InControl.UnityInputDeviceProfile.Button(j),
				Target = (InControl.InputControlType)(55 + j)
			});
		}
		base.ButtonMappings = list.ToArray();
		base.AnalogMappings = list2.ToArray();
	}
}
