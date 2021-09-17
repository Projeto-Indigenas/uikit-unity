using System;

namespace UIKit
{
    internal abstract class AInputField
    {
        public abstract string text { get; set; }

        public event Action<string> onEndEditing;

        protected void OnEndEditing(string newText)
        {
            onEndEditing?.Invoke(newText);
        }
    }
}
