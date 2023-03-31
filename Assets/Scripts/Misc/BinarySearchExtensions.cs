#nullable enable
using System;
using System.Collections.Generic;

public static class BinarySearchExtensions
{
    private const int NOT_FOUND = -1;

    /// <summary> Выполняет поиск значения в отсортированном массиве SortedList&lt;TKey, TValueT&gt;, используя алгоритм двоичного поиска.</summary>
    /// <param name="searchList">SortedList&lt;TKey, TValueT&gt; в котором надо найти занчение TKey requiredKey.</param>
    /// <param name="requiredKey">Искомый ключ в списке.</param>
    /// <param name="foundedValue">Значение искомого ключа.</param>
    /// <typeparam name="TKey">Тип ключа списка SortedList&lt;TKey, TValueT&gt;, наследуемый от IComparable&lt;TKey&gt;.</typeparam>
    /// <typeparam name="TValue">Тип занчения списка SortedList&lt;TKey, TValueT&gt;.</typeparam>
    /// <returns>Возвращает true eсли TKey найден в SortedList&lt;TKey, TValueT&gt; и возврощает TValue foundedValue со занчением от TKey,
    /// иначе TValue вернётся как default(TValue).</returns>
    public static bool BinarySearch<TKey, TValue> (this SortedList<TKey, TValue> searchList, TKey requiredKey, out TValue? foundedValue) 
        where TKey : IComparable<TKey>
    {
        int index = searchList.Keys.BinarySearch(requiredKey);

        bool isFounded = index != NOT_FOUND;
        
        foundedValue = isFounded ?
            searchList.Values[index] : 
            default(TValue);
        
        return isFounded;
    }

    /// <summary> Выполняет поиск значения в массиве IList&lt;T&gt;, используя алгоритм двоичного поиска.</summary>
    /// <remarks>Для корректной работы IList&lt;T&gt; нужно отсортировать через Sort().</remarks>
    /// <param name="searchList">IList&lt;T&gt; в котором ищется searchValue.</param>
    /// <param name="searchValue">Искомое знчение от <typeparamref name="T"/>.</param>
    /// <typeparam name="T">Тип T, который должен наследоваться от IComparable&lt;T&gt;.</typeparam>
    /// <returns>Возвращает int индекс позиции искомого заничения в searchList.</returns>
    /// <returns>Возвращает int -1 если не нашёл объект в IList&lt;T&gt;.</returns>
    public static int BinarySearch<T>(this IList<T> searchList, T searchValue) where T : IComparable<T> =>
        searchList.BinarySearch(searchValue, 0, searchList.Count-1);

    
    private static int BinarySearch<T>(this IList<T> searchList, T searchValue, int lowIndex, int highIndex)
        where T : IComparable<T>
    {
        if (highIndex < lowIndex) return NOT_FOUND;
        if (highIndex == lowIndex) return highIndex;

        int middle = (lowIndex + highIndex) >> 1; //TODO Мне кажется этот код можно сделать ещё быстрее

        return searchValue.CompareTo(searchList[middle]) switch 
        {
            > 0 => BinarySearch(searchList, searchValue, middle + 1, highIndex),
            < 0 => BinarySearch(searchList, searchValue, lowIndex, middle - 1),
            0 => middle
        };
    }
}
