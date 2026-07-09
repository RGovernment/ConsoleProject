using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public class Debuff : EffectStatus
{
    public Debuff(string name, string id, int coeffi, string description, int duration) : 
        base(name, id, coeffi, description, duration)
    {
    }
}
