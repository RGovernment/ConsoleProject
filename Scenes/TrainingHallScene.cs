using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Enums;

namespace ConsoleGameFramework.Scenes
{
    public class TrainingHallScene : SceneBase
    {
        private int position = 0;
        //세이브 구현이 아직 없으므로, 스킬 교체 구현 때 교체 시 초기화된다는 경고 문구 추가
        private static readonly List<MenuOption> Menu = new()
        {
            new (1, "스킬 강화", "보유중인 스킬을 강화합니다."),
            new (2, "스킬 교체", "보유중인 스킬을 교체합니다."),
            new (0, "마을로", "마을로 돌아갑니다.")
        };
        private static readonly List<MenuOption> Menu2 = new()
        {
            new (1, "1번 스킬 교체", "1번 스킬을 강화합니다."),
            new (2, "2번 스킬 교체", "2번 스킬을 강화합니다."),
            new (3, "3번 스킬 교체", "3번 스킬 강화합니다."),
            new (0, "마을로", "마을로 돌아갑니다.")
        };
        public override void Enter(GameContext context)
        {
            context.AddLog("훈련장에 진입했습니다.");
        }

        public override SceneKey Key => SceneKey.TrainingHall;

        public override void Render(GameContext context)
        {
            ConsoleUI.Clear();
            ConsoleUI.WriteTitle("훈련장", "힘이 모이는 곳");

            ConsoleUI.WriteTable(
            headers: ["소지금", GameManager.Instance.Context.NowMoney.ToString()],
            rows: new List<List<string>>()
            );

            if (position == 0)
            {
                ConsoleUI.WriteTrainingHall();
                ConsoleUI.WriteBox(
                    [
                    " 스킬 강화 : 캐릭터가 가진 스킬을 강화할 수 있다.",
                " 스킬 교체 : 캐릭터가 가진 스킬을 교체할 수 있다. ※\n교체시 이전 스킬은 초기화된다.※",
                "    마을로 : 마을로 돌아간다."
                    ], "마을 설명", ConsoleColor.DarkCyan);
                ConsoleUI.WriteMenu(Menu, "선택 메뉴");
            }
            else if(position == 1)
            {
                ConsoleUI.WriteBox(
                [
                    "캐릭터가 가진 스킬을 강화할 수 있습니다.",
                    "강화된 스킬은 기존 스킬보다 조금 더 강해지며, 강화시 스킬 단계에 비례해 골드가 소모됩니다.",
                    "강화된 스킬은 교체하기 전까지 이전 단계로 되돌릴 수 없으며, 교체시 다시 강화해야 합니다.",
                ], "스킬 강화 설명", ConsoleColor.DarkCyan);

                ConsoleUI.WriteMenu(Menu2, "선택 메뉴");
            }
            else if(position == 2)
            {
                ConsoleUI.WriteBox(
                [
                    "캐릭터가 가진 스킬을 교체할 수 있습니다.",
                    "교체에는 스킬의 단계에 비례하는 소량의 골드가 소모됩니다.",
                    "강화된 스킬을 교체하면 강화단계가 초기화 됩니다.",
                    "아직 미구현되었습니다.",
                ], "스킬 교체 설명", ConsoleColor.DarkCyan);
            }
        }

        public override void HandleInput(GameContext context)
        {
            int choice = ConsoleUI.ReadMenuChoice(Menu);
            if (position == 1)
            {
                string text = ConsoleUI.ReadString("정말 강화하시겠습니까? [예/아니오]", "아니오");

                if (text != null && text == "예")
                {
                    //choice된 숫자 강화 구현
                }
            }
            switch (choice)
            {
                case 3:
                    if (position == 1)
                    {
                    }
                    break;
                case 2:
                    if (position == 0) position = 2;
                    else if(position == 1)
                    {
                    }
                    break;
                case 1:
                    if(position == 0) position = 1;
                    else if (position == 1)
                    {
                    }
                    break;
                case 0:
                    if(position != 0) 
                    {
                        position = 0;
                    }
                    context.Game.RequestQuit();
                    break;
            }
        }
    }
}
