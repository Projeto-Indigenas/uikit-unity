using System;
using TMPro;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldView : View
    {
        private TMP_InputField _inputField = default;

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }
        
        public Action<string> DidEndEditing { get; set; }

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onEndEdit.AddListener(OnEndEdit);
        }

        #endregion

        private void OnEndEdit(string newValue) => DidEndEditing?.Invoke(newValue);
    }
}
