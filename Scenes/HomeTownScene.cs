

using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;
using System.Diagnostics.Metrics;
using static ConsoleGameFramework.Common.Enums;

namespace ConsoleGameFramework.Scenes
{
    public class HomeTownScene : SceneBase
    {
        private static readonly List<MenuOption> Menu = new List<MenuOption>
        {
            new MenuOption(1, "훈련장", "프로그램을 종료합니다."),
            new MenuOption(2, "상점", "프로그램을 종료합니다."),
            new MenuOption(0, "종료", "프로그램을 종료합니다.")
        };

        public override void Enter(GameContext context)
        {
            context.AddLog("샘플 화면에 들어왔습니다.");
        }

        public override SceneKey Key => SceneKey.HomeTown;

        public override void Render(GameContext context)
        {
            ConsoleUI.Clear();
            ConsoleUI.WriteTable(
            headers: ["소지금", GameManager.Instance.NowMoney.ToString()],
            rows: new List<List<string>>()
        );

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
}
