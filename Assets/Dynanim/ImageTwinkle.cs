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
    // Parâmetros
    [SerializeField] private AnimationCurve curva;                                                  // curva de animação da piscada
    [SerializeField] private float ciclo;                                                           // tempo em segundos para dar um ciclo completo de piscada
    private float duração;

    [Space]
    [SerializeField] private float minAlfa;                                                         // alfa mínimo
    [SerializeField] private float maxAlfa;                                                         // alfa máximo
    private float vale;
    private float pico;

    // Misc
    private float progression;
    private Image imagem;                                                                           // imagem que vai piscar

    // Modo
    [Space]
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
            float p = (progression % duração) / duração;
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

        vale = minAlfa;
        pico = maxAlfa;
        duração = ciclo;
        progression = progression = Mathf.Lerp(0, duração, t);
    }


    /// <summary>
    /// Começa o ciclo entre os pontos estipulados
    /// </summary>
    public void Ligar(float começo, float final)
    {
        isOn = true;
        isOneOff = false;

        vale = começo;
        pico = final;
        duração = ciclo;
        progression = progression = Mathf.Lerp(0, duração, 0);
    }

    /// <summary>
    /// Começa o ciclo entre os pontos estipulados no ponto t
    /// </summary>
    public void Ligar(float começo, float final, float t)
    {
        isOn = true;
        isOneOff = false;

        vale = começo;
        pico = final;
        duração = ciclo;
        progression = progression = Mathf.Lerp(0, duração, t);
    }

    /// <summary>
    /// Para de piscar
    /// </summary>
    public void Desligar()
    {
        isOn = false;
    }

    /// <summary>
    /// Pisca um único ciclo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff()
    {
        if (!isOn)
        {
            OneOff(minAlfa, maxAlfa, ciclo);
        }
    }

    /// <summary>
    /// Pisca um único ciclo que dura tantos segundos
    /// </summary>
    public void OneOff(float segundos)
    {
        if (!isOn)
        {
            OneOff(minAlfa, maxAlfa, segundos);
        }
    }

    /// <summary>
    /// Pisca um único ciclo indo do valor mínimo ao máximo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff(float min, float max)
    {
        if (!isOn)
        {
            OneOff(min, max, ciclo);
        }
    }

    /// <summary>
    /// Pisca um único ciclo se o ciclo normal estivar desligado segundo os parâmetros
    /// </summary>
    public void OneOff(float min, float max, float segundos)
    {
        if (!isOn)
        {
            isOneOff = true;

            vale = min;
            pico = max;
            duração = segundos;
            progression = 0;

            endEvent = OnCycleEnd;
        }
    }
}
