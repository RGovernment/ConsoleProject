using ConsoleGameFramework.Skills;
using Newtonsoft.Json.Linq;
using System;

namespace ConsoleGameFramework.Models;

public class Player : Character
{
    public Player(Player player): base(player)
    {

    }

    public Player(string id, string name, int maxHp, List<Skill> skillList) : 
		base(id, name, maxHp, skillList)
	{

	}
}
