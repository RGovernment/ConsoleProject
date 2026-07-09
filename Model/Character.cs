using ConsoleGameFramework.Common;
using ConsoleGameFramework.Core;
using ConsoleGameFramework.Skills;
using System.Xml.Linq;
using static ConsoleGameFramework.Common.Constants;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleGameFramework.Models;


public class Character
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int MaxHp { get; private set; }
    public int Hp { get; private set; }
    public int Sanity { get; private set; }
    public List<Skill> SkillList { get; }
    public List<string> ItemKeyList { get; private set; }
    public List<Buff> BuffList { get; private set; }
    public List<Debuff> DebuffList { get; private set; }

    public Queue<Skill> SkillQueue { get; private set; }
    private Queue<Skill> NextQueue;
    public bool IsAlive => Hp > 0;

    public event Action OnDead;
    public event Action OnClashWin;

    public Character(Character character)
    {
        Name = character.Name;
        MaxHp = character.MaxHp;
        Hp = character.Hp;
        SkillList = new(character.SkillList);
        Sanity = Math.Clamp(character.Sanity, -45, 45);
        ItemKeyList = new(character.ItemKeyList);
        BuffList = new();
        DebuffList = new();
        SkillQueue = new();
        NextQueue = new();
    }
    public Character(string id, string name, int maxHp, List<Skill> list, int sanity = 0)
    {
        Id = id;
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        SkillList = list;
        Sanity = sanity;
        ItemKeyList = new();
        BuffList = new();
        DebuffList = new();
        SkillQueue = new();
        NextQueue = new();
    }

    public void TakeDamage(int damage)
    {
        Hp = Math.Max(0, Hp - damage);

        if (Hp <= 0) OnDead?.Invoke();
    }

    public void TakeHeal(int heal)
    {
        Hp = Math.Min(MaxHp, Hp + heal);
    }

    //같은 버프 처리 안함
    public void TakeBuff(string condition, List<string[]> data)
    {
        data.ForEach(x => {
            TakeBuff(condition, x);
        });
    }

    public void TakeBuff(string condition, string[] data)
    {
        if ((data[0] == condition || data[0] == FORCE) && data[1] == ATK_POINT)
        {
            Buff? data2 = BuffList.FirstOrDefault(x => x.Id == ATK_POINT);

            if (data2 != null)
            {
                //같은 종류의 버프지만 효과가 더 좋은 경우
                if (data2.Coefficient < Convert.ToInt32(data[2]))
                {
                    // 지속, 효과 갱신
                    data2.Coefficient = Convert.ToInt32(data[2]);
                    data2.Duration = Convert.ToInt32(data[3]);
                }
                // 같은 종류의 버프이며 효과가 같지만 지속시간이 더 긴 경우
                else if (data2.Coefficient == Convert.ToInt32(data[2]) &&
                data2.Duration < Convert.ToInt32(data[3]))
                    // 지속 시간 갱신
                    data2.Duration = Convert.ToInt32(data[3]);
                // 같은 버프를 더 강한 효과로 가지고 있거나
                // 효과는 같지만 이미 걸려있는 효과가 지속시간이 더 긴 경우 무시
                // 단 연장형 효과는 추가시 예외 처리
            }
            else
            {
                BuffList.Add(new Buff("공격력 증가", ATK_POINT,
                Convert.ToInt32(data[2]),
                "공격력이 증가한다.",
                Convert.ToInt32(data[3]))
                );
            }
        }
    }

    public void TakeDebuff(string condition,List<string[]> data)
    {
        data.ForEach(x =>
        {
            TakeDebuff(condition, x);
        });

    }

    public void TakeDebuff(string condition, string[] data)
    {

        if ((data[0] == condition || data[0] == FORCE) && data[1] == BURN)
        {
            Debuff? data2 = DebuffList.FirstOrDefault(x => x.Id == BURN);
            if (data2 != null)
            {
                //같은 종류의 디버프지만 효과가 더 좋은 경우
                if (data2.Coefficient < Convert.ToInt32(data[2]))
                {
                    // 지속, 효과 갱신
                    data2.Coefficient = Convert.ToInt32(data[2]);
                    data2.Duration = Convert.ToInt32(data[3]);
                }
                // 같은 종류의 디버프이며 효과가 같지만 지속시간이 더 긴 경우
                else if (data2.Coefficient == Convert.ToInt32(data[2]) &&
                data2.Duration < Convert.ToInt32(data[3]))
                    // 지속 시간 갱신
                    data2.Duration = Convert.ToInt32(data[3]);
                // 같은 디버프를 더 강한 효과로 가지고 있거나
                // 효과는 같지만 이미 걸려있는 효과가 지속시간이 더 긴 경우 무시
                // 단 연장형 효과는 추가시 예외 처리
            }
            else
            {
                DebuffList.Add(
                            new Debuff("화상", BURN,
                            Convert.ToInt32(data[2]),
                            "매 턴이 끝날 때 수치만큼의 피해를 입고 수치가 절반 줄어든다.",
                            Convert.ToInt32(data[3]))
                        );
            }
        }
        //else if...

    }

    public void CommonBuffAndDebuffDurationDiscount()
    {
        List<int> removeList = new();

        for (int i = BuffList.Count - 1; i >= 0; i--)
        {
            Buff list = BuffList[i];

            if (list.Duration > 0)
            {
                list.Duration--;
                if (list.Duration == 0)
                    BuffList.RemoveAt(i);
            }
        }

        for (int i = DebuffList.Count - 1; i >= 0; i--)
        {
            Debuff list = DebuffList[i];

            if (list.Duration > 0)
            {
                list.Duration--;
                if (list.Duration == 0)
                    DebuffList.RemoveAt(i);
            }
        }
    }

    public void TakeDotDamage()
    {
        for(int i = DebuffList.Count -1; i >= 0; i--)
        {
            Debuff x = DebuffList[i];
            if (x.Id == BURN)
            {
                TakeDamage(x.Coefficient);
                GameManager.Instance.Context.AddLog($"{Name}이(가) 화상 피해 {x.Coefficient} 입음");
                x.Coefficient /= 2;
                if (x.Coefficient == 0) DebuffList.RemoveAt(i);
            }
        }
    }

    // 차후 고도화가 가능하면 변수로
    // 적은 최대 정신력 20으로 제한(억까 방지)
    public void GainSanity(int gain)
    {
        Sanity = Math.Clamp(Sanity + gain, -45, this is Enemy ? 20 : 45);
        GameManager.Instance.Context.AddLog($"{Name}이(가) 정신력 {gain} 회복");
    }

    public void MakeSkillQueue()
    {
        if (SkillList.Count < 3 || SkillQueue.Count != 0) return;

        if (SkillQueue.Count == 0)
        {
            List<Skill> skill = new()
            {
                SkillList[0],SkillList[0],SkillList[0],SkillList[1],SkillList[1],SkillList[2]
            };

            skill.Shuffle();
            skill.ForEach(x => SkillQueue.Enqueue(x));

            skill.Shuffle();
            skill.ForEach(x => NextQueue.Enqueue(x));
        }
    }

    /// <summary>
    /// skill을 섞고 재배치한 뒤 큐(뽑는 큐, 대기 큐 2개)에 넣고 순서대로 스킬 출력
    /// </summary>
    /// <returns></returns>
    public Skill GetSkillQueue()
    {
        if (SkillList.Count < 3) return null;

        if (SkillQueue.Count == 0)
        {
            List<Skill> skill = new()
            {
                SkillList[0],SkillList[0],SkillList[0],SkillList[1],SkillList[1],SkillList[2]
            };

            skill.Shuffle();
            skill.ForEach(x => SkillQueue.Enqueue(x));

            skill.Shuffle();
            skill.ForEach(x => NextQueue.Enqueue(x));

            Skill result = SkillQueue.Dequeue();

            SkillQueue.Enqueue(NextQueue.Dequeue());

            return result;
        }
        else if(NextQueue.Count == 0)
        {
            List<Skill> skill = new()
            {
                SkillList[0],SkillList[0],SkillList[0],SkillList[1],SkillList[1],SkillList[2]
            };

            skill.Shuffle();
            skill.ForEach(x => NextQueue.Enqueue(x));

            Skill result = SkillQueue.Dequeue();

            SkillQueue.Enqueue(NextQueue.Dequeue());

            return result;
        }
        else
        {
            Skill result = SkillQueue.Dequeue();

            SkillQueue.Enqueue(NextQueue.Dequeue());

            return result;
        }
    }

    public void ClearSkillQueue()
    {
        SkillQueue.Clear();
        NextQueue.Clear();
    }

    public void EffectReset()
    {
        BuffList.Clear();
        DebuffList.Clear();
    }
}
