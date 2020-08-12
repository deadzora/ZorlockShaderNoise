using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEroder
{
    ComputeShader advection;
    float simScale = 10f;
    float timeScale = .006f;
    float advecVelScale = 10.0f;
    float SuspRate = 22.0f;
    float DeposRate = 10.0f;
    float slopeFactor = 1.0f;
    float dragCoeff = 0.5f;
    float refCoeff = 23.0f;
    Vector4 windVel = new Vector4(1, 1, 1, 1);
    float viscosity = 0.00075f;
    float angleRepose; // TODO: make repose mask material + wetness angles between between 15 and 50
    float hardness; // TODO: make hardness mask materials types hard to soft igneous, metamorphic, sedimentary, clay, soil, sand
    float diffRate = .0001f;
    float jitter = 0.5f;
    float windSpeed = 2.0f;
    



    public void Erode(float[] map, Vector4[] windMask, int[] reposeMask, float[] hardMask, int width, int height, int scale,int thermalIteration, int iterations, int diffStep, int projStep, Vector2 texelSize, ComputeShader advec, 
                      ComputeShader proj, ComputeShader diff, ComputeShader util, 
                      ComputeShader aeol, ComputeShader thermal, ComputeShader arraytext,
                      ComputeShader textarray)
    {
        int advectKernelIdx = advec.FindKernel("Advect");
        int divergenceKernelIdx = proj.FindKernel("Divergence");
        int gradientSubtractKernelIdx = proj.FindKernel("GradientSubtract");
        int diffuseKernelIdx = diff.FindKernel("Diffuse");
        int remapKernelIdx = util.FindKernel("RemapValues");
        int addConstantIdx = util.FindKernel("AddConstant");
        int applyDragKernelIdx = aeol.FindKernel("ApplyHeightfieldDrag");
        int erodeKernelIdx = aeol.FindKernel("WindSedimentErode");
        int thermalKernelIdx = thermal.FindKernel("ThermalErosion");

        int xRes = width;
        int yRes = height;

        RenderTexture heightmapRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        RenderTexture heightmapPrevRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        RenderTexture collisionRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        RenderTexture sedimentPrevRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        RenderTexture sedimentRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);

        RenderTexture windVelRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
        RenderTexture windVelPrevRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
        RenderTexture divergenceRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);

        RenderTexture thermalSedimentRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);

        heightmapRT.enableRandomWrite = true;
        heightmapPrevRT.enableRandomWrite = true;
        collisionRT.enableRandomWrite = true;
        sedimentRT.enableRandomWrite = true;
        sedimentPrevRT.enableRandomWrite = true;
        windVelRT.enableRandomWrite = true;
        windVelPrevRT.enableRandomWrite = true;
        divergenceRT.enableRandomWrite = true;
        thermalSedimentRT.enableRandomWrite = true;

        ComputeBuffer mapBuffer = new ComputeBuffer(map.Length, sizeof(float));
        mapBuffer.SetData(map);
        util.SetInt("width", xRes);
        arraytext.SetTexture(0, "Height", heightmapRT);
        arraytext.SetTexture(0, "prevHeight", heightmapRT);
        arraytext.SetBuffer(0, "Constant", mapBuffer);
        arraytext.Dispatch(0, xRes, yRes, 1);
        mapBuffer.Release();


        float dx = (float)texelSize.x * simScale;
        float dy = (float)texelSize.y * simScale;

        float dxy = Mathf.Sqrt(dx * dx + dy * dy);
        Vector4 dxdy = new Vector4(dx, dy, 1.0f / dx, 1.0f / dy);

        advec.SetFloat("dt", timeScale);
        advec.SetFloat("velScale", advecVelScale);
        advec.SetVector("dxdy", dxdy);
        advec.SetVector("DomainRes", new Vector4((float)xRes, (float)yRes, 1.0f / (float)xRes, 1.0f / (float)yRes));

        diff.SetFloat("dt", timeScale);

        proj.SetVector("dxdy", dxdy);

        aeol.SetFloat("dt", timeScale);
        aeol.SetFloat("suspensionRate", SuspRate);
        aeol.SetFloat("depositRate", DeposRate);
        aeol.SetFloat("Slope", slopeFactor);
        aeol.SetFloat("dragCoeff", dragCoeff);
        aeol.SetFloat("reflectCoeff", refCoeff);
        aeol.SetVector("domainDim", new Vector4((float)xRes, (float)yRes, 0.0f, 0.0f));
        aeol.SetVector("terrainScale", new Vector4(width, height, scale, 0.0f));
        aeol.SetVector("dxdy", dxdy);

        diff.SetVector("texDim", new Vector4((float)width, (float)height, 0.0f, 0.0f));

        for(int i = 0; i < iterations; i++)
        {
            util.SetTexture(addConstantIdx, "OutputTex", windVelPrevRT);
            ComputeBuffer windBuffer = new ComputeBuffer(windMask.Length, sizeof(float)*4);
            windBuffer.SetData(windMask);
            util.SetInt("width", width);
            util.SetBuffer(addConstantIdx, "Constant", windBuffer);
            util.Dispatch(addConstantIdx, xRes, yRes, 1);
            windBuffer.Release();

            aeol.SetTexture(applyDragKernelIdx, "HeightIn", heightmapPrevRT);
            aeol.SetTexture(applyDragKernelIdx, "WindVel", windVelPrevRT);
            aeol.SetTexture(applyDragKernelIdx, "outWindVel", windVelRT);
            aeol.Dispatch(applyDragKernelIdx, xRes, yRes, 1);

            diff.SetFloat("diff", viscosity);

            for (int j = 0; j < diffStep; j++)
            {
                diff.SetTexture(diffuseKernelIdx, "InputTex", windVelRT);
                diff.SetTexture(diffuseKernelIdx, "OutputTex", windVelPrevRT);
                diff.Dispatch(diffuseKernelIdx, xRes, yRes, 1);
            }

            for (int j = 0; j < projStep; j++)
            {
                proj.SetTexture(divergenceKernelIdx, "velocityTex2D", windVelRT);
                proj.SetTexture(divergenceKernelIdx, "divergenceTex2D", divergenceRT);
                proj.Dispatch(divergenceKernelIdx, xRes, yRes, 1);

                proj.SetTexture(gradientSubtractKernelIdx, "pressuretex", divergenceRT);
                proj.SetTexture(gradientSubtractKernelIdx, "velocitytex", windVelRT);
                proj.SetTexture(gradientSubtractKernelIdx, "velocityouttex", windVelPrevRT);
                proj.Dispatch(gradientSubtractKernelIdx, xRes, yRes, 1);
            }

            advec.SetTexture(advectKernelIdx, "inputtex", windVelRT);
            advec.SetTexture(advectKernelIdx, "outputtex", windVelPrevRT);
            advec.SetTexture(advectKernelIdx, "velocitytex", windVelRT);

            advec.Dispatch(advectKernelIdx, xRes, yRes, 1);

            diff.SetFloat("diff", diffRate);
            for (int j = 0; j < diffStep; j++)
            {
                diff.SetTexture(diffuseKernelIdx, "inputTex", sedimentRT);
                diff.SetTexture(diffuseKernelIdx, "OutputTex", sedimentPrevRT);

                diff.Dispatch(diffuseKernelIdx, xRes, yRes, 1);
            }

            advec.SetTexture(advectKernelIdx, "inputtexfloat", sedimentRT);
            advec.SetTexture(advectKernelIdx, "outputtexfloat", sedimentPrevRT);
            advec.SetTexture(advectKernelIdx, "velocitytex", windVelRT);

            advec.Dispatch(advectKernelIdx, xRes, yRes, 1);

            aeol.SetTexture(erodeKernelIdx, "heightin", heightmapPrevRT);
            aeol.SetTexture(erodeKernelIdx, "insediment", sedimentPrevRT);
            aeol.SetTexture(erodeKernelIdx, "windvel", windVelRT);
            aeol.SetTexture(erodeKernelIdx, "outsediment", sedimentRT);
            aeol.SetTexture(erodeKernelIdx, "outheightmap", heightmapRT);
            aeol.Dispatch(erodeKernelIdx, xRes, yRes, 1);

            thermal.SetTexture(thermalKernelIdx, "prevHeight", heightmapPrevRT);
            thermal.SetTexture(thermalKernelIdx, "Height", heightmapRT);
            thermal.SetTexture(thermalKernelIdx, "sediment", thermalSedimentRT);
            ComputeBuffer reposeBuffer = new ComputeBuffer(reposeMask.Length, sizeof(int) );
            reposeBuffer.SetData(reposeMask);
            thermal.SetInt("width", width);
            thermal.SetBuffer(addConstantIdx, "ReposeMap", reposeBuffer); // reposemask
            thermal.SetTexture(thermalKernelIdx, "collision", collisionRT);
            ComputeBuffer hardBuffer = new ComputeBuffer(hardMask.Length, sizeof(float));
            hardBuffer.SetData(hardMask);
            thermal.SetBuffer(addConstantIdx, "hardness", hardBuffer);


            for (int j = 0; j < thermalIteration; j++)
            {

                thermal.Dispatch(thermalKernelIdx, xRes, yRes, 1);
                hardBuffer.Release();
                reposeBuffer.Release();
            }
        }

        textarray.SetTexture(0, "HeightMap", heightmapRT);
        ComputeBuffer finalBuffer = new ComputeBuffer(map.Length, sizeof(float));
        finalBuffer.SetData(map);
        textarray.SetInt("width", width);
        textarray.SetBuffer(0, "map", finalBuffer);
        textarray.Dispatch(0, xRes, yRes, 1);
        finalBuffer.GetData(map);
        finalBuffer.Release();
        RenderTexture.ReleaseTemporary(heightmapRT);
        RenderTexture.ReleaseTemporary(heightmapPrevRT);
        RenderTexture.ReleaseTemporary(collisionRT);
        RenderTexture.ReleaseTemporary(sedimentRT);
        RenderTexture.ReleaseTemporary(sedimentPrevRT);
        RenderTexture.ReleaseTemporary(windVelRT);
        RenderTexture.ReleaseTemporary(windVelPrevRT);
        RenderTexture.ReleaseTemporary(divergenceRT);
        RenderTexture.ReleaseTemporary(thermalSedimentRT);


    }

    public void CreateWindMap(Vector4[] windMap, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float angle = Random.Range(-10, 10);
                float r = 0.5f * (2.0f * UnityEngine.Random.value - 1.0f) * 0.01f * jitter;
                float rad = angle * Mathf.Deg2Rad;
                float speed = windSpeed + r;
                windMap[x * width + y] = speed *(new Vector4(-Mathf.Sin(rad), Mathf.Cos(rad),0.0f,0.0f));
            }
        }
    }

    public void CreateReposeMap(int[] reposeMap, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                reposeMap[x * width + height] = Random.Range(15, 55);
            }
        }
    }

    public void CreateHardMap(float[] hardMap, float[] heightMap, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                hardMap[x * width + y] = heightMap[x * width + y] * 0.7f;
            }
        }
    }

}
