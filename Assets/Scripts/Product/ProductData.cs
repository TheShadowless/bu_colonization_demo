using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "Scriptable Objects/ProductData")]
public sealed class ProductData : ScriptableObject
{
    public int id;
    public string productName;
    public Sprite[] icons;

    public Sprite IconFor(Hex hex)
    {
        if (icons == null || icons.Length == 0) return null;
        return id == 0 && hex != null && hex.HexType == HexType.Ocean && icons.Length > 1
            ? icons[1] : icons[0];
    }
}
