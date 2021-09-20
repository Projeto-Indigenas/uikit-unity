using System;
using UnityEngine;

namespace UIKit
{
    internal abstract class AInputField
    {
        public abstract string text { get; set; }

        public abstract void Clear();
    }
}
