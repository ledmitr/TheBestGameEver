using System.Collections.Generic;
using System.Linq;
using Assets.TD.scripts.Enums;
using UnityEngine;

namespace Assets.TD.scripts
{
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
                    //todo: PATH??
                    _knights.Add(CreateKnight(unitPosition, new Vector3[0]));
                }
            }
        }

        public KnightScript CreateKnight(Vector3 knightPosition, Vector3[] path)
        {
            var knight = (GameObject)Instantiate(KnightPrefab, knightPosition, Quaternion.identity);
            var knightScript = knight.GetComponent<KnightScript>();
            knightScript.SetPath(path);
            return knightScript;
        }

        public TowerScript CreateTower(Vector3 towerPosition)
        {
            var tower = (GameObject)Instantiate(TowerPrefab, towerPosition, Quaternion.identity);
            var towerScript = tower.GetComponent<TowerScript>();
            return towerScript;
        }
    }
}