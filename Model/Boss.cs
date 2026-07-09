using ConsoleGameFramework.Data;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Model;

public class Boss : Character
{
    private string[] BossPassive { get; set; }

    public Boss(string id, string name, int maxHp, BossData data,List<Skill> SkillList) 
        : base(id, name, maxHp, SkillList)
    {

    }
}
