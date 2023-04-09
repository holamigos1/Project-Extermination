using System.Collections.Generic;

public static class ArrayExtensions
{
    public static T First<T>(this IList<T> list) =>
        list.Count > 0 ? list[0] : default;
    
    public static List<T> GetAs<T>(this List<object> list)
    {
        if (list == null) return null;
        if (list.Count <= 0) return null;
            
        List<T> genericList = new List<T>();

        foreach (var obj in list)
        {
            if (obj.GetType() != typeof(T)) continue;
            genericList.Add((T)obj);
        }
            
        return genericList;
    }
        
    public static T GetFirstAs<T>(this List<object> list)
    {
        if (list == null) return default;
        if (list.Count <= 0) return default;
        if (list[0].GetType() == typeof(T)) return (T)list[0];
        return default;
    }
        
    public static bool IsEmpty(this List<object> list)
    {
        if (list == null) return true;
        if (list.Count <= 0) return true;
        return false;
    }
}