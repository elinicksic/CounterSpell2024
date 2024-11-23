using UnityEngine;

public class LightCaster : MonoBehaviour
{
    public int rayCount = 360; // Number of rays to cast
    public float lightRange = 5f; // Light range
    public LayerMask obstacleLayer; // Layer for walls/obstacles

    private Mesh lightMesh;

    void Start()
    {
        // Create a new mesh for the light
        GameObject lightMeshObject = new GameObject("LightMesh");
        lightMeshObject.transform.SetParent(transform);
        lightMeshObject.AddComponent<MeshFilter>();
        lightMeshObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        lightMesh = lightMeshObject.GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        GenerateLightMesh();
    }

    void GenerateLightMesh()
    {
        Vector3[] vertices = new Vector3[rayCount + 1];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero; // Center of the light

        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lightRange, obstacleLayer);

            vertices[i + 1] = hit.collider ? hit.point - (Vector2)transform.position : (direction * lightRange);

            if (i < rayCount - 1)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // Connect the last triangle back to the start
        triangles[(rayCount - 1) * 3] = 0;
        triangles[(rayCount - 1) * 3 + 1] = rayCount;
        triangles[(rayCount - 1) * 3 + 2] = 1;

        // Update the mesh
        lightMesh.Clear();
        lightMesh.vertices = vertices;
        lightMesh.triangles = triangles;
    }
}
