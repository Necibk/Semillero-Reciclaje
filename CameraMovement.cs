using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] public float Sensibilidad = 2f;
    public Transform Player;
    private PlayerMovement playerMovementScript;

    private float RotacionHorizontal = 0;
    private float RotacionVertical = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Player != null)
        {
            playerMovementScript = Player.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        // 1. Si el cursor está visible o el jugador está congelado, NO mover la cámara
        if (Cursor.visible || (playerMovementScript != null && playerMovementScript.freeze))
        {
            return;
        }

        float ValorX = Input.GetAxis("Mouse X") * Sensibilidad;
        float ValorY = Input.GetAxis("Mouse Y") * Sensibilidad;

        RotacionHorizontal += ValorX;
        RotacionVertical -= ValorY;

        RotacionVertical = Mathf.Clamp(RotacionVertical, -80f, 80f);

        transform.localRotation = Quaternion.Euler(RotacionVertical, 0f, 0f);

        if (Player != null)
        {
            Player.Rotate(Vector3.up * ValorX);
        }
    }
}