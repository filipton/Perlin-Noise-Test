using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct Block
{
    public int BlockID;
    public bool Visible;

    public float x;
    public float y;
    public float z;

    //simple struct builder
    public Block(int BlockID, float x, float y, float z, bool visible = true)
    {
        this.BlockID = BlockID;

        this.x = x;
        this.y = y;
        this.z = z;
        this.Visible = visible;
    }
}

public class Chunk : MonoBehaviour
{
    public ChunkGenerator Generator;

    public int ChunkX, ChunkY, ChunkZ;

    public List<Block> blocksInChunk = new List<Block>(); //i will replace it by 3 dimension table, without cords

    public bool ChunkNeedsToReload = false;

    public MeshRenderer MeshRenderer;

    private void Start()
    {
        MeshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ChunkNeedsToReload)
        {
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }

            GenerateChunkByListOfBlock();

            ChunkNeedsToReload = false;
        }
    }

    public void GenerateChunk(ChunkMesh chunkMesh)
    {
        Transform container = this.transform;
        GameObject combinedObj = new GameObject("All Combines");

        combinedObj.transform.parent = container;
        combinedObj.layer = 8;

        MeshFilter mf = combinedObj.AddComponent<MeshFilter>();

        MeshRenderer mr = combinedObj.AddComponent<MeshRenderer>();//add mesh renderer component
        List<Material> Mats = new List<Material>();


        //First we need to combine the wood into one mesh and then the leaf into one mesh
        Mesh combinedStoneMesh = new Mesh();
        combinedStoneMesh.CombineMeshes(chunkMesh.Stone.ToArray());

        Mesh combinedIronMesh = new Mesh();
        combinedIronMesh.CombineMeshes(chunkMesh.Iron.ToArray());

        Mesh combinedGoldMesh = new Mesh();
        combinedGoldMesh.CombineMeshes(chunkMesh.Gold.ToArray());

        //Create the array that will form the combined mesh
        List<CombineInstance> totalMesh = new List<CombineInstance>();
        //CombineInstance[] totalMesh = new CombineInstance[3];

        //Add the submeshes in the same order as the material is set in the combined mesh
        if(combinedStoneMesh.vertexCount > 0)
        {
            totalMesh.Add(new CombineInstance() { mesh = combinedStoneMesh, transform = combinedObj.transform.localToWorldMatrix });
            Mats.Add(Generator.Mats[0]);
        }
        if(combinedIronMesh.vertexCount > 0)
        {
            totalMesh.Add(new CombineInstance() { mesh = combinedIronMesh, transform = combinedObj.transform.localToWorldMatrix });
            Mats.Add(Generator.Mats[1]);
        }
        if(combinedGoldMesh.vertexCount > 0)
        {
            totalMesh.Add(new CombineInstance() { mesh = combinedGoldMesh, transform = combinedObj.transform.localToWorldMatrix });
            Mats.Add(Generator.Mats[2]);
        }

        //mr.materials = Mats.ToArray();
        mr.sharedMaterials = Mats.ToArray();

        //Create the final combined mesh
        Mesh combinedAllMesh = new Mesh();

        //Make sure it's set to false to get 2 separate meshes
        combinedAllMesh.CombineMeshes(totalMesh.ToArray(), false);
        combinedObj.GetComponent<MeshFilter>().mesh = combinedAllMesh;

        combinedObj.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
    }

    public void GenerateChunkByListOfBlock()
    {
        List<CombineInstance> StoneblockData = new List<CombineInstance>();//this will contain the data for the final mesh
        List<CombineInstance> IronblockData = new List<CombineInstance>();//this will contain the data for the final mesh
        List<CombineInstance> GoldblockData = new List<CombineInstance>();//this will contain the data for the final mesh
        MeshFilter blockMesh = Instantiate(Generator.CubePrefab, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();


        foreach (Block b in blocksInChunk)
        {
            blockMesh.transform.position = new Vector3(b.x, b.y, b.z);//move the unit cube to the intended position

            if (b.BlockID == 1 && b.Visible)
            {
                CombineInstance ci = new CombineInstance
                {//copy the data off of the unit cube
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix,
                };
                StoneblockData.Add(ci);//add the data to the list
            }
            else if (b.BlockID == 2 && b.Visible)
            {
                CombineInstance ci = new CombineInstance
                {//copy the data off of the unit cube
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix,
                };
                IronblockData.Add(ci);//add the data to the list
            }
            else if (b.BlockID == 3 && b.Visible)
            {
                CombineInstance ci = new CombineInstance
                {//copy the data off of the unit cube
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix,
                };
                GoldblockData.Add(ci);//add the data to the list
            }
        }

        Destroy(blockMesh.gameObject);

        GenerateChunk(new ChunkMesh { Stone = StoneblockData, Iron = IronblockData, Gold = GoldblockData });
    }

    private Block DeleteBlock(float x, float y, float z)
    {
        Block b = blocksInChunk.Find(i => i.x == x && i.y == y && i.z == z);
        blocksInChunk.Remove(b);

        return b;
    }

    public void SetBlock(float x, float y, float z, int blockID)
    {
        DeleteBlock(x, y, z);

        Block b = new Block(blockID, x, y, z);
        blocksInChunk.Add(b);

        CubeSaver.Instance.SaveChangedBlock(b);

        ChunkNeedsToReload = true;
    }

    public void SetBlock(float x, float y, float z, int blockID, out Block block)
    {
        block = DeleteBlock(x, y, z);

        Block b = new Block(blockID, x, y, z);

        if (blockID != 0)
        {
            blocksInChunk.Add(b);
        }

        CubeSaver.Instance.SaveChangedBlock(b);

        ChunkNeedsToReload = true;
    }
}