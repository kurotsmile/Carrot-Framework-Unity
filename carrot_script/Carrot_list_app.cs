using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Carrot
{
    public enum carrot_app_type { all, app, game }
    public class Carrot_list_app
    {
        private Carrot carrot;

        private Carrot_Box box_list_app;
        private Carrot_Box_Btn_Item btn_header_all;
        private Carrot_Box_Btn_Item btn_header_game;
        private Carrot_Box_Btn_Item btn_header_app;

        private Carrot_Window_Exit window_exit;

        private List<GameObject> list_btn_gamepad;
        private carrot_app_type type = carrot_app_type.all;

        private string s_data_carrotapp_all;
        private string s_data_carrotapp_app;
        private string s_data_carrotapp_game;

        public Carrot_list_app(Carrot carrot)
        {
            this.carrot = carrot;
            this.s_data_carrotapp_all = PlayerPrefs.GetString("s_data_carrotapp_all");
            this.s_data_carrotapp_app = PlayerPrefs.GetString("s_data_carrotapp_app");
            this.s_data_carrotapp_game = PlayerPrefs.GetString("s_data_carrotapp_game");

            if (this.carrot.is_offline())
            {
                this.carrot.s_data_json_carrotapp_all_temp = this.s_data_carrotapp_all;
                this.carrot.s_data_json_carrotapp_app_temp = this.s_data_carrotapp_app;
                this.carrot.s_data_json_carrotapp_game_temp = this.s_data_carrotapp_game;
            }
        }

        [ContextMenu("show_list_carrot_app")]
        public void show_list_carrot_app()
        {
            this.carrot.play_sound_click();
            this.carrot.show_loading();
            if (this.type == carrot_app_type.all)
            {
                if (this.carrot.s_data_json_carrotapp_all_temp == "")
                {
                    if(this.carrot.is_online())
                        this.load_list_by_query(this.carrot.db.Collection("app"));
                    else
                        load_list_by_s_data(this.s_data_carrotapp_all);
                }  
                else
                    load_list_by_s_data(this.carrot.s_data_json_carrotapp_all_temp);
            }

            if (this.type == carrot_app_type.app)
            {
                if (this.carrot.s_data_json_carrotapp_app_temp == "")
                {
                    if (this.carrot.is_online())
                        this.load_list_by_query(this.carrot.db.Collection("app").WhereEqualTo("type", "app"));
                    else
                        load_list_by_s_data(this.s_data_carrotapp_app);
                } 
                else
                    load_list_by_s_data(this.carrot.s_data_json_carrotapp_app_temp);
            }

            if (this.type == carrot_app_type.game)
            {
                if (this.carrot.s_data_json_carrotapp_game_temp == "")
                {
                    if (this.carrot.is_online())
                        this.load_list_by_query(this.carrot.db.Collection("app").WhereEqualTo("type", "game"));
                    else
                        load_list_by_s_data(this.s_data_carrotapp_game);
                } 
                else
                    load_list_by_s_data(this.carrot.s_data_json_carrotapp_game_temp);
            }
        }

        private void load_list_by_query(Query AppQuery)
        {
            this.carrot.log("load_list_by_query (" + this.type + ")");
            AppQuery.GetSnapshotAsync().ContinueWithOnMainThread(task => {
                QuerySnapshot AppQuerySnapshot = task.Result;

                if (task.IsFaulted)
                {
                    if (this.type == carrot_app_type.all) this.load_list_by_s_data(this.s_data_carrotapp_all);
                    if (this.type == carrot_app_type.app) this.load_list_by_s_data(this.s_data_carrotapp_app);
                    if (this.type == carrot_app_type.game) this.load_list_by_s_data(this.s_data_carrotapp_game);
                }

                if (task.IsCompleted)
                {
                    if (AppQuerySnapshot.Count > 0)
                    {
                        create_list_box_app();
                        List<IDictionary> list_app = new List<IDictionary>();
                        foreach (DocumentSnapshot document in AppQuerySnapshot.Documents)
                        {
                            IDictionary app_data = document.ToDictionary();
                            app_data["id"] = document.Id;
                            add_item_to_list_box(app_data);
                            list_app.Add(app_data);
                        };

                        if (this.type == carrot_app_type.all)
                        {
                            this.s_data_carrotapp_all = Json.Serialize(list_app);
                            this.carrot.s_data_json_carrotapp_all_temp = this.s_data_carrotapp_all;
                            PlayerPrefs.SetString("s_data_carrotapp_all", this.s_data_carrotapp_all);
                        }

                        if (this.type == carrot_app_type.app)
                        {
                            this.s_data_carrotapp_app = Json.Serialize(list_app);
                            this.carrot.s_data_json_carrotapp_app_temp = this.s_data_carrotapp_app;
                            PlayerPrefs.SetString("s_data_carrotapp_app", this.s_data_carrotapp_app);
                        }

                        if (this.type == carrot_app_type.game)
                        {
                            this.s_data_carrotapp_game = Json.Serialize(list_app);
                            this.carrot.s_data_json_carrotapp_game_temp = this.s_data_carrotapp_game;
                            PlayerPrefs.SetString("s_data_carrotapp_game", this.s_data_carrotapp_game);
                        }

                        if (this.carrot.type_control != TypeControl.None)
                        {
                            this.carrot.game.set_list_button_gamepad_console(this.list_btn_gamepad);
                            this.box_list_app.update_gamepad_cosonle_control();
                            this.box_list_app.update_color_table_row();
                            this.carrot.game.set_index_button_gamepad_console(3);
                            this.carrot.game.set_scrollRect_gamepad_consoles(this.box_list_app.UI.scrollRect);
                        }
                    }
                }
            });
        }

        private void load_list_by_s_data(string s_data)
        {
            this.carrot.log("load_list_by_s_data (" + this.type + ")");
            if (s_data == "") return;
            create_list_box_app();
            IList list_app = (IList)Json.Deserialize(s_data);
            for (int i = 0; i < list_app.Count; i++)
            {
                IDictionary app_data = (IDictionary)list_app[i];
                add_item_to_list_box(app_data);
            };

            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(this.list_btn_gamepad);
                this.box_list_app.update_gamepad_cosonle_control();
                this.box_list_app.update_color_table_row();
                this.carrot.game.set_index_button_gamepad_console(3);
                this.carrot.game.set_scrollRect_gamepad_consoles(this.box_list_app.UI.scrollRect);
            }
        }

        private void create_list_box_app()
        {
            this.carrot.hide_loading();
            if (this.box_list_app != null) this.box_list_app.close();
            this.box_list_app = this.carrot.Create_Box();
            box_list_app.set_icon(this.carrot.icon_carrot);
            box_list_app.set_title(PlayerPrefs.GetString("list_app_carrot", "Applications from the developer"));

            this.btn_header_all = box_list_app.create_btn_menu_header(this.carrot.icon_carrot_all_category);
            this.btn_header_all.set_act(()=>this.act_btn_header_box(carrot_app_type.all));
            if (this.type == carrot_app_type.all) this.btn_header_all.set_icon_color(this.carrot.color_highlight);

            this.btn_header_app = box_list_app.create_btn_menu_header(this.carrot.icon_carrot_app);
            this.btn_header_app.set_act(()=>this.act_btn_header_box(carrot_app_type.app));
            if (this.type == carrot_app_type.app) this.btn_header_app.set_icon_color(this.carrot.color_highlight);

            this.btn_header_game = box_list_app.create_btn_menu_header(this.carrot.icon_carrot_game);
            this.btn_header_game.set_act(()=>this.act_btn_header_box(carrot_app_type.game));
            if (this.type == carrot_app_type.game) this.btn_header_game.set_icon_color(this.carrot.color_highlight);

            if (this.carrot.model_app == ModelApp.Develope)
            {
                Carrot_Box_Btn_Item btn_add_app = this.box_list_app.create_btn_menu_header(this.carrot.icon_carrot_add);
                btn_add_app.set_act(show_add_app);
            }
        }

        private void add_item_to_list_box(IDictionary data_item)
        {
            string s_key_lang = this.carrot.lang.get_key_lang();
            string s_key_store_public = this.carrot.store_public.ToString().ToLower();
            string s_id_app = data_item["id"].ToString();
            Carrot_Box_Item item_app = box_list_app.create_item(s_id_app);

            string s_name = "";
            if (data_item["name_" + s_key_lang] != null) s_name = data_item["name_" + s_key_lang].ToString();
            if (s_name == "")
            {
                if (data_item["name_en"] != null) s_name = data_item["name_en"].ToString();
            }
            item_app.set_title(s_name);
            if (data_item["type"] != null) item_app.set_tip(data_item["type"].ToString());

            var s_link = "";
            if (data_item[s_key_store_public] != null) s_link = data_item[s_key_store_public].ToString();
            if (s_link == "")
            {
                if (data_item["google_play"] != null) s_link = data_item["google_play"].ToString();
            }
            var s_link_carrot = s_link;
            Carrot_Box_Btn_Item app_btn_download = item_app.create_item();
            app_btn_download.set_icon(this.carrot.icon_carrot_download);
            app_btn_download.set_color(this.carrot.color_highlight);
            app_btn_download.set_act(() => this.open_link(s_link));

            Carrot_Box_Btn_Item app_btn_share = item_app.create_item();
            app_btn_share.set_icon(this.carrot.sp_icon_share);
            app_btn_share.set_color(this.carrot.color_highlight);
            app_btn_share.set_act(() => this.open_link_share(s_link_carrot));

            if (data_item["icon"] != null)
            {
                Sprite icon_app = this.carrot.get_tool().get_sprite_to_playerPrefs(s_id_app);
                if (icon_app != null)
                {
                    item_app.set_icon_white(icon_app);
                }
                else
                {
                    string s_url_icon = data_item["icon"].ToString();
                    if (s_url_icon != "") this.carrot.get_img_and_save_playerPrefs(s_url_icon, item_app.img_icon, s_id_app);
                }
            }
            item_app.set_act(() => this.open_link(s_link));
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

            if (this.type == carrot_app_type.all)
            {
                if (this.carrot.s_data_json_carrotapp_all_temp == "")
                    this.act_load_app_where_exit_by_query(this.carrot.db.Collection("app"));
                else
                    this.act_load_app_where_exit_by_s_data(this.carrot.s_data_json_carrotapp_all_temp);
            }

            if (this.type == carrot_app_type.app)
            {
                if (this.carrot.s_data_json_carrotapp_app_temp == "")
                    this.act_load_app_where_exit_by_query(this.carrot.db.Collection("app").WhereEqualTo("type", "app"));

                else
                    this.act_load_app_where_exit_by_s_data(this.carrot.s_data_json_carrotapp_app_temp);
            }

            if (this.type == carrot_app_type.game)
            {
                if (this.carrot.s_data_json_carrotapp_game_temp == "")

                    this.act_load_app_where_exit_by_query(this.carrot.db.Collection("app").WhereEqualTo("type", "game"));
                else
                    this.act_load_app_where_exit_by_s_data(this.carrot.s_data_json_carrotapp_game_temp);
            }

            this.window_exit.UI.set_theme(this.carrot.color_highlight);
        }

        private void act_load_app_where_exit_by_query(Query AppQueryS)
        {
            this.carrot.log("act_load_app_where_exit_by_query ("+this.type+")");
            AppQueryS.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot AppQuerySnapshotExit = task.Result;
                if (AppQuerySnapshotExit.Count > 0)
                {
                    int count_app_exit = 0;
                    this.window_exit.panel_list_app_other.SetActive(true);

                    List<IDictionary> list_app = new List<IDictionary>();
                    foreach (DocumentSnapshot document in AppQuerySnapshotExit.Documents)
                    {
                        IDictionary app_data = document.ToDictionary();
                        app_data["id"] = document.Id;
                        list_app.Add(app_data);
                        if (count_app_exit < 10) add_item_app_exit(app_data);
                        count_app_exit++;
                    };

                    if (this.type == carrot_app_type.all)
                    {
                        this.s_data_carrotapp_all = Json.Serialize(list_app);
                        this.carrot.s_data_json_carrotapp_all_temp = this.s_data_carrotapp_all;
                        PlayerPrefs.SetString("s_data_carrotapp_all", this.s_data_carrotapp_all);
                    }

                    if (this.type == carrot_app_type.app)
                    {
                        this.s_data_carrotapp_app = Json.Serialize(list_app);
                        this.carrot.s_data_json_carrotapp_app_temp = this.s_data_carrotapp_app;
                        PlayerPrefs.SetString("s_data_carrotapp_app", this.s_data_carrotapp_app);
                    }

                    if (this.type == carrot_app_type.game)
                    {
                        this.s_data_carrotapp_game = Json.Serialize(list_app);
                        this.carrot.s_data_json_carrotapp_game_temp = this.s_data_carrotapp_game;
                        PlayerPrefs.SetString("s_data_carrotapp_game", this.s_data_carrotapp_game);
                    }

                    this.list_btn_gamepad.Add(this.window_exit.UI.obj_gamepad[0]);
                    this.list_btn_gamepad.Add(this.window_exit.UI.obj_gamepad[1]);
                }
            });
        }

        private void act_load_app_where_exit_by_s_data(string s_data)
        {
            this.carrot.log("act_load_app_where_exit_by_s_data (" + this.type + ")");
            IList list_app_exit = (IList)Json.Deserialize(s_data);

            int count_app_exit = 0;
            this.window_exit.panel_list_app_other.SetActive(true);

            for (int i = 0; i < list_app_exit.Count; i++)
            {
                IDictionary app_data = (IDictionary)list_app_exit[i];
                add_item_app_exit(app_data);
                count_app_exit++;
                if (count_app_exit >= 10) break;
            };
            this.list_btn_gamepad.Add(this.window_exit.UI.obj_gamepad[0]);
            this.list_btn_gamepad.Add(this.window_exit.UI.obj_gamepad[1]);
        }

        private void add_item_app_exit(IDictionary data_app_exit)
        {
            string s_id_app = data_app_exit["id"].ToString();
            if (data_app_exit["icon"] != null)
            {
                var s_store = this.carrot.store_public.ToString().ToLower();
                var s_link = "";
                if (data_app_exit[s_store] != null) s_link = data_app_exit[s_store].ToString();
                var s_link_carrot = s_link;
                Carrot_Button_Item item_app_exit = this.window_exit.create_item();
                Sprite icon_app = this.carrot.get_tool().get_sprite_to_playerPrefs(s_id_app);
                if (icon_app != null)
                {
                    item_app_exit.set_icon(icon_app);
                }
                else
                {
                    string s_url_icon = data_app_exit["icon"].ToString();
                    if (s_url_icon != "") this.carrot.get_img_and_save_playerPrefs(s_url_icon, item_app_exit.img_icon, s_id_app);
                }
                item_app_exit.set_act_click(() => this.open_link(s_link));
            }
        }

        private void open_link(string s_link)
        {
            Application.OpenURL(s_link);
        }

        public void open_link_share(string s_link)
        {
            this.carrot.show_share(s_link, PlayerPrefs.GetString("share_tip", "Choose the platform below to share this great app with your friends or others"));
        }

        private void act_btn_header_box(carrot_app_type type_show)
        {
            this.type = type_show;
            show_list_carrot_app();
        }

        public void show_add_app()
        {
            Carrot_Box box_add_app=this.carrot.Create_Box();
            box_add_app.set_icon(this.carrot.icon_carrot_add);
        }
    }
}