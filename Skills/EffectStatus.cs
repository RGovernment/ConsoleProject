using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public abstract class EffectStatus
{
    //이름
    public string Name { get; set; }

    //아이디
    public string Id { get; set; }

    //설명
    public string Description { get; set; }

    // 위력
    public int Coefficient { get; set; }

    public int Duration { get; set; }

    public EffectStatus(string name, string id, int coefficient, string description, int duration = -1)
    {
        Name = name;
        Id = id;
        Coefficient = coefficient;
        Description = description;
        Duration = duration;
    }
}
