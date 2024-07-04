using UnityEngine;
using System;
using GDC.Enums;
using GDC.Common;

namespace GDC.Constants
{
    public class GameConstants
    {
        public const int MAX_X_SIZE = 16;
        public const int MAX_Y_SIZE = 8;
        public const int MAX_Z_SIZE = 16;
        
        public static readonly Vector2 obstacleIdBoundary = new Vector2(100, 299);
        public static readonly Vector2 playerChessIdBoundary = new Vector2(300, 305);
        public static readonly Vector2 enemyChessIdBoundary = new Vector2(400, 410);

        public const int MAX_LEVEL = 8;
        public const int MAX_CHAPTER = 4;
    }
}
