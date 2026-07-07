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
    // 참조할 스킬 데이터
    public Dictionary<string,object> SkillData { get; set; }
    // ID
    public string? Id => SkillData.TryGetValue(ID, out object? value) ? value.ToString() : "";
    // 이름
    public string? Name => SkillData.TryGetValue(NAME, out object? value) ? value.ToString() : "";

    //최대 강화 단계
    public int MaxGrade => SkillData.TryGetValue(MAX_GRADE, out object? value) ? Convert.ToInt32(value) : 0;
    // 현재 강화 단계
    public int Grade { get; set; } = 0;

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
    // 코인 값
    public int Coin {
        get => SkillData.TryGetValue(COIN, out object? value) && value is int coin ? coin : 0;
        set => SkillData[COIN] = value;
    }
    // 공격력
    public int AttackPoint
    {
        get => SkillData.TryGetValue(ATK_POINT, out object? value) && value is int atkPoint ? atkPoint : 1;
        set => SkillData[ATK_POINT] = value;
    }
    //코인 위력
    public int CoinPoint
    {
        get => SkillData.TryGetValue(COIN_POINT, out object? value) && value is int coinPoint ? coinPoint : 1;
        set => SkillData[COIN_POINT] = value;
    }

    // 설명
    public string? Description 
    {
        get => SkillData.TryGetValue(DESCRIPTION, out object? value) && value is string desc ? desc : "";
        set => SkillData[DESCRIPTION] = value ?? "";
    }

    // 해당 스킬의 효과
    public List<string[]> SkillEffect { get; private set; }

    public Skill(Dictionary<string, object> skillData, int grade)
    {
        SkillData = skillData;
        Grade = grade;
        if (skillData[SKILL_EFFECT] is List<string[]> data) SkillEffect = data;
        else SkillEffect = new();
    }
}
