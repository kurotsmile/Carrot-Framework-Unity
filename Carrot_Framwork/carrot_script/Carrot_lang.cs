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
        private string s_lang_name;
        private string s_lang_key;
        public GameObject item_lang_prefab;
        public string[] key;
        public Text[] emp_val;

        private UnityAction<string> act_after_selecting_lang;
        private bool is_ready_list_lang_cache = false;
        private List<bool> list_cache_item_list_lang;
        private Carrot_item_lang item_lang_temp;
        private Carrot_Box box_lang;
        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.list_cache_item_list_lang = new List<bool>();
            this.s_lang_name = PlayerPrefs.GetString("lang_name", "English");
            this.s_lang_key = PlayerPrefs.GetString("lang", "en");
            this.load_icon_lang();
            this.load_lang_emp();
        }

        private void load_icon_lang()
        {
            Sprite sp_lang_icon = this.carrot.get_tool().get_sprite_to_playerPrefs("lang_icon_" + this.s_lang_key);
            if (sp_lang_icon == null) sp_lang_icon = this.sp_lang_default_en;
            if (this.carrot.emp_show_lang != null) for (int i = 0; i < this.carrot.emp_show_lang.List_img_change_icon_lang.Length; i++) this.carrot.emp_show_lang.List_img_change_icon_lang[i].sprite = sp_lang_icon;
        }

        public Sprite get_sp_lang_cur()
        {
            Sprite sp_lang_icon = this.carrot.get_tool().get_sprite_to_playerPrefs("lang_icon_" + this.s_lang_key);
            if (sp_lang_icon == null)
                return this.sp_lang_default_en;
            else
                return sp_lang_icon;
        }

        public void show_list_lang()
        {
            this.carrot.play_sound_click();
            this.act_after_selecting_lang = null;
            if (this.is_ready_list_lang_cache)
                this.show_list_lang_cache();
            else
            {
                WWWForm frm = this.carrot.frm_act("list_country");
                this.carrot.send(frm, act_list_lang, act_show_list_fail);
            }
        }

        private void act_show_list_fail(string s_error)
        {
            this.show_list_lang_cache();
        }

        public void show_list_lang(UnityAction<string> fnc_after_sel_lang)
        {
            this.show_list_lang();
            this.act_after_selecting_lang = fnc_after_sel_lang;
        }

        private void act_list_lang(string data)
        {
            IList list_lang = (IList)Json.Deserialize(data);
            this.box_lang = this.carrot.Create_Box(PlayerPrefs.GetString("sel_lang_app", "Choose your language and country"), this.icon);
            for (int i = 0; i < list_lang.Count; i++)
            {
                IDictionary data_lang = (IDictionary)list_lang[i];
                string lang_key_item = data_lang["key"].ToString();
                GameObject item_lang = Instantiate(this.item_lang_prefab);
                item_lang.transform.SetParent(box_lang.area_all_item);
                item_lang.transform.localScale = new Vector3(1f, 1f, 1f);
                item_lang.transform.localPosition = new Vector3(item_lang.transform.localPosition.x, item_lang.transform.localPosition.y, 0f);
                item_lang.transform.localRotation = Quaternion.Euler(Vector3.zero);
                item_lang.GetComponent<Carrot_item_lang>().txt_name.text = data_lang["name"].ToString();
                item_lang.GetComponent<Carrot_item_lang>().s_key_lang = lang_key_item;
                item_lang.GetComponent<Carrot_item_lang>().index = i;

                if (lang_key_item == this.s_lang_key)
                {
                    ColorBlock cb = item_lang.GetComponent<Button>().colors;
                    cb.normalColor = this.carrot.get_color_highlight_blur(50);
                    item_lang.GetComponent<Button>().colors = cb;
                    item_lang.GetComponent<Carrot_item_lang>().txt_name.color = Color.white;
                }
                if (data_lang["sel"].ToString() == "1") item_lang.GetComponent<Carrot_item_lang>().obj_suggestions.SetActive(true); else item_lang.GetComponent<Carrot_item_lang>().obj_suggestions.SetActive(false);
                carrot.get_img_and_save_playerPrefs(data_lang["icon"].ToString(), item_lang.GetComponent<Carrot_item_lang>().img_icon, "lang_icon_" + lang_key_item);
                if (PlayerPrefs.GetString("lang_data_" + lang_key_item, "") == "") item_lang.GetComponent<Carrot_item_lang>().obj_data_offline.SetActive(false);
                this.list_cache_item_list_lang.Add(false);
            }
            PlayerPrefs.SetString("data_list_lang_cache", data);
            this.is_ready_list_lang_cache = true;
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(box_lang.UI.get_list_btn());
        }

        private void show_list_lang_cache()
        {
            string s_list_lang_data = PlayerPrefs.GetString("data_list_lang_cache");
            IList list_lang = (IList)Json.Deserialize(s_list_lang_data);
            this.box_lang = this.carrot.Create_Box(PlayerPrefs.GetString("sel_lang_app", "Choose your language and country"), this.icon);
            for (int i = 0; i < list_lang.Count; i++)
            {

                IDictionary data_lang = (IDictionary)list_lang[i];
                string lang_key_item = data_lang["key"].ToString();
                GameObject item_lang = Instantiate(this.item_lang_prefab);
                item_lang.transform.SetParent(box_lang.area_all_item);
                item_lang.transform.localScale = new Vector3(1f, 1f, 1f);
                item_lang.transform.localPosition = new Vector3(item_lang.transform.localPosition.x, item_lang.transform.localPosition.y, 0f);
                item_lang.transform.localRotation = Quaternion.Euler(Vector3.zero);
                item_lang.GetComponent<Carrot_item_lang>().txt_name.text = data_lang["name"].ToString();
                item_lang.GetComponent<Carrot_item_lang>().s_key_lang = lang_key_item;
                item_lang.GetComponent<Carrot_item_lang>().index = i;
                if (lang_key_item == this.s_lang_key)
                {
                    ColorBlock cb = item_lang.GetComponent<Button>().colors;
                    cb.normalColor = this.carrot.get_color_highlight_blur(50);
                    item_lang.GetComponent<Button>().colors = cb;
                    item_lang.GetComponent<Carrot_item_lang>().txt_name.color = Color.white;
                }

                if (list_cache_item_list_lang.Count > 0) if (list_cache_item_list_lang[i]) item_lang.GetComponent<Carrot_item_lang>().obj_data_offline.GetComponent<Image>().color = carrot.color_highlight;

                if (data_lang["sel"].ToString() == "1") item_lang.GetComponent<Carrot_item_lang>().obj_suggestions.SetActive(true); else item_lang.GetComponent<Carrot_item_lang>().obj_suggestions.SetActive(false);
                item_lang.GetComponent<Carrot_item_lang>().img_icon.sprite = this.carrot.get_tool().get_sprite_to_playerPrefs("lang_icon_" + lang_key_item);
                item_lang.GetComponent<Carrot_item_lang>().img_icon.color = Color.white;
                if (PlayerPrefs.GetString("lang_data_" + lang_key_item, "") == "") item_lang.GetComponent<Carrot_item_lang>().obj_data_offline.SetActive(false);
            }
            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(box_lang.UI.get_list_btn());
            }
        }

        public void load_data_lang(Carrot_item_lang item_lang)
        {
            this.item_lang_temp = item_lang;

            if (this.list_cache_item_list_lang.Count > 0)
            {
                if (this.list_cache_item_list_lang[item_lang.index] == false)
                {
                    WWWForm frm_download_lang = carrot.frm_act("download_lang");
                    frm_download_lang.AddField("key", item_lang.s_key_lang);
                    carrot.send(frm_download_lang, act_load_data_lang, act_load_data_fail);
                }
                else
                {
                    this.act_load_data_lang_cache(this.item_lang_temp.s_key_lang);
                }
            }
            else
            {
                this.act_load_data_lang_cache(this.item_lang_temp.s_key_lang);
            }
        }

        private void act_load_data_fail(string s_error)
        {
            this.list_cache_item_list_lang[this.item_lang_temp.index] = true;
            this.act_load_data_lang_cache(this.item_lang_temp.s_key_lang);
        }

        private void act_load_data_lang(string s_data)
        {
            IDictionary data_lang = (IDictionary)Json.Deserialize(s_data);
            this.s_lang_key = data_lang["key"].ToString();
            this.list_cache_item_list_lang[this.item_lang_temp.index] = true;
            PlayerPrefs.SetString("lang_data_" + this.s_lang_key, s_data);
            this.load_data_for_app_and_framword(data_lang, s_data);
        }

        private void act_load_data_lang_cache(string s_key)
        {
            string s_data = PlayerPrefs.GetString("lang_data_" + s_key);
            IDictionary data_lang = (IDictionary)Json.Deserialize(s_data);
            this.load_data_for_app_and_framword(data_lang, s_data);
        }

        private void load_data_for_app_and_framword(IDictionary data_lang, string s_data)
        {
            this.s_lang_key = data_lang["key"].ToString();
            this.s_lang_name = this.item_lang_temp.txt_name.text;
            PlayerPrefs.SetString("lang", data_lang["key"].ToString());
            PlayerPrefs.SetString("lang_name", this.s_lang_name);
            if (data_lang["lang_framework"] != null) if (data_lang["lang_framework"].ToString() != "") this.save_data_lang(data_lang["lang_framework"].ToString());
            if (data_lang["lang_app"] != null) if (data_lang["lang_app"].ToString() != "") this.save_data_lang(data_lang["lang_app"].ToString());
            this.load_icon_lang();
            this.box_lang.close();

            if (this.act_after_selecting_lang != null)
            {
                this.act_after_selecting_lang(s_data);
                this.act_after_selecting_lang = none_func;
            }
        }

        private void save_data_lang(string s_data_lang_app)
        {
            IDictionary data_lang = (IDictionary)Json.Deserialize(s_data_lang_app);
            foreach (var key in data_lang.Keys) PlayerPrefs.SetString(key.ToString(), data_lang[key.ToString()].ToString());
            this.load_lang_emp();
        }

        public void dowwnload_lang_by_key(string s_key_lang)
        {
            WWWForm frm_download_lang = carrot.frm_act("dowwnload_lang_by_key");
            frm_download_lang.AddField("key", s_key_lang);
            carrot.send_hide(frm_download_lang, act_dowwnload_lang_by_key);
        }

        private void act_dowwnload_lang_by_key(string s_data_lang)
        {
            IDictionary data_lang = (IDictionary)Json.Deserialize(s_data_lang);
            this.s_lang_name = data_lang["lang_key"].ToString();
            if (data_lang["lang_framework"].ToString() != "") this.save_data_lang(data_lang["lang_framework"].ToString());
            if (data_lang["lang_app"] != null) if (data_lang["lang_app"].ToString() != "") this.save_data_lang(data_lang["lang_app"].ToString());
            this.load_icon_lang();
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

        private void none_func(string s_data) { }

        public string get_name_lang()
        {
            return this.s_lang_name;
        }

        public string get_key_lang()
        {
            return this.s_lang_key;
        }
    }
}
