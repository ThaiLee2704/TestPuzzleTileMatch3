using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class TrayManager : MonoBehaviour
{
    public static TrayManager Instance;
    public List<Transform> slots;
    public LinkedList<TileController> tiles;
    Queue<IEnumerator> match3Queue = new Queue<IEnumerator>();


    private void Awake()
    {
        Instance = this;
        tiles = new LinkedList<TileController>();
        StartCoroutine(ProcessMatch3Queue());
    }
    IEnumerator ProcessMatch3Queue()
    {
        while (true)
        {
            if (match3Queue.Count > 0)
            {
                yield return StartCoroutine(match3Queue.Dequeue());
            }
            else
            {
                yield return null;
            }
        }
    }
    public Transform PushToSlot(TileController tile)
    {
        int index = 0;
        Transform slot = null;
        foreach (var item in tiles)
        {
            if (item.ID == tile.ID)
            {
                //insert
                slot = Insert(tile);
                break;
            }
            index++;
        }

        if (slot == null)
        {
            tiles.AddLast(tile);
            slot = slots[index];
        }
        match3Queue.Enqueue(IEMatch3(tile));
        return slot;
    }

    IEnumerator IEMatch3(TileController tile)
    {
        yield return null; // doi 1 frame de tile goi ham MoveToSlot
        yield return null; // doi 1 frame de tile goi ham MoveToSlot
        yield return new WaitForSeconds(tile.moveDuration);
        EatMatch3();
    }

    Transform Insert(TileController tile)
    {
        bool samevalue = false;
        bool slidable = false;
        bool inserted = false;
        Transform slotCanMove = null;
        for (int i = 0; i < tiles.Count;)
        {
            TileController item2 = tiles.ElementAt(i);
            Debug.Log("i = " + i);
            if (slidable)
            {
                item2.Slide(slots[i]);
                Debug.LogError($"Slide id = {item2.ID} to index = {i}", item2.transform);
            }
            else if (item2.ID == tile.ID)
            {
                samevalue = true;
                Debug.LogError($"Same value = true || id = {item2.ID} index = {i}", item2.transform);

            }
            else if (samevalue)
            {
                slidable = true;
                inserted = true;
                slotCanMove = slots[i];
                LinkedListNode<TileController> lastItemSameValue = tiles.Find(item2);
                tiles.AddBefore(lastItemSameValue, tile);
                Debug.LogError($"Insert id = {tile.ID} to index = {i}", tile.transform);
            }
            i++;
        }

        if (!inserted)
        {
            tiles.AddLast(tile);
        }
        Debug.LogError($"slidable = {slidable} || samevalue = {samevalue} || slotCanMove = {slotCanMove}", tile.transform);
        if (!slidable)
        {
            slotCanMove = slots[tiles.Count - 1];
        }
        return slotCanMove;
    }

    void EatMatch3()
    {
        int lastId = -1;
        int count = 0;
        for (int i = 0; i < tiles.Count;)
        {
            TileController item = tiles.ElementAt(i);
            if (item.ID == lastId)
            {
                count++;

                if (count == 3)
                {
                    Debug.LogError($"Eat match 3 || id = {item.ID} index = {i}", item.transform);
                    for (int j = i; j > i - 3; j--)
                    {
                        TileController item2 = tiles.ElementAt(j);
                        Debug.LogError($"Eat id = {item2.ID} index = {j}", item2.transform);
                        tiles.Remove(item2);
                        Destroy(item2.gameObject);
                    }
                    count = 0;
                    for (int j = i - 2; j < tiles.Count; j++)
                    {
                        TileController item2 = tiles.ElementAt(j);
                        item2.Slide(slots[j]);
                        Debug.LogError($"Slide id = {item2.ID} to index = {j} after eat", item2.transform);
                    }
                }
            }
            else
            {
                count = 1;
                lastId = item.ID;
            }
            i++;
        }
    }
}
