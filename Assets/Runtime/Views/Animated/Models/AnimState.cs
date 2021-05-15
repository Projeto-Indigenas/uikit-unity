using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIKit.Animated.Models
{
    [Serializable]
    public class AnimState : IEquatable<AnimatorStateInfo>
    {
        private static SortedList<int, AnimState> _availableStates = default;
        
        [SerializeField] private bool _isInitialized = default;

        [SerializeField] private string _fullPath = default;
        [SerializeField] private int _fullPathHash = default;
        [SerializeField] private string _name = default;
        [SerializeField] private int _shortNameHash = default;
        [SerializeField] private string _tag = default;
        [SerializeField] private int _tagHash = default;
        [SerializeField] private AnimLayer _layer = default;

        public string fullPath => _fullPath;
        public int fullPathHash => _fullPathHash;
        public string name => _name;
        public int shortNameHash => _shortNameHash;
        public string tag => _tag;
        public int tagHash => _tagHash;
        public AnimLayer layer => _layer;
        
        [field: NonSerialized] public AnimState[] children { get; private set; } = new AnimState[0];

        public static SortedList<int, AnimState> availableStates
        {
            get
            {
                if (_availableStates != null) return _availableStates;
                _availableStates = new SortedList<int, AnimState>();
                return _availableStates;
            }
        }

        public AnimState(string name, AnimLayer layer, string tag = "")
        {
            _fullPath = $"{layer.name}.{name}";
            _name = name;
            _tag = tag;

            _fullPathHash = Animator.StringToHash(fullPath);
            _shortNameHash = Animator.StringToHash(this.name);
            _tagHash = Animator.StringToHash(this.tag);

            _layer = layer;

            _isInitialized = true;
        }

        public AnimState(string name, AnimState parent, string tag = "")
        {
            _fullPath = $"{parent.fullPath}.{name}";
            _name = name;
            _tag = tag;

            _fullPathHash = Animator.StringToHash(fullPath);
            _shortNameHash = Animator.StringToHash(this.name);
            _tagHash = Animator.StringToHash(this.tag);

            _layer = parent.layer;

            parent.children = new List<AnimState>(parent.children)
            {
                this
            }.ToArray();

            _isInitialized = true;
        }

        public bool Is(AnimatorStateInfo info)
        {
            return _isInitialized &&
                (CheckValue(fullPathHash, info.fullPathHash) ||
                CheckValue(shortNameHash, info.shortNameHash) ||
                CheckValue(tagHash, info.tagHash));
        }

        public bool Is(int id)
        {
            return _isInitialized &&
                (CheckValue(fullPathHash, id) ||
                CheckValue(shortNameHash, id) ||
                CheckValue(tagHash, id));
        }

        bool IEquatable<AnimatorStateInfo>.Equals(AnimatorStateInfo info)
        {
            return Is(info);
        }

        public static void Add(AnimState state)
        {
            availableStates.Add(state.fullPathHash, state);
        }

        public override bool Equals(object obj)
        {
            if (obj == null && !_isInitialized)
            {
                return true;
            }
            return Equals(this, obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            return $"[AnimState] Full path: {_fullPath}, hash: {_fullPathHash}";
        }

        private bool CheckValue(int expected, int value)
        {
            return value != 0 && value == expected;
        }

        public static bool operator ==(AnimState lhs, AnimState rhs)
        {
            if (Equals(lhs, null))
            {
                return true;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(AnimState lhs, AnimState rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator int(AnimState data)
        {
            return data.fullPathHash;
        }
    }
}
