using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UIKit.Components.Attributes;
using UIKit.Components.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public class DropdownView : View, IComponentAction, IComponentActionBinder
    {
        private readonly ComponentActionEvent<Action<DropdownOption>> _valueDidChangeEvent = new ComponentActionEvent<Action<DropdownOption>>();

        [SerializeField] private DropdownOption[] _startOptions = default;

        private DropdownOption[] _options = default;
        private ADropdown _dropdown = default;

        [ComponentActionBinder]
        public event Action<DropdownOption> valueDidChange;

        public void SetOptions(DropdownOption[] options)
        {
            _options = options;
            _dropdown.SetOptions(options);
        }

        protected override void Awake()
        {
            base.Awake();

            TMP_Dropdown tmpDropdown = GetComponent<TMP_Dropdown>();
            if (tmpDropdown)
            {
                _dropdown = new DropdownTMP(tmpDropdown, this);
                SetOptions(_startOptions);

                return;
            }

            Dropdown dropdown = GetComponent<Dropdown>();
            if (dropdown)
            {
                _dropdown = new DropdownUI(dropdown, this);
                SetOptions(_startOptions);

                return;
            }

            throw new MissingComponentException($"Missing TMP_Dropdown or UI.Dropdown component.");
        }

        private void OnDestroy()
        {
            UnbindAll();

            if (_dropdown == null) return;

            _dropdown.Clear();
        }

        #region IComponentActionBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (eventInfo == null || !eventInfo.Name.Equals(nameof(valueDidChange))) return;

            Action<DropdownOption> action = (Action<DropdownOption>)info.CreateDelegate(typeof(Action<DropdownOption>), target);
            if (!_valueDidChangeEvent.AddEvent(action)) return;
            valueDidChange += action;
        }

        void IComponentActionBinder.UnbindActions()
        {
            UnbindAll();
        }

        #endregion

        internal void ValueDidChangeAction(int index) => valueDidChange?.Invoke(_options[index]);

        private void UnbindAll()
        {
            _valueDidChangeEvent.UnbindAll(each => valueDidChange -= each);
        }
    }
}