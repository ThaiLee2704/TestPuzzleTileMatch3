using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayManagerV2 : MonoBehaviour
{
    public static TrayManagerV2 Instance;
    public List<Transform> slots;

    public List<TileController> tiles = new List<TileController>();
    private Queue<IEnumerator> match3Queue = new Queue<IEnumerator>();

    private void Awake()
    {
        Instance = this;
        StartCoroutine(ProcessMatch3Queue());
    }

    IEnumerator ProcessMatch3Queue()
    {
        while (true)
        {
            if (match3Queue.Count > 0)
                yield return StartCoroutine(match3Queue.Dequeue());
            else
                yield return null;
        }
    }

    public Transform PushToSlot(TileController newTile)
    {
        int insertIndex = tiles.Count;

        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            if (tiles[i].ID == newTile.ID)
            {
                insertIndex = i + 1;
                break;
            }
        }

        tiles.Insert(insertIndex, newTile);

        for (int i = insertIndex + 1; i < tiles.Count; i++)
        {
            tiles[i].Slide(slots[i]);
        }

        //Ngữ cảnh: 3 Tile sam ID bay lên, IEMatch3 chỉ chờ moveDuration của Tile đầu tiên
        //tile đầu tiên vào đươc ô slot thì nó chạy hàm EatMatch3() luôn chứ không chờ tile thứ 2, 3 bay lên
        //Fix: Không lấy moveDuration của tile thứ 1 và 2 chỉ khi nó đủ 3 tile same ID thì mới lấy
        //moveDuration của tile thứ 3 để chờ cho tile thứ 3 bay lên trước khi chạy hàm EatMatch3()
        int countOfSameID = 0;
        foreach (var tile in tiles)
        {
            if (tile.ID == newTile.ID)
                countOfSameID++;
        }

        if (countOfSameID >= 3)
            match3Queue.Enqueue(IEMatch3(newTile));

        return slots[insertIndex];
    }

    private IEnumerator IEMatch3(TileController tile)
    {
        yield return null; // Wait for 1 frame to allow tile to call MoveToSlot 
        yield return null; // Đợi 1 frame nữa để tile thứ 4 cùng id chèn sau tile thứ 3 cùng id trước khi tile thứ 3 đó bị Match3
        yield return new WaitForSeconds(tile.moveDuration); // Wait for the tile to finish moving to the slot
        EatMatch3();
    }

    public void EatMatch3()
    {
        int count = 1;

        for (int i = 0; i < tiles.Count - 1; i++)
        {
            if (tiles[i].ID == tiles[i + 1].ID)
            {
                count++;
                if (count == 3)
                {
                    //Remove 
                    Destroy(tiles[i - 1].gameObject);
                    Destroy(tiles[i].gameObject);
                    Destroy(tiles[i + 1].gameObject);

                    //Xóa từ to đến nhỏ để bảo toàn Index
                    tiles.RemoveAt(i + 1);
                    tiles.RemoveAt(i);
                    tiles.RemoveAt(i - 1);

                    for (int j = i - 1; j < tiles.Count; j++)
                    {
                        tiles[j].Slide(slots[j]);
                    }

                    break;
                }
            }
            else
                count = 1;
        }
    }
}
