using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Realiza fade in e fade out do canvas group
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour
{
    // Parâmetros
    [Space]
    [SerializeField] private AnimationCurve curva;
    public float duração;
    public float minAlfa;
    public float maxAlfa;

    // Declarações e Referências
    private float _duração;
    private float origem;
    private float destino;
    private Canvas canvas;
    private CanvasGroup group;
    private Coroutine rotinaAtual;

    // Modo
    [Space]
    [Tooltip("se o fade in deve ou não começar com a criação do objeto")]
    [SerializeField] private bool onStart;                                                              // se o fade já começa com Awake
    [Tooltip("o tempo em segundos que o componente espera até começar o fade no início")]
    [SerializeField] private float delay;                                                               // o tempo em segundos que o componente espera até começar o fade no início
    [HideInInspector] public bool isOn;

    // Unity Events
    [Space]
    [SerializeField] UnityEvent onFadeInEnd;                                                            // evento pré definido para engatilhar com o final do fade
    [SerializeField] UnityEvent onFadeOutEnd;
    private UnityEvent FadeEndEvent;

    private IEnumerator fading(bool canvasState)
    {
        isOn = true;
        float progression = 0;
        float t = 0;
        float p;

        while (t < 1)
        {
            p = progression / duração;
            t = curva.Evaluate(p);
            group.alpha = Mathf.Lerp(origem, destino, t);

            progression += Time.deltaTime;

            yield return null;
        }

        group.alpha = destino;

        if (FadeEndEvent != null)
        {
            FadeEndEvent.Invoke();
        }

        isOn = false;
        canvas.enabled = canvasState;
    }

    private IEnumerator iniciar(float _delay)
    {
        while (_delay > 0)
        {
            _delay -= Time.deltaTime;

            yield return null;
        }

        FadeIn();
    }

    // Start is called before the first frame update
    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        if (onStart)
        {
            StartCoroutine(iniciar(delay));
        }
    }

    /// <summary>
    /// Inicia o processo de fade in a partir do alfa atual
    /// </summary>
    public void FadeInUE()
    {
        FadeIn(group.alpha, maxAlfa, duração, onFadeInEnd);
    }

    /// <summary>
    /// Inicia o processo de fade out a partir do alfa atual
    /// </summary>
    public void FadeOutUE()
    {
        FadeOut(group.alpha, minAlfa, duração, onFadeOutEnd);
    }

    /// <summary>
    /// Inicia o processo de fade in segundo os parâmetros pré estabelecidos
    /// </summary>
    public void FadeIn()
    {
        FadeIn(minAlfa, maxAlfa, duração, onFadeInEnd);
    }

    /// <summary>
    /// Inicia o processo de fade out segundo os parâmetros pré estabelecidos
    /// </summary>
    public void FadeOut()
    {
        FadeOut(maxAlfa, minAlfa, duração, onFadeOutEnd);
    }

    /// <summary>
    /// Inicia o processo de fade in levando o tempo estabelecido para acabar
    /// </summary>
    /// <param name="tempo"></param>
    private void FadeIn(float segundos)
    {
        FadeIn(minAlfa, maxAlfa, segundos, onFadeInEnd);
    }

    /// <summary>
    /// Inicia o processo de fade out levando o tempo estabelecido para acabar
    /// </summary>
    /// <param name="tempo"></param>
    private void FadeOut(float segundos)
    {
        FadeOut(maxAlfa, minAlfa, segundos, onFadeOutEnd);
    }

    /// <summary>
    /// Inicia o processo de fade in do valor mínimo até o valor máximo
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void FadeIn(float min, float max)
    {
        FadeIn(min, max, duração, onFadeInEnd);
    }

    /// <summary>
    /// Inicia o processo de fade out do valor máximo até o valor mínimo
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void FadeOut(float max, float min)
    {
        FadeOut(max, min, duração, onFadeOutEnd);
    }

    /// <summary>
    /// Inicia o processo de fade in do valor mínimo até o valor máximo levando essa quantidade de segundos
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="segundos"></param>
    public void FadeIn(float min, float max, float segundos)
    {
        FadeIn(min, max, segundos, onFadeInEnd);
    }

    /// <summary>
    /// Inicia o processo de fade out do valor máximo até o valor mínimo levando essa quantidade de segundos
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="segundos"></param>
    public void FadeOut(float max, float min, float segundos)
    {
        FadeOut(max, min, segundos, onFadeOutEnd);
    }

    /// <summary>
    /// Inicia o processo de fade in executando o Unity Event no final
    /// </summary>
    /// <param name="fadeEvent"></param>
    public void FadeIn(UnityEvent fadeEvent)
    {
        FadeIn(minAlfa, maxAlfa, duração, fadeEvent);
    }

    /// <summary>
    /// Inicia o processo de fade out executando o Unity Event no final
    /// </summary>
    /// <param name="fadeEvent"></param>
    public void FadeOut(UnityEvent fadeEvent)
    {
        FadeOut(maxAlfa, minAlfa, duração, fadeEvent);
    }

    /// <summary>
    /// Inicia o processo de fade in segundo os parâmetros precisos
    /// </summary>
    /// <param name="max"></param>
    /// <param name="min"></param>
    /// <param name="segundos"></param>
    /// <param name="fadeEvent"></param>
    public void FadeIn(float começo, float final, float segundos, UnityEvent fadeEvent)
    {
        canvas.enabled = true;
        FadeEndEvent = fadeEvent;
        _duração = segundos;
        origem = começo;
        destino = final;

        StopCoroutine(fading(true));
        StartCoroutine(fading(true));
    }

    /// <summary>
    /// Inicia o processo de fade out segundo os parâmetros precisos
    /// </summary>
    /// <param name="max"></param>
    /// <param name="min"></param>
    /// <param name="segundos"></param>
    /// <param name="fadeEvent"></param>
    public void FadeOut(float começo, float final, float segundos, UnityEvent fadeEvent)
    {
        FadeEndEvent = fadeEvent;
        _duração = segundos;
        origem = começo;
        destino = final;

        Halt();
        rotinaAtual = StartCoroutine(fading(false));
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
    /// Para o movimento atual imediatamente e define o alfa na posição t entre o mínimo e o máximo
    /// </summary>
    public void Halt(float t)
    {
        Halt();
        SetT(t);
    }

    /// <summary>
    /// Define o alfa segundo um valor t entre o valor mínimo e máximo
    /// </summary>
    /// <param name="t"></param>
    public void SetT(float t)
    {
        t = Mathf.Clamp01(t);
        var p = curva.Evaluate(t);
        group.alpha = Mathf.Lerp(origem, destino, p);
    }
}
