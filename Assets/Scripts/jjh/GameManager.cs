using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public List<InputController> Players = new List<InputController>();
    public GameObject Canvas;
}
