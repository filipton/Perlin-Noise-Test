using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CubeSaver : MonoBehaviour
{
    public List<Block> ChangedBlocks = new List<Block>();

    public ChunkGenerator Generator;
    public static CubeSaver Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void SaveChangedBlock(Block block)
    {
        int blockIndex = ChangedBlocks.FindIndex(x => x.x == block.x && x.y == block.y && x.z == block.z);

        if (blockIndex > -1)
        {
            Block b = ChangedBlocks[blockIndex];
            ChangedBlocks[blockIndex] = new Block(block.BlockID, b.x, b.y, b.z);
        }
        else
        {
            ChangedBlocks.Add(block);
        }
    }

    public void SaveBlocks()
    {
        string Save_File = string.Empty;

        Save_File += Generator.Seed + Environment.NewLine;

        foreach(Block b in ChangedBlocks)
        {
            Save_File += $"{b.BlockID}:{b.x}:{b.y}:{b.z};";
        }

        Save_File = Save_File.Remove(Save_File.Length - 1);

        File.WriteAllText(SaveManager.Instance.GetSaveFilePath(SaveManager.Instance.ActualSaveName), Save_File);
    }

    public void LoadBlocks()
    {
        string Saved_File = File.ReadAllText(SaveManager.Instance.GetSaveFilePath(SaveManager.Instance.ActualSaveName));
        Saved_File = Saved_File.Replace(Generator.Seed + Environment.NewLine, "");
        string[] Saved_Blocks = Saved_File.Split(';');

        foreach(string SB in Saved_Blocks)
        {
            ParseStringToBlock(SB, true);
        }

        foreach (Block b in ChangedBlocks.ToArray())
        {
            if(!b.Equals(new Block()))
            {
                Vector3 ChunkCoords = Generator.GetChunkCords(b.x, b.y, b.z);
                Chunk c = Generator.Chunks.Find(u => u.ChunkX == ChunkCoords.x && u.ChunkY == ChunkCoords.y && u.ChunkZ == ChunkCoords.z);
                if(c == null)
                {
                    GameObject chunkI = Instantiate(Generator.ChunkPrefab, Generator.ChunksParent.transform);
                    chunkI.name = $"Chunk ({ChunkCoords.x}, {ChunkCoords.y}, {ChunkCoords.z}) (With Loaded Cubes)";

                    Chunk chunk = chunkI.GetComponent<Chunk>();
                    chunk.Generator = Generator;

                    Generator.Chunks.Add(chunk);

                    //chunk.GenerateChunk(Generator.GenerateChunkAt(x, y, z, chunk, Generator.chunkSize));
                    chunk.ChunkX = (int)ChunkCoords.x;
                    chunk.ChunkY = (int)ChunkCoords.y;
                    chunk.ChunkZ = (int)ChunkCoords.z;

                    chunk.SetBlock(b.x, b.y, b.z, b.BlockID);
                }
                else
                {
                    c.SetBlock(b.x, b.y, b.z, b.BlockID);
                }
            }
        }
    }

    public static int GetSeedFromSaveFile()
    {
        return int.Parse(File.ReadAllLines(SaveManager.Instance.GetSaveFilePath(SaveManager.Instance.ActualSaveName))[0]);
    }

    public Block ParseStringToBlock(string content, bool AddToList = false)
    {
        Block new_block;

        string[] parts = content.Split(':');

        if(parts.Length == 4)
        {
            new_block.BlockID = int.Parse(parts[0]);
            new_block.x = int.Parse(parts[1]);
            new_block.y = int.Parse(parts[2]);
            new_block.z = int.Parse(parts[3]);
            new_block.Visible = true;

            ChangedBlocks.Add(new_block);

            return new_block;
        }

        return new Block();
    }

    private void OnApplicationQuit()
    {
        SaveBlocks();
    }
}