using UnityEngine;
using System.Collections;

public class Player {
    public int playerNum;
    public int localNum;
    public Color color;
    public bool isAi;
    public bool isLocal;

    public Player(int _playerNum, Color _color)
    {
        playerNum = _playerNum;
        localNum = _playerNum;
        color = _color;
        isAi = false;
    }

    public Player(int _playerNum, Color _color, bool _isAi)
    {
        playerNum = _playerNum;
        localNum = _playerNum;
        color = _color;
        isAi = _isAi;
    }
}
