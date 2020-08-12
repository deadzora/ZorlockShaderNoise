using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGeneration
{
    public static MeshData GenerateTerrainMesh(float[] heightMap, int Width, int Height, int scale)
    {
        int width = Width;
        int height = Height;
        float topLeftX = width / -2f;
        float topLeftZ = height / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftZ - x, heightMap[vertexIndex] * scale, topLeftX + y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)height, y / (float)width);

                if (x < width -1 && y < height -1)
                {
                    //meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    //meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    //meshData.AddTriangle(vertexIndex + width, vertexIndex + width + 1, vertexIndex);
                    //meshData.AddTriangle(vertexIndex + 1, vertexIndex, vertexIndex + width + 1);

                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
