using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Data;

public static class BossData
{
    // 구성
    // 보스 아이디 = [발동 시점/효과 범위/효과/효과 위력/효과 지속시간(-1은 영구지속)]
    // turnStart : 턴 시작시, always : 상시, turnEnd : 턴 종료시
    // field : 필드 전체(아군 포함)

    public readonly static Dictionary<string, string[]> bossData = new()
    {
        ["30000"] = ["turnEnd","field","burn","2","-1","턴 종료시 자신을 제외한 필드 전체의 캐릭터에게 화상 2 부여"]
    };

}
