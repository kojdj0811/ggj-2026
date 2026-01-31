using UnityEngine;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    public List<Player> Players = new List<Player>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Players[0].SelectColor(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Players[1].SelectColor(7);
        }
    }

    public void OnJoinedPlayer(Player player)
    {
        Players.Add(player);

        if (Players.Count == 1)
        {
            UIManager.Instance.Player1Panel.gameObject.SetActive(true);
        }
        else if (Players.Count == 2)
        {
            UIManager.Instance.Player2Panel.gameObject.SetActive(true);
        }
    }
}
