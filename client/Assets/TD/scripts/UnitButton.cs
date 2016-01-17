using Assets.TD.scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    public class UnitButton : MonoBehaviour
    {
        public ViewDrag CameraScript;

        public UnitType UnitType;
        public GameObject Unit;

        private Button _buttonComponent;

        // Use this for initialization
        private void Start ()
        {
            _buttonComponent = GetComponent<Button>();
            _buttonComponent.onClick.AddListener(ProcessClick);
            GetComponentInChildren<Text>().text = UnitType == UnitType.Knight ? "\uf007" : "\uf19c" ;
        }

        private void ProcessClick()
        {
            CameraScript.SetCameraPosition(new Vector2(Unit.transform.position.x, Unit.transform.position.z));
        }

        // Update is called once per frame
        private void Update () 
        {
	        
        }
    }
}
