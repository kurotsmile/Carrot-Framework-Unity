using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Share : MonoBehaviour
    {
        public Carrot_UI UI;
        private Carrot carrot;
        public Text txt_share_tip;
        public InputField inp_link_share;
        public Transform area_all_item_share;
        public GameObject item_share_prefab;

        private int length_app_share = 0;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.length_app_share = PlayerPrefs.GetInt("carrot_list_app_share_length", 0);
            this.txt_share_tip.text = PlayerPrefs.GetString("share_tip", "Choose the platform below to share this great app with your friends or others");
            this.inp_link_share.text = this.carrot.link_share_app;
            if (this.carrot.get_status_cache_app_share())
            {
                show_list_share_cache("");
            }
            else
            {
                WWWForm frm = this.carrot.frm_act("list_share");
                this.carrot.send_hide(frm, get_list_share, show_list_share_cache);
            }
            this.UI.set_theme(this.carrot.color_highlight);
        }

        public void show_list_app_share(string link_customer, string s_share_tip)
        {
            if (s_share_tip != "") this.txt_share_tip.text = s_share_tip;
            this.inp_link_share.text = link_customer;
            if (this.carrot.get_status_cache_app_share())
                show_list_share_cache("");
            else
            {
                WWWForm frm = this.carrot.frm_act("list_share");
                this.carrot.send_hide(frm, get_list_share, show_list_share_cache);
            }
        }

        private void get_list_share(string s_data)
        {
            this.carrot.clear_contain(this.area_all_item_share);
            IList list_share = (IList)Json.Deserialize(s_data);
            for (int i = 0; i < list_share.Count; i++)
            {
                IDictionary data_share = (IDictionary)list_share[i];
                GameObject item_share = UnityEngine.GameObject.Instantiate(this.item_share_prefab);
                item_share.transform.SetParent(this.area_all_item_share);
                item_share.transform.localPosition = new Vector3(item_share.transform.position.x, item_share.transform.position.y, 0f);
                item_share.transform.localScale = new Vector3(1f, 1f, 1f);
                item_share.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                item_share.GetComponent<Button>().onClick.AddListener(() => act_click_btn_share(data_share["url"].ToString()));
                this.carrot.get_img_and_save_playerPrefs(data_share["icon"].ToString(), item_share.GetComponent<Image>(), "carrot_share_app_icon_" + i);
                PlayerPrefs.SetString("carrot_share_app_url_" + i, data_share["url"].ToString());
            }
            PlayerPrefs.SetInt("carrot_list_app_share_length", list_share.Count);
            this.length_app_share = list_share.Count;
            this.carrot.set_status_cache_app_share(true);
            if(this.carrot.type_control!=TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.UI.get_list_btn());
        }

        private void show_list_share_cache(string s_error)
        {
            this.carrot.clear_contain(this.area_all_item_share);
            for (int i = 0; i < this.length_app_share; i++)
            {
                GameObject item_share = UnityEngine.GameObject.Instantiate(this.item_share_prefab);
                item_share.transform.SetParent(this.area_all_item_share);
                item_share.transform.localPosition = new Vector3(item_share.transform.position.x, item_share.transform.position.y, 0f);
                item_share.transform.localScale = new Vector3(1f, 1f,1f);
                item_share.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                string s_url = PlayerPrefs.GetString("carrot_share_app_url_" + i);
                item_share.GetComponent<Button>().onClick.AddListener(() => act_click_btn_share(s_url));
                item_share.GetComponent<Image>().sprite = this.carrot.get_tool().get_sprite_to_playerPrefs("carrot_share_app_icon_" + i);
                item_share.GetComponent<Image>().color = Color.white;
            }
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.UI.get_list_btn());
        }

        private void act_click_btn_share(string url)
        {
            string link_share = url.Replace("{url}", this.inp_link_share.text);
            Application.OpenURL(link_share);
            if (this.carrot.model_app == ModelApp.Develope) Debug.Log("Share open link:" + link_share);
        }

        public void close()
        {
            this.UI.close();
        }
    }
}
