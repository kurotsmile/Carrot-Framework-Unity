using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Carrot
{
    [System.Serializable]
    public class On_Action_where_online : UnityEvent {};
    public class Carrot_lost_internet : MonoBehaviour
    {
        public Carrot carrot;
        public GameObject[] emp_online;
        public GameObject[] emp_offline;

        public On_Action_where_online act_when_online;
        public On_Action_where_online act_when_offline;

        public void set_model_by_status_internet(bool is_offline)
        {
            if (is_offline) {
                for (int i = 0; i < this.emp_offline.Length; i++) this.emp_offline[i].SetActive(true);
                for (int i = 0; i < this.emp_online.Length; i++) this.emp_online[i].SetActive(false);
                this.act_when_offline.Invoke();
            }
            else
            {
                for (int i = 0; i < this.emp_offline.Length; i++) this.emp_offline[i].SetActive(false);
                for (int i = 0; i < this.emp_online.Length; i++) this.emp_online[i].SetActive(true);
                this.act_when_online.Invoke();
            }
        }

        public void try_connect()
        {
            StartCoroutine(act_try_connect());
        }

        IEnumerator act_try_connect()
        {
            yield return new WaitForSeconds(5);
            if (this.carrot.is_offline())
            {
                StartCoroutine(act_try_connect());
            }
            else
            {
                this.StopAllCoroutines();
                this.set_model_by_status_internet(false);
            }
        }

        [ContextMenu("Active OffLine")]
        public void offline()
        {
            this.set_model_by_status_internet(true);
            this.carrot.set_status_online(false);
        }

        [ContextMenu("Active OnLine")]
        public void online()
        {
            this.set_model_by_status_internet(false);
            this.carrot.set_status_online(true);
        }
    }
}
