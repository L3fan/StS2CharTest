using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Text;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.Core.RichTextTags;

namespace StS2CharTest.CustomNodes;

public partial class CustomMegaRichTextLabel : MegaRichTextLabel
{
    public override void _Ready()
    {
        AutoSizeEnabled = false;
        base._Ready();
    }
}