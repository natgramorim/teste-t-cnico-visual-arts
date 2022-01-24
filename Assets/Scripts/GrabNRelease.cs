using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Allows mouse movement control over the object by grabbing, dragging and releasing
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class GrabNRelease : MonoBehaviour
{
    // Parâmetros
    [SerializeField] private UnityEvent onGrab;
    [SerializeField] private UnityEvent onDrag;
    [SerializeField] private UnityEvent onRelease;
    [SerializeField] private UnityEvent onBind;

    // Componentes e Referências
    private Rigidbody2D myRb;
    private Collider2D myCollider;
    [HideInInspector] public TileSensor currentSpace;

    // Declarações
    private bool isGrabbed;
    private bool isBound;

    private Vector3 mousePos = new Vector3();
    private Vector2 oldPos;
    private Vector2 desiredPos = new Vector2();

    private float dislocThreshold = 0.15f;
    private Vector2 dislocation = new Vector2();
    private Vector2 direction = new Vector2();

    private ContactFilter2D contactFilter;
    private Collider2D[] collision;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        collision = new Collider2D[1];
        contactFilter.useTriggers = false;
    }

    private void FixedUpdate()
    {
        if (isGrabbed) Drag();
    }

    /// <summary>
    /// Grabs the piece and starts moving it with the mouse
    /// </summary>
    public void Grab()
    {
        // Set piece as being dragged
        isGrabbed = true;
        isBound = true;

        // Resets direction
        direction.x = 0; direction.y = 0;

        // Execute On Grab event
        onGrab.Invoke();
    }

    /// <summary>
    /// Releases the piece from mouse control
    /// </summary>
    public void Release()
    {
        // Sets the object as no longer being dragged
        isGrabbed = false;

        // Execute On Release event
        onRelease.Invoke();
    }

    /// <summary>
    /// Updates the position of the object according to mouse movement
    /// </summary>
    private void Drag()
    {
        // Get mouse world position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Register old position
        oldPos = transform.position;

        // Detect how much the mouse has moved
        dislocation.x = mousePos.x - oldPos.x;
        dislocation.y = mousePos.y - oldPos.y;

        // Identify the direction the piece should move, the cartesian axis
        if (direction.sqrMagnitude == 0)
        {
            if (Mathf.Abs(dislocation.x) > Mathf.Abs(dislocation.y) && Mathf.Abs(dislocation.x) >= dislocThreshold)
            {
                direction = new Vector2(1, 0); Debug.Log("eixo x");
            }
            else if (Mathf.Abs(dislocation.y) >= dislocThreshold)
            {
                direction = new Vector2(0, 1); Debug.Log("eixo y");
            }
        }

        // Calculate the desired position
        desiredPos.x = oldPos.x + (dislocation.x/* + mouseOffset.x*/) * direction.x;
        desiredPos.y = oldPos.y + (dislocation.y/* + mouseOffset.y*/) * direction.y;

        // Restrain it inside the bounds of the board
        desiredPos.x = Mathf.Max(desiredPos.x, GameManager.worldBoardBounds[0]);
        desiredPos.x = Mathf.Min(desiredPos.x, GameManager.worldBoardBounds[1]);
        desiredPos.y = Mathf.Max(desiredPos.y, GameManager.worldBoardBounds[2]);
        desiredPos.y = Mathf.Min(desiredPos.y, GameManager.worldBoardBounds[3]);

        // Bind piece to its current tile space if possible
        if (currentSpace != null && currentSpace.IsInside(desiredPos))
        {
            desiredPos = currentSpace.transform.position;

            if (!isBound)
            {
                isBound = true;
                onBind.Invoke();
            }
        }
        else
        {
            isBound = false;
        }

        // Move piece to the desired position
        myRb.position = desiredPos;

        // Check for possible collisions with other pieces
        if (myCollider.OverlapCollider(contactFilter, collision) > 0) myRb.MovePosition(oldPos);

        // Execute On Drag event
        onDrag.Invoke();
    }
}
