using Substance.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapDisplay : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Renderer textureRenderer;
    public SubstanceGraph graph;

    public void drawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
    public void LoadSubstances()
    {
        string assetPath = @"Assets/Material/LandscapeTest.sbsar";
        AssetDatabase.LoadAssetAtPath<Substance.Game.Substance>(assetPath);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        LoadSubstances();
        graph = SubstanceGraph.Find("LandscapeTest");
        graph.SetInputTexture("input", texture);
        graph.QueueForRender();
        Substance.Game.Substance.RenderSubstancesAsync();
        meshFilter.sharedMesh = meshData.CreateMesh();
        //meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
