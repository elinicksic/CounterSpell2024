using UnityEngine;

public class LightCaster : MonoBehaviour
{
    public int rayCount = 360; // Number of rays to cast
    public float lightRange = 5f; // Light range
    public LayerMask obstacleLayer; // Layer for walls/obstacles
    public float wallThickness = 0.1f; // Thickness of the wall in the mesh

    public GameObject lightMeshObject;

    private Mesh lightMesh;

    void Start()
    {
        // Create a new mesh for the light
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
        Vector3[] vertices = new Vector3[rayCount + 1]; // +1 for the center of the light
        int[] triangles = new int[rayCount * 3]; // Each ray forms a triangle

        // The center of the light is at the origin
        vertices[0] = Vector3.zero;

        // Loop through each ray to cast and calculate the points where the light interacts with the walls
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lightRange, obstacleLayer);

            // If the ray hits a wall, we use the point of intersection, else we use the max light range
            Vector3 hitPoint = hit.collider ? hit.point - (Vector2)transform.position : (direction * lightRange);

            // Add the hit point as a vertex
            vertices[i + 1] = hitPoint;

            // Generate the triangles that form the cone of light
            if (i < rayCount - 1)
            {
                triangles[i * 3] = 0; // Center vertex
                triangles[i * 3 + 1] = i + 1; // Current ray hit
                triangles[i * 3 + 2] = i + 2; // Next ray hit
            }
        }

        // Close the mesh by connecting the last ray to the first ray
        triangles[(rayCount - 1) * 3] = 0;
        triangles[(rayCount - 1) * 3 + 1] = rayCount;
        triangles[(rayCount - 1) * 3 + 2] = 1;

        // Update the mesh
        lightMesh.Clear();
        lightMesh.vertices = vertices;
        lightMesh.triangles = triangles;

        // Optionally add the thickness to the walls
        AddWallThickness(vertices, triangles);
    }

    void AddWallThickness(Vector3[] vertices, int[] triangles)
    {
        // Loop through the hit points and add a small offset to create a wall thickness effect
        for (int i = 0; i < rayCount; i++)
        {
            // Add a small offset to each wall point to simulate thickness
            Vector3 offset = (vertices[i + 1] - (Vector3)transform.position).normalized * wallThickness;
            vertices[i + 1] += offset;
        }

        // Here you could modify the triangles to form a "wall" around the light
        // (e.g., duplicating the triangles to form a thicker cone)
    }

}
