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
        #region Create Mesh

        //the creation of the final mesh from the data.

        Transform container = this.transform;//create container object
        GameObject stone = new GameObject("Stone Mesh");//create gameobject for the mesh
        stone.transform.parent = container;//set parent to the container we just made
        MeshFilter stonemf = stone.AddComponent<MeshFilter>();//add mesh component
        MeshRenderer stonemr = stone.AddComponent<MeshRenderer>();//add mesh renderer component
        stonemr.material = Generator.material;//set material to avoid evil pinkness of missing texture
        stonemf.mesh.CombineMeshes(chunkMesh.Stone.ToArray());//set mesh to the combination of all of the blocks in the list
        stone.AddComponent<MeshCollider>().sharedMesh = stonemf.sharedMesh;//setting colliders takes more time. disabled for testing.
        stone.layer = 8;

        GameObject iron = new GameObject("Iron Mesh");//create gameobject for the mesh
        iron.transform.parent = container;//set parent to the container we just made
        MeshFilter ironmf = iron.AddComponent<MeshFilter>();//add mesh component
        MeshRenderer ironmr = iron.AddComponent<MeshRenderer>();//add mesh renderer component
        ironmr.material = Generator.material1;//set material to avoid evil pinkness of missing texture
        ironmf.mesh.CombineMeshes(chunkMesh.Iron.ToArray());//set mesh to the combination of all of the blocks in the list
        iron.AddComponent<MeshCollider>().sharedMesh = ironmf.sharedMesh;//setting colliders takes more time. disabled for testing.
        iron.layer = 8;

        GameObject gold = new GameObject("Gold Mesh");//create gameobject for the mesh
        gold.transform.parent = container;//set parent to the container we just made
        MeshFilter goldmf = gold.AddComponent<MeshFilter>();//add mesh component
        MeshRenderer goldmr = gold.AddComponent<MeshRenderer>();//add mesh renderer component
        goldmr.material = Generator.material2;//set material to avoid evil pinkness of missing texture
        goldmf.mesh.CombineMeshes(chunkMesh.Gold.ToArray());//set mesh to the combination of all of the blocks in the list
        gold.AddComponent<MeshCollider>().sharedMesh = goldmf.sharedMesh;//setting colliders takes more time. disabled for testing.
        gold.layer = 8;

        #endregion
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

            if(b.BlockID == 1 && b.Visible)
            {
                CombineInstance ci = new CombineInstance
                {//copy the data off of the unit cube
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix,
                };
                StoneblockData.Add(ci);//add the data to the list
            }
            else if(b.BlockID == 2 && b.Visible)
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
        blocksInChunk.Add(b);

        CubeSaver.Instance.SaveChangedBlock(b);

        ChunkNeedsToReload = true;
    }
}