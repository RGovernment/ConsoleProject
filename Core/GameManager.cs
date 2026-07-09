using ConsoleGameFramework.Scenes;
using ConsoleGameFramework.UI;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleGameFramework.Core;

/// <summary>
/// 게임의 전체 흐름을 담당하는 클래스입니다.
///
/// 역할:
/// 1. Scene 등록
/// 2. 현재 Scene 실행
/// 3. Scene 전환
/// 4. 프로그램 종료 요청 처리
///
/// 참고: 이 클래스는 프로그램 전체에서 단 하나만 존재해야 하므로 Singleton 패턴으로 작성되어 있습니다.
/// (생성자를 private으로 막고, 정적 Instance 프로퍼티로만 접근을 허용)
/// </summary>
public class GameManager
{
    private readonly Dictionary<SceneKey, IScene> _scenes = new Dictionary<SceneKey, IScene>();
    private IScene? _currentScene;

    /// <summary>
    /// 프로그램 전체에서 하나만 사용하는 GameManager 인스턴스입니다.
    /// </summary>
    public static GameManager Instance { get; } = new GameManager();

    private GameManager()
    {
        Context = new GameContext(this);
        RegisterScenes();
    }

    public GameContext Context { get; }

    /// <summary>
    /// 게임에서 사용할 화면들을 등록합니다.
    /// 새 화면을 만들었다면 이곳에 AddScene(new MyScene()) 형식으로 추가합니다.
    /// </summary>
    private void RegisterScenes()
    {
        AddScene(new TitleScene());
        AddScene(new SampleScene());
        AddScene(new BattleScene());
        AddScene(new HomeTownScene());
        AddScene(new ShopScene());
        AddScene(new TrainingHallScene());
        AddScene(new LoadingScene());
    }

    private void AddScene(IScene scene)
    {
        _scenes[scene.Key] = scene;
    }

    /// <summary>
    /// 프로그램의 메인 루프입니다.
    /// 현재 Scene을 그리고(Render), 화면에 반영(Present), 입력을 처리(HandleInput)하는 과정을 반복합니다.
    /// </summary>
    public async void Run()
    {
        ChangeScene(SceneKey.Title);

        while (Context.IsRunning && _currentScene is not null)
        {
            _currentScene.Render(Context);

            ConsoleUI.Present();

            _currentScene.HandleInput(Context);
        }

        ConsoleUI.Clear();

        ConsoleUI.WriteTitle("프로그램 종료", "수고하셨습니다.");
        if (Context.IsLose)
            ConsoleUI.WriteBox(
            [
                "전투에서 패배했습니다."
            ], "The End", ConsoleColor.DarkRed);
        else
            ConsoleUI.WriteBox(
            [
                "C# 콘솔 게임 프레임워크가 종료되었습니다."
            ], "Good Bye", ConsoleColor.DarkCyan);
        ConsoleUI.Present();
    }

    /// <summary>
    /// 다른 화면으로 전환합니다.
    /// </summary>
    public void ChangeScene(SceneKey key)
    {
        if (!_scenes.TryGetValue(key, out IScene? nextScene))
        {
            Context.AddLog($"등록되지 않은 화면입니다: {key}");
            return;
        }

        Instance.Context.NowScene = key;
        Console.WriteLine(key);
        _currentScene?.Exit(Context);
        _currentScene = nextScene;
        _currentScene.Enter(Context);
    }

    /// <summary>
    /// 게임 종료를 요청합니다.
    /// </summary>
    public void RequestQuit()
    {
        Context.IsRunning = false;
    }
}
