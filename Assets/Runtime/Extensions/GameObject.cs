using System.IO;
using UnityEngine;

namespace UIKit.Extensions
{
    internal static class GameObjectExt
    {
        internal static string GetFullPath(this GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = Path.Combine(transform.name, path);
            }
            return path;
        }
    }
}