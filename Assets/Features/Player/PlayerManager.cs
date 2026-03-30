using UnityEngine;
using Unity.Netcode;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] public static GameObject[] playerList = new GameObject[4];
    [SerializeField] public Color[] PLAYER_COLORS;

    [Header("DEBUG")]
    public static Color[] curPlayerColors = new Color[4];
    public int[] curPlayerColorsNUM = new int[4];

    public void CycleSelectSkin(int playerNum)
    {
        playerNum -= 1;
        curPlayerColors[playerNum] = PLAYER_COLORS[curPlayerColorsNUM[playerNum] % PLAYER_COLORS.Length];
        curPlayerColorsNUM[playerNum] += 1;
    }
}
