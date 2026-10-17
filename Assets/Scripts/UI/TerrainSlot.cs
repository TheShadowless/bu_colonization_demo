using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class TerrainSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Hex hex;
    public Hex Hex => hex;
    [SerializeField] private Image terrainImage;
    [SerializeField] private Image forestImage;
    [SerializeField] private Image townImage;
    [SerializeField] private Transform yieldParent;
    [SerializeField] private Transform laborParent;
    public Transform LaborParent => laborParent;
    [SerializeField] private int[] normalYield = new int[8];
    public int[] NormalYield { get => normalYield; set => normalYield = value; }
    [SerializeField] private int[] actualYield = new int[8];
    public int[] ActualYield { get => actualYield; set => actualYield = value; }
    [SerializeField] private bool centerHex;
    public bool IsCenter => centerHex;
    [SerializeField] private TMP_Text yieldText;
    [SerializeField] private List<GameObject> yieldIconList = new List<GameObject>();
    public List<GameObject> YieldIconList { get => yieldIconList; set => yieldIconList = value; }
    [SerializeField] private GameManager gameMgr;
    [SerializeField] private UIManager uiMgr;

    public void HexSlotInit(Hex hexData)
    {
        gameMgr = GameManager.instance;
        uiMgr = UIManager.instance;
        hex = hexData;
        RemoveYieldIcons();
        System.Array.Clear(normalYield, 0, normalYield.Length);
        System.Array.Clear(actualYield, 0, actualYield.Length);
        SetImage(terrainImage, hex != null ? hex.TerrainSprite.sprite : null, true);
        SetImage(forestImage, hex != null && hex.HasForest ? hex.ForestSprite.sprite : null, true);
        SetImage(townImage, hex != null && hex.HasTown && hex.Town != null ? hex.Town.TownSprite.sprite : null, false);
        if (hex != null)
            for (int i = 0; i < normalYield.Length; i++) normalYield[i] = YieldRules.NormalYield(hex, i);
    }

    private static void SetImage(Image image, Sprite sprite, bool fit)
    {
        if (image == null) return;
        image.sprite = sprite;
        image.material = fit && sprite != null ? HexArtworkProfiles.GetUIMaterial(sprite) : null;
        image.gameObject.SetActive(sprite != null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (centerHex || hex == null || uiMgr == null || uiMgr.IsProfessionOpen) return;
        var drag = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<UnitDrag>() : null;
        if (drag == null || drag.LandUnit == null || drag.LandUnit.CurHex != gameMgr.CurTown.CurHex) return;
        if (hex.Labor != null && hex.Labor != drag.LandUnit) return;
        if (hex.Labor != drag.LandUnit)
        {
            drag.QuitOldTerrainSlot();
            hex.Labor = drag.LandUnit;
            hex.YieldID = -1;
        }
        drag.WorkAtNewTerrainSlot(this);
        drag.IconParent = laborParent;
        drag.transform.position = laborParent.position;
        drag.LandUnit.UnitStatus = UnitStatus.WorkInField;
        uiMgr.RefreshProduction();
        uiMgr.SelectProfession(this);
    }

    public void SelectYield(int id)
    {
        if (hex == null || hex.Labor == null || id < 0 || id >= 8 || normalYield[id] <= 0) return;
        hex.YieldID = id;
        uiMgr.RefreshProduction();
    }

    public void RemoveYieldIcons()
    {
        foreach (var icon in yieldIconList) if (icon != null) Destroy(icon);
        yieldIconList.Clear();
        if (yieldText != null) yieldText.gameObject.SetActive(false);
    }

    public void RefreshYieldDisplay()
    {
        RemoveYieldIcons();
        System.Array.Clear(actualYield, 0, actualYield.Length);
        if (hex == null) return;
        for (int i = 0; i < normalYield.Length; i++) normalYield[i] = YieldRules.NormalYield(hex, i);
        bool assigned = centerHex || (hex.Labor != null && hex.Labor.CurHex == gameMgr.CurTown.CurHex &&
            hex.Labor.UnitStatus == UnitStatus.WorkInField);
        int id = centerHex ? 0 : hex.YieldID;
        if (!assigned || id < 0 || id >= 8) return;
        actualYield[id] = normalYield[id];
        if (yieldText != null)
        {
            yieldText.text = actualYield[id] + " " + gameMgr.ProductData[id].productName;
            yieldText.gameObject.SetActive(true);
        }
        for (int i = 0; i < actualYield[id]; i++)
        {
            var obj = Instantiate(uiMgr.YieldIconPrefab, yieldParent);
            var image = obj.GetComponent<Image>();
            image.sprite = gameMgr.ProductData[id].IconFor(hex);
            image.preserveAspect = true;
            yieldIconList.Add(obj);
        }
        uiMgr.SetupParentSpacing(actualYield[id], yieldParent, 64, 210);
    }
}
