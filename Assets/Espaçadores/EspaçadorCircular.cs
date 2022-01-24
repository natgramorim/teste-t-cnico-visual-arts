using Freya;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Espaça os Games Objects internos ao longo da circunferência de forma regular
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
public class EspaçadorCircular : MonoBehaviour
{
    [Tooltip("o espaçamento deve ser constantemente atualizado no editor?")]
    [SerializeField] private bool updateSempre;                                                                                 // se o script roda no editor ou não

    [Space]
    [SerializeField] private float raio = 1;
    [SerializeField] private List<GameObject> objetos = new List<GameObject>();                                                 // os objetos que serão espaçados

    [Space]
    [Tooltip("os objetos espaçados chegam ao final da linha ou param uma posição antes?")]
    [SerializeField] private bool closed;                                                                                       // se os objetos espaçados chegam ao final ou param uma posição antes

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // desenhar o raio
        Handles.DrawWireDisc(transform.position, Vector3.forward, raio, 0.5f);
    }
#endif

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
    /// Atualiza a posição dos game objetos ao longo da linha
    /// </summary>
    [ContextMenu("Atualizar Posições")]
    public void UpdatePosition()
    {
        // encontrar porção T entre cada
        float t;

        if (closed)
        {
            t = 1f / (objetos.Count - 1);
        }
        else
        {
            t = 1f / objetos.Count;
        }

        // espaçar
        GameObject atual;

        for (int i = 0; i < objetos.Count; i++)
        {
            atual = objetos[i];
            atual.transform.position = transform.position + new Vector3(Mathfs.Cos(i * t * Mathfs.TAU), Mathfs.Sin(i * t * Mathfs.TAU), 0) * raio;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Cria um espaçador novo
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("GameObject/Espaçadores/Espaçador Circular", false, 1)]
    static void Create(MenuCommand menuCommand)
    {
        // Create a custom game object
        //GameObject go = new GameObject("Custom Game Object");
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Espaçadores/Espaçador Circular.prefab", typeof(GameObject));
        GameObject go = Instantiate(prefab);
        go.name = "Novo Espaçador Circular";

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
#endif
}
