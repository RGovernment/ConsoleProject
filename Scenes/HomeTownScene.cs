

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
            new MenuOption(1, "훈련장", "훈련장으로 이동합니다."),
            new MenuOption(2, "상점", "상점으로 이동합니다."),
            new MenuOption(0, "종료", "프로그램을 종료합니다.")
        };

        public override void Enter(GameContext context)
        {
            context.AddLog("던전 마을입니다. 던전 내부로 출발하기 전, 훈련소와 상점에서 정비를 마쳐주세요.");
        }

        public override SceneKey Key => SceneKey.HomeTown;

        public override void Render(GameContext context)
        {
            ConsoleUI.Clear();
            ConsoleUI.WriteTitle("홈타운", "던전의 시작");

            ConsoleUI.WriteTable(
            headers: ["소지금", GameManager.Instance.NowMoney.ToString()],
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
                    int count = 1;
                    while(count <  3)
                    {

                        ConsoleUI.Clear();
                        ConsoleUI.WriteTitle("홈타운", "던전의 시작");

                        ConsoleUI.WriteTable(
                        headers: ["소지금", GameManager.Instance.NowMoney.ToString()],
                        rows: new List<List<string>>()
                        );

                        ConsoleUI.WriteInTrainingHall(count);
                        await Task.Delay(Random.Shared.Next(300, 500));
                        count++;
                    }
                    

                    break;
                case 0:
                    context.Game.RequestQuit();
                    break;
            }
        }

    }
}
