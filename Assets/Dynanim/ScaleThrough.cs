using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Redimensiona o objeto de uma escala à outra
/// </summary>
public class ScaleThrough : MonoBehaviour
{
    // Parâmetros
    [SerializeField] private AnimationCurve curva;
    [SerializeField] private float duração;
    [Space]
    [SerializeField] private Vector3 começo = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 final = new Vector3(1, 1, 1);

    // Declarações
    private float _duração;
    private Vector3 _origem;
    private Vector3 _destino;
    private Coroutine rotinaAtual;

    // Modo
    [Space]
    [SerializeField] private bool onStart;
    [SerializeField] private float delay;
    [HideInInspector] public bool isOn;

    // Unity Event
    [Space]
    [SerializeField] private UnityEvent onScaleIn;
    [SerializeField] private UnityEvent onScaleOut;
    private UnityEvent ScaleEndEvent;

    private IEnumerator scaling()
    {
        isOn = true;
        float progression = 0;
        float t = 0;
        float p;

        while (t < 1)
        {
            p = progression / _duração;
            t = curva.Evaluate(p);
            transform.localScale = Vector3.Slerp(_origem, _destino, t);

            progression += Time.deltaTime;

            yield return null;
        }

        transform.localScale = _destino;

        if (ScaleEndEvent != null)
        {
            ScaleEndEvent.Invoke();
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

        ScaleIn();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (onStart)
        {
            StartCoroutine(iniciar(delay));
        }
    }

    /// <summary>
    /// Redimensiona o transform à escala final partindo da sua escala atual
    /// </summary>
    public void ScaleInUE()
    {
        Scale(transform.localScale, final, duração, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform à escala de começo partindo da sua escala atual
    /// </summary>
    public void ScaleOutUE()
    {
        Scale(transform.localScale, começo, duração, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de começo à escala de final
    /// </summary>
    public void ScaleIn()
    {
        Scale(começo, final, duração, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de final à escala de começo
    /// </summary>
    public void ScaleOut()
    {
        Scale(final, começo, duração, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de começo à escala de final durante tantos segundos
    /// </summary>
    public void ScaleIn(float segundos)
    {
        Scale(começo, final, segundos, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de final à escala de começo durante tantos segundos
    /// </summary>
    public void ScaleOut(float segundos)
    {
        Scale(final, começo, segundos, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de origem à escala de destino
    /// </summary>
    public void ScaleIn(Vector3 origem, Vector3 destino)
    {
        Scale(origem, destino, duração, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de destino à escala de origem
    /// </summary>
    public void ScaleOut(Vector3 destino, Vector3 origem)
    {
        Scale(destino, origem, duração, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de origem à escala de destino em tantos segundos
    /// </summary>
    public void ScaleIn(Vector3 origem, Vector3 destino, float segundos)
    {
        Scale(origem, destino, segundos, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de destino à escala de origem em tantos segundos
    /// </summary>
    public void ScaleOut(Vector3 destino, Vector3 origem, float segundos)
    {
        Scale(destino, origem, segundos, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de começo à escala de final executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void ScaleIn(UnityEvent endEvent)
    {
        Scale(começo, final, duração, endEvent);
    }

    /// <summary>
    /// Redimensiona o transform da escala de final à escala de começo executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void ScaleOut(UnityEvent endEvent)
    {
        Scale(final, começo, duração, endEvent);
    }

    /// <summary>
    /// Redimensiona o transformer da origem ao destino em tantos segundos e realiza o evento no final
    /// </summary>
    /// <param name="origem"></param>
    /// <param name="destino"></param>
    /// <param name="segundos"></param>
    public void Scale(Vector3 origem, Vector3 destino, float segundos, UnityEvent endEvent)
    {
        ScaleEndEvent = endEvent;
        _origem = origem;
        _destino = destino;
        _duração = segundos;

        Halt();
        rotinaAtual = StartCoroutine(scaling());
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
    /// Para o movimento atual imediatamente e define a escala na posição t entre o mínimo e o máximo
    /// </summary>
    public void Halt(float t)
    {
        Halt();
        SetT(t);
    }

    /// <summary>
    /// Define a dimensão segundo um valor t entre o valor mínimo e máximo
    /// </summary>
    /// <param name="t"></param>
    public void SetT(float t)
    {
        t = Mathf.Clamp01(t);
        var p = curva.Evaluate(t);
        transform.localScale = Vector3.Slerp(começo, final, p);

    }
}
