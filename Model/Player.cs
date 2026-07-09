using ConsoleGameFramework.Core;
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

    public void RoundClear()
    {
        int heal = (int)(MaxHp / 5f);
        TakeHeal(heal);
        GameManager.Instance.Context.AddLog($"라운드 클리어!");
        GameManager.Instance.Context.AddLog($"라운드 클리어로 체력이 {heal} 회복되었습니다.");
    }
}
