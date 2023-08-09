using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_lang : MonoBehaviour
    {
        public Sprite icon;
        public Sprite sp_lang_default_en;
        private Carrot carrot;
        private string s_lang_key;
        public string[] key;
        public Text[] emp_val;

        private UnityAction<string> act_after_selecting_lang;
        private Carrot_Box box_lang;
        private IDictionary data_lang_offline;
        public string s_data_json_list_lang_offline;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.data_lang_offline =(IDictionary)Json.Deserialize("{}");
            this.s_data_json_list_lang_offline = PlayerPrefs.GetString("s_data_json_list_lang_offline");
            this.s_lang_key = PlayerPrefs.GetString("lang", "en");
            this.load_icon_lang();
            this.load_lang_emp();
        }

        private void load_icon_lang()
        {
            Sprite sp_lang_icon = this.carrot.get_tool().get_sprite_to_playerPrefs("icon_" + this.s_lang_key);
            if (sp_lang_icon == null) sp_lang_icon = this.sp_lang_default_en;
            if (this.carrot.emp_show_lang != null) for (int i = 0; i < this.carrot.emp_show_lang.List_img_change_icon_lang.Length; i++) this.carrot.emp_show_lang.List_img_change_icon_lang[i].sprite = sp_lang_icon;
        }

        public Sprite get_sp_lang_cur()
        {
            Sprite sp_lang_icon = this.carrot.get_tool().get_sprite_to_playerPrefs("icon_" + this.s_lang_key);
            if (sp_lang_icon == null)
                return this.sp_lang_default_en;
            else
                return sp_lang_icon;
        }

        public void show_list_lang()
        {
            this.carrot.play_sound_click();
            this.act_after_selecting_lang = null;

            if (this.carrot.s_data_json_list_lang_temp == "")
                load_data_list_lang_by_query(this.carrot.db.Collection("lang"));
            else
                load_data_list_lang_by_s_data(this.carrot.s_data_json_list_lang_temp);
        }

        private void load_data_list_lang_by_query(Query query_lang)
        {
            this.carrot.log("load_data_list_lang_by_query");
            query_lang.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot langQuerySnapshot = task.Result;
                if (task.IsCompleted)
                {
                    if (langQuerySnapshot.Count > 0)
                    {
                        this.box_lang = this.carrot.Create_Box(PlayerPrefs.GetString("sel_lang_app", "Choose your language and country"), this.icon);
                        List<IDictionary> list_lang = new List<IDictionary>();
                        foreach (DocumentSnapshot documentSnapshot in langQuerySnapshot.Documents)
                        {
                            IDictionary lang = documentSnapshot.ToDictionary();
                            lang["id"] = documentSnapshot.Id;
                            this.add_item_to_list_box(lang);
                            list_lang.Add(lang);
                        };

                        this.s_data_json_list_lang_offline = Json.Serialize(list_lang);
                        this.carrot.s_data_json_list_lang_temp = this.s_data_json_list_lang_offline;
                        PlayerPrefs.SetString("s_data_json_list_lang_offline", this.s_data_json_list_lang_offline);

                        if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.box_lang.UI.get_list_btn());
                    }
                }
            });
        }

        private void load_data_list_lang_by_s_data(string s_data)
        {
            this.carrot.log("load_data_list_lang_by_s_data");
            this.box_lang = this.carrot.Create_Box(PlayerPrefs.GetString("sel_lang_app", "Choose your language and country"), this.icon);
            IList list_lang = (IList) Json.Deserialize(s_data);
            for(int i=0;i<list_lang.Count;i++)
            {
                IDictionary lang = (IDictionary)list_lang[i];
                this.add_item_to_list_box(lang);
            };

            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.box_lang.UI.get_list_btn());
        }

        public void show_list_lang(UnityAction<string> fnc_after_sel_lang)
        {
            this.show_list_lang();
            this.act_after_selecting_lang = fnc_after_sel_lang;
        }

        private void add_item_to_list_box(IDictionary data_lang)
        {
            string s_id_lang = data_lang["id"].ToString();
            var s_key = data_lang["key"].ToString();
            string s_key_lang = data_lang["key"].ToString();
            Carrot_Box_Item item_lang = this.box_lang.create_item(s_id_lang);
            item_lang.set_icon(this.icon);
            if (data_lang["name"] != null) item_lang.set_title(data_lang["name"].ToString());
            item_lang.set_tip(s_key_lang);
            string s_id_icon_lang = "icon_" + s_key_lang;
            Sprite sp_icon_lang = this.carrot.get_tool().get_sprite_to_playerPrefs(s_id_icon_lang);
            if (sp_icon_lang != null)
                item_lang.set_icon_white(sp_icon_lang);
            else
                if (data_lang["icon"] != null) this.carrot.get_img_and_save_playerPrefs(data_lang["icon"].ToString(), item_lang.img_icon, s_id_icon_lang);

            if (s_key_lang == this.s_lang_key) item_lang.GetComponent<Image>().color = this.carrot.get_color_highlight_blur(50);

           

            if (Application.systemLanguage.ToString() == data_lang["name"].ToString())
            {
                Carrot_Box_Btn_Item btn_sugger_lang = item_lang.create_item();
                btn_sugger_lang.set_icon(this.carrot.icon_carrot_location);
                btn_sugger_lang.set_color(this.carrot.color_highlight);
                Destroy(btn_sugger_lang.GetComponent<Button>());
            }

            if (this.data_lang_offline!= null)
            {
                if (this.data_lang_offline["lang_data_" + s_key] != null)
                {
                    Carrot_Box_Btn_Item btn_datat_offline = item_lang.create_item();
                    btn_datat_offline.set_icon(this.carrot.icon_carrot_database);
                    btn_datat_offline.set_color(this.carrot.color_highlight);
                    Destroy(btn_datat_offline.GetComponent<Button>());
                    item_lang.set_act(() => this.select_lang_offline(s_key));
                }
                else
                {
                    item_lang.set_act(() => this.select_lang(s_key));
                }
            }
            else
            {
                item_lang.set_act(() => this.select_lang(s_key));
            }

        }

        public void load_lang_emp()
        {
            for (int i = 0; i < emp_val.Length; i++) if (PlayerPrefs.GetString(this.key[i], "") != "") this.emp_val[i].text = PlayerPrefs.GetString(this.key[i]);
            if (this.carrot.emp_show_lang != null)
            {
                for (int i = 0; i < this.carrot.emp_show_lang.key.Length; i++)
                    if (PlayerPrefs.GetString(this.carrot.emp_show_lang.key[i], "") != "") this.carrot.emp_show_lang.emp[i].text = PlayerPrefs.GetString(this.carrot.emp_show_lang.key[i]);
            }
        }

        public string get_key_lang()
        {
            return this.s_lang_key;
        }

        public void select_lang(string s_key)
        {
            DocumentReference langappRef = this.carrot.db.Collection("lang_app").Document(s_key);
            langappRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DocumentSnapshot collectionSnapshot = task.Result;
                    IDictionary lang_data= collectionSnapshot.ToDictionary();
                    foreach (var key in lang_data.Keys) PlayerPrefs.SetString(key.ToString(), lang_data[key.ToString()].ToString());
                    if(this.carrot.collection_document_lang == "") this.change_lang(s_key);
                    this.data_lang_offline["lang_data_"+s_key] = Json.Serialize(lang_data);
                }
            });


            if (this.carrot.collection_document_lang != "")
            {
                DocumentReference langRef = this.carrot.db.Collection(this.carrot.collection_document_lang).Document(s_key);
                langRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    DocumentSnapshot collectionSnapshot = task.Result;
                    if (task.IsCompleted)
                    {
                        if (collectionSnapshot.Exists)
                        {
                            IDictionary lang_data_customer = collectionSnapshot.ToDictionary();
                            foreach (var key in lang_data_customer.Keys) PlayerPrefs.SetString(key.ToString(), lang_data_customer[key.ToString()].ToString());
                            this.change_lang(s_key);
                            this.data_lang_offline["lang_data_customer_" + s_key] = Json.Serialize(lang_data_customer);
                        }
                        else
                        {
                            this.change_lang(s_key);
                        }
                    }

                    if (task.IsFaulted)
                    {
                        this.change_lang(s_key);
                    }
                });
            }
        }

        private void select_lang_offline(string s_key)
        {
            IDictionary lang_data = (IDictionary)Json.Deserialize(this.data_lang_offline["lang_data_"+s_key].ToString());
            foreach (var key in lang_data.Keys) PlayerPrefs.SetString(key.ToString(), lang_data[key.ToString()].ToString());
            if (this.carrot.collection_document_lang == "") this.change_lang(s_key);

            if (this.carrot.collection_document_lang != "")
            {
                if(this.data_lang_offline["lang_data_customer_" + s_key] != null) { 
                    IDictionary lang_data_customer=(IDictionary)Json.Deserialize(this.data_lang_offline["lang_data_customer_"+s_key].ToString());
                    foreach (var key in lang_data_customer.Keys) PlayerPrefs.SetString(key.ToString(), lang_data_customer[key.ToString()].ToString());
                }
                this.change_lang(s_key);
            }
        }

        private void change_lang(string s_key_new)
        {
            this.s_lang_key = s_key_new;
            this.load_icon_lang();
            this.load_lang_emp();
            if (this.act_after_selecting_lang != null) this.act_after_selecting_lang(this.s_lang_key);
            if (this.box_lang != null) this.box_lang.close();
            PlayerPrefs.SetString("lang", this.s_lang_key);
        }
    }
}
