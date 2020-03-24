using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cube;

    GameObject[,] grid;
    GameObject[,] toDestroy;
    bool spawnCube;

    void Start()
    {
        grid = new GameObject[4, 4];
        toDestroy = new GameObject[4, 4];
        SpawnNewCube();
    }

    void Update()
    {
        bool animating = Animating();

        if (!animating)
        {
            if (spawnCube)
            {
                SpawnNewCube();
                spawnCube = false;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveUp();
                spawnCube = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveDown();
                spawnCube = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
                spawnCube = true;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
                spawnCube = true;
            }
        }
    }

    /// <summary>
    /// Call this after combining 2 cubes.
    /// Increases the color to the next level.
    /// </summary>
    /// <param name="obj">The final, new cube</param>
    void IncreaseColor(GameObject obj)
    {
        Color32 currColor = obj.GetComponent<Renderer>().material.color;
        Color32 newColor = currColor;

        if(currColor.r < 250)
        {
            newColor.r += 50;
        }
        else if(currColor.g < 250)
        {
            newColor.g += 50;
        }
        else if(currColor.b < 250)
        {
            newColor.b += 50;
        }

        obj.GetComponent<Renderer>().material.color = newColor;
    }

    /// <summary>
    /// Checks if 2 objects have the same color
    /// </summary>
    /// <returns>True if they have the same color, false otherwise</returns>
    bool HaveSameColor(GameObject a, GameObject b)
    {
        return a.GetComponent<Renderer>().material.color == b.GetComponent<Renderer>().material.color;
    }

    void SpawnNewCube()
    {
        List<int> slotsX = new List<int>();
        List<int> slotsY = new List<int>();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == null)
                {
                    slotsX.Add(x);
                    slotsY.Add(y);
                }
            }
        }

        if (slotsX.Count > 0)
        {
            int index = Random.Range(0, slotsX.Count);
            grid[slotsX[index], slotsY[index]] = Instantiate(cube, new Vector3(slotsX[index] * 2, slotsY[index] * 2, 0), Quaternion.identity);
        }
    }

    /// <summary>
    /// Repositions the cubes to their correct location
    /// </summary>
    /// <returns>True if the cubes are still moving, false if everything is in the right place</returns>
    bool Animating()
    {
        bool animating = false;

        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == null) continue;

                if(grid[x, y].transform.position != new Vector3(x * 2, y * 2, 0))
                {
                    Vector3 currPos = grid[x, y].transform.position;
                    Vector3 newPos = currPos;
                    float speed = 0.5f;

                    if (currPos.x < x * 2) newPos.x += speed;
                    if (currPos.x > x * 2) newPos.x -= speed;
                    if (currPos.y < y * 2) newPos.y += speed;
                    if (currPos.y > y * 2) newPos.y -= speed;

                    grid[x, y].transform.position = newPos;
                    animating = true;

                    for(int i = 0; i < toDestroy.GetLength(0); i++)
                    {
                        for(int j = 0; j < toDestroy.GetLength(1); j++)
                        {
                            if(toDestroy[i, j] != null)
                            {
                                Vector3 pos = new Vector3(i * 2, j * 2, 0);

                                if (grid[x, y].transform.position == pos)
                                {
                                    Destroy(toDestroy[i, j]);
                                    toDestroy[i, j] = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        return animating;
    }

    void MoveUp()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = grid.GetLength(1) - 2; y >= 0; y--)
            {
                if (grid[x, y] == null) continue;

                for (int newY = grid.GetLength(1) - 1; newY > y; newY--)
                {
                    if(grid[x, newY] == null)
                    {
                        grid[x, newY] = grid[x, y];
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

    void MoveDown()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == null) continue;

                for (int newY = 0; newY < y; newY++)
                {
                    if (grid[x, newY] == null)
                    {
                        grid[x, newY] = grid[x, y];
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

    void MoveLeft()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == null) continue;

                // An attempt at combining cubes.
                /*
                int nextX = -1;
                for (int i = x - 1; i >= 0; i--)
                {
                    if (grid[i, y]) nextX = i;
                }

                if (nextX > -1 && HaveSameColor(grid[x, y], grid[nextX, y]))
                {
                    toDestroy[nextX, y] = grid[nextX, y];
                    grid[nextX, y] = grid[x, y];
                    grid[x, y] = null;
                    IncreaseColor(grid[nextX, y]);
                }
                */
                for (int newX = 0; newX < x; newX++)
                {
                    if (grid[newX, y] == null)
                    {
                        grid[newX, y] = grid[x, y];
                        grid[x, y] = null;
                    }
                }
            }
        }
    }

    void MoveRight()
    {
        for (int x = grid.GetLength(0) - 2; x >= 0; x--)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == null) continue;

                // An attempt at combining cubes
                /*
                int nextX = -1;
                for(int i = x + 1; i < grid.GetLength(0); i++)
                {
                    if (grid[i, y]) nextX = i;
                }

                if (nextX > -1 && HaveSameColor(grid[x, y], grid[nextX, y]))
                {
                    toDestroy[nextX, y] = grid[nextX, y];
                    grid[nextX, y] = grid[x, y];
                    grid[x, y] = null;
                    IncreaseColor(grid[nextX, y]);
                }
                */
                for (int newX = grid.GetLength(0) - 1; newX > x; newX--)
                {
                    if (grid[newX, y] == null)
                    {
                        grid[newX, y] = grid[x, y];
                        grid[x, y] = null;
                    }
                }
            }
        }
    }
}
