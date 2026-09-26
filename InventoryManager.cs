using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Referencias UI y Jugador")]
    public GameObject playerInventoryUI;
    public PlayerMovement playerMovement;
    public Transform slotContainer;
    public Transform hotbarContainer;
    public GameObject visibleItemsContainer;

    public Image ghostIcon; // Arrastra el objeto GhostIcon en el Inspector

    [Header("Items de Prueba (Opcional)")]
    public ItemScriptableObject itemDev;
    public ItemScriptableObject itemDev2;

    private List<Slot> allSlots = new List<Slot>();
    private Slot[] hotBarSlots;
    private Visibleitem[] visibleItems;
    private Visibleitem itemVisibleActivo;
    private int activeSlotIndex = -1;

    private KeyCode[] hotbarKeys =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
        KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
    };

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        // Cargar todos los slots (Inventario Principal + Hotbar)
        if (slotContainer != null)
        {
            allSlots.AddRange(slotContainer.GetComponentsInChildren<Slot>());
        }

        if (hotbarContainer != null)
        {
            hotBarSlots = hotbarContainer.GetComponentsInChildren<Slot>();
            allSlots.AddRange(hotbarContainer.GetComponentsInChildren<Slot>(true));
        }

        if (visibleItemsContainer != null)
        {
            visibleItems = visibleItemsContainer.GetComponentsInChildren<Visibleitem>(true);
        }

        Debug.Log("Inventario inicializado con " + allSlots.Count + " slots.");
    }

    private void Update()
    {
        // Teclas 1-0 para la Hotbar
        if (hotBarSlots != null)
        {
            int limite = Mathf.Min(hotBarSlots.Length, hotbarKeys.Length);
            for (int i = 0; i < limite; i++)
            {
                if (Input.GetKeyDown(hotbarKeys[i]))
                {
                    UsarSlot(i);
                }
            }
        }

        // Abrir / Cerrar Inventario con TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        // Teclas de prueba rápido (F1 / F2)
        if (Input.GetKeyDown(KeyCode.F1) && itemDev != null) AddItem(itemDev, 1);
        if (Input.GetKeyDown(KeyCode.F2) && itemDev2 != null) AddItem(itemDev2, 1);
    }

    // --- LÓGICA DEL INVENTARIO ---

    public int GetItemCount(ItemScriptableObject item)
    {
        int total = 0;
        foreach (var slot in allSlots)
        {
            if (slot.ItemScriptableObject == item)
            {
                total += slot.Cantidad;
            }
        }
        return total;
    }

    public int AddItem(ItemScriptableObject itemData, int cantidad)
    {
        int cantidadAGuardar = cantidad;

        // 1. Apilar en slots que ya tengan el mismo ítem y espacio libre
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (allSlots[i].ItemScriptableObject == itemData && allSlots[i].Cantidad < itemData.maxStock)
            {
                int espacioDisponible = itemData.maxStock - allSlots[i].Cantidad;
                int cantidadAAlmacenar = Mathf.Min(espacioDisponible, cantidadAGuardar);

                allSlots[i].SetItem(itemData, allSlots[i].Cantidad + cantidadAAlmacenar);
                cantidadAGuardar -= cantidadAAlmacenar;

                if (cantidadAGuardar <= 0) return 0;
            }
        }

        // 2. Si sobra, buscar slots vacíos
        if (cantidadAGuardar > 0)
        {
            for (int i = 0; i < allSlots.Count; i++)
            {
                if (allSlots[i].ItemScriptableObject == null)
                {
                    int cantidadAAlmacenar = Mathf.Min(itemData.maxStock, cantidadAGuardar);

                    allSlots[i].SetItem(itemData, cantidadAAlmacenar);
                    cantidadAGuardar -= cantidadAAlmacenar;

                    if (cantidadAGuardar <= 0) return 0;
                }
            }
        }

        return cantidadAGuardar; // Retorna lo que no cupo
    }

    public bool HasItem(ItemScriptableObject itemData, int cantidadRequerida)
    {
        int contador = 0;
        foreach (Slot slot in allSlots)
        {
            if (slot.ItemScriptableObject == itemData)
            {
                contador += slot.Cantidad;
                if (contador >= cantidadRequerida) return true;
            }
        }
        return false;
    }

    public void RemoveItem(ItemScriptableObject itemData, int cantidadARetirar)
    {
        foreach (Slot slot in allSlots)
        {
            if (slot.ItemScriptableObject == itemData)
            {
                if (slot.Cantidad <= cantidadARetirar)
                {
                    cantidadARetirar -= slot.Cantidad;
                    slot.ClearItem();
                }
                else
                {
                    slot.SetItem(itemData, slot.Cantidad - cantidadARetirar);
                    cantidadARetirar = 0;
                }

                if (cantidadARetirar <= 0) break;
            }
        }
    }

    // --- MANEJO DE HOTBAR Y VISUALES ---

    public void UsarSlot(int index)
    {
        if (hotBarSlots == null || index < 0 || index >= hotBarSlots.Length) return;
        if (hotBarSlots[index] == null) return;

        activeSlotIndex = index;

        if (itemVisibleActivo != null)
        {
            ActivarDesactivarVisibleItem(false, itemVisibleActivo);
        }

        if (hotBarSlots[index].ItemScriptableObject == null) return;

        int idBuscado = hotBarSlots[index].ItemScriptableObject.visibleItemID;

        if (visibleItems != null)
        {
            foreach (Visibleitem item in visibleItems)
            {
                if (item != null && item.visibleItemID == idBuscado)
                {
                    ActivarDesactivarVisibleItem(true, item);
                    break;
                }
            }
        }
    }

    private void ActivarDesactivarVisibleItem(bool estado, Visibleitem item)
    {
        if (item == null || item.itemVisible == null) return;

        item.itemVisible.SetActive(false);
        if (estado)
        {
            itemVisibleActivo = item;
            item.itemVisible.SetActive(true);
        }
        else
        {
            itemVisibleActivo = null;
        }
    }

    private void ToggleInventory()
    {
        if (playerInventoryUI == null) return;

        playerInventoryUI.SetActive(!playerInventoryUI.activeSelf);
        bool estaAbierto = playerInventoryUI.activeSelf;

        Cursor.visible = estaAbierto;
        Cursor.lockState = estaAbierto ? CursorLockMode.None : CursorLockMode.Locked;

        if (playerMovement != null)
        {
            playerMovement.freeze = estaAbierto;
        }
    }
}