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

    public void StageClear()
    {
        int heal = (int)(MaxHp / 5f);
        TakeHeal(heal);
        GameManager.Instance.Context.AddLog($"스테이지 클리어로 체력이 {heal} 회복되었습니다.");
    }
}
