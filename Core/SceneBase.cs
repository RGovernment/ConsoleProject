namespace ConsoleGameFramework.Core;

/// <summary>
/// Scene 구현을 조금 더 편하게 만들기 위한 추상 클래스입니다.
/// Enter/Exit은 기본 동작이 없어도 되므로 빈 메서드로 제공합니다.
/// </summary>
public abstract class SceneBase : IScene
{
    public abstract SceneKey Key { get; }

    public virtual void Enter(GameContext context)
    {
        // 필요할 때만 자식 Scene에서 override 합니다.
    }


    public abstract void Render(GameContext context);

    public abstract void HandleInput(GameContext context);

    public virtual void Exit(GameContext context)
    {
        // 필요할 때만 자식 Scene에서 override 합니다.
    }

    /// <summary>
    /// 자주 쓰는 화면 이동 코드를 짧게 쓰기 위한 도우미입니다.
    /// </summary>
    protected static void GoTo(GameContext context, SceneKey nextScene)
    {
        context.Game.ChangeScene(nextScene);
    }
}
