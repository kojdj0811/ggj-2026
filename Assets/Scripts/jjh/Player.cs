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

        // if (GameManager.Instance.Players.Count == 0)
        // {
        //     _myPlayerUI = PlayerUIObject[0].transform;
        //     PlayerUIObject[0].SetActive(true);
        //     PlayerID = 0;
        // }
        // else
        // {
        //     _myPlayerUI = PlayerUIObject[1].transform;
        //     PlayerUIObject[1].SetActive(true);
        //     PlayerID = 1;
        // }
        

        //_myPlayerUI.SetParent(UIManager.Instance.Canvas.transform);

        GameManager.Instance.OnJoinedPlayer(this);

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