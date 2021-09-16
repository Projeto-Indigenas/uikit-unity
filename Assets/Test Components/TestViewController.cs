using UIKit;
using UIKit.Components;
using UnityEngine;

namespace UIKitTests
{
    class TestViewController : ViewController
    {
        [SerializeField] private ComponentBinding<ButtonView> _navigationButtonBinding;
    }
}
