using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class EnemyLockCollider : MonoBehaviour
    {

        // Use this for initialization
        public GameObject Player;
        public GameObject m_Camera;
        public List<GameObject> LocalEnemies;
        void Start()
        {
            LocalEnemies = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Player.transform.position;

            transform.rotation = Quaternion.Euler(0, m_Camera.transform.rotation.eulerAngles.y, 0);

        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.GetComponent<Enemy>())
            {
                LocalEnemies.Add(col.gameObject);
                //  Debug.Log(LocalEnemies.Count);


            }
        }
        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.GetComponent<Enemy>())
            {
                LocalEnemies.Remove(col.gameObject);
                Player.GetComponent<PUC>().LostLock(col.gameObject);
                //  Debug.Log(LocalEnemies.Count);
                // LocalEnemies.Remove(col.gameObject);
            }
        }



        //public List<GameObject>FindEnemies ()
    }
}
