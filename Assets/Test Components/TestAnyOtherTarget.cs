using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEngine;

namespace UIKitTests
{
    public class TestAnyOtherTarget : MonoBehaviour
    {
        [SerializeField] private ComponentBinding _anyView = default;

        [ComponentAction]
        private void PossibleAction()
        {

        }

        [ComponentAction]
        private void PossibleAction(string newText)
        {

        }
    }
}
