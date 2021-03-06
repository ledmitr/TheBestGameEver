﻿using UnityEngine;

namespace Assets.TD.scripts
{
    public class SettingsMenu : MonoBehaviour
    {
        public GameObject MainMenuItems;
        public GameObject SettingsMenuItems;

        // Логическая переменная, отвечающая за нажатие кнопки High.
        public bool is_high;
        // Логическая переменная, отвечающая за нажатие кнопки Medium.
        public bool is_medium;
        // Логическая переменная, отвечающая за нажатие кнопки Low.
        public bool is_low;
        // Строковая переменная, хранящая название сцены Settings.
        //public string back;
        // Логическая переменная, отвечающая за нажатие кнопки Exit.
        public bool is_back;
	
        // Метод, описывающий действия при наведение курсора на объект.
        void OnMouseEnter() {
            // В данном случае, мы получаем объект и меняем его цвет на серый:
            GetComponent<Renderer>().material.color = Color.gray;
        }

        // Метод, описывающий действия при отпускании курсора с объекта.
        void OnMouseExit() {
            GetComponent<Renderer>().material.color = Color.white;
        }

        // Метод, описывающий действия при нажатии на объект. Если это 
        // объект с флагом is_high, то графика игры меняется на уровень
        // fantastic. Аналогично происходит и с другими блоками if. При
        // нажатии объекта с флагом is_back, возвращаемся в главное меню.
        void OnMouseUp() {
            if(is_high)
            {
                QualitySettings.SetQualityLevel(QualitySettings.names.Length-1, true);
            }
            if(is_medium)
            {
                if (QualitySettings.names.Length != 2)
                    QualitySettings.SetQualityLevel(QualitySettings.names.Length - 2);
            }
            if(is_low)
            {
                QualitySettings.SetQualityLevel(0, true);
            }
            if(is_back)
            {
                OnMouseExit();
                SettingsMenuItems.SetActive(false);
                MainMenuItems.SetActive(true);
                //Application.LoadLevel(back);
            }
        }
    }
}
