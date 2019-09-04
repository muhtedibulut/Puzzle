using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int row = 8;
    public int col = 9;
    public Color[] c;

    float hexWidth;
    float hexHeigth;
    List<Color> colors;
    System.Random random;
    float posX;
    float posY;
    float tileSize = 0.9f;

    void Start()
    {
        hexSize();
        colorAdd();
        generateGrid();
    }


    private void hexSize()
    {
        GameObject hex = (GameObject)Instantiate(Resources.Load("Hex"));
        hexWidth = hex.GetComponent<Renderer>().bounds.size.x;
        hexHeigth = hexWidth * 0.86f;
        Destroy(hex);

    }

    private void colorAdd()
    {
        random = new System.Random();
        colors = new List<Color>();
        colors.Add(Color.magenta);
        colors.Add(Color.red);
        colors.Add(Color.yellow);
        colors.Add(Color.blue);
        colors.Add(Color.green);
    }

    private void generateGrid()
    {
        GameObject hex = (GameObject)Instantiate(Resources.Load("Hex"));
        hex.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 90);

        for (int i = 0; i < row; i++)
        {
            posUpdate(i);
            for (int j = 0; j < col; j++)
            {
                GameObject tile = Instantiate(hex, posGrid(i, j), Quaternion.identity);
                tile.name = i + "+" + j;
                tile.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 90);

                HexBlock.pos[i, j] = tile.transform.position;
                tile.transform.position = new Vector3(0, 0, 0);
                tile.GetComponent<Renderer>().material.color = colors[random.Next(colors.Count)];


            }
        }

        Destroy(hex);
    }

    private void posUpdate(int i)
    {
        tileSize = hexWidth / 1.2f;
        posX = -tileSize;
        posY -= 1.5f;
    }

    private Vector3 posGrid(int i, int j)
    {
        float offset = 0.5f;
        if (j % 2 == 0)
        {
            posY += offset;
        }
        else
        {
            posY -= offset;
        }

        posX += tileSize;
        return new Vector3(posX, posY, 0);
    }

    void Update()
    {

    }
}
