
namespace GDC.Enums
{
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
   
    public enum SceneType
    {
        UNKNOWN = -999,
        MAINMENU = 0,
        GAMEPLAY,
        
    }

    public enum ChessManType
    {
        PAWN,
        CASTLE,
        KNIGHT,
        BISHOP,
        QUEEN,
        KING,
    }

    public enum TileType
    {
        NONE, //Ko co gi
        GROUND, //Dung duoc
        OBJECT, //Khong di qua duoc va khong dung duoc
        WATER, //Nuoc
        BOX, //Box
        BOULDER, //Boulder
        SLOPE_0, //Slope rotate 0 do
        SLOPE_90, //Slope rotate 90 do
        SLOPE_180,
        SLOPE_270,
        ENEMY_CHESS,
        PLAYER_CHESS,
    }

    public enum Language
    {
        English,
        Vietnamese,
    }
    
}
