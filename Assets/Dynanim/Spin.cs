using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Faz o objeto rotacionar continuamente
/// </summary>
public class Spin : MonoBehaviour
{
    private enum Modo
    {
        Local,
        Global
    }

    // Parâmetros
    private float duração;

    [Space]
    [SerializeField] private Vector3 inicial;                                                       // a rotação inicial do objeto
    [SerializeField] private Vector3 direção;                                                       // quantos ângulos o objeto rotaciona num ciclo
    [SerializeField] private float ciclo;                                                           // quantos segundos até completar o ciclo   
    private Vector3 rotação;
    private float _duração;

    // Misc
    private float progression;

    // Modo
    [Space]
    [SerializeField] private Modo modo;
    public bool onStart;                                                                            // se a o componente deve começar o ciclo nesse momento
    [HideInInspector] public bool isOn;                                                             // se o ciclo está ocorrendo nesse momento
    private bool isClosing;                                                                         // se o componente está apenas terminando um ciclo
    private bool isOneOff;                                                                          // se o componente está em um ciclo one off
    public float delay;                                                                             // tempo que a imagem deve esperar para começar a piscar
    private float espera;
    [SerializeField] [Range(0, 1)] float startT;                                                    // posição T inicial do ciclo entre a mínima e máxima 

    //Unity Event
    [Space]
    public UnityEvent OnCycleEnd;
    private UnityEvent endEvent;

    // Start is called before the first frame update
    void Awake()
    {
        espera = delay;
    }

    private void Start()
    {
        if (onStart)
        {
            Ligar(startT);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (espera >= 0)
        {
            espera -= Time.deltaTime;
        }

        if ((isOn || isClosing || isOneOff) && espera < 0)
        {
            Vector3 euler;

            switch (modo)
            {
                case Modo.Local:
                    euler = transform.localRotation.eulerAngles;
                    euler += rotação * (Time.deltaTime / _duração);
                    transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;

                case Modo.Global:
                    euler = transform.rotation.eulerAngles;
                    euler += rotação * (Time.deltaTime / _duração);
                    transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;
            }
            
            progression += Time.deltaTime;
        }
        
        if ((isClosing || isOneOff) && progression >= _duração)
        {
            // desligar one off
            isOneOff = false;
            progression = _duração;

            // restituir rotação original
            Vector3 euler = inicial;
            switch (modo)
            {
                case Modo.Local:
                    transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;

                case Modo.Global:
                    transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;
            }

            if (endEvent != null)
            {
                endEvent.Invoke();
            }
        }
    }

    /// <summary>
    /// Começa o ciclo
    /// </summary>
    public void Ligar()
    {
        Ligar(0);
    }

    /// <summary>
    /// Começa o ciclo já no ponto t estipulado
    /// </summary>
    public void Ligar(float t)
    {
        isOn = true;
        isOneOff = false;
        isClosing = false;

        rotação = direção;
        _duração = ciclo;
        progression = Mathf.Lerp(0, ciclo, t);
    }

    /// <summary>
    /// Para o ciclo
    /// </summary>
    public void Desligar()
    {
        isOn = false;
        isClosing = true;
        progression = progression % _duração;
    }

    /// <summary>
    /// Executa um único ciclo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff()
    {
        if (!isOn)
        {
            OneOff(direção);
        }
    }

    /// <summary>
    /// Executa um único ciclo se o ciclo normal estivar desligado segundo os parâmetros
    /// </summary>
    public void OneOff(Vector3 eulers)
    {
        if (!isOn)
        {
            isOneOff = true;

            rotação = eulers;
            _duração = ciclo;
            progression = 0;

            endEvent = OnCycleEnd;
        }
    }
}
