using DevHung.Scripts.DesginPattern;

namespace DevHung.Scripts.Data
{
    public class PlayerData : MonoSingleton<PlayerData>
    {
        private int level;

        private void Start()
        {
            // PROPERTIES
            if (SecurePlayerPrefs.HasKey("Level"))
            {
                level = SecurePlayerPrefs.GetInt("Level");
            }
            else
            {
                level = 0;
                SecurePlayerPrefs.SetInt("Level", level);
            }
        }
        
        public void SetLevel()
        {
            level ++;
            SecurePlayerPrefs.SetInt("Level", level);
        }
        public int GetLevel()
        {
            return level;
        }
    }
}