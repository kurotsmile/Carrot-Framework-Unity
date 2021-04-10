using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Carrot
{
    public enum ModelApp { Publish, Develope }
    public enum Host { Carrotstore, Localhost }
    public enum LangApp { MultiLanguage, OneLanguge }
    public enum Msg_Icon { Alert, Error, Success };
    public enum OS { Android, Window, Ios, Samsung };

    public class Carrot : MonoBehaviour
    {

        public Canvas Canvas_carrot;
        [Header("Config App")]
        public Host host_app;
        public ModelApp model_app;
        public LangApp lang_app;
        public OS os_app;
        private string url;
        private string s_os;
        public string path_php_file_action;
        public string link_share_app;

        private string s_data_user_login;
        private string s_id_user_login;
        private string s_password_user_login;

        [Header("Panel Obj")]
        public GameObject panel_rate;
        public GameObject panel_box;
        public GameObject panel_msg;
        public GameObject panel_account_login;
        public GameObject panel_account_lost_password;
        public GameObject panel_exit;
        public GameObject panel_loading;
        public GameObject panel_share;

        [Header("Login Obj")]
        public Image img_btn_login;
        public InputField inp_login_username;
        public InputField inp_login_password;
        public InputField inp_lost_password;

        [Header("Icon sprite")]
        public Sprite icon_carrot;
        public Sprite icon_user_register;
        public Sprite icon_user_edit;
        public Sprite icon_user_info;
        public Sprite icon_user_logout;
        public Sprite icon_user_change_password;
        public Sprite icon_user_login_true;
        public Sprite icon_user_login_false;

        [Header("Box obj")]
        public Text txt_box_title;
        public Image img_box_icon;

        [Header("Msg Obj")]
        public Sprite icon_msg_alert;
        public Sprite icon_msg_error;
        public Sprite icon_msg_success;
        public Image icon_msg_img;
        public Text msg_txt;
        public Text msg_title;

        [Header("Box Obj")]
        public Transform area_body_box;
        public Transform area_body_list;
        public Transform area_body_grid;

        [Header("Prefab Obj")]
        public GameObject item_app_prfab;
        public GameObject item_user_info_prefab;
        public GameObject item_user_edit_prefab;

        [Header("Return Exit Obj")]
        public GameObject item_app_exit_prfab;
        public GameObject panel_exit_app_other;
        public Transform area_app_other_exit;

        [Header("Infor user")]
        public Color32 color_edit;
        public Color32 color_logout;
        public Color32 color_change_password;

        [Header("Share Obj")]
        public InputField inp_link_share;
        public void Load_Carrot()
        {
            this.Canvas_carrot.renderMode = RenderMode.ScreenSpaceOverlay;
            if (this.host_app == Host.Carrotstore) this.url = "https://carrotstore.com";
            if (this.host_app == Host.Localhost) this.url = "http://localhost";

            if (this.os_app == OS.Android) this.s_os = "android";
            if (this.os_app == OS.Window) this.s_os = "window";
            if (this.os_app == OS.Ios) this.s_os = "ios";
            if (this.os_app == OS.Samsung) this.s_os = "samsung";

            if (this.lang_app == LangApp.OneLanguge) Destroy(this.GetComponent<Carrot_lang>());
            this.s_data_user_login = PlayerPrefs.GetString("data_user_login");
            this.s_id_user_login= PlayerPrefs.GetString("id_user_login");
            this.s_password_user_login= PlayerPrefs.GetString("password_user_login");
            this.check_btn_login();
            this.hide_all_panel();
        }

        private void check_btn_login()
        {
            if (this.img_btn_login != null)
            {
                if (this.s_id_user_login != "")
                    this.img_btn_login.sprite = this.icon_user_login_true;
                else
                    this.img_btn_login.sprite = this.icon_user_login_false;
            }
        }

        private void hide_all_panel()
        {
            this.panel_account_login.SetActive(false);
            this.panel_account_lost_password.SetActive(false);
            this.panel_exit.SetActive(false);
            this.panel_box.SetActive(false);
            this.panel_msg.SetActive(false);
            this.panel_rate.SetActive(false);
            this.panel_loading.SetActive(false);
            this.panel_share.SetActive(false);
            this.Canvas_carrot.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (this.panel_exit.activeInHierarchy)
                {
                    this.app_exit();
                    return;
                }

                if (this.Canvas_carrot.isActiveAndEnabled)
                    this.close();
                else
                    this.show_exit();
            }
        }

        public void show_rate()
        {
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_rate.SetActive(true);
        }

        public void show_share()
        {
            this.inp_link_share.text = this.link_share_app;
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_share.SetActive(true);
        }

        public void app_rate()
        {
            Application.OpenURL("market://details?id=" + Application.identifier);
        }

        public void close()
        {
            this.hide_all_panel();
            //if(Random.Range(0,10)==1)this.show_rate();
        }

        public void show_login()
        {
            this.hide_all_panel();
            this.Canvas_carrot.gameObject.SetActive(true);
            if (this.s_data_user_login != "")
            {
                IDictionary data_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
                this.show_info_user_by_data(data_user);
            }
            else
            {
                this.panel_account_login.SetActive(true);
                this.inp_login_username.text = PlayerPrefs.GetString("login_username");
            }
        }

        public void show_msg(string s_msg)
        {
            this.icon_msg_img.sprite = this.icon_msg_alert;
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_msg.SetActive(true);
            this.msg_title.gameObject.SetActive(false);
            this.msg_txt.text = s_msg;
        }

        public void show_msg(string s_title, string s_msg)
        {
            this.show_msg(s_msg);
            this.msg_title.gameObject.SetActive(true);
            this.msg_title.text = s_title;
        }

        public void show_msg(string s_title, string s_msg, Msg_Icon icon)
        {
            this.show_msg(s_title,s_msg);
            if (icon == Msg_Icon.Error) this.icon_msg_img.sprite = this.icon_msg_error;
            if (icon == Msg_Icon.Success) this.icon_msg_img.sprite = this.icon_msg_success;
        }

        public void show_loading()
        {
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_loading.SetActive(true);
        }

        public void hide_loading()
        {
            if (this.panel_account_login.activeInHierarchy || this.panel_account_lost_password.activeInHierarchy || this.panel_box.activeInHierarchy)
                this.panel_loading.SetActive(false);
            else
                this.close();
        }

        public void hide_msg()
        {
            if (this.panel_account_login.activeInHierarchy || this.panel_account_lost_password.activeInHierarchy || this.panel_box.activeInHierarchy)
                this.panel_msg.SetActive(false);
            else
                this.close();
        }

        public void hide_share()
        {
            if (this.panel_account_login.activeInHierarchy || this.panel_account_lost_password.activeInHierarchy || this.panel_box.activeInHierarchy)
                this.panel_share.SetActive(false);
            else
                this.close();
        }

        public void show_lost_password()
        {
            this.hide_all_panel();
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_account_lost_password.SetActive(true);
        }

        public void show_list_box(string s_title, Sprite icon)
        {
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_box.SetActive(true);
            this.area_body_box = this.area_body_list;
            this.clear_contain(this.area_body_box);
            this.txt_box_title.text = s_title;
            this.img_box_icon.sprite = icon;
        }

        public void show_grid_box()
        {
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_box.SetActive(true);
            this.area_body_box = this.area_body_grid;
            this.clear_contain(this.area_body_box);
        }

        public void show_exit()
        {
            this.panel_exit_app_other.SetActive(false);
            this.Canvas_carrot.gameObject.SetActive(true);
            this.panel_exit.SetActive(true);
            StartCoroutine(this.act_get_list_app_carrot(false));
        }

        public void app_exit()
        {
            Debug.Log("Application Exit");
            Application.Quit();
        }

        public IEnumerator get_img_form_url(string s_url_img, Image img)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(s_url_img))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                    img.sprite = sprite;
                    img.color = Color.white;
                }
            }
        }

        public void show_list_carrot_app()
        {
            this.show_loading();
            StartCoroutine(this.act_get_list_app_carrot(true));
        }

        public WWWForm frm_act(string func)
        {
            WWWForm frm = new WWWForm();
            frm.AddField("function", func);
            frm.AddField("id_device", SystemInfo.deviceUniqueIdentifier);
            frm.AddField("os", this.s_os);
            return frm;
        }

        IEnumerator act_get_list_app_carrot(bool is_show_list)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), this.frm_act("list_app_carrot")))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    if (is_show_list)
                    {
                        this.hide_loading();
                        this.show_msg("Error", www.error, Msg_Icon.Error);
                    }
                }
                else
                {
                    if (model_app == ModelApp.Develope) Debug.Log("act_get_list_app_carrot:" + www.downloadHandler.text);

                    if (is_show_list)
                    {
                        this.hide_loading();
                        this.show_list_box("Applications from the developer", this.icon_carrot);
                    }
                    else
                    {
                        this.clear_contain(this.area_app_other_exit);
                        this.panel_exit_app_other.SetActive(true);
                    }

                    IList list_app_carrot = (IList)Json.Deserialize(www.downloadHandler.text);

                    for (int i = 0; i < list_app_carrot.Count; i++)
                    {
                        IDictionary item_data = (IDictionary)list_app_carrot[i];
                        GameObject item_p;
                        if (is_show_list)
                        {
                            item_p = Instantiate(this.item_app_prfab);
                            item_p.transform.SetParent(this.area_body_box);
                        }
                        else
                        {
                            item_p = Instantiate(this.item_app_exit_prfab);
                            item_p.transform.SetParent(this.area_app_other_exit);
                        }
                        item_p.transform.localScale = new Vector3(1f, 1f, 1f);
                        item_p.transform.localPosition = new Vector3(item_p.transform.localPosition.x, item_p.transform.localPosition.y, 0f);
                        item_p.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        item_p.GetComponent<Carrot_item_app>().link = item_data[this.s_os].ToString();
                        if (is_show_list) item_p.GetComponent<Carrot_item_app>().txt_name.text = item_data["name"].ToString();
                        StartCoroutine(get_img_form_url(item_data["icon"].ToString(), item_p.GetComponent<Carrot_item_app>().img_icon));
                    }

                }
            }
        }

        public string get_url()
        {
            return this.url + "/" + this.path_php_file_action;
        }

        public void clear_contain(Transform area_body)
        {
            foreach (Transform child in area_body)
            {
                Destroy(child.gameObject);
            }
        }

        public void show_user_register()
        {
            this.show_loading();
            StartCoroutine(act_show_register());
        }

        IEnumerator act_show_register()
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), this.frm_act("show_register")))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                }
                else
                {
                    if (model_app == ModelApp.Develope) Debug.Log("show_register:" + www.downloadHandler.text);
                    this.hide_loading();
                    this.panel_account_login.SetActive(false);
                    this.show_list_box("Sign up for an account", this.icon_user_register);
                    IDictionary obj_reg=(IDictionary)Json.Deserialize(www.downloadHandler.text);
                    IList list_info_reg = (IList)obj_reg["list_info"];
                    this.show_field_update_by_data(list_info_reg);
                    this.add_field_user_func(this.icon_msg_success, "Done", "After filling out the above information or click this button to complete", this.color_change_password, "register_user");
                }
            }
        }

        public void user_login()
        {
            this.show_loading();
            StartCoroutine(act_login_user());
        }

        IEnumerator act_login_user()
        {
            WWWForm frm = this.frm_act("login");
            frm.AddField("login_username", this.inp_login_username.text);
            frm.AddField("login_password", this.inp_login_password.text);
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                }
                else
                {
                    if (model_app == ModelApp.Develope) Debug.Log("login:" + www.downloadHandler.text);
                    IDictionary login_obj = (IDictionary)Json.Deserialize(www.downloadHandler.text);
                    this.hide_loading();
                    
                    if (login_obj["error"].ToString() == "1")
                    {
                        this.show_msg("Login", login_obj["msg"].ToString(), Msg_Icon.Error);
                    }
                    else
                    {
                        this.set_data_user_login(www.downloadHandler.text,login_obj["user_id"].ToString(), login_obj["user_password"].ToString());
                        PlayerPrefs.SetString("login_username", this.inp_login_username.text);

                        this.check_btn_login();
                        this.show_info_user_by_data(login_obj);
                    }
                }
            }
        }

        private void set_data_user_login(string s_data_user,string s_id_user,string s_password_user)
        {
            this.s_data_user_login = s_data_user;
            this.s_id_user_login = s_id_user;
            this.s_password_user_login = s_password_user;
            PlayerPrefs.SetString("data_user_login", this.s_data_user_login);
            PlayerPrefs.SetString("id_user_login", this.s_id_user_login);
            PlayerPrefs.SetString("password_user_login", this.s_password_user_login);
        }

        private void delete_data_user_login()
        {
            this.s_data_user_login = "";
            this.s_id_user_login = "";
            this.s_password_user_login = "";
            PlayerPrefs.DeleteKey("data_user_login");
            PlayerPrefs.DeleteKey("id_user_login");
            PlayerPrefs.DeleteKey("password_user_login");
        }

        private void show_info_user_by_data(IDictionary data_user_obj)
        {
            IList list_info_user = (IList)data_user_obj["list_info"];
            this.show_list_box("Account Information", this.icon_user_info);
            for (int i = 0; i < list_info_user.Count; i++)
            {
                IDictionary data_user = (IDictionary)list_info_user[i];
                if (data_user["val"].ToString() != "")
                {
                    GameObject item_user_info = Instantiate(this.item_user_info_prefab);
                    item_user_info.transform.SetParent(this.area_body_box);
                    item_user_info.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_user_info.transform.localPosition = new Vector3(item_user_info.transform.localPosition.x, item_user_info.transform.localPosition.y, 0f);
                    item_user_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    item_user_info.GetComponent<Carrot_item_user_info>().txt_title.text = data_user["title"].ToString();
                    item_user_info.GetComponent<Carrot_item_user_info>().txt_value.text = data_user["val"].ToString();
                    if (data_user["act"] != null) item_user_info.GetComponent<Carrot_item_user_info>().act = data_user["act"].ToString();
                }
            }

            if (s_id_user_login == data_user_obj["user_id"].ToString())
            {
                this.add_field_user_func(this.icon_user_edit, "Update info", "Click this button to update your account information", this.color_edit, "edit");
                this.add_field_user_func(this.icon_user_change_password, "Change Password", "Click here to change the login password", this.color_change_password, "show_change_password");
                this.add_field_user_func(this.icon_user_logout, "Logout", "Click here to sign out of your account", this.color_logout, "logout");
            }
        }

        private void show_field_update_by_data(IList list_field)
        {
            for (int i = 0; i < list_field.Count; i++)
            {
                IDictionary data_edit = (IDictionary)list_field[i];
                if (data_edit["id_name"] != null)
                {
                    this.add_field_user_edit(data_edit["id_name"].ToString(), data_edit["title"].ToString(), data_edit["type_update"].ToString(), Json.Serialize(data_edit["val_update"]), data_edit["val"].ToString());
                }
            }
        }

        public void add_field_user_func(Sprite icon,string s_name,string s_tip,Color32 color_func,string act)
        {
            GameObject item_user_info = Instantiate(this.item_user_info_prefab);
            item_user_info.transform.SetParent(this.area_body_box);
            item_user_info.transform.localScale = new Vector3(1f, 1f, 1f);
            item_user_info.transform.localPosition = new Vector3(item_user_info.transform.localPosition.x, item_user_info.transform.localPosition.y, 0f);
            item_user_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_user_info.GetComponent<Carrot_item_user_info>().img_icon.sprite = icon;
            item_user_info.GetComponent<Carrot_item_user_info>().txt_title.text = s_tip;
            item_user_info.GetComponent<Carrot_item_user_info>().txt_value.text = s_name;
            item_user_info.GetComponent<Carrot_item_user_info>().act = act;
            item_user_info.GetComponent<Image>().color = color_func;
        }

        private void add_field_user_edit(string s_id_name,string s_title, string s_type, string s_type_update,string s_val)
        {
            GameObject item_edit = Instantiate(this.item_user_edit_prefab);
            item_edit.name = "field_update";
            item_edit.transform.SetParent(this.area_body_box);
            item_edit.transform.localScale = new Vector3(1f, 1f, 1f);
            item_edit.transform.localPosition = new Vector3(item_edit.transform.localPosition.x, item_edit.transform.localPosition.y, 0f);
            item_edit.transform.localRotation = Quaternion.Euler(Vector3.zero);

            item_edit.GetComponent<Carrot_item_user_edit>().s_name = s_id_name;
            item_edit.GetComponent<Carrot_item_user_edit>().txt_title.text = s_title;
            item_edit.GetComponent<Carrot_item_user_edit>().set_data(s_type, s_type_update, s_val);

        }

        public void act_btn_filed_info_user(string s_act)
        {
            if (s_act == "logout")
            {
                this.delete_data_user_login();
                this.show_login();
            }

            if (s_act == "register_user") this.register_user();
            if (s_act == "edit") this.show_update_info_user();
            if (s_act == "update_user") this.update_user();
            if (s_act == "show_change_password") this.show_change_password();
            if (s_act == "done_change_password") this.done_change_password();
        }

        private void done_change_password()
        {
            WWWForm frm_change_password = this.frm_act("change_password");
            frm_change_password.AddField("user_id", s_id_user_login);
            foreach (Transform child_field in this.area_body_box)
            {
                if (child_field.gameObject.name == "field_update")
                {
                    Carrot_item_user_edit item_data_edit = child_field.GetComponent<Carrot_item_user_edit>();
                    frm_change_password.AddField(item_data_edit.s_name, item_data_edit.get_val());
                }
            }
            StartCoroutine(act_done_change_password(frm_change_password));
        }

        IEnumerator act_done_change_password(WWWForm frm_update)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm_update))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                }
                else
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) Debug.Log("change_password:" + www.downloadHandler.text);
                    IDictionary obj_user = (IDictionary)Json.Deserialize(www.downloadHandler.text);
                    if (obj_user["error"].ToString() == "0")
                    {
                        this.panel_box.SetActive(false);
                        this.show_msg("Change Password", obj_user["msg"].ToString(), Msg_Icon.Success);
                        this.delete_data_user_login();
                    }
                    else
                    {
                        this.show_msg("Change Password", obj_user["msg"].ToString(), Msg_Icon.Error);
                    }
                }
            }
        }

        private void show_change_password()
        {
            this.show_list_box("Change password", this.icon_user_change_password);
            if(this.s_password_user_login!="")this.add_field_user_edit("password", "Enter your current password", "3", "", "");
            this.add_field_user_edit("password_new", "Enter your new password", "3", "", "");
            this.add_field_user_edit("password_re_new", "Re-enter your new password", "3", "", "");
            this.add_field_user_func(this.icon_msg_success, "Done", "Click here to complete the password change", this.color_change_password, "done_change_password");
        }

        private void register_user()
        {
            this.show_loading();
            WWWForm frm = this.frm_act("register");
            foreach (Transform child_field in this.area_body_box)
            {
                if (child_field.gameObject.name == "field_update")
                {
                    Carrot_item_user_edit item_data_edit = child_field.GetComponent<Carrot_item_user_edit>();
                    frm.AddField(item_data_edit.s_name, item_data_edit.get_val());
                }
            }
            StartCoroutine(act_register_account(frm));
        }

        IEnumerator act_register_account(WWWForm frm_update)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm_update))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                }
                else
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) Debug.Log("update_account:" + www.downloadHandler.text);
                    IDictionary obj_user = (IDictionary)Json.Deserialize(www.downloadHandler.text);
                    if (obj_user["error"].ToString() == "0")
                    {
                        this.panel_box.SetActive(false);
                        this.show_msg("Register Account", obj_user["msg"].ToString(), Msg_Icon.Success);
                    }
                    else
                    {
                        this.show_msg("Register Account", obj_user["msg"].ToString(), Msg_Icon.Error);
                    }
                }
            }
        }

        private void update_user()
        {
            this.show_loading();
            WWWForm frm = this.frm_act("update_account");
            frm.AddField("user_id", s_id_user_login);
            foreach (Transform child_field in this.area_body_box)
            {
                if (child_field.gameObject.name == "field_update")
                {
                    Carrot_item_user_edit item_data_edit = child_field.GetComponent<Carrot_item_user_edit>();
                    frm.AddField(item_data_edit.s_name, item_data_edit.get_val());
                }
            }
            StartCoroutine(act_update_account(frm));
        }

        IEnumerator act_update_account(WWWForm frm_update)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm_update))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                }
                else
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) Debug.Log("update_account:" + www.downloadHandler.text);
                    IDictionary obj_user = (IDictionary)Json.Deserialize(www.downloadHandler.text);
                    if (obj_user["error"].ToString() == "0")
                    {
                        this.panel_box.SetActive(false);
                        this.show_msg("Update Account", obj_user["msg"].ToString(), Msg_Icon.Success);
                        this.set_data_user_login(www.downloadHandler.text, obj_user["user_id"].ToString(), obj_user["user_password"].ToString());
                    }
                    else
                    {
                        this.show_msg("Update Account", obj_user["msg"].ToString(), Msg_Icon.Error);
                    }
                }
            }
        }

        public void show_update_info_user()
        {
            this.show_list_box("Update Account", this.icon_user_edit);
            IDictionary obj_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            IList list_field = (IList)obj_user["list_info"];
            this.add_field_user_edit("avatar", "Avatar", "6", "", "");
            this.show_field_update_by_data(list_field);
            this.add_field_user_func(this.icon_msg_success, "Update", "Click here to complete the update", this.color_change_password, "update_user");
        }

        public void show_user_by_id(string s_id_user,string s_lang_user)
        {
            this.show_loading();
            StartCoroutine(act_get_user_by_id(s_id_user, s_lang_user));
        }

        IEnumerator act_get_user_by_id(string s_user_id,string s_user_lang)
        {
            WWWForm frm = this.frm_act("get_user_by_id");
            frm.AddField("user_id", s_user_id);
            frm.AddField("user_lang", s_user_lang);
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                }
                else
                {
                    if (model_app == ModelApp.Develope) Debug.Log("get_user_by_id:" + www.downloadHandler.text);
                    IDictionary login_obj = (IDictionary)Json.Deserialize(www.downloadHandler.text);
                    this.hide_loading();
                    this.show_info_user_by_data(login_obj);
                }
            }
        }

        public void get_passowrd()
        {
            StartCoroutine(this.act_get_password());
        }

        IEnumerator act_get_password()
        {
            WWWForm frm = this.frm_act("get_password");
            frm.AddField("inp_info", this.inp_lost_password.text);
            using (UnityWebRequest www = UnityWebRequest.Post(this.get_url(), frm))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) this.show_msg("Error", www.error, Msg_Icon.Error);
                }
                else
                {
                    this.hide_loading();
                    if (model_app == ModelApp.Develope) Debug.Log("get_password:" + www.downloadHandler.text);
                    this.show_msg(www.downloadHandler.text);
                }
            }
        }

        public string get_id_user_login()
        {
            return this.s_id_user_login;
        }

        public void share_send_mail()
        {
            string email = "mailyou@example.com";
            string subject = "Carrotstore";
            string body = "Get nows:"+this.link_share_app;
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        public void share_send_sms()
        {
            Application.OpenURL("sms:0?body=" + this.link_share_app);
        }

        public void share_send_telegram()
        {
            if (os_app == OS.Window)
                Application.OpenURL("https://t.me/share/url?url=" + this.link_share_app);
            else
                Application.OpenURL("tg://msg?text=" + this.link_share_app);
        }

        public void share_send_facebook()
        {
            if (os_app == OS.Window)
                Application.OpenURL("https://www.facebook.com/sharer/sharer.php?u="+this.link_share_app);
            else
                Application.OpenURL("fb://publish/?text="+this.link_share_app);
        }

        public void share_send_messenger()
        {
            Application.OpenURL("fb-messenger://share?link=" + this.link_share_app);
        }

        public void share_send_twitter()
        {
            if (os_app == OS.Window)
                Application.OpenURL("https://twitter.com/intent/tweet?url=" + this.link_share_app);
            else
                Application.OpenURL("twitter://post?message=" + this.link_share_app);
        }
    }
}