using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UnitDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private bool dragging;
    [SerializeField]
    private Image unitImage;

    [SerializeField]
    private LandUnit landUnit;
    public LandUnit LandUnit { get { return landUnit; } set { landUnit = value; } }

    [SerializeField]
    private Transform iconParent;
    public Transform IconParent { get { return iconParent; } set { iconParent = value; } }

    [SerializeField]
    private TerrainSlot terrainSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnitInit(LandUnit landUnit)
    {
        unitImage.sprite = landUnit.UnitSprite.sprite;
        this.landUnit = landUnit;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (UIManager.instance.IsProfessionOpen) return;
        dragging = true;
        iconParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        unitImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragging) transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging) return;
        dragging = false;
        transform.SetParent(iconParent);
        unitImage.raycastTarget = true;
    }

    public void QuitOldTerrainSlot()
    {
        if (terrainSlot != null)
        {
            if (terrainSlot.Hex != null && terrainSlot.Hex.Labor == landUnit)
            {
                terrainSlot.Hex.Labor = null;
                terrainSlot.Hex.YieldID = -1;
            }
            terrainSlot = null;
        }
    }

    public void WorkAtNewTerrainSlot(TerrainSlot terrainSlot)
    {
        this.terrainSlot = terrainSlot;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!dragging && terrainSlot != null && eventData.button == PointerEventData.InputButton.Left &&
            !UIManager.instance.IsProfessionOpen) UIManager.instance.SelectProfession(terrainSlot);
    }
}
