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
    private static readonly List<MenuOption> Menu = new()
    {
        new MenuOption(0, "종료", "프로그램을 종료합니다.")
    };

    public override void Enter(GameContext context)
    {
        context.AddLog("로딩 화면에 들어왔습니다.");
    }

    public override SceneKey Key => SceneKey.Loading;

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
