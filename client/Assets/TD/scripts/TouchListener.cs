using System.Linq;
using Assets.TD.scripts;
using UnityEngine;
using UnityEngine.UI;

public class TouchListener : MonoBehaviour
{
    public GameState GameState { get; private set; }

    // Use this for initialization
    private void Start()
    {
        GameState = GameState.Playing;
        SidePanel.SetActive(false);
        HumburgerButton.SetActive(true);
        HintPanel.SetActive(false);
        MessagePanel.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            //MessagePanel.SetActive(true);
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
                //if (_isCreateTowerMode && hit.collider.tag == ApplicationConst.FieldTag)
                //{
                //    CreateTower(hit.transform.position);
                //    HintPanel.SetActive(false);
                //    _isCreateTowerMode = false;
                //} else if (_isCreateKnightMode && hit.collider.tag == ApplicationConst.TentTag)
                //{
                //    CreateKnight(hit.collider.gameObject);
                //    HintPanel.SetActive(false);
                //    _isCreateKnightMode = false;
                //}
                //else
                //{
                //    StartCoroutine(ProcessHit(hit));
                //}
            }
        }
    }

    public GameObject MessagePanel;
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToGameFromMessageCanvas()
    {
        MessagePanel.SetActive(false);
    }

    private void ProcessClick(RaycastHit hit)
    {
        if (WasJustAButton())
            return;        

        Debug.Log(GameState);
        Debug.Log(hit.collider.tag);
        switch (hit.collider.tag)
        {
            case ApplicationConst.KnightTag:
                GameState = GameState.KnightSelected;
                hit.collider.gameObject.GetComponent<KnightScript>().Select();
                break;
            case ApplicationConst.TentTag:
                //hit.collider.gameObject.GetComponent<TentScript>().Select();
                //GameState = GameState.TentSelected;
                if (GameState == GameState.ChooseNewKnightPosition)
                {
                    CreateKnight(hit.collider.gameObject);
                    HintPanel.SetActive(false);
                    GameState = GameState.Playing;
                }
                break;
            case ApplicationConst.TowerTag:
                if (GameState == GameState.KnightSelected)
                {
                    var selectedKnight = GameObject.FindGameObjectsWithTag(ApplicationConst.KnightTag)
                        .Select(x => x.GetComponent<KnightScript>())
                        .Single(x => x.IsSelected);
                    selectedKnight.GetComponent<KnightScript>().TargetPositionChanged(hit.collider.transform.position);
                    GameState = GameState.Playing;
                }
                else
                {
                    GameState = GameState.TowerSelected;
                    //hit.collider.gameObject.GetComponent<>().Select();
                    //hit.collider.gameObject.GetComponent<TowerScript>().Select();
                }
                break;
            case ApplicationConst.LandTag:
                if (GameState == GameState.ChooseNewTowerPosition)
                {
                    CreateTower(hit.point);
                    HintPanel.SetActive(false);
                    //_isCreateTowerMode = false;
                    GameState = GameState.Playing;
                }
                break;
        }
        Debug.Log(GameState);
    }
    
    private void UnchooseAll() {
        //hide all chooseCircles
        var circles = GameObject.FindGameObjectsWithTag(ApplicationConst.ChooseCircleTag);
        foreach (var circleRendererL in circles.Select(x => x.GetComponent<Renderer>()))
        {
            circleRendererL.enabled = false;
        }
    }

    //private IEnumerator ProcessHit(RaycastHit hit)
    //{
    //    if (hit.collider.tag == ApplicationConst.TowerTag || hit.collider.tag == ApplicationConst.TentTag)
    //    {
    //        UnchooseAll();

    //        //show chooseCircle at selected object
    //        var circleRenderer = hit.transform.gameObject.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.gameObject.tag == ApplicationConst.ChooseCircleTag);
    //        if (circleRenderer != null)
    //            circleRenderer.enabled = !circleRenderer.enabled;
    //    }
    //    yield return 0;
    //}

    public void BackToMenu()
    {
        Application.LoadLevel("MainMenu");
    }

    public GameObject HumburgerButton;
    public GameObject SidePanel;
    public GameObject CreateTowerButton;
    public GameObject CreateKnightButton;

    public void ProcessHamburgerButton()
    {
        HumburgerButton.SetActive(!HumburgerButton.activeSelf);
        SidePanel.SetActive(!SidePanel.activeSelf);
        //CreateKnightButton.SetActive(false);
        //CreateTowerButton.SetActive(true);
    }

    public GameObject TowerPrefab;
    public GameObject KnightPrefab;
    public GameObject HintPanel;

    //private bool _isCreateTowerMode = false;
    //private bool _isCreateKnightMode = false;

    private void CreateTower(Vector3 targetTowerPosition)
    {
        //var position = new Vector3(targetTowerPosition.x, targetTowerPosition.y + 2.95F, targetTowerPosition.z);
        var position = targetTowerPosition;
        Instantiate(TowerPrefab, position, Quaternion.identity);
    }

    public void ProcessCreateTowerButton()
    {
        ProcessHamburgerButton();
        HintPanel.SetActive(true);
        //_isCreateTowerMode = true;
        GameState = GameState.ChooseNewTowerPosition;
    }

    private void CreateKnight(GameObject tent)
    {
        var knight = KnightPrefab;
        knight.transform.localScale.Set(1, 1, 1);
        var knightPosition = new Vector3(tent.transform.position.x + 1.6F, tent.transform.position.y, tent.transform.position.z - 2.5F);
        Instantiate(knight, knightPosition, Quaternion.identity);
    }

    public void ProcessCreateKnightButton()
    {
        ProcessHamburgerButton();
        HintPanel.SetActive(true);
        //HintPanel.FindChild("HintText").SetText("TAP ON TENT TO CREATE KNIGHT")
        //_isCreateKnightMode = true;
        GameState = GameState.ChooseNewKnightPosition;
    }

    public void HideSidePanel() {
        SidePanel.SetActive(false);
        HumburgerButton.SetActive(true);
        UnchooseAll();
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



