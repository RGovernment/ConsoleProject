using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public class Buff : EffectStatus
{
    public Buff(string name, string id,string desc ,List<string> Effect) : base(name, id, desc, Effect)
    {

    }
}
