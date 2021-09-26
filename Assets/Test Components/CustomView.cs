using UIKit;
using UIKit.Components;
using UnityEngine;

namespace UIKitTests
{
    public class CustomView : View
    {
        [SerializeField] private ComponentBinding _anyView = default;
    }
}
