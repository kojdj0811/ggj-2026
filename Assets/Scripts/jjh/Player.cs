using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerID = 0;
    public int ColorID = 0;
    public GameObject[] PlayerUIObject;
    public InputController InputController;
    private Transform _myPlayerUI;

    void Awake()
    {
        Debug.Log("New Player Joined");

        //_myPlayerUI.SetParent(UIManager.Instance.Canvas.transform);

        GameManager.Instance.OnJoinedPlayer(this);
        PlayerID = GameManager.Instance.Players.Count - 1;

        if (InputController == null)
        {
            InputController = GetComponent<InputController>();
        }
    }

    void Start()
    {

    }

    public void SelectColor(int colorID)
    {
        ColorID = colorID;
        Debug.Log($"Player {PlayerID} selected color {ColorID}");
    }
}