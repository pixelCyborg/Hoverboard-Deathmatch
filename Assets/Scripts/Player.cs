using UnityEngine;
using System.Collections;

public class Player {
    public int playerNum;
    public int localNum;
    public Color color;
    public bool isLocal;

    public Player(int _playerNum, Color _color)
    {
        playerNum = _playerNum;
        localNum = _playerNum;
        color = _color;
    }
}
