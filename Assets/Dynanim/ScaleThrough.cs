using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Redimensiona o objeto de uma escala � outra
/// </summary>
public class ScaleThrough : MonoBehaviour
{
    // Par�metros
    [SerializeField] private AnimationCurve curva;
    [SerializeField] private float dura��o;
    [Space]
    [SerializeField] private Vector3 come�o = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 final = new Vector3(1, 1, 1);

    // Declara��es
    private float _dura��o;
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
            p = progression / _dura��o;
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
    /// Redimensiona o transform � escala final partindo da sua escala atual
    /// </summary>
    public void ScaleInUE()
    {
        Scale(transform.localScale, final, dura��o, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform � escala de come�o partindo da sua escala atual
    /// </summary>
    public void ScaleOutUE()
    {
        Scale(transform.localScale, come�o, dura��o, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de come�o � escala de final
    /// </summary>
    public void ScaleIn()
    {
        Scale(come�o, final, dura��o, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de final � escala de come�o
    /// </summary>
    public void ScaleOut()
    {
        Scale(final, come�o, dura��o, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de come�o � escala de final durante tantos segundos
    /// </summary>
    public void ScaleIn(float segundos)
    {
        Scale(come�o, final, segundos, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de final � escala de come�o durante tantos segundos
    /// </summary>
    public void ScaleOut(float segundos)
    {
        Scale(final, come�o, segundos, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de origem � escala de destino
    /// </summary>
    public void ScaleIn(Vector3 origem, Vector3 destino)
    {
        Scale(origem, destino, dura��o, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de destino � escala de origem
    /// </summary>
    public void ScaleOut(Vector3 destino, Vector3 origem)
    {
        Scale(destino, origem, dura��o, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de origem � escala de destino em tantos segundos
    /// </summary>
    public void ScaleIn(Vector3 origem, Vector3 destino, float segundos)
    {
        Scale(origem, destino, segundos, onScaleIn);
    }

    /// <summary>
    /// Redimensiona o transform da escala de destino � escala de origem em tantos segundos
    /// </summary>
    public void ScaleOut(Vector3 destino, Vector3 origem, float segundos)
    {
        Scale(destino, origem, segundos, onScaleOut);
    }

    /// <summary>
    /// Redimensiona o transform da escala de come�o � escala de final executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void ScaleIn(UnityEvent endEvent)
    {
        Scale(come�o, final, dura��o, endEvent);
    }

    /// <summary>
    /// Redimensiona o transform da escala de final � escala de come�o executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void ScaleOut(UnityEvent endEvent)
    {
        Scale(final, come�o, dura��o, endEvent);
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
        _dura��o = segundos;

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
    /// Para o movimento atual imediatamente e define a escala na posi��o t entre o m�nimo e o m�ximo
    /// </summary>
    public void Halt(float t)
    {
        Halt();
        SetT(t);
    }

    /// <summary>
    /// Define a dimens�o segundo um valor t entre o valor m�nimo e m�ximo
    /// </summary>
    /// <param name="t"></param>
    public void SetT(float t)
    {
        t = Mathf.Clamp01(t);
        var p = curva.Evaluate(t);
        transform.localScale = Vector3.Slerp(come�o, final, p);

    }
}
