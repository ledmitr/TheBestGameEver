using UnityEngine;
using System.Collections;

public class StatisticsMenu : MonoBehaviour {
	// Строковая переменная, хранящая название сцены Settings.
	public string back;
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

	// Метод, описывающий действия при нажатии на объект. 
	void OnMouseUp() {
		if(is_back==true)
		{
			Application.LoadLevel(back);
		}
	}

}
