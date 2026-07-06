using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Items;

public class Item
{
    public string Id {  get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Effect {  get; set; }
}
