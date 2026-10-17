using UnityEngine;
using UnityEngine.EventSystems;

public class Town : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string townName;
    public string TownName => townName;
    [SerializeField] private int[] warehouse = new int[16];
    public int[] Warehouse { get => warehouse; set => warehouse = value; }
    [SerializeField] private int[] totalYieldThisTurn = new int[16];
    public int[] TotalYieldThisTurn { get => totalYieldThisTurn; set => totalYieldThisTurn = value; }
    [SerializeField] private int crossNum;
    public int CrossNum => crossNum;
    [SerializeField] private int bellNum;
    public int BellNum => bellNum;
    [SerializeField]
    protected Faction faction;
    public Faction Faction { get { return faction; } }

    [SerializeField]
    protected int visualRange;
    public int VisualRange { get { return visualRange; } }

    [SerializeField]
    private Vector2 curPos;
    public Vector2 CurPos { get { return curPos; } set { curPos = value; } }

    [SerializeField]
    protected Hex curHex;
    public Hex CurHex { get { return curHex; } set { curHex = value; } }

    [Header("Town")]
    [SerializeField]
    protected SpriteRenderer townSprite;
    public SpriteRenderer TownSprite { get { return townSprite; } }

    [Header("Border")]
    [SerializeField]
    protected SpriteRenderer borderSprite;

    [Header("Flag")]
    [SerializeField]
    protected SpriteRenderer flagSprite;

    [SerializeField]
    protected GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TownInit(GameManager gameMgr, Faction fact)
    {
        gameManager = gameMgr;
        faction = fact;
        flagSprite.sprite = fact.FlagIcon;
        townSprite.sprite = fact.TownIcon;
        townName = fact.Nation + (fact.IsEuropean ? " Colony " : " Village ") + (fact.Towns.Count + 1);
    }

    public void RecalculateProduction()
    {
        if (totalYieldThisTurn == null || totalYieldThisTurn.Length != YieldRules.ProductCount)
            totalYieldThisTurn = new int[YieldRules.ProductCount];
        System.Array.Clear(totalYieldThisTurn, 0, totalYieldThisTurn.Length);
        if (curHex == null || gameManager == null) return;
        curHex.YieldID = 0;
        totalYieldThisTurn[0] = YieldRules.NormalYield(curHex, 0);
        foreach (var hex in HexCalculator.GetHexAround(gameManager.AllHexes, curHex))
        {
            var worker = hex.Labor;
            if (worker == null || worker.CurHex != curHex || worker.Faction != faction ||
                worker.UnitStatus != UnitStatus.WorkInField || hex.YieldID < 0 ||
                hex.YieldID >= YieldRules.RawProductCount) continue;
            totalYieldThisTurn[hex.YieldID] += YieldRules.NormalYield(hex, hex.YieldID);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left &&
            gameManager != null && gameManager.PlayerTurn && faction == gameManager.PlayerFaction &&
            UIManager.instance != null && !UIManager.instance.IsTownOpen)
            gameManager.SetupCurrentTown(this);
    }
}
