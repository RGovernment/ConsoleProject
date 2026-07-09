using ConsoleGameFramework.Data;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Constants;
namespace ConsoleGameFramework.Models;

public class Boss : Enemy
{
    public string[] BossPassive { get; set; }

    public Boss(string id, string name, int maxHp, string[] data, List<Skill> SkillList) 
        : base(id, name, maxHp, SkillList)
    {
        BossPassive = data;
    }


    /// <summary>
    /// 턴 시작시 발동하는 보스 패시브 (+구현 요)
    /// </summary>
    public void TurnStartPassiveEffect()
    {
        for (int i = 0; i < BossPassive.Length; i++) {
            string[] passiveData = BossPassive;

            if (passiveData[0] == TURN_START)
            {
            }
        }
         
    }


    /// <summary>
    /// 상시 발동하는 보스 패시브 (+구현 요)
    /// </summary>
    public void AlwaysPassiveEffect()
    {
        for (int i = 0; i < BossPassive.Length; i++)
        {
            string[] passiveData = BossPassive;

            if (passiveData[0] == ALWAYS)
            {
            }
        }
    }

    /// <summary>
    /// 턴 종료시 발동하는 보스 패시브 발동
    /// </summary>
    public void TurnEndPassiveEffect(Player player, List<Enemy> enemys)
    {
        for (int i = 0; i < BossPassive.Length; i++)
        {
            string[] passiveData = BossPassive;

            if (passiveData[0] == TURN_END)
            {
                if (passiveData[1] == FIELD)
                {
                    if (passiveData[2] == BURN)
                    {
                        int coeffi = Convert.ToInt32(passiveData[3]);
                        int duration = Convert.ToInt32(passiveData[3]);
                        Debuff data = new("화상",BURN,coeffi,
                            "매 턴이 끝날 때 수치만큼의 피해를 입고 수치가 절반 줄어든다.", duration
                            );
                        string[] debuffData =
                            [FORCE, passiveData[2], passiveData[3], passiveData[4] ];
                        player.TakeDebuff(FORCE, debuffData);
                        enemys.ForEach(x => { if(x is not Boss) x.TakeDebuff(FORCE, debuffData); });
                    }
                }
            }
        }
    }
}
