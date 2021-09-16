using UnityEngine;

namespace UIKit.Editor.Extensions
{
    public static class ColorExt
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
