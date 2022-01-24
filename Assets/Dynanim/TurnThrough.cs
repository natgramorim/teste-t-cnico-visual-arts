using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Rotaciona o objeto de um �ngulo ao outro ao longo de um certo tempo
/// </summary>
public class TurnThrough : MonoBehaviour
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
    [SerializeField] private UnityEvent onTurnIn;
    [SerializeField] private UnityEvent onTurnOut;
    private UnityEvent TurnEndEvent;

    private IEnumerator turning()
    {
        isOn = true;
        float progression = 0;
        float t = 0;
        float p;
        Vector3 euler;

        while (t < 1)
        {
            p = progression / _dura��o;
            t = curva.Evaluate(p);

            switch (modo)
            {
                case Modo.Local:
                    euler = transform.localEulerAngles;
                    euler = Vector3.Lerp(_origem, _destino, t);
                    transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;

                case Modo.Global:
                    euler = transform.eulerAngles;
                    euler = Vector3.Lerp(_origem, _destino, t);
                    transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;
            }

            progression += Time.deltaTime;

            yield return null;
        }

        switch (modo)
        {
            case Modo.Local:
                euler = _destino;
                transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                break;

            case Modo.Global:
                euler = _destino;
                transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                break;
        }

        if (TurnEndEvent != null)
        {
            TurnEndEvent.Invoke();
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

        TurnIn();
    }

    private void Start()
    {
        if (onStart)
        {
            StartCoroutine(iniciar(delay));
        }
    }

    /// <summary>
    /// Rotaciona o transform ao �ngulo final partindo do seu �ngulo atual
    /// </summary>
    public void TurnInUE()
    {
        Vector3 _come�o = new Vector3();

        switch (modo)
        {
            case Modo.Local:
                _come�o = transform.localEulerAngles;
                break;

            case Modo.Global:
                _come�o = transform.eulerAngles;
                break;
        }

        Turn(_come�o, final, dura��o, onTurnIn);
    }

    /// <summary>
    /// Rotaciona o transform ao �ngulo do come�o partindo do seu �ngulo atual
    /// </summary>
    public void TurnOutUE()
    {
        Vector3 _final = new Vector3();

        switch (modo)
        {
            case Modo.Local:
                _final = transform.localEulerAngles;
                break;

            case Modo.Global:
                _final = transform.eulerAngles;
                break;
        }

        Turn(_final, come�o, dura��o, onTurnOut);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo do come�o ao �ngulo do final
    /// </summary>
    public void TurnIn()
    {
        Turn(come�o, final, dura��o, onTurnIn);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo do come�o ao �ngulo do final
    /// </summary>
    public void TurnOut()
    {
        Turn(final, come�o, dura��o, onTurnOut);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo do come�o ao �ngulo do final durante tantos segundos
    /// </summary>
    public void TurnIn(float segundos)
    {
        Turn(come�o, final, segundos, onTurnIn);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo do final ao �ngulo do come�o durante tantos segundos
    /// </summary>
    public void TurnOut(float segundos)
    {
        Turn(final, come�o, segundos, onTurnOut);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo de origem ao �ngulo de destino
    /// </summary>
    public void TurnIn(Vector3 origem, Vector3 destino)
    {
        Turn(origem, destino, dura��o, onTurnIn);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo de destino ao �ngulo de origem
    /// </summary>
    public void TurnOut(Vector3 destino, Vector3 origem)
    {
        Turn(destino, origem, dura��o, onTurnOut);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo de origem ao �ngulo de destino em tantos segundos
    /// </summary>
    public void TurnIn(Vector3 origem, Vector3 destino, float segundos)
    {
        Turn(origem, destino, segundos, onTurnIn);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo de destino ao �ngulo de origem em tantos segundos
    /// </summary>
    public void TurnOut(Vector3 destino, Vector3 origem, float segundos)
    {
        Turn(destino, origem, segundos, onTurnOut);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo de come�o ao �ngulo de final executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void TurnIn(UnityEvent endEvent)
    {
        Turn(come�o, final, dura��o, endEvent);
    }

    /// <summary>
    /// Rotaciona o transform do �ngulo de come�o ao �ngulo de final executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void TurnOut(UnityEvent endEvent)
    {
        Turn(final, come�o, dura��o, endEvent);
    }

    /// <summary>
    /// Rotaciona o transform da origem ao destino em tantos segundos e realiza o evento no final
    /// </summary>
    /// <param name="origem"></param>
    /// <param name="destino"></param>
    /// <param name="segundos"></param>
    public void Turn(Vector3 origem, Vector3 destino, float segundos, UnityEvent endEvent)
    {
        TurnEndEvent = endEvent;
        _origem = origem;
        _destino = destino;
        _dura��o = segundos;

        Halt();
        rotinaAtual = StartCoroutine(turning());
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
    /// Para o movimento atual imediatamente e define o �ngulo na posi��o t entre o m�nimo e o m�ximo
    /// </summary>
    public void Halt(float t)
    {
        Halt();
        SetT(t);
    }

    /// <summary>
    /// Define a rota��o segundo um valor t entre o �ngulo m�nimo e m�ximo
    /// </summary>
    /// <param name="t"></param>
    public void SetT(float t)
    {
        t = Mathf.Clamp01(t);
        var p = curva.Evaluate(t);
        Vector3 euler;

        switch (modo)
        {
            case Modo.Local:
                euler = transform.localEulerAngles;
                euler = Vector3.Lerp(come�o, final, p);
                transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                break;

            case Modo.Global:
                euler = transform.eulerAngles;
                euler = Vector3.Lerp(come�o, final, p);
                transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                break;
        }
    }
}
