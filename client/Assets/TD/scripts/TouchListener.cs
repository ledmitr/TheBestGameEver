﻿using System.Linq;
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
            if (MessagePanel.activeSelf)
            {
                BackToGameFromMessageCanvas();
                return;
            }
            else
            {
                /*SidePanel.SetActive(false);
                HumburgerButton.SetActive(false);
                HintPanel.SetActive(false);*/
                MessagePanel.SetActive(true);
                return;
            }
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
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToGameFromMessageCanvas()
    {
        /*SidePanel.SetActive(false);
        HumburgerButton.SetActive(true);
        HintPanel.SetActive(false);*/
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
                }
                break;
            case ApplicationConst.LandTag:
                if (GameState == GameState.ChooseNewTowerPosition)
                {
                    CreateTower(hit.point);
                    HintPanel.SetActive(false);
                    GameState = GameState.Playing;
                }
                break;
        }
        Debug.Log(GameState);
    }

    private void SetHint(string hintText)
    {
        var text = HintPanel.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = hintText;
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
    }

    public GameObject TowerPrefab;
    public GameObject KnightPrefab;
    public GameObject HintPanel;
    
    private void CreateTower(Vector3 targetTowerPosition)
    {
        //var position = new Vector3(targetTowerPosition.x, targetTowerPosition.y + 2.95F, targetTowerPosition.z);
        var position = targetTowerPosition;
        Instantiate(TowerPrefab, position, Quaternion.identity);
    }

    public void ProcessCreateTowerButton()
    {
        ProcessHamburgerButton();
        SetHint(ApplicationConst.CreateTowerHint);
        HintPanel.SetActive(true);
        GameState = GameState.ChooseNewTowerPosition;
    }

    private void CreateKnight(GameObject tent)
    {
        var knightPrefab = KnightPrefab;
        knightPrefab.transform.localScale.Set(1, 1, 1);
        var knightPosition = new Vector3(tent.transform.position.x + 1.6F, tent.transform.position.y, tent.transform.position.z - 2.5F);
        var knight = (GameObject)Instantiate(knightPrefab, knightPosition, Quaternion.identity);

        var tentScript = tent.GetComponent<TentScript>();
        var knightScript = knight.GetComponent<KnightScript>();
        knightScript.SetPath(tentScript.GetPath());
    }

    public void ProcessCreateKnightButton()
    {
        ProcessHamburgerButton();
        SetHint(ApplicationConst.CreateKnightHint);
        HintPanel.SetActive(true);
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



