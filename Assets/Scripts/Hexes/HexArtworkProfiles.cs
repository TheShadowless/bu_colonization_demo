using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Shared materials that fit the illustrated tile silhouettes to the map grid.</summary>
public sealed class HexArtworkProfiles : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public Sprite sprite;
        public Material material;
    }

    public Entry[] entries = Array.Empty<Entry>();
    private static Dictionary<Sprite, Material> materials;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCache() => materials = null;

    public static void Apply(SpriteRenderer renderer)
    {
        if (renderer == null || renderer.sprite == null)
            return;

        if (materials == null)
        {
            materials = new Dictionary<Sprite, Material>();
            var profiles = Resources.Load<HexArtworkProfiles>("HexArtworkProfiles");
            if (profiles != null)
                foreach (var entry in profiles.entries)
                    if (entry.sprite != null && entry.material != null)
                    {
                        var instance = new Material(entry.material) { name = entry.material.name + " (Runtime Fit)" };
                        SetUVTransform(entry.sprite, instance);
                        materials[entry.sprite] = instance;
                    }
        }

        if (materials.TryGetValue(renderer.sprite, out var material))
            renderer.sharedMaterial = material;
    }

    private static void SetUVTransform(Sprite sprite, Material material)
    {
        var uv = sprite.uv;
        var vertices = sprite.vertices;
        // Derive an affine transform from the mesh, including rotated atlas entries.
        for (int i = 1; i < uv.Length - 1; i++)
        {
            Vector2 a = uv[i] - uv[0], b = uv[i + 1] - uv[0];
            float determinant = a.x * b.y - a.y * b.x;
            if (Mathf.Abs(determinant) < 0.00000001f) continue;

            Vector2 origin = Vector2.Scale(vertices[0] * sprite.pixelsPerUnit + sprite.pivot,
                new Vector2(1 / sprite.rect.width, 1 / sprite.rect.height));
            Vector2 p = Vector2.Scale((vertices[i] - vertices[0]) * sprite.pixelsPerUnit,
                new Vector2(1 / sprite.rect.width, 1 / sprite.rect.height));
            Vector2 q = Vector2.Scale((vertices[i + 1] - vertices[0]) * sprite.pixelsPerUnit,
                new Vector2(1 / sprite.rect.width, 1 / sprite.rect.height));
            Vector2 x = new Vector2(p.x * b.y - q.x * a.y, a.x * q.x - b.x * p.x) / determinant;
            Vector2 y = new Vector2(p.y * b.y - q.y * a.y, a.x * q.y - b.x * p.y) / determinant;
            material.SetVector("_UVToCanvasX", new Vector4(x.x, x.y, origin.x - Vector2.Dot(x, uv[0]), 0));
            material.SetVector("_UVToCanvasY", new Vector4(y.x, y.y, origin.y - Vector2.Dot(y, uv[0]), 0));
            return;
        }
    }
}
