using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Faz o objeto redimensionar entre duas escalas em um ciclo
/// </summary>
public class Pulse : MonoBehaviour
{
    private enum Modo
    {
        Local,
        Global
    }

    // Par�metros
    [SerializeField] private AnimationCurve curva;                                                  // curva de anima��o da piscada
    [SerializeField] private float ciclo;                                                           // tempo em segundos para dar um ciclo completo de piscada
    private float dura��o;

    [Space]
    [SerializeField] private Vector3 come�o;                                                        // ponto do come�o
    [SerializeField] private Vector3 final;                                                         // ponto do final
    private Vector3 vale;
    private Vector3 pico;

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
            float p = (progression % dura��o) / dura��o;
            float t = curva.Evaluate(p);
            transform.localScale = Vector3.Lerp(vale, pico, t);

            progression += Time.deltaTime;
        }

        if ((isClosing || isOneOff) && progression >= ciclo)
        {
            // desligar
            isOneOff = false;
            progression = ciclo;

            // restituir ponto original
            transform.localScale = vale;

            if (endEvent != null)
            {
                endEvent?.Invoke();
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

        vale = come�o;
        pico = final;
        dura��o = ciclo;
        progression = progression = Mathf.Lerp(0, dura��o, t);
    }


    /// <summary>
    /// Come�a o ciclo entre os pontos estipulados
    /// </summary>
    public void Ligar(Vector3 come�o, Vector3 final)
    {
        isOn = true;
        isOneOff = false;

        vale = come�o;
        pico = final;
        dura��o = ciclo;
        progression = progression = Mathf.Lerp(0, dura��o, 0);
    }

    /// <summary>
    /// Come�a o ciclo entre os pontos estipulados no ponto t
    /// </summary>
    public void Ligar(Vector3 come�o, Vector3 final, float t)
    {
        isOn = true;
        isOneOff = false;

        vale = come�o;
        pico = final;
        dura��o = ciclo;
        progression = progression = Mathf.Lerp(0, dura��o, t);
    }

    /// <summary>
    /// Para o ciclo
    /// </summary>
    public void Desligar()
    {
        isOn = false;
    }

    /// <summary>
    /// Executa um �nico ciclo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff()
    {
        if (!isOn)
        {
            OneOff(come�o, final, ciclo);
        }
    }

    /// <summary>
    /// Executa um �nico ciclo que dura tantos segundos
    /// </summary>
    public void OneOff(float segundos)
    {
        if (!isOn)
        {
            OneOff(come�o, final, segundos);
        }
    }

    /// <summary>
    /// Executa um �nico ciclo indo do valor m�nimo ao m�ximo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff(Vector3 origem, Vector3 destino)
    {
        if (!isOn)
        {
            OneOff(origem, destino, ciclo);
        }
    }

    /// <summary>
    /// Executa um �nico ciclo se o ciclo normal estivar desligado segundo os par�metros
    /// </summary>
    public void OneOff(Vector3 origem, Vector3 destino, float segundos)
    {
        if (!isOn)
        {
            isOneOff = true;

            vale = origem;
            pico = destino;
            dura��o = segundos;
            progression = 0;

            endEvent = OnCycleEnd;
        }
    }

    public void OneOff(Vector3 origem, Vector3 destino, float segundos, UnityEvent onEnd)
    {
        if (!isOn)
        {
            isOneOff = true;

            vale = origem;
            pico = destino;
            dura��o = segundos;
            progression = 0;

            endEvent = onEnd;
        }
    }
}
