using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PerlinNoiseTest : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public float OffsetX;
    public float OffsetY;

    public float Scale;

    public bool RandomSeed;

    public string Seed;
    public float SeedHash;

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
    }

    public void RecalculateNoise()
    {
        //ScaleX = (2.56f / width);
        //ScaleY = (2.56f / height);

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        //generate perlin noise
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Color color = CalcColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    Color CalcColor(float x, float y)
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
        return new Color(sample, sample, sample);
    }
}