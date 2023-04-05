using System.Collections.Generic;

public static class ArrayExtensions
{
    public static T First<T>(this IList<T> list) =>
        list.Count > 0 ? list[0] : default;
}