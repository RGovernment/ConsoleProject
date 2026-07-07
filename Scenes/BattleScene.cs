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
        new MenuOption(1, "공격","몬스터를 공격합니다."),
        new MenuOption(0, "도주")
    };

    /*[SerializeField]*/ private BattleManager battleManager;
    StringBuilder sb = new();
    StringBuilder sb2 = new();
    public override SceneKey Key => SceneKey.Battle;

    public override void Enter(GameContext context)
    {
        battleManager = new BattleManager();

        battleManager.StartBattleInit(context.NowRound, 
            RoundData.StageRoundList[context.NowStage][context.NowRound]);
    }

    public override void Render(GameContext context)
    {
        //●○◐◑
        ConsoleUI.Clear();
        ConsoleUI.WriteTitle("전투 개시", $"라운드 : {context.NowRound + 1}");
        ConsoleUI.WriteStatusBar(battleManager.Player.Name, 
            battleManager.Player.Hp, 
            battleManager.Player.MaxHp);
        sb.Clear();
        sb2.Clear();

        battleManager.Enemy.ForEach(x =>
        {
            ConsoleUI.WriteStatusBar(x.Name, x.Hp, x.MaxHp);
        });


        ConsoleUI.WriteLoad(context.NowWallType,
            fifth: "교전중 입니다.", 
            clearActive:false);
        ConsoleUI.WriteMenu(Menu, "행동 메뉴");
        ConsoleUI.WriteLog(context.Logs);
    }

    public override void HandleInput(GameContext context)
    {
        int choice = ConsoleUI.ReadMenuChoice(Menu);
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