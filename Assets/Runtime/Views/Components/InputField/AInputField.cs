using System;
using UnityEngine;

namespace UIKit
{
    internal abstract class AInputField
    {
        public abstract string text { get; set; }

        public event Action<string> didEndEditing;
        public event Action<string> valueDidChange;
        public event Func<string, int, char, char> validateInput;

        public abstract void Clear();

        protected void DidEndEditing(string newText) => didEndEditing?.Invoke(newText);
        protected void ValueDidChange(string newText) => valueDidChange?.Invoke(newText);
        protected char ValidateInput(string text, int charIndex, char addedChar) 
            => validateInput.Invoke(text, charIndex, addedChar);
    }
}
