using System;
using UnityEngine;

namespace UIKit.Components
{
    [Serializable]
    public class ComponentBinding
    {
        [SerializeField] protected View _view;

        public View view
        {
            get => _view;
            private set => _view = value;
        }
    }

    [Serializable]
    public class ComponentBinding<TComponent> : ComponentBinding
        where TComponent : View
    {
        [SerializeField] private UnityEngine.Object _target = default;
        [SerializeField] private string _methodName = default;

        public new TComponent view => (TComponent)_view;
    }
}
