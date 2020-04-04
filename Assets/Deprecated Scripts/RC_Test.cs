using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RC_Test : MonoBehaviour
{
    public Transform destroyHighlight;
    public Transform placeHighlight;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit Hit;
            Ray dir = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if(Physics.Raycast(dir, out Hit, 100f))
            {
                Vector3 hitCoord = new Vector3(Hit.point.x, Hit.point.y, Hit.point.z);
                hitCoord += (new Vector3(Hit.normal.x, Hit.normal.y, Hit.normal.z)) * -0.5f;
                //Hit.collider.GetComponentInParent<Chunk>().DeleteBlock(Mathf.RoundToInt(hitCoord.x), Mathf.RoundToInt(hitCoord.y), Mathf.RoundToInt(hitCoord.z));
            }
        }

        Ray ray = GetComponentInChildren<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        RaycastHit hits;
        if (Physics.Raycast(ray, out hits, 100))
        {
            Chunk chunk = hits.collider.GetComponentInParent<Chunk>();
            if (chunk == null)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 p = hits.point;
                p -= hits.normal / 4;
                p = FloorToInt(p);
                //chunk.DeleteBlock(p.x, p.y, p.z);
                print(p);
            }
        }
    }

    public void BreakBlock(Vector3 pos, Chunk c)
    {
        //c.DeleteBlock(pos.x, pos.y, pos.z);
    }

    protected virtual Vector3Int FloorToInt(Vector3 vector)
    {
        Vector3Int vectorInt = new Vector3Int();
        vectorInt.x = Mathf.FloorToInt(vector.x);
        vectorInt.y = Mathf.FloorToInt(vector.y);
        vectorInt.z = Mathf.FloorToInt(vector.z);
        return vectorInt;
    }

    protected virtual Vector3Int Truncate(Vector3 vector)
    {
        Vector3Int vectorInt = new Vector3Int();
        vectorInt.x = (int)Decimal.Truncate((decimal)vector.x);
        vectorInt.y = (int)Decimal.Truncate((decimal)vector.y);
        vectorInt.z = (int)Decimal.Truncate((decimal)vector.z);
        return vectorInt;
    }

    Vector3 BlockToPlacePos(RaycastHit BlockHit)
    {
        Vector3 BlockPosition = BlockHit.transform.position;
        Vector3 PlayerPosition = gameObject.transform.position;

        if (BlockPosition.y < PlayerPosition.y)
        {
            return new Vector3(BlockPosition.x, BlockPosition.y + 1, BlockPosition.z);
        }
        else if (BlockPosition.y > PlayerPosition.y)
        {
            return new Vector3(BlockPosition.x, BlockPosition.y - 1, BlockPosition.z);
        }
        else if (BlockPosition.x < PlayerPosition.x)
        {
            return new Vector3(BlockPosition.x + 1, BlockPosition.y, BlockPosition.z);
        }
        else if (BlockPosition.x > PlayerPosition.x)
        {
            return new Vector3(BlockPosition.x - 1, BlockPosition.y, BlockPosition.z);
        }
        else if (BlockPosition.z < PlayerPosition.z)
        {
            return new Vector3(BlockPosition.x, BlockPosition.y, BlockPosition.z + 1);
        }
        else if (BlockPosition.z > PlayerPosition.z)
        {
            return new Vector3(BlockPosition.x, BlockPosition.y, BlockPosition.z - 1);
        }
        else
        {
            return new Vector3(BlockPosition.x, BlockPosition.y + 1, BlockPosition.z);
        }
    }
}
