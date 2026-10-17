using UnityEngine;

[CreateAssetMenu(fileName = "HexData", menuName = "Scriptable Objects/HexData")]
public class HexData : ScriptableObject
{
    public string hexName;
    public HexType type;
    public Sprite[] terrainSprites;
    public Sprite[] forestSprites;
    public int[] resourceYield;
    // Authored forest-only potential: Fur, Lumber. Base Week 2 yields remain unchanged.
    public int[] forestProducts = new int[2];
    public int moveCost;
}
