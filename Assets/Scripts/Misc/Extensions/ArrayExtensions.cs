using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T First<T>(this T[] list) 
            where T : class =>
            list?.Length > 0 ?
                list[Index.Start] :
                default(T);
        
        [CanBeNull] 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T First<T>(this List<object> list) =>
            list?.Count > 0 ?
                (T)list[Index.Start] :
                default(T);
        
        [CanBeNull] 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this T[] list) 
            where T : class =>
            list?.Length > 0 ?
                list[Index.FromEnd(1)] :
                null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FindObjects<T>(this T[] list)
            where T : class
        {
            if (list == null) 
                return Array.Empty<T>();

            if (list.Length == 0)
                return Array.Empty<T>();
            
            List<T> genericList = new List<T>(list.Length);
            Type genericType = typeof(T);
            
            foreach (object obj in list!)
            {
                if (obj.GetType() != genericType) 
                    continue;
                
                genericList.Add((T)obj);
            }
            
            if(genericList.Count < list.Length)
                genericList.TrimExcess();//TODO надо ли это делать
            
            return genericList.ToArray();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this object[] array) =>
            array?.Length == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotEmpty(this object[] array) => 
            array?.Length != 0;
    }
}