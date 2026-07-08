using ConsoleGameFramework.Common;
using ConsoleGameFramework.Data;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Skills;
using ConsoleGameFramework.UI;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using static ConsoleGameFramework.Common.Constants;
using static ConsoleGameFramework.Common.Utility;
namespace ConsoleGameFramework.Core;


public class BattleManager
{
    // 윈도우 콘솔 제어를 위한 Windows API 가져오기
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    const int STD_INPUT_HANDLE = -10;
    const uint ENABLE_ECHO_INPUT = 0x0004;    // 사용자가 입력한 키를 화면에 그리는 모드
    const uint ENABLE_LINE_INPUT = 0x0002;    // 엔터키를 누를 때까지 입력받는 모드

    // 플레이어
    public Player Player { get; private set; }
	// 적
	public List<Enemy> Enemy { get; private set; }

	// 플레이어와 적을 생성하고, 초기화하는 함수.
	public void StartBattleInit(int round, Dictionary<string, object> roundData)
	{
		Player = new(PlayerManager.Instance.playerStatus);
		Enemy = new();

        foreach (var data in roundData)
		{
			Dictionary<string, object> enemyData = (Dictionary<string, object>) data.Value;
			string id =
                enemyData.TryGetValue(ID, out object? idVal) &&
                idVal is string idData ? idData : "defaultId";
            string name = 
				enemyData.TryGetValue(NAME, out object? nameVal) &&
                nameVal is string nameData ? nameData : "기본수";
			int maxHp = enemyData.TryGetValue(HP, out object? maxHpVal) &&
                maxHpVal is int hpData ? hpData : 80;
			
			Enemy.Add(new(id, name, maxHp,
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
    /// <param name="first">대결할 스킬 1, 생성해 넣을 것</param>
    /// <param name="second">대결할 스킬 2, 생성해 넣을 것</param>
    /// <param name="firstSan">대결할 스킬1 보유자의 정신력</param>
    /// <param name="secondSan">대결할 스킬2 보유자의 정신력</param>
	/// <param name="firstId">대결할 스킬1 보유자의 ID</param>
	/// <param name="secondId">대결할 스킬2 보유자의 ID</param>
    /// <returns>계산 완료된 스킬</returns>
    public async Task<(string winner, Skill skill)> SkillClash(
		Skill first, Skill second, 
		Character firstChara, Character secondChara)
	{
		/// 콘솔 입력 방지ㅡ 나중에 테스트 해보고 안되면 제거
        /*IntPtr hInput = GetStdHandle(STD_INPUT_HANDLE);
        GetConsoleMode(hInput, out uint originalMode);
        SetConsoleMode(hInput, originalMode & ~(ENABLE_ECHO_INPUT | ENABLE_LINE_INPUT));*/
		///
        StringBuilder sb = new();
        StringBuilder sb2 = new();
        char leftCoin = '◐';
        char reverseCoin = '○';
        char rightCoin = '◑';
        char forwardCoin = '●';

		Dictionary<string, int> firstCoinAtkVal = CoinAndAtkPointCalc(first, secondChara);
        Dictionary<string, int> secondCoinAtkVal = CoinAndAtkPointCalc(second, firstChara);

        int firstAtk = first.AttackPoint + firstCoinAtkVal[ATK];
        int secondAtk = second.AttackPoint + secondCoinAtkVal[ATK];
        int firstCoinPoint = first.CoinPoint + firstCoinAtkVal[COIN_POINT];
        int secondCoinPoint = second.CoinPoint + secondCoinAtkVal[COIN_POINT];
        int firstCoinPointTotal = 0;
        int secondCoinPointTotal = 0;
        // 이름 배정
        int vsLeng = 0;
		//코인 배치
/*        for (int i = 0; i < first.Coin; i++) sb.Append(leftCoin);
        for (int i = 0; i < second.Coin; i++) sb2.Append(leftCoin);*/

		if (first.Name?.Length >= second.Name?.Length) vsLeng = first.Name.Length;
		else vsLeng = second.Name != null ? second.Name.Length : 2;

		StringBuilder vsLengSb = new();
		vsLengSb.StrLengExtend(Math.Max(2, vsLeng), "vs");

		//이후 구간, 계산 및 출력 반복

		ConsoleUI.WriteBox(
			[
			$"{first.Name} [ 공격력 : {first.AttackPoint} + ? ]",
			"",
            vsLengSb.ToString(),
            $"{second.Name} [ 공격력 : {second.AttackPoint} + ? ]",
			""
			],
			"합",ConsoleColor.DarkYellow
			);

        // 출력된 9줄 시작점으로 이동해 다시 콘솔 출력(코인 던지기 반복 어색하지 않게)
        int currentLineCursor = Console.CursorTop - 9;
        Console.SetCursorPosition(0, currentLineCursor);
        
        int count = 0;
		//루프
		while (true)
		{
            if (count >= first.Coin && count >= second.Coin) break;
            

            bool firstNowToss = CoinToss(firstChara.Sanity);
			bool secondNowToss= CoinToss(secondChara.Sanity);
			int coinFlip = 0;
			int turnCount = 0;
            int turnMax = 20;

            // 공격력 or 코인 위력에 추가 조건이 있는지 검사
			
            if (first.Coin > count) sb.Append(leftCoin);
            if (second.Coin > count) sb2.Append(leftCoin);

            while (turnCount < turnMax)
			{
				char nowCoin = coinFlip == 0 ? leftCoin    : 
							   coinFlip == 1 ? reverseCoin : 
							   coinFlip == 2 ? rightCoin   : 
							                   forwardCoin ;
				
                if (coinFlip < 3) coinFlip++;
				else coinFlip = 0;

                if (first.Coin > count && sb.Length > 0) sb[^1] = nowCoin;
                if (second.Coin > count && sb2.Length > 0) sb2[^1] = nowCoin;

                Console.SetCursorPosition(0, currentLineCursor);

                ConsoleUI.Clear();
                ConsoleUI.WriteBox(
				[
					$"{first.Name} [ 공격력 : {first.AttackPoint + firstCoinPointTotal} ]",
								sb.ToString(),
								vsLengSb.ToString(),
								$"{second.Name} [ 공격력 : {second.AttackPoint + secondCoinPointTotal} ]",
								sb2.ToString()
				],
				"합", ConsoleColor.DarkYellow
				);
                ConsoleUI.Present();
                turnCount++;
                await Task.Delay(50);
            }

			if (first.Coin > count)
			{
				if (firstNowToss)
				{
					sb[^1] = forwardCoin;

					firstCoinPointTotal += firstCoinPoint;
				}
				else sb[^1] = reverseCoin;
			}

			if (second.Coin > count)
			{
				if (secondNowToss)
				{
					sb2[^1] = forwardCoin;

					secondCoinPointTotal += secondCoinPoint;
				}
				else sb2[^1] = reverseCoin;
			}

            Console.SetCursorPosition(0, currentLineCursor);
            ConsoleUI.Clear();
            ConsoleUI.WriteBox(
			[
				$"{first.Name} [ 공격력 : {first.AttackPoint + firstCoinPointTotal} ]",
				sb.ToString(),
				vsLengSb.ToString(),
				$"{second.Name} [ 공격력 : {second.AttackPoint + secondCoinPointTotal} ]",
				sb2.ToString()
			],
			"합", ConsoleColor.DarkYellow
			);
            ConsoleUI.Present();
            //count가 첫번째 스킬 코인보다도 높고, 두번째 스킬 코인 보다도 높을때 종료

            await Task.Delay(300);
			count++;

		}

		///콘솔 입력 방지 해제
/*        while (Console.KeyAvailable) Console.ReadKey(intercept: true);
        SetConsoleMode(hInput, originalMode);*/
		/// 


		// 합 승리 계산 후 재 루프

		//결과 반환
        return ("", first);
	}

	public Dictionary<string, int> CoinAndAtkPointCalc(Skill skill, Character target)
	{
		int atk = 0;
		int coinPoint = 0;
		int coin = 0;

		foreach(var data in skill.SkillEffect)
		{
			string condition = data[0];
			string type = data[1];
            if (int.TryParse(data[2], out int value)) value = 0;
            
            switch (condition)
			{

				case HAVE_BURN: 
					if (type == ATK) atk += value;
					if (type == COIN_POINT) coinPoint += value;
                    
					break; 

            }

			//data[0]

        }

        Dictionary<string, int> result = new()
        {
            [ATK] = atk,
            [COIN] = coin,
            [COIN_POINT] = coinPoint
        };


        return result;
	}

}
