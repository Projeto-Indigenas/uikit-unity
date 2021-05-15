using System;
using UnityEngine;

namespace UIKit.Animated.Models
{
    public class AnimTransition : IEquatable<AnimatorTransitionInfo>
    {
        private readonly int fullPathHash;
        private readonly int userNameHash;

        public AnimTransition(AnimState fromState, AnimState toState)
        {
            fullPathHash = Animator.StringToHash($"{fromState.fullPath} -> {toState.fullPath}");
            userNameHash = 0;
        }

        public AnimTransition(string userName)
        {
            userNameHash = Animator.StringToHash(userName);
            fullPathHash = 0;
        }

        public bool Is(AnimatorTransitionInfo info)
        {
            return CheckValue(fullPathHash, info.fullPathHash) ||
                CheckValue(userNameHash, info.userNameHash);
        }

        bool IEquatable<AnimatorTransitionInfo>.Equals(AnimatorTransitionInfo info)
        {
            return Is(info);
        }

        private bool CheckValue(int expected, int value)
        {
            return value != 0 && value == expected;
        }
    }
}
