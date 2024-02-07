using Firebase.Extensions;
using Firebase.Firestore;
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

        private string s_data_json_share = "";

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.s_data_json_share = PlayerPrefs.GetString("s_data_json_share");
            this.txt_share_tip.text = PlayerPrefs.GetString("share_tip", "Choose the platform below to share this great app with your friends or others");
            this.UI.set_theme(this.carrot.color_highlight);
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.UI.get_list_btn());
            if (this.carrot.is_online())
            {
                if (this.carrot.s_data_json_share_temp == "")
                    this.load_list_share();
                else
                    this.load_list_share_by_data(this.carrot.s_data_json_share_temp);
            }
            else
                this.load_list_share_by_data(this.s_data_json_share);
        }

        public void show_list_app_share(string link_customer, string s_share_tip)
        {
            if (s_share_tip != "") this.txt_share_tip.text = s_share_tip;
            this.inp_link_share.text = link_customer;
            if (this.carrot.is_online())
            {
                if (this.carrot.s_data_json_share_temp == "")
                    this.load_list_share();
                else
                    this.load_list_share_by_data(this.carrot.s_data_json_share_temp);
            }
            else
                this.load_list_share_by_data(this.s_data_json_share);
        }

        private void load_list_share()
        {
            Query UserQuery = this.carrot.db.Collection("share");
            UserQuery.GetSnapshotAsync().ContinueWithOnMainThread(task => {
                QuerySnapshot ShareQuerySnapshot = task.Result;
                if (task.IsCompleted)
                {
                    if (ShareQuerySnapshot.Count > 0)
                    {
                        List<IDictionary> list_share = new List<IDictionary>();
                        string s_os_app = this.carrot.os_app.ToString().ToLower();
                        foreach (DocumentSnapshot document_share in ShareQuerySnapshot.Documents)
                        {
                            string s_id_share = document_share.Id;
                            IDictionary data_share = document_share.ToDictionary();
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
                            {
                                btn_share_item.set_icon_white(sp_icon_share);
                            }
                            else
                                this.carrot.get_img_and_save_playerPrefs(data_share["icon"].ToString(), btn_share_item.img_icon, s_id_share);

                            if (s_link_share != "") btn_share_item.set_act_click(() => act_click_btn_share(s_link_share));
                            list_share.Add(data_share);
                        };

                        this.s_data_json_share = Json.Serialize(list_share);
                        this.carrot.s_data_json_share_temp = this.s_data_json_share;
                        PlayerPrefs.SetString("s_data_json_share", this.s_data_json_share);
                    }
                }
            });
        }

        [ContextMenu("load_list_share_offline")]
        private void load_list_share_by_data(string s_data)
        {
            if (s_data == "") return;
            IList list_share = (IList)Json.Deserialize(s_data);
            string s_os_app = this.carrot.os_app.ToString().ToLower();
            for (int i = 0; i < list_share.Count; i++)
            {
                IDictionary data_share = (IDictionary)list_share[i];
                string s_id_share = data_share["id"].ToString();
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