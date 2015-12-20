using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    public class EnemyHealth : MonoBehaviour
    {
        private int _health;

        public int MaxHealth = 100;
        public int StartHealth = 100;
        
        public Image HealthBar;

        public float MinValue = 0.0f;
        public float MaxValue = 1.0f;
        public Color MinColor = Color.red;
        public Color MaxColor = Color.green;

        // Use this for initialization
        private void Start()
        {
            _health = StartHealth;
            if (HealthBar == null)
                HealthBar = GetComponent<Image>();
        }

        // Update is called once per frame
        private void Update()
        {
            var newWidth = (float)_health / MaxHealth;
            HealthBar.transform.localScale = new Vector3(newWidth, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
            //uncomment to enable color changing
            //HealthBar.color = Color.Lerp(minColor,maxColor,Mathf.Lerp(minValue,maxValue,transform.localScale.x));
        }
        
        private void AdjustCurrentHealth(int adj)
        {
            _health += adj;

            if (_health <= 0)
            {
                Destroy(gameObject);
                return;
            }

            if (_health > MaxHealth)
                _health = MaxHealth;

            if (MaxHealth < 1)
                MaxHealth = 1;
        }
        
        public void Damage(int damageAmount)
        {
            AdjustCurrentHealth(-damageAmount);
        }

        public void Heal(int healAmount)
        {
            AdjustCurrentHealth(healAmount);
        }

        public void HealFull()
        {
            _health = MaxHealth;
        }
    }
}