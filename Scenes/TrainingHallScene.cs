using ConsoleGameFramework.Core;
using ConsoleGameFramework.Skills;
using ConsoleGameFramework.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Enums;

namespace ConsoleGameFramework.Scenes
{
    public class TrainingHallScene : SceneBase
    {
        private List<Skill> nowSkillData = new();
        private bool[] upgradeAble = new bool[3];
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
            new (1, "1번 스킬 강화", "1번 스킬을 강화합니다."),
            new (2, "2번 스킬 강화", "2번 스킬을 강화합니다."),
            new (3, "3번 스킬 강화", "3번 스킬을 강화합니다."),
            new (0, "목록으로", "훈련장 목록으로 돌아갑니다.")
        };
        private static readonly List<MenuOption> Menu3 = new()
        {
            new (1, "1번 스킬 교체", "1번 스킬을 교체합니다."),
            new (2, "2번 스킬 교체", "2번 스킬을 교체합니다."),
            new (3, "3번 스킬 교체", "3번 스킬을 교체합니다."),
            new (0, "목록으로", "훈련장 목록으로 돌아갑니다.")
        };
        private Dictionary<int, int> SkillUpgradeCost = new()
        {
            [1] = 50,
            [2] = 100,
            [3] = 200
        };

        public override void Enter(GameContext context)
        {
            context.AddLog("훈련장에 진입했습니다.");
            nowSkillData = PlayerManager.Instance.playerStatus.SkillList;
        }

        public override SceneKey Key => SceneKey.TrainingHall;

        public override void Render(GameContext context)
        {
            ConsoleUI.Clear();
            ConsoleUI.WriteTitle("훈련장", "힘이 모이는 곳");

            ConsoleUI.WriteTable(
            headers: ["이름", "로벤"],
            rows: new List<List<string>>() { 
                new(){ "소지금", GameManager.Instance.Context.NowMoney.ToString() }
            }
            );

            if (position == 0)
            {
                ConsoleUI.WriteTrainingHall();
                ConsoleUI.WriteBox(
                    [
                    " 스킬 강화 : 캐릭터가 가진 스킬을 강화할 수 있다.",
                " 스킬 교체 : 캐릭터가 가진 스킬을 교체할 수 있다.",
                "           ※ 교체시 이전 스킬은 초기화된다.※",
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
                // 현재 스킬 조회 
                int count = 0;
                nowSkillData.ForEach(x => {

                    // 현재 단계가 최대 단계가 아닐 경우
                    if(x.Grade < x.MaxGrade)
                    {
                        // 현재 스킬 강화 가능 여부 갱신
                        upgradeAble[count] = true;
                        // 강화 정보 로딩
                        string gradeEffect = x.GradeEffect[x.Grade];

                        // "그레이드 단계(초기 단계 0 고정),
                        // 그레이드 이펙트(발동조건/발동효과),
                        // 그레이드 이펙트에 대한 수치,
                        // 효과에 대한 Description 텍스트"
                        string[] gradeEffectList = gradeEffect.Split(',');

                        // 배열 해체
                        string thisGrade = gradeEffectList[0];
                        string[] thisGradeEffectSet = gradeEffectList[1].Split("/");

                        // 전체 정보
                        // 발동 조건
                        string thisGradeCondition = thisGradeEffectSet[0]; 
                        // 발동 효과
                        string thisGradeEffect = thisGradeEffectSet[1];
                        // 발동 시 위력
                        string thisGradeCoefficient = gradeEffectList[2];
                        // 발동 효과 설명
                        string thisGradeDescription = gradeEffectList[3];
                        string[] writeData = [
                            "=스킬 정보=",
                            $"현재 단계 : {x.Grade}",
                            $"   공격력 : {x.AttackPoint}",
                            $"  코인 수 : {x.Coin}개 ",
                            $"코인 위력 : +{x.CoinPoint}",
                            $"- 사용 시 효과 -"
                        ];
                        string[] writeDesc = x.Description.Split("/");
                        
                        int gradeCost = SkillUpgradeCost[count + 1] * int.Parse(thisGrade);
                        string[] writeData2 = [
                            "→",
                            "=변경 사항=",
                            $"{thisGrade}단계로 강화",
                            $"{thisGradeDescription}",
                            $"",
                            $"강화 비용 : {gradeCost}"
                        ];
                        
                        ConsoleUI.WriteBox(
                            writeData.Concat(writeDesc).Concat(writeData2), 
                            $"{x.Name}", ConsoleColor.DarkCyan);
                    }
                    // 현재 스킬이 최대 단계일 경우
                    else
                    {
                        // 현재 스킬 강화 가능 여부 갱신
                        upgradeAble[count] = false;

                        // 강화 불가
                        string[] writeData = [
                            "=스킬 정보=",
                            $"현재 단계 : {x.Grade}",
                            $"   공격력 : {x.AttackPoint}",
                            $"  코인 수 : {x.Coin}개 ",
                            $"코인 위력 : +{x.CoinPoint}",
                            $"- 사용 시 효과 -"
];
                        string[] writeDesc = x.Description.Split("/");

                        string[] writeData2 = [
                            $"→",
                            "이미 최대 단계입니다."
                        ];

                        ConsoleUI.WriteBox(writeData.Concat(writeDesc).Concat(writeData2), 
                            $"{x.Name}", ConsoleColor.DarkCyan);
                    }
                    count++;
                });



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

            ConsoleUI.WriteLog(context.Logs);
        }


        public override void HandleInput(GameContext context)
        {
            int choice = position == 0 ? 
                ConsoleUI.ReadMenuChoice(Menu) : 
                position == 1 ? ConsoleUI.ReadMenuChoice(Menu2) 
                : ConsoleUI.ReadMenuChoice(Menu3);
            if (choice > 0 && position == 1 && upgradeAble.Length >= choice)
            {
                if (!upgradeAble[choice - 1])
                {
                    context.AddLog("이 스킬은 더 이상 강화할 수 없습니다.");
                    return;
                }

                string text = ConsoleUI.ReadString("정말 강화하시겠습니까? [Y(y)]");

                if (text != null && text.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (SkillUpgradeCost.TryGetValue(choice, out int val) &&
                        nowSkillData.Count < choice)
                    {
                        context.AddLog("입력이 잘못되었습니다.");
                        return;
                    }

                    if(val * (nowSkillData[choice - 1].Grade + 1) <= GameManager.Instance.Context.NowMoney)
                        nowSkillData[choice - 1].Grade++;
                    else
                    {
                        context.AddLog("금액이 부족합니다.");
                        return;
                    }
                        
                }
                else
                {
                    context.AddLog("취소되었습니다.");
                    return;
                }

                int cost = SkillUpgradeCost[choice];

                SkillManager.UpgradeSkill(cost, nowSkillData[choice - 1]);
                context.AddLog($"{cost}골드를 소모하여 [{nowSkillData[choice - 1].Name}] 스킬 업그레이드");
                return;
            }

            // 스킬 강화시 조건없는 강화의 경우 SkillEffect에 추가하지 않고 영구적으로 수치가 상승
            // 이외의 효과는 skillEffect에 배열 형태로 재정리 하여 추가

            
            switch (choice)
            {
                case 1:
                    if (position == 0)
                    {
                        context.AddLog("스킬 강화 시작");
                        position = 1;
                    }
                    else if (position == 1)
                    {

                    }
                    break;
                case 2:
                    if (position == 0) position = 2;
                    else if (position == 1)
                    {

                    }
                    break;
                case 3:
                    if (position == 1)
                    {

                    }
                    break;
                case 0:
                    if (position != 0)
                    {
                        position = 0;
                        return;
                    }
                    else GoTo(context, SceneKey.HomeTown);
                    break;
            }
        }
    }
}
