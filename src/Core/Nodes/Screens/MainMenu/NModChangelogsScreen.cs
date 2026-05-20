namespace RebalancedSpire.Core.Nodes.Screens.MainMenu;

using System.Globalization;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

public partial class NModChangelogsScreen : Control, IScreenContext
{
    private const string ArrowTexturePath = "res://images/packed/common_ui/settings_tiny_left_arrow.png";
    private const string ShaderPath = "res://shaders/hsv.gdshader";

    private static readonly AddedNode<NModChangelogsScreen, NBackButton> BackButtonNode = new(_ =>
    {
        var button = PreloadManager.Cache.GetScene("res://scenes/ui/back_button.tscn").Instantiate<NBackButton>();
        button.Name = "BackButton";
        button.UniqueNameInOwner = true;
        button.LayoutMode = 1;
        return button;
    });
    private static readonly AddedNode<NModChangelogsScreen, NGoldArrowButton> PrevButtonNode = new(_ =>
    {
        var button = new NGoldArrowButton();
        button.Name = "PrevButton";
        button.CustomMinimumSize = new Vector2(128, 128);
        button.LayoutMode = 1;
        button.AnchorsPreset = 8;
        button.AnchorLeft = 0.5f;
        button.AnchorTop = 0.5f;
        button.AnchorRight = 0.5f;
        button.AnchorBottom = 0.5f;
        button.OffsetLeft = -720;
        button.OffsetTop = -64;
        button.OffsetRight = -592;
        button.OffsetBottom = 64;
        button.GrowHorizontal = GrowDirection.Both;
        button.GrowVertical = GrowDirection.Both;
        button.PivotOffset = new Vector2(72, 94);

        var material = new ShaderMaterial();
        material.ResourceLocalToScene = true;
        material.Shader = ResourceLoader.Load<Shader>(ShaderPath);
        material.SetShaderParameter("h", 1);
        material.SetShaderParameter("s", 1);
        material.SetShaderParameter("v", 1);

        var texRect = new TextureRect();
        texRect.Name = "TextureRect";
        texRect.Material = material;
        texRect.LayoutMode = 1;
        texRect.AnchorsPreset = 15;
        texRect.AnchorRight = 1;
        texRect.AnchorBottom = 1;
        texRect.GrowHorizontal = GrowDirection.Both;
        texRect.GrowVertical = GrowDirection.Both;
        texRect.PivotOffset = new Vector2(64, 64);
        texRect.MouseFilter = MouseFilterEnum.Ignore;
        texRect.Texture = ResourceLoader.Load<Texture2D>(ArrowTexturePath);
        texRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        texRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        button.AddChild(texRect);
        return button;
    });
    private static readonly AddedNode<NModChangelogsScreen, NGoldArrowButton> NextButtonNode = new(_ =>
    {
        var button = new NGoldArrowButton();
        button.Name = "NextButton";
        button.CustomMinimumSize = new Vector2(128, 128);
        button.LayoutMode = 1;
        button.AnchorsPreset = 8;
        button.AnchorLeft = 0.5f;
        button.AnchorTop = 0.5f;
        button.AnchorRight = 0.5f;
        button.AnchorBottom = 0.5f;
        button.OffsetLeft = 592;
        button.OffsetTop = -64;
        button.OffsetRight = 720;
        button.OffsetBottom = 64;
        button.GrowHorizontal = GrowDirection.Both;
        button.GrowVertical = GrowDirection.Both;
        button.PivotOffset = new Vector2(72, 94);

        var material = new ShaderMaterial();
        material.ResourceLocalToScene = true;
        material.Shader = ResourceLoader.Load<Shader>(ShaderPath);
        material.SetShaderParameter("h", 1);
        material.SetShaderParameter("s", 1);
        material.SetShaderParameter("v", 1);

        var texRect = new TextureRect();
        texRect.Name = "TextureRect";
        texRect.Material = material;
        texRect.LayoutMode = 1;
        texRect.AnchorsPreset = 15;
        texRect.AnchorRight = 1;
        texRect.AnchorBottom = 1;
        texRect.GrowHorizontal = GrowDirection.Both;
        texRect.GrowVertical = GrowDirection.Both;
        texRect.PivotOffset = new Vector2(56, 64);
        texRect.MouseFilter = MouseFilterEnum.Ignore;
        texRect.Texture = ResourceLoader.Load<Texture2D>(ArrowTexturePath);
        texRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        texRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        texRect.FlipH = true;
        button.AddChild(texRect);
        return button;
    });

    public Control? DefaultFocusedControl => null;

    private NScrollableContainer? _screenContents;
    private MarginContainer? _marginContainer;
    private MegaRichTextLabel? _changelogsText;
    private MegaLabel? _dateLabel;
    private NBackButton? _backButton;
    private NButton? _prevButton;
    private NButton? _nextButton;
    private NButton? _changelogsButton;
    private PackedScene? _cachedScene;
    private Tween? _tween;

    private List<string>? _changelogsPaths;
    private int _currentScrollLine;
    private int _index;
    private bool _isOpen;

    public static NModChangelogsScreen Create()
    {
        return PreloadManager.Cache.GetScene("res://scenes/screens/mod_changelogs_screen.tscn").Instantiate<NModChangelogsScreen>();
    }

    public override void _Ready()
    {
        _cachedScene = ResourceLoader.Load<PackedScene>("res://scenes/screens/patch_screen_contents.tscn");
        CreateNewUpdateEntry();

        _prevButton = PrevButtonNode.Get(this);
        _prevButton?.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ =>
        {
            PreviousChangelog();
        }));
        AddChild(_prevButton);

        _nextButton = NextButtonNode.Get(this);
        _nextButton?.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ =>
        {
            NextChangelog();
        }));
        _nextButton?.SetVisible(false);
        AddChild(_nextButton);

        _changelogsButton = GetNode<NButton>("%ModChangelogsToggle");
        _changelogsButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => { Close(); }));
        _changelogsButton.SetVisible(false);
        _changelogsButton.Disable();

        _backButton = BackButtonNode.Get(this);
        _backButton?.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ => { Close(); }));
        AddChild(_backButton);
    }

    public void Open()
    {
        _isOpen = true;
        NGame.Instance?.MainMenu?.EnableBackstop();
        _changelogsButton?.Enable();
        _changelogsButton?.SetVisible(true);
        _backButton?.Enable();
        SetVisible(true);
        _tween?.FastForwardToCompletion();
        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(this, "modulate:a", 1f, 0.25);
        _changelogsPaths ??= (from fileName in DirAccess.GetFilesAt("res://RebalancedSpire/localization/zhs/changelogs") select "res://RebalancedSpire/localization/zhs/changelogs/" + fileName).Reverse().ToList();
        LoadChangelogsText(_changelogsPaths[_index]);
        ActiveScreenContext.Instance.Update();
        NHotkeyManager.Instance?.PushHotkeyReleasedBinding(MegaInput.left, PreviousChangelog);
        NHotkeyManager.Instance?.PushHotkeyReleasedBinding(MegaInput.right, NextChangelog);
        NHotkeyManager.Instance?.PushHotkeyReleasedBinding(MegaInput.pauseAndBack, Close);
    }

    private void Close()
    {
        NHotkeyManager.Instance?.RemoveHotkeyReleasedBinding(MegaInput.left, PreviousChangelog);
        NHotkeyManager.Instance?.RemoveHotkeyReleasedBinding(MegaInput.right, NextChangelog);
        NHotkeyManager.Instance?.RemoveHotkeyReleasedBinding(MegaInput.pauseAndBack, Close);
        _changelogsButton?.Disable();
        _changelogsButton?.SetVisible(false);
        _backButton?.Disable();
        NGame.Instance?.MainMenu?.DisableBackstop();
        _tween?.FastForwardToCompletion();
        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(this, "modulate:a", 0f, 0.25);
        _tween.TweenCallback(Callable.From(() =>
        {
            _isOpen = false;
            SetVisible(false);
            ActiveScreenContext.Instance.Update();
        }));
    }

    private void CreateNewUpdateEntry()
    {
        _screenContents = _cachedScene?.Instantiate<NScrollableContainer>();
        AddChild(_screenContents);
        MoveChild(_screenContents, 0);
        _marginContainer = _screenContents?.GetNode<MarginContainer>("Content");
        _changelogsText = _screenContents?.GetNode<MegaRichTextLabel>("Content/PatchText");
        _dateLabel = _changelogsText?.GetNode<MegaLabel>("DateLabel");
        if (_changelogsPaths == null)
        {
            return;
        }

        var changelogsPath = _changelogsPaths[_index];
        LoadChangelogsText(changelogsPath);
    }

    private void NextChangelog()
    {
        if (_nextButton?.IsVisible() == false || _changelogsPaths == null)
        {
            return;
        }

        _index--;
        _prevButton?.SetVisible(true);
        if (_index == 0)
        {
            _nextButton?.SetVisible(false);
        }
        _screenContents?.QueueFreeSafely();
        CreateNewUpdateEntry();
    }

    private void PreviousChangelog()
    {
        if (_prevButton?.IsVisible() == false || _changelogsPaths == null)
        {
            return;
        }

        _index++;
        _nextButton?.SetVisible(true);
        if (_index == _changelogsPaths.Count - 1)
        {
            _prevButton?.SetVisible(false);
        }
        _screenContents?.QueueFreeSafely();
        CreateNewUpdateEntry();
    }

    private void LoadChangelogsText(string changelogPath)
    {
        _changelogsText?.ScrollToLine(0);
        _currentScrollLine = 0;
        var textAutoSize = ReadChangelogsFile(changelogPath);
        _changelogsText?.SetTextAutoSize(textAutoSize);
        UpdateDateLabel(changelogPath);
    }

    private static string ReadChangelogsFile(string endChangelogPath)
    {
        var language = LocManager.Instance.Language;
        if (language != "zhs")
        {
            var text = "res://RebalancedSpire/localization/" + language + "/changelogs/" + GetFileNameFromPath(endChangelogPath);
            if (FileAccess.FileExists(text))
            {
                using FileAccess engNotes = FileAccess.Open(text, FileAccess.ModeFlags.Read);
                if (engNotes != null)
                {
                    return engNotes.GetAsText();
                }
            }
        }
        using FileAccess otherLangNotes = FileAccess.Open(endChangelogPath, FileAccess.ModeFlags.Read);
        return otherLangNotes != null ? otherLangNotes.GetAsText() : "";
    }

    private void UpdateDateLabel(string patchNotePath)
    {
        var fileNameFromPath = GetFileNameFromPath(patchNotePath);
        var text = RemoveFileExtension(fileNameFromPath);
        if (!TryParseDate(text, out var formattedDate))
        {
            return;
        }

        _dateLabel?.SetTextAutoSize(formattedDate);
    }

    private static string GetFileNameFromPath(string path)
    {
        var num = path.LastIndexOf('/') + 1;
        return path[num..];
    }

    private static string RemoveFileExtension(string fileName)
    {
        return fileName.Split('.')[0];
    }

    private static bool TryParseDate(string dateString, out string formattedDate)
    {
        if (DateTime.TryParseExact(dateString, "yyyy_MM_d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            formattedDate = result.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
            return true;
        }
        formattedDate = string.Empty;
        return false;
    }
}