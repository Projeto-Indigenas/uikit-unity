using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;

namespace UIKit
{
    internal class LabelTMP : ALabel
    {
        private readonly TMP_Text _textLabel = default;

        public override string text 
        { 
            get => _textLabel.text; 
            set => _textLabel.text = value;
        }

        public LabelTMP(TMP_Text label)
        {
            _textLabel = label;
        }
    }
}