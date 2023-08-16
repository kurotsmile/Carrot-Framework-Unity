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

        public Carrot_Button_Item create_item()
        {
            GameObject obj_btn_ext = Instantiate(this.item_app_other_exit_prefab);
            obj_btn_ext.transform.SetParent(this.area_list_app);
            obj_btn_ext.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_btn_ext.transform.localPosition = Vector3.zero;
            obj_btn_ext.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            return obj_btn_ext.GetComponent<Carrot_Button_Item>();
        }
    }
}
