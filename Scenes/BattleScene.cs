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
        new MenuOption(0, "도주")
    };
    Skill[] useAbleSkill = new Skill[2];
    /*[SerializeField]*/
    private BattleManager battleManager;

    public override SceneKey Key => SceneKey.Battle;

    public override void Enter(GameContext context)
    {
        context.AddLog("전투를 시작합니다.");
        battleManager = new BattleManager();

        battleManager.StartBattleInit(context.NowRound, 
            RoundData.StageRoundList[context.NowStage][context.NowRound]);
    }

    public override void Render(GameContext context)
    {
        Player nowPlayer = battleManager.Player;
        //●○◐◑
        ConsoleUI.Clear();
        ConsoleUI.WriteTitle("전투 개시", $"라운드 : {context.NowRound + 1}");
        ConsoleUI.WriteStatusBar(nowPlayer.Name, 
            nowPlayer.Sanity,
            nowPlayer.Hp, 
            max:battleManager.Player.MaxHp,
            fillColor:
            nowPlayer.Hp / (float)nowPlayer.MaxHp < 0.5f ? 
            ConsoleColor.Yellow : nowPlayer.Hp / (float)nowPlayer.MaxHp < 0.1f ? 
            ConsoleColor.DarkRed : ConsoleColor.Green);

        battleManager.Enemy.ForEach(x =>
        {
            ConsoleUI.WriteStatusBar(x.Name, x.Sanity, x.Hp, max:x.MaxHp,
                fillColor:
                x.Hp / 100.0f < 0.5f ?
                ConsoleColor.Yellow : x.Hp / (float)x.MaxHp < 0.1f ?
                ConsoleColor.DarkRed : ConsoleColor.Green
            );
        });


        ConsoleUI.WriteLoad(context.NowWallType,
            fifth: "교전중...", 
            clearActive:false);

        

        for (int i = useAbleSkill.Length - 1; i >= 0; i--){
            useAbleSkill[i] = nowPlayer.GetSkillQueue();
            Menu.Add(new MenuOption(i + 1, $"[{useAbleSkill[i].Name}] 사용하기"));
        }
        

        ConsoleUI.WriteMenu(Menu, "행동 메뉴");
        ConsoleUI.WriteLog(context.Logs);
    }

    public async override void HandleInput(GameContext context)
    {
        int choice = ConsoleUI.ReadMenuChoice(Menu);


        await battleManager.SkillClash(
                battleManager.Player.SkillList[choice],
                battleManager.Enemy[0].SkillList[2],
                battleManager.Player,
                battleManager.Enemy[0]
                );

        switch (choice)
        {
            case 1:
                // 플레이어의 공격 Enemy의 Take 데미지
                BattleManager.BattleOutcome result = battleManager.PlayerAttack();
                
                if (result == BattleManager.BattleOutcome.Victory)
                {
                    context.AddLog($"Victory: {result}");
                    // 마을씬으로 이동
                }
                else if (result == BattleManager.BattleOutcome.Defeat)
                {
                    context.AddLog("패배했습니다.");
                    // 게임종료
                }

                break;

            case 0:
                GoTo(context, SceneKey.HomeTown);
                break;
        }
    }
}