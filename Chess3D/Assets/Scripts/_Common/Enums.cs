
namespace GDC.Enums
{
    //public enum SceneType
    //{
    //    HOME = 0,


    //    SUNNY_VALLEY = 100,



    //    WHISPERING_FOREST = 200,



    //    FOGGY_HOUSE = 300,



    //    ENLIGHTED_FROST_MOUNTAIN = 400,



    //    RAINBOW_VOLCANO = 500,



    //    SHADOW_DESERT = 600,



    //}

    public enum LedgeType
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }
    public enum DialogueState
    {
        HEAD,
        BRANCH,
        TAIL,
    }
    public enum TransitionType
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN,
        IN,
        FADE,
    }
    public enum TransitionLoadSceneType
    {
        NEW_SCENE, //Load sang scene moi
        RELOAD_WITH_TRANSITION, //Load lai scene cu nhung van co transition
        RELOAD_WITHOUT_TRANSITION //Load lai scene cu va khong co transition
    }
    public enum IngredientType
    {
        SLIME_LIQUID = 0,



    }

    public enum ItemType
    {
        INGREDIENT = 0,

        CLOTHES = 10,

        WEAPON = 20,

        USEABLE_ITEM = 30,

        KEY_ITEM = 40,
    }

    public enum EquipmentType
    {
        NONE = 0,
        HELMET,
        ARMOR,
        WEAPON,
        SHOE,
    }
    public enum MonsterType
    {
        HOME = 0,


        SUNNY_VALLEY = 100,
        SLIME = 101,
        BIG_SLIME = 102,
        SPIKE_SLIME = 103,
        MOLD_WORM = 104,
        FROGGY = 105,


        WHISPERING_FOREST = 200,
        CROW = 201,
        PUFF_STOOL = 202,


        FOGGY_HOUSE = 300,
        


        ENLIGHTED_FROST_MOUNTAIN = 400,
        PEP = 401,
        ELECTRIC_JELLY = 402,
        PENGUIN = 403,


        RAINBOW_VOLCANO = 500,
        FIREMON = 501,
        BIG_FIREMON = 502,
        HOT_LIPS =503,



        SHADOW_DESERT = 600,
        CACTUS_MAN = 601,
        FLEEPER = 602,


    }

    public enum WeaponType
    {
        SWORD = 0,
        BOW = 1,
    }

    public enum SpellType
    {
        //Phai dung thu tu
        NONE = 0,
        SPIKE_DART,
        POISON_BOMB,
        FIRE_BREATH,
        ICE_SPIT,
        ENERGY_BALL,
        THUNDER_SHOCK,
        SPIN_ATTACK,
        HYPNOSIS,
        HYPER_BEAM,    
        TELEPORT,
        ATK_BUFF,
        DEF_BUFF,     
        SPE_BUFF,
    }

    public enum PetType
    {
        NONE,
        BUFF,
        ATTACK,
        MIX,
    }

    public enum PetID
    {
        NONE,
        BLUE_FOLEY,
        GREEN_FOLEY,
        PINK_FOLEY,
        RED_FOLEY,
        YELLOW_FOLEY,
    }

    public enum EggType
    {

    }

    public enum EffectType
    {
        NORMAL = 0,
        ATK_BUFF,
        DEF_BUFF,
        SPE_BUFF,
        DIZZY,
        FREEZE,
        POISON,
        SLEEP,
        SLOW,
    }
    public enum ePlayerEffectStatus
    {
        Attack_Buff = 0,
        Defense_Buff,
        Dizzy,
        Freeze,
        Poison,
        Sleep,
        Slow, 
        Speed_Buff,
        Heal
    }


    public enum PointType
    {
        HAPPINESS = 1,
        DECORATION = 2
    }
    public enum ChestType
    {
        Wood = 0,
        Gold = 1,
        Diamond = 2,
    }
    public enum RarityLevel
    {
        COMMON = 0,
        RARE = 1,
        EPIC = 2,
        LEGENDARY = 3
    }
    public enum HomePanelType
    {
        NONE = -1,
        SHOP = 0,
        INVENTORY = 1,
        GACHA = 2,
        MAP = 3,
        SETTING = 4,
        DATE_TIME_NOTIFICATION = 5
    }
    public enum SceneType
    {
        UNKNOWN = -999,
        MAIN = 0,
        HOME = 1,
        TUTORIAL = 2,
        INTRO = 3,
        __________REAL_WORLD_AREA__________ = 50,
        REAL_WORLD_AREA_01,
        __________BEGIN_AREA__________ = 100,
        BEGIN_AREA_1,
        BEGIN_AREA_2,
        BEGIN_AREA_3,
        BEGIN_AREA_4,
        BEGIN_AREA_5,
        BEGIN_AREA_6,
        BEGIN_AREA_7,
        BEGIN_AREA_8,
        BEGIN_AREA_HOUSE,
        __________AREA01__________ = 150,
        AREA1_1,
        AREA1_2,
        AREA1_3,
        AREA1_4,
        AREA1_5,
        AREA1_DUNGEON,
        AREA1_SUNFLOWER,
        AREA1_BONUS,
    }
    public enum AreaType
    {
        REAL_WORLD_AREA,
        BEGIN_AREA,
        AREA_1,
        AREA_2,
        AREA_3,
        AREA_4,
        AREA_5,
    }
    public enum TimeOfDayType
    {
        DAWN = 0,
        DAY = 1,
        SUNSET = 2,
        NIGHT = 3
    }

    
}
