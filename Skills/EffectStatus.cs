using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public abstract class EffectStatus
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string Description { get; set; }
    public List<string> Effect { get; set; }

    public EffectStatus(string name, string id, string description, List<string> effect)
    {
        Name = name;
        Id = id;
        Description = description;
        Effect = effect;
    }
}
