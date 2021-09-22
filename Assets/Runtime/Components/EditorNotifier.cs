#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace UIKit.Components
{
    public class EditorNotifier
    {
        public static readonly EditorNotifier instance = new EditorNotifier();

        private readonly List<KeyValuePair<View, ComponentBinding>> _buffer = new List<KeyValuePair<View, ComponentBinding>>();

        private Action<View, ComponentBinding> _addAction = default;
        private Action<View, ComponentBinding> _removeAction = default;

        public void Register(
            Action<View, ComponentBinding> addAction,
            Action<View, ComponentBinding> removeAction)
        {
            _addAction = addAction;
            _removeAction = removeAction;

            if (_addAction != null)
            {
                for (int index = 0; index < _buffer.Count; index++)
                {
                    KeyValuePair<View, ComponentBinding> pair = _buffer[index];
                    NotifyAdding(pair.Key, pair.Value);
                }

                _buffer.Clear();
            }
        }

        public void NotifyAdding(View view, ComponentBinding binding)
        {
            if (_addAction != null)
            {
                _addAction.Invoke(view, binding);

                return;
            }

            _buffer.Add(new KeyValuePair<View, ComponentBinding>(view, binding));
        }

        public void NotifyRemoving(View view, ComponentBinding binding)
        {
            _removeAction?.Invoke(view, binding);
        }
    }
}
#endif
