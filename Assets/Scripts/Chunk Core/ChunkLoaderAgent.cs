using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ChunkLoaderAgent : MonoBehaviour
{
    [Header("Rendering Settings")]
    public int ChunkRenderDistance = 12;
    public bool ChunksNeedsToReload = false;

    [Header("References")]
    public ChunkGenerator Generator;
    //public List<Chunk> DynamicLoadedChunks = new List<Chunk>();

    [Header("Percentage")]
    public Slider PercSlider;
    public Text PercText;

    public float MaxChunks;
    public float ActualChunk = 0;

    public float Percentage;

    [Header("After Loading")]
    public FirstPersonController FPS_Controller;
    public CubeSaver CS;

    Stopwatch sw = new Stopwatch();

    private void Start()
    {
        Generator = FindObjectOfType<ChunkGenerator>();

        StartCoroutine(GenerateChunkAgent());
    }

    public IEnumerator GenerateChunkAgent()
    {
        sw.Start();
        MaxChunks = Mathf.Pow(Generator.chunksCount, 3);

        yield return null;

        //Vector3Int ActualChunk = GetChunkCords(transform.position.x, transform.position.y, transform.position.z);

        for (int x = 0; x < Generator.chunksCount; x++)
        {
            for (int y = 0; y < Generator.chunksCount; y++)
            {
                for (int z = 0; z < Generator.chunksCount; z++)
                {
                    if (Generator.Chunks.Find(u => u.ChunkX == x && u.ChunkY == y && u.ChunkZ == z) == null)
                    {
                        GameObject chunkI = Instantiate(Generator.ChunkPrefab, Generator.ChunksParent.transform);
                        chunkI.name = $"Chunk ({x}, {y}, {z}) (With Agent)";

                        Chunk chunk = chunkI.GetComponent<Chunk>();
                        chunk.Generator = Generator;

                        Generator.Chunks.Add(chunk);

                        Generator.GenerateChunkAt(x, y, z, chunk, Generator.chunkSize);
                        chunk.ChunkX = x;
                        chunk.ChunkY = y;
                        chunk.ChunkZ = z;

                        if(chunk.blocksInChunk.Count == 0)
                        {
                            Generator.Chunks.Remove(chunk);
                            Destroy(chunk.gameObject);
                        }

                        ActualChunk++;

                        Percentage = ActualChunk / MaxChunks;

                        PercSlider.value = Percentage;
                        PercText.text = Percentage.ToString("P2");

                        yield return null;
                    }
                }
            }
        }

        AfterLoading();
    }

    void AfterLoading()
    {
        FPS_Controller.enabled = true;
        sw.Stop();
        PercText.text = $"Done! Time: {sw.Elapsed.TotalSeconds}s";

        CS.LoadBlocks();
    }

    private void Update()
    {
        if(ChunksNeedsToReload)
        {
            StartCoroutine(GenerateChunkAgent());
            ChunksNeedsToReload = false;
        }
    }

    public Vector3Int GetChunkCords(float x, float y, float z)
    {
        return new Vector3Int(Mathf.FloorToInt(x / Generator.chunkSize), Mathf.FloorToInt(y / Generator.chunkSize), Mathf.FloorToInt(z / Generator.chunkSize));
    }
}