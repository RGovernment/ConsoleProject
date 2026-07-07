using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Enums;

namespace ConsoleGameFramework.Scenes;

public class ShopScene : SceneBase
{
    //일단 총 5~6개에 목록 2~3개만 (새로고침 없음)
    private static readonly List<MenuOption> Menu = new()
    {
        new MenuOption(1, "아이템 구매", "도움이 아이템을 구매합니다."),
        new MenuOption(0, "마을로", "마을로 돌아갑니다.")
    };

    public override void Enter(GameContext context)
    {
        context.AddLog("상점에 들어왔습니다.");
    }

    public override SceneKey Key => SceneKey.Shop;

    public override void Render(GameContext context)
    {
        ConsoleUI.Clear();

    }

    public override void HandleInput(GameContext context)
    {
        int choice = ConsoleUI.ReadMenuChoice(Menu);

        switch (choice)
        {
            case 0:
                context.Game.RequestQuit();
                break;
        }
    }
}
