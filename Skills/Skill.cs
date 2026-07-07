using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Constants;

namespace ConsoleGameFramework.Skills;

public class Skill
{
    public Dictionary<string,object> SkillData { get; set; }
    public string? Id => SkillData.TryGetValue(ID, out object? value) ? value.ToString() : "";
    public string? Name => SkillData.TryGetValue(NAME, out object? value) ? value.ToString() : "";
    public int MaxGrade => SkillData.TryGetValue(MAX_GRADE, out object? value) ? Convert.ToInt32(value) : 0;
    public int Grade { get; set; }

    // GradeEffect는 스플릿으로 사용할 것(JObject 대체)
    // 구조 "그레이드 단계,그레이드 이펙트,그레이드 이펙트에 대한 수치,효과에 대한 Description 텍스트"
    public string[]? GradeEffect
    {
        get {
            if (SkillData.TryGetValue(GRADE_EFFECT, out object? value) && value is string[] data)
            {
                return data;
            }else 
                return [];
        }
    }
    public int Coin => SkillData.TryGetValue(COIN, out object? value) ? Convert.ToInt32(value) :0;
    public int AttackPoint => SkillData.TryGetValue(ATK_POINT, out object? value) ? Convert.ToInt32(value) : 0;
    public int CoinPoint => SkillData.TryGetValue(COIN_POINT, out object? value) ? Convert.ToInt32(value) : 0;
    public string? Description => SkillData.TryGetValue(DESCRIPTION, out object? value) ? value.ToString() : "";
    public List<string[]> SkillEffect { get; private set; }

    public Skill(Dictionary<string, object> skillData, int grade)
    {
        SkillData = skillData;
        Grade = grade;
        if (skillData[SKILL_EFFECT] is List<string[]> data) SkillEffect = data;
        else SkillEffect = new();
    }
}
