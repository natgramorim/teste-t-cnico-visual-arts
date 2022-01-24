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

    // Parâmetros
    [SerializeField] private AnimationCurve curva;
    [SerializeField] private float duração;
    [Space]
    [SerializeField] private Vector3 começo;
    [SerializeField] private Vector3 final;

    // Declarações
    private float _duração;
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
            p = progression / _duração;
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
    /// Move o transform do ponto ao ponto de final partindo da sua posição atual
    /// </summary>
    public void MoveInUE()
    {
        Vector3 _começo = new Vector3();

        switch (modo)
        {
            case Modo.Local:
                _começo = transform.localPosition;
                break;

            case Modo.Global:
                _começo = transform.position;
                break;
        }

        Move(_começo, final, duração, onMoveIn);
    }

    /// <summary>
    /// Move o transform ao ponto de começo partindo da sua posição atual
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

        Move(_final, começo, duração, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto de começo ao ponto de final
    /// </summary>
    public void MoveIn()
    {
        Move(começo, final, duração, onMoveIn);
    }

    /// <summary>
    /// Move o transform do ponto de final ao ponto de começo
    /// </summary>
    public void MoveOut()
    {
        Move(final, começo, duração, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto de começo ao ponto de final durante tantos segundos
    /// </summary>
    public void MoveIn(float segundos)
    {
        Move(começo, final, segundos, onMoveIn);
    }

    /// <summary>
    /// Move o transform do ponto de final ao ponto de começo durante tantos segundos
    /// </summary>
    public void MoveOut(float segundos)
    {
        Move(final, começo, segundos, onMoveOut);
    }

    /// <summary>
    /// Move o transform do ponto origem ao ponto de destino
    /// </summary>
    public void MoveIn(Vector3 origem, Vector3 destino)
    {
        Move(origem, destino, duração, onMoveIn);
    }

    /// <summary>
    /// Move o transform do ponto destino ao ponto de origem
    /// </summary>
    public void MoveOut(Vector3 destino, Vector3 origem)
    {
        Move(destino, origem, duração, onMoveOut);
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
    /// Move o transform do ponto de começo ao ponto de final executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void MoveIn(UnityEvent endEvent)
    {
        Move(começo, final, duração, endEvent);
    }

    /// <summary>
    /// Move o transform do ponto de final ao ponto de começo executando o evento no final
    /// </summary>
    /// <param name="endEvent"></param>
    public void MoveOut(UnityEvent endEvent)
    {
        Move(final, começo, duração, endEvent);
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
        _duração = segundos;
        
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
    /// Para o movimento atual imediatamente e define a posição na posição t entre o mínimo e o máximo
    /// </summary>
    public void Halt(float t)
    {
        Halt();
        SetT(t);
    }

    /// <summary>
    /// Define a posição segundo um valor t entre o ponto mínimo e máximo
    /// </summary>
    /// <param name="t"></param>
    public void SetT(float t)
    {
        t = Mathf.Clamp01(t);
        var p = curva.Evaluate(t);

        switch (modo)
        {
            case Modo.Local:
                transform.localPosition = Vector3.Slerp(começo, final, p);
                break;

            case Modo.Global:
                transform.position = Vector3.Slerp(começo, final, p);
                break;
        }
    }
}
