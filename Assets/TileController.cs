using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int ID;
    public float moveDuration = 0f;
    float speed = 30;

    private void OnMouseDown()
    {
        moveDuration = 0f;
        var slot = TrayManager.Instance.PushToSlot(this);
        MoveToSlot(slot);
    }

    Tween move;
    void MoveToSlot(Transform slot)
    {
        move = Tween.PositionAtSpeed(transform, slot.position, speed, Ease.Linear);
        moveDuration = Vector3.Distance(transform.position, slot.position) / speed;
        Debug.LogError("Move duration 1= " + moveDuration);
    }

    public float Slide(Transform slot)
    {
        move.Stop();
        move = Tween.PositionAtSpeed(transform, slot.position, speed, Ease.Linear);
        return Vector3.Distance(transform.position, slot.position) / speed;
    }
}
