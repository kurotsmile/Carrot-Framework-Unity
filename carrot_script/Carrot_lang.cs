using System.Collections;
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
        private string s_key_lang_temp = "";

        private UnityAction<string> act_after_selecting_lang = null;
        private Carrot_Box box_lang;
        private IDictionary data_lang_offline;
        private IDictionary data_lang_value = null;
        public string s_data_json_list_lang_offline;

        private Transform tr_item_lang_systemLanguage = null;

        public void On_load(Carrot carrot)
        {
            this.carrot = carrot;
            this.data_lang_offline = (IDictionary)Json.Deserialize("{}");
            if(this.carrot.is_offline()) this.s_data_json_list_lang_offline = PlayerPrefs.GetString("s_data_json_list_lang_offline");
            this.s_lang_key = PlayerPrefs.GetString("lang", "en");

            this.Load_val_data_lang();
            this.Load_icon_lang();
            this.Load_lang_emp();
        }

        private void Load_val_data_lang()
        {
            string s_data_lang = PlayerPrefs.GetString("data_lang_value_" + this.s_lang_key,"");
            if (s_data_lang != "")
            {
                Fire_Document fc = new(s_data_lang);
                this.data_lang_value = fc.Get_IDictionary();
            }

            if(this.data_lang_value!= null)
            {
                string s_data_lang_customer = PlayerPrefs.GetString("data_lang_customer_value_" + this.s_lang_key, "");
                if (s_data_lang_customer != "")
                {
                    Fire_Document fc = new(s_data_lang_customer);
                    IDictionary lang_data_customer = fc.Get_IDictionary();
                    foreach (var key in lang_data_customer.Keys) this.data_lang_value[key.ToString()] = lang_data_customer[key.ToString()].ToString();
                }
            }
        }

        private void Load_icon_lang()
        {
            Sprite sp_lang_icon = this.carrot.get_tool().get_sprite_to_playerPrefs("icon_" + this.s_lang_key) ?? this.sp_lang_default_en;
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
            this.carrot.show_loading();
            this.carrot.play_sound_click();
            this.act_after_selecting_lang = null;

            if (this.s_data_json_list_lang_offline== "")
            {
                StructuredQuery q = new("lang");
                this.carrot.server.Get_doc(q.ToJson(), Get_doc_done, Get_doc_fail);
            }
            else
            {
                this.Load_list_lang_by_data(this.s_data_json_list_lang_offline);
            }     
        }

        private void Get_doc_done(string s_data)
        {
            this.s_data_json_list_lang_offline = s_data;
            PlayerPrefs.SetString("s_data_json_list_lang_offline", s_data);
            this.Load_list_lang_by_data(s_data);
        }

        private void Load_list_lang_by_data(string s_data)
        {
            Fire_Collection fc = new(s_data);

            this.carrot.hide_loading();

            if (!fc.is_null)
            {
                this.box_lang = this.carrot.Create_Box(this.carrot.lang.Val("sel_lang_app", "Choose your language and country"), this.icon);
                foreach (var doc in fc.fire_document)
                {
                    IDictionary lang = doc.Get_IDictionary();
                    this.Add_item_to_list_box(lang);
                };

                if (this.tr_item_lang_systemLanguage != null) this.tr_item_lang_systemLanguage.SetSiblingIndex(0);
                if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.box_lang.UI.get_list_btn());
            }
        }

        private void Get_doc_fail(string s_error)
        {
            this.carrot.show_msg(s_error);
        }

        public void show_list_lang(UnityAction<string> fnc_after_sel_lang)
        {
            this.show_list_lang();
            this.act_after_selecting_lang = fnc_after_sel_lang;
        }

        private void Add_item_to_list_box(IDictionary data_lang)
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
                this.tr_item_lang_systemLanguage = item_lang.transform;
            }

            if (this.data_lang_offline != null)
            {
                if (this.data_lang_offline["lang_data_" + s_key] != null)
                {
                    Carrot_Box_Btn_Item btn_datat_offline = item_lang.create_item();
                    btn_datat_offline.set_icon(this.carrot.icon_carrot_database);
                    btn_datat_offline.set_color(this.carrot.color_highlight);
                    Destroy(btn_datat_offline.GetComponent<Button>());
                    item_lang.set_act(() => this.Select_lang_offline(s_key));
                }
                else
                {
                    item_lang.set_act(() => this.Select_lang(s_key));
                }
            }
            else
            {
                item_lang.set_act(() => this.Select_lang(s_key));
            }

        }

        public void Load_lang_emp()
        {
            if (this.carrot.emp_show_lang != null)
            {
                for (int i = 0; i < this.carrot.emp_show_lang.key.Length; i++) if(this.Val(this.carrot.emp_show_lang.key[i])!="") this.carrot.emp_show_lang.emp[i].text =this.Val(this.carrot.emp_show_lang.key[i]);
            }
        }

        public string get_key_lang()
        {
            return this.s_lang_key;
        }

        public void Select_lang(string s_key)
        {
            this.carrot.show_loading();
            this.s_key_lang_temp = s_key;
            this.carrot.server.Get_doc_by_path("lang_app",this.s_key_lang_temp, Act_sel_lang_done, Act_load_and_sel_fail);
        }

        private void Act_sel_lang_done(string s_data)
        {
            PlayerPrefs.SetString("data_lang_value_" + this.s_key_lang_temp, s_data);
            Fire_Document data_lang = new(s_data);
            this.data_lang_value = data_lang.Get_IDictionary();
            this.data_lang_offline["lang_data_" + this.s_key_lang_temp] = Json.Serialize(this.data_lang_value);
            if (this.carrot.collection_document_lang != "") this.carrot.server.Get_doc_by_path(this.carrot.collection_document_lang, this.s_key_lang_temp, Act_sel_lang_customer_done);
        }

        private void Act_load_and_sel_fail(string s_error)
        {
            this.Change_lang(this.s_key_lang_temp);
        }

        private void Act_sel_lang_customer_done(string s_data)
        {
            PlayerPrefs.SetString("data_lang_customer_value_" + this.s_key_lang_temp, s_data);
            Fire_Document data_lang = new(s_data);
            IDictionary lang_data_customer = data_lang.Get_IDictionary();
            foreach (var key in lang_data_customer.Keys) this.data_lang_value[key.ToString()] = lang_data_customer[key.ToString()].ToString();
            this.Change_lang(this.s_key_lang_temp);
            this.data_lang_offline["lang_data_customer_" + this.s_key_lang_temp] = Json.Serialize(lang_data_customer);
        }

        private void Select_lang_offline(string s_key)
        {
            this.data_lang_value=(IDictionary)Json.Deserialize(this.data_lang_offline["lang_data_" + s_key].ToString());
            if (this.carrot.collection_document_lang == "") this.Change_lang(s_key);

            if (this.carrot.collection_document_lang != "")
            {
                if (this.data_lang_offline["lang_data_customer_" + s_key] != null)
                {
                    IDictionary lang_data_customer = (IDictionary)Json.Deserialize(this.data_lang_offline["lang_data_customer_" + s_key].ToString());
                    foreach (var key in lang_data_customer.Keys) this.data_lang_value[key.ToString()] = lang_data_customer[key.ToString()];
                }
                this.Change_lang(s_key);
            }
        }

        private void Change_lang(string s_key_new)
        {
            this.carrot.hide_loading();
            this.s_lang_key = s_key_new;
            this.Load_icon_lang();
            this.Load_lang_emp();
            act_after_selecting_lang?.Invoke(this.s_lang_key);
            if (this.box_lang != null) this.box_lang.close();
            PlayerPrefs.SetString("lang", this.s_lang_key);
        }

        public string Val(string s_key,string s_default="")
        {
            if (data_lang_value != null)
            {
                if (s_key=="")
                {
                    return "";
                }else if (s_key == null)
                {
                    return "";
                }
                else
                {
                    if (this.data_lang_value[s_key] != null)
                        return this.data_lang_value[s_key].ToString();
                    else
                        return s_default;
                }
            }
            else
            {
                return s_default;
            }
        }
    }
}