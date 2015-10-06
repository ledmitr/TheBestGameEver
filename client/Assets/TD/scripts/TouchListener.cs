using System.Collections;
using System.Linq;
using UnityEngine;

public class TouchListener : MonoBehaviour
{
    private const string ChooseCircleTagName = "chooseCircle";
    private const string TowerTagName = "Tower";
    // Use this for initialization
    private void Start()
    {
        _sidePanel.SetActive(false);
        _humburgerButton.SetActive(true);
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
                StartCoroutine(ProcessHit(hit));
            }
        }
    }

    private IEnumerator ProcessHit(RaycastHit hit)
    {
        if (hit.collider.tag == TowerTagName)
        {
            var circles = GameObject.FindGameObjectsWithTag(ChooseCircleTagName);
            foreach (var circleRendererL in circles.Select(x => x.GetComponent<Renderer>()))
            {
                circleRendererL.enabled = false;
                yield return 0;
            }

            var circleRenderer = hit.transform.gameObject.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.gameObject.tag == ChooseCircleTagName);
            if (circleRenderer != null)
                circleRenderer.enabled = !circleRenderer.enabled; 
        }
    }

    public void BackToMenu()
    {
        Application.LoadLevel("MainMenu");
    }

    public GameObject _humburgerButton;
    public GameObject _sidePanel;

    public void ProcessHamburgerButton()
    {
        _humburgerButton.SetActive(!_humburgerButton.activeSelf);
        _sidePanel.SetActive(!_sidePanel.activeSelf);
    }
}



