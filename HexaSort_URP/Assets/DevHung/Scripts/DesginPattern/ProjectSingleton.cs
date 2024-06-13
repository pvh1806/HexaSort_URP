namespace DevHung.Scripts
{
    public class ProjectSingleton<T> where T : class, new()
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
            set
            {
                if (value != null)
                    instance  = value;
                else instance = new T();
            }
        }
    }
}