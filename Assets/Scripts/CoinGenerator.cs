using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    public GameObject coinPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            while (true)
            {
                int x = Random.Range(0, 127);
                int y = Random.Range(0, 127);

                if (mazeGenerator.getMazeGrid()[x, y] == 0)
                {
                    print("Made a coin!");
                    Instantiate(coinPrefab, new Vector3(x * mazeGenerator.cellSize, y * mazeGenerator.cellSize, 0), Quaternion.identity, transform);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
