using System;

namespace UIKit
{
    internal abstract class AInputField
    {
        public abstract string text { get; set; }

        public abstract event Action<string> onEndEditing;
    }
}
