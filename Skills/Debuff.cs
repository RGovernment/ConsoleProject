using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public class Debuff : EffectStatus
{
    public Debuff(string name, string id, string description, List<string> effect) : base(name, id, description, effect)
    {
    }
}
