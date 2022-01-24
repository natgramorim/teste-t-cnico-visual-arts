using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Colore texto pela lista de cores
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextColorThrough : MonoBehaviour
{
    private enum Modo
    {
        Float,
        Int
    }

    // Par�metros
    [Space]
    [SerializeField] private List<Color> cores;
    [SerializeField] private AnimationCurve curva;
    public float dura��o;
    private Color currentColor;

    // Declara��es e Refer�ncias
    private float _dura��o;
    private int origem;
    private int destino;
    private TextMeshProUGUI _texto;
    private TextMeshProUGUI texto
    {
        get
        {
            if (_texto == null)
            {
                _texto = GetComponent<TextMeshProUGUI>();
            }
            return _texto;
        }
    }
    private Coroutine rotinaAtual;

    // Modo
    [Space]
    [SerializeField] private Modo modo;
    [Tooltip("se a colora��o deve ou n�o come�ar com o start")]
    [SerializeField] private bool onStart;                                                              // se a colora��o j� come�a com o start
    [Tooltip("o tempo em segundos que o componente espera at� come�ar a colora��o no in�cio")]
    [SerializeField] private float delay;                                                               // o tempo em segundos que o componente espera at� come�ar a colora��o no in�cio
    [HideInInspector] public bool isOn;

    // Unity Events
    [Space]
    [SerializeField] UnityEvent onColorInEnd;                                                            // evento pr� definido para engatilhar com o final do fade
    [SerializeField] UnityEvent onColorOutEnd;
    private UnityEvent ColorEndEvent;

    private IEnumerator coloring()
    {
        isOn = true;
        float progression = 0;
        float t = 0;
        float p;
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

        Color cor1;
        Color cor2;

        while (t < 1)
        {
            p = progression / dura��o;
            t = curva.Evaluate(p);

            // determinar em que step est�
            if (t < 1)
            {
                switch (modo)
                {
                    case Modo.Float:
                        //step = Portion(t / portion);
                        step = Mathf.FloorToInt(t / portion);
                        cor1 = cores[Step(step)];
                        cor2 = cores[Step(step + 1)];
                        stepT = Mathf.InverseLerp(Step(step) * portion, Step(step + 1) * portion, RealT(t));
                        texto.color = Color.Lerp(cor1, cor2, stepT);
                        break;

                    case Modo.Int:
                        step = Mathf.FloorToInt(t / portion);
                        texto.color = cores[Step(step)];
                        break;
                }
            }

            progression += Time.deltaTime;

            yield return null;
        }

        currentColor = cores[destino];

        if (ColorEndEvent != null)
        {
            ColorEndEvent.Invoke();
        }

        isOn = false;
    }

    private IEnumerator iniciar(float _delay)
    {
        while (_delay > 0)
        {
            _delay -= Time.deltaTime;

            yield return null;
        }

        ColorIn();
    }

    private void Awake()
    {
        currentColor = texto.color;
    }

    private void Start()
    {
        if (onStart)
        {
            StartCoroutine(iniciar(delay));
        }
    }

    private void Update()
    {
        texto.color = currentColor;
    }

    private float RealT(float t)
    {
        if (origem < destino)
        {
            return t;
        }
        else
        {
            return 1 - t;
        }
    }

    private int Step(int valor)
    {
        float dir = Mathf.Sign((float)destino - (float)origem);
        return origem + valor * (int)dir;
    }

    /// <summary>
    /// Colore a imagem come�ando da primeira cor e terminando na �ltima
    /// </summary>
    public void ColorIn()
    {
        Debug.Log("color in");
        ColorSet(0, cores.Count - 1, dura��o, onColorInEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da �ltima cor e terminando na primeira
    /// </summary>
    public void ColorOut()
    {
        ColorSet(cores.Count - 1, 0, dura��o, onColorOutEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da primeira cor e terminando na �ltima em tantos segundos
    /// </summary>
    public void ColorIn(float segundos)
    {
        ColorSet(0, cores.Count - 1, segundos, onColorInEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da �ltima cor e terminando na primeira em tantos segundos
    /// </summary>
    public void ColorOut(float segundos)
    {
        ColorSet(cores.Count - 1, 0, segundos, onColorOutEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da primeira cor e terminando na �ltima
    /// </summary>
    public void ColorIn(int primeira, int �ltima)
    {
        ColorSet(primeira, �ltima, dura��o, onColorInEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da �ltima cor e terminando na primeira
    /// </summary>
    public void ColorOut(int primeira, int �ltima)
    {
        ColorSet(�ltima, primeira, dura��o, onColorOutEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da primeira cor e terminando na �ltima em tantos segundos
    /// </summary>
    public void ColorIn(int primeira, int �ltima, float segundos)
    {
        ColorSet(primeira, �ltima, segundos, onColorInEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da �ltima cor e terminando na primeira em tantos segundos
    /// </summary>
    public void ColorOut(int primeira, int �ltima, float segundos)
    {
        ColorSet(primeira, �ltima, segundos, onColorOutEnd);
    }

    /// <summary>
    /// Colore a imagem come�ando da primeira cor e terminando na �ltima, executando o evento no final
    /// </summary>
    public void ColorIn(UnityEvent endEvent)
    {
        ColorSet(0, cores.Count - 1, dura��o, endEvent);
    }

    /// <summary>
    /// Colore a imagem come�ando da �ltima cor e terminando na primeira, executando o evento no final
    /// </summary>
    public void ColorOut(UnityEvent endEvent)
    {
        ColorSet(cores.Count - 1, 0, dura��o, endEvent);
    }

    /// <summary>
    /// Colore a imagem come�ando da primeira cor e terminando na �ltima durante tantos segundos, executando o evento no final
    /// </summary>
    public void ColorSet(int primeira, int �ltima, float segundos, UnityEvent endEvent)
    {
        ColorEndEvent = endEvent;
        _dura��o = segundos;
        origem = primeira;
        destino = �ltima;

        Halt();
        rotinaAtual = StartCoroutine(coloring());
    }

    /// <summary>
    /// Para o movimento atual imediatamente
    /// </summary>
    public void Halt()
    {
        if (rotinaAtual != null)
        {
            StopCoroutine(rotinaAtual);
        }
    }

    /// <summary>
    /// Para o movimento atual imediatamente e define a cor na posi��o t entre o m�nimo e o m�ximo
    /// </summary>
    public void Halt(float t)
    {
        Halt();
        SetT(t);
    }

    /// <summary>
    /// Define o alfa segundo um valor t entre o valor m�nimo e m�ximo
    /// </summary>
    /// <param name="t"></param>
    public void SetT(float t)
    {
        t = Mathf.Clamp01(t);
        float portion = 0;
        int step;

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
                Color cor1 = cores[Step(step)];
                Color cor2 = cores[Step(step + 1)];
                float stepT = Mathf.InverseLerp(Step(step) * portion, Step(step + 1) * portion, RealT(t));
                texto.color = Color.Lerp(cor1, cor2, stepT);
                break;

            case Modo.Int:
                step = Mathf.FloorToInt(t / portion);
                texto.color = cores[Step(step)];
                break;
        }
    }
}
