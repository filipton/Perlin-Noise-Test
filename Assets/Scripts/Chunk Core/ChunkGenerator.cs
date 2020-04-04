using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Generator Settings")]
    public float Radius;
    [Range(-15000000, 15000000)]
    public int Seed;
    public float noiseScale = .05f;
    [Range(0, 1)]
    public float threshold = .45f;

    [Header("Chunk Settings")]
    public int chunksCount = 15;
    public int chunkSize = 8;

    [Header("Breaking Settings")]
    public float MaxDistance = 8;
    public LayerMask BreakLayerMask;

    [Header("Lists")]
    public List<Chunk> Chunks = new List<Chunk>();

    [Header("GameObjects And Other")]
    public Material material;
    public Material material1;
    public Material material2;
    public GameObject CubePrefab;
    public GameObject ChunkPrefab;
    public GameObject ChunksParent;

    // Start is called before the first frame update
    void Start()
    {
        //GenerateChunks();
        //Seed = FindObjectOfType<MainMenuRef>().Seed;
        Seed = CubeSaver.GetSeedFromSaveFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetChunkCords(float x, float y, float z)
    {
        return new Vector3(Mathf.FloorToInt(x / chunkSize), Mathf.FloorToInt(y / chunkSize), Mathf.FloorToInt(z / chunkSize));
    }

    void GenerateChunks()
    {
        for (int x = 0; x < chunksCount; x++)
        {
            for (int y = 0; y < chunksCount; y++)
            {
                for (int z = 0; z < chunksCount; z++)
                {
                    GameObject chunkI = Instantiate(ChunkPrefab, ChunksParent.transform);
                    chunkI.name = $"Chunk ({x}, {y}, {z})";

                    Chunk chunk = chunkI.GetComponent<Chunk>();
                    chunk.Generator = this;

                    Chunks.Add(chunk);

                    GenerateChunkAt(x, y, z, chunk, chunkSize);
                    chunk.ChunkX = x;
                    chunk.ChunkY = y;
                    chunk.ChunkZ = z;
                }
            }
        }
    }

    public void GenerateChunkAt(int xc, int yc, int zc, Chunk chunk, int chunk_size)
    {
        List<CombineInstance> StoneblockData = new List<CombineInstance>();//this will contain the data for the final mesh
        List<CombineInstance> IronblockData = new List<CombineInstance>();//this will contain the data for the final mesh
        List<CombineInstance> GoldblockData = new List<CombineInstance>();//this will contain the data for the final mesh
        MeshFilter blockMesh = Instantiate(CubePrefab, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        for (int x = (xc * chunk_size); x < ((xc + 1) * chunk_size); x++)
        {
            for (int y = (yc * chunk_size); y < ((yc + 1) * chunk_size); y++)
            {
                for (int z = (zc * chunk_size); z < ((zc + 1) * chunk_size); z++)
                {
                    float noiseValue = Perlin3D((x + Seed) * noiseScale, (y + Seed) * noiseScale, (z + Seed) * noiseScale);//get value of the noise at given x, y, and z.
                    if (noiseValue >= threshold)
                    {
                        float raduis = Radius / 2;
                        if (Vector3.Distance(new Vector3(x, y, z), Vector3.one * raduis) > raduis)
                            continue;

                        blockMesh.transform.position = new Vector3(x, y, z);//move the unit cube to the intended position


                        if(noiseValue >= 0.6 && noiseValue <= 0.625 || noiseValue >= 0.55 && noiseValue <= 0.575)
                        {
                            chunk.blocksInChunk.Add(new Block(2, x, y, z));
                        }
                        else if(noiseValue >= 0.5 && noiseValue <= 0.525 || noiseValue >= 0.7 && noiseValue <= 0.725)
                        {
                            chunk.blocksInChunk.Add(new Block(3, x, y, z));
                        }
                        else
                        {
                            chunk.blocksInChunk.Add(new Block(1, x, y, z));
                        }

                        //File.AppendAllText(@"E:\noiseValue.txt", noiseValue.ToString() + Environment.NewLine);
                    }
                }
            }
        }

        chunk.GenerateChunkByListOfBlock();

        Destroy(blockMesh.gameObject);
        
    }

    public float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}

public class ChunkMesh
{
    public List<CombineInstance> Stone = new List<CombineInstance>();
    public List<CombineInstance> Iron = new List<CombineInstance>();
    public List<CombineInstance> Gold = new List<CombineInstance>();
}