using UnityEngine;

/// <summary>The final eight-resource normal-yield formula in DOC Week 7.</summary>
public static class YieldRules
{
    public const int RawProductCount = 8;
    public const int ProductCount = 16;

    public static int NormalYield(Hex hex, int id)
    {
        if (hex == null || id < 0 || id >= RawProductCount) return 0;
        int value = hex.GetResourcePotential(id);
        if (id == 4 || id == 5) { if (!hex.HasForest) value = 0; }
        else if (hex.HasForest) value -= 2;
        if (hex.HasTown) value = Mathf.CeilToInt(value / 2f);
        return Mathf.Max(0, value);
    }
}
