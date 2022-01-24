using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Espaça os Games Objects internos
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public class Espaçador : MonoBehaviour
{
    [Tooltip("o espaçamento deve ser constantemente atualizado no editor?")]
    [SerializeField] protected bool updateSempre;                                                                               // se o script roda no editor ou não

    [Space]
    [Tooltip("evitar mexer")]
    [SerializeField] protected Transform[] pontos;                                                                                // os pontos guias do espaçador
    [SerializeField] protected List<GameObject> objetos = new List<GameObject>();                                                 // os objetos que serão espaçados

    [Space]
    [Tooltip("os objetos espaçados chegam ao final da linha ou param uma posição antes?")]
    [SerializeField] protected bool closed;

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR

        if (updateSempre)
        {
            UpdatePosition();
        }

#endif
    }

    /// <summary>
    /// Atualiza a posição dos game objetos dentro do espaçador
    /// </summary>
    [ContextMenu("Atualizar Posições")]
    public virtual void UpdatePosition()
    {
    }

    /// <summary>
    /// Retorna um array com os objetos organizados pelo espaçador
    /// </summary>
    /// <returns></returns>
    public virtual GameObject[] ObjetosEspaçados()
    {
        GameObject[] objs = new GameObject[objetos.Count];
        for (int i = 0; i < objetos.Count; i++) objs[i] = objetos[i];
        return objs;
    }

    /// <summary>
    /// Esvazia completamente esse espaçador
    /// </summary>
    public virtual void Resetar()
    {
        objetos.Clear();
    }

    /// <summary>
    /// Muda a posição local dos pontos
    /// </summary>
    public virtual void Reshape(Vector3[] novos)
    {
        for (int i = 0; i < novos.Length && i < pontos.Length; i++)
        {
            pontos[i].localPosition = novos[i];
        }

        UpdatePosition();
    }

    /// <summary>
    /// Insere um objeto novo
    /// </summary>
    public virtual void InsertObject(GameObject objeto)
    {
        objetos.Add(objeto);
        UpdatePosition();
    }
}
