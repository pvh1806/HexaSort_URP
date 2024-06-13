using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevHung.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform posMainCamera;
        private List<int> listNumberHexaEmpty = new List<int> { 3, 5, 8, 11, 13 };

        public void SetMainCamera(int index)
        {
            if (listNumberHexaEmpty.Contains(index))
            {
                int offSetY = listNumberHexaEmpty.IndexOf(index);
               SetPosCam(offSetY);
            }
        }

        public void SetUpOnAwake(int index)
        {
            int closestValue = listNumberHexaEmpty[0];
            int closestDifference = Math.Abs(index - closestValue);
            foreach (int value in listNumberHexaEmpty)
            {
                int currentDifference = Math.Abs(index - value);
                if (currentDifference < closestDifference)
                {
                    closestValue = value;
                    closestDifference = currentDifference;
                }
            }

            int offSetY = listNumberHexaEmpty.IndexOf(closestValue);
            SetPosCam(offSetY);
        }

        private void SetPosCam(int index)
        {
            posMainCamera.transform.rotation = Quaternion.Euler(45.8f + 0.2f * (index + 1), 45, 0);
        }
    }
    
}