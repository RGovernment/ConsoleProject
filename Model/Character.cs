using ConsoleGameFramework.Skills;
using Newtonsoft.Json.Linq;
using System;

namespace ConsoleGameFramework.Models;


public class Character
{
    public string Name { get; private set; }
    public int MaxHp { get; private set; }
    public int Hp { get; private set; }
    public int Sanity { get; private set; }
    public List<Skill> SkillList{ get; }
    public List<string> ItemKeyList { get; private set; }
    public List<Buff> BuffList { get; private set; }
    public List<Debuff> DebuffList { get; private set; }
    public bool IsAlive => Hp > 0;
    public Character(string name, int maxHp, List<Skill> list, int sanity = 0)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        SkillList = list;
        Sanity = sanity;
        ItemKeyList = new();
        BuffList = new();
        DebuffList = new();
    }

    public void TakeDamage(int damage)
    {
        Hp = Math.Max(0, Hp - damage);
    }
}
