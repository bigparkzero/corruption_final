using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public bool testbool;
    private void OnValidate()
    {
        if (testbool)
        {
            testbool = false;
            setup();
        }
    }

    public Terrain terrain; // 변환할 Terrain
    public float heightMultiplier = 1.0f; // 높이 배율 (옵션)

    
    void setup()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Mesh terrainMesh = GenerateMeshFromTerrain(terrain.terrainData);
        meshFilter.mesh = terrainMesh;

        // 여기에서 재질(Material)을 지정할 수 있습니다.
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
    Mesh GenerateMeshFromTerrain(TerrainData terrainData)
    {
        int width = terrainData.heightmapResolution; // 터레인의 해상도
        int height = terrainData.heightmapResolution;

        // 높이 데이터를 가져옵니다.
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        // 버텍스, UV 및 삼각형 배열을 만듭니다.
        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uv = new Vector2[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6]; // 각 사각형마다 6개의 정점

        // 버텍스와 UV 설정
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float terrainHeight = heights[y, x] * heightMultiplier;
                vertices[y * width + x] = new Vector3(x, terrainHeight, y);
                uv[y * width + x] = new Vector2((float)x / width, (float)y / height);
            }
        }

        // 삼각형 설정
        int triIndex = 0;
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int current = y * width + x;
                int nextRow = (y + 1) * width + x;

                triangles[triIndex] = current;
                triangles[triIndex + 1] = nextRow + 1;
                triangles[triIndex + 2] = nextRow;

                triangles[triIndex + 3] = current;
                triangles[triIndex + 4] = current + 1;
                triangles[triIndex + 5] = nextRow + 1;

                triIndex += 6;
            }
        }

        // 메쉬 생성 및 속성 설정
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 법선 계산

        return mesh;
    }
}

