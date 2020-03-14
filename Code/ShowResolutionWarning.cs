// OptionsMenu
private void ShowResolutionWarning()
{
	UI.SetGenericText(ui.AddUI(dialogBoxPrefab.Clone()), "<sprite name=\"Warning\" tint=1> 本游戏的画面效果是以\n默认像素度来设计的。\n因此这些功能是\n以能用为目的加进来的。");
}
