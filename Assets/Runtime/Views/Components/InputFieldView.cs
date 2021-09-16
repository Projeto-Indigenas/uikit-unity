using System;
using TMPro;
using UIKit.Components;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldView : AView, IComponentAction<string>
    {
        private TMP_InputField _inputField = default;

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }
        
        public Action<string> action { get; set; }

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onEndEdit.AddListener(OnEndEdit);
        }

        #endregion

        private void OnEndEdit(string newValue) => action?.Invoke(newValue);
    }
}
