using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteract : MonoBehaviour
{
    public ChunkGenerator Generator;
    public Inventory Inv;
    public CubeSaver cubeSaver;

    public GameObject BreakBlock;
    public GameObject PlaceBlock;

    public float BreakingTime;
    public float MaxBreakingTime = 1;

    public bool InteractShow = false;

    public static BlockInteract Instance;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        RaycastHit Hit;
        Ray dir = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(dir, out Hit, Generator.MaxDistance, Generator.BreakLayerMask))
        {
            if (!BreakBlock.activeSelf && InteractShow)
            {
                BreakBlock.SetActive(true);
            }
            if (!PlaceBlock.activeSelf && InteractShow)
            {
                PlaceBlock.SetActive(true);
            }

            //break block
            Vector3 hitCoord = new Vector3(Hit.point.x, Hit.point.y, Hit.point.z);
            hitCoord += (new Vector3(Hit.normal.x, Hit.normal.y, Hit.normal.z)) * -0.5f;

            int x = Mathf.RoundToInt(hitCoord.x);
            int y = Mathf.RoundToInt(hitCoord.y);
            int z = Mathf.RoundToInt(hitCoord.z);

            BreakBlock.transform.position = new Vector3(x, y, z);

            //place block
            Vector3 placeCoord = new Vector3(Hit.point.x, Hit.point.y, Hit.point.z);
            placeCoord += (new Vector3(Hit.normal.x, Hit.normal.y, Hit.normal.z)) * 0.5f;

            int px = Mathf.RoundToInt(placeCoord.x);
            int py = Mathf.RoundToInt(placeCoord.y);
            int pz = Mathf.RoundToInt(placeCoord.z);

            PlaceBlock.transform.position = new Vector3(px, py, pz);

            if (Input.GetKey(KeyCode.Mouse0))
            {
                BreakingTime += Time.deltaTime;

                if (BreakingTime >= MaxBreakingTime)
                {
                    Vector3 ChunkCoords = Generator.GetChunkCords(x, y, z);
                    Chunk c = Generator.Chunks.Find(u => u.ChunkX == ChunkCoords.x && u.ChunkY == ChunkCoords.y && u.ChunkZ == ChunkCoords.z);
                    c.SetBlock(x, y, z, 0, out Block b);

                    Inv.AddToInventory(b.BlockID);

                    BreakingTime = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Vector3 ChunkCoords = Generator.GetChunkCords(px, py, pz);
                Chunk c = Generator.Chunks.Find(u => u.ChunkX == ChunkCoords.x && u.ChunkY == ChunkCoords.y && u.ChunkZ == ChunkCoords.z);
                if(c == null)
                {
                    GameObject chunkI = Instantiate(Generator.ChunkPrefab, Generator.ChunksParent.transform);
                    chunkI.name = $"Chunk ({ChunkCoords.x}, {ChunkCoords.y}, {ChunkCoords.z}) (With Placing)";

                    Chunk chunk = chunkI.GetComponent<Chunk>();
                    chunk.Generator = Generator;

                    Generator.Chunks.Add(chunk);

                    chunk.ChunkX = (int)ChunkCoords.x;
                    chunk.ChunkY = (int)ChunkCoords.y;
                    chunk.ChunkZ = (int)ChunkCoords.z;

                    chunk.SetBlock(px, py, pz, 1);
                }
                else
                {
                    c.SetBlock(px, py, pz, 1);
                }
            }
        }
        else
        {
            if (BreakBlock.activeSelf && InteractShow)
            {
                BreakBlock.SetActive(false);
            }
            if (PlaceBlock.activeSelf && InteractShow)
            {
                PlaceBlock.SetActive(false);
            }

            if (BreakingTime != 0)
            {
                BreakingTime = 0;
            }
        }
    }
}