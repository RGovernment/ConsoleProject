using ConsoleGameFramework.Skills;
using Newtonsoft.Json.Linq;
using System;

namespace ConsoleGameFramework.Models;

public class Player : Character
{
    public Player(Player player): base(player)
    {

    }

    public Player(string name, int maxHp, List<Skill> skillList) : 
		base(name, maxHp, skillList)
	{

	}
}
