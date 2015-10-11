using UnityEngine;

// Класс главного меню.
public class MainMenu : MonoBehaviour {
    
    public void Start()
    {
        QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
    }

    // Логическая переменная, отвечающая за нажатие кнопки Start.
	public bool is_start;
	// Строковая переменная, хранящая название сцены Start.
	public string start;
	// Логическая переменная, отвечающая за нажатие кнопки Statistics.
	public bool is_statistics;
	// Строковая переменная, хранящая название сцены Statistics.
	public string statistics;
	// Логическая переменная, отвечающая за нажатие кнопки Settings.
	public bool is_settings;
	// Строковая переменная, хранящая название сцены Settings.
	public string settings;
	// Логическая переменная, отвечающая за нажатие кнопки Exit.
	public bool is_exit;

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
	// объект с флагом is_start, то делается переход на сцену с именем
	// start. Аналогично происходит и с другими блоками if.
	void OnMouseUp() {
		if(is_start==true)
		{
			Application.LoadLevel(start);
		}
		if(is_statistics==true)
		{
			Application.LoadLevel(statistics);
		}
		if(is_settings==true)
		{
			Application.LoadLevel(settings);
		}
		if(is_exit==true)
		{
			Application.Quit();
		}
	}
}