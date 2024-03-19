using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Share : MonoBehaviour
    {
        public Carrot_UI UI;
        private Carrot carrot;
        public Text txt_share_title;
        public Text txt_share_tip;
        public InputField inp_link_share;
        public Transform area_all_item_share;
        public GameObject item_share_prefab;

        private string s_data_json_share_offline = "";

        public void load(Carrot carrot)
        {
            this.carrot = carrot;

            if (this.carrot.is_offline()) this.s_data_json_share_offline = PlayerPrefs.GetString("s_data_json_share_offline");
            this.txt_share_title.text = this.carrot.lang.Val("share", "Share");
            this.txt_share_tip.text = this.carrot.lang.Val("share_tip", "Choose the platform below to share this great app with your friends or others");
            this.UI.set_theme(this.carrot.color_highlight);
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.UI.get_list_btn());
            this.get_and_load_list_share();
        }

        public void show_list_app_share(string link_customer, string s_share_tip)
        {
            if (s_share_tip != "") this.txt_share_tip.text = s_share_tip;
            this.inp_link_share.text = link_customer;
            this.get_and_load_list_share();
        }

        private void get_and_load_list_share()
        {
            if (this.s_data_json_share_offline == "")
            {
                StructuredQuery q = new("share");
                this.carrot.server.Get_doc(q.ToJson(), get_list_share_done, get_list_share_fail);
            }
            else
                this.load_list_share(s_data_json_share_offline);
        }

        private void get_list_share_done(string s_data)
        {
            PlayerPrefs.SetString("s_data_json_share_offline", s_data);
            this.s_data_json_share_offline = s_data;
            this.load_list_share(s_data);
        }

        private void get_list_share_fail(string s_error)
        {
            if (this.s_data_json_share_offline != "") this.load_list_share(this.s_data_json_share_offline);
        }

        private void load_list_share(string s_data)
        {
            Fire_Collection fc = new(s_data);
            if (!fc.is_null)
            {
                string s_os_app = this.carrot.os_app.ToString().ToLower();
                for (int i = 0; i < fc.fire_document.Length; i++)
                {
                    IDictionary data_share = fc.fire_document[i].Get_IDictionary();
                    string s_id_share = data_share["id"].ToString();
                    data_share["id"] = s_id_share;
                    GameObject item_share = UnityEngine.GameObject.Instantiate(this.item_share_prefab);
                    item_share.transform.SetParent(this.area_all_item_share);
                    item_share.transform.localPosition = new Vector3(item_share.transform.position.x, item_share.transform.position.y, 0f);
                    item_share.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_share.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    Carrot_Button_Item btn_share_item = item_share.GetComponent<Carrot_Button_Item>();
                    btn_share_item.set_icon(this.carrot.sp_icon_share);
                    btn_share_item.img_icon.color = Color.black;

                    string s_link_share = "";
                    if (data_share[s_os_app] != null) s_link_share = data_share[s_os_app].ToString();
                    if (s_link_share == "") s_link_share = data_share["window"].ToString();

                    s_id_share = "share_" + s_id_share;
                    Sprite sp_icon_share = this.carrot.get_tool().get_sprite_to_playerPrefs(s_id_share);
                    if (sp_icon_share != null)
                        btn_share_item.set_icon_white(sp_icon_share);
                    else
                        this.carrot.get_img_and_save_playerPrefs(data_share["icon"].ToString(), btn_share_item.img_icon, s_id_share);

                    if (s_link_share != "") btn_share_item.set_act_click(() => act_click_btn_share(s_link_share));
                };
            }
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