using ConsoleGameFramework.Common;
using ConsoleGameFramework.Data;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Skills;
using ConsoleGameFramework.UI;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using static ConsoleGameFramework.Common.Constants;
using static ConsoleGameFramework.Common.Utility;
using static ConsoleGameFramework.Data.BossData;
namespace ConsoleGameFramework.Core;


public class BattleManager
{
/*    // 윈도우 콘솔 제어를 위한 Windows API 가져오기
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    const uint ENABLE_ECHO_INPUT = 0x0004;    // 사용자가 입력한 키를 화면에 그리는 모드
    const uint ENABLE_LINE_INPUT = 0x0002;    // 엔터키를 누를 때까지 입력받는 모드*/

    // 플레이어
    public Player Player { get; private set; }
	// 적
	public List<Enemy> Enemy { get; private set; }

    public Action TurnActive;

    public Action<Player,List<Enemy>> TurnEnd;
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
			int maxHp = enemyData.TryGetValue(MAX_HP, out object? maxHpVal) &&
                maxHpVal is int hpData ? hpData : 80;
			
			Enemy.Add(new(id, name, maxHp,
				[
                    new Skill(SkillData.EnemySkills[0], 0),
                    new Skill(SkillData.EnemySkills[1], 0),
                    new Skill(SkillData.EnemySkills[2], 0)
                ] 
				));
		}

        TurnActive -= Player.CommonBuffAndDebuffDurationDiscount;
        TurnActive -= Player.TakeDotDamage;
        TurnActive += Player.CommonBuffAndDebuffDurationDiscount;
        TurnActive += Player.TakeDotDamage;

        // 보스 턴 시작, 지속 이벤트 구독은 제외
        Enemy.ForEach(x => {
            TurnActive += x.CommonBuffAndDebuffDurationDiscount;
            TurnActive += x.TakeDotDamage;
        });
        
        
    }
    // next
    public void NextBattleInit(int round,Player player, Dictionary<string, object> roundData)
    {
        Player = player;
        Player.EffectReset();
        Enemy = new();

        foreach (var data in roundData)
        {
            Dictionary<string, object> enemyData = (Dictionary<string, object>)data.Value;
            string id =
                enemyData.TryGetValue(ID, out object? idVal) &&
                idVal is string idData ? idData : "defaultId";
            string name =
                enemyData.TryGetValue(NAME, out object? nameVal) &&
                nameVal is string nameData ? nameData : "기본수";
            int maxHp = enemyData.TryGetValue(MAX_HP, out object? maxHpVal) &&
                maxHpVal is int hpData ? hpData : 80;

            // 보스 스테이지일 경우
            if (enemyData.ContainsKey(IS_BOSS))
            {
                Enemy.Add(new Boss(id, name, maxHp, bossData[id],
                [
                    new Skill(SkillData.BossSkills[0], 0),
                    new Skill(SkillData.BossSkills[1], 0),
                    new Skill(SkillData.BossSkills[2], 0)
                ]
               ));
            }
            else
            {
                Enemy.Add(new(id, name, maxHp,
                [
                    new Skill(SkillData.EnemySkills[0], 0),
                    new Skill(SkillData.EnemySkills[1], 0),
                    new Skill(SkillData.EnemySkills[2], 0)
                ]
                ));
            }
        }

        TurnActive -= Player.CommonBuffAndDebuffDurationDiscount;
        TurnActive -= Player.TakeDotDamage;
        TurnActive += Player.CommonBuffAndDebuffDurationDiscount;
        TurnActive += Player.TakeDotDamage;

        Enemy.ForEach(x => {
            if (x is Boss bX) TurnEnd += bX.TurnEndPassiveEffect;
            TurnActive += x.CommonBuffAndDebuffDurationDiscount;
            TurnActive += x.TakeDotDamage;
        });
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
    public (Character winner, Character loser, Skill skill) SkillClash(
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
        Dictionary<string, int> firstCoinAtkBuffVal = CoinAndAtkPointBuffCalc(firstChara);
        Dictionary<string, int> secondCoinAtkBuffVal = CoinAndAtkPointBuffCalc(secondChara);

        int firstAtk = first.AttackPoint + firstCoinAtkVal[ATK] + firstCoinAtkBuffVal[ATK];
        int secondAtk = second.AttackPoint + secondCoinAtkVal[ATK] + secondCoinAtkBuffVal[ATK];
        int firstCoinPoint = first.CoinPoint + firstCoinAtkVal[COIN_POINT] + firstCoinAtkBuffVal[COIN_POINT];
        int secondCoinPoint = second.CoinPoint + secondCoinAtkVal[COIN_POINT] + secondCoinAtkBuffVal[COIN_POINT];

        int totalCrashCount = 0;
        // 이름 배정
        int vsLeng = 0;

		if (first.Name?.Length >= second.Name?.Length) vsLeng = first.Name.Length;
		else vsLeng = second.Name != null ? second.Name.Length : 2;

		StringBuilder vsLengSb = new();
        vsLengSb.Append("vs");
        string vsString = vsLengSb.StrLengExtend(Math.Max(2, vsLeng), "vs");
        
        string[] lastData = new string[5];
        //이후 구간, 계산 및 출력 반복
        // 출력된 10줄 시작점으로 이동해 다시 콘솔 출력(코인 던지기 반복 어색하지 않게)
        int currentLineCursor = Console.CursorTop - 10;
        
        Console.SetCursorPosition(0, currentLineCursor);
        for (int i = 0; i < 10; i++)
        {
            Console.Write(new string(' ', Console.WindowWidth));
        }

        firstChara.TakeBuff(USE, first.SkillEffect);
        secondChara.TakeBuff(USE, second.SkillEffect);

        //합 전체가 끝날때까지 루프
        while (true)
		{
			if (first.Coin == 0 || second.Coin == 0) break;
			int firstCoinPointTotal = 0;
			int secondCoinPointTotal = 0;
            
			sb.Clear();
			sb2.Clear();
			int count = 0;

			// 루프
			while (true)
			{
				if (count >= first.Coin && count >= second.Coin) break;
                // 공격자 방어자 코인 앞/뒤 체크
				bool firstNowToss = CoinToss(firstChara.Sanity);
				bool secondNowToss = CoinToss(secondChara.Sanity);
				int coinFlip = 0;
				int turnCount = 0;
				int turnMax = 20;
                // 현재 합 횟수 출력
                vsLengSb.Clear();
                vsLengSb.Append(vsString); 
                if(totalCrashCount > 0) vsLengSb.Append($"{totalCrashCount}합");

                // 공격력 or 코인 위력에 추가 조건이 있는지 검사
                if (first.Coin > count) sb.Append(leftCoin);
				if (second.Coin > count) sb2.Append(leftCoin);

				while (turnCount < turnMax)
				{
					char nowCoin = coinFlip == 0 ? leftCoin :
								   coinFlip == 1 ? reverseCoin :
								   coinFlip == 2 ? rightCoin :
												   forwardCoin;

					if (coinFlip < 3) coinFlip++;
					else coinFlip = 0;

					if (first.Coin > count && sb.Length > 0) sb[^1] = nowCoin;
					if (second.Coin > count && sb2.Length > 0) sb2[^1] = nowCoin;
					Console.SetCursorPosition(0, currentLineCursor);
					if (count != 0 || turnCount != 0)
					{
						ConsoleUI.ClearRange(10);
					}

					ConsoleUI.WriteBox(
					[
						$"{first.Name} [ 공격력 : {firstAtk + firstCoinPointTotal} ]",
									sb.ToString(),
                                    vsLengSb.ToString(),
									$"{second.Name} [ 공격력 : {secondAtk + secondCoinPointTotal} ]",
									sb2.ToString()
					],
					"합", ConsoleColor.DarkYellow
					);
					ConsoleUI.Present();
					turnCount++;
					Thread.Sleep(18);
				}
                // 공격자가 아직 코인이 남은 경우 코인 계산
				if (first.Coin > count)
				{
					if (firstNowToss)
					{
						sb[^1] = forwardCoin;

						firstCoinPointTotal += firstCoinPoint;
					}
					else sb[^1] = reverseCoin;
				}
                // 방어자가 아직 코인이 남은 경우 코인 계산 2
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
				ConsoleUI.ClearRange(10);
                lastData = [
                    $"{first.Name} [ 공격력 : {firstAtk + firstCoinPointTotal} ]",
                    sb.ToString(),
                    vsLengSb.ToString(),
                    $"{second.Name} [ 공격력 : {secondAtk + secondCoinPointTotal} ]",
                    sb2.ToString()
                ];

                ConsoleUI.WriteBox(
				[
					$"{first.Name} [ 공격력 : {firstAtk + firstCoinPointTotal} ]",
					sb.ToString(),
                    vsLengSb.ToString(),
					$"{second.Name} [ 공격력 : {secondAtk + secondCoinPointTotal} ]",
					sb2.ToString()
				],
				"합", ConsoleColor.DarkYellow
				);
				ConsoleUI.Present();
				//count가 첫번째 스킬 코인보다도 높고, 두번째 스킬 코인 보다도 높을때 종료

				Thread.Sleep(100);

				count++;

                if (turnCount != 0 || count != 0 && count >= first.Coin && count >= second.Coin)
                {
                    Console.SetCursorPosition(0, currentLineCursor);
                    for (int i = 0; i < 10; i++)
                    {
                        Console.Write(new string(' ',
                            Console.WindowWidth));
                    }
                }
            }

			// 합 승리 계산 후 재 루프
			if (firstAtk + firstCoinPointTotal > secondAtk + secondCoinPointTotal)
				second.Coin--;
			else if (firstAtk + firstCoinPointTotal < secondAtk + secondCoinPointTotal)
				first.Coin--;

            totalCrashCount++;
        }
        ///콘솔 입력 방지 해제
        /*        while (Console.KeyAvailable) Console.ReadKey(intercept: true);
                SetConsoleMode(hInput, originalMode);*/
        /// 
        int bonus = (totalCrashCount >= 2) ? (totalCrashCount - 1) * 2 : 0;
        //결과 반환
        //ConsoleUI.Present();
        if (first.Coin == 0)
        {
            Character winner = secondChara;
            winner.GainSanity(10 + bonus);

            return (winner, firstChara, second);
        }
        else 
        {
            Character winner = firstChara;
            winner.GainSanity(10 + bonus);

            return (winner, secondChara, first);
        }
	}

    /// <summary>
    /// 공격 스킬 대미지 판정 계산 (동전 있음), 구조는 합 로직과 동일
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <returns></returns>
	public int SkillDamage(Skill skill,Character user, Character target)
	{
        // 기본 정보
        StringBuilder sb = new();
        char leftCoin = '◐';
        char reverseCoin = '○';
        char rightCoin = '◑';
        char forwardCoin = '●';
        //

        // skill 데이터 로드, 추가 조건 검사(위력 / 코인값 추가되는지)
        Dictionary<string, int> coinAtkVal = CoinAndAtkPointCalc(skill, target);
        Dictionary<string, int> coinAtkBuffVal = CoinAndAtkPointBuffCalc(user);

        int atk = skill.AttackPoint + coinAtkVal[ATK] + coinAtkBuffVal[ATK];
        int coinPoint = skill.CoinPoint + coinAtkVal[COIN_POINT] + coinAtkBuffVal[COIN_POINT];
        int coinPointTotal = 0;
        // 

        sb.Clear();
        int count = 0;

        // 출력된 7줄 시작점으로 이동해 다시 콘솔 출력(코인 던지기 반복 어색하지 않게)
        int currentLineCursor = Console.CursorTop - 7;

        // 루프
        while (true)
        {
            if (count >= skill.Coin) break;
            bool firstNowToss = CoinToss(user.Sanity);
            int coinFlip = 0;
            int turnCount = 0;
            int turnMax = 20;

            if (skill.Coin > count) sb.Append(leftCoin);

            while (turnCount < turnMax)
            {
                char nowCoin = coinFlip == 0 ? leftCoin :
                               coinFlip == 1 ? reverseCoin :
                               coinFlip == 2 ? rightCoin :
                                               forwardCoin;

                if (coinFlip < 3) coinFlip++;
                else coinFlip = 0;

                if (skill.Coin > count && sb.Length > 0) sb[^1] = nowCoin;
                Console.SetCursorPosition(0, currentLineCursor);
                if (count != 0 || turnCount != 0)
                {
                    ConsoleUI.ClearRange(7);
                }

                ConsoleUI.WriteBox(
                [
                    $"{skill.Name} [ 공격력 : {atk + coinPointTotal} ]",
                                    sb.ToString()
                ],
                "대미지", ConsoleColor.DarkYellow
                );
                ConsoleUI.Present();
                turnCount++;
                Thread.Sleep(9);
            }

            if (skill.Coin > count)
            {
                if (firstNowToss)
                {
                    sb[^1] = forwardCoin;

                    coinPointTotal += coinPoint;
                }
                else sb[^1] = reverseCoin;
            }

            Console.SetCursorPosition(0, currentLineCursor);
            ConsoleUI.ClearRange(7);
            ConsoleUI.WriteBox(
            [
                $"{skill.Name} [ 공격력 : {atk + coinPointTotal} ]",
                    sb.ToString()
            ],
            "대미지", ConsoleColor.DarkYellow
            );
            ConsoleUI.Present();

            Thread.Sleep(50);
            count++;
        }
        Thread.Sleep(50);
        int damageIncrease = 100 + CoinAndAtkPointCalc(skill, target)[DAMAGE];
        int totalDamage = 0;
        if (damageIncrease == 0)
        {
            totalDamage = atk + coinPointTotal;
        }
        else
            totalDamage = (int)Math.Round((atk + coinPointTotal) * damageIncrease / 100f);

        target.TakeDebuff(GIVEN_HIT, skill.SkillEffect);
		target.TakeDamage(totalDamage);

		return totalDamage;
    }

    /// <summary>
    /// 공격 스킬 대미지 판정 계산 (동전 없음)
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="user"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public int SkillDamageInstant(Skill skill, Character user, Character target)
    {
        // skill 데이터 로드, 추가 조건 검사(위력 / 코인값 추가되는지)
        Dictionary<string, int> coinAtkVal = CoinAndAtkPointCalc(skill, target);
        Dictionary<string, int> coinAtkBuffVal = CoinAndAtkPointBuffCalc(user);

        int atk = skill.AttackPoint + coinAtkVal[ATK] + coinAtkBuffVal[ATK];
        int coinPoint = skill.CoinPoint + coinAtkVal[COIN_POINT] + coinAtkBuffVal[COIN_POINT];
        int coinPointTotal = 0;
        // 

        int count = 0;



        // 루프
        while (true)
        {
            if (count >= skill.Coin) break;
            bool firstNowToss = CoinToss(user.Sanity);
            int turnCount = 0;

            if(firstNowToss) coinPointTotal += coinPoint;
 
            count++;
        }
        int damageIncrease = CoinAndAtkPointCalc(skill, target)[DAMAGE];
        int totalDamage = 0;
        if (damageIncrease == 0)
        {
            totalDamage = atk + coinPointTotal;
        }
        else
            totalDamage = (int)Math.Round((atk + coinPointTotal) * damageIncrease / 100f);


        target.TakeDebuff(GIVEN_HIT, skill.SkillEffect);
        target.TakeDamage(totalDamage);

        return totalDamage;
    }

    public void CharaListClean()
	{
        Enemy.ForEach(x => {
            if (!x.IsAlive)
            {
                TurnActive -= x.TakeDotDamage;
                TurnActive -= x.CommonBuffAndDebuffDurationDiscount;
            }
        });
        Enemy.RemoveAll(x => !x.IsAlive);
    }
    public void GameOver()
    {
        Console.WriteLine("게임 오버");
        GameManager.Instance.ChangeScene(SceneKey.Title);
    }

    public Dictionary<string, int> CoinAndAtkPointCalc(Skill skill, Character target)
	{
		int atk = 0;
		int coinPoint = 0;
		int coin = 0;
        int damage = 0;
        // 디버프로 인한 스킬효과로 상승하는 피해량
		foreach(var data in skill.SkillEffect)
		{
			string condition = data[0];
			string type = data[1];
            if (int.TryParse(data[2], out int value))
            {
                switch (condition)
                {
                    case HAVE_BURN:
                        if (target.DebuffList.Count > 0 &&
                            target.DebuffList.Any(x => x.Id == BURN))
                        {
                            if (type == ATK) atk += value;
                            if (type == COIN_POINT) coinPoint += value;
                            if (type == DAMAGE) damage += value;
                        }
                        break;
                }
            }
        }

        Dictionary<string, int> result = new()
        {
            [ATK] = atk,
            [COIN] = coin,
            [COIN_POINT] = coinPoint,
            [DAMAGE] = damage
        };


        return result;
	}

    public Dictionary<string, int> CoinAndAtkPointBuffCalc(Character user)
    {
        int atk = 0;
        int coinPoint = 0;
        int coin = 0;
        int damage = 0;


        //버프를 가지고 있을 경우 상승하는 피해량
        foreach (var data in user.BuffList)
        {
            string id =data.Id;
            if(id == ATK_POINT)
            {
                 atk += data.Coefficient;
            }
        }

        Dictionary<string, int> result = new()
        {
            [ATK] = atk,
            [COIN] = coin,
            [COIN_POINT] = coinPoint,
            [DAMAGE] = damage
        };


        return result;
    }

    public bool PlayCk()
    {
        if(Enemy.Any(x => x is Boss data && !data.IsAlive))
        {
            Boss data = (Boss)Enemy.Find(x => x is Boss);
            TurnEnd -= data.TurnEndPassiveEffect;
            //TurnStart -= data.TurnStartPassiveEffect;
            //Always -= data.AlwaysPassiveEffect;
        }

        return Player.IsAlive && Enemy.Any(x => x.IsAlive);
    }

}
