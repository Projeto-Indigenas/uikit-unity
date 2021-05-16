using System;
using System.Collections.Generic;

namespace UIKit.Extensions
{
    internal static class ArrayExt
    {
        internal static TResult[] Map<TValue, TResult>(this TValue[] array, Func<TValue, TResult> mapper)
        {
            if (array == null) return null;
            TResult[] newArray = new TResult[array.Length];
            for (int index = 0; index < array.Length; index++) 
                newArray[index] = mapper.Invoke(array[index]);
            return newArray;
        }

        internal static List<TValue> AsList<TValue>(this IEnumerable<TValue> array) => new List<TValue>(array);
    }
}