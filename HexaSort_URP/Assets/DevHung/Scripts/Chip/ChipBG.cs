using UnityEngine;

namespace DevHung.Scripts
{
    public class ChipBG : MonoBehaviour
    {
        private StackChipController stackChipController;
        [SerializeField] private ParticleSystem vfxWin;
        public void OnInit(StackChipController _stackChipController)
        {
            stackChipController = _stackChipController;
        }

        public void Refresh()
        {
            vfxWin.gameObject.SetActive(false);
            Destroy(stackChipController.gameObject);
        }
        public StackChipController GetStackController()
        {
            return stackChipController;
        }

        public void FullStack()
        {
            vfxWin.gameObject.SetActive(true);
        }
    }
}