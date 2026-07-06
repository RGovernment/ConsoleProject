using System;
using ConsoleGameFramework.Models;

namespace ConsoleGameFramework.Core;

public class BattleManager
{
	private static BattleManager instance = null;

	public static BattleManager Instance
	{
		get
		{
			if(instance == null)
				instance = new BattleManager();

			return instance;
		}
	}
	// 플레이어
	public Player Player { get; private set; }

	// 적
	public Enemy Enemy { get; private set; }

	// 플레이어와 적을 생성하고, 초기화하는 함수.
	public void StartBattleInit(string name)
	{
		//Player = new Player(name,100,10);
		//Enemy = new Enemy("고블린",40,5);
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
		//Enemy.TakeDamage(Player.Attack);

		if(!Enemy.IsAlive)
		{
			return BattleOutcome.Victory;
		}
		// 적이 플레이어를 때리는 함수
		//Player.TakeDamage(Enemy.Attack);

		if (Player.IsAlive)
			return BattleOutcome.Continuing;
		else
			return BattleOutcome.Defeat;
	}

}
