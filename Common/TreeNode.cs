using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Common;

public class TreeNode<T>
{
    public T Data { get; set; }
    public List<TreeNode<T>> Links { get; private set; }

    public TreeNode(T data)
    {
        Data = data;
        Links = new();
    }
}
