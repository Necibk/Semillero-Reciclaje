using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "item", menuName = "Inventario/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public int Id=0;
    public string nombre = "";
    public Sprite Icono;
    public int maxStock=1;
    public int visibleItemID = -1;

}