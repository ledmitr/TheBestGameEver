using System.Collections;
using UnityEngine;

public class CreateCubesArray : MonoBehaviour
{
    private const int CubeArrayDimension = 10;

    public GameObject Prototype;
    private GameObject[,] CubeArray = new GameObject[CubeArrayDimension, CubeArrayDimension];

    private short[][] CubeArrayDefiner =
    {
        new short[] {4,3,2,1,1,1,1,1,1,1},
        new short[] {3,3,2,1,0,0,0,0,1,1},
        new short[] {2,2,1,0,0,1,1,1,1,1},
        new short[] {1,1,1,0,1,1,0,0,0,2},
        new short[] {1,1,0,0,0,0,0,2,3,3},
        new short[] {1,1,0,1,1,1,1,2,3,4},
        new short[] {1,1,0,1,0,0,0,0,2,3},
        new short[] {1,0,0,0,0,1,1,0,2,2},
        new short[] {1,1,1,1,1,1,1,1,1,1},
        new short[] {1,1,1,1,1,1,1,1,1,1},
    };
    
    void Start()
    {
        StartCoroutine(CreateCubes());
    }

    private IEnumerator CreateCubes()
    {
        Prototype = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Prototype.transform.localScale = new Vector3(5, 5, 5);
        for (int x = 0; x < CubeArrayDimension; x++)
        {
            for (int z = 0; z < CubeArrayDimension; z++)
            {
                //print("X:" + x);
                if (CubeArrayDefiner[x][z] != 0)
                {
                    var cube = (GameObject)Instantiate(Prototype, new Vector3(x * 6, CubeArrayDefiner[x][z], z * 6), transform.rotation);
                    CubeArray[x, z] = cube;
                }
                yield return 0;
            }
        }
    }
}