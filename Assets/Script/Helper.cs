using System;
using System.Collections.Generic;

public class Helper
{
    /// <summary>
    /// 去除list的重复元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="repeatList"></param>
    public static void ListSetly<T>(List<T> repeatList)
    {
        HashSet<T> seen = new HashSet<T>();
        List<T> uniqueList = new List<T>();

        foreach (T item in repeatList)
        {
            if (seen.Add(item))
            {
                uniqueList.Add(item);
            }
        }
        repeatList.Clear();
        repeatList.AddRange(uniqueList);
    }

    /// <summary>
    /// 移动list中的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void MoveListElement<T>(List<T> list, int oldIndex, int newIndex)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (oldIndex < 0 || oldIndex >= list.Count || newIndex < 0 || newIndex >= list.Count)
            throw new ArgumentOutOfRangeException("下标超出范围");
        if (oldIndex == newIndex)
            return;
        T element = list[oldIndex];
        list.RemoveAt(oldIndex);
        if (oldIndex < newIndex)
        {
            newIndex--;
        }
        list.Insert(newIndex, element);
    }

    /// <summary>
    /// 根据下标移除list中的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void ReomoveListElementByIndex<T>(List<T> list, int index)
    {
        List<T> uniqueList = new List<T>();
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        if (index < 0 || index >= list.Count)
            throw new ArgumentOutOfRangeException("下标超出范围");
        for (int i = 0; i < list.Count; i++)
        {
            if (i != index)
            {
                uniqueList.Add(list[i]);
            }
        }
        list.Clear();
        list.AddRange(uniqueList);
    }

    /// <summary>
    /// 根据元素移除list的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="element"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ReomoveListElementByElement<T>(List<T> list, T element)
    {
        List<T> uniqueList = new List<T>();
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        for (int i = 0; i < list.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(list[i], element))
            {
                uniqueList.Add(list[i]);
            }
        }
        list.Clear();
        list.AddRange(uniqueList);
    }
}