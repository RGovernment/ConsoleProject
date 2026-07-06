using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;
using static ConsoleGameFramework.Common.Enums;
namespace ConsoleGameFramework.Scenes;

/// <summary>
/// ConsoleUI가 제공하는 출력 기능(상태바, 표, 로그)을 한 화면에서 확인해볼 수 있는 샘플입니다.
/// 실습에서는 이 화면을 참고해서 여러분의 실제 게임 화면(전투, 상점 등)으로 바꿔 쓰면 됩니다.
/// </summary>
public class SampleScene : SceneBase
{
    private const int CounterMax = 10;

    private static readonly List<MenuOption> Menu = new List<MenuOption>
    {
        new MenuOption(1, "카운터 증가", "카운터를 1 올리고 로그를 남깁니다."),
        new MenuOption(2, "카운터 초기화", "카운터를 0으로 되돌립니다."),
        new MenuOption(9, "타이틀로", "첫 화면으로 돌아갑니다."),
        new MenuOption(0, "종료", "프로그램을 종료합니다.")
    };

    private int _counter;

    public override SceneKey Key => SceneKey.Sample;

    private static readonly string[] lines = new[]
        {
            "이 화면은 상태바, 표, 로그 출력 예시를 보여줍니다.",
            "WriteStatusBar, WriteTable, WriteLog를 어떻게 쓰는지 참고하세요."
        };

    public override void Enter(GameContext context)
    {
        context.AddLog("샘플 화면에 들어왔습니다.");
    }

    public override void Render(GameContext context)
    {
        ConsoleUI.Clear();
        ConsoleUI.WriteTitle("샘플 화면", "ConsoleUI 기능 미리보기");
        List<string> header =
        [
            "이건","테스트","표입니다."
        ];
        List<List<string>> test =
        [
            ["아래의", "텍스트는", "테스트"],
            ["아래의", "이미지는", "테스트2"],
            ["아래의", "숫자는", "스트3"],
            ["아래의", "게이지는", "테스트4"]
        ];
        ConsoleUI.WriteTable(header, test);
        ConsoleUI.WriteBox(lines, "안내", ConsoleColor.DarkCyan);
        
        // 상태바 예시: 값 / 최댓값 형태의 진행도를 보여줄 때 사용
        ConsoleUI.WriteStatusBar("카운터", _counter, CounterMax, fillColor: ConsoleColor.Yellow);

        // 표 예시: 여러 항목을 행/열로 정리해서 보여줄 때 사용
        ConsoleUI.WriteTable(
            headers: ["항목", "값"],
            rows:
            [
                ["카운터", _counter.ToString()],
                ["최댓값", CounterMax.ToString()]
            ]
        );


        ConsoleUI.WriteMenu(Menu, "행동 선택");
        ConsoleUI.WriteLog(context.Logs);
    }

    public override void HandleInput(GameContext context)
    {
        int choice = ConsoleUI.ReadMenuChoice(Menu);

        switch (choice)
        {
            case 1:
                _counter = Math.Min(CounterMax, _counter + 1);
                context.AddLog($"카운터 증가: {_counter}");
                break;

            case 2:
                _counter = 0;
                context.AddLog("카운터를 초기화했습니다.");
                break;

            case 9:
                GoTo(context, SceneKey.Title);
                break;

            case 0:
                context.Game.RequestQuit();
                break;
        }
    }
}
