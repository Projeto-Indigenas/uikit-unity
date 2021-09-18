using UIKit.Components.Attributes;
using UnityEngine;

namespace UIKitTests
{
    public class TestAnyOtherTarget : MonoBehaviour
    {
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
