
using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;
using static ConsoleGameFramework.Common.Enums;
using System.Diagnostics.Metrics;
using System.Text;

namespace ConsoleGameFramework.Scenes
{
    public class LoadingScene : SceneBase
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

        public async override void Render(GameContext context)
        {
            ConsoleUI.Clear();
            int type = 1;

            int count = 0;
            StringBuilder sb = new();
            sb.Append("로딩중");
            while (count < 20) {
                
                if (type > 4) { 
                    type = 1; 
                    sb.Clear();
                    sb.Append("로딩중");
                }
                else type++;
                sb.Append('.');
                ConsoleUI.WriteLoad(type, fifth:sb.ToString());
                GameManager.Instance.Context.NowWallType = type;
                await Task.Delay(Random.Shared.Next(100, 150));
                count++;
            }
            GoTo(context, context.LodingTarget);
        }

        public override void HandleInput(GameContext context)
        {
             
        }

    }
}
