using System;
using UnityEngine;

namespace UIKit
{
    [Serializable]
    public class DropdownOption
    {
        [SerializeField] private string _text = default;
        [SerializeField] private Sprite _image = default;

        public string text => _text;
        public Sprite image => _image;

        protected DropdownOption(string text = null, Sprite image = null) 
        {
            _text = text;
            _image = image;
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