using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    [Header("Referencias")]
    public bool freeze;

    [SerializeField] private bool UsarGetAxisRaw = true;
    [SerializeField] private Transform camara;

    [Header("Movimiento")]
    [SerializeField] private float VMovimiento = 5f;
    private CharacterController controlador;

    [Header("Gravedad")]
    private Vector3 velocidadVertical;
    public float Gravedad = -9f;
    public float salto = 2;

    void Start()
    {
        
    }

    void Update()
    {
        if(!freeze)
        {
            MoverJugadorEnPlano();
            AplicarGravedad();
        }
        
    }

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        if (camara == null && Camera.main != null) 
        {
            camara = Camera.main.transform;
        }
    }
  
    private void MoverJugadorEnPlano() 
    {
        float ValorHorizontal = UsarGetAxisRaw ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float ValorVertical = UsarGetAxisRaw ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        adelanteCamara.y = 0;
        derechaCamara.y = 0;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionPlano = (derechaCamara * ValorHorizontal + adelanteCamara * ValorVertical);

        if (direccionPlano.sqrMagnitude > 0.0001f)
        {
            direccionPlano.Normalize();
        }

        Vector3 desplazamientoXZ = direccionPlano * (VMovimiento * Time.deltaTime);

        controlador.Move(desplazamientoXZ);

        if (Input.GetButtonDown("Jump") && controlador.isGrounded)
        {
            velocidadVertical.y = Mathf.Sqrt(salto * -2f * Gravedad);
        }
    }

    private void AplicarGravedad()
    {
        velocidadVertical.y += Gravedad * Time.deltaTime;
        controlador.Move(velocidadVertical * Time.deltaTime);

        if (controlador.isGrounded && velocidadVertical.y < 0) 
        {
            velocidadVertical.y = -2f;
        }
    }

}