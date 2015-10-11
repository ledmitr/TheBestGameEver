using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Texture2D hMap;

    public string FieldFileName = "Assets/field.txt";
    
    private float[,] _heights;
    
    public void Start()
    {
        //StartCoroutine(ReadHeights(FieldFileName));
        //StartCoroutine(SetHeightFromImage(hMap));
        //SetHeightsAsTerrainData();
    }

    private IEnumerator ReadHeights(string fileName)
    {
        var filelines = File.ReadAllLines(fileName);
        var arr = new float[filelines.Length][];
        for (var i = 0; i < filelines.Length; i++)
        {
            arr[i] = filelines[i].Split(',').Select(x => (float)int.Parse(x)).ToArray();
            yield return 0;
        }

        _heights = new float[arr.Length, arr.Max(x => x.Length)];
        for (var i = 0; i < arr.Length; i++)
        {
            for (var j = 0; j < arr[i].Length; j++)
            {
                _heights[i, j] = arr[i][j];
            }
        }
    }

    private void SetHeightsAsTerrainData()
    {
        var terrainData = GetComponent<Terrain>().terrainData;
        terrainData.SetHeights(0, 0, _heights);
    }
    
    private IEnumerator SetHeightFromImage(Texture2D image)
    {
        _heights = new float[image.height, image.width];
        for (int i = 0; i < image.height; i++)
        {
            for (int j = 0; j < image.width; j++)
            {
                _heights[i, j] = image.GetPixel(i, j).grayscale;
            }
            yield return 0;
        }
    }
}
