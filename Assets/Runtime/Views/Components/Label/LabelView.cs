using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    
    public class LabelView : View
    {
        private ALabel _label = default;

        public string text
        {
            get => _label.text;
            set => _label.text = value;
        }

        protected override void Awake()
        {
            base.Awake();

            TMP_Text tmpLabel = GetComponent<TMP_Text>();
            if (tmpLabel)
            {
                _label = new LabelTMP(tmpLabel);

                return;
            }

            Text label = GetComponent<Text>();
            if (label)
            {
                _label = new LabelUI(label);

                return;
            }

            throw new MissingComponentException($"Missing TMP_Text or UI.Text component.");
        }
    }
}