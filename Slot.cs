using System;
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

    void Start()
    {
        cantidadText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetItem(ItemScriptableObject ItemScriptableObject, int cantidad)
    {
        this.ItemScriptableObject = ItemScriptableObject;
        this.Cantidad = cantidad;

        if (Icono != null)
        {
            Icono.enabled = true; 
            Icono.sprite = ItemScriptableObject.Icono; 
        }

        if (cantidadText != null)
        {
            cantidadText.text = cantidad.ToString();
        }
    }

    public void SubstraerCantidad(int cantidadARestar)
    {
        Cantidad -= cantidadARestar;
        if (Cantidad <= 0)
        {
            ClearItem(); // Limpia la imagen y referencias del slot
        }
        else
        {
            if (cantidadText != null)
                cantidadText.text = Cantidad.ToString();
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemScriptableObject == null) return;

        if (cantidadText != null) cantidadText.text = "";
        if (Icono != null) Icono.enabled = false;

        UIManager.Instance.ghostIcon.enabled = true;
        UIManager.Instance.ghostIcon.sprite = ItemScriptableObject.Icono;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ItemScriptableObject == null) return;
        UIManager.Instance.ghostIcon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ItemScriptableObject == null) return;

        if (Icono != null) Icono.enabled = true;
        UIManager.Instance.ghostIcon.enabled = false;
        if (cantidadText != null) cantidadText.text = Cantidad.ToString();

        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("Slot"))
        {
            Slot slotDestino = eventData.pointerEnter.GetComponent<Slot>();

            if (slotDestino != null && slotDestino != this)
            {

                if (slotDestino.ItemScriptableObject == null)
                {
                    slotDestino.SetItem(ItemScriptableObject, Cantidad);
                    ClearItem();
                    return;
                }                else if (slotDestino.ItemScriptableObject == ItemScriptableObject)
                {
                    if (slotDestino.Cantidad + Cantidad <= ItemScriptableObject.maxStock) 
                    {
                        slotDestino.SetItem(ItemScriptableObject, slotDestino.Cantidad + Cantidad);
                        ClearItem();
                    }
                    else //Si no pude, sumo lo que puedo
                    {
                        int cantidadAmover = ItemScriptableObject.maxStock - slotDestino.Cantidad;
                        slotDestino.SetItem(ItemScriptableObject, ItemScriptableObject.maxStock);
                        this.SetItem(ItemScriptableObject, Cantidad - cantidadAmover);
                    }
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
