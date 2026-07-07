using ConsoleGameFramework.Data;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Skills;
using ConsoleGameFramework.UI;
using System;
using System.Text;
using System.Xml.Linq;
using static ConsoleGameFramework.Common.Constants;
namespace ConsoleGameFramework.Core;


public class BattleManager
{
	// 플레이어
	public Player Player { get; private set; }
	// 적
	public List<Enemy> Enemy { get; private set; }

	// 플레이어와 적을 생성하고, 초기화하는 함수.
	public void StartBattleInit(int round, Dictionary<string, object> roundData)
	{
		Player = new(PlayerManager.Instance.playerStatus);

		foreach (var data in roundData)
		{
			Dictionary<string, object> enemyData = (Dictionary<string, object>) data.Value;

            string name = 
				enemyData.TryGetValue(NAME, out object? nameVal) &&
                nameVal is string namedata ? namedata : "기본수";
			int maxHp = enemyData.TryGetValue(HP, out object? maxHpVal) &&
                maxHpVal is int hpData ? hpData : 80;
			
			Enemy.Add(new(name, maxHp,
				[
                    new Skill(SkillData.EnemySkills[0], 0),
                    new Skill(SkillData.EnemySkills[1], 0),
                    new Skill(SkillData.EnemySkills[2], 0)
                ] 
				));
		}
	}


	public enum BattleOutcome
	{
		Continuing,
		Victory,
		Defeat
	}
	// 플레이어가 적을 공격하는 함수
	public BattleOutcome PlayerAttack()
	{
		if (Enemy.Count > 0 && !Enemy.Any(x => x.IsAlive)) return BattleOutcome.Victory;

		if (Player.IsAlive)
			return BattleOutcome.Continuing;
		else
			return BattleOutcome.Defeat;
	}

	/// <summary>
	/// 두 스킬의 대결 여부 반환
	/// </summary>
	/// <param name="first">대결할 스킬 1</param>
	/// <param name="second">대결할 스킬 2</param>
	/// <param name="firstSan">대결할 스킬1 보유자의 정신력</param>
	/// <param name="secondSan">대결할 스킬2 보유자의 정신력</param>
	/// <returns></returns>
	public (string winner, Skill skill) SkillClash(Skill first,Skill second, int firstSan, int secondSan)
	{
		StringBuilder sb = new();
        StringBuilder sb2 = new();
        char reverseCoin = '○';
        char forwardCoin = '●';
        char leftCoin = '◐';
        char rightCoin = '◑';
        int firstAtk = first.AttackPoint;
		int secondAtk = second.AttackPoint;
		// 이름 배정
		sb.Append($"{first.Name} : ");
        sb2.Append($"{second.Name} : ");

		//코인 배치
        for (int i = 0; i < first.Coin; i++) sb.Append(leftCoin);
        for (int i = 0; i < second.Coin; i++) sb2.Append(leftCoin);

		ConsoleUI.WriteBox(
			[
			sb.ToString(),
			"    vs    ",
			sb2.ToString()
			],
			"합",ConsoleColor.DarkYellow
			);

		// 출력된 6줄 삭제(코인 던지기 반복 어색하지 않게)
        int currentLineCursor = Console.CursorTop - 6;
        Console.SetCursorPosition(0, currentLineCursor);

        Task.Delay(Random.Shared.Next(100, 150));


        return ("", first);
	}

}
