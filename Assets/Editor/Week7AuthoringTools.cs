using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Authors the existing Town Canvas and Week 7 data through Editor APIs.</summary>
public static class Week7AuthoringTools
{
    private const string Art = "Assets/Textures/StudentDraft/";
    private static readonly Color Navy = new Color32(16, 33, 45, 255);
    private static readonly Color Panel = new Color32(27, 48, 60, 255);
    private static readonly Color Ink = new Color32(245, 240, 226, 255);
    private static readonly Color Gold = new Color32(215, 183, 122, 255);
    private static readonly string[] Keys = { "corn", "sugar", "tobacco", "cotton", "furs", "lumber", "iron_ore", "silver", "horse", "rum", "cigar", "cloth", "coat", "trade_goods", "tools", "musket" };
    private static readonly string[] Names = { "Food", "Sugar", "Tobacco", "Cotton", "Furs", "Lumber", "Iron Ore", "Silver", "Horses", "Rum", "Cigars", "Clothes", "Coats", "Trade Goods", "Tools", "Musket" };

    public static void ImportArt()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (var folder in new[] { "Resources", "UI" })
            foreach (var path in Directory.GetFiles(Art + folder, "*.png"))
            {
                string assetPath = path.Replace('\\', '/');
                var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.spritePixelsPerUnit = 100;
                importer.maxTextureSize = 2048;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.spriteAlignment = (int)SpriteAlignment.Center;
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }
    }

    public static void SetupData()
    {
        ImportArt();
        Directory.CreateDirectory("Assets/Scripts/ProductSO");
        AssetDatabase.Refresh();
        var products = new ProductData[16];
        for (int i = 0; i < products.Length; i++)
        {
            string path = "Assets/Scripts/ProductSO/" + i.ToString("D2") + "_" + Keys[i] + ".asset";
            var data = AssetDatabase.LoadAssetAtPath<ProductData>(path);
            if (data == null) { data = ScriptableObject.CreateInstance<ProductData>(); AssetDatabase.CreateAsset(data, path); }
            data.id = i; data.productName = Names[i];
            data.icons = i == 0 ? new[] { Sprite("Resources/resource_corn.png"), Sprite("Resources/resource_fish.png") } : new[] { Sprite("Resources/resource_" + Keys[i] + ".png") };
            EditorUtility.SetDirty(data); products[i] = data;
        }
        var gm = UnityEngine.Object.FindFirstObjectByType<GameManager>();
        BindArray(gm, "productData", products);
        int[] moves = { 4, 5, 6, 8, 6, 6 }, strength = { 2, 6, 10, 8, 16, 24 }, cargo = { 2, 4, 6, 2, 4, 6 };
        foreach (string guid in AssetDatabase.FindAssets("t:NavalUnitData", new[] { "Assets/Scripts/NavalUnitSO" }))
        {
            var data = AssetDatabase.LoadAssetAtPath<NavalUnitData>(AssetDatabase.GUIDToAssetPath(guid));
            int id = (int)data.navalUnitType;
            data.unitName = new[] { "Caravel", "Merchantman", "Galleon", "Privateer", "Frigate", "Man of War" }[id];
            data.movePointMax = moves[id]; data.strength = strength[id]; data.cargoHoldNum = cargo[id];
            data.visualRange = 1; data.armed = id >= 3; EditorUtility.SetDirty(data);
        }
        foreach (string guid in AssetDatabase.FindAssets("t:LandUnitData", new[] { "Assets/Scripts/LandUnitSO" }))
        {
            var data = AssetDatabase.LoadAssetAtPath<LandUnitData>(AssetDatabase.GUIDToAssetPath(guid));
            data.strength = data.landUnitType == LandUnitType.VeteranSoldiers ? 2 : 1;
            data.visualRange = data.landUnitType == LandUnitType.SeasonedScouts ? 2 : 1;
            if (data.landUnitType == LandUnitType.SeasonedScouts) data.hasHorse = true;
            if (data.landUnitType == LandUnitType.VeteranSoldiers) { data.armed = true; data.hasMusket = true; }
            EditorUtility.SetDirty(data);
        }
        var catalogue = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("docs/draft/design-catalogue.json"));
        foreach (var forest in catalogue["forests"])
            foreach (string guid in AssetDatabase.FindAssets("t:HexData", new[] { "Assets/Scripts/HexSO" }))
            {
                var data = AssetDatabase.LoadAssetAtPath<HexData>(AssetDatabase.GUIDToAssetPath(guid));
                if ((int)data.type != (int)forest["base"]) continue;
                data.forestProducts = new[] { (int)forest["resources"][4], (int)forest["resources"][5] };
                EditorUtility.SetDirty(data);
            }
        var europeTown = Sprite("UI/town_european.png");
        foreach (string guid in AssetDatabase.FindAssets("t:FactionData", new[] { "Assets/Scripts/FactionSO" }))
        {
            var data = AssetDatabase.LoadAssetAtPath<FactionData>(AssetDatabase.GUIDToAssetPath(guid));
            if (data.isEuropean) { data.townIcon = europeTown; EditorUtility.SetDirty(data); }
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Colonization/Build Week 7 Town UI")]
    public static void BuildTownUI()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before authoring");
        var ui = UnityEngine.Object.FindFirstObjectByType<UIManager>();
        if (ui == null || ui.TownPanel == null) throw new InvalidOperationException("Open Map01 with its existing Town UI");
        var root = ui.TownPanel.GetComponent<RectTransform>();
        Undo.RegisterFullObjectHierarchyUndo(root.gameObject, "Author Week 7 Town UI");
        foreach (string path in new[] { "UnitPanel/PortPanel", "UnitPanel/SpecialPanel" })
        {
            var node = root.Find(path);
            if (node != null && node.GetComponent<LayoutGroup>() != null) UnityEngine.Object.DestroyImmediate(node.GetComponent<LayoutGroup>());
        }
        root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one; root.offsetMin = Vector2.zero; root.offsetMax = Vector2.zero;
        var backdrop = root.GetComponent<Image>(); backdrop.sprite = null; backdrop.color = Navy; backdrop.raycastTarget = true;
        var canvas = ui.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = ui.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = 0.5f;

        var header = Box(root, "NamePanel", 0, 0, 1920, 74, Panel);
        var title = Text(header, "TownNameText", "NEW COLONY", 32, 8, 1660, 54, 30, Gold);
        var close = Button(header, "ExitButton", "EXIT", 1750, 12, 138, 48);
        close.onClick = new Button.ButtonClickedEvent(); UnityEventTools.AddBoolPersistentListener(close.onClick, ui.ToggleTownPanel, false);
        Bind(ui, "townNameText", title);

        var content = Box(root, "ContentPanel", 32, 88, 1856, 600, Navy);
        var buildings = Box(content, "BuildingPanel", 0, 0, 990, 600, Panel);
        var scene = Picture(buildings, "TownIllustration", Sprite("UI/town_scene.png"), 0, 0, 990, 600);
        scene.color = new Color(1, 1, 1, 0.88f); scene.preserveAspect = false;
        Text(buildings, "BuildingTitle", "SETTLEMENT  /  BUILDING SITES", 22, 12, 946, 40, 25, Ink);
        for (int i = 0; i < 16; i++)
        {
            var place = Box(buildings, "BuildingSite" + (i + 1).ToString("D2"), 30 + i % 4 * 239, 90 + i / 4 * 122, 213, 94, new Color(0.05f, 0.12f, 0.16f, 0.65f));
            Text(place, "SiteLabel", "SITE " + (i + 1).ToString("D2"), 10, 26, 193, 40, 20, Gold);
        }
        var terrain = Box(content, "TerrainPanel", 1010, 0, 846, 600, new Color32(10, 24, 32, 255));
        Text(terrain, "TerrainTitle", "SURROUNDING LAND", 20, 8, 806, 35, 25, Gold);
        var uiData = new SerializedObject(ui);
        var center = (TerrainSlot)uiData.FindProperty("centerSlot").objectReferenceValue;
        var area = uiData.FindProperty("areaSlots");
        var positions = new[] { new Vector2(-109.5f,164.25f), new Vector2(109.5f,164.25f), new Vector2(219,0), new Vector2(109.5f,-164.25f), new Vector2(-109.5f,-164.25f), new Vector2(-219,0) };
        FitSlot(center, terrain, Vector2.zero);
        for (int i = 0; i < 6; i++) FitSlot((TerrainSlot)area.GetArrayElementAtIndex(i).objectReferenceValue, terrain, positions[i]);
        center.transform.SetAsLastSibling();

        var units = Box(root, "UnitPanel", 32, 706, 1856, 220, Navy);
        var idle = Box(units, "OutsidePanel", 0, 0, 670, 220, Panel);
        Text(idle, "IdleTitle", "IDLE WORKERS  /  DRAG TO A HEX", 18, 8, 634, 34, 23, Gold);
        var idleItems = Box(idle, "IdleItems", 18, 52, 634, 150, new Color(0,0,0,0));
        var oldDrop = idle.GetComponent<OutsideSlot>(); if (oldDrop != null) UnityEngine.Object.DestroyImmediate(oldDrop);
        if (idleItems.GetComponent<OutsideSlot>() == null) idleItems.gameObject.AddComponent<OutsideSlot>();
        var oldLayout = idle.GetComponent<HorizontalLayoutGroup>(); if (oldLayout != null) UnityEngine.Object.DestroyImmediate(oldLayout);
        Layout(idleItems, 14, false);
        Bind(ui, "outsideTownParent", idleItems.gameObject);
        var port = Box(units, "PortPanel", 688, 0, 450, 220, Panel);
        Text(port, "PortTitle", "HARBOUR", 18, 8, 414, 34, 23, Gold);
        var portText = Text(port, "PortText", "NO SHIPS NEARBY", 18, 46, 414, 28, 18, Ink);
        var portImages = new Image[3];
        for (int i = 0; i < portImages.Length; i++) portImages[i] = Picture(port, "Ship" + i, null, 15 + 140 * i, 82, 130, 112);
        BindArray(ui, "portImages", portImages); Bind(ui, "portText", portText);
        var special = Box(units, "SpecialPanel", 1156, 0, 700, 220, Panel);
        var foodText = Text(special, "FoodText", "FOOD PER TURN  0", 18, 8, 400, 34, 24, Gold);
        var food = Box(special, "FoodPanel", 18, 70, 360, 64, new Color(0,0,0,0)); Layout(food, 2, false);
        var info = Text(special, "InfoText", "BELLS  0\nCROSSES  0", 420, 48, 260, 140, 23, Ink);
        Bind(ui, "foodParent", food); Bind(ui, "foodText", foodText); Bind(ui, "infoText", info);

        var stock = Box(root, "ResourcePanel", 32, 942, 1856, 122, Panel);
        Text(stock, "StockTitle", "WAREHOUSE   /   STORED  (+ PER TURN)", 14, 2, 1828, 24, 18, Gold);
        var stockTexts = new TMP_Text[16];
        var products = (ProductData[])typeof(GameManager).GetField("productData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(UnityEngine.Object.FindFirstObjectByType<GameManager>());
        for (int i = 0; i < 16; i++)
        {
            var cell = Box(stock, "Product" + i.ToString("D2"), 8 + i * 115, 28, 113, 90, Navy);
            Picture(cell, "Icon", products[i].icons[0], 3, 2, 50, 50);
            Text(cell, "Name", new[] { "Food", "Sugar", "Tobacco", "Cotton", "Furs", "Lumber", "Iron", "Silver", "Horse", "Rum", "Cigar", "Cloth", "Coat", "Goods", "Tools", "Musket" }[i], 2, 58, 109, 26, 16, Ink);
            stockTexts[i] = Text(cell, "Quantity", "0 (+0)", 53, 10, 60, 40, 16, Gold);
        }
        BindArray(ui, "warehouseTexts", stockTexts);

        var block = Box(root, "BlockImage", 0, 0, 1920, 1080, new Color(0,0,0,0.75f));
        var profession = Box(root, "ProfessionPanel", 340, 185, 1240, 710, Panel);
        var question = Text(profession, "QuestionText", "CHOOSE A PROFESSION", 34, 20, 1172, 58, 32, Gold);
        var buttons = new Button[8]; var texts = new TMP_Text[8]; var images = new Image[8];
        for (int i = 0; i < 8; i++)
        {
            var button = Button(profession, "Profession" + i, "", 30 + i % 2 * 600, 110 + i / 2 * 125, 580, 108);
            images[i] = Picture(button.transform, "ProductIcon", products[i].icons[0], 18, 16, 76, 76);
            texts[i] = Text(button.transform, "Text", Names[i], 110, 10, 452, 88, 25, Ink);
            texts[i].alignment = TextAlignmentOptions.Left;
            button.onClick = new Button.ButtonClickedEvent(); UnityEventTools.AddIntPersistentListener(button.onClick, ui.SelectYield, i);
            buttons[i] = button;
        }
        var cancel = Button(profession, "CancelButton", "CANCEL", 950, 635, 260, 52);
        cancel.onClick = new Button.ButtonClickedEvent(); UnityEventTools.AddPersistentListener(cancel.onClick, ui.CancelProfession);
        Bind(ui, "blockImage", block.gameObject); Bind(ui, "professionPanel", profession.gameObject); Bind(ui, "labelQuestionText", question);
        BindArray(ui, "professionButtons", buttons); BindArray(ui, "btnYieldTexts", texts); BindArray(ui, "professionImages", images);
        block.gameObject.SetActive(false); profession.gameObject.SetActive(false);
        SetupPrefabs(ui);
        root.gameObject.SetActive(false);
        EditorSceneManager.MarkSceneDirty(root.gameObject.scene);
        EditorSceneManager.SaveScene(root.gameObject.scene);
        AssetDatabase.SaveAssets();
    }

    private static void FitSlot(TerrainSlot slot, RectTransform parent, Vector2 pos)
    {
        var rect = slot.GetComponent<RectTransform>(); rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f,0.5f); rect.pivot = new Vector2(0.5f,1f/3f);
        rect.sizeDelta = new Vector2(220,330); rect.anchoredPosition = pos + new Vector2(0,-12);
        var target = slot.GetComponent<Image>(); if (target != null) { target.sprite = null; target.color = new Color(0,0,0,0); target.raycastTarget = true; }
        if (slot.GetComponent<HexSlotHitArea>() == null) slot.gameObject.AddComponent<HexSlotHitArea>();
        var data = new SerializedObject(slot);
        foreach (string field in new[] { "terrainImage", "forestImage", "townImage" })
        {
            var image = (Image)data.FindProperty(field).objectReferenceValue;
            var child = image.rectTransform; child.SetParent(rect,false); child.anchorMin = child.anchorMax = new Vector2(0.5f,1f/3f);
            child.pivot = field == "townImage" ? new Vector2(0.5f,0.5f) : new Vector2(0.5f,1f/3f);
            child.sizeDelta = field == "townImage" ? new Vector2(174,174) : new Vector2(220,330);
            child.anchoredPosition = Vector2.zero; image.raycastTarget = false; image.color = Color.white; image.preserveAspect = false;
        }
        var labor = (Transform)data.FindProperty("laborParent").objectReferenceValue;
        var yield = (Transform)data.FindProperty("yieldParent").objectReferenceValue;
        PlaceGround(labor.GetComponent<RectTransform>(), new Vector2(124,112), new Vector2(0,12));
        PlaceGround(yield.GetComponent<RectTransform>(), new Vector2(210,64), new Vector2(0,-63)); Layout((RectTransform)yield, 2, false);
        var label = (TMP_Text)data.FindProperty("yieldText").objectReferenceValue;
        PlaceGround(label.rectTransform,new Vector2(180,28),new Vector2(0,-93)); label.fontSize = 18; label.color = Ink; label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false; label.gameObject.SetActive(false);
        EditorUtility.SetDirty(slot);
    }

    private static void PlaceGround(RectTransform rect, Vector2 size, Vector2 pos)
    { rect.anchorMin = rect.anchorMax = new Vector2(0.5f,1f/3f); rect.pivot = new Vector2(0.5f,0.5f); rect.sizeDelta = size; rect.anchoredPosition = pos; }

    private static void SetupPrefabs(UIManager ui)
    {
        const string yieldPath = "Assets/Prefabs/UI/YieldIcon.prefab";
        var icon = new GameObject("YieldIcon", typeof(RectTransform), typeof(Image));
        icon.GetComponent<RectTransform>().sizeDelta = new Vector2(64,64);
        icon.GetComponent<Image>().sprite = Sprite("Resources/resource_corn.png"); icon.GetComponent<Image>().preserveAspect = true; icon.GetComponent<Image>().raycastTarget = false;
        PrefabUtility.SaveAsPrefabAsset(icon,yieldPath); UnityEngine.Object.DestroyImmediate(icon);
        Bind(ui,"yieldIconPrefab",AssetDatabase.LoadAssetAtPath<GameObject>(yieldPath));
        var serialized = new SerializedObject(ui);
        string dragPath = AssetDatabase.GetAssetPath(serialized.FindProperty("unitDragPrefab").objectReferenceValue);
        var drag = PrefabUtility.LoadPrefabContents(dragPath);
        drag.GetComponent<RectTransform>().sizeDelta = new Vector2(92,112);
        drag.GetComponent<Image>().preserveAspect = true;
        PrefabUtility.SaveAsPrefabAsset(drag,dragPath); PrefabUtility.UnloadPrefabContents(drag);
    }

    private static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
    {
        var found = parent.Find(name);
        var rect = found != null ? found.GetComponent<RectTransform>() : new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent,false); rect.anchorMin = rect.anchorMax = new Vector2(0,1); rect.pivot = new Vector2(0,1);
        rect.anchoredPosition = new Vector2(x,-y); rect.sizeDelta = new Vector2(width,height); rect.localScale = Vector3.one;
        return rect;
    }
    private static RectTransform Box(Transform parent,string name,float x,float y,float w,float h,Color color)
    {
        var rect = Rect(parent,name,x,y,w,h); var image = rect.GetComponent<Image>() ?? rect.gameObject.AddComponent<Image>();
        image.sprite = null; image.overrideSprite = null; image.type = Image.Type.Simple; image.color = color; image.raycastTarget = true; return rect;
    }
    private static TMP_Text Text(Transform parent,string name,string text,float x,float y,float w,float h,int size,Color color)
    {
        var rect = Rect(parent,name,x,y,w,h); var label = rect.GetComponent<TextMeshProUGUI>() ?? rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.font = TMP_Settings.defaultFontAsset; label.text = text; label.fontSize = size; label.color = color;
        label.alignment = TextAlignmentOptions.MidlineLeft; label.textWrappingMode = TextWrappingModes.Normal; label.raycastTarget = false; return label;
    }
    private static Image Picture(Transform parent,string name,Sprite sprite,float x,float y,float w,float h)
    { var rect = Rect(parent,name,x,y,w,h); var image = rect.GetComponent<Image>() ?? rect.gameObject.AddComponent<Image>(); image.sprite = sprite; image.color = Color.white; image.preserveAspect = true; image.raycastTarget = false; return image; }
    private static Button Button(Transform parent,string name,string text,float x,float y,float w,float h)
    {
        var rect = Box(parent,name,x,y,w,h,new Color32(43,72,84,255)); var button = rect.GetComponent<Button>() ?? rect.gameObject.AddComponent<Button>();
        button.targetGraphic = rect.GetComponent<Image>();
        if (!string.IsNullOrEmpty(text)) { var label = Text(rect,"Label",text,8,4,w-16,h-8,23,Ink); label.alignment = TextAlignmentOptions.Center; }
        return button;
    }
    private static void Layout(RectTransform rect,float spacing,bool expand)
    { var layout = rect.GetComponent<HorizontalLayoutGroup>() ?? rect.gameObject.AddComponent<HorizontalLayoutGroup>(); layout.spacing = spacing; layout.childControlWidth = false; layout.childControlHeight = false; layout.childForceExpandWidth = expand; layout.childForceExpandHeight = false; layout.childAlignment = TextAnchor.MiddleLeft; }
    private static Sprite Sprite(string path)
    { var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Art+path); if (sprite == null) throw new InvalidOperationException("Missing sprite: "+path); return sprite; }
    private static void Bind(UnityEngine.Object target,string field,UnityEngine.Object value)
    { var data = new SerializedObject(target); data.FindProperty(field).objectReferenceValue = value; data.ApplyModifiedProperties(); EditorUtility.SetDirty(target); }
    private static void BindArray<T>(UnityEngine.Object target,string field,T[] values) where T : UnityEngine.Object
    { var data = new SerializedObject(target); var array = data.FindProperty(field); array.arraySize = values.Length; for(int i=0;i<values.Length;i++) array.GetArrayElementAtIndex(i).objectReferenceValue = values[i]; data.ApplyModifiedProperties(); EditorUtility.SetDirty(target); }
}
