using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject Mesh;

    MapDisplay display;

    public bool autoUpdate;
    public int Width = 256;
    public int Height = 256;
    public int Scale = 50;

    public float freq = 0.5f;
    public int octaves = 4;
    public float lacunarity = 2;
    public float gain = .5f;
    public float perturb = 2f;
    public float sharpness = .7f;
    public float amplify = .7f;
    public float altErode = .7f;
    public float ridgeErode = .7f;
    public float slopeErode = .7f;
    public float periodX = .1f;
    public float periodY = .1f;
    public float warp = .5f;
    public float amp = .5f;

    public ComputeShader Billow;
    public ComputeShader HRMF;
    public ComputeShader Erosion;
    public ComputeShader ThermalErosion;
    public ComputeShader thermalErosion2;
    public ComputeShader UberNoise;
    public ComputeShader FBM;
    public ComputeShader RMF;
    public ComputeShader MultiFractal;
    public ComputeShader WindErosion;

    float[] testheight;
    float[] expArray;
    float[] correctScale;
    float[] correctBias;
    Vector3[] windMap;
    int[] windSpeed;
    MapData testData;

    private int maxNoiseValue = 20;

    public float amplitude = 0;
    public float tanAngle = 0.6f;

    public int numErosionIterations = 50000;
    public int erosionBrushRadius = 3;

    public int thermalIteration = 10;
    public int iterations = 50;
    public int diffStep = 1;
    public int projStep = 2;

    public int maxLifetime = 30;
    public float sedimentCapacityFactor = 3;
    public float minSedimentCapacity = .1f;
    public float depositSpeed = .3f;
    public float erodeSpeed = 0.3f;
    public float erosionRate = 0.15f;

    public float evaporateSpeed = 0.1f;
    public float gravity = 4;
    public float startSpeed = 1;
    public float startWater = 1;
    public float inertia = 0.3f;
    public float slope = 0.7f;

    public float angleBias = 0.1f;
    public float angleCoeff = 0.8f;
    public float timeScale = 1f;

    public Stopwatch timer;

    public WindEroder winderode;


    // Start is called before the first frame update
    void Start()
    {
        display = FindObjectOfType<MapDisplay>();
        timer = new Stopwatch();
        timer.Start();
        Generate();
        timer.Stop();
        UnityEngine.Debug.Log(timer);

    }

    public void Generate()
    {
        //display = FindObjectOfType<MapDisplay>();
        timer = new Stopwatch();
        timer.Start();
        System.Diagnostics.Stopwatch.StartNew();
        expArray = new float[maxNoiseValue];
        correctScale = new float[maxNoiseValue];
        correctBias = new float[maxNoiseValue];
        testheight = new float[Width * Height];


        //BillowWeights();
        //GenerateBillow(testheight,expArray,correctScale,correctBias);

        //HRMFWeights();
        //GenerateHRMF(testheight, expArray, correctScale, correctBias);

        //RMFWeights();
        //GenerateRMF(testheight, expArray, correctScale, correctBias);

        //MultiFractalWeights();
        //GenerateMultiFractal(testheight, expArray, correctScale, correctBias);

        //FBMWeights();
        //GenerateFBM(testheight, expArray, correctScale, correctBias);

        //GenerateUber(testheight);
        //clampValues(testheight);


        //LoadTiles();       
        //for (int i = 0; i < iterations; i++)
        //{
        //   thermalErosion(testheight);
        //}
        //hydraulicErosion(testheight);
        display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        timer.Stop();
        UnityEngine.Debug.Log("Generation Time: " + timer.ElapsedMilliseconds + " ms") ;
    }
    void Update()
    {
        //Mesh.transform.Rotate(0, 5 * Time.deltaTime, 0);

        if (Input.GetKeyDown("1"))
        {
            expArray = new float[maxNoiseValue];
            correctScale = new float[maxNoiseValue];
            correctBias = new float[maxNoiseValue];
            testheight = new float[Width * Height];
            FBMWeights();
            GenerateFBM(testheight, expArray, correctScale, correctBias);
            clampValues(testheight);
            LoadTiles();
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }
        if (Input.GetKeyDown("2"))
        {
            expArray = new float[maxNoiseValue];
            correctScale = new float[maxNoiseValue];
            correctBias = new float[maxNoiseValue];
            testheight = new float[Width * Height];
            BillowWeights();
            GenerateBillow(testheight,expArray,correctScale,correctBias);
            clampValues(testheight);
            LoadTiles();
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }
        if (Input.GetKeyDown("3"))
        {
            expArray = new float[maxNoiseValue];
            correctScale = new float[maxNoiseValue];
            correctBias = new float[maxNoiseValue];
            testheight = new float[Width * Height];
            RMFWeights();
            GenerateRMF(testheight, expArray, correctScale, correctBias);
            clampValues(testheight);
            LoadTiles();
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }
        if (Input.GetKeyDown("4"))
        {
            expArray = new float[maxNoiseValue];
            correctScale = new float[maxNoiseValue];
            correctBias = new float[maxNoiseValue];
            testheight = new float[Width * Height];
            MultiFractalWeights();
            GenerateMultiFractal(testheight, expArray, correctScale, correctBias);
            clampValues(testheight);
            LoadTiles();
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }
        if (Input.GetKeyDown("5"))
        {
            expArray = new float[maxNoiseValue];
            correctScale = new float[maxNoiseValue];
            correctBias = new float[maxNoiseValue];
            testheight = new float[Width * Height];
            HRMFWeights();
            GenerateHRMF(testheight, expArray, correctScale, correctBias);
            clampValues(testheight);
            LoadTiles();
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }
        if (Input.GetKeyDown("6"))
        {
            for (int i = 0; i < iterations; i++)
            {
               thermalErosion(testheight);
            }
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }
        if (Input.GetKeyDown("7"))
        {
            hydraulicErosion(testheight);
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }
        if (Input.GetKeyDown("8"))
        {
            for (int i = 0; i < iterations; i++)
            {
                windErosion(testheight);
            }
            display.DrawMesh(MeshGeneration.GenerateTerrainMesh(testheight, Width, Height, Scale), TextureGenerator.TextureFromHeightMap(testheight, Width, Height));
        }


    }
    public void HRMFWeights()
    {

        for (int i = 0; i < maxNoiseValue; i++)
        {
            expArray[i] = (float)System.Math.Pow((double)lacunarity, (double)-i * 0.25f);
        }
        float a = -1;
        float b = 1;
        float minVal = .7f - 1.0f;
        float maxVal = 0.7f + 1.0f;
        float WeightMin = 1.0f * minVal;
        float WeightMax = 1.0f * maxVal;

        float scale = (b - a) / (maxVal - minVal);
        float bias = a - minVal * scale;
        correctScale[0] = scale;
        correctBias[0] = bias;

        for (int i = 0; i < maxNoiseValue; i++)
        {
            if (WeightMin > 1.0f) WeightMin = 1.0f;
            if (WeightMax < 1.0f) WeightMax = 1.0f;

            float signal = (.7f - 1.0f) * expArray[i];
            minVal += signal * WeightMin;
            WeightMin *= 1.0f * signal;

            signal = (0.7f + 1) * expArray[i];
            maxVal += signal * WeightMax;
            WeightMax *= 1.0f * signal;

            scale = (b - a) / (maxVal - minVal);
            bias = a - minVal * scale;
            correctScale[i] = scale;
            correctBias[i] = bias;
        }
    }
    public void GenerateHRMF(float[] map, float[] exp, float[] scale, float[] bias)
    {
        int seedx = Random.Range(0, 50);
        int seedy = Random.Range(0, 50);
        int seedz = Random.Range(0, 50);
        int seedw = Random.Range(0, 50);

        int numThreadsx = Width / 8;
        int numThreadsy = Width / 8;
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        ComputeBuffer expBuffer = new ComputeBuffer(exp.Length, sizeof(float));
        ComputeBuffer scaleBuffer = new ComputeBuffer(scale.Length, sizeof(float));
        ComputeBuffer biasBuffer = new ComputeBuffer(bias.Length, sizeof(float));

        mapBuffer.SetData(map);
        expBuffer.SetData(exp);
        scaleBuffer.SetData(scale);
        biasBuffer.SetData(bias);
        HRMF.SetBuffer(0, "map", mapBuffer);
        HRMF.SetBuffer(0, "expArray", expBuffer);
        HRMF.SetBuffer(0, "correctScale", scaleBuffer);
        HRMF.SetBuffer(0, "correctBias", biasBuffer);
        HRMF.SetInt("Width", Width);
        HRMF.SetInt("Height", Height);
        HRMF.SetInt("octaves", octaves);
        HRMF.SetFloat("lacunarity", lacunarity);
        HRMF.SetFloat("frequency", freq);
        HRMF.SetInt("seedx", seedx);
        HRMF.SetInt("seedy", seedy);
        HRMF.SetInt("seedz", seedz);
        HRMF.SetInt("seedw", seedw);
        HRMF.Dispatch(0, numThreadsx, numThreadsy, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
        expBuffer.Release();
        scaleBuffer.Release();
        biasBuffer.Release();
    }
    public void FBMWeights()
    {

        for (int i = 0; i < maxNoiseValue; i++)
        {
            expArray[i] = (float)System.Math.Pow((double)lacunarity, (double)-i * 1);
        }
        float minVal = 0.0f;
        float maxVal = 0.0f;

        for (int i = 0; i < maxNoiseValue; i++)
        {
            minVal += -1 * expArray[i];
            maxVal += 1 * expArray[i];
            const float a = -1.0f;
            const float b = 1.0f;
            float scale = (b - a) / (maxVal - minVal);
            float bias = a - minVal * scale;
            correctScale[i] = scale;
            correctBias[i] = bias;
        }
    }
    public void GenerateFBM(float[] map, float[] exp, float[] scale, float[] bias)
    {
        int seedx = Random.Range(0, 50);
        int seedy = Random.Range(0, 50);
        int seedz = Random.Range(0, 50);
        int seedw = Random.Range(0, 50);
        int numThreadsx = Width / 8;
        int numThreadsy = Width / 8;
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        ComputeBuffer expBuffer = new ComputeBuffer(exp.Length, sizeof(float));
        ComputeBuffer scaleBuffer = new ComputeBuffer(scale.Length, sizeof(float));
        ComputeBuffer biasBuffer = new ComputeBuffer(bias.Length, sizeof(float));

        mapBuffer.SetData(map);
        expBuffer.SetData(exp);
        scaleBuffer.SetData(scale);
        biasBuffer.SetData(bias);
        FBM.SetBuffer(0, "map", mapBuffer);
        FBM.SetBuffer(0, "expArray", expBuffer);
        FBM.SetBuffer(0, "correctScale", scaleBuffer);
        FBM.SetBuffer(0, "correctBias", biasBuffer);
        FBM.SetInt("Width", Width);
        FBM.SetInt("seedx", seedx);
        FBM.SetInt("seedy", seedy);
        FBM.SetInt("seedz", seedz);
        FBM.SetInt("seedw", seedw);
        FBM.SetInt("Height", Height);
        FBM.SetInt("octaves", octaves);
        FBM.SetFloat("lacunarity", lacunarity);
        FBM.SetFloat("frequency", freq);
        FBM.Dispatch(0, numThreadsx, numThreadsy, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
        expBuffer.Release();
        scaleBuffer.Release();
        biasBuffer.Release();
    }
    public void RMFWeights()
    {

        for (int i = 0; i < maxNoiseValue; i++)
        {
            expArray[i] = (float)System.Math.Pow((double)lacunarity, (double)-i * .9f);
        }
        float minVal = 0.0f;
        float maxVal = 0.0f;

        for (int i = 0; i < maxNoiseValue; i++)
        {
            minVal += 0 * expArray[i];
            maxVal += 1 * expArray[i];
            const float a = -1.0f;
            const float b = 1.0f;
            float scale = (b - a) / (maxVal - minVal);
            float bias = a - minVal * scale;
            correctScale[i] = scale;
            correctBias[i] = bias;
        }
    }
    public void GenerateRMF(float[] map, float[] exp, float[] scale, float[] bias)
    {
        int seedx = 1;// Random.Range(0, 50);
        int seedy = 1;// Random.Range(0, 50);
        int seedz = 1;// Random.Range(0, 50);
        int seedw = 1;// Random.Range(0, 50);
        int numThreadsx = Width / 8;
        int numThreadsy = Width / 8;
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        ComputeBuffer expBuffer = new ComputeBuffer(exp.Length, sizeof(float));
        ComputeBuffer scaleBuffer = new ComputeBuffer(scale.Length, sizeof(float));
        ComputeBuffer biasBuffer = new ComputeBuffer(bias.Length, sizeof(float));

        mapBuffer.SetData(map);
        expBuffer.SetData(exp);
        scaleBuffer.SetData(scale);
        biasBuffer.SetData(bias);
        RMF.SetBuffer(0, "map", mapBuffer);
        RMF.SetBuffer(0, "expArray", expBuffer);
        RMF.SetBuffer(0, "correctScale", scaleBuffer);
        RMF.SetBuffer(0, "correctBias", biasBuffer);
        RMF.SetInt("Width", Width);
        RMF.SetInt("seedx", seedx);
        RMF.SetInt("seedy", seedy);
        RMF.SetInt("seedz", seedz);
        RMF.SetInt("seedw", seedw);
        RMF.SetInt("Height", Height);
        RMF.SetInt("octaves", octaves);
        RMF.SetFloat("lacunarity", lacunarity);
        RMF.SetFloat("frequency", freq);
        RMF.Dispatch(0, numThreadsx, numThreadsy, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
        expBuffer.Release();
        scaleBuffer.Release();
        biasBuffer.Release();
    }
    public void MultiFractalWeights()
    {

        for (int i = 0; i < maxNoiseValue; i++)
        {
            expArray[i] = (float)System.Math.Pow((double)lacunarity, (double)-i * 1);
        }
        float minVal = 1.0f;
        float maxVal = 1.0f;

        for (int i = 0; i < maxNoiseValue; i++)
        {
            minVal += -1 * expArray[i];
            maxVal += 1 * expArray[i];
            const float a = -1.0f;
            const float b = 1.0f;
            float scale = (b - a) / (maxVal - minVal);
            float bias = a - minVal * scale;
            correctScale[i] = scale;
            correctBias[i] = bias;
        }
    }
    public void GenerateMultiFractal(float[] map, float[] exp, float[] scale, float[] bias)
    {
        int seedx = Random.Range(0, 50);
        int seedy = Random.Range(0, 50);
        int seedz = Random.Range(0, 50);
        int seedw = Random.Range(0, 50);
        int numThreadsx = Width / 8;
        int numThreadsy = Width / 8;
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        ComputeBuffer expBuffer = new ComputeBuffer(exp.Length, sizeof(float));
        ComputeBuffer scaleBuffer = new ComputeBuffer(scale.Length, sizeof(float));
        ComputeBuffer biasBuffer = new ComputeBuffer(bias.Length, sizeof(float));

        mapBuffer.SetData(map);
        expBuffer.SetData(exp);
        scaleBuffer.SetData(scale);
        biasBuffer.SetData(bias);
        MultiFractal.SetBuffer(0, "map", mapBuffer);
        MultiFractal.SetBuffer(0, "expArray", expBuffer);
        MultiFractal.SetBuffer(0, "correctScale", scaleBuffer);
        MultiFractal.SetBuffer(0, "correctBias", biasBuffer);
        MultiFractal.SetInt("Width", Width);
        MultiFractal.SetInt("seedx", seedx);
        MultiFractal.SetInt("seedy", seedy);
        MultiFractal.SetInt("seedz", seedz);
        MultiFractal.SetInt("seedw", seedw);
        MultiFractal.SetInt("Height", Height);
        MultiFractal.SetInt("octaves", octaves);
        MultiFractal.SetFloat("lacunarity", lacunarity);
        MultiFractal.SetFloat("frequency", freq);
        MultiFractal.Dispatch(0, numThreadsx, numThreadsy, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
        expBuffer.Release();
        scaleBuffer.Release();
        biasBuffer.Release();
    }
    public void BillowWeights()
    {

        for (int i = 0; i < maxNoiseValue; i++)
        {
            expArray[i] = (float)System.Math.Pow((double)lacunarity, (double)-i * 1.0);
        }
        float minVal = 0.0f;
        float maxVal = 0.0f;

        for (int i = 0; i < maxNoiseValue; i++)
        {
            minVal += -1 * expArray[i];
            maxVal += 1 * expArray[i];
            const float a = -1.0f;
            const float b = 1.0f;
            float scale = (b - a) / (maxVal - minVal);
            float bias = a - minVal * scale;
            correctScale[i] = scale;
            correctBias[i] = bias;
        }
    }
    public void GenerateBillow(float[] map, float[] exp, float[] scale, float[] bias)
    {
        int seedx = 1;//Random.Range(0, 50);
        int seedy = 1;// Random.Range(0, 50);
        int seedz = 1;// Random.Range(0, 50);
        int seedw = 1;// Random.Range(0, 50);
        int numThreadsx = Width / 8;
        int numThreadsy = Width / 8;
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        ComputeBuffer expBuffer = new ComputeBuffer(exp.Length, sizeof(float));
        ComputeBuffer scaleBuffer = new ComputeBuffer(scale.Length, sizeof(float));
        ComputeBuffer biasBuffer = new ComputeBuffer(bias.Length, sizeof(float));

        mapBuffer.SetData(map);
        expBuffer.SetData(exp);
        scaleBuffer.SetData(scale);
        biasBuffer.SetData(bias);
        Billow.SetBuffer(0, "map", mapBuffer);
        Billow.SetBuffer(0, "expArray", expBuffer);
        Billow.SetBuffer(0, "correctScale", scaleBuffer);
        Billow.SetBuffer(0, "correctBias", biasBuffer);
        Billow.SetInt("Width", Width);
        Billow.SetInt("seedx", seedx);
        Billow.SetInt("seedy", seedy);
        Billow.SetInt("seedz", seedz);
        Billow.SetInt("seedw", seedw);
        Billow.SetInt("Height", Height);
        Billow.SetInt("octaves", octaves);
        Billow.SetFloat("lacunarity", lacunarity);
        Billow.SetFloat("frequency", freq);
        Billow.Dispatch(0, numThreadsx, numThreadsy, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
        expBuffer.Release();
        scaleBuffer.Release();
        biasBuffer.Release();
    }
    public void GenerateUber(float[] map)
    {
        float seedx = Random.Range(0.001f, 1f);
        float seedy = Random.Range(0.001f, 1f);
        float seedz = Random.Range(0.001f, 1f);
        float seedw = Random.Range(0.001f, 1f);

        int numThreadsx = Width / 8;
        int numThreadsy = Width / 8;
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        

        mapBuffer.SetData(map);
        UberNoise.SetBuffer(0, "map", mapBuffer);
        UberNoise.SetInt("Width", Width);
        UberNoise.SetInt("Height", Height);
        UberNoise.SetFloat("Freq", freq);
        UberNoise.SetInt("Octaves", octaves);
        UberNoise.SetFloat("Lacunarity", lacunarity);
        UberNoise.SetFloat("Gain", gain);
        UberNoise.SetFloat("Perturb", perturb);
        UberNoise.SetFloat("Sharpness", sharpness);
        UberNoise.SetFloat("Amplify", amplify);
        UberNoise.SetFloat("AltitudeErosion", altErode);
        UberNoise.SetFloat("RidgeErosion", ridgeErode);
        UberNoise.SetFloat("SlopeErosion", slopeErode);
        UberNoise.SetFloat("PeriodX", periodX);
        UberNoise.SetFloat("PeriodY", periodY);
        UberNoise.SetFloat("seedx", seedx);
        UberNoise.SetFloat("seedy", seedx);
        UberNoise.SetFloat("seedz", seedx);
        UberNoise.SetFloat("seedw", seedx);
        UberNoise.Dispatch(0, numThreadsx, numThreadsy, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
    }
    public void hydraulicErosion(float[] map)
    {
        int numThreads = numErosionIterations / 1024;
    
        //Create Brush List
        List<int> brushIndexOffsets = new List<int>();
        List<float> brushWeights = new List<float>();
    
        float weightSum = 0;
        for (int brushY = -erosionBrushRadius; brushY <= erosionBrushRadius; brushY++)
        {
            for (int brushX = -erosionBrushRadius; brushX <= erosionBrushRadius; brushX++)
            {
                float sqrDst = brushX * brushX + brushY * brushY;
                if (sqrDst < erosionBrushRadius * erosionBrushRadius)
                {
                    brushIndexOffsets.Add(brushY * Width + brushX);
                    float brushWeight = 1 - Mathf.Sqrt(sqrDst) / erosionBrushRadius;
                    weightSum += brushWeight;
                    brushWeights.Add(brushWeight);
                }
            }
        }
    
        for (int i = 0; i < brushWeights.Count; i++)
        {
            brushWeights[i] /= weightSum;
        }
    
        ComputeBuffer brushIndexBuffer = new ComputeBuffer(brushIndexOffsets.Count, sizeof(int));
        ComputeBuffer brushWeightBuffer = new ComputeBuffer(brushWeights.Count, sizeof(int));
        brushIndexBuffer.SetData(brushIndexOffsets);
        brushWeightBuffer.SetData(brushWeights);
        Erosion.SetBuffer(0, "brushIndices", brushIndexBuffer);
        Erosion.SetBuffer(0, "brushWeights", brushWeightBuffer);
    
        int[] randomIndices = new int[numErosionIterations];
        for (int i = 0; i < numErosionIterations; i++)
        {
            int randomX = Random.Range(erosionBrushRadius, Width + erosionBrushRadius);
            int randomY = Random.Range(erosionBrushRadius, Width + erosionBrushRadius);
            randomIndices[i] = randomY * Width + randomX;
        }
    
        ComputeBuffer randomIndexBuffer = new ComputeBuffer(randomIndices.Length, sizeof(int));
        randomIndexBuffer.SetData(randomIndices);
        Erosion.SetBuffer(0, "randomIndices", randomIndexBuffer);
    
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        mapBuffer.SetData(map);
        Erosion.SetBuffer(0, "map", mapBuffer);
        Erosion.SetInt("borderSize", erosionBrushRadius);
        Erosion.SetInt("mapSize", Width);
        Erosion.SetInt("brushLength", brushIndexOffsets.Count);
        Erosion.SetInt("maxLifetime", maxLifetime);
        Erosion.SetFloat("inertia", inertia);
        Erosion.SetFloat("sedimentCapacityFactor", sedimentCapacityFactor);
        Erosion.SetFloat("minSedimentCapacity", minSedimentCapacity);
        Erosion.SetFloat("depositSpeed", depositSpeed);
        Erosion.SetFloat("erodeSpeed", erodeSpeed);
        Erosion.SetFloat("evaporateSpeed", evaporateSpeed);
        Erosion.SetFloat("gravity", gravity);
        Erosion.SetFloat("startSpeed", startSpeed);
        Erosion.SetFloat("startWater", startWater);
        // Run compute shader
        Erosion.Dispatch(0, numThreads, 1, 1);
        mapBuffer.GetData(map);
    
        // Release buffers
        mapBuffer.Release();
        randomIndexBuffer.Release();
        brushIndexBuffer.Release();
        brushWeightBuffer.Release();
    
    }
    public void thermalErosion(float[] map)
    {
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));

        mapBuffer.SetData(map);
        ThermalErosion.SetBuffer(0, "map", mapBuffer);
        ThermalErosion.SetInt("gridSize", Width);
        ThermalErosion.SetFloat("amplitude", amplitude);
        ThermalErosion.SetFloat("cellSize", 1 / Width);
        ThermalErosion.SetFloat("tanAngle", tanAngle);
        ThermalErosion.Dispatch(0, Width / 8, Width / 8, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
    }
    public void windErosion(float[] map)
    {
        GenerateWindMap();
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        ComputeBuffer windBuffer = new ComputeBuffer(windMap.Length, sizeof(float)* 3);

        mapBuffer.SetData(map);
        WindErosion.SetBuffer(0, "map", mapBuffer);
        windBuffer.SetData(windMap);
        WindErosion.SetBuffer(0, "windMap", windBuffer);
        WindErosion.SetInt("gridSize", Width);
        WindErosion.SetFloat("amplitude", amplitude);
        WindErosion.SetFloat("cellSize", 1 / Width);
        WindErosion.Dispatch(0, Width / 8, Width / 8, 1);
        mapBuffer.GetData(map);
        mapBuffer.Release();
        windBuffer.GetData(windMap);
        windBuffer.Release();
    }
    public void GenerateWindMap()
    {
        windMap = new Vector3[Width * Height];
        int newx = 0;
        int newy = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (x > Width/2)
                {
                    newx = -1;
                }
                if (x <= Width/2)
                {
                    newx = 1;
                }
                if (y > Height/2)
                {
                    newy = -1;
                }
                if (y <= Height / 2)
                {
                    newy = 1;
                }
                windMap[x * Width + y].x = newx;
                windMap[x * Width + y].y = newy;
                windMap[x * Width + y].z = 1;
            }
        }
    }
    void clampValues(float[] map)
    {
        testData = new MapData(Width, Height);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float value = map[y * Width + x];
                if (value < testData.Min)
                {
                    testData.Min = value;
                }
                if (value > testData.Max)
                {
                    testData.Max = value;
                }

                testData.Data[y * Width + x] = value;
            }
        }
    }
    private void LoadTiles()
    {

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {



                float tvalue = testData.Data[y*Width+x];
                tvalue = (tvalue - testData.Min) / (testData.Max - testData.Min);


                testheight[y * Width + x] = tvalue;
            }
        }
    }
}

public class MapData
{

    public float[] Data;
    public float Min { get; set; }
    public float Max { get; set; }

    public MapData(int width, int height)
    {
        Data = new float[width * height];
        Min = float.MaxValue;
        Max = float.MinValue;
    }
}