using ConsoleGameFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework.Skills;

public class SkillManager// : MonoBehaviour
{
    public void UpgradeSkill(int cost, Skill skillData)
    {
        // 강화 정보 로딩
        string gradeEffect = skillData.GradeEffect[skillData.Grade - 1];

        // "그레이드 단계(초기 단계 0 고정),
        // 그레이드 이펙트(발동조건/발동효과),
        // 그레이드 이펙트에 대한 수치,
        // 효과에 대한 Description 텍스트"
        string[] gradeEffectList = gradeEffect.Split(',');

        // 배열 해체
        string thisGrade = gradeEffectList[0];
        string[] thisGradeEffectSet = gradeEffectList[1].Split("/");

        // 전체 정보
        // 발동 조건
        string thisGradeCondition = thisGradeEffectSet[0];
        // 발동 효과
        string thisGradeEffect = thisGradeEffectSet[1];
        // 발동 시 위력
        string thisGradeCoefficient = gradeEffectList[2];
        // 발동 효과 설명
        string thisGradeDescription = gradeEffectList[3];

        
        GameManager.Instance.Context.NowMoney -= cost * skillData.Grade;

    }
}
