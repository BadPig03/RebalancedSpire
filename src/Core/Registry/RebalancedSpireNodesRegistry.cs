namespace RebalancedSpire.Core.Registry;

using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using Nodes.Screens.MainMenu;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;

internal sealed class RebalancedSpireNodesRegistry
{
    internal static void Initialize()
    {
        var registry = ModNodeAttachmentRegistry.For(RebalancedSpireMain.ModId);
        registry.RegisterReadyChild<NMainMenu, NModChangelogsScreen>("ModChangelogsScreen", _ =>
            {
                var screenNode = NModChangelogsScreen.Create();
                screenNode.SetVisible(false);
                return screenNode;
            },
            new NodeAttachmentOptions
            {
                Name = "ModChangelogsScreen",
                UniqueNameInOwner = true,
                AddMode = NodeAttachmentAddMode.AddChildDirect
            }
        );
        registry.RegisterReadyChild<NMainMenu, NModChangelogsButton>("ModChangelogsButton", m =>
            {
                var buttonNode = NModChangelogsButton.Create();
                buttonNode.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(_ =>
                {
                    if (!registry.TryGetAttached<NMainMenu, NModChangelogsScreen>(m, "ModChangelogsScreen", out var screenNode))
                    {
                        return;
                    }

                    screenNode.Open();
                }));
                return buttonNode;
            },
            new NodeAttachmentOptions
            {
                Name = "ModChangelogsButton",
                UniqueNameInOwner = true,
                AddMode = NodeAttachmentAddMode.AddChildDirect
            }
        );
    }
}