using System.Collections.Generic;
using System.Linq;
using Assets.TD.scripts.Enums;
using UnityEngine;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Управляет юнитами.
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        public GameObject KnightPrefab;
        public GameObject TowerPrefab;

        private List<TowerScript> _towers;
        private List<KnightScript> _knights;

        private void Start()
        {
            _towers = new List<TowerScript>();
            _knights = new List<KnightScript>();
        }

        private void Update()
        {

        }

        /// <summary>
        /// Обновляет состояние юнита. Создаёт нового, если такого не существует.
        /// </summary>
        /// <param name="unitData">Сообщение от сервера о состоянии юнита.</param>
        public void UpdateUnit(ActualDataUnit unitData)
        {
            var unitPosition = new Vector3(unitData.position_x, unitData.position_y);
            if (unitData.type_unit == UnitType.Tower){
                var unit = _towers.FirstOrDefault(x => x.Id == unitData.id);
                if (unit != null)
                {
                    unit.transform.position = unitPosition;
                }
                else
                {
                    _towers.Add(CreateTower(unitPosition));
                }
            }
            else
            {
                var unit = _knights.FirstOrDefault(x => x.Id == unitData.id);
                if (unit != null)
                {
                    unit.transform.position = unitPosition;
                }
                else
                {
                    _knights.Add(CreateKnight(unitPosition));
                }
            }
        }

        /// <summary>
        /// Создаёт рыцаря.
        /// </summary>
        /// <param name="knightPosition">Позиция рыцаря.</param>
        /// <returns>Компонент управления рыцарем.</returns>
        private KnightScript CreateKnight(Vector3 knightPosition)
        {
            var knight = (GameObject)Instantiate(KnightPrefab, knightPosition, Quaternion.identity);
            var knightScript = knight.GetComponent<KnightScript>();
            //knightScript.SetPath(path);
            return knightScript;
        }

        /// <summary>
        /// Создаёт башню.
        /// </summary>
        /// <param name="towerPosition">Позиция башни.</param>
        /// <returns>Компонент управления башней.</returns>
        private TowerScript CreateTower(Vector3 towerPosition)
        {
            var tower = (GameObject)Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
            var towerScript = tower.GetComponent<TowerScript>();
            return towerScript;
        }

        public void UpdateUnits(ActualData actualData)
        {
            var processedTowersIds = new List<int>();
            var processedKnightsIds = new List<int>();
            var towerIds = _towers.Select(x => x.Id).ToList();
            var knightsIds = _knights.Select(x => x.Id).ToList();
            foreach (var actualDataContentItem in actualData.content)
            {
                foreach (var unitData in actualDataContentItem.units)
                {
                    var unitPosition = new Vector3(unitData.position_x, unitData.position_y);
                    if (unitData.type_unit == UnitType.Tower)
                    {
                        var unit = _towers.SingleOrDefault(x => x.Id == unitData.id);
                        if (unit != null)
                        {
                            unit.transform.position = unitPosition;
                            processedTowersIds.Add(unit.Id);
                        }
                        else
                        {
                            _towers.Add(CreateTower(unitPosition));
                        }
                    }
                    else
                    {
                        var unit = _knights.FirstOrDefault(x => x.Id == unitData.id);
                        if (unit != null)
                        {
                            unit.transform.position = unitPosition;
                            processedKnightsIds.Add(unit.Id);
                        }
                        else
                        {
                            _knights.Add(CreateKnight(unitPosition));
                        }
                    }
                }
            }
            var knightsIntersect = knightsIds.Except(processedKnightsIds);
            foreach (var id in knightsIntersect)
            {
                var knight = _knights.Single(x => x.Id == id);
                Destroy(knight.gameObject);
            }
            var towersIntersect = towerIds.Except(processedTowersIds);
            foreach (var id in towersIntersect)
            {
                var tower = _towers.Single(x => x.Id == id);
                Destroy(tower.gameObject);
            }
        }
    }
}