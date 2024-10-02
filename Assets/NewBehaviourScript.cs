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

    public Terrain terrain; // ��ȯ�� Terrain
    public float heightMultiplier = 1.0f; // ���� ���� (�ɼ�)

    
    void setup()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Mesh terrainMesh = GenerateMeshFromTerrain(terrain.terrainData);
        meshFilter.mesh = terrainMesh;

        // ���⿡�� ����(Material)�� ������ �� �ֽ��ϴ�.
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
    Mesh GenerateMeshFromTerrain(TerrainData terrainData)
    {
        int width = terrainData.heightmapResolution; // �ͷ����� �ػ�
        int height = terrainData.heightmapResolution;

        // ���� �����͸� �����ɴϴ�.
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        // ���ؽ�, UV �� �ﰢ�� �迭�� ����ϴ�.
        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uv = new Vector2[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6]; // �� �簢������ 6���� ����

        // ���ؽ��� UV ����
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float terrainHeight = heights[y, x] * heightMultiplier;
                vertices[y * width + x] = new Vector3(x, terrainHeight, y);
                uv[y * width + x] = new Vector2((float)x / width, (float)y / height);
            }
        }

        // �ﰢ�� ����
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

        // �޽� ���� �� �Ӽ� ����
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // ���� ���

        return mesh;
    }
}

