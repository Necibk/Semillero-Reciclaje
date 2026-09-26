using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public ItemScriptableObject ItemScriptableObject;
    [HideInInspector] public int Cantidad;

    public Image Icono;
    private TextMeshProUGUI cantidadText;

    private void Awake()
    {
        cantidadText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetItem(ItemScriptableObject newItem, int nuevaCantidad)
    {
        ItemScriptableObject = newItem;
        Cantidad = nuevaCantidad;

        if (Icono != null)
        {
            Icono.enabled = true;
            Icono.sprite = ItemScriptableObject.Icono;
        }

        if (cantidadText != null)
        {
            cantidadText.text = Cantidad > 1 ? Cantidad.ToString() : "";
        }
    }

    public void ClearItem()
    {
        ItemScriptableObject = null;
        Cantidad = 0;

        if (Icono != null)
        {
            Icono.sprite = null;
            Icono.enabled = false;
        }

        if (cantidadText != null)
        {
            cantidadText.text = "";
        }
    }

    // --- DRAG AND DROP ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemScriptableObject == null) return;

        if (cantidadText != null) cantidadText.text = "";
        if (Icono != null) Icono.enabled = false;

        if (InventoryManager.Instance != null && InventoryManager.Instance.ghostIcon != null)
        {
            InventoryManager.Instance.ghostIcon.enabled = true;
            InventoryManager.Instance.ghostIcon.sprite = ItemScriptableObject.Icono;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ItemScriptableObject == null) return;

        if (InventoryManager.Instance != null && InventoryManager.Instance.ghostIcon != null)
        {
            InventoryManager.Instance.ghostIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ItemScriptableObject == null) return;

        if (Icono != null) Icono.enabled = true;
        if (cantidadText != null) cantidadText.text = Cantidad > 1 ? Cantidad.ToString() : "";

        if (InventoryManager.Instance != null && InventoryManager.Instance.ghostIcon != null)
        {
            InventoryManager.Instance.ghostIcon.enabled = false;
        }

        // Intercambio con el slot destino
        if (eventData.pointerEnter != null)
        {
            Slot slotDestino = eventData.pointerEnter.GetComponent<Slot>();

            if (slotDestino != null && slotDestino != this)
            {
                // Slot vacío
                if (slotDestino.ItemScriptableObject == null)
                {
                    slotDestino.SetItem(ItemScriptableObject, Cantidad);
                    ClearItem();
                }
                // Mismo tipo de ítem -> intentar apilar
                else if (slotDestino.ItemScriptableObject == ItemScriptableObject)
                {
                    if (slotDestino.Cantidad + Cantidad <= ItemScriptableObject.maxStock)
                    {
                        slotDestino.SetItem(ItemScriptableObject, slotDestino.Cantidad + Cantidad);
                        ClearItem();
                    }
                    else
                    {
                        int cantidadAmover = ItemScriptableObject.maxStock - slotDestino.Cantidad;
                        slotDestino.SetItem(ItemScriptableObject, ItemScriptableObject.maxStock);
                        this.SetItem(ItemScriptableObject, Cantidad - cantidadAmover);
                    }
                }
                // Ítem diferente -> intercambiar posiciones
                else
                {
                    ItemScriptableObject temporalItemData = slotDestino.ItemScriptableObject;
                    int temporalCantidad = slotDestino.Cantidad;

                    slotDestino.SetItem(ItemScriptableObject, Cantidad);
                    this.SetItem(temporalItemData, temporalCantidad);
                }
            }
        }
    }
}