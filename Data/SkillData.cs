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
        [SKILL_EFFECT] = new List<string> { "damageUp/burn/damage,30" }
    };
    public static Dictionary<string, object> DoubleThrust = new()
    {
        [ID] = "doubleThrust",
        [NAME] = "연속 찌르기",
        [MAX_GRADE] = "2",
        [GRADE_EFFECT] = new string[2] { "1,givenHit/burn,1,적중 시 화상 1 부여", "2,coin/coinPoint,1,코인 위력 1 증가" },
        [COIN] = 2,
        [ATK_POINT] = 6,
        [COIN_POINT] = 3,
        [DESCRIPTION] = "1. 적중 시 적에게 화상 1 부여\n2. 적에게 화상이 있을 경우 코인 위력 1 증가",
        [SKILL_EFFECT] = new List<string> { "givenHit/burn,1", "haveBurn/cointPoint,1" }
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
        [DESCRIPTION] = "1. 적에게 화상이 있을 경우 코인 위력 2 증가\n2. 사용 시 다음 턴에 사용하는 스킬의 공격력 3 증가",
        [SKILL_EFFECT] = new List<string> { "haveBurn/cointPoint,2", "use/nextAtkPoint,3" }
    };

    public static Dictionary<string, object> EnemyAttack1 = new()
    {
        [ID] = "enemy1",
        [NAME] = "얍얍",
        [MAX_GRADE] = "0",
        [GRADE_EFFECT] = Array.Empty<string>(),
        [COIN] = 1,
        [ATK_POINT] = 3,
        [COIN_POINT] = 4,
        [DESCRIPTION] = "얍얍얍!!",
        [SKILL_EFFECT] = new List<string> {}
    };

    public static Dictionary<string, object> EnemyAttack2 = new()
    {
        [ID] = "enemy2",
        [NAME] = "얍얍2",
        [MAX_GRADE] = "0",
        [GRADE_EFFECT] = Array.Empty<string>(),
        [COIN] = 2,
        [ATK_POINT] = 4,
        [COIN_POINT] = 3,
        [DESCRIPTION] = "얍얍얍!!!",
        [SKILL_EFFECT] = new List<string> { }
    };

    public static Dictionary<string, object> EnemyAttack3 = new()
    {
        [ID] = "enemy3",
        [NAME] = "얍얍3",
        [MAX_GRADE] = "0",
        [GRADE_EFFECT] = Array.Empty<string>(),
        [COIN] = 3,
        [ATK_POINT] = 3,
        [COIN_POINT] = 4,
        [DESCRIPTION] = "얍얍얍!!!",
        [SKILL_EFFECT] = new List<string> { }
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
    /*    public Dictionary<string, object> SkillData { get; set; }
        public string? Id => SkillData.TryGetValue("id", out object? value) ? value.ToString() : "";
        public string? Name => SkillData.TryGetValue("name", out object? value) ? value.ToString() : "";
        public int Grade { get; set; }

        // GradeEffect는 스플릿으로 사용할 것(JObject 대체)
        // 구조 "그레이드 단계,그레이드 이펙트,그레이드 이펙트에 대한 수치,효과에 대한 Description 텍스트"
        public string? GradeEffect => SkillData.TryGetValue("gradeEffect", out object? value) ? value.ToString() : "";
        public int Coin => SkillData.TryGetValue("coin", out object? value) ? Convert.ToInt32(value) : 0;
        public int AttackPoint => SkillData.TryGetValue("atkPoint", out object? value) ? Convert.ToInt32(value) : 0;
        public int CoinPoint => SkillData.TryGetValue("coinPoint", out object? value) ? Convert.ToInt32(value) : 0;
        public string? Description => SkillData.TryGetValue("description", out object? value) ? value.ToString() : "";
        public List<string> SkillEffect { get; set; }
        public List<EffectStatus> EffectList { get; set; }*/
}
