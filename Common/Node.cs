using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Common;

public struct Node
{
    public int Row;
    public int Col;

    public Node()
    {
        Row = 0;
        Col = 0;
    }

    public Node(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public override string ToString()
    {
        return $"Row : {Row} | Col : {Col}";

    }
}
