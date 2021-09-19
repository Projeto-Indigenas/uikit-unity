using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public class DropdownView : View, IComponentAction, IComponentActionBinder
    {
        [SerializeField] private DropdownOption[] _startOptions = default;

        private ADropdown _dropdown = default;
        private Action<DropdownOption> _valueDidChange = default;

        [ComponentActionBinder]
        public event Action<DropdownOption> valueDidChange
        {
            add => _dropdown.valueDidChange += value;
            remove => _dropdown.valueDidChange -= value;
        }

        public void SetOptions(DropdownOption[] options) => _dropdown.SetOptions(options);

        protected override void Awake()
        {
            base.Awake();

            TMP_Dropdown tmpDropdown = GetComponent<TMP_Dropdown>();
            if (tmpDropdown)
            {
                _dropdown = new DropdownTMP(tmpDropdown);
                _dropdown.SetOptions(_startOptions);

                BindAction();

                return;
            }

            Dropdown dropdown = GetComponent<Dropdown>();
            if (dropdown)
            {
                _dropdown = new DropdownUI(dropdown);
                _dropdown.SetOptions(_startOptions);

                BindAction();

                return;
            }

            throw new MissingComponentException($"Missing TMP_Dropdown or UI.Dropdown component.");
        }

        private void OnDestroy()
        {
            if (_dropdown == null) return;

            _dropdown.Clear();
            _dropdown.valueDidChange -= _valueDidChange;
        }

        #region IComponentActionBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (!eventInfo.Name.Equals(nameof(valueDidChange))) return;

            _valueDidChange = (Action<DropdownOption>)info.CreateDelegate(typeof(Action<DropdownOption>), target);

            BindAction();
        }

        #endregion
        
        private void BindAction()
        {
            if (_dropdown == null) return;

            _dropdown.valueDidChange -= _valueDidChange;
            _dropdown.valueDidChange += _valueDidChange;
        }
    }
}