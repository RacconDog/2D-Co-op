using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject curMenu = null;
    public static MenuTypes curMenuType = MenuTypes.localMenuSetup;

    public enum MenuTypes
    {
        main,
        localMenuSetup
    }

    [SerializeField] SpriteRenderer[] playerSlots = new SpriteRenderer[4];

    public void MenuSelect(GameObject selectedMenu, MenuTypes selectedMenuType)
    {
        curMenu.SetActive(false);
        selectedMenu.SetActive(true);

        curMenu = selectedMenu;
        curMenuType = selectedMenuType;
    }

    void Update()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            playerSlots[i].color = PlayerManager.curPlayerColors[i];
        }
    }
}
