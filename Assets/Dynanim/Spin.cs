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

    // Par�metros
    private float dura��o;

    [Space]
    [SerializeField] private Vector3 inicial;                                                       // a rota��o inicial do objeto
    [SerializeField] private Vector3 dire��o;                                                       // quantos �ngulos o objeto rotaciona num ciclo
    [SerializeField] private float ciclo;                                                           // quantos segundos at� completar o ciclo   
    private Vector3 rota��o;
    private float _dura��o;

    // Misc
    private float progression;

    // Modo
    [Space]
    [SerializeField] private Modo modo;
    public bool onStart;                                                                            // se a o componente deve come�ar o ciclo nesse momento
    [HideInInspector] public bool isOn;                                                             // se o ciclo est� ocorrendo nesse momento
    private bool isClosing;                                                                         // se o componente est� apenas terminando um ciclo
    private bool isOneOff;                                                                          // se o componente est� em um ciclo one off
    public float delay;                                                                             // tempo que a imagem deve esperar para come�ar a piscar
    private float espera;
    [SerializeField] [Range(0, 1)] float startT;                                                    // posi��o T inicial do ciclo entre a m�nima e m�xima 

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
                    euler += rota��o * (Time.deltaTime / _dura��o);
                    transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;

                case Modo.Global:
                    euler = transform.rotation.eulerAngles;
                    euler += rota��o * (Time.deltaTime / _dura��o);
                    transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;
            }
            
            progression += Time.deltaTime;
        }
        
        if ((isClosing || isOneOff) && progression >= _dura��o)
        {
            // desligar one off
            isOneOff = false;
            progression = _dura��o;

            // restituir rota��o original
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
    /// Come�a o ciclo
    /// </summary>
    public void Ligar()
    {
        Ligar(0);
    }

    /// <summary>
    /// Come�a o ciclo j� no ponto t estipulado
    /// </summary>
    public void Ligar(float t)
    {
        isOn = true;
        isOneOff = false;
        isClosing = false;

        rota��o = dire��o;
        _dura��o = ciclo;
        progression = Mathf.Lerp(0, ciclo, t);
    }

    /// <summary>
    /// Para o ciclo
    /// </summary>
    public void Desligar()
    {
        isOn = false;
        isClosing = true;
        progression = progression % _dura��o;
    }

    /// <summary>
    /// Executa um �nico ciclo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff()
    {
        if (!isOn)
        {
            OneOff(dire��o);
        }
    }

    /// <summary>
    /// Executa um �nico ciclo se o ciclo normal estivar desligado segundo os par�metros
    /// </summary>
    public void OneOff(Vector3 eulers)
    {
        if (!isOn)
        {
            isOneOff = true;

            rota��o = eulers;
            _dura��o = ciclo;
            progression = 0;

            endEvent = OnCycleEnd;
        }
    }
}
