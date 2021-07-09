using System;
using UnityEngine;

namespace UIKit.Animated.Models
{
    [Serializable]
    internal class Layer
    {
        [SerializeField] private int _index = default;
        [SerializeField] private string _name = default;

        public string name => _name;

        public Layer(int index, string name)
        {
            _index = index;
            _name = name;
        }

        public static implicit operator int(Layer data)
        {
            return data._index;
        }
    }

}
