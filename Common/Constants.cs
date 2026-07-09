namespace ConsoleGameFramework.Common;

public static class Constants
{
    public const int LOAD_TEXT_LENG = 43;
    public const int LOG_LINE_LIMIT = 10;
    public const string DEFAULT_STRING = "                                           ";
    public const string TYPE = "type";
    public static int[] X_DIRECTION = { -1, 1, 0, 0 };
    public static int[] Y_DIRECTION = { 0, 0, -1, 1 };
    public const int MIN_SANITY = -45;
    public const int MAX_Sanity = 45;
    public const int INITIAL_AMOUNT = 500;

    //통상 호출
    public const string HP = "hp";
    public const string MAX_HP = "maxHp";
    public const string NOW_HP = "nowHp";

    //스킬 호출 string
    public const string ATK = "atk";
    public const string ATK_POINT = "atkPoint";
    public const string COIN = "coin";
    public const string COIN_POINT = "coinPoint";
    public const string DAMAGE = "damage";
    public const string DESCRIPTION = "description";
    public const string GIVEN_HIT = "givenHit";
    public const string GRADE_EFFECT = "gradeEffect";
    public const string ID = "id";
    public const string MAX_GRADE = "maxGrade";
    public const string NAME = "name";
    public const string SKILL_EFFECT = "skillEffect";
    public const string TAKEN_HIT = "takenHit";

    // 조건 Id
    public const string HAVE_BURN = "haveBurn";
    public const string USE = "use";
    public const string TURN_START = "turnStart";
    public const string ALWAYS = "always";
    public const string TURN_END = "turnEnd";
    public const string IS_BOSS = "isBoss";
    public const string FIELD = "field";
    public const string FORCE = "force";

    // 버프/디버프 관련 호출
    public const string BURN = "burn";
    
}
