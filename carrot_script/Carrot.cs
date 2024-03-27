#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Carrot
{
    public enum ModelApp { Publish, Develope }
    public enum OS { Android, Window, Ios, Web };
    public enum Store { Google_Play, Samsung_Galaxy_Store, Microsoft_Store, Amazon_app_store, Carrot_store, Huawei_store, Itch,Uptodown};
    public enum TypeApp { App, Game }
    public enum TypeRate { Market_Android, Ms_Windows_Store, Amazon_app_store, Link_Share_CarrotApp }
    public enum TypeAds {Admod_Unity_Carrot,Unity_Admob_Carrot, Carrot}
    public enum TypeControl { None, GamePad, D_pad }
    public enum Setting_Option { Show, Hide }

    public enum PayApp { UnitySDKPay, CarrotPay }

    public class On_event_change : UnityEvent { };
    public class Carrot : MonoBehaviour
    {
        [Header("Config Server")]
        public string mainhost = "https://carrotstore.web.app";
        public string key_api_rest_firestore = "";
        public string key_api_google_location_map= "";

        [Header("Config App")]
        public ModelApp model_app;
        public OS os_app;
        public Store store_public;
        public TypeApp type_app;
        public PayApp pay_app;
        public TypeRate type_rate;
        public TypeAds type_ads;
        public TypeControl type_control;
        public string collection_document_lang;
        public string WindowUWP_ProductId;
        public string Carrotstore_AppId;
        public Color32 color_highlight;
        public bool check_lost_internet;
        public bool auto_open_rate_store = true;

        private bool is_next_check_exit = true;
        private bool is_sound = true;
        private bool is_vibrate = true;
        private Carrot_list_app carrot_list_app;
        private Carrot_tool tool;
        private List<GameObject> list_Window;
        private Carrot_Window_Msg msg;
        private Carrot_Window_Loading loading;
        private List<string> list_log;
        private int count_change_model_app = 0;

        [Header("Login Obj")]
        public Image img_btn_login;

        [Header("Lang set Emp app")]
        public Carrot_lang_show emp_show_lang;

        [Header("Setting Config")]
        [Tooltip("Change the value to enable the ad buying function")]
        public int index_inapp_remove_ads = -1;
        [Tooltip("Change the value to enable the background music buying function")]
        public int index_inapp_buy_bk_music = -1;
        public Setting_Option setting_lang = Setting_Option.Hide;
        public Setting_Option setting_login = Setting_Option.Hide;
        public Setting_Option setting_rank = Setting_Option.Hide;
        public Setting_Option setting_vibrate = Setting_Option.Hide;
        public Setting_Option setting_soundtrack = Setting_Option.Hide;
        public Setting_Option setting_theme = Setting_Option.Hide;

        [Header("Ads Config")]
        public int click_show_ads = 5;
        public GameObject[] emp_btn_remove_ads;

        [Header("Admob Ads Android config")]
        public string id_ads_Interstitial_android;
        public string id_ads_Banner_android;
        public string id_ads_Rewarded_android;

        [Header("Admob Ads Ios config")]
        public string id_ads_Interstitial_ios = "unused";
        public string id_ads_Banner_ios = "unused";
        public string id_ads_Rewarded_ios = "unused";

        [Header("Unity Ads Android config")]
        public bool ads_uniy_test_mode;
        public string id_ads_unity_App_android;
        public string id_ads_unity_Interstitial_android;
        public string id_ads_unity_Banner_android;
        public string id_ads_unity_Rewarded_android;

        [Header("Carrot Obj")]
        public Carrot_lang lang;
        public Carrot_User user;
        public Carrot_shop shop;
        public Carrot_ads ads;
        public Carrot_game game;
        public Carrot_camera camera_pro;
        public Carrot_location location;
        public Carrot_Theme theme;
        public Carrot_Server server;

        [Header("Panel Obj")]
        public GameObject window_msg_prefab;
        public GameObject window_loading_prefab;
        public GameObject window_exit_prefab;
        public GameObject window_share_prefab;
        public GameObject window_rate_prefab;
        public GameObject window_box_prefab;
        public GameObject window_input_prefab;
        public GameObject window_photoshop_prefab;
        public GameObject carrot_btn_prefab;

        [Header("Icon sprite")]
        public Sprite icon_carrot;
        public Sprite icon_carrot_all_category;
        public Sprite icon_carrot_app;
        public Sprite icon_carrot_game;
        public Sprite icon_carrot_search;
        public Sprite icon_carrot_write;
        public Sprite icon_carrot_gamepad;
        public Sprite icon_carrot_bug;
        public Sprite icon_carrot_d_pad;
        public Sprite icon_carrot_ads;
        public Sprite icon_carrot_link;
        public Sprite icon_carrot_download;
        public Sprite sp_icon_setting;
        public Sprite sp_icon_dev;
        public Sprite icon_carrot_gamepad_off;
        public Sprite icon_carrot_gamepad_on;
        public Sprite icon_carrot_done;
        public Sprite icon_carrot_cancel;
        public Sprite icon_carrot_visible_off;
        public Sprite icon_carrot_visible_on;
        public Sprite icon_carrot_nomal;
        public Sprite icon_carrot_advanced;
        public Sprite icon_carrot_add;
        public Sprite icon_carrot_mail;
        public Sprite icon_carrot_phone;
        public Sprite icon_carrot_address;
        public Sprite icon_carrot_avatar;
        public Sprite icon_carrot_sex;
        public Sprite icon_carrot_sex_boy;
        public Sprite icon_carrot_sex_girl;
        public Sprite icon_carrot_location;
        public Sprite icon_carrot_database;
        public Sprite icon_carrot_buy;
        public Sprite icon_carrot_price;
        public Sprite icon_carrot_support;
        public Sprite sp_icon_sound_on;
        public Sprite sp_icon_sound_off;
        public Sprite sp_icon_rate;
        public Sprite sp_icon_share;
        public Sprite sp_icon_more_app;
        public Sprite sp_icon_removeads;
        public Sprite sp_icon_restore;
        public Sprite sp_icon_del_data;
        public Sprite sp_icon_bk_music;
        public Sprite sp_icon_vibrate_on;
        public Sprite sp_icon_vibrate_off;
        public Sprite sp_icon_theme_color;
        public Sprite sp_icon_picker_color;
        public Sprite sp_icon_mixer_color;
        public Sprite sp_icon_table_color;
        public Sprite icon_carrot_donation;
        public Sprite icon_carrot_facebook;

        [Header("Lost Internet")]
        public Carrot_lost_internet carrot_lost_internet;
        private bool is_online_internet = true;
        public Sprite icon_lost_internet;

        [Header("Setting Carrot")]
        public AudioSource sound_click;
        Carrot_Box box_setting = null;
        Carrot_Box_Item item_setting_lang = null;
        Carrot_Box_Item item_setting_ads = null;

        [Header("Event Customer")]
        public UnityAction act_check_exit_app;
        public UnityAction act_after_close_all_box;
        public UnityAction act_after_delete_all_data;
        private UnityAction act_result_msg_config;
        private bool is_ready = false;
        
        public void Load_Carrot()
        {
            this.list_log = new List<string>();

            this.game.Load_carrot_game();

            this.tool = new Carrot_tool();
            this.lang.On_load(this);
            this.user.On_load(this);
            this.camera_pro.On_load();
            this.shop.On_load(this);
            this.shop.onCarrotPaySuccess += this.carrot_by_success;
            this.shop.onCarrotRestoreSuccess += this.carrot_restore_success;
            this.ads.On_load(this);
            this.theme.On_load(this);

            if (this.type_app == TypeApp.Game) this.ads.onRewardedSuccess += this.game.OnRewardedSuccess;
            this.ads.onRewardedSuccess += this.theme.OnRewardedSuccess;

            this.carrot_list_app = new Carrot_list_app(this);
            this.list_Window = new List<GameObject>();

            if (PlayerPrefs.GetInt("is_sound", 0) == 0)
                this.is_sound = true;
            else
                this.is_sound = false;

            if (PlayerPrefs.GetInt("is_vibrate", 0) == 0)
                this.is_vibrate = true;
            else
                this.is_vibrate = false;
            if (this.check_lost_internet) this.check_connect_internet();
            this.is_ready = true;
        }

        public void Load_Carrot(UnityAction act_check_exit_app)
        {
            Load_Carrot();
            this.act_check_exit_app = act_check_exit_app;
        }

        public void show_list_carrot_app() { this.carrot_list_app.show_list_carrot_app(); }
        public void clear_contain(Transform area_body) { foreach (Transform child in area_body) { Destroy(child.gameObject); } }
        public void show_login() { this.user.show_login(); }
        public void show_user_register() { this.user.show_user_register(); }
        public string L(string s_key, string s_default = ""){ return lang.Val(s_key,s_default); }
        public Carrot_tool get_tool() { return this.tool; }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) this.act_Escape();
        }

        public void act_Escape()
        {
            this.play_sound_click();
            this.is_next_check_exit = true;
            if (this.act_check_exit_app != null) this.act_check_exit_app.Invoke();
            if (this.is_next_check_exit)
            {
                if (this.list_Window.Count > 0)
                {
                    int index_box_check = this.list_Window.Count - 1;

                    if (this.list_Window[index_box_check] != null)
                    {
                        if (this.list_Window[index_box_check].name == "window_ads") return;

                        if (this.list_Window[index_box_check].name == "window_exit")
                        {
                            this.app_exit();
                            return;
                        }

                        if (this.list_Window[index_box_check] != null)
                        {
                            this.close_window(index_box_check);
                            return;
                        }
                    }
                }

                this.carrot_list_app.show_list_app_where_exit();
            }
        }

        #region Rate
        public void show_rate()
        {
            this.play_sound_click();
            GameObject window_rate = this.create_window(this.window_rate_prefab);
            window_rate.name = "Window Rate";
            Carrot_Window_Rate rate = window_rate.GetComponent<Carrot_Window_Rate>();
            rate.load(this);
        }
        #endregion

        #region Share
        public void show_share()
        {
            this.play_sound_click();
            GameObject window_share = this.create_window(this.window_share_prefab);
            window_share.GetComponent<Carrot_Window_Share>().load(this);
            window_share.GetComponent<Carrot_Window_Share>().inp_link_share.text = this.mainhost + "?p=app&id=" + this.Carrotstore_AppId;
        }

        public void show_share(string link_customer, string s_share_tip)
        {
            GameObject window_share = this.create_window(this.window_share_prefab);
            window_share.GetComponent<Carrot_Window_Share>().load(this);
            window_share.GetComponent<Carrot_Window_Share>().txt_share_tip.text = s_share_tip;
            window_share.GetComponent<Carrot_Window_Share>().inp_link_share.text = link_customer;
        }
        #endregion

        public void close()
        {
            this.close_all_window();
            if (this.act_after_close_all_box != null) this.act_after_close_all_box();
        }

        private Carrot_Window_Msg create_msg()
        {
            GameObject window_msg = this.create_window(this.window_msg_prefab);
            window_msg.name = "Window Msg";
            window_msg.GetComponent<Carrot_Window_Msg>().load(this);
            return window_msg.GetComponent<Carrot_Window_Msg>();
        }

        public Carrot_Window_Msg show_msg(string s_msg)
        {
            Carrot_Window_Msg msg = this.create_msg();
            msg.set_msg(s_msg);
            msg.update_btns_gamepad_console();
            return msg;
        }

        public Carrot_Window_Msg show_msg(string s_title, string s_msg)
        {
            Carrot_Window_Msg msg = this.show_msg(s_msg);
            msg.set_title(s_title);
            return msg;
        }

        public Carrot_Window_Msg Show_msg(string s_title, string s_msg, Msg_Icon icon)
        {
            Carrot_Window_Msg msg = show_msg(s_title, s_msg);
            msg.set_icon(icon);
            return msg;
        }

        public Carrot_Window_Msg Show_msg(string s_title, string s_msg, UnityAction act_msg_yes)
        {
            this.act_result_msg_config = act_msg_yes;
            this.msg = show_msg(s_title, s_msg);
            msg.set_icon(Msg_Icon.Question);
            msg.add_btn_msg(lang.Val("msg_yes", "Yes"), Act_msg_config_yes);
            msg.add_btn_msg(lang.Val("cancel", "No"), Act_msg_config_no);
            msg.update_btns_gamepad_console();
            return msg;
        }

        private void Act_msg_config_yes()
        {
            play_sound_click();
            this.act_result_msg_config?.Invoke();
            if (this.msg != null) this.msg.close();
        }

        private void Act_msg_config_no()
        {
            play_sound_click();
            if (this.msg != null) this.msg.close();
        }

        public Carrot_Window_Loading show_loading()
        {
            GameObject window_loading = this.create_window(this.window_loading_prefab);
            window_loading.name = "window_loading";
            this.loading = window_loading.GetComponent<Carrot_Window_Loading>();
            this.loading.slider_loading.gameObject.SetActive(false);
            this.loading.UI.set_theme(this.get_color_highlight_blur(100));
            if (this.type_control != TypeControl.None) this.game.set_list_button_gamepad_console(this.loading.UI.get_list_btn());
            return this.loading;
        }

        public Carrot_Window_Loading show_loading(IEnumerator session)
        {
            Carrot_Window_Loading wl = this.show_loading();
            this.loading = wl;
            wl.set_session(session);
            return wl;
        }

        public Carrot_Window_Loading show_loading_with_process_bar(IEnumerator session)
        {
            this.loading = this.show_loading(session);
            this.loading.show_process();
            return this.loading;
        }

        public Carrot_Window_Loading get_loading_cur()
        {
            return this.loading;
        }

        public void hide_loading()
        {
            if (this.loading != null) this.loading.close();
        }

        public Carrot_Box show_grid()
        {
            GameObject box_grid = this.create_window(this.window_box_prefab);
            Carrot_Box grid = box_grid.GetComponent<Carrot_Box>();
            grid.load(this);
            grid.set_type(Carrot_Box_Type.Grid_Box);
            return grid;
        }

        public Carrot_Box show_grid(Sprite icon)
        {
            Carrot_Box box_grid = this.show_grid();
            box_grid.set_icon(icon);
            return box_grid;
        }

        public void app_exit()
        {
            Debug.Log("Application Exit");
            Application.Quit();
        }

        public void get_img(string url, Image img)
        {
            if (url == "") return;
            StartCoroutine(this.tool.get_img_form_url(url, img));
        }

        public void get_img(string url, UnityAction<Texture2D> act_done, UnityAction<string> act_fail = null)
        {
            if (url == "") return;
            StartCoroutine(this.tool.get_img_form_url(url, null, act_done, act_fail));
        }

        public void get_img_and_save_file(string url, Image img, string path_save) { StartCoroutine(this.tool.get_img_form_url_and_save_file(url, img, path_save)); }

        public void get_img_and_save_playerPrefs(string url, Image img, string s_key, UnityAction<Texture2D> act_done = null, UnityAction<string> act_fail = null)
        {
            if (url == "") return;
            StartCoroutine(this.tool.get_img_form_url_and_save_playerPrefs(url, img, s_key, act_done, act_fail));
        }

        public void get_img(string url, UnityAction<Texture2D> act_download_img)
        {
            StartCoroutine(this.tool.download_img_form_url(url, act_download_img));
        }

        public Carrot_Window_Input show_search(UnityAction<string> act_done_search, string s_search_tip = "Enter what you want to search for")
        {
            GameObject obj_search = this.create_window(this.window_input_prefab);
            Carrot_Window_Input box_search = obj_search.GetComponent<Carrot_Window_Input>();
            box_search.load(this);
            box_search.set_icon(this.icon_carrot_search);
            box_search.set_title(lang.Val("search", "Search"));
            box_search.set_tip(s_search_tip);
            box_search.set_act_done(act_done_search);
            return box_search;
        }

        public Carrot_Window_Input show_input(string s_title, string s_tip = "", string s_txt = "", Window_Input_value_Type type_val = Window_Input_value_Type.input_field)
        {
            GameObject obj_box_inp = this.create_window(this.window_input_prefab);
            Carrot_Window_Input box_inp = obj_box_inp.GetComponent<Carrot_Window_Input>();
            box_inp.load(this);
            box_inp.set_icon(this.icon_carrot_write);
            box_inp.set_title(s_title);
            box_inp.set_tip(s_tip);
            box_inp.set_inp_type(type_val);
            box_inp.set_val(s_txt);
            return box_inp;
        }

        [ContextMenu("Delete All Data")]
        public void Delete_all_data()
        {
            this.play_sound_click();
            this.msg = this.Show_msg(L("delete_all_data", "Clear all application data"), L("delete_all_data_tip", "Confirm erase all data and set up") + "?", () =>
            {
                if (this.msg != null) this.msg.close();
                this.user.delete_data_user_login();
                PlayerPrefs.DeleteAll();
                if (this.type_control != TypeControl.None) game.Destroy_all_gamepad();
                this.get_tool().delete_file("music_bk");
                if (model_app == ModelApp.Develope) Debug.Log("Delete All Data Success!!!");
                this.msg = this.Show_msg(L("delete_all_data", "Clear all application data"), L("delete_all_data_success", "Erase all settings settings and app data successfully!"), Msg_Icon.Success);
                this.delay_function(2f, ()=>this.Restart_app());
            });
        }

        private void Restart_app()
        {
            if(this.msg!=null) this.msg.close();
            close_all_window();
            if(this.act_after_delete_all_data!=null)
                this.act_after_delete_all_data.Invoke();
            else
                this.Load_Carrot(this.act_check_exit_app);
        }

        public Carrot_Window_Loading send(string url, WWWForm frm, UnityAction<string> done_func = null, UnityAction<string> fail_func = null)
        {
            if (model_app == ModelApp.Develope) this.log("Send Request.." + frm.ToString() + " url:" + url);
            return this.show_loading(act_send(url, frm, done_func, fail_func));
        }

        IEnumerator act_send(string url, WWWForm frm_send, UnityAction<string> done_func, UnityAction<string> fail_func)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, frm_send))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.Show_msg("Error", www.error, Msg_Icon.Error);
                    if (fail_func != null) fail_func(www.error);
                }
                else
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) Debug.Log("Server response:" + www.downloadHandler.text);
                    if (done_func != null) done_func(www.downloadHandler.text);
                }
            }
        }

        public void send_hide(string url, WWWForm frm, UnityAction<string> done_func = null, UnityAction<string> error_func = null)
        {
            StartCoroutine(act_send_hide(url, frm, done_func, error_func));
        }

        IEnumerator act_send_hide(string url, WWWForm frm_send, UnityAction<string> done_func = null, UnityAction<string> error_func = null)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, frm_send))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    if (model_app == ModelApp.Develope) Debug.Log("Send hide server response:" + www.downloadHandler.text);
                    if (done_func != null) done_func(www.downloadHandler.text);
                }
                else
                {
                    if (error_func != null) error_func(www.error);
                }
            }
        }

        public void show_list_lang()
        {
            this.lang.Show_list_lang();
        }

        public void show_list_lang(UnityAction<string> call_func)
        {
            this.lang.Show_list_lang(call_func);
        }

        public void buy_product(int index)
        {
            this.shop.buy_product(index);
        }

        public void restore_product()
        {
            this.shop.restore_product();
        }

        public void set_color(Color32 color_change)
        {
            this.color_highlight = color_change;
        }

        public void set_no_check_exit_app()
        {
            this.is_next_check_exit = false;
        }

        public Carrot_Window_Loading get_mp3(string url, UnityAction<UnityWebRequest> act_success, UnityAction act_fail = null)
        {
            this.loading = this.show_loading_with_process_bar(get_mp3_form_url(url, act_success, act_fail));
            return this.loading;
        }

        IEnumerator get_mp3_form_url(string s_url_audio, UnityAction<UnityWebRequest> act_success, UnityAction act_fail)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(s_url_audio, AudioType.MPEG))
            {
                www.SendWebRequest();
                while (!www.isDone)
                {
                    if (this.loading != null) this.loading.slider_loading.value = www.downloadProgress;
                    yield return null;
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (act_fail != null) act_fail();
                }
                else
                {
                    this.hide_loading();
                    act_success(www);
                }
            }
        }

        public void stop_all_act()
        {
            if (this.model_app == ModelApp.Develope) Debug.Log("Carrot StopAllCoroutines");
            this.StopAllCoroutines();
        }

        public void check_buy_music_item_bk(string id_product_success)
        {
            this.GetComponent<Carrot_game>().check_buy_music_item_bk(id_product_success);
        }

        public bool is_offline()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                this.is_online_internet = false;
                return true;
            }
            else
            {
                this.is_online_internet = true;
                return false;
            }
        }

        public bool is_online()
        {
            return this.is_online_internet;
        }

        public void set_status_online(bool is_online)
        {
            this.is_online_internet = is_online;
        }

        public void check_connect_internet()
        {
            bool status_internet_offline = is_offline();
            if (status_internet_offline)
            {
                if (this.carrot_lost_internet != null) this.carrot_lost_internet.carrot = this;
                this.msg = this.create_msg();
                msg.set_title(lang.Val("lost_connect", "Lost Internet connection"));
                msg.set_msg(lang.Val("lost_connect_msg", "Please check your network connection, currently the app cannot access the Internet"));
                msg.set_icon_customer(this.icon_lost_internet);
                msg.add_btn_msg(lang.Val("lost_connect_try", "Try checking again!"), act_try_connect_internet);
                this.carrot_lost_internet.try_connect();
            }
            if (this.carrot_lost_internet != null) this.carrot_lost_internet.set_model_by_status_internet(status_internet_offline);
        }

        private void act_try_connect_internet()
        {
            play_sound_click();
            this.msg.close();
        }

        public void delay_function(float timer, UnityAction act_func)
        {
            if (this.is_ready) if (this.model_app == ModelApp.Develope) this.log("Delay function " + timer + "s");
            StartCoroutine(act_delay_function(timer, act_func));
        }

        private IEnumerator act_delay_function(float timer, UnityAction act_func)
        {
            yield return new WaitForSeconds(timer);
            act_func();
        }

        public GameObject create_window(GameObject obj_prefab)
        {
            GameObject obj_window = Instantiate(obj_prefab);
            obj_window.name = "Carrot Window";
            obj_window.transform.SetParent(GameObject.Find("Canvas").transform);
            obj_window.transform.localPosition = new Vector3(obj_window.transform.localPosition.x, obj_window.transform.localPosition.y, 0f);
            obj_window.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_window.transform.localRotation = Quaternion.identity;
            RectTransform rectTransform = obj_window.GetComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.offsetMin = new Vector2(0f, 0f);
            rectTransform.offsetMax = new Vector2(0f, 0f);
            this.add_window(obj_window);
            return obj_window;
        }

        public Carrot_Box Create_Box()
        {
            GameObject box_window = this.create_window(this.window_box_prefab);
            Carrot_Box box = box_window.GetComponent<Carrot_Box>();
            box.load(this);
            return box.GetComponent<Carrot_Box>();
        }

        public Carrot_Box Create_Box(string s_title)
        {
            Carrot_Box box = this.Create_Box();
            box.set_title(s_title);
            return box;
        }

        public Carrot_Box Create_Box(string s_title, Sprite icon)
        {
            Carrot_Box box = this.Create_Box(s_title);
            box.set_icon(icon);
            return box;
        }

        public Carrot_Box Create_Setting(GameObject[] array_item_customer = null)
        {
            this.play_sound_click();
            this.box_setting = this.Create_Box(lang.Val("setting", "Setting"));
            if (this.model_app == ModelApp.Publish)
                this.box_setting.set_icon(this.sp_icon_setting);
            else
                this.box_setting.set_icon(this.sp_icon_dev);

            box_setting.name = "Box Setting";
            box_setting.set_title(lang.Val("setting", "Setting"));
            Button btn_icon_setting = this.box_setting.img_icon.gameObject.AddComponent<Button>();
            btn_icon_setting.onClick.AddListener(() => this.act_check_change_model_app());

            if (array_item_customer != null) for (int i = 0; i < array_item_customer.Length; i++) box_setting.add_item(array_item_customer[i]);

            if (this.setting_lang == Setting_Option.Show)
            {
                this.item_setting_lang = box_setting.create_item("lang");
                item_setting_lang.set_icon_white(this.lang.Get_sp_lang_cur());
                item_setting_lang.set_title(lang.Val("sel_lang_app", "Change application language"));
                item_setting_lang.set_tip(this.lang.Get_key_lang());
                item_setting_lang.set_lang_data("sel_lang_app", "sel_lang_app_tip");
                item_setting_lang.set_act(() => this.lang.Show_list_lang(change_lang_in_setting));
            }

            if (this.setting_login == Setting_Option.Show)
            {
                this.user.user_login_item_setting = box_setting.create_item("setting_login");
                this.user.check_and_show_item_login_setting();
            }

            if (this.setting_rank == Setting_Option.Show)
            {
                Carrot_Box_Item item_setting_top_player = box_setting.create_item("top_player");
                item_setting_top_player.set_icon(this.game.icon_top_player);
                item_setting_top_player.set_title(lang.Val("top_player", "Player rankings"));
                item_setting_top_player.set_tip(lang.Val("top_player_tip","User score leaderboard"));
                item_setting_top_player.set_act(this.game.Show_List_Top_player);
                item_setting_top_player.set_lang_data("top_player", "top_player_tip");
            }

            Carrot_Box_Item item_setting_sound = box_setting.create_item("sound");
            if (this.is_sound)
                item_setting_sound.set_icon(this.sp_icon_sound_on);
            else
                item_setting_sound.set_icon(this.sp_icon_sound_off);
            item_setting_sound.set_title(lang.Val("sound_app", "Sound"));
            item_setting_sound.set_tip(lang.Val("sound_app_tip", "On or Off Sound click"));
            item_setting_sound.set_lang_data("sound_app", "sound_app_tip");
            item_setting_sound.set_act(() => this.Change_status_sound(item_setting_sound));

            if (this.index_inapp_remove_ads != -1)
            {
                this.item_setting_ads = box_setting.create_item("remove_ads");
                this.item_setting_ads.set_icon(this.sp_icon_removeads);
                this.item_setting_ads.set_title(lang.Val("remove_ads", "Remove Ads"));
                this.item_setting_ads.set_tip(lang.Val("remove_ads_tip", "Buy and remove advertising function, No ads in the app"));
                this.item_setting_ads.set_lang_data("remove_ads", "remove_ads_tip");
                this.item_setting_ads.set_act(this.buy_inapp_removeads);

                if (this.ads.get_status_ads()) this.item_setting_ads.gameObject.SetActive(true);
                else this.item_setting_ads.gameObject.SetActive(false);
            }


            if (this.index_inapp_buy_bk_music != -1)
            {
                if (this.setting_soundtrack == Setting_Option.Show)
                {
                    Carrot_Box_Item item_setting_bk_music = box_setting.create_item("list_bk_music");
                    item_setting_bk_music.set_icon(this.sp_icon_bk_music);
                    item_setting_bk_music.set_title(lang.Val("list_bk_music", "Soundtrack"));
                    item_setting_bk_music.set_tip(lang.Val("list_bk_music_tip", "Select and change background music"));
                    item_setting_bk_music.set_lang_data("list_bk_music", "list_bk_music_tip");
                    item_setting_bk_music.set_act(() => this.game.show_list_music_game(item_setting_bk_music));

                    if (this.tool.check_file_exist("music_bk"))
                    {
                        Carrot_Box_Btn_Item btn_del_bk_music = item_setting_bk_music.create_item();
                        btn_del_bk_music.set_color(this.color_highlight);
                        btn_del_bk_music.set_icon(this.sp_icon_del_data);
                        btn_del_bk_music.GetComponent<Button>().onClick.RemoveAllListeners();
                        btn_del_bk_music.GetComponent<Button>().onClick.AddListener(() => this.game.delete_bk_music());
                        btn_del_bk_music.GetComponent<Button>().onClick.AddListener(() => this.Reload_setting());
                    }
                }
            }

            if (this.type_control != TypeControl.None)
            {
                List<Carrot_Gamepad> list_gamepad = this.game.get_list_gamepad();
                for (int i = list_gamepad.Count - 1; i >= 0; i--)
                {
                    var i_index = i;
                    Carrot_Box_Item item_setting_gamepad = box_setting.create_item("gamepad_control");
                    if (this.type_control == TypeControl.GamePad) item_setting_gamepad.set_icon(this.icon_carrot_gamepad);
                    if (this.type_control == TypeControl.D_pad) item_setting_gamepad.set_icon(this.icon_carrot_d_pad);
                    item_setting_gamepad.set_title("Customize Gamepad (" + list_gamepad[i].get_id_gamepad() + ")");
                    item_setting_gamepad.set_tip("Change the handle keys");
                    item_setting_gamepad.set_act(list_gamepad[i].show_setting_gamepad);
                    Carrot_Box_Btn_Item btn_enable_gamepad = item_setting_gamepad.create_item();
                    if (list_gamepad[i].get_status_use_gampead())
                        btn_enable_gamepad.icon.sprite = this.icon_carrot_gamepad_off;
                    else
                        btn_enable_gamepad.icon.sprite = this.icon_carrot_gamepad_on;
                    btn_enable_gamepad.GetComponent<Button>().onClick.AddListener(() => list_gamepad[i_index].change_status_use_gamepad(btn_enable_gamepad));
                    btn_enable_gamepad.GetComponent<Image>().color = this.color_highlight;
                }
            }

            if (this.setting_vibrate == Setting_Option.Show)
            {
                Carrot_Box_Item item_setting_vibrate = box_setting.create_item("vibrate");
                if (this.is_vibrate)
                    item_setting_vibrate.set_icon(this.sp_icon_vibrate_on);
                else
                    item_setting_vibrate.set_icon(this.sp_icon_vibrate_off);
                item_setting_vibrate.set_title(lang.Val("vibrate_app", "Vibrate"));
                item_setting_vibrate.set_tip(lang.Val("vibrate_app_tip", "Turn vibration on or off"));
                item_setting_vibrate.set_lang_data("vibrate_app", "vibrate_app_tip");
                item_setting_vibrate.set_act(() => this.change_status_vibrate(item_setting_vibrate));
            }

            if (this.setting_theme == Setting_Option.Show)
            {
                Carrot_Box_Item item_setting_theme = box_setting.create_item("theme");
                item_setting_theme.set_icon(this.sp_icon_theme_color);
                item_setting_theme.set_title("Choose a color theme");
                item_setting_theme.set_tip("Change the main color theme of the app");
                item_setting_theme.set_act(() => this.theme.show_list_theme());
            }

            Carrot_Box_Item item_setting_rate = box_setting.create_item("rate");
            item_setting_rate.set_icon(this.sp_icon_rate);
            item_setting_rate.set_title(lang.Val("rate", "Evaluate"));
            item_setting_rate.set_tip(lang.Val("rate_tip", "Please take a moment of your time to rate this app."));
            item_setting_rate.set_lang_data("rate", "rate_tip");
            item_setting_rate.set_act(this.show_rate);

            Carrot_Box_Item item_setting_share = box_setting.create_item("share");
            item_setting_share.set_icon(this.sp_icon_share);
            item_setting_share.set_title(lang.Val("share", "Share"));
            item_setting_share.set_tip(lang.Val("share_tip", "Choose the platform below to share this great app with your friends or others"));
            item_setting_share.set_lang_data("share", "share_tip");
            item_setting_share.set_act(this.show_share);

            Carrot_Box_Item item_setting_other_app = box_setting.create_item();
            item_setting_other_app.set_icon(this.sp_icon_more_app);
            item_setting_other_app.set_title(lang.Val("list_app_carrot", "Applications from the developer"));
            item_setting_other_app.set_tip(lang.Val("exit_app_other", "Perhaps you will love our other apps"));
            item_setting_other_app.set_lang_data("list_app_carrot", "exit_app_other");
            item_setting_other_app.set_act(this.show_list_carrot_app);

            Carrot_Box_Item item_setting_restore = box_setting.create_item("in_app_restore");
            item_setting_restore.set_icon(this.sp_icon_restore);
            item_setting_restore.set_title(lang.Val("in_app_restore", "Restore purchased items"));
            item_setting_restore.set_tip(lang.Val("in_app_restore_tip", "Restore purchased services and functions"));
            item_setting_restore.set_lang_data("in_app_restore", "in_app_restore_tip");
            item_setting_restore.set_act(this.shop.restore_product);

            if (this.model_app == ModelApp.Develope)
            {
                Carrot_Box_Item item_setting_bug = box_setting.create_item();
                item_setting_bug.set_icon(this.icon_carrot_bug);
                item_setting_bug.set_title("Programming tools");
                item_setting_bug.set_tip("Review the application activity log");
                item_setting_bug.set_lang_data("carrot_bug", "carrot_bug_tip");
                item_setting_bug.set_act(this.show_log);

                Carrot_Box_Btn_Item btn_dev_info = item_setting_bug.create_item();
                btn_dev_info.set_icon(user.icon_user_info);
                btn_dev_info.set_color(this.color_highlight);
                btn_dev_info.set_act(() => Show_box_dev_info());

                Carrot_Box_Btn_Item btn_dev_ads = item_setting_bug.create_item();
                btn_dev_ads.set_icon(icon_carrot_ads);
                btn_dev_ads.set_color(this.color_highlight);
                btn_dev_ads.set_act(() => ads.Show_box_dev_test());
            }

            Carrot_Box_Item item_setting_del_data = box_setting.create_item();
            item_setting_del_data.set_icon(this.sp_icon_del_data);
            item_setting_del_data.set_title(lang.Val("delete_all_data", "Clear all application data"));
            item_setting_del_data.set_tip(lang.Val("delete_all_data_tip", "Erase all data and reinstall the app"));
            item_setting_del_data.set_lang_data("delete_all_data", "delete_all_data_tip");
            item_setting_del_data.set_act(this.Delete_all_data);

            Carrot_Box_Btn_Item btn_support = box_setting.create_btn_menu_header(icon_carrot_support);
            btn_support.set_act(Show_Support);

            return box_setting;
        }

        public void Reload_setting()
        {
            if (this.user.get_cur_window_user_login() != null) this.user.get_cur_window_user_login().close();
            if (this.box_setting != null) this.box_setting.close();
            if (this.box_setting.get_act_before_closing() != null)
            {
                UnityAction act_close_setting = this.box_setting.get_act_before_closing();
                this.box_setting = this.Create_Setting();
                this.box_setting.set_act_before_closing(act_close_setting);
            }
            else if (this.box_setting.get_act_before_closing_change() != null)
            {
                UnityAction<List<string>> act_close_setting = this.box_setting.get_act_before_closing_change();
                this.box_setting = this.Create_Setting();
                this.box_setting.set_act_before_closing(act_close_setting);
            }
            else
            {
                this.box_setting = this.Create_Setting();
            }
        }

        private void Show_Support()
        {
            Carrot_Box box_support = Create_Box();
            box_support.set_icon(icon_carrot_support);
            box_support.set_title(L("support", "Support"));

            Carrot_Box_Item item_donnation = box_support.create_item();
            item_donnation.set_icon(icon_carrot_donation);
            item_donnation.set_title("Donate");
            item_donnation.set_tip(L("donate_tip","Please contribute to the cost of maintaining the server and developing tools to serve everyone"));
            item_donnation.set_act(() =>
            {
                play_sound_click();
                Application.OpenURL("https://www.paypal.com/paypalme/kurotsmile");
            });

            Carrot_Box_Item item_mail = box_support.create_item();
            item_mail.set_icon(this.icon_carrot_mail);
            item_mail.set_title("Email");
            item_mail.set_tip(L("email_dev", "Email contact the developer"));
            item_mail.set_act(() =>
            {
                play_sound_click();
                Application.OpenURL("mailto:tranthienthanh93@gmail.com");
            });

            Carrot_Box_Item item_whatapp = box_support.create_item();
            item_whatapp.set_icon(this.icon_carrot_app);
            item_whatapp.set_title("WhatsApp");
            item_whatapp.set_tip(L("whatsapp_dev", "Call or text to talk directly with the developer"));
            item_whatapp.set_act(() =>
            {
                play_sound_click();
                Application.OpenURL("https://api.whatsapp.com/send?phone=+8409786515778&text=hello");
            });

            Carrot_Box_Item item_fb = box_support.create_item();
            item_fb.set_icon(this.icon_carrot_facebook);
            item_fb.set_title("Facebook");
            item_fb.set_tip(L("facebook_dev", "Follow the developer's news page on social network Facebook"));
            item_fb.set_act(() =>
            {
                play_sound_click();
                Application.OpenURL("https://www.facebook.com/virtuallover/");
            });

            Carrot_Box_Item item_x = box_support.create_item();
            item_x.set_icon(this.icon_carrot_cancel);
            item_x.set_title("X");
            item_x.set_tip(L("x_dev","Follow the developer's news page on social network X"));
            item_x.set_act(() =>
            {
                play_sound_click();
                Application.OpenURL("https://twitter.com/carrotstore1");
            });
        }

        public void Change_status_sound(Carrot_Box_Item item_status_sound)
        {
            if (this.is_sound)
            {
                if(item_status_sound!=null) item_status_sound.set_icon(this.sp_icon_sound_off);
                PlayerPrefs.SetInt("is_sound", 1);
                this.is_sound = false;
                if (this.type_app == TypeApp.Game)
                {
                    if(this.game.get_audio_source_bk()!=null) this.game.get_audio_source_bk().Stop();
                }
            }
            else
            {
                if (item_status_sound != null) item_status_sound.set_icon(this.sp_icon_sound_on);
                PlayerPrefs.SetInt("is_sound", 0);
                this.is_sound = true;
                this.play_sound_click();
                if (this.type_app == TypeApp.Game)
                {
                    if (this.game.get_audio_source_bk() != null) this.game.get_audio_source_bk().Play();
                }
            }
            if (item_status_sound != null) item_status_sound.set_change_status(true);
        }

        private void change_status_vibrate(Carrot_Box_Item item_status_vibrate)
        {
            this.play_sound_click();
            item_status_vibrate.set_change_status(true);
            if (this.is_vibrate)
            {
                PlayerPrefs.SetInt("is_vibrate", 1);
                item_status_vibrate.set_icon(this.sp_icon_vibrate_off);
                this.is_vibrate = false;
            }
            else
            {
                PlayerPrefs.SetInt("is_vibrate", 0);
                item_status_vibrate.set_icon(this.sp_icon_vibrate_on);
                this.is_vibrate = true;
                this.play_sound_click();
            }
        }

        private void change_lang_in_setting(string s_data)
        {
            this.item_setting_lang.set_tip(this.lang.Get_key_lang());
            this.item_setting_lang.set_icon_white(this.lang.Get_sp_lang_cur());
            if (this.item_setting_lang != null) this.item_setting_lang.set_change_status(true);
            this.box_setting.set_title(lang.Val("setting", "Setting"));
            foreach (Transform tr in this.box_setting.area_all_item)
            {
                if (tr.gameObject.GetComponent<Carrot_Box_Item>()) tr.gameObject.GetComponent<Carrot_Box_Item>().load_lang_data();
            }
        }

        public void buy_inapp_removeads()
        {
            this.shop.buy_product(this.index_inapp_remove_ads);
        }

        public bool get_status_sound()
        {
            return this.is_sound;
        }

        public bool get_status_vibrate()
        {
            return this.is_vibrate;
        }

        public Color32 get_color_highlight_blur(int fade = 10)
        {
            int c_b_i = (int)this.color_highlight.b;
            int c_r_i = (int)this.color_highlight.r;
            int c_g_i = (int)this.color_highlight.g;

            c_b_i += fade; if (c_b_i > 255) c_b_i = 255;
            c_r_i += fade; if (c_r_i > 255) c_r_i = 255;
            c_g_i += fade; if (c_g_i > 255) c_g_i = 255;

            byte c_b = (byte)c_b_i;
            byte c_r = (byte)c_r_i; ;
            byte c_g = (byte)c_g_i;

            return new Color32(c_r, c_g, c_b, (byte)255);
        }

        public bool close_window(int index_close)
        {
            bool is_close_success = false;
            if (this.list_Window.Count <= index_close) return false;

            if (this.list_Window[index_close] == null) return false;

            Carrot_UI window_cur = this.list_Window[index_close].GetComponent<Carrot_UI>();
            if (window_cur.is_not_destroy)
            {
                if (this.list_Window[index_close].name == "Window_Gamepad")
                    this.list_Window[index_close].GetComponent<Carrot_Gamepad>().panel_gamepad.SetActive(false);
                else
                    this.list_Window[index_close].SetActive(false);
            }
            else
            {
                is_close_success = true;
                Destroy(this.list_Window[index_close].gameObject);
            }


            this.list_Window.RemoveAt(index_close);

            if (this.list_Window.Count > 0)
            {
                if (this.type_control != TypeControl.None)
                {
                    if(this.list_Window[this.list_Window.Count - 1] != null)
                    {
                        Carrot_UI window_last = this.list_Window[this.list_Window.Count - 1].GetComponent<Carrot_UI>();
                        if (window_last != null)
                        {
                            this.game.set_list_button_gamepad_console(window_last.get_list_btn());
                            if (window_last.scrollRect != null) this.game.set_scrollRect_gamepad_consoles(window_last.scrollRect);
                        }
                    }
                }
            }
            else
            {
                if (this.act_after_close_all_box != null) this.act_after_close_all_box();
            }
            return is_close_success;
        }

        public void remove_window(int index_window)
        {
            this.close_window(index_window);
        }

        public void add_window(GameObject obj_window)
        {
            obj_window.GetComponent<Carrot_UI>().index_window = this.list_Window.Count;
            this.list_Window.Add(obj_window);
        }

        public void close_all_window()
        {
            for (int i = 0; i < this.list_Window.Count; i++) this.close_window(i);
            this.list_Window = new List<GameObject>();
        }

        private void carrot_by_success(string s_id_product)
        {
            if (this.index_inapp_remove_ads != -1) if (s_id_product == this.shop.get_id_by_index(this.index_inapp_remove_ads)) this.in_app_remove_ads();
            if (this.index_inapp_buy_bk_music != -1) this.game.check_buy_music_item_bk(s_id_product);
        }

        private void carrot_restore_success(string[] arr_id)
        {
            for (int i = 0; i < arr_id.Length; i++)
            {
                string s_id_product = arr_id[i];
                if (this.index_inapp_remove_ads != -1) if (s_id_product == this.shop.get_id_by_index(this.index_inapp_remove_ads)) this.in_app_remove_ads();
            }
        }

        private void in_app_remove_ads()
        {
            if (item_setting_ads) Destroy(item_setting_ads.gameObject);
            this.ads.remove_ads();
            this.Show_msg(lang.Val("shop", "Shop"), lang.Val("ads_remove_success", "Ad removal successful!"), Msg_Icon.Success);
        }

        public void play_sound_click()
        {
            if (this.is_sound) this.sound_click.Play();
        }

        public void play_vibrate()
        {
#if !UNITY_STANDALONE && !UNITY_WEBGL
            if (this.is_vibrate) Handheld.Vibrate();
#endif
        }

        public void change_sound_click(AudioClip Clip_Click)
        {
            this.sound_click.clip = Clip_Click;
        }

        public void log(string s_log)
        {
            if (this.model_app == ModelApp.Develope) Debug.Log(s_log);
            this.list_log.Add(s_log);
        }

        public void show_log()
        {
            Carrot_Box box_log = this.Create_Box("Log");
            box_log.set_icon(this.icon_carrot_bug);
            box_log.set_title("Review the application activity log");

            for (int i = 0; i < this.list_log.Count; i++)
            {
                Carrot_Box_Item box_log_item = box_log.create_item("log_" + i);
                box_log_item.set_icon(this.icon_carrot_bug);
                box_log_item.set_title("Log " + i);
                box_log_item.set_tip(this.list_log[i]);
            }
        }

        [ContextMenu("CaptureScreenshot")]
        public void CaptureScreenshot()
        {
            ScreenCapture.CaptureScreenshot("screenshot" + UnityEngine.Random.Range(0, 100) + ".png");
        }

        private void act_check_change_model_app()
        {
            this.play_sound_click();
            this.count_change_model_app++;
            if (this.count_change_model_app >= 3)
            {
                this.act_change_model_app();
                this.count_change_model_app = 0;
            }
        }

        private void act_change_model_app()
        {
            if (this.model_app == ModelApp.Publish)
            {
                this.box_setting.set_icon(this.sp_icon_dev);
                this.model_app = ModelApp.Develope;
                this.ads.set_status_ads(false);
            }
            else
            {
                this.box_setting.set_icon(this.sp_icon_setting);
                this.model_app = ModelApp.Publish;
                this.ads.set_status_ads(true);
            }
            this.Show_msg("Change Model App Success!!!", this.model_app.ToString(), Msg_Icon.Success);
        }

        public Carrot_Button_Item create_button(string s_text)
        {
            GameObject obj_btn = Instantiate(this.carrot_btn_prefab);
            Carrot_Button_Item item_btn = obj_btn.GetComponent<Carrot_Button_Item>();
            item_btn.txt_val.text = s_text;
            return item_btn;
        }

        public Carrot_Button_Item create_button(string s_text, Transform tr_father)
        {
            Carrot_Button_Item item_btn = this.create_button(s_text);
            item_btn.transform.SetParent(tr_father);
            item_btn.transform.localPosition = Vector3.zero;
            item_btn.transform.localScale = new Vector3(1f, 1f, 1f);
            item_btn.transform.rotation = Quaternion.Euler(0, 0, 0);
            return item_btn;
        }

        public string generateID()
        {
            return Guid.NewGuid().ToString("N");
        }

        private void Show_box_dev_info()
        {
            Carrot_Box box_dev = this.Create_Box();
            box_dev.set_title("Info");
            box_dev.set_icon(user.icon_user_info);

            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                if (field.DeclaringType == GetType())
                {
                    object value = field.GetValue(this);
                    Carrot_Box_Item item_info = box_dev.create_item();
                    item_info.set_title(field.Name);
                    if(value!=null)
                        item_info.set_tip(value.ToString());
                    else
                        item_info.set_tip("Null");
                }
            }
        }

        [ContextMenu("Set public Goople play")]
        public void Set_public_Gooogle_play()
        {
            this.store_public = Store.Google_Play;
            this.os_app = OS.Android;
            this.type_ads = TypeAds.Admod_Unity_Carrot;
            this.type_rate = TypeRate.Market_Android;
            this.pay_app = PayApp.UnitySDKPay;
        }

        [ContextMenu("Set public Amazon")]
        public void Set_public_Amazon()
        {
            this.store_public = Store.Amazon_app_store;
            this.os_app = OS.Android;
            this.type_ads = TypeAds.Admod_Unity_Carrot;
            this.type_rate = TypeRate.Amazon_app_store;
            this.pay_app = PayApp.UnitySDKPay;
        }

        [ContextMenu("Set public Huawei")]
        public void Set_public_Huawei()
        {
            this.store_public = Store.Huawei_store;
            this.os_app = OS.Android;
            this.type_ads = TypeAds.Admod_Unity_Carrot;
            this.type_rate = TypeRate.Market_Android;
            this.pay_app = PayApp.CarrotPay;
        }

        [ContextMenu("Set public Uptodown")]
        public void Set_public_Uptodown()
        {
            this.store_public = Store.Uptodown;
            this.os_app = OS.Android;
            this.type_ads = TypeAds.Admod_Unity_Carrot;
            this.type_rate = TypeRate.Market_Android;
            this.pay_app = PayApp.CarrotPay;
        }

        [ContextMenu("Set public Microsoft Store")]
        public void Set_public_Microsoft_Store()
        {
#if UNITY_EDITOR
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
#endif
            this.store_public = Store.Microsoft_Store;
            this.os_app = OS.Window;
            this.type_ads = TypeAds.Carrot;
            this.type_rate = TypeRate.Ms_Windows_Store;
            this.pay_app = PayApp.UnitySDKPay;
        }

        [ContextMenu("Set public Itch(Mobile Android)")]
        public void Set_public_Itch_Android()
        {
            this.store_public = Store.Itch;
            this.os_app = OS.Android;
            this.type_ads = TypeAds.Admod_Unity_Carrot;
            this.type_rate = TypeRate.Market_Android;
            this.pay_app = PayApp.CarrotPay;
        }

        [ContextMenu("Set public Itch(Web)")]
        public void Set_public_Itch_Web()
        {
#if UNITY_EDITOR
            PlayerSettings.defaultWebScreenWidth = 1024;
            PlayerSettings.defaultWebScreenHeight = 640;
#endif

            this.store_public = Store.Itch;
            this.os_app = OS.Web;
            this.type_ads = TypeAds.Carrot;
            this.type_rate = TypeRate.Ms_Windows_Store;
            this.pay_app = PayApp.CarrotPay;
        }

        [ContextMenu("Set public Itch(Desktop)")]
        public void Set_public_Itch_Desktop()
        {
#if UNITY_EDITOR
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1024;
            PlayerSettings.defaultScreenHeight = 620;
#endif
            this.store_public = Store.Itch;
            this.os_app = OS.Window;
            this.type_ads = TypeAds.Carrot;
            this.type_rate = TypeRate.Ms_Windows_Store;
            this.pay_app = PayApp.CarrotPay;
        }
    }
}