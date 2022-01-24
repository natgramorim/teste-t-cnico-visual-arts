using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the detection of number pieces inside this space
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class TileSensor : MonoBehaviour
{
    private CircleCollider2D myCollider;
    public Vector2Int coordinate;

    private void Awake()
    {
        myCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GrabNRelease grabComponent = collision.gameObject.GetComponent<GrabNRelease>();
        if (grabComponent != null && grabComponent != this)
        {
            grabComponent.currentSpace = this;
            //grabComponent.onBind.Invoke();
            GameManager.I.UpdateTile(grabComponent.GetComponent<NumberTile>().number, coordinate);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GrabNRelease grabComponent = collision.gameObject.GetComponent<GrabNRelease>();
        if (grabComponent != null && grabComponent.currentSpace == this)
        {
            grabComponent.currentSpace = null;
            //GameManager.I.UpdateTile(0, coordinate);
        }
    }

    /// <summary>
    /// Returns if a given point its inside of the bound of the collider
    /// </summary>
    public bool IsInside(Vector2 coord) => myCollider.OverlapPoint(coord);
}
