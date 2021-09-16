using System;
using TMPro;
using UnityEngine;

namespace UIKit
{
    [Serializable]
    public class DropdownOption
    {
        public readonly string text;
        public readonly Sprite image;

        protected DropdownOption(string text = null, Sprite image = null) 
        {
            this.text = text;
            this.image = image;
        }
    }

    public class DropdownOption<TValue> : DropdownOption
    {
        public readonly TValue value;

        public DropdownOption(TValue value, string text, Sprite image = null) 
            : base(text, image)
        {
            this.value = value;
        }
    }
}