using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Config/UI/ChessHolderConfig")]
public class ChessHolderConfig : ScriptableObject
{
    public Color chessColor;
    public Color defaultBorder;
    public Color defaultBackground;
    public Color activeBorder;
    public Color activeBackground;

}
