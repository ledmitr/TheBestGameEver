using System.Collections;
using System.Linq;
using Assets.TD.scripts;
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
                    _isCreateTowerMode = false;
                }
                else
                {
                    StartCoroutine(ProcessHit(hit));
                }
            }
        }
    }

    private void CreateTower(Vector3 targetTowerPosition)
    {
        var position = new Vector3(targetTowerPosition.x, targetTowerPosition.y + 5.0F, targetTowerPosition.z);
        Debug.Log(position);
        Instantiate(TowerPrefab, position, Quaternion.identity);
        HintPanel.SetActive(false);
    }

    private IEnumerator ProcessHit(RaycastHit hit)
    {
        if (hit.collider.tag == ApplicationConst.TowerTag)
        {
            var circles = GameObject.FindGameObjectsWithTag(ApplicationConst.ChooseCircleTag);
            foreach (var circleRendererL in circles.Select(x => x.GetComponent<Renderer>()))
            {
                circleRendererL.enabled = false;
                yield return 0;
            }

            var circleRenderer = hit.transform.gameObject.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.gameObject.tag == ApplicationConst.ChooseCircleTag);
            if (circleRenderer != null)
                circleRenderer.enabled = !circleRenderer.enabled; 
        }
    }

    public void BackToMenu()
    {
        Application.LoadLevel("MainMenu");
    }

    public GameObject HumburgerButton;
    public GameObject SidePanel;

    public void ProcessHamburgerButton()
    {
        HumburgerButton.SetActive(!HumburgerButton.activeSelf);
        SidePanel.SetActive(!SidePanel.activeSelf);
    }

    public GameObject TowerPrefab;
    public GameObject HintPanel;

    private bool _isCreateTowerMode = false;

    public void ProcessCreateTowerButton()
    {
        ProcessHamburgerButton();
        HintPanel.SetActive(true);
        _isCreateTowerMode = true;
    }

    private bool WasJustADamnedButton()
    {
        UnityEngine.EventSystems.EventSystem ct
              = UnityEngine.EventSystems.EventSystem.current;

        if (!ct.IsPointerOverGameObject()) return false;
        if (!ct.currentSelectedGameObject) return false;
        if (ct.currentSelectedGameObject.GetComponent<Button>() == null)
            return false;

        return true;
    }
}



