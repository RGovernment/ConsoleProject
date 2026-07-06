using System;
using System.Text;

namespace ConsoleGameFramework.Core;


/// <summary>
/// 게임의 전체 설정 : 난이도 설정, 
/// 적 생성 개수 설정, 
/// 이름 변경 등등 게임의 설정을 바꿀 수 있는 싱글톤 클래스다.
/// </summary>
public class GameSettingManager
{
    private static GameSettingManager instance = null;

    public static GameSettingManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameSettingManager();

            return instance;
        }
    }
    private GameSettingManager()
    {

    }

}