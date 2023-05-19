using System.Collections.Generic;
using JetBrains.Annotations;

namespace Misc.Extensions
{
    public static class ArrayExtensions
    {

        /// <summary> Берёт первый элемент из списка </summary>
        /// <param name="list">Массив из которого надо взять первый T</param>
        /// <typeparam name="T">Искомый тип типа class</typeparam>
        /// <returns>Если есть искомый тип он его вернёт, иначе вернёт null</returns>
        [CanBeNull] 
        public static T First<T>(this IList<T> list) 
                where T : class =>
                list?.Count > 0 ?
                    list[0] :
                    default(T);
        
        [CanBeNull] 
        public static T First<T>(this IList<object> list) =>
            list?.Count > 0 ?
                (T)list[0] :
                default(T);
        
        [CanBeNull] 
        public static T Last<T>(this IList<T> list) where T : class =>
            list?.Count > 0 ?
                list[^1] :
                null;

        public static IList<T> FindFirstOne<T>(this IList<object> list)
        {
            if (list == null) 
                return null;
            
            if (list.Count <= 0) 
                return null;
            
            IList<T> genericList = new List<T>();

            foreach (object obj in list)
            {
                if (obj.GetType() != typeof(T)) 
                    continue;
                
                genericList.Add((T)obj);
            }
            
            return genericList;
        }

        public static bool IsEmpty(this IList<object> list)
        {
            if (list == null) 
                return true;
            
            return list.Count <= 0;
        }
    }
}