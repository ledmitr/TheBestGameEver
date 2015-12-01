using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.TD.scripts
{
    public class TerrainGenerator : MonoBehaviour
    {
        public Texture2D hMap;

        public string FieldFileName = "Assets/field.txt";

        private float[,] _heights;

        public void Start()
        {
            //StartCoroutine(ReadHeights(FieldFileName));
            SetHeightFromImage(hMap);
            SetHeightsAsTerrainData();
            Texture();
        }

        private void Texture()
        {
            // Get the attached terrain component
            var terrain = GetComponent<Terrain>();

            terrain.legacyShininess = 0;

            // Get a reference to the terrain data
            var terrainData = terrain.terrainData;

            // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
            var splatmapData =
                new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            for (var y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (var x = 0; x < terrainData.alphamapWidth; x++)
                {
                    // Normalise x/y coordinates to range 0-1 
                    var y01 = y/(float) terrainData.alphamapHeight;
                    var x01 = x/(float) terrainData.alphamapWidth;

                    // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                    var height = terrainData.GetHeight(Mathf.RoundToInt(y01*terrainData.heightmapHeight),
                        Mathf.RoundToInt(x01*terrainData.heightmapWidth));

                    // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                    var normal = terrainData.GetInterpolatedNormal(y01, x01);

                    // Calculate the steepness of the terrain
                    var steepness = terrainData.GetSteepness(y01, x01);

                    // Setup an array to record the mix of texture weights at this point
                    var splatWeights = new float[terrainData.alphamapLayers];

                    // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                    // Texture[0] has constant influence
                    splatWeights[0] = 0.5f;

                    // Texture[1] is stronger at lower altitudes
                    //splatWeights[1] = Mathf.Clamp01((terrainData.heightmapHeight - height));

                    // Texture[2] stronger on flatter terrain
                    // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                    // Subtract result from 1.0 to give greater weighting to flat surfaces
                    splatWeights[1] = 1.0f - Mathf.Clamp01(steepness*steepness/(terrainData.heightmapHeight/1.0f));

                    // Texture[3] increases with height but only on surfaces facing positive Z axis 
                    splatWeights[2] = height*Mathf.Clamp01(normal.z);

                    // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                    var z = splatWeights.Sum();

                    // Loop through each terrain texture
                    for (var i = 0; i < terrainData.alphamapLayers; i++)
                    {
                        // Normalize so that sum of all texture weights = 1
                        splatWeights[i] /= z;

                        // Assign this point to the splatmap array
                        splatmapData[x, y, i] = splatWeights[i];
                    }
                }
            }

            // Finally assign the new splatmap to the terrainData:
            terrainData.SetAlphamaps(0, 0, splatmapData);
        }

        private IEnumerator ReadHeights(string fileName)
        {
            var filelines = File.ReadAllLines(fileName);
            var arr = new float[filelines.Length][];
            for (var i = 0; i < filelines.Length; i++)
            {
                arr[i] = filelines[i].Split(',').Select(x => (float) int.Parse(x)).ToArray();
                yield return null;
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

        private void SetHeightFromImage(Texture2D image)
        {
            _heights = new float[image.height, image.width];
            for (var i = 0; i < image.height; i++)
            {
                for (var j = 0; j < image.width; j++)
                {
                    _heights[i, j] = image.GetPixel(i, j).grayscale;
                }
            }
        }
    }
}
