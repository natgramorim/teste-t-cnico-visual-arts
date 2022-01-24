using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

/// <summary>
/// Espaça os Games Objects internos entre os quatro pontos de forma regular
/// </summary>
public class EspaçadorRetangular : Espaçador
{
    [Space]
    [SerializeField] private Vector2Int dimensão = new Vector2Int();

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // desenhar a linha entre eles
        Handles.color = Color.red;
        Vector3[] pos1 = { pontos[0].position, pontos[1].position };
        Handles.DrawAAPolyLine(3, pos1);

        Handles.color = Color.green;
        Vector3[] pos2 = { pontos[2].position, pontos[3].position };
        Handles.DrawAAPolyLine(3, pos2);

        Handles.color = Color.yellow;
        Vector3[] pos3 = { pontos[0].position, pontos[2].position };
        Handles.DrawAAPolyLine(3, pos3);

        Handles.color = Color.blue;
        Vector3[] pos4 = { pontos[1].position, pontos[3].position };
        Handles.DrawAAPolyLine(3, pos4);

        Handles.color = Color.white;

        // desenhar os pontos
        Gizmos.DrawSphere(pontos[0].position, 0.2f);

        Gizmos.DrawSphere(pontos[1].position, 0.2f);

        Gizmos.DrawSphere(pontos[2].position, 0.2f);

        Gizmos.DrawSphere(pontos[3].position, 0.2f);
    }
    #endif

    /// <summary>
    /// Atualiza a posição dos game objetos dentro do retângulo
    /// </summary>
    [ContextMenu("Atualizar Posições")]
    public override void UpdatePosition()
    {
        // encontrar porção T entre cada
        float Tx;
        float Ty;

        if (closed)
        {
            Tx = 1f / (dimensão.x - 1);
            Ty = 1f / (dimensão.y - 1);
        }
        else
        {
            Tx = 1f / (dimensão.x / 2);
            Ty = 1f / (dimensão.y / 2);
        }

        // espaçar
        GameObject atual;
        Vector2Int coord = new Vector2Int();

        Vector3 pt1;
        Vector3 pt2;
        Vector3 pt3;
        Vector3 pt4;

        Vector3 md1;
        Vector3 md2;

        for (int i = 0; i < objetos.Count; i++)
        {
            atual = objetos[i];
            coord.x = i % dimensão.x;
            coord.y = Mathf.FloorToInt(i / dimensão.x);

            pt1 = Vector3.Lerp(pontos[0].position, pontos[1].position, Tx * coord.x);
            pt2 = Vector3.Lerp(pontos[2].position, pontos[3].position, Tx * coord.x);
            pt3 = Vector3.Lerp(pontos[0].position, pontos[2].position, Ty * coord.y);
            pt4 = Vector3.Lerp(pontos[1].position, pontos[3].position, Ty * coord.y);

            md1 = Vector3.Lerp(pt1, pt2, Ty * coord.y);
            md2 = Vector3.Lerp(pt3, pt4, Tx * coord.x);

            atual.transform.position = Vector3.Lerp(md1, md2, 0.5f);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Cria um espaçador novo
    /// </summary>
    /// <param name="menuCommand"></param>
    [MenuItem("GameObject/Espaçadores/Espaçador Retangular", false, 2)]
    static void Create(MenuCommand menuCommand)
    {
        // Create a custom game object
        //GameObject go = new GameObject("Custom Game Object");
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Espaçadores/Espaçador Retangular.prefab", typeof(GameObject));
        GameObject go = Instantiate(prefab);
        go.name = "Novo Espaçador Retangular";

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
#endif
}
