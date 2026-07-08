using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ConsoleGameFramework.Core;

namespace ConsoleGameFramework.Common;
public static class UtilityExtension
{
    /// <summary>
    /// Fisher-Yates Shuffle, List용
    /// </summary>
    /// <typeparam name="T">모든 변수 타입</typeparam>
    /// <param name="values"></param>
    public static void Shuffle<T>(this IList<T> values)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(0, i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }
    }

    public static void MatrixFill<T>(this T[,] data, T fill)
    {
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
                data[i, j] = fill;
        }
    }

    public static bool NodeDFS<T>(this TreeNode<T> tree, T data)
    {
        if (tree == null) return false;
        Console.WriteLine(tree.Data);
        bool isTrue = false;
        if (EqualityComparer<T>.Default.Equals(tree.Data, data))
            return true;
        if (tree.Links != null)
        {
            for (int i = 0; i < tree.Links.Count; i++)
            {
                if (NodeDFS(tree.Links[i], data))
                    isTrue = true;
            }
        }


        return isTrue;
    }

    public static string StrLengExtend(this StringBuilder sb, int range, string data, bool isLeft = false)
    {
        int byteCount = GameManager.Instance.Context.Euckr.GetByteCount(data);

        if (byteCount < range)
        {
            if (isLeft)
                sb.Insert(0, ' ');
            else
                sb.Append(' ');
            isLeft = !isLeft;
            return StrLengExtend(sb, range, sb.ToString(), isLeft);
        }
        else if (byteCount > range)
        {
            for (int i = range; i >= range / 2; i--)
            {
                if (data.Length < i) continue;

                string ck = data[..i];
                if (GameManager.Instance.Context.Euckr.GetByteCount(ck) <= range)
                {
                    sb.Clear();
                    sb.Append(ck);
                    return StrLengExtend(sb, range, ck, isLeft);
                }
            }
            // 이거 일어나면 안됨 인코딩상 2바이트 초과하는 글자가 없어서 불가능함;
            return "버그남;;";
        }
        else
        {
            return sb.ToString();
        }

    }
}


