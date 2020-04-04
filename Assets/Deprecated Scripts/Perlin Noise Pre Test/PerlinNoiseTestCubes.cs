using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PerlinNoiseTestCubes : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public float OffsetX;
    public float OffsetY;

    public float Scale;

    public bool RandomSeed;

    public string Seed;
    public float SeedHash;

    public GameObject[] Cubes;
    public GameObject CubeParent;

    // Start is called before the first frame update
    void Start()
    {
        RecalculateNoise();
        if (RandomSeed)
        {
            Seed = ((ulong)Random.Range(100101021, 748923749832998943)).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RecalculateNoise();
    }

    public void RandomSeedGen()
    {
        if (RandomSeed)
        {
            Seed = ((ulong)Random.Range(100101021, 748923749832998943)).ToString();
        }

        RecalculateNoise();
    }

    public void RecalculateNoise()
    {
        //ScaleX = (2.56f / width);
        //ScaleY = (2.56f / height);

        StartCoroutine(GenerateCubes());
    }

    IEnumerator GenerateCubes()
    {
        int ci = 0;

        //generate perlin noise
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cube = Cubes[ci];
                cube.transform.position = new Vector3(x, GetYPoint(x, y), y);

                ci++;
                yield return new WaitForEndOfFrame();
            }
        }

        print("OPERATION COMPLETE!");
    }

    float GetYPoint(float x, float y)
    {
        float seedHash = (float)(Seed.GetHashCode() * (Seed.GetHashCode() / Seed.Length) / (Seed.GetHashCode() / 69.1653f));
        SeedHash = seedHash;

        x += OffsetX;
        y += OffsetY;

        x *= Scale;
        y *= Scale;

        x /= width;
        y /= height;

        x += seedHash;
        y += seedHash;


        float sample = Mathf.PerlinNoise(x, y);
        return sample;
    }
}