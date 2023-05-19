using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Exit : MonoBehaviour
    {
        public Text txt_exit_msg;
        public Text txt_title_app_other;
        public Transform area_list_app;
        public GameObject panel_list_app_other;
        public GameObject item_app_other_exit_prefab;
        public Carrot_UI UI;

        public void btn_close()
        {
            this.UI.close();
        }

        public void btn_exit_app()
        {
            GameObject.Find("Carrot").GetComponent<Carrot>().app_exit();
        }
    }
}
