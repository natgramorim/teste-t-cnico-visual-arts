using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Move o objeto de um ponto ao outro ao longo de um certo tempo
/// </summary>
public class MoveThrough : MonoBehaviour
{
    private enum Modo
    {
        Local,
        Global
    }

    // Par�metros
    [SerializeField] private AnimationCurve curva;
    [SerializeField] private float dura��o;
    [Space]
    [SerializeField] private Vector3 come�o;
    [SerializeField] private Vector3 final;

    // Declara��es
    private float _dura��o;
    private Vector3 _origem;
    private Vector3 _destino;
    private Coroutine rotinaAtual;

    // Modo
    [Space]
    [SerializeField] private Modo modo;
    [SerializeField] private bool onStart;
    [SerializeField] private float delay;
    [HideInInspector] public bool isOn;

    // Unity Event
    [Space]
    [SerializeField] private UnityEvent onMoveIn;
    [SerializeField] private UnityEvent onMoveOut;
    private UnityEvent MoveEndEvent;

    private IEnumerator moving()
    {
        isOn = true;
        float progression = 0;
        float t = 0;
        float p;

        while (t < 1)
        {
            p = progression / _dura��o;
            t = curva.Evaluate(p);
            
            switch (modo)
            {
                case Modo.Local:
                    transform.localPosition = Vector3.Lerp(_origem, _destino, t);
                    break;

                case Modo.Global:
                    transform.position = Vector3.Lerp(_origem, _destino, t);
                    break;
            }

            progression += Time.deltaTime;

            yield return null;
        }

        switch (modo)
        {
            case Modo.Local:
                transform.localPosition = _destino;
                break;

            case Modo.Global:
                transform.position = _destino;
                break;
        }

        if (MoveEndEvent != null)
        {
            MoveEndEvent.Invoke();
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

        MoveIn();
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
    /// Move o transform do ponto ao ponto de final partindo da sua posi��o atual
    /// </summary>
    public void MoveInUE()
    {
        Vector3 _come�o = new Vector3();

        switch (modo)
        {
            case Modo.Local:
                _come�o = transform.localPosition;
                break;

            case Modo.Global:
                _come�o = transform.position;
                break;
        }

        Move(_come�o, final, dura��o, onMoveIn);
    }

    /// <summary>
    /// Move o transform ao ponto de come�o partindo da sua posi��o atual
    /// </summary>
    public void MoveOutUE()
    {
        Vector3 _final = new Vector3();

        switch (modo)
        {
            case Modo.Local:
                _final = transform.localPosition;
                break;

            case Modo.Global:
                _final = transform.position;
                break;
        }

        Move(_final, come�o, dura��o, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto de come�o ao ponto de final
    /// </summary>
    public void MoveIn()
    {
        Move(come�o, final, dura��o, onMoveIn);
    }

    /// <summary>
    /// Move o transform do ponto de final ao ponto de come�o
    /// </summary>
    public void MoveOut()
    {
        Move(final, come�o, dura��o, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto de come�o ao ponto de final durante tantos segundos
    /// </summary>
    public void MoveIn(float segundos)
    {
        Move(come�o, final, segundos, onMoveIn);
    }

    /// <summary>
    /// Move o transform do ponto de final ao ponto de come�o durante tantos segundos
    /// </summary>
    public void MoveOut(float segundos)
    {
        Move(final, come�o, segundos, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto origem ao ponto de destino
    /// </summary>
    public void MoveIn(Vector3 origem, Vector3 destino)
    {
        Move(origem, destino, dura��o, onMoveIn);
    }

    /// <summary>
    /// Move o transform do ponto destino ao ponto de origem
    /// </summary>
    public void MoveOut(Vector3 destino, Vector3 origem)
    {
        Move(destino, origem, dura��o, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto origem ao ponto de destino em tantos segundos
    /// </summary>
    public void MoveIn(Vector3 origem, Vector3 destino, float segundos)
    {
        Move(origem, destino, segundos, onMoveIn);
    }

    /// <summary>
    /// Move o transform do ponto destino ao ponto de origem em tantos segundos
    /// </summary>
    public void MoveOut(Vector3 destino, Vector3 origem, float segundos)
    {
        Move(destino, origem, segundos, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto de come�o ao ponto de final executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void MoveIn(UnityEvent endEvent)
    {
        Move(come�o, final, dura��o, endEvent);
    }

    /// <summary>
    /// Move o transform do ponto de final ao ponto de come�o executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void MoveOut(UnityEvent endEvent)
    {
        Move(final, come�o, dura��o, endEvent);
    }

    /// <summary>
    /// Move o transformer da origem ao destino em tantos segundos e realiza o evento no final
    /// </summary>
    /// <param name="origem"></param>
    /// <param name="destino"></param>
    /// <param name="segundos"></param>
    public void Move(Vector3 origem, Vector3 destino, float segundos, UnityEvent endEvent)
    {
        MoveEndEvent = endEvent;
        _origem = origem;
        _destino = destino;
        _dura��o = segundos;
        
        Halt();
        rotinaAtual = StartCoroutine(moving());
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
    /// Para o movimento atual imediatamente e define a posi��o na posi��o t entre o m�nimo e o m�ximo
    /// </summary>
    public void Halt(float t)
    {
        Halt();
        SetT(t);
    }

    /// <summary>
    /// Define a posi��o segundo um valor t entre o ponto m�nimo e m�ximo
    /// </summary>
    /// <param name="t"></param>
    public void SetT(float t)
    {
        t = Mathf.Clamp01(t);
        var p = curva.Evaluate(t);

        switch (modo)
        {
            case Modo.Local:
                transform.localPosition = Vector3.Slerp(come�o, final, p);
                break;

            case Modo.Global:
                transform.position = Vector3.Slerp(come�o, final, p);
                break;
        }
    }
}
