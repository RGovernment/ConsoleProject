using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Constants;

namespace ConsoleGameFramework.Data;
// 스테이지 적 정보 
public class RoundData
{
    
    // 스테이지 1 라운드 1
    private static Dictionary<string, object> round1 = new()
    {
        ["dummy"] = new Dictionary<string, object>()
        {
            [NAME] = "잡몹1",
            [MAX_HP] = 100
        }

        ["dummy2"] = new Dictionary<string, object>()
        {
            [NAME] = "잡몹2",
            [MAX_HP] = 80
        }
    };

    // 스테이지 1 라운드 2
    private static Dictionary<string, object> round2 = new()
    {

    };

    // 보스 스테이지
    private static Dictionary<string, object> bossRound1 = new()
    {

    };


    // 현재 라운드가 총 라운드수를 넘어서면 보스 라운드 돌입
    public static List<Dictionary<string, object>> RoundList = new()
    {
        round1, round2
    };

    // 모든 보스 라운드(스테이지 당 1번만 존재)
    public static List<Dictionary<string, object>> BossRoundList = new()
    {
        bossRound1
    };

    // 모든 스테이지 적 목록
    public static List<List<Dictionary<string, object>>> StageRoundList = new()
    {
        RoundList
    };
}
