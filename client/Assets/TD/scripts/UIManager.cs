using Assets.TD.scripts.Constants;
using Assets.TD.scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    public class UIManager : MonoBehaviour
    {
        public GameObject StatBar;
        public Text StatBarCoins;

        public GameObject PreparingStartBar;

        public GameObject HumburgerButton;
        public GameObject CreateTowerButton;
        public GameObject CreateKnightButton;

        public GameObject SidePanel;
        public GameObject HintPanel;

        public GameObject MessagePanel;
        public GameObject MessageExitButton;
        public Text MessageText;

        private void Start()
        {
            Debug.Assert(StatBar != null);
            Debug.Assert(PreparingStartBar != null);
            
            SidePanel.SetActive(false);
            HumburgerButton.SetActive(true);
            HintPanel.SetActive(false);

            StatBar.SetActive(false);
            PreparingStartBar.SetActive(false);
        }

        public void SetPreparingTime(int secToPrepare)
        {
            var textComponent = PreparingStartBar.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = string.Format("Game will start in {0} seconds", secToPrepare);
            }
        }

        private void Update()
        {
            UpdateStatBar();
        }

        private void UpdateStatBar()
        {
            StatBarCoins.text = GameInfo.CoinsAmount.ToString();
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
