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
    private readonly static Dictionary<string, object> round1 = new()
    {
        ["dummy"] = new Dictionary<string, object>()
        {
            [ID] = "20000",
            [NAME] = "잡몹1",
            [MAX_HP] = 30
        },
        ["dummy2"] = new Dictionary<string, object>()
        {
            [ID] = "20001",
            [NAME] = "잡몹2",
            [MAX_HP] = 35
        }
    };

    // 스테이지 1 라운드 2
    private readonly static Dictionary<string, object> round2 = new()
    {
        ["dummy"] = new Dictionary<string, object>()
        {
            [ID] = "20000",
            [NAME] = "잡몹1",
            [MAX_HP] = 30
        },
        ["dummy2"] = new Dictionary<string, object>()
        {
            [ID] = "20001",
            [NAME] = "잡몹2",
            [MAX_HP] = 35
        },
        ["dummy3"] = new Dictionary<string, object>()
        {
            [ID] = "20002",
            [NAME] = "잡몹3",
            [MAX_HP] = 25
        }
    };

    // 보스 스테이지
    private readonly static Dictionary<string, object> bossRound1 = new()
    {
        ["boss1"] = new Dictionary<string, object>()
        {
            [ID] = "30000",
            [NAME] = "재앙의 전조",
            [MAX_HP] = 150,
            [IS_BOSS] = true
        },
        ["dummy1"] = new Dictionary<string, object>()
        {
            [ID] = "20000",
            [NAME] = "잡몹1",
            [MAX_HP] = 15
        },
        ["dummy2"] = new Dictionary<string, object>()
        {
            [ID] = "20001",
            [NAME] = "잡몹2",
            [MAX_HP] = 18
        },
        ["dummy3"] = new Dictionary<string, object>()
        {
            [ID] = "20002",
            [NAME] = "잡몹3",
            [MAX_HP] = 13
        }

    };


    // 현재 라운드가 총 라운드수를 넘어서면 보스 라운드 돌입
    public readonly static List<Dictionary<string, object>> RoundList = new()
    {
        round1, round2
    };

    // 모든 보스 라운드(스테이지 당 1번만 존재)
    public readonly static List<Dictionary<string, object>> BossRoundList = new()
    {
        bossRound1
    };

    // 모든 스테이지 적 목록
    public readonly static List<List<Dictionary<string, object>>> StageRoundList = new()
    {
        RoundList
    };
}
