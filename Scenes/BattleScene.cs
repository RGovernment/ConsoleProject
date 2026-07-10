using ConsoleGameFramework.Core;
using ConsoleGameFramework.Data;
using ConsoleGameFramework.Data;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Skills;
using ConsoleGameFramework.UI;
using System.Runtime.CompilerServices;
using System.Text;
using static ConsoleGameFramework.Common.Constants;

namespace ConsoleGameFramework.Scenes;

public class BattleScene : SceneBase
{
    private static readonly List<MenuOption> Menu = new List<MenuOption>
    {
    };
    List<Skill> useAbleSkill = new();
    /*[SerializeField]*/
    private BattleManager battleManager;
    private int chooseSkillNum = 0;
    private int chooseEnemyNum = 0;

    

    public override SceneKey Key => SceneKey.Battle;

    public override void Enter(GameContext context)
    {
        Menu.Clear();
        useAbleSkill.Clear();
        context.AddLog("전투를 시작합니다.");
        battleManager = new BattleManager();
        // 데이터 로드 
        battleManager.StartBattleInit(context.NowRound, 
            RoundData.StageRoundList[context.NowStage][context.NowRound]);
        battleManager.Player.OnDead += battleManager.GameOver;

        // 아군 첫 스킬 목록 로딩
        for (int i = 0; i < 2; i++)
        {
            useAbleSkill.Add(battleManager.Player.GetSkillQueue());
            Menu.Add(new MenuOption(i + 1, $"[{useAbleSkill[i].Name}] 스킬 사용"));
        }

        Menu.Add(new MenuOption(0, "도주"));
    }

    public override void Render(GameContext context)
    {
        // 플레이어 로드

        Player nowPlayer = battleManager.Player;

        //턴  시작시
        battleManager.TurnActive?.Invoke();

        //턴 시작 효과로 인해 사망한 캐릭터 여부 확인
        bool ck = battleManager.PlayCk();
        if (!ck)
        {
            if (nowPlayer.IsAlive)
            {
                if (context.IsBoss)
                {
                    context.NowStage++;
                    // 보스 승리 시
                    // 승리 멘트 + 게임 종료
                    Console.WriteLine("게임 오버");
                    context.Game.RequestQuit();
                    return;
                }
                else
                {
                    context.NowRound++;
                    battleManager = new BattleManager();
                    // 이번 스테이지에 다음 일반 라운드가 존재하지 않을 경우 보스 라운드로 진입
                    if (RoundData.StageRoundList[context.NowStage].Count <= context.NowRound)
                    {
                        // 보스 데이터 로드
                        battleManager.NextBattleInit(context.NowStage, nowPlayer,
                        RoundData.BossRoundList[context.NowStage]);
                        context.IsBoss = true;
                    }
                    // 아닌 경우 일반 라운드로 진입
                    else
                    {
                        // 데이터 로드, 플레이어 정보를 유지하기 위해 Enemy만 로드
                        battleManager.NextBattleInit(context.NowRound, nowPlayer,
                        RoundData.StageRoundList[context.NowStage][context.NowRound]);

                    }
                    battleManager.Player.RoundClear();
                    return;
                }
            }
            else
            {
                context.IsLose = true;
                ConsoleUI.WriteLine("전투 패배");
                context.Game.RequestQuit();
            }
            return;
        }

        ConsoleUI.Clear();
        if (context.IsBoss)
        {
            Boss? data = battleManager.Enemy.Find(x => x is Boss data2) as Boss;

            ConsoleUI.WriteTitle("보스 라운드", $"※ 보스는 항상 최소 1개 이상의 패시브를 가지고 있습니다.");
            StringBuilder sb = new();
            if (data != null)
            {
                string[] passiveData = BossData.bossData[data.Id];

                ConsoleUI.WriteBox([passiveData[5], $""], "보스 패시브");
            }
            else
            {
                ConsoleUI.WriteBox(["없음", $""], "보스 패시브");
            }

        } else
            ConsoleUI.WriteTitle("전투 개시", $"라운드 : {context.NowRound + 1}");
        // StatusBar 오버로딩으로 Sanity까지 표시하도록 변경
        ConsoleUI.WriteStatusBar(nowPlayer, nowPlayer.Name,
            nowPlayer.Sanity,
            nowPlayer.Hp, 
            max:battleManager.Player.MaxHp,
            fillColor:
            nowPlayer.Hp / (float)nowPlayer.MaxHp < 0.5f ? 
            ConsoleColor.DarkYellow : nowPlayer.Hp / (float)nowPlayer.MaxHp < 0.1f ? 
            ConsoleColor.DarkRed : ConsoleColor.Green);
        // 적 상태바
        battleManager.Enemy.ForEach(x =>
        {
            ConsoleUI.WriteStatusBar(x, x.Name, x.Sanity, x.Hp, max:x.MaxHp,
                fillColor:
                x.Hp / (float)x.MaxHp < 0.5f ?
                ConsoleColor.DarkYellow : x.Hp / (float)x.MaxHp < 0.1f ?
                ConsoleColor.DarkRed : ConsoleColor.Green
            );
            // 적 스킬 목록 생성
            x.MakeSkillQueue();

            // 사용할 스킬 확인
            Skill data = x.SkillQueue.Peek();

            // 확인 스킬 정보 출력
            string[] writeData = [
                        "=스킬 정보=",
                        $"이름 : {data.Name}",
                        $"공격력 : {data.AttackPoint}",
                        $"코인 수 : {data.Coin}개 ",
                        $"코인 위력 : +{data.CoinPoint}",
                        $"- 사용 시 효과 -"
            ];
            string[] writeDesc = data.Description.Split("/");
            ConsoleUI.WriteBox(writeData.Concat(writeDesc),"이번 턴 사용 스킬");


            // 적 사망 로직 추가
            x.OnDead -= battleManager.CharaListClean;
            x.OnDead += battleManager.CharaListClean;
        });

        /*ConsoleUI.WriteLoad(context.NowWallType,
            fifth: "교전중...", 
            clearActive:false);*/
        
        ConsoleUI.WriteMenu(Menu, "행동 메뉴");
        ConsoleUI.WriteLog(context.Logs);
        
        // 스킬 출력
        ConsoleUI.Present();
        chooseSkillNum = ConsoleUI.ReadMenuChoice(Menu);
    }

    public async override void HandleInput(GameContext context)
    {
        if (context.IsLose) return;
        if (chooseSkillNum == 0)
        {
            useAbleSkill.Clear();
            battleManager = new BattleManager();
            context.NowRound = 0;
            context.NowStage = 0;
            GoTo(context, SceneKey.HomeTown); return;
        }

        List<MenuOption> Menu2 = new();
        Skill playerSkill = useAbleSkill[chooseSkillNum - 1];
        // 선택한 스킬 정보 출력
        string[] writeData = [
                        "=스킬 정보=",
                        $"이름 : {playerSkill.Name}",
                        $"공격력 : {playerSkill.AttackPoint}",
                        $"코인 수 : {playerSkill.Coin}개 ",
                        $"코인 위력 : +{playerSkill.CoinPoint}",
                        $"- 사용 시 효과 -"
            ];
        string[] writeDesc = playerSkill.Description.Split("/");
        ConsoleUI.WriteBox(writeData.Concat(writeDesc), "선택한 스킬");
        for (int i = 0; i < battleManager.Enemy.Count; i++)
            Menu2.Add(new(i + 1, battleManager.Enemy[i].Name));
        Menu2.Add(new(0, "스킬 사용 취소"));

        ConsoleUI.WriteMenu(Menu2, "행동 메뉴");
        // 적 선택지 출력
        int choice = ConsoleUI.ReadInt("적 선택", 0, battleManager.Enemy.Count);

        if (choice == 0)
        {
            context.AddLog("스킬 사용 취소");
            return;
        }
        // 공격자/피격자 스킬 로드 
        
        Skill enemySkill = battleManager.Enemy[choice - 1].GetSkillQueue();
        Player playerData = battleManager.Player;
        Enemy enemyData = battleManager.Enemy[choice - 1];

        // 합 결과
        (Character winner,Character loser, Skill skill) = battleManager.SkillClash(
                new(playerSkill),
                new (enemySkill),
                playerData,
                enemyData
                );

        // 합 결과로 남은 스킬 데이터로 대미지 계산
        int damage = battleManager.SkillDamage(skill, winner, loser);

        // 로그
        context.AddLog($"{winner.Name}이(가) {loser.Name}에게 {damage}의 피해를 주었습니다.");

        // 목록에서 플레이어가 사용한 스킬 제거
        useAbleSkill.RemoveAt(useAbleSkill.FindIndex(x=> x.Id == playerSkill.Id));
        // 큐에서 스킬 하나 뽑아옴
        useAbleSkill.Add(playerData.GetSkillQueue());
        // 메뉴 클리어
        Menu.Clear();
        // 메뉴 리로드
        for (int i = 0; i < useAbleSkill.Count; i++)
        {
            Menu.Add(new MenuOption(i + 1, $"[{useAbleSkill[i].Name}] 스킬 사용"));
        }
        Menu.Add(new MenuOption(0, "도주"));

        //플레이어와 합을 하지 않은 적의 일방공격 계산
        for (int i = battleManager.Enemy.Count - 1; i >= 0; i--)
        {
            Enemy x = battleManager.Enemy[i];

            if (x.Id != enemyData.Id)
            {
                Skill otherSkill = x.GetSkillQueue();
                x.TakeBuff(USE, otherSkill.SkillEffect);
                int oneDamage = battleManager.SkillDamageInstant(otherSkill, x, playerData);
                context.AddLog($"{x.Name}이(가) {playerData.Name}에게 {oneDamage}의 피해를 주었습니다.");
            }
        }

        battleManager.TurnEnd?.Invoke(battleManager.Player, battleManager.Enemy);

        bool ck = battleManager.PlayCk();
        if (!ck)
        {
            if (playerData.IsAlive)
            {
                if (context.IsBoss)
                {
                    context.NowStage++;
                    // 보스 승리 시
                    // 승리 멘트 + 게임 종료
                    Console.WriteLine("게임 오버");
                    context.Game.RequestQuit();
                    return;
                }
                else
                {
                    context.NowRound++;
                    battleManager = new BattleManager();
                    // 이번 스테이지에 다음 일반 라운드가 존재하지 않을 경우 보스 라운드로 진입
                    if(RoundData.StageRoundList[context.NowStage].Count <= context.NowRound)
                    {
                        // 보스 데이터 로드
                        battleManager.NextBattleInit(context.NowStage, playerData,
                        RoundData.BossRoundList[context.NowStage]);
                        context.IsBoss = true;
                    }
                    // 아닌 경우 일반 라운드로 진입
                    else
                    {
                        // 데이터 로드, 플레이어 정보를 유지하기 위해 Enemy만 로드
                        battleManager.NextBattleInit(context.NowRound, playerData,
                        RoundData.StageRoundList[context.NowStage][context.NowRound]);
                        
                    }
                    battleManager.Player.RoundClear();
                    return;
                }
            }
            else
            {
                context.IsLose = true;
                ConsoleUI.WriteLine("전투 패배");
                context.Game.RequestQuit();
            }
        }

        return;
    }
}