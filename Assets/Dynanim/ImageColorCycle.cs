using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Colore a imagem em um ciclo
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageColorCycle : MonoBehaviour
{
    private enum Modo
    {
        Float,
        Int
    }

    // Par�metros
    [SerializeField] private List<Color> cores;
    [SerializeField] private AnimationCurve curva;                                                  // curva de anima��o da piscada
    [SerializeField] private float ciclo;                                                           // tempo em segundos para dar um ciclo completo de piscada
    private float dura��o;

    // Declara��es
    private int vale;
    private int pico;

    // Misc
    private float progression;
    private Image imagem;                                                                           // imagem que vai piscar

    // Modo
    [Space]
    [SerializeField] private Modo modo;
    public bool onStart;                                                                            // se a o componente deve come�ar o ciclo nesse momento
    [HideInInspector] public bool isOn;                                                             // se o ciclo est� ocorrendo nesse momento
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

        if ((isOn || isOneOff) && espera < 0)
        {
            progression += Time.deltaTime;
            float p = (progression % dura��o) / dura��o;
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
                    imagem.color = Color.Lerp(cor1, cor2, stepT);
                    break;

                case Modo.Int:
                    step = Mathf.FloorToInt(t / portion);
                    imagem.color = cores[step];
                    break;
            }
        }

        if (isOneOff && progression >= ciclo)
        {
            isOneOff = false;

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

        vale = 0;
        pico = cores.Count - 1;
        dura��o = ciclo;
        progression = progression = Mathf.Lerp(0, dura��o, t);
    }


    /// <summary>
    /// Come�a o ciclo entre os pontos estipulados
    /// </summary>
    public void Ligar(int come�o, int final)
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
    public void Ligar(int come�o, int final, float t)
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
            OneOff(0, cores.Count - 1, ciclo);
        }
    }

    /// <summary>
    /// Pisca um �nico ciclo que dura tantos segundos
    /// </summary>
    public void OneOff(float segundos)
    {
        if (!isOn)
        {
            OneOff(0, cores.Count - 1, segundos);
        }
    }

    /// <summary>
    /// Pisca um �nico ciclo indo do valor m�nimo ao m�ximo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff(int min, int max)
    {
        if (!isOn)
        {
            OneOff(min, max, ciclo);
        }
    }

    /// <summary>
    /// Pisca um �nico ciclo se o ciclo normal estivar desligado segundo os par�metros
    /// </summary>
    public void OneOff(int min, int max, float segundos)
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
