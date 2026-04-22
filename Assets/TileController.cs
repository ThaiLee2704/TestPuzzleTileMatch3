using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int ID;
    public float moveDuration = 0f;
    [SerializeField] private float speed = 3;

    private Tween moveTween;

    private void OnMouseDown()
    {
        GetComponent<Collider2D>().enabled = false;

        //moveDuration = 0f;
        var slot = TrayManagerV2.Instance.PushToSlot(this);
        MoveToSlot(slot);
    }

    private void MoveToSlot(Transform slot)
    {
        moveDuration = Vector3.Distance(transform.position, slot.position) / speed;
        moveTween = Tween.PositionAtSpeed(transform, slot.position, speed, Ease.Linear);
        //Debug.LogError("Move duration 1= " + moveDuration);
    }

    public void Slide(Transform slot)
    {
        moveTween.Stop();
        moveTween = Tween.PositionAtSpeed(transform, slot.position, speed, Ease.Linear);
    }

    //public float Slide(Transform slot)
    //{
    //    moveTween.Stop();
    //    moveTween = Tween.PositionAtSpeed(transform, slot.position, speed, Ease.Linear);
    //    return Vector3.Distance(transform.position, slot.position) / speed;
    //}
}
