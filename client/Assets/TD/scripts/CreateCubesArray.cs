using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.TD.scripts.Constants;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class CreateCubesArray : MonoBehaviour
    {
        private const int CubeArrayDimension = 10;

        public GameObject Prototype;
        public GameObject TentPrefab;
        public GameObject MainTowerPrefab;

        private GameObject[,] CubeArray = new GameObject[CubeArrayDimension, CubeArrayDimension];

        private int[][] _cubeArrayDefiner =
        {
            new [] {4,3,2,1,1,1,1,1,1,1},
            new [] {3,3,2,1,0,0,0,-2,1,1},
            new [] {2,2,1,0,0,1,1,1,1,1},
            new [] {1,1,1,0,1,1,0,0,-2,2},
            new [] {1,1,0,0,0,0,0,2,3,3},
            new [] {1,1,0,1,1,1,1,2,3,4},
            new [] {1,1,0,1,0,0,0,0,2,3},
            new [] {1,-1,0,0,0,1,1,-2,2,2},
            new [] {1,1,1,1,1,1,1,1,1,1},
            new [] {1,1,1,1,1,1,1,1,1,1},
        };
    
        public string FieldFileName = "Assets/field.txt";
    
        private void FillCubeArrayDefiner()
        {
            using (var file = File.OpenRead(FieldFileName))
            {
                if (file.Length == 0)
                    return;
                //yield break;

                int i = 0, j = 0;
                var listOfLists = new List<int[]>();
                var tempList = new List<int>();
                while (file.CanRead)
                {
                    int readedElement = file.ReadByte();
                    if ((char)readedElement != '\r')
                    {
                        tempList.Add(readedElement - '0');
                        i++;
                    }
                    else
                    {
                        if (file.ReadByte() == '\n')
                            listOfLists.Add(tempList.ToArray());
                        tempList = new List<int>();
                        j++;
                    }
                    //yield return 0;
                }
                if (listOfLists.All(x => x.Length == i)) 
                    _cubeArrayDefiner = listOfLists.ToArray();
            }
        }

        void Start()
        {
            //StartCoroutine(FillCubeArrayDefiner());
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
                
                    if (_cubeArrayDefiner[x][z] > 0)
                    {
                        var objectPosition = new Vector3(x * 6, _cubeArrayDefiner[x][z], z * 6);
                        var cube = (GameObject)Instantiate(Prototype, objectPosition, transform.rotation);
                        cube.tag = ApplicationConst.FieldTag;
                        CubeArray[x, z] = cube;
                    }
                    else if (_cubeArrayDefiner[x][z] == -1) {
                        var objectPosition = new Vector3(x * 6, MainTowerPrefab.transform.localScale.y, z * 6);
                        var tower = (GameObject)Instantiate(MainTowerPrefab, objectPosition, MainTowerPrefab.transform.rotation);
                        tower.tag = ApplicationConst.TowerTag;
                        CubeArray[x, z] = tower;
                    }
                    else if (_cubeArrayDefiner[x][z] == -2)
                    {
                        var objectPosition = new Vector3(x * 6, TentPrefab.transform.position.y, z * 6);
                        var tent = (GameObject)Instantiate(TentPrefab, objectPosition, TentPrefab.transform.rotation);
                        tent.tag = ApplicationConst.TentTag;
                        CubeArray[x, z] = tent;
                    }
                    yield return 0;
                }
            }
        }
    }
}