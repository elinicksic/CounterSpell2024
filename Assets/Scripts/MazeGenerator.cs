using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21; // Maze width (odd numbers work best)
    public int height = 21; // Maze height (odd numbers work best)
    public GameObject wallPrefab; // Prefab of the wall sprite
    public GameObject player; // Player object
    public float cellSize = 1f; // Size of each cell in the maze grid

    public int spawnSize = 5;

    private int[,] mazeGrid; // 0 = empty, 1 = wall
    private Vector2Int spawnCenter;

    void Start()
    {
        GenerateMaze();
        GenerateSpawn();
        DrawMaze();
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        player.transform.position = new Vector3(spawnCenter.x, spawnCenter.y, 0);
    }

    void GenerateSpawn()
    {
        spawnCenter = new Vector2Int(Random.Range(spawnSize, width - spawnSize), Random.Range(spawnSize, height - spawnSize));
        for (int x = spawnCenter.x - spawnSize; x < spawnCenter.x + spawnSize; x++)
        {
            for (int y = spawnCenter.y - spawnSize; y < spawnCenter.y + spawnSize; y++)
            {
                mazeGrid[x, y] = 0;
            }
        }
    }

    void GenerateMaze()
    {
        mazeGrid = new int[width, height];

        // Initialize maze grid: set all cells as walls (1)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mazeGrid[x, y] = 1;
            }
        }

        // Start maze generation with Prim's algorithm
        List<Vector2Int> frontier = new List<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);
        mazeGrid[start.x, start.y] = 0; // Mark start as a path
        AddFrontiers(start, frontier);

        while (frontier.Count > 0)
        {
            // Pick a random frontier cell
            int rndIndex = Random.Range(0, frontier.Count);
            Vector2Int current = frontier[rndIndex];
            frontier.RemoveAt(rndIndex);

            // Check if it has a neighboring cell that is part of the maze
            Vector2Int neighbor = GetValidNeighbor(current);
            if (neighbor != Vector2Int.zero)
            {
                // Make the current cell a path
                mazeGrid[current.x, current.y] = 0;

                // Make the wall between current and neighbor a path
                Vector2Int wallBetween = (current + neighbor) / 2;
                mazeGrid[wallBetween.x, wallBetween.y] = 0;

                // Add new frontiers
                AddFrontiers(current, frontier);
            }
        }
    }

    void AddFrontiers(Vector2Int cell, List<Vector2Int> frontier)
    {
        foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            Vector2Int neighbor = cell + dir * 2; // Skip one cell to ensure walls between paths
            if (IsInBounds(neighbor) && mazeGrid[neighbor.x, neighbor.y] == 1 && !frontier.Contains(neighbor))
            {
                frontier.Add(neighbor);
            }
        }
    }

    Vector2Int GetValidNeighbor(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            Vector2Int neighbor = cell + dir * 2;
            if (IsInBounds(neighbor) && mazeGrid[neighbor.x, neighbor.y] == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        if (neighbors.Count > 0)
        {
            return neighbors[Random.Range(0, neighbors.Count)];
        }

        return Vector2Int.zero;
    }

    bool IsInBounds(Vector2Int cell)
    {
        return cell.x > 0 && cell.x < width - 1 && cell.y > 0 && cell.y < height - 1;
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mazeGrid[x, y] == 1) // If it's a wall
                {
                    Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}
