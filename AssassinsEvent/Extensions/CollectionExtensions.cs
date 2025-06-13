using System;
using System.Collections.Generic;
using System.Linq;

namespace AssassinsEvent.Extensions;

public static class CollectionExtensions
{
    public static T? RandomValue<T>(this T[] array) =>
        array.Length == 0 ? default : array[UnityEngine.Random.Range(0, array.Length)];

    public static T? RandomValue<T>(this IEnumerable<T> collection) => 
        collection.ToArray().RandomValue();

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable) 
            action(item);
    }
}
