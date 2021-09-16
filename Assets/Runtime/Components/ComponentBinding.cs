using System;
using UnityEngine;

namespace UIKit.Components
{
    [Serializable]
    public class ComponentBinding
    {
        [SerializeField] protected AView _view;

        public AView view
        {
            get => _view;
            private set => _view = value;
        }
    }

    [Serializable]
    public class ComponentBinding<TComponent> : ComponentBinding
        where TComponent : AView
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
        [SerializeField] private UnityEngine.Object _target = default;
        [SerializeField] private string _methodName = default;
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0044 // Add readonly modifier

        public new TComponent view => (TComponent)_view;
    }
}
