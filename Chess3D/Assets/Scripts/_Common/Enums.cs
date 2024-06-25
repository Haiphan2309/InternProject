
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
    
}
