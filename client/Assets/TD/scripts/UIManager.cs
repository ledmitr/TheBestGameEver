using System.Collections;
using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    public class UIManager : MonoBehaviour
    {
        public GameObject MainCamera;
        
        public GameObject LoadingPanel;
        public Text LoadingDetails;

        public GameObject StatBar;
        public Text StatBarCoins;
        public Text StatBarKilled;

        public GameObject PreparingStartBar;
        private Text _preparingTime;

        public GameObject HumburgerButton;
        public GameObject CreateTowerButton;
        public GameObject CreateKnightButton;

        public GameObject SidePanel;
        public GameObject HintPanel;

        public GameObject MessagePanel;
        public GameObject MessageExitButton;
        public Text MessageText;

        public GameObject GoToMainUnitButton;
        private string TentText = "TENT";
        private string FortressText = "FORT\nRESS";
        private string _preparingTimeString = "Game will start in {0} sec";

        private void Start()
        {
            HumburgerButton.SetActive(false);
            SidePanel.SetActive(false);
            HintPanel.SetActive(false);
            StatBar.SetActive(false);
            PreparingStartBar.SetActive(false);
            GoToMainUnitButton.SetActive(false);
        }

        public void SetUiActive()
        {
            LoadingPanel.SetActive(false);
            HumburgerButton.SetActive(true);
            SidePanel.SetActive(false);
            HintPanel.SetActive(false);
            StatBar.SetActive(true);
            PreparingStartBar.SetActive(true);
        }

        public void SetLoadingDetails(string text)
        {
            LoadingDetails.text = text;
        }

        public void SetPreparingTime(int secToPrepare)
        {
            if (_preparingTime == null)
            {
                _preparingTime = PreparingStartBar.GetComponentInChildren<Text>();
            }
            if (_preparingTime != null)
            {
                _preparingTime.text = string.Format(_preparingTimeString, secToPrepare);
                StartCoroutine(PreparingTimeUpdate(secToPrepare));
            }
        }

        private IEnumerator PreparingTimeUpdate(int secToPrepare)
        {
            for (int i = secToPrepare; i > 0; i--)
            {
                _preparingTime.text = string.Format(_preparingTimeString, i);
                yield return new WaitForSeconds(1);
            }
        }

        public void UpdateGoToMainUnitButton(PlayerRole role)
        {
            GoToMainUnitButton.SetActive(true);
            var textComponent = GoToMainUnitButton.GetComponentInChildren<Text>();
            textComponent.text = role == PlayerRole.Attacker ? TentText : FortressText;
        }

        public void ProcessGoToMainUnit()
        {
            var cameraScriptComponent = MainCamera.GetComponent<ViewDrag>();
            cameraScriptComponent.SetCameraPosition(GameInfo.MainUnitCoords);
        }

        private void Update()
        {
            UpdateStatBar();
        }

        private void UpdateStatBar()
        {
            StatBarCoins.text = GameInfo.CoinsAmount.ToString();
            StatBarKilled.text = GameInfo.KilledAmount.ToString();
        }

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

        /// <summary>
        /// Скрывает боковую панель.
        /// </summary>
        public void HideSidePanel()
        {
            SidePanel.SetActive(false);
            HumburgerButton.SetActive(true);
            //UnselectAll();
        }
        
        public void HideHintPanel()
        {
            HintPanel.SetActive(false);
        }

        public void ShowHintPanel()
        {
            HintPanel.SetActive(true);
        }

        public void SetHint(string hintText)
        {
            var text = HintPanel.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = hintText;
            }
        }

        public void ProcessEscapeButton()
        {
            MessagePanel.SetActive(!MessagePanel.activeSelf);
        }

        public void ProcessFinish()
        {
            MessagePanel.SetActive(true);
            MessageExitButton.SetActive(false);
            MessageText.text = ApplicationConst.GameFinishedStr;
        }

        public void UpdateButtonState(UnitType unitType)
        {
            bool isKnight = unitType == UnitType.Knight;
            int unitCost = isKnight ? ApplicationConst.KnightCost : ApplicationConst.TowerCost;
            GameObject createButton = isKnight ? CreateKnightButton : CreateTowerButton;
            var buttonComponent = createButton.GetComponent<Button>();
            if (buttonComponent != null)
                buttonComponent.interactable = GameInfo.CoinsAmount > unitCost;
        }
    }
}
