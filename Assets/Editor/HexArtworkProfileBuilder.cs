using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>Builds rendering profiles without changing the submitted PNG artwork.</summary>
public static class HexArtworkProfileBuilder
{
    private const string Output = "Assets/Materials/HexArtworkFit";

    [MenuItem("Tools/Colonization/Rebuild Hex Artwork Fit")]
    public static void Rebuild()
    {
        Directory.CreateDirectory(Output);
        Directory.CreateDirectory("Assets/Resources");
        AssetDatabase.Refresh();
        var shader = Shader.Find("Colonization/Hex Artwork Fit");
        if (shader == null)
            throw new System.InvalidOperationException("Missing Hex Artwork Fit shader");

        var entries = new List<HexArtworkProfiles.Entry>();
        foreach (var folder in new[] { "Terrain", "Forests" })
        {
            foreach (var path in Directory.GetFiles("Assets/Textures/StudentDraft/" + folder, "*.png"))
            {
                var assetPath = path.Replace('\\', '/');
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null)
                    throw new System.InvalidOperationException("Missing sprite: " + assetPath);

                var decoded = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                decoded.LoadImage(File.ReadAllBytes(path));
                var pixels = decoded.GetPixels32();
                int width = decoded.width, height = decoded.height;
                int bottom = height, top = -1;
                // Ignore faint antialiasing dust outside the visible illustration.
                for (int y = 0; y < height; y++)
                {
                    int count = 0;
                    for (int x = 0; x < width; x++)
                        if (pixels[y * width + x].a >= 192) count++;
                    if (count >= 3) { bottom = Mathf.Min(bottom, y); top = y; }
                }
                if (top <= bottom)
                    throw new System.InvalidOperationException("Empty illustration: " + assetPath);

                const int rows = 256;
                var spans = new Color[rows];
                for (int row = 0; row < rows; row++)
                {
                    float t = (row + 0.5f) / rows;
                    int y = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(bottom, top, t)), bottom, top);
                    int left = width, right = -1;
                    // Select the longest filled run, avoiding disconnected flecks.
                    int run = -1;
                    for (int x = 0; x <= width; x++)
                    {
                        bool filled = x < width && pixels[y * width + x].a >= 192;
                        if (filled && run < 0) run = x;
                        if (!filled && run >= 0)
                        {
                            if (x - run > right - left + 1) { left = run; right = x - 1; }
                            run = -1;
                        }
                    }
                    if (right < left)
                        throw new System.InvalidOperationException("Unfilled illustration row: " + assetPath);
                    float inset = Mathf.Min(0.75f, (right - left) * 0.25f);
                    spans[row] = new Color((left + 0.5f + inset) / width,
                        (right + 0.5f - inset) / width, (y + 0.5f) / height, 1);
                }
                Object.DestroyImmediate(decoded);

                string name = Path.GetFileNameWithoutExtension(path);
                var lookup = AssetDatabase.LoadAssetAtPath<Texture2D>(Output + "/" + name + "_rows.asset");
                if (lookup == null)
                {
                    lookup = new Texture2D(1, rows, TextureFormat.RGBAFloat, false, true);
                    AssetDatabase.CreateAsset(lookup, Output + "/" + name + "_rows.asset");
                }
                lookup.name = name + "_rows";
                lookup.wrapMode = TextureWrapMode.Clamp;
                lookup.filterMode = FilterMode.Bilinear;
                lookup.SetPixels(spans);
                lookup.Apply(false, false);
                EditorUtility.SetDirty(lookup);

                var material = AssetDatabase.LoadAssetAtPath<Material>(Output + "/" + name + ".mat");
                if (material == null)
                {
                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, Output + "/" + name + ".mat");
                }
                material.shader = shader;
                material.SetTexture("_ArtworkTex", AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath));
                material.SetTexture("_RowSpans", lookup);
                material.SetVector("_Canvas", new Vector4(width, height, sprite.pixelsPerUnit, 256));
                material.SetVector("_UVToCanvasX", new Vector4(1, 0, 0, 0));
                material.SetVector("_UVToCanvasY", new Vector4(0, 1, 0, 0));
                EditorUtility.SetDirty(material);
                entries.Add(new HexArtworkProfiles.Entry { sprite = sprite, material = material });
            }
        }
        const string profilePath = "Assets/Resources/HexArtworkProfiles.asset";
        var profiles = AssetDatabase.LoadAssetAtPath<HexArtworkProfiles>(profilePath);
        if (profiles == null)
        {
            profiles = ScriptableObject.CreateInstance<HexArtworkProfiles>();
            AssetDatabase.CreateAsset(profiles, profilePath);
        }
        profiles.entries = entries.ToArray();
        EditorUtility.SetDirty(profiles);
        AssetDatabase.SaveAssets();
        Debug.Log("Hex artwork profiles rebuilt: " + entries.Count);
    }
}
