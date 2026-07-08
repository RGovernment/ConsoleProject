using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public class Buff : EffectStatus
{
    public Buff(string name, string id,int coeffi,string desc,int duration) 
        : base(name, id, coeffi, desc, duration)
    {

    }
}
