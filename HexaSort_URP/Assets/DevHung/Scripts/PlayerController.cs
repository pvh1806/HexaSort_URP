using UnityEngine;

namespace DevHung.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            RaycastHit hit = CastRay();
            if (hit.collider == null) return;
            StackChipController x;
            x = hit.collider.CompareTag("Chip") ? hit.collider.GetComponentInParent<StackChipController>() : hit.collider.GetComponent<ChipBG>().GetStackController();
            if (!CacheGameData.Instance.IsCheckMove)
                CacheGameData.Instance.CheckStackListChip(x);
        }

        private RaycastHit CastRay()
        {
            Vector3 screenMousePosFar =
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Vector3 screenMousePosNear =
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
            Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
            RaycastHit hit;
            Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);
            return hit;
        }
    }
}