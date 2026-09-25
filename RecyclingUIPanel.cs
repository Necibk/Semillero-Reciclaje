using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecyclingUIPanel : MonoBehaviour
{
    [Header("Lista de Recetas")]
    public List<RecipeScriptableObject> todasLasRecetas;
    public Transform contenedorListaRecetas; // El Grid/Content a la izquierda
    public GameObject prefabBotonReceta;     // El botón prefab para la lista

    [Header("Panel de Detalles (Derecha)")]
    public Image iconoResultado;
    public TextMeshProUGUI textoNombreResultado;
    public Transform contenedorIngredientes; // Donde se dibujan los ingredientes necesarios
    public GameObject prefabIconoIngrediente; // Prefab con Image + Text (para la cantidad)
    public Button botonCraft;

    private RecipeScriptableObject recetaSeleccionada;

    private void OnEnable()
    {
        GenerarListaRecetas();
    }

    private void GenerarListaRecetas()
    {
        // Limpiar lista anterior
        foreach (Transform child in contenedorListaRecetas)
        {
            Destroy(child.gameObject);
        }

        // Crear un botón por cada receta
        foreach (var receta in todasLasRecetas)
        {
            GameObject btn = Instantiate(prefabBotonReceta, contenedorListaRecetas);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = receta.nombreReceta;

            // Al hacer clic en este botón de la lista, mostramos los detalles de esta receta
            btn.GetComponent<Button>().onClick.AddListener(() => SeleccionarReceta(receta));
        }

        // Seleccionar la primera receta por defecto
        if (todasLasRecetas.Count > 0)
        {
            SeleccionarReceta(todasLasRecetas[0]);
        }
    }

    public void SeleccionarReceta(RecipeScriptableObject receta)
    {
        recetaSeleccionada = receta;

        // Mostrar icono y nombre del resultado
        if (iconoResultado != null && receta.resultado != null)
        {
            iconoResultado.sprite = receta.resultado.Icono;
            iconoResultado.enabled = true;
        }

        if (textoNombreResultado != null)
        {
            textoNombreResultado.text = receta.nombreReceta;
        }

        // Dibujar ingredientes requeridos
        DibujarIngredientes(receta);

        // Actualizar estado del botón Craft (si se puede craftear o no)
        ActualizarEstadoBoton();
    }

    private void DibujarIngredientes(RecipeScriptableObject receta)
    {
        foreach (Transform child in contenedorIngredientes)
        {
            Destroy(child.gameObject);
        }

        foreach (var ing in receta.ingredientes)
        {
            GameObject elem = Instantiate(prefabIconoIngrediente, contenedorIngredientes);

            // Imagen del ingrediente
            Image img = elem.GetComponentInChildren<Image>();
            if (img != null) img.sprite = ing.item.Icono;

            // Texto de cantidad (ej: 3/5 o solo la cantidad requerida)
            TextMeshProUGUI txt = elem.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                int cantidadJugador = InventoryManager.Instance.GetItemCount(ing.item);
                txt.text = $"{cantidadJugador}/{ing.cantidad}";

                // Color verde si tiene suficientes materiales, rojo si le faltan
                txt.color = cantidadJugador >= ing.cantidad ? Color.green : Color.red;
            }
        }
    }

    private void ActualizarEstadoBoton()
    {
        if (botonCraft != null)
        {
            bool sePuede = CraftingManager.Instance.PuedeCraftear(recetaSeleccionada);
            botonCraft.interactable = sePuede;
        }
    }

    public void CraftearSeleccionado()
    {
        if (recetaSeleccionada != null)
        {
            if (CraftingManager.Instance.IntentarCraftear(recetaSeleccionada))
            {
                // Volver a actualizar la UI para refrescar las cantidades de los ingredientes
                SeleccionarReceta(recetaSeleccionada);
            }
        }
    }
}