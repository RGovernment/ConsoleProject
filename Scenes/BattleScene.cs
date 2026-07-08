using ConsoleGameFramework.Core;
using ConsoleGameFramework.Data;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Skills;
using ConsoleGameFramework.UI;
using System;
using System.Text;

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

    private Action TurnActive;

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
        // 플레이어 사망 로직 이벤트 추가
        nowPlayer.OnDead += battleManager.GameOver;
        ConsoleUI.Clear();
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
                x.Hp / 100.0f < 0.5f ?
                ConsoleColor.DarkYellow : x.Hp / (float)x.MaxHp < 0.1f ?
                ConsoleColor.DarkRed : ConsoleColor.Green
            );
            // 적 사망 로직 추가
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
        if (chooseSkillNum == 0)
        {
            useAbleSkill.Clear();
            battleManager = new BattleManager();
            GoTo(context, SceneKey.HomeTown); return;
        }

        List<MenuOption> Menu2 = new();

        for(int i = 0; i < battleManager.Enemy.Count; i++)
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
        Skill playerSkill = useAbleSkill[chooseSkillNum - 1];
        Skill enemySkill = battleManager.Enemy[choice - 1].GetSkillQueue();
        Player playerData = battleManager.Player;
        Enemy enemyData = battleManager.Enemy[choice - 1];

        // 합 결과
        (Character winner,Character loser, Skill skill) = battleManager.SkillClash(
                new(playerSkill),
                new (enemyData.GetSkillQueue()),
                playerData,
                enemyData
                ).Result;

        // 합 결과로 남은 스킬 데이터로 대미지 계산
        int damage = battleManager.SkillDamage(skill, winner, loser).Result;

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

        bool ck = battleManager.PlayCk();
        if (!ck)
        {
            context.NowRound++;
            battleManager = new BattleManager();
            
            // 데이터 로드, 플레이어 정보를 유지하기 위해 Enemy만 로드
            battleManager.NextBattleInit(context.NowRound, playerData,
                RoundData.StageRoundList[context.NowStage][context.NowRound]);
            battleManager.Player.StageClear();
        }

        return;
    }
}