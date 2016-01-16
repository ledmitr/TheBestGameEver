using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using UnityEngine;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Управляет юнитами.
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        public GameObject CubePrototype = null;
        public GameObject RoadPrototype = null;
        public GameObject KnightPrefab = null;
        public GameObject TowerPrefab = null;
        public GameObject LandPrefab = null;
        public GameObject MainTowerPrefab = null;
        public GameObject TentPrefab = null;

        private Dictionary<int, GameObject> _towers;
        private Dictionary<int, GameObject> _knights;

        private GameObject[,] _cubeArray;

        public void InitCubeArray(int height, int width)
        {
            _cubeArray = new GameObject[height, width];
        }

        private void Start()
        {
            Debug.Assert(CubePrototype != null);
            Debug.Assert(RoadPrototype != null);
            Debug.Assert(KnightPrefab != null);
            Debug.Assert(TowerPrefab != null);
            Debug.Assert(MainTowerPrefab != null);
            Debug.Assert(TentPrefab != null);

            _towers = new Dictionary<int, GameObject>(3);
            _knights = new Dictionary<int, GameObject>(10);
        }

        private void Update()
        {

        }

        private static void SetMinimumHealth(ActualDataUnit unitData, GameObject unit)
        {
            var healthComponent = unit.GetComponent<EnemyHealth>();
            healthComponent.MaxHealth = unitData.hit_point;
        }
        
        /// <summary>
        /// Создаёт рыцаря.
        /// </summary>
        /// <param name="unitData">Позиция рыцаря.</param>
        /// <returns>Компонент управления рыцарем.</returns>
        private GameObject CreateKnight(ActualDataUnit unitData)
        {
            var knightPosition = new Vector3(unitData.position_x, 0, unitData.position_y);
            var knight = (GameObject)Instantiate(KnightPrefab, knightPosition, Quaternion.identity);
            SetMinimumHealth(unitData, knight);
            return knight;
        }
        
        /// <summary>
        /// Создаёт башню.
        /// </summary>
        /// <param name="unitData">Позиция башни.</param>
        /// <returns>Компонент управления башней.</returns>
        private GameObject CreateTower(ActualDataUnit unitData)
        {
            var towerPosition = new Vector3(unitData.position_x, 0, unitData.position_y);
            var tower = (GameObject)Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
            SetMinimumHealth(unitData, tower);
            return tower;
        }

        public void UpdateUnits(ActualData actualData)
        {
            var processedTowersIds = new HashSet<int>();
            var processedKnightsIds = new HashSet<int>();
            foreach (var actualDataContentItem in actualData.content)
            {
                foreach (var unitData in actualDataContentItem.units)
                {
                    if (unitData.type_unit == UnitType.Tower)
                    {
                        UpdateUnit(_towers, unitData, processedTowersIds);
                    }
                    else
                    {
                        UpdateUnit(_knights, unitData, processedKnightsIds);
                    }
                }
            }

            DestroyNotExistedUnits(processedTowersIds, _towers);
            DestroyNotExistedUnits(processedKnightsIds, _knights);
        }

        private static void DestroyNotExistedUnits(IEnumerable<int> processedIds, Dictionary<int, GameObject> gameObjects)
        {
            // выбираем все идентификаторы, которые есть
            var ids = gameObjects.Select(x => x.Key);
            // вычитаем из них все обработанные, то есть присланные сервером, то есть сущзествующие в игре
            var unitsIntersect = ids.Except(processedIds);
            // удалем те, которые не существуют
            foreach (var id in unitsIntersect)
            {
                // находим объект в списке по идентификатору
                var unit = gameObjects[id].gameObject;
                // уничтожаем игровой объект
                Destroy(unit);
                // удалем его из списка
                gameObjects.Remove(id);
            }
        }

        private void UpdateUnit(IDictionary<int, GameObject> gameObjects, ActualDataUnit unitData, ICollection<int> processedIds)
        {
            if (gameObjects.ContainsKey(unitData.id_unit))
            {
                AdjustUnitData(gameObjects[unitData.id_unit], unitData);
            }
            else
            {
                if (unitData.type_unit == UnitType.Knight)
                    _knights.Add(unitData.id_unit, CreateKnight(unitData));
                else
                    _towers.Add(unitData.id_unit, CreateTower(unitData));
            }
            processedIds.Add(unitData.id_unit);
        }

        private static readonly Dictionary<UnitDirection, int> DirectionAnglesDictionary = new Dictionary<UnitDirection, int>
        {
            {UnitDirection.Top, 180}, {UnitDirection.Bottom, 270}, {UnitDirection.Left, 90}, {UnitDirection.Right, 0} 
        }; 
        
        private static void AdjustUnitData(GameObject unit, ActualDataUnit unitData)
        {
            if (unitData.type_unit == UnitType.Knight)
            {
                var unitPosition = new Vector3(unitData.position_y, 0, unitData.position_x);
                unit.transform.position = unitPosition;

                float rotationAngle = DirectionAnglesDictionary[unitData.direction];
                unit.transform.eulerAngles = new Vector3(0, rotationAngle, 0);
            }

            var healthScript = unit.GetComponent<EnemyHealth>();
            healthScript.SetHealth(unitData.hit_point);
        }

        public IEnumerator InstantinateMap(GameMap map)
        {
            for (int x = 0; x < map.Height; x++)
            {
                for (int z = 0; z < map.Width; z++)
                {
                    var cellPosition = new Vector2(x, z);
                    switch ((MapCellType)map.Map[x][z])
                    {
                        case MapCellType.Fortress:
                            InstantiateObjectOnMap(x, z, MainTowerPrefab, ApplicationConst.FortressTag);
                            GameInfo.Map.FortressPosition = cellPosition;
                            break;
                        case MapCellType.Tent:
                            var tent = InstantiateObjectOnMap(x, z, TentPrefab, ApplicationConst.TentTag);
                            var tentClosestRoad = GameInfo.Map.CalcTentClosestRoad(cellPosition);
                            //Debug.Log(string.Format("tent closest road is : {0}", tentClosestRoad));
                            RotateTent(tent, tentClosestRoad);
                            GameInfo.Map.TentPosition = cellPosition;
                            break;
                        case MapCellType.Mountains:
                            InstantiateObjectOnMap(x, z, CubePrototype, ApplicationConst.MountainTag);
                            break;
                        case MapCellType.Road:
                            InstantiateObjectOnMap(x, z, RoadPrototype, ApplicationConst.RoadTag);
                            break;
                        case MapCellType.Empty:
                            InstantiateObjectOnMap(x, z, LandPrefab, ApplicationConst.LandTag);
                            break;
                    }
                }
                yield return 0;
            }
            GameInfo.MainUnitCoords = GameInfo.Role == PlayerRole.Attacker ? GameInfo.Map.TentPosition : GameInfo.Map.FortressPosition;
        }

        private void RotateTent(GameObject tent, Vector2 tentClosestRoad)
        {
            var forward = new Vector3(tentClosestRoad.x + tent.transform.position.x, 0, tent.transform.position.z + tentClosestRoad.y);
            tent.transform.rotation = Quaternion.LookRotation(forward);
        }

        private GameObject InstantiateObjectOnMap(int x, int z, GameObject prefab, string objectTag)
        {
            var objectPosition = new Vector3(x, 0, z);
            var newObject = (GameObject) Instantiate(prefab, objectPosition, prefab.transform.rotation);
            newObject.tag = objectTag;
            _cubeArray[x, z] = newObject;
            return newObject;
        }
    }
}