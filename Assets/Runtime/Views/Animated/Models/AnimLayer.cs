using System;
using UnityEngine;

namespace UIKit.Animated.Models
{
    [Serializable]
    public class AnimLayer
    {
        [SerializeField] private int _index = default;
        [SerializeField] private string _name = default;

        public string name => _name;

        public AnimLayer(int index, string name)
        {
            _index = index;
            _name = name;
        }

        public static implicit operator int(AnimLayer data)
        {
            return data._index;
        }
    }

}