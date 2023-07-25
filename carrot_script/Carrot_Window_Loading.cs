using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Loading : MonoBehaviour
    {
        public Carrot_UI UI;
        [Header("Loading Obj")]
        public Slider slider_loading;
        public GameObject obj_btn_close_session;
        private IEnumerator session_loading=null;
        private UnityAction act_cancel_session;

        public void show_process()
        {
            this.gameObject.SetActive(true);
            this.slider_loading.gameObject.SetActive(true);
        }

        public void set_session(IEnumerator session)
        {
            this.session_loading = session;
            this.obj_btn_close_session.SetActive(true);
            StartCoroutine(this.session_loading);
        }

        public void set_act_cancel_session(UnityAction act)
        {
            this.act_cancel_session = act;
        }

        public void close()
        {
            if (this.act_cancel_session != null) this.act_cancel_session();
            if (session_loading != null) StopCoroutine(this.session_loading);
            UI.close();
        }
    }
}

