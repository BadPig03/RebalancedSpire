namespace RebalancedSpire.Core.Nodes.Screens.MainMenu;

using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

public partial class NModChangelogsButton : NButton
{
    private ShaderMaterial? _hsv;
    private Control? _icon;

    public static NModChangelogsButton Create()
    {
        var button = PreloadManager.Cache.GetScene("res://scenes/screens/main_menu/mod_changelogs_button.tscn").Instantiate<NModChangelogsButton>();
        button.OffsetLeft = -1905;
        button.OffsetTop = 1001;
        button.OffsetRight = -1841;
        button.OffsetBottom = 1065;
        return button;
    }

    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNode<TextureRect>("Icon");
        _hsv = (ShaderMaterial) _icon.Material;
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        _hsv?.SetShaderParameter("v", 1.2f);
        _icon?.SetRotationDegrees(5f);
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        _hsv?.SetShaderParameter("v", 1f);
        _icon?.SetRotationDegrees(0);
    }
}