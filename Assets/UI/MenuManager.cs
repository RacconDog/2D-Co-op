using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] GameObject UIHolder;
    [SerializeField] GameObject GameHolder;

    [Header("Menus")]
    [SerializeField] GameObject[] menus;
    public static int currentMenuIndex = 0;

    [SerializeField] SpriteRenderer[] playerPreviews; 

    [SerializeField] Color[] skinOptions;
    public static int[] curSkinIndex = new int[4];


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateMenuVisibility();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentMenuIndex == 1)
        {
            foreach (var controller in PlayerManager.Instance.controllerList)
            {
                if (controller != null)
                {
                    controller.GetComponent<AbstractController>()?.SpawnPlayer();
                }
            }

            PlayerManager.Instance.StartGame(GameHolder, UIHolder);
        }
    }

    // =========================
    // INPUT ENTRY POINT
    // =========================
    public void SetInput(PlayerInputData input)
    {
        int id = input.playerID;

        if (input.UISelect)
        {
            Debug.Log($"Player {id} SELECT");
            // You can hook button logic here later

        }

        if (input.UIBack)
        {
            Debug.Log($"Player {id} BACK");
            ChangeMenu(currentMenuIndex - 1);
        }

        if (input.UIShiftRight)
        {
            Debug.Log($"Player {id} SHIFT RIGHT");
            
            if (currentMenuIndex == 1) 
            {
                curSkinIndex[id] += 1;
                curSkinIndex[id] %= skinOptions.Length;

                PlayerManager.curSkin[id] = skinOptions[curSkinIndex[id]];

                playerPreviews[id].color = PlayerManager.curSkin[id];
            }
        }

        if (input.UIShiftLeft)
        {
            Debug.Log($"Player {id} SHIFT LEFT");

            if (currentMenuIndex == 1) 
            {
                curSkinIndex[id] -= 1;
                if (curSkinIndex[id] < 0) curSkinIndex[id] += skinOptions.Length;

                PlayerManager.curSkin[id] = skinOptions[curSkinIndex[id]];
                playerPreviews[id].color = PlayerManager.curSkin[id];
            }
        }
    }

    // =========================
    // MENU SWITCHING
    // =========================
    public void ChangeMenu(int newIndex)
    {
        if (menus.Length == 0) return;

        // Wrap around
        if (newIndex < 0)
            newIndex = menus.Length - 1;
        else if (newIndex >= menus.Length)
            newIndex = 0;

        currentMenuIndex = newIndex;

        UpdateMenuVisibility();
    }

    void UpdateMenuVisibility()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i] != null)
                menus[i].SetActive(i == currentMenuIndex);
        }
    }
}