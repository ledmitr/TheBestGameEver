using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    
    /// <summary>
    /// Класс обрабатывает пользовательский ввод, управляет интерфейсом.
    /// </summary>
    public class TouchListener : MonoBehaviour
    {
        public GameObject ConnectionManager;
        private bool _StartRole = false;
        private ConnectToServer ConnectionToServer;

        public UnitManager UnitManager;

        // Use this for initialization
        private void Start()
        {
            GameInfo.GameState = GameState.Playing;
            SidePanel.SetActive(false);
            HumburgerButton.SetActive(true);
            HintPanel.SetActive(false);
            MessagePanel.SetActive(false);

            ConnectionToServer = ConnectionManager.GetComponent<ConnectToServer>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                if (MessagePanel.activeSelf)
                {
                    BackToGameFromMessageCanvas();
                }
                else
                {
                    MessagePanel.SetActive(true);
                }
                return;
            }

            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
            {
                //declare a variable of RaycastHit struct
                RaycastHit hit;
                //Create a Ray on the tapped / clicked position
                Ray ray;
#if UNITY_EDITOR
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
                //Check if the ray hits any collider
                if (Physics.Raycast(ray, out hit))
                {
                    ProcessClick(hit);
                }
            }
        }

        public GameObject MessagePanel;

        /// <summary>
        /// Выйти из приложения.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
        /// <summary>
        /// 
        /// </summary>
        public void BackToGameFromMessageCanvas()
        {
            MessagePanel.SetActive(false);
        }

        private void ProcessClick(RaycastHit hit)
        {
            if (WasJustAButton())
                return;

            Debug.Log(GameInfo.GameState);
            Debug.Log(hit.collider.tag);
            switch (GameInfo.GameState)
            {
                case GameState.Playing:
                    if (hit.collider.tag == ApplicationConst.KnightTag)
                    {
                        UnselectAll();
                        GameInfo.GameState = GameState.KnightSelected;
                        hit.collider.gameObject.GetComponent<KnightScript>().Select(true);
                    }
                    break;
                case GameState.ChooseNewTowerPosition:
                    if (hit.collider.tag == ApplicationConst.LandTag)
                    {
                        CreateTower(hit.point);
                        HintPanel.SetActive(false);
                        GameInfo.GameState = GameState.Playing;
                    }
                    break;
                case GameState.ChooseNewKnightPosition:
                    if (hit.collider.tag == ApplicationConst.TentTag)
                    {
                        CreateKnight(hit.collider.gameObject);
                        HintPanel.SetActive(false);
                        GameInfo.GameState = GameState.Playing;
                    }
                    break;
                case GameState.TentSelected:
                    break;
                case GameState.KnightSelected:
                    if (hit.collider.tag == ApplicationConst.KnightTag)
                    {
                        var knightScript = hit.collider.gameObject.GetComponent<KnightScript>();
                        if (!knightScript.IsSelected())
                        {
                            UnselectAll();
                            knightScript.Select(true);
                        }
                    } 
                    else if (hit.collider.tag == ApplicationConst.TowerTag)
                    {
                        SendKnightToTower(hit);
                    }
                    else if (hit.collider.tag == ApplicationConst.LandTag)
                    {
                        UnselectAll();
                    }
                    break;
                case GameState.TowerSelected:
                    break;
                case GameState.Finished:
                    break;
                case GameState.Planning:
                    break;
                case GameState.Preparing:
                    break;
            }
            Debug.Log(GameInfo.GameState);
        }

        private void SendKnightToTower(RaycastHit hit)
        {
            var knights = GameObject.FindGameObjectsWithTag(ApplicationConst.KnightTag);
            KnightScript selectedKnight = null;
            foreach (var knight in knights)
            {
                var knightComponent = knight.GetComponent<KnightScript>();
                if (knightComponent != null && knightComponent.IsSelected())
                {
                    selectedKnight = knightComponent;
                    break;
                }
            }
            if (selectedKnight != null)
            {
                selectedKnight.TargetPositionChanged(hit.collider.transform.position);
                //selectedKnight.Select(false);
                GameInfo.GameState = GameState.Playing;
            }
            UnselectAll();
        }

        private void SetHint(string hintText)
        {
            var text = HintPanel.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = hintText;
            }
        }

        private void UnselectAll() {
            var selectableObjects = GameObject.FindGameObjectsWithTag(ApplicationConst.SelectableTag);
            if (selectableObjects != null && selectableObjects.Length != 0)
            {
                foreach (GameObject selectableObject in selectableObjects)
                {
                    Selectable objectMain = selectableObject.transform.parent.gameObject.GetComponent<Selectable>();
                    if (objectMain != null)
                        objectMain.Select(false);
                }
            }
        }
        /// <summary>
        /// Вернуться в главное меню.
        /// </summary>
        public void BackToMenu()
        {
            Application.LoadLevel("MainMenu");
        }

        public GameObject HumburgerButton;
        public GameObject SidePanel;
        public GameObject CreateTowerButton;
        public GameObject CreateKnightButton;

        /// <summary>
        /// Открывает боковую панель.
        /// </summary>
        public void ProcessHamburgerButton()
        {
            HumburgerButton.SetActive(!HumburgerButton.activeSelf);
            SidePanel.SetActive(!SidePanel.activeSelf);
            CreateKnightButton.SetActive(GameInfo.Role == PlayerRole.Attacker);
            CreateTowerButton.SetActive(GameInfo.Role == PlayerRole.Defender);
                
        }

        public GameObject TowerPrefab;
        public GameObject KnightPrefab;
        public GameObject HintPanel;

        private void CreateTower(Vector3 targetTowerPosition)
        {
            ConnectionToServer.SendAddUnitRequest(UnitType.Tower, targetTowerPosition);
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки "создать башню".
        /// </summary>
        public void ProcessCreateTowerButton()
        {
            ProcessHamburgerButton();
            SetHint(ApplicationConst.CreateTowerHint);
            HintPanel.SetActive(true);
            GameInfo.GameState = GameState.ChooseNewTowerPosition;
        }

        private void CreateKnight(GameObject tent)
        {   
            //todo: refactor magic numbers
            var knightPosition = new Vector3(tent.transform.position.x + 1.6F, tent.transform.position.y, tent.transform.position.z - 2.5F);

            ConnectionToServer.SendAddUnitRequest(UnitType.Knight, knightPosition);
        }


        /// <summary>
        /// Обрабатывает нажатие кнопки "создать рыцаря".
        /// </summary>
        public void ProcessCreateKnightButton()
        {
            ProcessHamburgerButton();
            SetHint(ApplicationConst.CreateKnightHint);
            HintPanel.SetActive(true);
            GameInfo.GameState = GameState.ChooseNewKnightPosition;
        }

        /// <summary>
        /// Скрывает боковую панель.
        /// </summary>
        public void HideSidePanel() {
            SidePanel.SetActive(false);
            HumburgerButton.SetActive(true);
            UnselectAll();
        }

        private static bool WasJustAButton()
        {
            var ct = UnityEngine.EventSystems.EventSystem.current;

            if (!ct.IsPointerOverGameObject()) return false;
            if (!ct.currentSelectedGameObject) return false;
            if (ct.currentSelectedGameObject.GetComponent<Button>() == null)
                return false;

            return true;
        }
    }
}