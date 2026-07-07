using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public class Skill
{
    public Dictionary<string,object> SkillData { get; set; }
    public string? Id => SkillData.TryGetValue("id", out object? value) ? value.ToString() : "";
    public string? Name => SkillData.TryGetValue("name", out object? value) ? value.ToString() : "";
    public int Grade { get; set; }

    // GradeEffect는 스플릿으로 사용할 것(JObject 대체)
    // 구조 "그레이드 단계,그레이드 이펙트,그레이드 이펙트에 대한 수치,효과에 대한 Description 텍스트"
    public string? GradeEffect => SkillData.TryGetValue("gradeEffect", out object? value) ? value.ToString() : "";
    public int Coin => SkillData.TryGetValue("coin", out object? value) ? Convert.ToInt32(value) :0;
    public int AttackPoint => SkillData.TryGetValue("atkPoint", out object? value) ? Convert.ToInt32(value) : 0;
    public int CoinPoint => SkillData.TryGetValue("coinPoint", out object? value) ? Convert.ToInt32(value) : 0;
    public string? Description => SkillData.TryGetValue("description", out object? value) ? value.ToString() : "";
    public List<string> SkillEffect { get; set; }
    public List<EffectStatus> EffectList { get; set; }

    public Skill(Dictionary<string, object> skillData, int grade, List<string> skillEffect, List<EffectStatus> effect)
    {
        SkillData = skillData;
        Grade = grade;
        SkillEffect = new(skillEffect);
        EffectList = new(effect);
    }
}
