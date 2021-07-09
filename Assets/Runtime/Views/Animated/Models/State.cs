using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIKit.Animated.Models
{
    [Serializable]
    internal class State : IEquatable<AnimatorStateInfo>
    {
        private static SortedList<int, State> _availableStates = default;
        
        [SerializeField] private bool _isInitialized = default;

        [SerializeField] private string _fullPath = default;
        [SerializeField] private int _fullPathHash = default;
        [SerializeField] private string _name = default;
        [SerializeField] private int _shortNameHash = default;
        [SerializeField] private string _tag = default;
        [SerializeField] private int _tagHash = default;
        [SerializeField] private Layer _layer = default;

        public string fullPath => _fullPath;
        public int fullPathHash => _fullPathHash;
        public string name => _name;
        public int shortNameHash => _shortNameHash;
        public string tag => _tag;
        public int tagHash => _tagHash;
        public Layer layer => _layer;
        
        [field: NonSerialized] public State[] children { get; private set; } = new State[0];

        public static SortedList<int, State> availableStates
        {
            get
            {
                if (_availableStates != null) return _availableStates;
                _availableStates = new SortedList<int, State>();
                return _availableStates;
            }
        }

        public State(string name, Layer layer, string tag = "")
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

        public State(string name, State parent, string tag = "")
        {
            _fullPath = $"{parent.fullPath}.{name}";
            _name = name;
            _tag = tag;

            _fullPathHash = Animator.StringToHash(fullPath);
            _shortNameHash = Animator.StringToHash(this.name);
            _tagHash = Animator.StringToHash(this.tag);

            _layer = parent.layer;

            parent.children = new List<State>(parent.children)
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

        public static void Add(State state)
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

        public static bool operator ==(State lhs, State rhs)
        {
            if (Equals(lhs, null))
            {
                return true;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(State lhs, State rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator int(State data)
        {
            return data.fullPathHash;
        }
    }
}
