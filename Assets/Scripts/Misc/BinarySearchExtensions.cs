using System;
using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public static class BinarySearchExtensions
    {
        private const int NOT_FOUND = -1;
        
        /// <summary>
        /// Выполняет поиск значения в отсортированном массиве SortedList, используя для этого алгоритм двоичного поиска.
        /// </summary>
        /// <param name="searchList">SortedList в котором надо найти занчение.</param>
        /// <param name="requiredKey">Искомое знчение.</param>
        /// <typeparam name="TKey">Ключ листа SortedList.</typeparam>
        /// <typeparam name="TValue">Занчение листа SortedList.</typeparam>
        /// <returns>Возвращает Index позиции искомого заничения в искомом массиве и TKey foundedKey.
        /// Возвращает int -1 если не нашёл объект в searchList.</returns>
        public static (int Index, TKey FoundedKey) BinarySearch<TKey, TValue>(
            this SortedList<TKey, TValue> searchList, 
            TKey requiredKey) 
            where TKey : IComparable<TKey>
        {
            int index = BinarySearch(searchList.Keys, requiredKey, 0, searchList.Keys.Count-1);
            return index != NOT_FOUND ? 
                (index, searchList.Keys[index]) : 
                (NOT_FOUND, default);
        }
        
        /// <summary>
        /// Выполняет поиск значения в массиве IList<T>, используя для этого алгоритм двоичного поиска.
        /// Для корректной работы IList нужно отсортировать через Sort()
        /// </summary>
        /// <param name="searchList">IList<T> в котором надо найти занчение.</param>
        /// <param name="searchValue">Искомое знчение.</param>
        /// <param name="lowIndex">Начальный элемент искомого списка</param>
        /// <param name="highIndex">Последний элемент искомого списка</param>
        /// <typeparam name="T">where T : IComparable<T>.</typeparam>
        /// <returns>Возвращает int позиции искомого заничения в искомом массиве.
        /// Возвращает int -1 если не нашёл объект в searchList.</returns>
        public static int BinarySearch<T>(this IList<T> searchList, T searchValue, int lowIndex, int highIndex)
            where T : IComparable<T>
        {
            if (highIndex < lowIndex) return NOT_FOUND;
            if (highIndex == lowIndex) return highIndex;

            int middle = (int)((uint)(lowIndex + highIndex) >> 1);

            return searchValue.CompareTo(searchList[middle]) switch
            {
                > 0 => BinarySearch(searchList, searchValue, middle + 1, highIndex),
                < 0 => BinarySearch(searchList, searchValue, lowIndex, middle - 1),
                0 => middle
            };
        }
    }
}