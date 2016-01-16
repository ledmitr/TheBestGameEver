using UnityEngine;
using UnityEngine.UI;

namespace Assets.TD.scripts
{
    /// <summary>
    /// Отображает показатель здоровья и контролирует его состояние.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        private int _health;

        public void SetHealth(int health)
        {
            _health = health;
        }

        public int MaxHealth = 100;

        public Image HealthBar;

        public float MinValue = 0.0f;
        public float MaxValue = 1.0f;
        public Color MinColor = Color.red;
        public Color MaxColor = Color.green;
        
        // Use this for initialization
        private void Start()
        {
            _health = MaxHealth;
            if (HealthBar == null)
                HealthBar = GetComponent<Image>();
        }

        // Update is called once per frame
        private void Update()
        {
            var newWidth = (float)_health / MaxHealth;
            HealthBar.transform.localScale = new Vector3(newWidth, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
            //uncomment to enable color changing
            HealthBar.color = Color.Lerp(MinColor, MaxColor, Mathf.Lerp(MinValue, MaxValue, transform.localScale.x));
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
    }
}