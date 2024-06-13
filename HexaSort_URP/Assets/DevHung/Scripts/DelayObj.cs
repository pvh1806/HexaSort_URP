using System;
using System.Collections;
using UnityEngine;

namespace DevHung.Scripts
{
    public class DelayObj : MonoBehaviour
    {
        [SerializeField] private Obj[] obj;

        private void Start()
        {
            for (int i = 0; i < obj.Length; i++)
            {
                StartCoroutine(SetActive(obj[i].obj, obj[i].timerDelay));
            }
        }

        private IEnumerator SetActive(GameObject _obj , float timerDelay)
        {
            yield return new WaitForSeconds(timerDelay);
            _obj.SetActive(true);
        }
    }

    [Serializable]
    public class Obj
    {
        public GameObject obj;
        public float timerDelay;
        
    }
}