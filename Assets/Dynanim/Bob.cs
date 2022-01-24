using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Faz o objeto rotacionar entre dois ângulos em um ciclo
/// </summary>
public class Bob : MonoBehaviour
{
    private enum Modo
    {
        Local,
        Global
    }

    // Parâmetros
    [SerializeField] private AnimationCurve curva;                                                  // curva de animação da piscada
    [SerializeField] private float ciclo;                                                           // tempo em segundos para dar um ciclo completo de piscada
    private float duração;

    [Space]
    [SerializeField] private Vector3 começo;                                                        // ponto do começo
    [SerializeField] private Vector3 final;                                                         // ponto do final
    private Vector3 vale;
    private Vector3 pico;

    // Misc
    private float progression;

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
            float p = (progression % duração) / duração;
            float t = curva.Evaluate(p);
            Vector3 euler;

            switch (modo)
            {
                case Modo.Local:
                    euler = Vector3.Lerp(vale, pico, t);
                    transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;

                case Modo.Global:
                    euler = Vector3.Lerp(vale, pico, t);
                    transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;
            }

            progression += Time.deltaTime;
        }

        if ((isClosing || isOneOff) && progression >= ciclo)
        {
            // desligar
            isOneOff = false;
            progression = ciclo;

            // restituir ponto original
            Vector3 euler = vale;

            switch (modo)
            {
                case Modo.Local:
                    transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;

                case Modo.Global:
                    transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);
                    break;
            }

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

        vale = começo;
        pico = final;
        duração = ciclo;
        progression = progression = Mathf.Lerp(0, duração, t);
    }

    /// <summary>
    /// Começa o ciclo entre os pontos estipulados
    /// </summary>
    public void Ligar(Vector3 começo, Vector3 final)
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
    public void Ligar(Vector3 começo, Vector3 final, float t)
    {
        isOn = true;
        isOneOff = false;

        vale = começo;
        pico = final;
        duração = ciclo;
        progression = progression = Mathf.Lerp(0, duração, t);
    }

    /// <summary>
    /// Para o ciclo
    /// </summary>
    public void Desligar()
    {
        isOn = false;
    }

    /// <summary>
    /// Executa um único ciclo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff()
    {
        if (!isOn)
        {
            OneOff(começo, final, ciclo);
        }
    }

    /// <summary>
    /// Executa um único ciclo que dura tantos segundos
    /// </summary>
    public void OneOff(float segundos)
    {
        if (!isOn)
        {
            OneOff(começo, final, segundos);
        }
    }

    /// <summary>
    /// Executa um único ciclo indo do valor mínimo ao máximo se o ciclo normal estivar desligado
    /// </summary>
    public void OneOff(Vector3 origem, Vector3 destino)
    {
        if (!isOn)
        {
            OneOff(origem, destino, ciclo);
        }
    }

    /// <summary>
    /// Executa um único ciclo se o ciclo normal estivar desligado segundo os parâmetros
    /// </summary>
    public void OneOff(Vector3 origem, Vector3 destino, float segundos)
    {
        if (!isOn)
        {
            isOneOff = true;

            vale = origem;
            pico = destino;
            duração = segundos;
            progression = 0;

            endEvent = OnCycleEnd;
        }
    }
}
