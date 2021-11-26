using UnityEngine.UI;

namespace UIKit
{
    internal class LabelUI : ALabel
    {
        private readonly Text _textLabel = default;

        public override string text
        {
            get => _textLabel.text; 
            set => _textLabel.text = value;
        }

        public LabelUI(Text label)
        {
            _textLabel = label;
        }
    }
}