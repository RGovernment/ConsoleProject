using ConsoleGameFramework.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Constants;

namespace ConsoleGameFramework.Data;

public static class SkillData
{
    // GRADE_EFFECT 구조
    // "그레이드 단계(초기 단계 0 고정),그레이드 이펙트(발동조건/발동효과),그레이드 이펙트에 대한 수치,효과에 대한 Description 텍스트"
    // SKILL_EFFECT 구조
    // "발동 조건 / 스킬 효과, 효과 위력"

    public static Dictionary<string, object> Thrust = new()
    {
        [ID] = "thrust",
        [NAME] = "찌르기",
        [MAX_GRADE] = "2",
        [GRADE_EFFECT] = new string[2] { "1,givenHit/burn,2,적중 시 화상 2 부여", "2,coin/coinPoint,1,코인 위력 1 증가" },
        [COIN] = 1,
        [ATK_POINT] = 5,
        [COIN_POINT] = 6,
        [DESCRIPTION] = "적에게 화상이 있을 경우 피해량 30% 증가",
        [SKILL_EFFECT] = 
        new List<string[]> { 
            new [] { "haveBurn", "damage", "30" , "0"} 
        }
    };
    public static Dictionary<string, object> DoubleThrust = new()
    {
        [ID] = "doubleThrust",
        [NAME] = "연속 찌르기",
        [MAX_GRADE] = "2",
        [GRADE_EFFECT] = new string[2] { "1,atk/atkPoint,2,공격력 2 증가", "2,coin/coinPoint,1,코인 위력 1 증가" },
        [COIN] = 2,
        [ATK_POINT] = 6,
        [COIN_POINT] = 3,
        [DESCRIPTION] = "적중 시 적에게 화상 3 부여/적에게 화상이 있을 경우 코인 위력 1 증가",
        [SKILL_EFFECT] = 
        new List<string[]> { 
            new [] { "givenHit", "burn", "3", "-1" },
            new [] {"haveBurn", "coinPoint", "1", "0" }
        }
    };
    public static Dictionary<string, object> SilverPiercing = new ()
    {
        [ID] = "silverPiercing",
        [NAME] = "은빛 섬광",
        [MAX_GRADE] = "2",
        [GRADE_EFFECT] = new string[2] { "1,coin/coinPoint,2,코인 위력 1 증가", "2,atk/atkPoint,3,공격력 3 증가" },
        [COIN] = 4,
        [ATK_POINT] = 7,
        [COIN_POINT] = 2,
        [DESCRIPTION] = "적에게 화상이 있을 경우 코인 위력 2 증가/사용 시 2턴 동안 사용하는 스킬의 공격력 3 증가",
        [SKILL_EFFECT] = 
        new List<string[]> {
            new[] { "haveBurn", "coinPoint", "2", "0" },
            new[] { "use", "atkPoint", "3" , "2"}
        }
    };

    public static Dictionary<string, object> EnemyAttack1 = new()
    {
        [ID] = "enemy1",
        [NAME] = "밀치기",
        [MAX_GRADE] = "0",
        [GRADE_EFFECT] = Array.Empty<string>(),
        [COIN] = 1,
        [ATK_POINT] = 3,
        [COIN_POINT] = 4,
        [DESCRIPTION] = "없음",
        [SKILL_EFFECT] = new List<string[]> {}
    };

    public static Dictionary<string, object> EnemyAttack2 = new()
    {
        [ID] = "enemy2",
        [NAME] = "베기",
        [MAX_GRADE] = "0",
        [GRADE_EFFECT] = Array.Empty<string>(),
        [COIN] = 2,
        [ATK_POINT] = 4,
        [COIN_POINT] = 3,
        [DESCRIPTION] = "없음",
        [SKILL_EFFECT] = new List<string[]> { }
    };

    public static Dictionary<string, object> EnemyAttack3 = new()
    {
        [ID] = "enemy3",
        [NAME] = "후려치기",
        [MAX_GRADE] = "0",
        [GRADE_EFFECT] = Array.Empty<string>(),
        [COIN] = 3,
        [ATK_POINT] = 3,
        [COIN_POINT] = 4,
        [DESCRIPTION] = "없음",
        [SKILL_EFFECT] = new List<string[]> { }
    };

    public static List<Dictionary<string, object>> PlayerSkills = new()
    {
        Thrust,
        DoubleThrust,
        SilverPiercing
    };
    public static List<Dictionary<string, object>> EnemySkills = new()
    {
        EnemyAttack1,
        EnemyAttack2,
        EnemyAttack3
    };
}
