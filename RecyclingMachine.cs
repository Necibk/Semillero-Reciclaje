using UnityEngine;

public class RecyclingMachine : MonoBehaviour, IInteractable
{
    public GameObject UI_Recicladora;
    public GameObject hotbarUI;

    public void Interactuar()
    {
        if (UI_Recicladora == null) return;

        bool nuevoEstado = !UI_Recicladora.activeSelf;
        UI_Recicladora.SetActive(nuevoEstado);

        if (hotbarUI != null)
        {
            hotbarUI.SetActive(!nuevoEstado);
        }

        // Liberar o bloquear cursor
        Cursor.visible = nuevoEstado;
        Cursor.lockState = nuevoEstado ? CursorLockMode.None : CursorLockMode.Locked;

        // Congelar movimiento del jugador
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.freeze = nuevoEstado;
        }
    }
}