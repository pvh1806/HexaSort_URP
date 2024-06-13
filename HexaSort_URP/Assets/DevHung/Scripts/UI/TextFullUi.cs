using TMPro;
using UnityEngine;

namespace DevHung.Scripts.UI
{
    public class TextFullUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        public void OnInit(string text)
        {
            textMeshProUGUI.SetText(text);
        }
        public void DestroyObj()
        {
            Destroy(gameObject);
        }
    }
}