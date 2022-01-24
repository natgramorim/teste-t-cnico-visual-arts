using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Faz a imagem piscar em um ciclo
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageTwinkle : MonoBehaviour
{
    // Par�metros
    [SerializeField] private AnimationCurve curva;                                                  // curva de anima��o da piscada
    [SerializeField] private float ciclo;                                                           // tempo em segundos para dar um ciclo completo de piscada
    private float dura��o;

    [Space]
    [SerializeField] private float minAlfa;                                                         // alfa m�nimo
    [SerializeField] private float maxAlfa;                                                         // alfa m�ximo
    private float vale;
    private float pico;

    // Misc
    private float progression;
    private Image imagem;                                                                           // imagem que vai piscar

    // Modo
    [Space]
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
        imagem = GetComponent<Image>();

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
            progression += Time.deltaTime;
            float p = (progression % dura��o) / dura��o;
            float t = curva.Evaluate(p);
            float a = Mathf.Lerp(vale, pico, t);

            Color cor = imagem.color;
            cor.a = a;
            imagem.color = cor;
        }

        if ((isClosing || isOneOff) && progression >= ciclo)
        {
            // desligar
            isOneOff = false;
            progression = ciclo;

            // restituir ponto original
            Color cor = imagem.color;
            cor.a = minAlfa;
            imagem.color = cor;

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

        vale = minAlfa;
        pico = maxAlfa;
        dura��o = ciclo;
        progression = progression = Mathf.Lerp(0, dura��o, t);
    }


    /// <summary>
    /// Come�a o ciclo entre os pontos estipulados
    /// </summary>
    public void Ligar(float come�o, float final)
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
    public void Ligar(float come�o, float final, float t)
    {
        isOn = true;
        isOneOff = false;

        vale = come�o;
        pico = final;
        dura��o = ciclo;
        progression = progression = Mathf.Lerp(0, dura��o, t);
    }

    /// <summary>
    /// Para de piscar
    /// </summary>
    public void Desligar()
    {
        isOn = false;
    }

    /// <summary>
    /// Pisca um �nico ciclo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff()
    {
        if (!isOn)
        {
            OneOff(minAlfa, maxAlfa, ciclo);
        }
    }

    /// <summary>
    /// Pisca um �nico ciclo que dura tantos segundos
    /// </summary>
    public void OneOff(float segundos)
    {
        if (!isOn)
        {
            OneOff(minAlfa, maxAlfa, segundos);
        }
    }

    /// <summary>
    /// Pisca um �nico ciclo indo do valor m�nimo ao m�ximo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff(float min, float max)
    {
        if (!isOn)
        {
            OneOff(min, max, ciclo);
        }
    }

    /// <summary>
    /// Pisca um �nico ciclo se o ciclo normal estivar desligado segundo os par�metros
    /// </summary>
    public void OneOff(float min, float max, float segundos)
    {
        if (!isOn)
        {
            isOneOff = true;

            vale = min;
            pico = max;
            dura��o = segundos;
            progression = 0;

            endEvent = OnCycleEnd;
        }
    }
}
