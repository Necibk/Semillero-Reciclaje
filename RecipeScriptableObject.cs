using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Ingredient
{
    public ItemScriptableObject item;
    public int cantidad;
}

[CreateAssetMenu(fileName = "Nueva Receta", menuName = "Inventario/Receta de Crafteo")]
public class RecipeScriptableObject : ScriptableObject
{
    public string nombreReceta;
    public List<Ingredient> ingredientes = new List<Ingredient>();
    public ItemScriptableObject resultado;
    public int cantidadResultado = 1;
}