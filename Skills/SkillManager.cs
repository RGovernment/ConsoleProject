using ConsoleGameFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Constants;

namespace ConsoleGameFramework.Skills;

public static class SkillManager// : MonoBehaviour
{
    public static bool UpgradeSkill(int cost, Skill skillData)
    {
        // 강화 정보 로딩
        if(skillData.GradeEffect.Length == 0) return false;

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


        //"1,coin/coinPoint,2,코인 위력 1 증가", "2,atk/atkPoint,3,공격력 3 증가"
        //1,atk/atkPoint,1,공격력 2 증가", "2,coin/coinPoint,1,코인 위력 1 증가
        //1,givenHit/burn,2,적중 시 화상 2 부여", "2,coin/coinPoint,1,코인 위력 1 증가

        // 발동 조건 검사
        switch (thisGradeCondition)
        {
            //코인 관련
            case COIN :
                //코인 위력 증감 
                if (thisGradeEffect == COIN_POINT)
                {
                    skillData.CoinPoint += Convert.ToInt32(thisGradeCoefficient);
                }
                // else
                break;
            // 공격력 관련
            case ATK:
                // 공격력 증감
                if (thisGradeEffect == ATK_POINT)
                {
                    skillData.AttackPoint += Convert.ToInt32(thisGradeCoefficient);
                }
                // else
            break;
            // 타격 관련
            case GIVEN_HIT:
                //SkillEffect 구조 { "haveBurn", "cointPoint", "2" }
                // 화상 부여
                if (thisGradeEffect == BURN)
                {
                    skillData.SkillEffect.Add([GIVEN_HIT, BURN, thisGradeCoefficient]);
                    skillData.Description += $"/{thisGradeDescription}";
                }
                break;
            // 피격 관련
            case TAKEN_HIT:
                break;
            default:break;
        }

        GameManager.Instance.Context.NowMoney -= cost * skillData.Grade;
        
        return true;
    }
}
