using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject townPanel;
    public GameObject TownPanel { get => townPanel; set => townPanel = value; }
    public bool IsTownOpen => townPanel != null && townPanel.activeSelf;
    public bool IsProfessionOpen => professionPanel != null && professionPanel.activeSelf;
    [SerializeField] private TerrainSlot centerSlot;
    [SerializeField] private TerrainSlot[] areaSlots;
    [SerializeField] private GameObject unitDragPrefab;
    [SerializeField] private GameObject outsideTownParent;
    [SerializeField] private GameObject yieldIconPrefab;
    public GameObject YieldIconPrefab { get => yieldIconPrefab; set => yieldIconPrefab = value; }
    [SerializeField] private List<GameObject> allUnitDrags = new List<GameObject>();
    public List<GameObject> AllUnitDrags { get => allUnitDrags; set => allUnitDrags = value; }
    [SerializeField] private TerrainSlot currentSlot;
    [SerializeField] private GameObject blockImage;
    [SerializeField] private GameObject professionPanel;
    [SerializeField] private TMP_Text labelQuestionText;
    [SerializeField] private TMP_Text[] btnYieldTexts;
    [SerializeField] private Button[] professionButtons;
    [SerializeField] private Image[] professionImages;
    [SerializeField] private Transform foodParent;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private List<GameObject> foodIconList = new List<GameObject>();
    [SerializeField] private TMP_Text townNameText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text[] warehouseTexts;
    [SerializeField] private Image[] portImages;
    [SerializeField] private TMP_Text portText;
    public static UIManager instance;
    private static readonly string[] Professions = {
        "Farmer", "Sugar Planter", "Tobacco Planter", "Cotton Planter",
        "Fur Trapper", "Lumberjack", "Ore Miner", "Silver Miner"
    };

    private void Awake() => instance = this;

    private void Update()
    {
        if (IsTownOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (IsProfessionOpen) CancelProfession();
            else ToggleTownPanel(false);
        }
    }

    public void SetupHexSlots(Hex centerHex, Hex[] aroundHexes)
    {
        centerSlot.HexSlotInit(centerHex);
        for (int i = 0; i < areaSlots.Length; i++)
            areaSlots[i].HexSlotInit(i < aroundHexes.Length ? aroundHexes[i] : null);
    }

    private void DestroyOldUnitDrag()
    {
        foreach (var obj in allUnitDrags) if (obj != null) Destroy(obj);
        allUnitDrags.Clear();
    }

    private void HideUnitWorkInTown(Hex hex)
    {
        if (hex == null) return;
        foreach (var unit in hex.UnitsInHex)
            if (unit.UnitStatus == UnitStatus.WorkInField) unit.gameObject.SetActive(false);
    }

    public void SetupUnitDragOutsideTown(Hex hex)
    {
        foreach (var unit in hex.UnitsInHex)
        {
            if (unit.UnitType != UnitType.Land || unit.UnitStatus != UnitStatus.None) continue;
            var obj = Instantiate(unitDragPrefab, outsideTownParent.transform);
            allUnitDrags.Add(obj);
            obj.GetComponent<UnitDrag>().UnitInit((LandUnit)unit);
        }
    }

    public void SetupUnitDragWorkingInTerrain()
    {
        foreach (var slot in areaSlots)
        {
            if (slot.Hex == null || slot.Hex.Labor == null ||
                slot.Hex.Labor.CurHex != GameManager.instance.CurTown.CurHex) continue;
            var obj = Instantiate(unitDragPrefab, slot.LaborParent);
            allUnitDrags.Add(obj);
            var drag = obj.GetComponent<UnitDrag>();
            drag.UnitInit((LandUnit)slot.Hex.Labor);
            drag.IconParent = slot.LaborParent;
            drag.WorkAtNewTerrainSlot(slot);
        }
    }

    private void RebuildWorkerIcons()
    {
        DestroyOldUnitDrag();
        SetupUnitDragOutsideTown(GameManager.instance.CurTown.CurHex);
        SetupUnitDragWorkingInTerrain();
    }

    public void ToggleTownPanel(bool show)
    {
        if (!show)
        {
            if (IsProfessionOpen) CancelProfession();
            DestroyOldUnitDrag();
            RemoveAllYieldIcons();
            if (GameManager.instance != null && GameManager.instance.CurTown != null)
                HideUnitWorkInTown(GameManager.instance.CurTown.CurHex);
        }
        townPanel.SetActive(show);
    }

    public void SetupCurrentTown(Hex curHex, Hex[] aroundHexes)
    {
        CloseProfession();
        SetupHexSlots(curHex, aroundHexes);
        RebuildWorkerIcons();
        RefreshProduction();
    }

    public void RefreshProduction()
    {
        var gm = GameManager.instance;
        if (gm == null || gm.CurTown == null || gm.ProductData == null || gm.ProductData.Length != 16) return;
        var town = gm.CurTown;
        town.RecalculateProduction();
        centerSlot.RefreshYieldDisplay();
        foreach (var slot in areaSlots) slot.RefreshYieldDisplay();
        UpdateTotalFoodIcons();
        if (townNameText != null)
            townNameText.text = town.TownName + "     " + gm.CurrentYear + "     TURN " + gm.GameTurn + "     GOLD " + town.Faction.Gold;
        if (infoText != null)
            infoText.text = "BELLS  " + town.BellNum + "\nCROSSES  " + town.CrossNum + "\n" + town.Faction.Nation;
        if (warehouseTexts != null)
            for (int i = 0; i < warehouseTexts.Length && i < 16; i++)
                if (warehouseTexts[i] != null)
                    warehouseTexts[i].text = town.Warehouse[i] + "  (+" + town.TotalYieldThisTurn[i] + ")";
        RefreshPort();
    }

    public void SetupParentSpacing(int n, Transform parent, int iconWidth, int parentWidth)
    {
        var layout = parent != null ? parent.GetComponent<HorizontalLayoutGroup>() : null;
        if (layout == null) return;
        layout.spacing = n > 1 ? Mathf.Min(4, (parentWidth - iconWidth * n) / (float)(n - 1)) : 0;
    }

    public void UpdateTotalFoodIcons()
    {
        foreach (var obj in foodIconList) if (obj != null) Destroy(obj);
        foodIconList.Clear();
        int total = GameManager.instance.CurTown.TotalYieldThisTurn[0];
        if (foodText != null) foodText.text = "FOOD PER TURN  " + total;
        if (foodParent == null) return;
        for (int i = 0; i < total; i++)
        {
            var obj = Instantiate(yieldIconPrefab, foodParent);
            obj.GetComponent<Image>().sprite = GameManager.instance.ProductData[0].icons[0];
            foodIconList.Add(obj);
        }
        SetupParentSpacing(total, foodParent, 64, 360);
    }

    public void RemoveAllYieldIcons()
    {
        centerSlot.RemoveYieldIcons();
        foreach (var slot in areaSlots) slot.RemoveYieldIcons();
        foreach (var obj in foodIconList) if (obj != null) Destroy(obj);
        foodIconList.Clear();
    }

    public void SelectProfession(TerrainSlot slot)
    {
        if (slot == null || slot.Hex == null || slot.Hex.Labor == null) return;
        currentSlot = slot;
        labelQuestionText.text = "Choose work for " + slot.Hex.Labor.UnitName;
        for (int i = 0; i < 8; i++)
        {
            int amount = slot.NormalYield[i];
            string role = i == 0 && slot.Hex.HexType == HexType.Ocean ? "Fisherman" : Professions[i];
            btnYieldTexts[i].text = role + "\n" + amount + " " + GameManager.instance.ProductData[i].productName + " / turn";
            professionButtons[i].interactable = amount > 0;
            professionImages[i].sprite = GameManager.instance.ProductData[i].IconFor(slot.Hex);
        }
        blockImage.SetActive(true);
        professionPanel.SetActive(true);
    }

    public void SelectYield(int id)
    {
        if (currentSlot == null || id < 0 || id >= 8 || currentSlot.NormalYield[id] <= 0) return;
        currentSlot.SelectYield(id);
        CloseProfession();
    }

    public void CancelProfession()
    {
        bool rebuild = false;
        if (currentSlot != null && currentSlot.Hex != null &&
            currentSlot.Hex.YieldID == -1 && currentSlot.Hex.Labor != null)
        {
            currentSlot.Hex.Labor.UnitStatus = UnitStatus.None;
            currentSlot.Hex.Labor = null;
            rebuild = true;
        }
        CloseProfession();
        if (rebuild) RebuildWorkerIcons();
        RefreshProduction();
    }

    private void CloseProfession()
    {
        if (blockImage != null) blockImage.SetActive(false);
        if (professionPanel != null) professionPanel.SetActive(false);
        currentSlot = null;
    }

    private void RefreshPort()
    {
        if (portImages == null) return;
        var town = GameManager.instance.CurTown;
        var hexes = HexCalculator.GetHexAround(GameManager.instance.AllHexes, town.CurHex);
        hexes.Add(town.CurHex);
        int count = 0;
        foreach (var hex in hexes)
            foreach (var unit in hex.UnitsInHex)
                if (unit.UnitType == UnitType.Naval && unit.Faction == town.Faction && count < portImages.Length)
                {
                    portImages[count].sprite = unit.UnitSprite.sprite;
                    portImages[count].gameObject.SetActive(true);
                    count++;
                }
        for (int i = count; i < portImages.Length; i++) portImages[i].gameObject.SetActive(false);
        if (portText != null) portText.text = count == 0 ? "NO SHIPS NEARBY" : count + " SHIP(S) NEARBY";
    }
}
