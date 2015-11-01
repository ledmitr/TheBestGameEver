using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    public class EnemyHealth : MonoBehaviour
    {
        public int MaxHealth = 100;
        public int CurrentHealth = 100;
        
        public Image HealthBar;

        public float minValue = 0.0f;
        public float maxValue = 1.0f;
        public Color minColor = Color.red;
        public Color maxColor = Color.green;

        // Use this for initialization
        private void Start()
        {
            if (HealthBar == null)
                HealthBar = GetComponent<Image>();
        }

        // Update is called once per frame
        private void Update()
        {
            var newWidth = (float)CurrentHealth / MaxHealth;
            HealthBar.transform.localScale = new Vector3(newWidth, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
            //uncomment to enable color changing
            //HealthBar.color = Color.Lerp(minColor,maxColor,Mathf.Lerp(minValue,maxValue,transform.localScale.x));
        }
        
        public void AddjustCurrentHealth(int adj)
        {
            CurrentHealth += adj;

            if (CurrentHealth < 0)
                CurrentHealth = 0;

            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;

            if (MaxHealth < 1)
                MaxHealth = 1;
        }
    }
}