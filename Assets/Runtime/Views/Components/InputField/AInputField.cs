using System;

namespace UIKit
{
    internal abstract class AInputField
    {
        public abstract string text { get; set; }

        public event Action<string> onEndEditing;

        public abstract void Clear();

        protected void OnEndEditing(string newText)
        {
            onEndEditing?.Invoke(newText);
        }
    }
}
