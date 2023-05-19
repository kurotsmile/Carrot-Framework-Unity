using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Carrot
{
    public enum carrot_app_type {all,app,game}
    public class Carrot_list_app
    {
        private Carrot carrot;
        private bool is_ready_cache_app_other = false;

        private Carrot_Box box_list_app;
        private Carrot_Box_Btn_Item btn_header_all;
        private Carrot_Box_Btn_Item btn_header_game;
        private Carrot_Box_Btn_Item btn_header_app;

        private Carrot_Window_Exit window_exit;

        private List<GameObject> list_btn_gamepad;
        private carrot_app_type type = carrot_app_type.all;
        private string s_data_list_app;

        public Carrot_list_app(Carrot carrot)
        {
            this.carrot = carrot;
            this.s_data_list_app = PlayerPrefs.GetString("s_data_list_app","");
        }

        public void show_list_carrot_app()
        {
            this.carrot.play_sound_click();
            this.carrot.show_loading(act_get_list_app_carrot(true));
        }

        public void show_list_app_where_exit()
        {
            this.carrot.play_sound_click();
            this.list_btn_gamepad = new List<GameObject>();
            GameObject window_exit = this.carrot.create_window(this.carrot.window_exit_prefab);
            window_exit.name = "window_exit";
            this.window_exit = window_exit.GetComponent<Carrot_Window_Exit>();
            this.window_exit.txt_exit_msg.text = PlayerPrefs.GetString("exit_msg", "Are you sure you want to exit the application?\nPlease press the back button one more time to exit");
            this.window_exit.txt_title_app_other.text = PlayerPrefs.GetString("exit_app_other", "Perhaps you will enjoy our other applications");
            this.window_exit.panel_list_app_other.SetActive(false);
            this.window_exit.UI.set_theme(this.carrot.color_highlight);

            this.list_btn_gamepad.Add(this.window_exit.UI.obj_gamepad[0]);
            this.list_btn_gamepad.Add(this.window_exit.UI.obj_gamepad[1]); 

            if (this.is_ready_cache_app_other)
                this.show_list_app_cache(false);
            else
                this.carrot.StartCoroutine(act_get_list_app_carrot(false));
        }

        private IEnumerator act_get_list_app_carrot(bool is_show_list)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.carrot.get_url(), this.carrot.frm_act("list_app_carrot")))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.show_list_app_cache(is_show_list);
                }
                else
                {
                    if (this.carrot.model_app == ModelApp.Develope) this.carrot.log("act_get_list_app_carrot:" + www.downloadHandler.text);
                    this.show_list_by_data(www.downloadHandler.text, is_show_list);
                    this.update_data_offline(www.downloadHandler.text);
                }
            }
        }

        private void update_data_offline(string s_data)
        {
            this.s_data_list_app = s_data;
            PlayerPrefs.SetString("s_data_list_app", s_data);
            this.is_ready_cache_app_other = true;
        }

        private void open_link(string s_link)
        {
            Application.OpenURL(s_link);
        }

        public void open_link_share(string s_link)
        {
            GameObject.Find("Carrot").GetComponent<Carrot>().show_share(s_link, PlayerPrefs.GetString("share_tip", "Choose the platform below to share this great app with your friends or others"));
        }

        private void show_list_app_cache(bool is_show_list)
        {
            if (this.s_data_list_app != "")
            {
                this.show_list_by_data(this.s_data_list_app, is_show_list);
            }
        }

        private void reset_all_color_btn_header()
        {
            this.btn_header_all.set_icon_color(Color.black);
            this.btn_header_app.set_icon_color(Color.black);
            this.btn_header_game.set_icon_color(Color.black);
        }

        private void act_btn_header_all_category()
        {
            this.type = carrot_app_type.all;
            this.reset_all_color_btn_header();
            this.btn_header_all.set_icon_color(this.carrot.color_highlight);
            this.carrot.send(this.frm_list_app_carrot_by_category(""),this.act_list_app_category_success,this.act_list_app_category_fail);
        }

        private void act_btn_header_app_category()
        {
            this.type = carrot_app_type.app;
            this.reset_all_color_btn_header();
            this.btn_header_app.set_icon_color(this.carrot.color_highlight);
            this.carrot.send(this.frm_list_app_carrot_by_category("mobile_application"), this.act_list_app_category_success, this.act_list_app_category_fail);
        }

        private void act_btn_header_game_category()
        {
            this.type = carrot_app_type.game;
            this.reset_all_color_btn_header();
            this.btn_header_game.set_icon_color(this.carrot.color_highlight);
            this.carrot.send(this.frm_list_app_carrot_by_category("mobile_game"), this.act_list_app_category_success, this.act_list_app_category_fail);
        }

        private WWWForm frm_list_app_carrot_by_category(string s_type)
        {
            WWWForm frm=this.carrot.frm_act("list_app_carrot");
            if(s_type!="") frm.AddField("type", s_type);
            return frm;
        }

        private void act_list_app_category_success(string s_data)
        {
            this.show_list_by_data(s_data, true);
        }

        private void act_list_app_category_fail(string s_error)
        {
            this.show_list_app_cache(true);
        }

        private void show_list_by_data(string s_data,bool is_show_list)
        {
            if (is_show_list)
            {
                if (this.box_list_app != null) this.box_list_app.close();
                this.list_btn_gamepad = new List<GameObject>();
                this.carrot.hide_loading();
                this.box_list_app = this.carrot.Create_Box();
                box_list_app.set_icon(this.carrot.icon_carrot);
                box_list_app.set_title(PlayerPrefs.GetString("list_app_carrot", "Applications from the developer"));

                this.btn_header_all = box_list_app.create_btn_menu_header(this.carrot.icon_carrot_all_category);
                this.btn_header_all.set_act(this.act_btn_header_all_category);
                if(this.type==carrot_app_type.all)this.btn_header_all.set_icon_color(this.carrot.color_highlight);

                this.btn_header_app = box_list_app.create_btn_menu_header(this.carrot.icon_carrot_app);
                this.btn_header_app.set_act(this.act_btn_header_app_category);
                if (this.type == carrot_app_type.app) this.btn_header_app.set_icon_color(this.carrot.color_highlight);

                this.btn_header_game = box_list_app.create_btn_menu_header(this.carrot.icon_carrot_game);
                this.btn_header_game.set_act(this.act_btn_header_game_category);
                if (this.type == carrot_app_type.game) this.btn_header_game.set_icon_color(this.carrot.color_highlight);

                this.list_btn_gamepad.Add(this.btn_header_all.gameObject);
                this.list_btn_gamepad.Add(this.btn_header_app.gameObject);
                this.list_btn_gamepad.Add(this.btn_header_game.gameObject);
            }
            else
            {
                this.carrot.clear_contain(this.window_exit.area_list_app);
                this.window_exit.panel_list_app_other.SetActive(true);
            }

            IList list_app_carrot = (IList)Json.Deserialize(s_data);

            for (int i = 0; i < list_app_carrot.Count; i++)
            {
                IDictionary item_data = (IDictionary)list_app_carrot[i];
                string id_app = item_data["id_app"].ToString();

                if (is_show_list)
                {
                    var s_link = item_data["url"].ToString();
                    var s_link_carrot = item_data["link_carrot_app"].ToString();

                    Carrot_Box_Item item_app = this.box_list_app.create_item("app_item_" + i);
                    item_app.set_title(item_data["name"].ToString());
                    if (this.type == carrot_app_type.all)
                        item_app.set_tip("Download And Play");
                    else if (this.type == carrot_app_type.app)
                        item_app.set_tip("App");
                    else
                        item_app.set_tip("Game");

                    Sprite sp_avatar_app = this.carrot.get_tool().get_sprite_to_playerPrefs("carrot_list_app_icon_" + id_app);
                    if (sp_avatar_app != null)
                        item_app.set_icon_white(sp_avatar_app);
                    else
                        this.carrot.get_img_and_save_playerPrefs(item_data["icon"].ToString(), item_app.img_icon, "carrot_list_app_icon_" + id_app);
                    item_app.set_act(() => this.open_link(s_link));

                    Carrot_Box_Btn_Item app_btn_download = item_app.create_item();
                    app_btn_download.set_icon(this.carrot.icon_carrot_download);
                    app_btn_download.set_color(this.carrot.color_highlight);
                    app_btn_download.set_act(() => this.open_link(s_link));

                    Carrot_Box_Btn_Item app_btn_share = item_app.create_item();
                    app_btn_share.set_icon(this.carrot.sp_icon_share);
                    app_btn_share.set_color(this.carrot.color_highlight);
                    app_btn_share.set_act(() => this.open_link_share(s_link_carrot));

                    Carrot_Box_Btn_Item app_btn_link = item_app.create_item();
                    app_btn_link.set_icon(this.carrot.icon_carrot_link);
                    app_btn_link.set_color(this.carrot.color_highlight);
                    app_btn_link.set_act(() => this.open_link(s_link_carrot));
                }
                else
                {
                    GameObject item_p;
                    item_p = UnityEngine.GameObject.Instantiate(this.window_exit.item_app_other_exit_prefab);
                    item_p.transform.SetParent(this.window_exit.area_list_app);
                    item_p.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_p.transform.localPosition = new Vector3(item_p.transform.localPosition.x, item_p.transform.localPosition.y, 0f);
                    item_p.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    item_p.GetComponent<Carrot_item_app>().link = item_data["url"].ToString();
                    item_p.GetComponent<Carrot_item_app>().link_carrot_app = item_data["link_carrot_app"].ToString();
                    this.list_btn_gamepad.Add(item_p);
                    Sprite sp_avatar_app = this.carrot.get_tool().get_sprite_to_playerPrefs("carrot_list_app_icon_" + id_app);
                    if (sp_avatar_app != null)
                        item_p.GetComponent<Carrot_item_app>().img_icon.sprite = sp_avatar_app;
                    else
                        this.carrot.get_img_and_save_playerPrefs(item_data["icon"].ToString(), item_p.GetComponent<Carrot_item_app>().img_icon, "carrot_list_app_icon_" + id_app);
                }
            }

            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(this.list_btn_gamepad);
                if (is_show_list)
                {
                    this.box_list_app.update_gamepad_cosonle_control();
                    this.box_list_app.update_color_table_row();
                    this.carrot.game.set_index_button_gamepad_console(3);
                    this.carrot.game.set_scrollRect_gamepad_consoles(this.box_list_app.UI.scrollRect);
                }
                else
                    this.carrot.game.set_index_button_gamepad_console(0);
            }
        }
    }
}