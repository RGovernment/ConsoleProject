using ConsoleGameFramework.Common;
using ConsoleGameFramework.Skills;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ConsoleGameFramework.Models;


public class Character
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int MaxHp { get; private set; }
    public int Hp { get; private set; }
    public int Sanity { get; private set; }
    public List<Skill> SkillList { get; }
    public List<string> ItemKeyList { get; private set; }
    public List<Buff> BuffList { get; private set; }
    public List<Debuff> DebuffList { get; private set; }

    public Queue<Skill> SkillQueue { get; private set; }
    private Queue<Skill> NextQueue;
    public bool IsAlive => Hp > 0;
    public Character(Character character)
    {
        Name = character.Name;
        MaxHp = character.MaxHp;
        Hp = character.Hp;
        SkillList = new(character.SkillList);
        Sanity = character.Sanity;
        ItemKeyList = new(character.ItemKeyList);
        BuffList = new();
        DebuffList = new();
        SkillQueue = new();
        NextQueue = new();
    }
    public Character(string id, string name, int maxHp, List<Skill> list, int sanity = 0)
    {
        Id = id;
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        SkillList = list;
        Sanity = sanity;
        ItemKeyList = new();
        BuffList = new();
        DebuffList = new();
        SkillQueue = new();
        NextQueue = new();
    }

    public void TakeDamage(int damage)
    {
        Hp = Math.Max(0, Hp - damage);
    }

    /// <summary>
    /// skill을 섞고 재배치한 뒤 큐(뽑는 큐, 대기 큐 2개)에 넣고 순서대로 스킬 출력
    /// </summary>
    /// <returns></returns>
    public Skill GetSkillQueue()
    {
        if (SkillList.Count < 3) return null;

        if (SkillQueue.Count == 0)
        {
            List<Skill> skill = new()
            {
                SkillList[0],SkillList[0],SkillList[0],SkillList[1],SkillList[1],SkillList[2]
            };

            skill.Shuffle();
            skill.ForEach(x => SkillQueue.Enqueue(x));

            skill.Shuffle();
            skill.ForEach(x => NextQueue.Enqueue(x));

            Skill result = SkillQueue.Dequeue();

            SkillQueue.Enqueue(NextQueue.Dequeue());

            return result;
        }
        else if(NextQueue.Count == 0)
        {
            List<Skill> skill = new()
            {
                SkillList[0],SkillList[0],SkillList[0],SkillList[1],SkillList[1],SkillList[2]
            };

            skill.Shuffle();
            skill.ForEach(x => NextQueue.Enqueue(x));

            Skill result = SkillQueue.Dequeue();

            SkillQueue.Enqueue(NextQueue.Dequeue());

            return result;
        }
        else
        {
            Skill result = SkillQueue.Dequeue();

            SkillQueue.Enqueue(NextQueue.Dequeue());

            return result;
        }
    }

    public void ClearSkillQueue()
    {
        SkillQueue.Clear();
        NextQueue.Clear();
    }
}
