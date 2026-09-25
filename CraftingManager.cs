using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool PuedeCraftear(RecipeScriptableObject receta)
    {
        if (receta == null || receta.ingredientes == null) return false;

        foreach (var ing in receta.ingredientes)
        {
            if (!InventoryManager.Instance.HasItem(ing.item, ing.cantidad))
            {
                return false;
            }
        }
        return true;
    }

    public bool IntentarCraftear(RecipeScriptableObject receta)
    {
        if (!PuedeCraftear(receta))
        {
            Debug.Log("No tienes los residuos necesarios para reciclar esto.");
            return false;
        }

        foreach (var ing in receta.ingredientes)
        {
            InventoryManager.Instance.RemoveItem(ing.item, ing.cantidad);
        }

        InventoryManager.Instance.AddItem(receta.resultado, receta.cantidadResultado);
        Debug.Log($"¡Reciclado exitoso! Obtuviste: {receta.resultado.nombre}");
        return true;
    }
}