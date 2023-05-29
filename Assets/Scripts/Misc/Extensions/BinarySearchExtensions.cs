#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.Utilities;

namespace Misc.Extensions
{
    public static class BinarySearchExtensions
    {
        private const int NOT_FOUND = -1;

        /// <summary> Выполняет поиск значения в отсортированном массиве SortedList&lt;TKey, TValueT&gt;, используя алгоритм двоичного поиска.</summary>
        /// <param name="searchList">SortedList&lt;TKey, TValue&gt; в котором надо найти значение TKey requiredKey.</param>
        /// <param name="requiredKey">Ключ который надо найти в списке.</param>
        /// <param name="foundedValue">Выходное значение искомого ключа.</param>
        /// <typeparam name="TKey">Тип ключа списка, наследуемый от IComparable&lt;TKey&gt;.</typeparam>
        /// <typeparam name="TValue">Тип значения списка.</typeparam>
        /// <seealso cref="IComparable &lt;T&gt;"/>
        /// <returns>Возвращает true eсли TKey найден и возвращает TValue foundedValue со значением от TKey,
        /// иначе вернёт false и TValue вернётся как default(TValue).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryBinarySearch<TKey, TValue> (this SortedList<TKey, TValue> searchList, 
                                                          TKey requiredKey, 
                                                          out TValue? foundedValue) 
                                                          where TKey : IComparable<TKey>
        {
            int index = searchList.Keys.BinarySearch(requiredKey, true);

            foundedValue = index != NOT_FOUND ?
                searchList.Values[index] : 
                default(TValue);
        
            return index != NOT_FOUND;
        }

        /// <summary> Выполняет поиск индекса значения в массиве IList&lt;T&gt;, используя алгоритм двоичного поиска.</summary>
        /// <remarks>Для корректной работы IList&lt;T&gt; нужно отсортировать через Sort().</remarks>
        /// <param name="searchList">IList&lt;T&gt; в котором ищется searchValue.</param>
        /// <param name="searchValue">Искомое значение от <typeparamref name="TComparable"/>.</param>
        /// <param name="isSorted">Отсортирован ли передаваемый массив?</param>
        /// <typeparam name="TComparable">Тип T, который должен наследоваться от IComparable&lt;T&gt;.</typeparam>
        /// <seealso cref="IComparable &lt;T&gt;"/>
        /// <returns>Возвращает int индекс позиции искомого заничения в searchList. </returns>
        /// <returns>Возвращает int -1 если не нашёл объект в IList&lt;T&gt;.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<TComparable>(this IList<TComparable> searchList, 
                                                    TComparable searchValue,
                                                    bool isSorted)
                                                    where TComparable : IComparable<TComparable>
        {
            if (!isSorted)
                searchList.Sort();

            return searchList.BinarySearch(searchValue, 0, searchList.Count - 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearch<T>(this IList<T> searchList, T searchValue, int lowIndex, int highIndex)
            where T : IComparable<T>
        {
            if (highIndex < lowIndex) return NOT_FOUND;
            if (highIndex == lowIndex) return highIndex;

            int middleIndex = (lowIndex + highIndex) >> 1; 

            return searchValue.CompareTo(searchList[middleIndex]) switch 
            {
                > 0 => BinarySearch(searchList, searchValue, middleIndex + 1, highIndex),
                < 0 => BinarySearch(searchList, searchValue, lowIndex, middleIndex - 1),
                0 => middleIndex
            };
        }
    }
}
