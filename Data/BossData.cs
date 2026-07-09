using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Data;

public static class BossData
{
    // 구성
    // 보스 아이디 = [효과 범위/효과/효과 위력/효과 지속시간(-1은 영구지속)]
    // field : 전체(아군 포함)
    static Dictionary<string, string[]> bossData = new()
    {
        ["30000"] = ["field/turnEndBurn/2/-1"]
    };

}
