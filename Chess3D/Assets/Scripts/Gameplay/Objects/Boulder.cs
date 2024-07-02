using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : GameplayObject
{
    
    public override void MoveAnim(Vector3 posIndexToMove, float speed, bool isRoundInteger = false) 
        //posIndexToMove: ở đây không phải vị trí cuối cùng, mà chỉ là vị trí mà chessman đẩy đến (sát bên chessman),
        //sẽ phải tự tính toàn tiếp vị trí tiếp theo nếu dưới chân nó là NONE (thì sẽ rơi) hoặc SLOPE (thì sẽ lăn) với tốc độ defaultSpeed. 
        //speed: là speed của chessMan đẩy boulder, boulder sẽ di chuyển cùng tốc độ với chessman đẩy box,
        //khi nào đẩy đến rơi xuống vực hoặc lăn trên dốc mới di chuyển với tốc độ defaultSpeed;
    {
        //todo anim
    }
}
