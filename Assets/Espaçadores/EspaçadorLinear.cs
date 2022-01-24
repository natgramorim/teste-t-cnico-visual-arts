using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

/// <summary>
/// Espaça os Games Objects internos de um ponto ao outro de forma regular
/// </summary>
public class EspaçadorLinear : Espaçador
{
    [Space]
    public int tamanho;                                                                                     // se os objetos espaçados chegam ao final ou param uma posição antes

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // desenhar a linha entre eles
        Vector3[] pos = { pontos[0].position, pontos[1].position };
        Handles.DrawAAPolyLine(3, pos);

        // desenhar os pontos
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pontos[0].position, 0.2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pontos[1].position, 0.2f);

        Gizmos.color = Color.white;
    }
    #endif

    /// <summary>
    /// Insere um objeto novo
    /// </summary>
    public override void InsertObject(GameObject objeto)
    {
        tamanho += 1;
        base.InsertObject(objeto);
    }

    /// <summary>
    /// Atualiza a posição dos game objetos ao longo da linha
    /// </summary>
    [ContextMenu("Atualizar Posições")]
    public override void UpdatePosition()
    {
        if (objetos.Count > 0)
        {
            // encontrar porção T entre cada
            float t;

            if (closed)
            {
                t = 1f / (Mathf.Max(tamanho - 1, 1));
            }
            else
            {
                t = 1f / tamanho;
            }

            // espaçar
            GameObject atual;

            for (int i = 0; i < objetos.Count; i++)
            {
                atual = objetos[i];
                atual.transform.position = Vector3.Lerp(pontos[0].position, pontos[1].position, t * i);
            }
        }
    }

    /// <summary>
    /// Esvazia completamente esse espaçador
    /// </summary>
    public override void Resetar()
    {
        base.Resetar();
        tamanho = 0;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Cria um espaçador novo
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("GameObject/Espaçadores/Espaçador Linear", false, 1)]
    static void Create(MenuCommand menuCommand)
    {
        // Create a custom game object
        //GameObject go = new GameObject("Custom Game Object");
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Espaçadores/Espaçador Linear.prefab", typeof(GameObject));
        GameObject go = Instantiate(prefab);
        go.name = "Novo Espaçador Linear";

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
#endif
}
