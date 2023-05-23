using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Carrot
{
    public enum ModelApp { Publish, Develope }
    public enum Host { Carrotstore, Localhost }
    public enum OS {Android,Window,Ios,Web};
    public enum Store { Google_Play, Samsung_Galaxy_Store, Microsoft_Store, Amazon_app_store, Carrot_store};
    public enum TypeApp { App,Game}
    public enum TypeRate {Market_Android,Ms_Windows_Store,Amazon_app_store,Link_Share_CarrotApp}
    public enum TypeAds {Admob_Ads,Vungle_Ads,Carrot_Ads,all_ads}
    public enum TypeControl {None, GamePad,D_pad}
    public enum Setting_Option { Show,Hide }

    public enum PayApp { UnitySDKPay, CarrotPay }

    public class On_event_change: UnityEvent{};
    public class Carrot : MonoBehaviour
    {
        [Header("Config App")]
        public Host host_app;
        public ModelApp model_app;
        public OS os_app;
        public Store store_public;
        public TypeApp type_app;
        public PayApp pay_app;
        public TypeRate type_rate;
        public TypeAds type_ads;
        public TypeControl type_control;
        private string url;
        private string s_os;
        private string s_store;
        public string path_php_file_action;
        public string link_share_app;
        public string WindowUWP_ProductId;
        public string Carrotstore_AppId;
        public Color32 color_highlight;
        public bool check_lost_internet;
        public bool auto_open_rate_store=true;

        private bool is_next_check_exit = true;
        private bool is_ready_cache_app_share = false;
        private bool is_sound = true;
        private bool is_vibrate = true;
        private Carrot_list_app carrot_list_app;
        private Carrot_tool tool;
        private List<GameObject> list_Window;
        private Carrot_Window_Msg msg_lost_interne;
        private Carrot_Window_Msg msg_delete_all_data;
        private Carrot_Window_Loading loading;
        private List<string> list_log;
        private int count_change_model_app=0;

        [Header("Login Obj")]
        public Image img_btn_login;

        [Header("Lang set Emp app")]
        public Carrot_lang_show emp_show_lang;

        [Header("Setting Config")]
        [Tooltip("Change the value to enable the ad buying function")]
        public int index_inapp_remove_ads = -1;
        [Tooltip("Change the value to enable the background music buying function")]
        public int index_inapp_buy_bk_music = -1;
        public Setting_Option setting_lang=Setting_Option.Hide;
        public Setting_Option setting_login = Setting_Option.Hide;
        public Setting_Option setting_rank = Setting_Option.Hide;
        public Setting_Option setting_vibrate = Setting_Option.Hide;
        public Setting_Option setting_soundtrack = Setting_Option.Hide;
        public Setting_Option setting_theme = Setting_Option.Hide;

        [Header("Ads Config")]
        public int click_show_ads = 5;
        public bool show_banner_ads_start = true;
        public GameObject[] emp_btn_remove_ads;

        [Header("Admob Ads Android config")]
        public string id_ads_Interstitial_android;
        public string id_ads_Banner_android;
        public string id_ads_Rewarded_android;

        [Header("Admob Ads Ios config")]
        public string id_ads_Interstitial_ios = "unused";
        public string id_ads_Banner_ios = "unused";
        public string id_ads_Rewarded_ios = "unused";

        [Header("Ads Vungle config")]
        public string ads_vungle_game_id_window = "";
        public string ads_vungle_interstitial_placement_id = "";
        public string ads_vungle_rewarded_placement_id = "";

        [Header("Carrot Obj")]
        public Carrot_lang lang;
        public Carrot_User user;
        public Carrot_shop shop;
        public Carrot_ads ads;
        public Carrot_game game;
        public Carrot_camera camera_pro;
        public Carrot_location location;
        public Carrot_Theme theme;

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

        [Header("Lost Internet")]
        private bool is_online_internet=true;
        public Sprite icon_lost_internet;

        public Carrot_lost_internet carrot_lost_internet;

        [Header("Setting Carrot")]
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
        public AudioSource sound_click;
        Carrot_Box box_setting = null;
        Carrot_Box_Item item_setting_lang = null;
        Carrot_Box_Item item_setting_ads = null;

        [Header("Event Customer")]
        public UnityAction act_check_exit_app;
        public UnityAction act_after_close_all_box;
        public UnityAction act_after_delete_all_data;

        private bool is_ready = false;

        public void Load_Carrot()
        {
            this.list_log = new List<string>();
            if (this.host_app == Host.Carrotstore) this.url = "https://carrotstore.com";
            if (this.host_app == Host.Localhost) this.url = "http://localhost";

            this.s_store = this.store_public.ToString().ToLower();
            this.s_os = this.os_app.ToString().ToLower();

            if (this.type_app == TypeApp.App)
                Destroy(this.GetComponent<Carrot_game>());
            else
                this.GetComponent<Carrot_game>().load_carrot_game();

            this.tool = new Carrot_tool();
            this.lang.load(this);
            this.user.load(this);
            this.camera_pro.load(this);
            this.shop.load(this);
            this.shop.onCarrotPaySuccess += this.carrot_by_success;
            this.shop.onCarrotRestoreSuccess += this.carrot_restore_success;
            this.ads.load(this);
            this.theme.on_load(this);

            if (this.type_app == TypeApp.Game) this.ads.onRewardedSuccess += this.GetComponent<Carrot_game>().onRewardedSuccess;
            this.ads.onRewardedSuccess += this.theme.onRewardedSuccess;
            
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
        public string get_url() { return this.url + "/" + this.path_php_file_action; }
        public string get_url_host() { return this.url; }
        public void clear_contain(Transform area_body) { foreach (Transform child in area_body) { Destroy(child.gameObject); } }
        public void show_login() { this.user.show_login(); }
        public void show_user_register() { this.user.show_user_register(); }
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

                    if (this.list_Window[index_box_check].name == "window_ads") return;

                    if (this.list_Window[index_box_check].name== "window_exit")
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
            GameObject window_share=this.create_window(this.window_share_prefab);
            window_share.GetComponent<Carrot_Window_Share>().load(this);
        }

        public void show_share(string link_customer,string s_share_tip)
        {
            GameObject window_share = this.create_window(this.window_share_prefab);
            window_share.GetComponent<Carrot_Window_Share>().load(this);
            window_share.GetComponent<Carrot_Window_Share>().txt_share_tip.text = s_share_tip;
            window_share.GetComponent<Carrot_Window_Share>().inp_link_share.text = link_customer;
        }

        public bool get_status_cache_app_share() { return this.is_ready_cache_app_share; }
        public void set_status_cache_app_share(bool is_ready) { this.is_ready_cache_app_share = is_ready; }
        #endregion

        public void close()
        {
            this.close_all_window();
            if(this.act_after_close_all_box!=null) this.act_after_close_all_box();
        }

        private Carrot_Window_Msg create_msg()
        {
            GameObject window_msg= this.create_window(this.window_msg_prefab);
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

        public Carrot_Window_Msg show_msg(string s_title, string s_msg, Msg_Icon icon)
        {
            Carrot_Window_Msg msg = show_msg(s_title, s_msg);
            msg.set_icon(icon);
            return msg;
        }

        public Carrot_Window_Msg show_msg(string s_title, string s_msg,UnityAction act_msg_yes, UnityAction act_msg_no)
        {
            Carrot_Window_Msg msg = show_msg(s_title, s_msg);
            msg.set_icon(Msg_Icon.Question);
            msg.add_btn_msg(PlayerPrefs.GetString("msg_yes","Yes"), act_msg_yes);
            msg.add_btn_msg(PlayerPrefs.GetString("cancel", "No"), act_msg_no);
            msg.update_btns_gamepad_console();
            return msg;
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
            GameObject box_grid=this.create_window(this.window_box_prefab);
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

        public void get_img(string url,Image img)
        {
            if (url == "") return;
            StartCoroutine(this.tool.get_img_form_url(url,img));
        }

        public void get_img(string url, UnityAction<Texture2D> act_done,UnityAction<string> act_fail=null)
        {
            if (url == "") return;
            StartCoroutine(this.tool.get_img_form_url(url,null, act_done, act_fail));
        }

        public void get_img_and_save_file(string url, Image img,string path_save){StartCoroutine(this.tool.get_img_form_url_and_save_file(url, img, path_save));}

        public void get_img_and_save_playerPrefs(string url, Image img, string s_key,UnityAction<Texture2D> act_done=null, UnityAction<string> act_fail = null)
        {
            if (url == "") return;
            StartCoroutine(this.tool.get_img_form_url_and_save_playerPrefs(url, img, s_key, act_done, act_fail));
        }

        public void get_img(string url, UnityAction<Texture2D> act_download_img)
        {
            StartCoroutine(this.tool.download_img_form_url(url, act_download_img));
        }

        public WWWForm frm_act(string func)
        {
            if (this.model_app == ModelApp.Develope) this.log("Create Form:" + func);
            WWWForm frm = new WWWForm();
            frm.AddField("function", func);
            frm.AddField("lang", PlayerPrefs.GetString("lang", "en"));
            frm.AddField("lang_name", PlayerPrefs.GetString("lang_name", "English"));
            frm.AddField("lang_sys", Application.systemLanguage.ToString());
            frm.AddField("id_device", SystemInfo.deviceUniqueIdentifier);
            frm.AddField("os", this.s_os);
            frm.AddField("store", this.s_store);
            return frm;
        }

        public Carrot_Window_Input show_search(UnityAction<string> act_done_search)
        {
            GameObject obj_search=this.create_window(this.window_input_prefab);
            Carrot_Window_Input box_search=obj_search.GetComponent<Carrot_Window_Input>();
            box_search.load(this);
            box_search.set_icon(this.icon_carrot_search);
            box_search.set_title(PlayerPrefs.GetString("search","Search"));
            box_search.set_tip("");
            box_search.set_frm(this.frm_act("search"));
            box_search.set_act_done(act_done_search);
            return box_search;
        }

        public Carrot_Window_Input show_search(WWWForm frm_search, UnityAction<string> act_done_search,string s_search_tip)
        {
            Carrot_Window_Input box_search = this.show_search(act_done_search);
            box_search.set_frm(frm_search);
            box_search.set_tip(s_search_tip);
            return box_search;
        }

        public Carrot_Window_Input show_input(string s_title, string s_tip="",string s_txt="",Window_Input_value_Type type_val=Window_Input_value_Type.input_field)
        {
            GameObject obj_box_inp= this.create_window(this.window_input_prefab);
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
        public void delete_all_data()
        {
            this.play_sound_click();
           this.msg_delete_all_data=this.show_msg(PlayerPrefs.GetString("delete_all_data", "Clear all application data"), PlayerPrefs.GetString("question_delete_all_data", "Confirm erase all data and set up?"),this.act_delete_all_data_yes,this.act_delete_all_data_no);
        }

        private void act_delete_all_data_yes()
        {
            this.msg_delete_all_data.close();
            this.msg_delete_all_data = this.show_msg(PlayerPrefs.GetString("delete_all_data", "Clear all application data"), PlayerPrefs.GetString("delete_all_data_success", "Erase all settings settings and app data successfully!"), Msg_Icon.Success);
            this.user.delete_data_user_login();
            PlayerPrefs.DeleteAll();
            this.get_tool().delete_file("lang_icon");
            if (type_app == TypeApp.Game) this.get_tool().delete_file("music_bk");
            if (model_app == ModelApp.Develope) Debug.Log("Delete All Data Success!!!");
            this.delay_function(1f, this.restart_app);
        }

        private void restart_app()
        {
            this.msg_delete_all_data.close();
            this.close_all_window();
            this.Load_Carrot(this.act_check_exit_app);
            if (this.act_after_delete_all_data != null) this.act_after_delete_all_data();
        }

        private void act_delete_all_data_no()
        {
            this.msg_delete_all_data.close();
            if (model_app == ModelApp.Develope) this.log("Cancel Delete All Data!");
        }

        public Carrot_Window_Loading send(WWWForm frm, UnityAction<string> done_func=null, UnityAction<string> fail_func=null)
        {
            if (model_app == ModelApp.Develope) this.log("Send Request.."+frm.ToString());
            return this.show_loading(act_send(frm, done_func, fail_func));
        }

        IEnumerator act_send(WWWForm frm_send, UnityAction<string> done_func, UnityAction<string> fail_func)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm_send))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                    if(fail_func!=null) fail_func(www.error);
                }
                else
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) Debug.Log("Server response:" + www.downloadHandler.text);
                    if(done_func!=null) done_func(www.downloadHandler.text);
                }
            }
        }

        public void send_hide(WWWForm frm, UnityAction<string> done_func=null, UnityAction<string> error_func = null)
        {
            StartCoroutine(act_send_hide(frm, done_func, error_func));
        }

        IEnumerator act_send_hide(WWWForm frm_send,UnityAction<string> done_func=null, UnityAction<string> error_func=null)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm_send))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    if (model_app == ModelApp.Develope) Debug.Log("Send hide server response:" + www.downloadHandler.text);
                    if(done_func!=null) done_func(www.downloadHandler.text);
                }
                else
                {
                    if(error_func!=null) error_func(www.error);
                }
            }
        }

        public void show_list_lang()
        {
            this.lang.show_list_lang();
        }

        public void show_list_lang(UnityAction<string> call_func)
        {
            this.lang.show_list_lang(call_func);
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

        public Carrot_Window_Loading get_mp3(string url,UnityAction<UnityWebRequest> act_success, UnityAction act_fail=null)
        {
            this.loading=this.show_loading_with_process_bar(get_mp3_form_url(url, act_success, act_fail));
            return this.loading;
        }

        IEnumerator get_mp3_form_url(string s_url_audio, UnityAction<UnityWebRequest> act_success, UnityAction act_fail)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(s_url_audio, AudioType.MPEG))
            {
                www.SendWebRequest();
                while (!www.isDone)
                {
                    if(this.loading!=null) this.loading.slider_loading.value = www.downloadProgress;
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
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                this.is_online_internet = false;
                return true;
            }
            else {
                this.is_online_internet = true;
                return false;
            }
        }

        public bool is_online()
        {
            return this.is_online_internet;
        }

        public void check_connect_internet()
        {
            bool status_internet_offline = is_offline();
            if (status_internet_offline)
            {
                if (this.carrot_lost_internet != null) this.carrot_lost_internet.carrot = this;
                this.msg_lost_interne=this.create_msg();
                msg_lost_interne.set_title(PlayerPrefs.GetString("lost_connect", "Lost Internet connection"));
                msg_lost_interne.set_title(PlayerPrefs.GetString("lost_connect_msg", "Please check your network connection, currently the app cannot access the Internet"));
                msg_lost_interne.set_icon_customer(this.icon_lost_internet);
                msg_lost_interne.add_btn_msg(PlayerPrefs.GetString("lost_connect_try", "Try checking again!"), act_try_connect_internet);
                this.carrot_lost_internet.try_connect();
            }
            if(this.carrot_lost_internet!=null) this.carrot_lost_internet.set_model_by_status_internet(status_internet_offline);
        }

        private void act_try_connect_internet()
        {
            this.msg_lost_interne.close();
        }

        public void delay_function(float timer,UnityAction act_func)
        {
            if(this.is_ready) if (this.model_app == ModelApp.Develope) this.log("Delay function " + timer + "s");
            StartCoroutine(act_try_connect(timer, act_func));
        }

        private IEnumerator act_try_connect(float timer,UnityAction act_func)
        {
            yield return new WaitForSeconds(timer);
            act_func();
        }

        public GameObject create_window(GameObject obj_prefab)
        {
            GameObject obj_window = Instantiate(obj_prefab);
            obj_window.name = "Carrot Window";
            obj_window.transform.SetParent(GameObject.Find("Canvas").transform);
            obj_window.transform.localPosition = new Vector3(obj_window.transform.localPosition.x, obj_window.transform.localPosition.y,0f);
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
            Carrot_Box box= box_window.GetComponent<Carrot_Box>();
            box.load(this);
            return box.GetComponent<Carrot_Box>();
        }

        public Carrot_Box Create_Box(string s_title)
        {
            Carrot_Box box = this.Create_Box();
            box.set_title(s_title);
            return box;
        }

        public Carrot_Box Create_Box(string s_title,Sprite icon)
        {
            Carrot_Box box = this.Create_Box(s_title);
            box.set_icon(icon);
            return box;
        }

        public Carrot_Box Create_Setting(GameObject[] array_item_customer=null)
        {
            this.play_sound_click();
            this.box_setting = this.Create_Box(PlayerPrefs.GetString("setting", "Setting"));
            if (this.model_app == ModelApp.Publish)
                this.box_setting.set_icon(this.sp_icon_setting);
            else
                this.box_setting.set_icon(this.sp_icon_dev);

            box_setting.name = "Box Setting";
            Button btn_icon_setting=this.box_setting.img_icon.gameObject.AddComponent<Button>();
            btn_icon_setting.onClick.AddListener(() => this.act_check_change_model_app());

            if (array_item_customer != null) for(int i=0;i<array_item_customer.Length;i++) box_setting.add_item(array_item_customer[i]);

            if (this.setting_lang == Setting_Option.Show)
            {
                this.item_setting_lang = box_setting.create_item("lang");
                item_setting_lang.set_icon_white(this.lang.get_sp_lang_cur());
                item_setting_lang.set_title(PlayerPrefs.GetString("sel_lang_app", "Change application language"));
                item_setting_lang.set_tip(this.lang.get_name_lang());
                item_setting_lang.set_lang_data("sel_lang_app", "sel_lang_app_tip");
                item_setting_lang.set_act(() => this.lang.show_list_lang(change_lang_in_setting));
            }

            if (this.setting_login == Setting_Option.Show)
            {
                Carrot_Box_Item item_setting_login = box_setting.create_item("setting_login");
                if (this.user.get_id_user_login() == "")
                {
                    item_setting_login.set_title(PlayerPrefs.GetString("login", "Login"));
                    item_setting_login.set_tip(PlayerPrefs.GetString("login_tip", "Sign in to your carrot account to manage data, and use many other services"));
                    item_setting_login.set_lang_data("login", "login_tip");

                    Carrot_Box_Btn_Item item_btn_regiter= item_setting_login.create_item();
                    item_btn_regiter.set_icon(this.user.icon_user_register);
                    item_btn_regiter.set_color(this.color_highlight);
                    item_btn_regiter.set_act(this.user.show_user_register);

                    Carrot_Box_Btn_Item item_btn_password = item_setting_login.create_item();
                    item_btn_password.set_icon(this.user.icon_user_change_password);
                    item_btn_password.set_color(this.color_highlight);
                    item_btn_password.set_act(this.user.show_window_lost_password);
                }
                else
                {
                    item_setting_login.set_title(PlayerPrefs.GetString("acc_info", "Account Information"));
                    item_setting_login.set_tip(PlayerPrefs.GetString("acc_edit_tip", "Click this button to update account information"));
                    item_setting_login.set_lang_data("acc_info", "acc_edit_tip");
                }

                item_setting_login.set_act(() => this.user.show_login(this.reload_setting));
                this.user.load_avatar_user_login(item_setting_login.img_icon);
            }

            if (this.setting_rank == Setting_Option.Show)
            {
                Carrot_Box_Item item_setting_top_player = box_setting.create_item("top_player");
                item_setting_top_player.set_icon(this.game.icon_top_player);
                item_setting_top_player.set_title("Player rankings");
                item_setting_top_player.set_tip("User score leaderboard");
                item_setting_top_player.set_act(this.game.Show_List_Top_player);
                item_setting_top_player.set_lang_data("top_player", "top_player_tip");
            }

            Carrot_Box_Item item_setting_sound = box_setting.create_item("sound");
            if(this.is_sound)
                item_setting_sound.set_icon(this.sp_icon_sound_on);
            else
                item_setting_sound.set_icon(this.sp_icon_sound_off);
            item_setting_sound.set_title(PlayerPrefs.GetString("sound_app", "Sound"));
            item_setting_sound.set_tip(PlayerPrefs.GetString("sound_app_tip", "On or Off Sound click"));
            item_setting_sound.set_lang_data("sound_app", "sound_app_tip");
            item_setting_sound.set_act(()=>this.change_status_sound(item_setting_sound));

            if (this.index_inapp_remove_ads != -1)
            {
                this.item_setting_ads = box_setting.create_item("remove_ads");
                this.item_setting_ads.set_icon(this.sp_icon_removeads);
                this.item_setting_ads.set_title(PlayerPrefs.GetString("remove_ads", "Remove Ads"));
                this.item_setting_ads.set_tip(PlayerPrefs.GetString("remove_ads_tip", "Buy and remove advertising function, No ads in the app"));
                this.item_setting_ads.set_lang_data("remove_ads", "remove_ads_tip");
                this.item_setting_ads.set_act(this.buy_inapp_removeads);

                if (this.ads.get_status_ads()) this.item_setting_ads.gameObject.SetActive(true);
                else this.item_setting_ads.gameObject.SetActive(false);
            }

            if (this.type_app == TypeApp.Game)
            {
                if (this.index_inapp_buy_bk_music != -1)
                {
                    if (this.setting_soundtrack == Setting_Option.Show)
                    {
                        Carrot_Box_Item item_setting_bk_music = box_setting.create_item("list_bk_music");
                        item_setting_bk_music.set_icon(this.sp_icon_bk_music);
                        item_setting_bk_music.set_title(PlayerPrefs.GetString("list_bk_music", "Soundtrack"));
                        item_setting_bk_music.set_tip(PlayerPrefs.GetString("list_bk_music_tip", "Select and change background music"));
                        item_setting_bk_music.set_lang_data("list_bk_music", "list_bk_music_tip");
                        item_setting_bk_music.set_act(() => this.game.show_list_music_game(item_setting_bk_music));

                        if (this.tool.check_file_exist("music_bk"))
                        {
                            Carrot_Box_Btn_Item btn_del_bk_music = item_setting_bk_music.create_item();
                            btn_del_bk_music.set_color(this.color_highlight);
                            btn_del_bk_music.set_icon(this.sp_icon_del_data);
                            btn_del_bk_music.GetComponent<Button>().onClick.RemoveAllListeners();
                            btn_del_bk_music.GetComponent<Button>().onClick.AddListener(() => this.game.delete_bk_music());
                            btn_del_bk_music.GetComponent<Button>().onClick.AddListener(() => this.reload_setting());
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
            }

            if (this.setting_vibrate == Setting_Option.Show)
            {
                Carrot_Box_Item item_setting_vibrate = box_setting.create_item("vibrate");
                if (this.is_vibrate)
                    item_setting_vibrate.set_icon(this.sp_icon_vibrate_on);
                else
                    item_setting_vibrate.set_icon(this.sp_icon_vibrate_off);
                item_setting_vibrate.set_title(PlayerPrefs.GetString("vibrate_app", "Vibrate"));
                item_setting_vibrate.set_tip(PlayerPrefs.GetString("vibrate_app_tip", "Turn vibration on or off"));
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
            item_setting_rate.set_title(PlayerPrefs.GetString("rate","Evaluate"));
            item_setting_rate.set_tip(PlayerPrefs.GetString("rate_tip","Please take a moment of your time to rate this app."));
            item_setting_rate.set_lang_data("rate", "rate_tip");
            item_setting_rate.set_act(this.show_rate);

            Carrot_Box_Item item_setting_share = box_setting.create_item("share");
            item_setting_share.set_icon(this.sp_icon_share);
            item_setting_share.set_title(PlayerPrefs.GetString("share","Share"));
            item_setting_share.set_tip(PlayerPrefs.GetString("share_tip", "Choose the platform below to share this great app with your friends or others"));
            item_setting_share.set_lang_data("share", "share_tip");
            item_setting_share.set_act(this.show_share);

            Carrot_Box_Item item_setting_other_app = box_setting.create_item();
            item_setting_other_app.set_icon(this.sp_icon_more_app);
            item_setting_other_app.set_title(PlayerPrefs.GetString("list_app_carrot", "Applications from the developer"));
            item_setting_other_app.set_tip(PlayerPrefs.GetString("exit_app_other", "Perhaps you will love our other apps"));
            item_setting_other_app.set_lang_data("list_app_carrot", "exit_app_other");
            item_setting_other_app.set_act(this.show_list_carrot_app);

            Carrot_Box_Item item_setting_restore = box_setting.create_item();
            item_setting_restore.set_icon(this.sp_icon_restore);
            item_setting_restore.set_title(PlayerPrefs.GetString("in_app_restore", "Restore purchased items"));
            item_setting_restore.set_tip(PlayerPrefs.GetString("in_app_restore_tip", "Restore purchased services and functions"));
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
            }

            Carrot_Box_Item item_setting_del_data = box_setting.create_item();
            item_setting_del_data.set_icon(this.sp_icon_del_data);
            item_setting_del_data.set_title(PlayerPrefs.GetString("delete_all_data", "Clear all application data"));
            item_setting_del_data.set_tip(PlayerPrefs.GetString("delete_all_data_tip", "Erase all data and reinstall the app"));
            item_setting_del_data.set_lang_data("delete_all_data", "delete_all_data_tip");
            item_setting_del_data.set_act(this.delete_all_data);
            return box_setting;
        }

        private void reload_setting()
        {
            if (this.box_setting != null) this.box_setting.close();
            if (this.box_setting.get_act_before_closing()!=null)
            {
                UnityAction act_close_setting = this.box_setting.get_act_before_closing();
                this.box_setting=this.Create_Setting();
                this.box_setting.set_act_before_closing(act_close_setting);
            }else if(this.box_setting.get_act_before_closing_change() != null)
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

        private void change_status_sound(Carrot_Box_Item item_status_sound)
        {
            if (this.is_sound)
            {
                item_status_sound.set_icon(this.sp_icon_sound_off);
                PlayerPrefs.SetInt("is_sound", 1);
                this.is_sound = false;
                if (this.type_app == TypeApp.Game) this.game.get_audio_source_bk().Stop();
            }
            else
            {
                item_status_sound.set_icon(this.sp_icon_sound_on);
                PlayerPrefs.SetInt("is_sound", 0);
                this.is_sound = true;
                this.play_sound_click();
                if (this.type_app == TypeApp.Game) this.game.get_audio_source_bk().Play();
            }
            item_status_sound.set_change_status(true);
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
            this.item_setting_lang.set_tip(this.lang.get_name_lang());
            this.item_setting_lang.set_icon_white(this.lang.get_sp_lang_cur());
            if (this.item_setting_lang != null) this.item_setting_lang.set_change_status(true);
            this.box_setting.set_title(PlayerPrefs.GetString("setting", "Setting"));
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

            c_b_i = c_b_i + fade; if (c_b_i > 255) c_b_i = 255;
            c_r_i = c_r_i + fade; if (c_r_i > 255) c_r_i = 255;
            c_g_i = c_g_i + fade; if (c_g_i > 255) c_g_i = 255;

            byte c_b = (byte)c_b_i;
            byte c_r = (byte)c_r_i; ;
            byte c_g = (byte)c_g_i;

            return new Color32(c_r, c_g, c_b,(byte) 255);
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
                    Carrot_UI window_last = this.list_Window[this.list_Window.Count - 1].GetComponent<Carrot_UI>();
                    if (window_last != null)
                    {
                        this.game.set_list_button_gamepad_console(window_last.get_list_btn());
                        if (window_last.scrollRect != null) this.game.set_scrollRect_gamepad_consoles(window_last.scrollRect);
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
            if(this.index_inapp_remove_ads!=-1) if (s_id_product == this.shop.get_id_by_index(this.index_inapp_remove_ads)) this.in_app_remove_ads();
            if(this.index_inapp_buy_bk_music != -1) this.game.check_buy_music_item_bk(s_id_product);
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
            this.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("ads_remove_success", "Ad removal successful!"), Msg_Icon.Success);
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
            Carrot_Box box_log=this.Create_Box("Log");
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
            ScreenCapture.CaptureScreenshot("screenshot"+Random.Range(0,100)+".png");
        }

        private void act_check_change_model_app()
        {
            this.play_sound_click();
            this.count_change_model_app++;
            if (this.count_change_model_app >= 3) {
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
            }
            else
            {
                this.box_setting.set_icon(this.sp_icon_setting);
                this.model_app = ModelApp.Publish;
            }
            this.show_msg("Change Model App Success!!!",this.model_app.ToString(),Msg_Icon.Success);
        }

        public Carrot_Button_Item create_button(string s_text)
        {
            GameObject obj_btn = Instantiate(this.carrot_btn_prefab);
            Carrot_Button_Item item_btn= obj_btn.GetComponent<Carrot_Button_Item>();
            item_btn.txt_val.text = s_text;
            return item_btn;
        }

        public Carrot_Button_Item create_button(string s_text,Transform tr_father)
        {
            Carrot_Button_Item item_btn = this.create_button(s_text);
            item_btn.transform.SetParent(tr_father);
            item_btn.transform.localPosition = Vector3.zero;
            item_btn.transform.localScale = new Vector3(1f, 1f, 1f);
            item_btn.transform.rotation = Quaternion.Euler(0, 0, 0);
            return item_btn;
        }
    }
}