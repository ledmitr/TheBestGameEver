using System.Collections;
using System.Linq;
using Assets.TD.scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TouchListener : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        SidePanel.SetActive(false);
        HumburgerButton.SetActive(true);
        HintPanel.SetActive(false);
        //CreateKnightButton.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            //declare a variable of RaycastHit struct
            RaycastHit hit;
            //Create a Ray on the tapped / clicked position
            Ray ray;
            //for unity editor
#if UNITY_EDITOR
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //for touch device
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif
            //Check if the ray hits any collider
            if (Physics.Raycast(ray, out hit))
            {
                if (_isCreateTowerMode && hit.collider.tag == ApplicationConst.FieldTag)
                {
                    CreateTower(hit.transform.position);
                    HintPanel.SetActive(false);
                    _isCreateTowerMode = false;
                } else if (_isCreateKnightMode && hit.collider.tag == ApplicationConst.TentTag)
                {
                    CreateKnight(hit.collider.gameObject);
                    HintPanel.SetActive(false);
                    _isCreateKnightMode = false;
                }
                else
                {
                    StartCoroutine(ProcessHit(hit));
                }
            }
        }
    }


    private void UnchooseAll() {
        //hide all chooseCircles
        var circles = GameObject.FindGameObjectsWithTag(ApplicationConst.ChooseCircleTag);
        foreach (var circleRendererL in circles.Select(x => x.GetComponent<Renderer>()))
        {
            circleRendererL.enabled = false;
        }
    }

    private IEnumerator ProcessHit(RaycastHit hit)
    {
        if (hit.collider.tag == ApplicationConst.TowerTag || hit.collider.tag == ApplicationConst.TentTag)
        {
            UnchooseAll();

            //show chooseCircle at selected object
            var circleRenderer = hit.transform.gameObject.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.gameObject.tag == ApplicationConst.ChooseCircleTag);
            if (circleRenderer != null)
                circleRenderer.enabled = !circleRenderer.enabled;

            /*if (hit.collider.tag == ApplicationConst.TentTag) {
                CreateKnightButton.SetActive(true);
                HumburgerButton.SetActive(false);
                SidePanel.SetActive(true);
            }*/
        }
        yield return 0;
    }

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

    private bool _isCreateTowerMode = false;
    private bool _isCreateKnightMode = false;

    private void CreateTower(Vector3 targetTowerPosition)
    {
        var position = new Vector3(targetTowerPosition.x, targetTowerPosition.y + 5.0F, targetTowerPosition.z);
        Instantiate(TowerPrefab, position, Quaternion.identity);
    }

    public void ProcessCreateTowerButton()
    {
        ProcessHamburgerButton();
        HintPanel.SetActive(true);
        _isCreateTowerMode = true;
    }

    private void CreateKnight(GameObject tent)
    {
        var knight = KnightPrefab;
        knight.transform.localScale.Set(1, 1, 1);
        var knightPosition = new Vector3(tent.transform.position.x, 0.5F, tent.transform.position.z - 3);
        Instantiate(knight, knightPosition, Quaternion.identity);
    }

    public void ProcessCreateKnightButton()
    {
        ProcessHamburgerButton();
        HintPanel.SetActive(true);
        //HintPanel.FindChild("HintText").SetText("TAP ON TENT TO CREATE KNIGHT")
        _isCreateKnightMode = true;
    }

    public void HideSidePanel() {
        SidePanel.SetActive(false);
        HumburgerButton.SetActive(true);
        UnchooseAll();
    }
}



