using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public class DropdownView : View, IComponentAction<DropdownOption>, IComponentActionSetup
    {
        [SerializeField] private DropdownOption[] _startOptions = default;

        private ADropdown _dropdown = default;

        public event Action<DropdownOption> onValueChanged
        {
            add => _dropdown.onValueChanged += value;
            remove => _dropdown.onValueChanged -= value;
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

                return;
            }

            Dropdown dropdown = GetComponent<Dropdown>();
            if (dropdown)
            {
                _dropdown = new DropdownUI(dropdown);

                _dropdown.SetOptions(_startOptions);

                return;
            }

            throw new MissingComponentException($"Missing TMP_Dropdown or UI.Dropdown component.");
        }

        private void OnDestroy() => _dropdown.Clear();

        #region IComponentAction<ADropdownOption>

        event Action<DropdownOption> IComponentAction<DropdownOption>.action
        {
            add => onValueChanged += value;
            remove => onValueChanged -= value;
        }

        #endregion

        #region IComponentActionSetup

        void IComponentActionSetup.SetupAction(UnityEngine.Object target, MethodInfo info)
        {
            Action<DropdownOption> action = (Action<DropdownOption>)info.CreateDelegate(typeof(Action<DropdownOption>), target);
            onValueChanged += action;
        }

        #endregion
    }
}