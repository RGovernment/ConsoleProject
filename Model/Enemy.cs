using ConsoleGameFramework.Skills;
using System;

namespace ConsoleGameFramework.Models;

public class Enemy : Character
{
	public Enemy(string name, int maxHp, List<Skill> SkillList) : base(name, maxHp, SkillList)
	{
	}
}
