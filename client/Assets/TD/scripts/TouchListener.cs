using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Класс обрабатывает пользовательский ввод, управляет интерфейсом.
    /// </summary>
    public class TouchListener : MonoBehaviour
    {
        public GameObject ConnectionManager;
        public UIManager UIManager;
        //private bool _start = false;
        //public static Timer _ticker;
        //public int _cnt = 10;
        private ConnectToServer _connectionToServer;

        // Use this for initialization
        private void Start()
        {
            UIManager.MessagePanel.SetActive(false);
            _connectionToServer = ConnectionManager.GetComponent<ConnectToServer>();
        }

        // Update is called once per frame
        private void Update()
        {            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.ProcessEscapeButton();
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

        /// <summary>
        /// Выйти из приложения.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }

        private void ProcessClick(RaycastHit hit)
        {
            //block clicks under UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Debug.Log(string.Format("you hit on {0}", hit.collider.tag));
            Debug.Log(GameInfo.GameState);
            
            switch (GameInfo.GameState)
            {
                case GameState.Playing:
                    /*if (hit.collider.tag == ApplicationConst.KnightTag)
                    {
                        UnselectAll();
                        GameInfo.GameState = GameState.KnightSelected;
                        hit.collider.gameObject.GetComponent<KnightScript>().Select(true);
                    }*/
                    break;
                case GameState.ChooseNewTowerPosition:
                    if (hit.collider.tag == ApplicationConst.LandTag)
                    {
                        CreateTower(hit.point);
                        UIManager.HideHintPanel();
                        GameInfo.GameState = GameState.Playing;
                    }
                    break;
                case GameState.ChooseNewKnightPosition:
                    if (hit.collider.tag == ApplicationConst.TentTag)
                    {
                        CreateKnight(hit.collider.gameObject);
                        UIManager.HideHintPanel();
                        GameInfo.GameState = GameState.Playing;
                    }
                    break;
                case GameState.TentSelected:
                    break;
                /*case GameState.KnightSelected:
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
                    }*/
                    break;
                case GameState.TowerSelected:
                    break;
                case GameState.Finished:
                    break;
                case GameState.Planning:
                    //PreparingStartBar.SetActive(true);
                    //ticker = new Timer(TimerMethod, null, 1000, 1000);
                    break;
                case GameState.Preparing:
                    break;
            }
            Debug.Log(GameInfo.GameState);
        }
        
        private void SetTextInBar(int numArg, string text, GameObject obj)
        {
            var textComponent = obj.GetComponentsInChildren<Text>();
            if (textComponent != null)
            {
                textComponent[numArg].text = text;
            }
        }

        /// <summary>
        /// Вернуться в главное меню.
        /// </summary>
        public void BackToMenu()
        {
            Application.LoadLevel("MainMenu");
        }
        
        private void CreateTower(Vector3 targetTowerPosition)
        {
            GameInfo.CoinsAmount -= ApplicationConst.TowerCost;
            UIManager.UpdateButtonState(UnitType.Tower);
            _connectionToServer.SendAddUnitRequest(UnitType.Tower, new Vector2(targetTowerPosition.z, targetTowerPosition.x));
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки "создать башню".
        /// </summary>
        public void ProcessCreateTowerButton()
        {
            UIManager.ProcessHamburgerButton();
            UIManager.SetHint(ApplicationConst.CreateTowerHint);
            UIManager.ShowHintPanel();
            GameInfo.GameState = GameState.ChooseNewTowerPosition;
        }

        private void CreateKnight(GameObject tent)
        {
            GameInfo.CoinsAmount -= ApplicationConst.KnightCost;
            UIManager.UpdateButtonState(UnitType.Knight);

            var knightPosition = new Vector2(tent.transform.position.z, tent.transform.position.x);
            _connectionToServer.SendAddUnitRequest(UnitType.Knight, knightPosition);
        }
        
        /// <summary>
        /// Обрабатывает нажатие кнопки "создать рыцаря".
        /// </summary>
        public void ProcessCreateKnightButton()
        {
            if (GameInfo.CoinsAmount < ApplicationConst.KnightCost)
                return;

            UIManager.ProcessHamburgerButton();
            UIManager.SetHint(ApplicationConst.CreateKnightHint);
            UIManager.ShowHintPanel();
            GameInfo.GameState = GameState.ChooseNewKnightPosition;
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

        /*public static void TimerMethod(object state)
        {
            _cnt--;
            SetTextInBar(2, _cnt.ToString(), PreparingStartBar);
            if (_cnt == 0 || GameInfo.GameState==GameState.Playing) _ticker.Dispose(); 
        }*/
    }
}