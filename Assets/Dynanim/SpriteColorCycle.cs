using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Colore o sprite em um ciclo
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteColorCycle : MonoBehaviour
{
    private enum Modo
    {
        Float,
        Int
    }

    // Parâmetros
    [SerializeField] private List<Color> cores;
    [SerializeField] private AnimationCurve curva;                                                  // curva de animação da piscada
    [SerializeField] private float ciclo;                                                           // tempo em segundos para dar um ciclo completo de piscada
    private float duração;

    // Declarações
    private int vale;
    private int pico;

    // Misc
    private float progression;
    private SpriteRenderer spriteRenderer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();

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

            int step;
            float stepT;
            float portion = 0;

            switch (modo)
            {
                case Modo.Float:
                    portion = 1f / (cores.Count - 1);
                    break;

                case Modo.Int:
                    portion = 1f / cores.Count;
                    break;
            }

            switch (modo)
            {
                case Modo.Float:
                    step = Mathf.FloorToInt(t / portion);
                    Color cor1 = cores[step];
                    Color cor2 = cores[step + 1];
                    stepT = Mathf.InverseLerp(step * portion, (step + 1) * portion, t);
                    spriteRenderer.color = Color.Lerp(cor1, cor2, stepT);
                    break;

                case Modo.Int:
                    step = Mathf.FloorToInt(t / portion);
                    spriteRenderer.color = cores[step];
                    break;
            }
        }

        if ((isClosing || isOneOff) && progression >= ciclo)
        {
            // desligar
            isOneOff = false;
            progression = ciclo;

            // restituir ponto original
            spriteRenderer.color = cores[0];

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

        vale = 0;
        pico = cores.Count - 1;
        duração = ciclo;
        progression = progression = Mathf.Lerp(0, duração, t);
    }

    /// <summary>
    /// Começa o ciclo entre os pontos estipulados
    /// </summary>
    public void Ligar(int começo, int final)
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
    public void Ligar(int começo, int final, float t)
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
            OneOff(0, cores.Count - 1, ciclo);
        }
    }

    /// <summary>
    /// Pisca um único ciclo que dura tantos segundos
    /// </summary>
    public void OneOff(float segundos)
    {
        if (!isOn)
        {
            OneOff(0, cores.Count - 1, segundos);
        }
    }

    /// <summary>
    /// Pisca um único ciclo indo do valor mínimo ao máximo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff(int min, int max)
    {
        if (!isOn)
        {
            OneOff(min, max, ciclo);
        }
    }

    /// <summary>
    /// Pisca um único ciclo se o ciclo normal estivar desligado segundo os parâmetros
    /// </summary>
    public void OneOff(int min, int max, float segundos)
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
