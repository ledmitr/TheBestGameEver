using Assets.TD.scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    public class UIManager : MonoBehaviour
    {
        public GameObject StatBar;
        public GameObject PreparingStartBar;

        public GameObject HumburgerButton;
        public GameObject CreateTowerButton;
        public GameObject CreateKnightButton;

        public GameObject SidePanel;
        public GameObject HintPanel;
        public GameObject MessagePanel;

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

        private void Update()
        {
            
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

        /// <summary>
        /// 
        /// </summary>
        private void HideMessagePanel()
        {
            MessagePanel.SetActive(false);
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
            if (MessagePanel.activeSelf)
            {
                HideMessagePanel();
            }
            else
            {
                MessagePanel.SetActive(true);
            }
        }
    }
}
