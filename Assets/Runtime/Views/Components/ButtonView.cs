using System;
using JetBrains.Annotations;
using UIKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : View, IComponentAction
    {
        private Button _button = default;

        [UsedImplicitly] public Action action { get; set; }
        
        #region Life cycle
        
        protected override void Awake()
        {
            base.Awake();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(InvokeAction);
        }

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        #endregion

        private void InvokeAction() => action?.Invoke();
    }
}
