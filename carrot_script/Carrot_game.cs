using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Carrot
{
    public struct Carrot_rank_data
    {
        public Carrot_Rate_user_data user { get; set; }
        public string type { get; set; }
        public string scores { get; set; }
        public string date { get; set; }
    }

    public class carrot_game_rank_type
    {
        public string s_name;
        public Sprite icon;
    }

    public enum Carrot_game_rank_order
    {
        Descending,
        Ascending
    }

    public class Carrot_game : MonoBehaviour
    {
        private Carrot carrot;
        [Header("Music Background Game")]
        public Sprite icon_list_music_game;
        public Sprite icon_pause_music_game;
        public Sprite icon_play_music_game;
        public Sprite icon_waiting_music_game;
        private AudioSource sound_bk_game = null;
        public AudioSource sound_bk_game_test;
        private Carrot_Box_Item box_setting_item_bkmusic = null;
        private Carrot_Box box_list;
        private List<string> list_link_music_bk;
        private byte[] data_music_temp;
        private Carrot_Box_Item item_bk_music_play;
        private string id_select_play_bk_music;
        private string id_buy_bk_music_temp;
        private int index_buy_music_link_temp;

        [Header("GamePad")]
        public GameObject obj_gamepad_prefab;
        private int index_gampad_item = 0;
        private List<GameObject> list_obj_gamapad_console;
        private ScrollRect scrollRect_gamepad_console;
        private Color32 color_sel_item_btn_gamepad;
        private Color32 color_nomal_item_btn_gamepad;
        private List<Carrot_Gamepad> list_gamepad;
        private bool is_user_gampad_for_console=true;
        private UnityAction<bool> act_handle_detection = null;

        [Header("Top player")]
        public Sprite icon_top_player;
        public GameObject item_top_player_prefab;
        public Sprite[] icon_rank_player;
        private string s_data_offline_rank_player;
        private int index_edit_rank = -1;
        private int rank_scores_temp = 0;
        private int rank_type_temp = 0;
        private Carrot_game_rank_order order_top_player = Carrot_game_rank_order.Descending;
        private List<carrot_game_rank_type> list_rank_type;

        public void Load_carrot_game()
        {
            this.carrot = this.GetComponent<Carrot>();
            this.sound_bk_game = this.GetComponent<AudioSource>();

            if (this.carrot.type_control != TypeControl.None)
            {
                this.list_gamepad = new List<Carrot_Gamepad>();
                this.check_connect_gamepad();
                EventSystem.current.sendNavigationEvents = false;
            }
            this.color_sel_item_btn_gamepad = this.carrot.get_color_highlight_blur(100);
            this.color_nomal_item_btn_gamepad = Color.white;

            this.list_rank_type = new List<carrot_game_rank_type>();
            if(this.carrot.is_offline()) this.s_data_offline_rank_player = PlayerPrefs.GetString("s_data_offline_rank_player");
        }

        #region Background Music Game
        public void show_list_music_game(Carrot_Box_Item item_setting=null)
        {
            this.carrot.show_loading();
            this.box_setting_item_bkmusic = item_setting;
            StructuredQuery q = new("audio");
            q.Set_limit(20);
            this.carrot.server.Get_doc(q.ToJson(), get_list_music_game_done, get_list_music_game_fail);
        }

        private void get_list_music_game_done(string s_data)
        {
            this.carrot.hide_loading();
            Fire_Collection fc = new(s_data);
            if (!fc.is_null)
            {
                List<IDictionary> list_music = new();
                for(int i=0;i<fc.fire_document.Length;i++)
                {
                    IDictionary audio = fc.fire_document[i].Get_IDictionary();
                    list_music.Add(audio);
                };
                this.carrot.log("show_list_music_game from server..." + list_music.Count);

                if (this.sound_bk_game != null) this.sound_bk_game.Stop();
                if (this.box_list != null) this.box_list.close();
                this.box_list = this.carrot.Create_Box("carrot_list_bk_music");
                box_list.set_title(this.carrot.lang.Val("list_bk_music","Background music games"));
                box_list.set_icon(this.icon_list_music_game);
                box_list.set_act_before_closing(this.act_close_list_music);

                this.list_link_music_bk = new List<string>();
                this.item_bk_music_play = null;
                this.id_select_play_bk_music = "";
                this.id_buy_bk_music_temp = "";
                this.index_buy_music_link_temp = -1;

                for (int i = 0; i < list_music.Count; i++)
                {
                    IDictionary item_data_music = (IDictionary)list_music[i];

                    Carrot_Box_Item item_music_bk = this.box_list.create_item("item_bk_music_" + i);
                    item_music_bk.set_icon(this.icon_list_music_game);
                    item_music_bk.set_title(item_data_music["name"].ToString());
                    this.list_link_music_bk.Add(item_data_music["mp3"].ToString());

                    var index_link = i;
                    var id_bk_music = item_data_music["id"].ToString();
                    Carrot_Box_Btn_Item btn_play = item_music_bk.create_item();
                    btn_play.set_icon(this.icon_play_music_game);
                    btn_play.set_color(this.carrot.color_highlight);
                    btn_play.set_act(() => this.play_item_music_background_game(item_music_bk, index_link, id_bk_music));

                    bool is_buy = false;

                    if (item_data_music["buy"].ToString() == "0") is_buy = false;
                    else is_buy = true;

                    if (is_buy)
                    {
                        if (PlayerPrefs.GetInt("is_buy_bk_" + item_data_music["id"].ToString(), 0) == 1) is_buy = false;
                    }

                    if (is_buy)
                    {
                        Carrot_Box_Btn_Item btn_buy = item_music_bk.create_item();
                        btn_buy.set_icon(this.carrot.icon_carrot_buy);
                        btn_buy.set_color(this.carrot.color_highlight);
                        btn_buy.set_act(() => this.act_buy_music_bk(id_bk_music, index_link));
                        item_music_bk.set_tip("Please buy to use this track");
                        item_music_bk.set_act(() => this.act_buy_music_bk(id_bk_music, index_link));

                        if (carrot.os_app != OS.Window)
                        {
                            if (this.carrot.id_ads_Rewarded_android != "" || this.carrot.id_ads_Rewarded_ios != "")
                            {
                                Carrot_Box_Btn_Item btn_ads = item_music_bk.create_item();
                                btn_ads.set_icon(this.carrot.icon_carrot_ads);
                                btn_ads.set_color(this.carrot.color_highlight);
                                btn_ads.set_act(() => this.act_watch_ads_to_music_bk(id_bk_music, index_link));
                            }
                        }
                    }
                    else
                    {
                        item_music_bk.set_tip("Free");
                        item_music_bk.set_act(() => this.act_change_bk_music_game(id_bk_music, index_link));
                    }

                    this.box_list.update_color_table_row();
                }
                if (this.carrot.type_control != TypeControl.None)
                {
                    this.set_list_button_gamepad_console(box_list.UI.get_list_btn());
                    this.set_scrollRect_gamepad_consoles(box_list.UI.scrollRect);
                }
            }
            else
            {
                this.carrot.Show_msg("Background music games", "There are no songs in the list");
            }
        }

        private void get_list_music_game_fail(string s_error)
        {
            this.carrot.hide_loading();
            this.carrot.log(s_error);
        }

        private void act_close_list_music()
        {
            this.id_buy_bk_music_temp = "";
            this.index_buy_music_link_temp = -1;
            this.sound_bk_game_test.Stop();
            if(this.carrot.get_status_sound())
                if(this.sound_bk_game!=null) this.sound_bk_game.Play();
            else
                if(this.sound_bk_game!=null) this.sound_bk_game.Stop();
        }

        public void play_item_music_background_game(Carrot_Box_Item item_muic_bk,int index_links,string id_music_bk)
        {
            if(this.id_select_play_bk_music== id_music_bk)
            {
                this.sound_bk_game_test.Stop();
                this.item_bk_music_play.set_icon(this.icon_list_music_game);
                this.id_select_play_bk_music = "";
                return;
            }

            if (this.item_bk_music_play != null)
            {
                this.sound_bk_game_test.Stop();
                this.item_bk_music_play.set_icon(this.icon_list_music_game);
            }

            this.item_bk_music_play = item_muic_bk;
            this.id_select_play_bk_music = id_music_bk;
            
            this.carrot.log("id_select_play_bk_music:" + id_select_play_bk_music);
            this.item_bk_music_play.set_icon(this.icon_waiting_music_game);
            this.carrot.get_mp3(this.list_link_music_bk[index_links], act_play_music,this.act_cancel_play_music);
            Carrot_Window_Loading loading = this.carrot.get_loading_cur();
            loading.set_act_cancel_session(this.act_cancel_play_music);
        }

        private void act_play_music(UnityWebRequest unityWebRequest)
        {
            this.item_bk_music_play.set_icon(this.icon_pause_music_game);
            this.sound_bk_game_test.clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);
            this.data_music_temp = unityWebRequest.downloadHandler.data;
            this.sound_bk_game_test.Play();
        }

        private void act_buy_music_bk(string id_bk_music,int index_link)
        {
            this.id_buy_bk_music_temp = id_bk_music;
            this.index_buy_music_link_temp = index_link;
            this.carrot.buy_product(this.carrot.index_inapp_buy_bk_music);
        }

        private void act_watch_ads_to_music_bk(string id_bk_music, int index_link)
        {
            this.id_buy_bk_music_temp = id_bk_music;
            this.index_buy_music_link_temp = index_link;
            this.carrot.ads.show_ads_Rewarded();
        }

        private void act_cancel_play_music()
        {
            this.item_bk_music_play.set_icon(this.icon_list_music_game);
        }

        private void act_change_bk_music_game(string id_change_bk_music,int index_link_bk_music)
        {
            if (id_change_bk_music != this.id_select_play_bk_music)
            {
                this.carrot.get_mp3(this.list_link_music_bk[index_link_bk_music], download_and_set_bk_music);
            }
            else
            {
                this.carrot.get_tool().save_file("music_bk", this.data_music_temp);
                this.load_bk_music(this.sound_bk_game);
                if (this.box_setting_item_bkmusic != null) this.box_setting_item_bkmusic.set_change_status(true);
                if (this.box_list != null) this.box_list.close();
            }
        }

        private void download_and_set_bk_music(UnityWebRequest unityWebRequest)
        {
            this.carrot.get_tool().save_file("music_bk", unityWebRequest.downloadHandler.data);
            this.load_bk_music(this.sound_bk_game);
            if (this.box_setting_item_bkmusic != null) this.box_setting_item_bkmusic.set_change_status(true);
            if (this.box_list != null) this.box_list.close();
        }


        public void load_bk_music(AudioSource audio_bk_music)
        {
            this.sound_bk_game = audio_bk_music;
            if (this.carrot.get_tool().check_file_exist("music_bk"))
            {
                if (Application.isEditor)
                    StartCoroutine(this.downloadAudio("file://" + Application.dataPath + "/music_bk"));
                else
                    StartCoroutine(this.downloadAudio("file://" + Application.persistentDataPath + "/music_bk"));
            }
            else
            {
                if (audio_bk_music.clip != null)
                {
                    if(this.carrot.get_status_sound())
                        this.sound_bk_game.Play();
                    else
                        this.sound_bk_game.Stop();
                }
            }
        }

        public void delete_bk_music()
        {
            if (this.carrot.get_tool().check_file_exist("music_bk")) this.carrot.get_tool().delete_file("music_bk");
            if (this.sound_bk_game != null) this.sound_bk_game.Stop();
        }

        IEnumerator downloadAudio(string url_audio)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url_audio, AudioType.MPEG))
            {
                www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return null;
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    if (this.sound_bk_game != null)
                    {
                        this.sound_bk_game.clip = DownloadHandlerAudioClip.GetContent(www);
                        this.sound_bk_game.Play();
                    }
                }
            }
        }

        public void check_buy_music_item_bk(string id_product_success)
        {
            if (id_product_success == this.carrot.shop.get_id_by_index(this.carrot.index_inapp_buy_bk_music))
            {
                PlayerPrefs.SetInt("is_buy_bk_" +this.id_buy_bk_music_temp, 1);
                this.act_change_bk_music_game(this.id_buy_bk_music_temp,this.index_buy_music_link_temp);
                this.carrot.Show_msg(this.carrot.lang.Val("shop", "Shop"), this.carrot.lang.Val("buy_bk_success", "Buy background music successfully!"), Msg_Icon.Success);
                this.id_buy_bk_music_temp = "";
            }
        }

        public void OnRewardedSuccess()
        {
            if (this.id_buy_bk_music_temp != "")
            {
                if (this.id_buy_bk_music_temp != null)
                {
                    Debug.Log("buy music:" + this.id_buy_bk_music_temp.ToString());
                    this.act_change_bk_music_game(this.id_buy_bk_music_temp, this.index_buy_music_link_temp);
                    this.carrot.Show_msg(this.carrot.lang.Val("shop", "Shop"), "You have received the background music reward!", Msg_Icon.Success);
                    this.id_buy_bk_music_temp = "";
                }

            } 
        }

        public AudioSource get_audio_source_bk()
        {
            return this.sound_bk_game;
        }
        #endregion

        #region GamePad
        private void check_connect_gamepad()
        {
            StartCoroutine(try_check_gamepad_connect());
        }

        private IEnumerator try_check_gamepad_connect()
        {
            yield return new WaitForSeconds(2.5f);
            string[] names_gamepad = Input.GetJoystickNames();
            if (names_gamepad.Length > 0)
            {
                for (int i = this.list_gamepad.Count-1; i >=0; i--)
                {
                    if (names_gamepad[i] != null)
                    {
                        this.list_gamepad[i].txt_gamepad_name.text = names_gamepad[i];
                        this.list_gamepad[i].set_joystick_enable_play(true);
                    }
                    else
                    {
                        this.list_gamepad[i].set_joystick_enable_play(false);
                    }
                }
                if (this.act_handle_detection != null) this.act_handle_detection(true);
            }
            else
            {
                if (this.act_handle_detection != null) this.act_handle_detection(false);
            }
            this.check_connect_gamepad();
        }

        private void reset_btn_console()
        {
            if (this.list_obj_gamapad_console == null || this.is_user_gampad_for_console == false) return;
            for (int i = 0; i < this.list_obj_gamapad_console.Count; i++)
            {
                if(this.list_obj_gamapad_console[i].GetComponent<Button>())
                    this.list_obj_gamapad_console[i].GetComponent<Image>().color = this.list_obj_gamapad_console[i].GetComponent<Button>().colors.normalColor;
                else if (this.list_obj_gamapad_console[i].GetComponent<InputField>())
                    this.list_obj_gamapad_console[i].GetComponent<Image>().color = this.list_obj_gamapad_console[i].GetComponent<InputField>().colors.normalColor;
                else
                    this.list_obj_gamapad_console[i].GetComponent<Image>().color = this.color_nomal_item_btn_gamepad;
            }
        }

        public void set_index_button_gamepad_console(int index_sel)
        {
            this.reset_btn_console();
            this.index_gampad_item = index_sel;
            this.list_obj_gamapad_console[index_sel].GetComponent<Image>().color = this.color_sel_item_btn_gamepad;
            EventSystem.current.SetSelectedGameObject(this.list_obj_gamapad_console[index_sel]);
        }

        public void gamepad_keydown_up_console()
        {
            if (this.list_obj_gamapad_console == null || this.is_user_gampad_for_console == false) return;
            this.reset_btn_console();
            this.index_gampad_item--;
            if (this.index_gampad_item < 0) this.index_gampad_item = this.list_obj_gamapad_console.Count - 1;
            this.list_obj_gamapad_console[this.index_gampad_item].GetComponent<Image>().color = this.color_sel_item_btn_gamepad;
            EventSystem.current.SetSelectedGameObject(this.list_obj_gamapad_console[this.index_gampad_item].gameObject);
            if (this.scrollRect_gamepad_console != null) this.update_pos_scrollrect();
        }

        public void gamepad_keydown_down_console()
        {
            if (this.list_obj_gamapad_console == null || this.is_user_gampad_for_console == false) return;
            this.reset_btn_console();
            this.index_gampad_item++;
            if (this.index_gampad_item >= this.list_obj_gamapad_console.Count) this.index_gampad_item = 0;
            this.list_obj_gamapad_console[this.index_gampad_item].GetComponent<Image>().color = this.color_sel_item_btn_gamepad;
            EventSystem.current.SetSelectedGameObject(this.list_obj_gamapad_console[this.index_gampad_item].gameObject);
            if (this.scrollRect_gamepad_console != null) this.update_pos_scrollrect();
        }

        private void update_pos_scrollrect()
        {
            RectTransform objTransform = this.list_obj_gamapad_console[this.index_gampad_item].GetComponent<RectTransform>();
            RectTransform scrollTransform = this.scrollRect_gamepad_console.GetComponent<RectTransform>();
            ContentSizeFitter obj_body_size = this.scrollRect_gamepad_console.gameObject.GetComponentInChildren<ContentSizeFitter>();
            RectTransform obj_body = obj_body_size.GetComponent<RectTransform>();
            float normalizePosition = scrollTransform.anchorMin.y - objTransform.anchoredPosition.y;
            normalizePosition += (float)objTransform.transform.GetSiblingIndex() / (float)this.scrollRect_gamepad_console.content.transform.childCount;
            normalizePosition /= obj_body.sizeDelta.y- objTransform.sizeDelta.y;
            normalizePosition = Mathf.Clamp01(1 - normalizePosition);
            this.scrollRect_gamepad_console.verticalNormalizedPosition = normalizePosition;
        }

        public void gamepad_keydown_enter_console()
        {
            if (this.list_obj_gamapad_console == null||this.is_user_gampad_for_console==false) return;
            StartCoroutine(act_keydown_enter_console());
        }

        private IEnumerator act_keydown_enter_console()
        {
            yield return new WaitForSeconds(0.3f);
            if (this.list_obj_gamapad_console[this.index_gampad_item].GetComponent<Button>())
                this.list_obj_gamapad_console[this.index_gampad_item].GetComponent<Button>().onClick.Invoke();
            else
                this.list_obj_gamapad_console[this.index_gampad_item].GetComponent<InputField>().ForceLabelUpdate();
        }

        private void set_list_button_gamepad_console(List<GameObject> objs)
        {
            if (objs != null)
            {
                List<GameObject> list_obj = new List<GameObject>();
                for (int i = 0; i < objs.Count; i++) if (objs[i].activeInHierarchy) list_obj.Add(objs[i]);
                this.is_user_gampad_for_console = true;
                this.scrollRect_gamepad_console = null;
                this.list_obj_gamapad_console = list_obj;
                this.set_index_button_gamepad_console(0);
            }
        }

        public void set_list_button_gamepad_console(List<GameObject> objs, int fade_color_sel=100)
        {
            if (objs == null) return;
            this.color_sel_item_btn_gamepad = this.carrot.get_color_highlight_blur(fade_color_sel);
            this.set_list_button_gamepad_console(objs);
        }

        public void set_list_button_gamepad_console(List<GameObject> objs, ScrollRect sr)
        {
            if (objs == null) return;
            this.color_sel_item_btn_gamepad = this.carrot.get_color_highlight_blur(100);
            this.set_list_button_gamepad_console(objs);
            this.set_scrollRect_gamepad_consoles(sr);
        }

        public void set_scrollRect_gamepad_consoles(ScrollRect sr)
        {
            this.scrollRect_gamepad_console = sr;
        }

        public void clear_button_gamepad_console()
        {
            this.list_obj_gamapad_console = null;
            this.scrollRect_gamepad_console = null;
        }

        public void set_enable_gamepad_console(bool is_user_console)
        {
            this.is_user_gampad_for_console = is_user_console;
        }

        public void set_color_emp_gamepad_nomal(Color32 color_set)
        {
            this.color_nomal_item_btn_gamepad = color_set;
        }

        public void set_color_emp_gamepad_select(Color32 color_set)
        {
            this.color_sel_item_btn_gamepad = color_set;
        }

        public bool get_status_gamepad_console()
        {
            return this.is_user_gampad_for_console;
        }

        public Carrot_Gamepad create_gamepad(string s_id_gamepad)
        {
            GameObject obj_gamepad = Instantiate(this.obj_gamepad_prefab);
            obj_gamepad.transform.SetParent(GameObject.Find("Canvas").transform);
            obj_gamepad.transform.localPosition = obj_gamepad.transform.localPosition;
            obj_gamepad.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rectTransform = obj_gamepad.GetComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.offsetMin = new Vector2(0f, 0f);
            rectTransform.offsetMax = new Vector2(0f, 0f);
            obj_gamepad.GetComponent<Carrot_Gamepad>().set_carrot(this.carrot);
            obj_gamepad.GetComponent<Carrot_Gamepad>().load_gamepad_data();
            obj_gamepad.GetComponent<Carrot_Gamepad>().set_id_gamepad(s_id_gamepad);
            obj_gamepad.GetComponent<Carrot_Gamepad>().panel_gamepad.SetActive(false);
            this.list_gamepad.Add(obj_gamepad.GetComponent<Carrot_Gamepad>());
            return obj_gamepad.GetComponent<Carrot_Gamepad>();
        }

        public List<Carrot_Gamepad> get_list_gamepad()
        {
            return this.list_gamepad;
        }

        public void set_act_handle_detection(UnityAction<bool> act)
        {
            this.act_handle_detection = act;
        }

        public void Destroy_all_gamepad()
        {
            if (this.list_gamepad != null)
            {
                for (int i = 0; i < this.list_gamepad.Count; i++) Destroy(this.list_gamepad[i].gameObject);
            }
            this.list_gamepad = new();
        }
        #endregion

        #region Top_Player
        public void Add_type_rank(string s_name,Sprite icon=null)
        {
            carrot_game_rank_type type_rank = new()
            {
                s_name = s_name,
                icon = icon
            };
            this.list_rank_type.Add(type_rank);
        }

        public void Show_List_Top_player()
        {
            this.carrot.show_loading();
            StructuredQuery q = new StructuredQuery("app");
            q.Add_where("name_en",Query_OP.EQUAL,this.carrot.Carrotstore_AppId);
            q.Add_select("rank");
            q.Set_limit(1);
            this.carrot.server.Get_doc(q.ToJson(), Act_get_data_Top_player, Act_get_List_Top_player_fail);
        }

        private void Act_get_data_Top_player(string s_data)
        {
            this.s_data_offline_rank_player = s_data;
            PlayerPrefs.SetString("s_data_offline_rank_player", s_data);
            this.Act_get_List_Top_player_done(s_data);
        }

        private void Act_show_Top_player_by_type(int type)
        {
            this.rank_type_temp = type;
            this.Show_List_Top_player();
        }

        private void Act_get_List_Top_player_done(string s_data)
        {
            this.carrot.hide_loading();
            Fire_Collection fc = new(s_data);

            if (!fc.is_null)
            {
                IDictionary app = fc.fire_document[0].Get_IDictionary();
                if (app["rank"] != null)
                {
                    if (this.box_list != null) this.box_list.close();

                    IList rank = (IList)app["rank"];
                    box_list = this.carrot.Create_Box();
                    box_list.set_icon(this.icon_top_player);
                    box_list.set_title(this.carrot.lang.Val("top_player","Player rankings"));

                    string id_user_cur = this.carrot.user.get_id_user_login();

                    IList<IDictionary> list_rank = new List<IDictionary>();

                    if (this.list_rank_type.Count > 0)
                    {
                        Carrot_Box_Btn_Panel panel_type_rank= box_list.create_panel_btn();
                        for(int i = 0; i < this.list_rank_type.Count; i++)
                        {
                            var index = i;
                            Carrot_Button_Item btn_rank=panel_type_rank.create_btn("btn_rank_" + i);
                            btn_rank.set_icon_white(this.list_rank_type[i].icon);
                            btn_rank.set_label(this.list_rank_type[i].s_name);
                            btn_rank.set_label_color(Color.white);
                            btn_rank.set_act_click(() => this.Act_show_Top_player_by_type(index));
                            if (this.rank_type_temp==i) 
                                btn_rank.set_bk_color(this.carrot.color_highlight);
                            else
                                btn_rank.set_bk_color(Color.black);
                        }
                    }

                    for (int i = 0; i < rank.Count; i++)
                    {
                        list_rank.Add((IDictionary) rank[i]);
                    }

                    list_rank=this.SortListByScoresKey(list_rank);

                    for (int i = 0; i < list_rank.Count; i++)
                    {
                        IDictionary data_rank = list_rank[i];
                        IDictionary data_user = (IDictionary)data_rank["user"];

                        if (data_rank["type"].ToString()!=this.rank_type_temp.ToString()) continue;

                        var user_id = data_user["id"].ToString();
                        var user_lang = data_user["lang"].ToString();
                        GameObject obj_item_player_top = Instantiate(this.item_top_player_prefab);
                        obj_item_player_top.transform.SetParent(box_list.area_all_item);
                        obj_item_player_top.transform.localPosition = new Vector3(obj_item_player_top.transform.localPosition.x, obj_item_player_top.transform.localPosition.y, 0f);
                        obj_item_player_top.transform.localScale = new Vector3(1f, 1f, 1f);
                        obj_item_player_top.transform.localRotation = Quaternion.identity;
                        Carrot_Item_top_player top_player = obj_item_player_top.GetComponent<Carrot_Item_top_player>();
                        top_player.txt_user_name.text = data_user["name"].ToString();
                        top_player.txt_user_scores.text = data_rank["scores"].ToString();

                        if (i < this.icon_rank_player.Length)
                        {
                            top_player.img_rank.gameObject.SetActive(true);
                            top_player.img_rank.sprite = this.icon_rank_player[i];
                        }
                        else
                        {
                            top_player.img_rank.gameObject.SetActive(false);
                        }

                        if (id_user_cur == user_id)
                        {
                            this.index_edit_rank = i;
                            top_player.GetComponent<Image>().color = this.carrot.get_color_highlight_blur(100);
                        }
                        Sprite sp_avatar = this.carrot.get_tool().get_sprite_to_playerPrefs("avatar_user_" + user_id);
                        if (sp_avatar != null)
                        {
                            top_player.img_user.sprite = sp_avatar;
                            top_player.img_user.color = Color.white;
                        }
                        else
                            this.carrot.get_img_and_save_playerPrefs(data_user["avatar"].ToString(), top_player.img_user, "avatar_user_" + user_id);

                        top_player.set_act_click(() => this.carrot.user.show_user_by_id(user_id, user_lang));
                    }

                    if (this.carrot.type_app == TypeApp.Game)
                    {
                        box_list.update_gamepad_cosonle_control();
                    }
                }
                else
                {
                    this.carrot.Show_msg(this.carrot.lang.Val("top_player", "Player rankings"), carrot.lang.Val("top_player_none", "No player scores have been ranked yet, log in and play to add points to the rankings!"));
                }
            }
            else
            {
                this.carrot.Show_msg(this.carrot.lang.Val("top_player", "Player rankings"), carrot.lang.Val("top_player_none", "No player scores have been ranked yet, log in and play to add points to the rankings!"));
            }
        }

        IList<IDictionary> SortListByScoresKey(IList<IDictionary> list)
        {
            if(this.order_top_player==Carrot_game_rank_order.Ascending)
                return list.OrderBy(dict => int.Parse(dict["scores"].ToString())).ToList();
            else
                return list.OrderByDescending(dict => int.Parse(dict["scores"].ToString())).ToList();
        }

        private void Act_get_List_Top_player_fail(string s_error)
        {
            this.carrot.hide_loading();
            if (this.s_data_offline_rank_player != "") this.Act_get_List_Top_player_done(this.s_data_offline_rank_player);
        }

        public void Set_Order_By_Top_player(Carrot_game_rank_order order)
        {
            this.order_top_player = order;
        }

        [ContextMenu("Test update_scores_player")]
        public void Test_update_scores_player()
        {
            this.update_scores_player(UnityEngine.Random.Range(0, 20), 0);
        }

        public void update_scores_player(int scores,int type=0)
        {
            this.rank_type_temp = type;
            this.rank_scores_temp = scores;

            string user_id_login = this.carrot.user.get_id_user_login();
            if (user_id_login != "")
            {
                StructuredQuery q = new("app");
                q.Add_where("name_en",Query_OP.EQUAL,this.carrot.Carrotstore_AppId);
                q.Set_limit(1);
                q.Add_select("rank");
                this.carrot.server.Get_doc(q.ToJson(), Act_get_data_app_done, Act_update_scores_fail);
            }
        }

        private void Act_get_data_app_done(string s_data)
        {
            string user_id_login = this.carrot.user.get_id_user_login();

            Fire_Collection fc = new(s_data);

            if (fc.is_null) return;

            IDictionary app = fc.fire_document[0].Get_IDictionary();
            IList rank;
            this.index_edit_rank = -1;
            if (app["rank"]!=null)
            {
                rank = (IList)app["rank"];
                for (int i = 0; i < rank.Count; i++)
                {
                    IDictionary data_rank = (IDictionary)rank[i];
                    IDictionary data_user = (IDictionary)data_rank["user"];
                    if (data_user["id"].ToString() == user_id_login&& data_rank["type"].ToString()==this.rank_type_temp.ToString())
                    {
                        this.index_edit_rank = i;
                        break;
                    }
                }
            }
            else
            {
                rank = (IList)Json.Deserialize("[]");
            }

            Carrot_rank_data rank_item = new();
            rank_item.type = this.rank_type_temp.ToString();
            rank_item.scores = this.rank_scores_temp.ToString();
            rank_item.date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            Carrot_Rate_user_data user_login = new();
            user_login.name = this.carrot.user.get_data_user_login("name");
            user_login.id = this.carrot.user.get_id_user_login();
            user_login.lang = this.carrot.user.get_lang_user_login();
            user_login.avatar = this.carrot.user.get_data_user_login("avatar");
            rank_item.user = user_login;
            
            if (this.index_edit_rank == -1)
                rank.Add(rank_item);
            else
                rank[this.index_edit_rank] = rank_item;

            app["rank"] = rank;
            IDictionary app_data = (IDictionary)Json.Deserialize(JsonConvert.SerializeObject(app));
            string s_json = this.carrot.server.Convert_IDictionary_to_json(app_data);
            this.carrot.server.Update_Field_Document("app", this.carrot.Carrotstore_AppId,"rank", s_json);
        }


        private void Act_update_scores_fail(string s_error)
        {
            this.carrot.Show_msg(this.carrot.lang.Val("top_player", "Player rankings"), s_error, Msg_Icon.Error);
        }
        #endregion
    }
}
