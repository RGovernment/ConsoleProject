

using ConsoleGameFramework.Core;
using ConsoleGameFramework.Data;
using ConsoleGameFramework.Skills;
using ConsoleGameFramework.UI;
using System.Diagnostics.Metrics;
using static ConsoleGameFramework.Common.Enums;

namespace ConsoleGameFramework.Scenes
{
    public class HomeTownScene : SceneBase
    {
        private static readonly List<MenuOption> Menu = new()
        {
            new (1, "훈련장", "훈련장으로 이동합니다."),
            new (2, "상점", "상점으로 이동합니다."),
            new (3, "출발", "던전 내부로 이동합니다."),
            new (0, "메인화면", "메인화면으로 이동합니다.")
        };

        public override void Enter(GameContext context)
        {
            context.AddLog("던전 마을입니다.");
        }

        public override SceneKey Key => SceneKey.HomeTown;

        public override void Render(GameContext context)
        {
            ConsoleUI.Clear();
            ConsoleUI.WriteTitle("홈타운", "던전의 시작");

            ConsoleUI.WriteTable(
            headers: ["소지금", GameManager.Instance.Context.NowMoney.ToString()],
            rows: new List<List<string>>()
            );

            ConsoleUI.WriteTown();
            ConsoleUI.WriteBox(
                [
                "  훈련장 : 캐릭터가 가진 스킬을 강화할 수 있다.",
                            "    상점 : 캐릭터가 소지할 수 있는 아이템을 살 수 있다.",
                            "던전입구 : 던전으로 출발한다."
                ], "마을 설명", ConsoleColor.DarkCyan);
            ConsoleUI.WriteMenu(Menu, "시작 메뉴");
        }

        public async override void HandleInput(GameContext context)
        {
            int choice = ConsoleUI.ReadMenuChoice(Menu);

            switch (choice)
            {
                case 1:
                    GoTo(context, SceneKey.TrainingHall);
                    break;
                case 2:
                    GoTo(context, SceneKey.Shop);
                    break;
                case 3:
                    GoTo(context, SceneKey.Loading);
                    break;
                case 0:
                    GoTo(context, SceneKey.Title);
                    break;
            }
        }

    }
}
