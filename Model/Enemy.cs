using ConsoleGameFramework.Skills;
using System;

namespace ConsoleGameFramework.Models;

public class Enemy : Character
{
	public Enemy(string id, string name, int maxHp, List<Skill> SkillList) : base(id, name, maxHp, SkillList)
	{
	}
}
