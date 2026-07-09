using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleGameFramework.Common.Constants;

namespace ConsoleGameFramework.Models;

public class EffectData
{
    public static Dictionary<string, Dictionary<string, string>> EffectList = new()
    {
        [BURN] = new() { 
            [NAME] = "화상",
            [DESCRIPTION] = "매 턴이 끝날 때 수치만큼의 피해를 입고 수치가 절반 줄어든다."
        }
        //...
    };

}
