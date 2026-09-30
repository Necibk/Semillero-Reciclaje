using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject playerInventoryUI;
    public PlayerMovement playerMovement; 
    public Inventary playerInventory; 
    public Transform hotbarContainer; // Arrastra el objeto HotBar en el Inspector
    public GameObject VidibleItemsContainer;
    
    public Transform dropPoint;

    private Slot[] hotBarSlots;
    private int activeSlots = -1;
    private PlayerStats playerStats;
    private Visibleitem[] visibleItems;
    private Visibleitem itemVisibleActivo;

    // Arreglo para mapear teclas 1-0
    private KeyCode[] hotbarKeys =
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
        KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
    };

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        // Se obtiene el componente Inventary automáticamente del mismo GameObject
        playerInventory = GetComponent<Inventary>();

        if (hotbarContainer != null)
        {
            hotBarSlots = hotbarContainer.GetComponentsInChildren<Slot>();
        }

        if (VidibleItemsContainer != null)
        {
            visibleItems = VidibleItemsContainer.GetComponentsInChildren<Visibleitem>(true);
        }
    }

    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SoltarItemActivo();
        }
        // Abrir y cerrar inventario
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerInventoryUI.SetActive(!playerInventoryUI.activeSelf);

            if (playerInventoryUI.activeSelf)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                if (playerMovement != null) playerMovement.freeze = true;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                if (playerMovement != null) playerMovement.freeze = false;
            }
        }
    }
  public void SoltarItemActivo()
    {
        // 1. Validar que haya un slot de hotbar seleccionado
        if (activeSlots < 0 || activeSlots >= hotBarSlots.Length)
        {
            Debug.LogWarning("No has seleccionado ninguna casilla con los números 1-9.");
            return;
        }

        Slot slotActivo = hotBarSlots[activeSlots];

        // 2. Verificar que el slot realmente contenga un ítem
        if (slotActivo == null || slotActivo.ItemScriptableObject == null)
        {
            Debug.LogWarning("La casilla seleccionada está vacía.");
            return;
        }

        ItemScriptableObject itemASoltar = slotActivo.ItemScriptableObject;

        // 3. Determinar posición de aparición (spawn)
        Vector3 spawnPos = (dropPoint != null) 
            ? dropPoint.position 
            : (transform.position + transform.forward * 1.5f + Vector3.up * 0.5f);

        // 4. Instanciar en el juego
        if (itemASoltar.prefabObjeto != null)
        {
            GameObject itemInstanciado = Instantiate(itemASoltar.prefabObjeto, spawnPos, Quaternion.identity);

            // Asignar ScriptableObject y cantidad al componente AgarrarItem para que se pueda volver a recoger[cite: 2]
            AgarrarItem agarrarComp = itemInstanciado.GetComponent<AgarrarItem>();
            if (agarrarComp != null)
            {
                agarrarComp.ItemScriptableObject = itemASoltar;
                agarrarComp.cantidad = 1;
            }

            // Impulso hacia adelante si tiene Rigidbody
            Rigidbody rb = itemInstanciado.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * 3f, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogError("El ItemScriptableObject " + itemASoltar.nombre + " no tiene un prefabObjeto asignado en el Inspector.");
        }

        // 5. Descontar del inventario/slot
        playerInventory.RemoveItemFromSlot(slotActivo, 1);

        // 6. Refrescar visualmente la mano y el slot
        UsarSlot(activeSlots);
    }  
public void UsarSlot(int index)
    {
        if (hotBarSlots == null || index < 0 || index >= hotBarSlots.Length) return;
        if (hotBarSlots[index] == null) return;

        activeSlots = index;

        // Desactivar el ítem que esté actualmente visible en mano
        if (itemVisibleActivo != null)
        {
            ActivarDesactivarSibleItem(false, itemVisibleActivo);
        }

        // Si el slot no tiene ítem, salimos
        if (hotBarSlots[index].ItemScriptableObject == null) return;

        // Obtenemos el ID del ScriptableObject
        int idBuscado = hotBarSlots[index].ItemScriptableObject.visibleItemID;

        // Buscamos cuál Visibleitem coincide con ese ID
        if (visibleItems != null)
        {
            foreach (Visibleitem item in visibleItems)
            {
                if (item != null && item.visibleItemID == idBuscado)
                {
                    ActivarDesactivarSibleItem(true, item);
                    break;
                }
            }
        }
    }

    public void ActivarDesactivarSibleItem(bool estado, Visibleitem item)
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
}
