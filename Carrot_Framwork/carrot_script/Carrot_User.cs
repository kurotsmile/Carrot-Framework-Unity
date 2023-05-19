using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_User : MonoBehaviour
    {
        private Carrot carrot;
        private Carrot_Box box_info_user;
        private On_event_change event_after_login_user;

        private string s_data_user_login;
        private string s_id_user_login;
        private string s_password_user_login;

        public Sprite icon_user_login_true;
        public Sprite icon_user_login_false;

        public GameObject window_login_prefab;
        public GameObject window_lost_password_prefab;

        public UnityAction<string> act_after_show_view_by_id;
        public UnityAction<string> event_customer_user_field;

        [Header("Icon Emp user")]
        public Sprite icon_user_register;
        public Sprite icon_user_edit;
        public Sprite icon_user_info;
        public Sprite icon_user_logout;
        public Sprite icon_user_done;
        public Sprite icon_user_change_password;

        [Header("Prefab Obj")]
        public GameObject item_user_info_prefab;
        public GameObject item_user_edit_prefab;

        [Header("Infor user")]
        public Color32 color_edit;
        public Color32 color_logout;
        public Color32 color_change_password;
        private byte[] data_avatar_user;
        private bool is_change_avatar = false;
        public List<GameObject> list_field_user_login;
        public List<GameObject> list_field_user_info;
        public GameObject box_info_item_prefab;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.s_data_user_login = PlayerPrefs.GetString("data_user_login");
            this.s_id_user_login = PlayerPrefs.GetString("id_user_login");
            this.s_password_user_login = PlayerPrefs.GetString("password_user_login");
            this.check_btn_login();
            this.list_field_user_info = new List<GameObject>();
            this.list_field_user_login = new List<GameObject>();
        }

        public void show_login(UnityAction act_afte_login=null)
        {
            this.carrot.play_sound_click();
            if (this.s_data_user_login != "")
                this.show_user_cur_info();
            else
                this.carrot.user.show_window_login(act_afte_login);  
        }

        private void show_user_cur_info()
        {
            IDictionary data_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            this.show_info_user_by_data(data_user);
        }

        private void show_window_login(UnityAction act_login_success)
        {
            GameObject window_login=this.carrot.create_window(this.window_login_prefab);
            window_login.GetComponent<Carrot_Window_User_Login>().load(this.carrot);
            window_login.GetComponent<Carrot_Window_User_Login>().act_after_login_success = act_login_success;
            window_login.GetComponent<Carrot_lang_show>().load_lang_emp(this.carrot.lang.get_sp_lang_cur());
            if (this.carrot.type_control!=TypeControl.None) this.carrot.game.set_list_button_gamepad_console(window_login.GetComponent<Carrot_UI>().get_list_btn());
        }

        public void show_window_lost_password()
        {
            GameObject window_lost_password = this.carrot.create_window(this.window_lost_password_prefab);
            window_lost_password.GetComponent<Carrot_Window_User_Lost_Password>().load(this.carrot);
            window_lost_password.GetComponent<Carrot_lang_show>().load_lang_emp();
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(window_lost_password.GetComponent<Carrot_UI>().get_list_btn());
        }

        public void set_data_user_login(IDictionary data_user)
        {
            this.s_data_user_login = Json.Serialize(data_user);
            this.s_id_user_login = data_user["user_id"].ToString();
            this.s_password_user_login = data_user["user_password"].ToString();
            PlayerPrefs.SetString("data_user_login", this.s_data_user_login);
            PlayerPrefs.SetString("id_user_login", this.s_id_user_login);
            PlayerPrefs.SetString("password_user_login", this.s_password_user_login);
            if (this.carrot.img_btn_login != null) this.carrot.get_img_and_save_playerPrefs(data_user["avatar"].ToString(), this.carrot.img_btn_login, "carrot_user_avatar");
            if (this.event_after_login_user != null) this.event_after_login_user.Invoke();
        }

        public void delete_data_user_login()
        {
            this.s_data_user_login = "";
            this.s_id_user_login = "";
            this.s_password_user_login = "";
            PlayerPrefs.DeleteKey("data_user_login");
            PlayerPrefs.DeleteKey("id_user_login");
            PlayerPrefs.DeleteKey("password_user_login");
            PlayerPrefs.DeleteKey("carrot_user_avatar");
            this.check_btn_login();
        }

        public void check_btn_login()
        {
            if (this.carrot.img_btn_login != null) this.load_avatar_user_login(this.carrot.img_btn_login);
        }

        public void load_avatar_user_login(Image img_emp_user)
        {
            if (this.s_id_user_login != "")
            {
                Sprite sp_avatar_user = this.carrot.get_tool().get_sprite_to_playerPrefs("carrot_user_avatar");
                if (sp_avatar_user != null)
                {
                    img_emp_user.sprite = sp_avatar_user;
                    img_emp_user.color = Color.white;
                }
                else
                {
                    img_emp_user.sprite = this.icon_user_login_true;
                    img_emp_user.color = this.carrot.color_highlight;
                }
            }
            else
            {
                img_emp_user.sprite = this.icon_user_login_false;
                img_emp_user.color = this.carrot.color_highlight;
            }
        }

        public void show_info_user_by_data(IDictionary data_user_obj)
        {
            IList list_info_user = (IList)data_user_obj["list_info"];
            this.box_info_user = this.carrot.Create_Box(PlayerPrefs.GetString("acc_info", "Account Information"), this.icon_user_info);
            this.box_info_user.name = "Window Account Information";
            for (int i = 0; i < list_info_user.Count; i++)
            {
                IDictionary data_user = (IDictionary)list_info_user[i];
                if (data_user["val"].ToString() != "")
                {
                    GameObject item_user_info = Instantiate(this.item_user_info_prefab);
                    item_user_info.transform.SetParent(box_info_user.area_all_item);
                    item_user_info.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_user_info.transform.localPosition = new Vector3(item_user_info.transform.localPosition.x, item_user_info.transform.localPosition.y, 0f);
                    item_user_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    item_user_info.GetComponent<Carrot_item_user_info>().txt_title.text = PlayerPrefs.GetString(data_user["title"].ToString(), data_user["title_en"].ToString());

                    if (data_user["val_update"] != null)
                        item_user_info.GetComponent<Carrot_item_user_info>().set_val_en(Json.Serialize(data_user["val_update_en"]), data_user["val"].ToString());
                    else
                        item_user_info.GetComponent<Carrot_item_user_info>().txt_value.text = data_user["val"].ToString();

                    if (data_user["act"] != null) item_user_info.GetComponent<Carrot_item_user_info>().act = data_user["act"].ToString();
                    if (data_user["type_update"] != null) item_user_info.GetComponent<Carrot_item_user_info>().type = data_user["type_update"].ToString();
                    if (data_user["icon"] != null)
                    {
                        if (PlayerPrefs.GetString(data_user["icon"].ToString(), "") != "")
                        {
                            item_user_info.GetComponent<Carrot_item_user_info>().img_icon.sprite = this.carrot.get_tool().get_sprite_to_playerPrefs(data_user["icon"].ToString());
                            item_user_info.GetComponent<Carrot_item_user_info>().img_icon.color = Color.white;
                        }
                        else
                            this.carrot.get_img_and_save_playerPrefs(data_user["icon"].ToString(), item_user_info.GetComponent<Carrot_item_user_info>().img_icon, data_user["icon"].ToString());
                    }
                }
            }

            for (int i = 0; i < this.list_field_user_info.Count; i++) box_info_user.add_item(this.list_field_user_info[i]).GetComponent<Carrot_Box_Item>().set_act(this.list_field_user_info[i].GetComponent<Carrot_Box_Item>().get_act_click());

            if (s_id_user_login == data_user_obj["user_id"].ToString())
            {
                for (int i = 0; i < this.list_field_user_login.Count; i++) box_info_user.add_item(this.list_field_user_login[i]).GetComponent<Carrot_Box_Item>().set_act(this.list_field_user_login[i].GetComponent<Carrot_Box_Item>().get_act_click());

                this.add_field_user_func(this.icon_user_edit, PlayerPrefs.GetString("acc_edit", "Update info"), PlayerPrefs.GetString("acc_edit_tip", "Click this button to update your account information"), this.color_edit, "edit");
                this.add_field_user_func(this.icon_user_change_password, PlayerPrefs.GetString("acc_change_pass", "Change Password"), PlayerPrefs.GetString("acc_change_pass_tip", "Click here to change the login password"), this.color_change_password, "show_change_password");
                this.add_field_user_func(this.icon_user_logout, PlayerPrefs.GetString("logout", "Logout"), PlayerPrefs.GetString("logout_tip", "Click here to sign out of your account"), this.color_logout, "logout");
            }

            if (data_user_obj["avatar"].ToString() != "") this.carrot.get_img(data_user_obj["avatar"].ToString(), box_info_user.img_icon);

            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(box_info_user.GetComponent<Carrot_UI>().get_list_btn());
                this.carrot.game.set_scrollRect_gamepad_consoles(box_info_user.GetComponent<Carrot_UI>().scrollRect);
            }
        }

        public void add_field_user_func(Sprite icon, string s_name, string s_tip, Color32 color_func, string act)
        {
            GameObject item_user_info = Instantiate(this.item_user_info_prefab);
            item_user_info.transform.SetParent(this.box_info_user.area_all_item);
            item_user_info.transform.localScale = new Vector3(1f, 1f, 1f);
            item_user_info.transform.localPosition = new Vector3(item_user_info.transform.localPosition.x, item_user_info.transform.localPosition.y, 0f);
            item_user_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_user_info.GetComponent<Carrot_item_user_info>().img_icon.sprite = icon;
            item_user_info.GetComponent<Carrot_item_user_info>().txt_title.text = s_tip;
            item_user_info.GetComponent<Carrot_item_user_info>().txt_value.text = s_name;
            item_user_info.GetComponent<Carrot_item_user_info>().act = act;
            item_user_info.GetComponent<Image>().color = color_func;
            Button b = item_user_info.GetComponent<Button>();
            ColorBlock cb = b.colors;
            cb.normalColor = color_func;
            b.colors = cb;
            item_user_info.GetComponent<Button>().colors = cb;
        }

        private void add_field_user_edit(string s_id_name, string s_title, string s_type, string s_val)
        {
            GameObject item_edit = this.box_info_user.add_item(this.item_user_edit_prefab);
            item_edit.name = "field_update";
            item_edit.transform.localScale = new Vector3(1f, 1f, 1f);
            item_edit.transform.localPosition = new Vector3(item_edit.transform.localPosition.x, item_edit.transform.localPosition.y, 0f);
            item_edit.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_edit.GetComponent<Carrot_item_user_edit>().s_name = s_id_name;
            item_edit.GetComponent<Carrot_item_user_edit>().txt_title.text = s_title;
            item_edit.GetComponent<Carrot_item_user_edit>().set_data(s_type, s_val);
            if (s_id_name == "avatar")
            {
                Sprite avatar_user = this.carrot.get_tool().get_sprite_to_playerPrefs("carrot_user_avatar");
                if (avatar_user != null)
                {
                    item_edit.GetComponent<Carrot_item_user_edit>().img_avatar.sprite = avatar_user;
                    item_edit.GetComponent<Carrot_item_user_edit>().img_avatar.color = Color.white;
                }
                else
                    item_edit.GetComponent<Carrot_item_user_edit>().img_avatar.sprite = this.icon_user_login_true;
            }
        }

        private void add_field_user_edit_select_down(string s_id_name, string s_title, string s_val_update, string s_val_update_en, string s_val_sel)
        {
            GameObject item_edit = Instantiate(this.item_user_edit_prefab);
            item_edit.name = "field_update";
            item_edit.transform.SetParent(this.box_info_user.area_all_item);
            item_edit.transform.localScale = new Vector3(1f, 1f, 1f);
            item_edit.transform.localPosition = new Vector3(item_edit.transform.localPosition.x, item_edit.transform.localPosition.y, 0f);
            item_edit.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_edit.GetComponent<Carrot_item_user_edit>().s_name = s_id_name;
            item_edit.GetComponent<Carrot_item_user_edit>().txt_title.text = s_title;
            item_edit.GetComponent<Carrot_item_user_edit>().set_data_select_down(s_val_update, s_val_update_en, s_val_sel);
        }

        public void show_user_register() { this.carrot.send(this.carrot.frm_act("show_register"), act_show_user_register); }

        private void act_show_user_register(string data)
        {
            this.box_info_user=this.carrot.Create_Box(PlayerPrefs.GetString("register", "Sign up for an account"), this.icon_user_register);
            IDictionary obj_reg = (IDictionary)Json.Deserialize(data);
            IList list_info_reg = (IList)obj_reg["list_info"];
            this.show_field_update_by_data(list_info_reg, true);
            this.add_field_user_func(this.icon_user_done, PlayerPrefs.GetString("done", "Done"), PlayerPrefs.GetString("done_tip", "After filling out the above information or click this button to complete"), this.color_change_password, "register_user");
            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(box_info_user.GetComponent<Carrot_UI>().get_list_btn());
                this.carrot.game.set_scrollRect_gamepad_consoles(box_info_user.GetComponent<Carrot_UI>().scrollRect);
            }
        }

        private void show_field_update_by_data(IList list_field, bool is_register)
        {
            for (int i = 0; i < list_field.Count; i++)
            {
                IDictionary data_edit = (IDictionary)list_field[i];
                if (data_edit["id_name"] != null)
                {
                    string s_id_field_name = data_edit["id_name"].ToString();
                    if (is_register)
                    {
                        if (data_edit["val_update_en"] == null)
                            this.add_field_user_edit(data_edit["id_name"].ToString(), PlayerPrefs.GetString(data_edit["title"].ToString(), data_edit["title_en"].ToString()), data_edit["type_update"].ToString(), data_edit["val"].ToString());
                        else
                            this.add_field_user_edit_select_down(data_edit["id_name"].ToString(), PlayerPrefs.GetString(data_edit["title"].ToString(), data_edit["title_en"].ToString()), Json.Serialize(data_edit["val_update"]), Json.Serialize(data_edit["val_update_en"]), data_edit["val"].ToString());
                    }
                    else
                    {
                        if (s_id_field_name == "name" || s_id_field_name == "sdt" || s_id_field_name == "address" || s_id_field_name == "email" || s_id_field_name == "sex" || s_id_field_name == "status")
                        {
                            if (data_edit["val_update_en"] == null)
                                this.add_field_user_edit(data_edit["id_name"].ToString(), PlayerPrefs.GetString(data_edit["title"].ToString(), data_edit["title_en"].ToString()), data_edit["type_update"].ToString(), data_edit["val"].ToString());
                            else
                                this.add_field_user_edit_select_down(data_edit["id_name"].ToString(), PlayerPrefs.GetString(data_edit["title"].ToString(), data_edit["title_en"].ToString()), Json.Serialize(data_edit["val_update"]), Json.Serialize(data_edit["val_update_en"]), data_edit["val"].ToString());
                        }
                    }
                }
            }
        }

        public void act_btn_filed_info_user(string s_act)
        {
            if (s_act == "logout")
            {
                this.box_info_user.close();
                this.delete_data_user_login();
                this.carrot.show_login();
            }
            if (s_act == "register_user") this.register_user();
            if (s_act == "edit")
            {
                this.box_info_user.close();
                this.show_update_info_user();
            }
            if (s_act == "update_user") this.update_user();
            if (s_act == "show_change_password")
            {
                this.box_info_user.close();
                this.show_change_password();
            }
            if (s_act == "done_change_password") this.done_change_password();
            if (s_act == "field_customer") this.event_customer_user_field.Invoke(s_act);
        }

        private void done_change_password()
        {
            WWWForm frm_change_password = this.carrot.frm_act("change_password");
            frm_change_password.AddField("user_id", s_id_user_login);
            frm_change_password.AddField("user_lang", this.get_lang_user_login());
            foreach (Transform child_field in this.box_info_user.area_all_item)
            {
                if (child_field.gameObject.name == "field_update")
                {
                    Carrot_item_user_edit item_data_edit = child_field.GetComponent<Carrot_item_user_edit>();
                    frm_change_password.AddField(item_data_edit.s_name, item_data_edit.get_val());
                }
            }
            this.carrot.send(frm_change_password, act_change_password);
        }

        private void act_change_password(string data)
        {
            IDictionary obj_user = (IDictionary)Json.Deserialize(data);
            if (obj_user["error"].ToString() == "0")
            {
                this.box_info_user.close();
                this.carrot.show_msg(PlayerPrefs.GetString("acc_change_pass", "Change Password"), PlayerPrefs.GetString(obj_user["msg"].ToString(), obj_user["msg_en"].ToString()), Msg_Icon.Success);
                this.delete_data_user_login();
            }
            else
            {
                this.carrot.show_msg(PlayerPrefs.GetString("acc_change_pass", "Change Password"), PlayerPrefs.GetString(obj_user["msg"].ToString(), obj_user["msg_en"].ToString()), Msg_Icon.Error);
            }
        }

        private void show_change_password()
        {
            this.box_info_user = this.carrot.Create_Box(PlayerPrefs.GetString("acc_change_pass", "Change password"), this.icon_user_change_password);
            if (this.s_password_user_login != "") this.add_field_user_edit("password", PlayerPrefs.GetString("inp_pass_current", "Enter your current password"), "3", "");
            this.add_field_user_edit("password_new", PlayerPrefs.GetString("inp_pass_new", "Enter your new password"), "3", "");
            this.add_field_user_edit("password_re_new", PlayerPrefs.GetString("inp_pass_rep_new", "Re -enter your new password"), "3", "");
            this.add_field_user_func(this.icon_user_done, PlayerPrefs.GetString("done", "Done"), PlayerPrefs.GetString("done_tip", "Click here to complete the password change"), this.color_change_password, "done_change_password");
            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(box_info_user.GetComponent<Carrot_UI>().get_list_btn());
                this.carrot.game.set_scrollRect_gamepad_consoles(box_info_user.GetComponent<Carrot_UI>().scrollRect);
            }
        }

        private void register_user()
        {
            WWWForm frm = this.carrot.frm_act("register");
            foreach (Transform child_field in this.box_info_user.area_all_item)
            {
                if (child_field.gameObject.name == "field_update")
                {
                    Carrot_item_user_edit item_data_edit = child_field.GetComponent<Carrot_item_user_edit>();
                    frm.AddField(item_data_edit.s_name, item_data_edit.get_val());
                }
            }
            this.carrot.send(frm, act_register_user);
        }

        private void act_register_user(string data)
        {
            IDictionary obj_user = (IDictionary)Json.Deserialize(data);
            if (obj_user["error"].ToString() == "0")
            {
                this.box_info_user.close();
                this.carrot.show_msg(PlayerPrefs.GetString("register", "Register Account"), PlayerPrefs.GetString(obj_user["msg"].ToString(), obj_user["msg_en"].ToString()), Msg_Icon.Success);
            }
            else
            {
                this.carrot.show_msg(PlayerPrefs.GetString("register", "Register Account"), PlayerPrefs.GetString(obj_user["msg"].ToString(), obj_user["msg_en"].ToString()), Msg_Icon.Error);
            }
        }

        private void update_user()
        {
            WWWForm frm = this.carrot.frm_act("update_account");
            frm.AddField("user_id", s_id_user_login);
            frm.AddField("user_lang", this.get_lang_user_login());
            foreach (Transform child_field in this.box_info_user.area_all_item)
            {
                if (child_field.gameObject.name == "field_update")
                {
                    Carrot_item_user_edit item_data_edit = child_field.GetComponent<Carrot_item_user_edit>();
                    if (item_data_edit.s_name == "avatar")
                    {
                        if (this.is_change_avatar)
                            frm.AddBinaryData("avatar", this.data_avatar_user);
                    }
                    else
                        frm.AddField(item_data_edit.s_name, item_data_edit.get_val());
                }
            }
            this.carrot.send(frm, done_update_user);
        }

        private void done_update_user(string data)
        {
            IDictionary obj_user = (IDictionary)Json.Deserialize(data);
            if (obj_user["error"].ToString() == "0")
            {
                this.box_info_user.close();
                Carrot_Window_Msg msg=this.carrot.show_msg(PlayerPrefs.GetString("acc_edit", "Update Account"), PlayerPrefs.GetString(obj_user["msg"].ToString(), obj_user["msg_en"].ToString()), Msg_Icon.Success);
                this.set_data_user_login(obj_user);
                msg.add_btn_msg(PlayerPrefs.GetString("acc_info", "Account Information"), this.show_user_cur_info);
                msg.add_btn_msg(PlayerPrefs.GetString("cancel", "Close"),msg.close);
                msg.update_btns_gamepad_console();
            }
            else
            {
                this.carrot.show_msg(PlayerPrefs.GetString("acc_edit", "Update Account"), PlayerPrefs.GetString(obj_user["msg"].ToString(), obj_user["msg_en"].ToString()), Msg_Icon.Error);
            }
        }

        public void show_update_info_user()
        {
            this.is_change_avatar = false;
            this.data_avatar_user = null;
            this.box_info_user = this.carrot.Create_Box(PlayerPrefs.GetString("acc_edit", "Update Account"), this.icon_user_edit);
            IDictionary obj_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            IList list_field = (IList)obj_user["list_info"];
            this.add_field_user_edit("avatar", PlayerPrefs.GetString("user_avatar", "Avatar"), "6", "");
            this.show_field_update_by_data(list_field, false);
            this.add_field_user_func(this.icon_user_done, PlayerPrefs.GetString("done", "Done"), PlayerPrefs.GetString("done_tip", "Click here to complete the update"), this.color_change_password, "update_user");

            if (this.carrot.type_control != TypeControl.None)
            {
                this.carrot.game.set_list_button_gamepad_console(box_info_user.GetComponent<Carrot_UI>().get_list_btn());
                this.carrot.game.set_scrollRect_gamepad_consoles(box_info_user.GetComponent<Carrot_UI>().scrollRect);
            }
        }

        public string get_id_user_login()
        {
            return this.s_id_user_login;
        }

        public void show_user_by_id(string s_id_user, string s_lang_user)
        {
            WWWForm frm = this.carrot.frm_act("get_user_by_id");
            frm.AddField("user_id", s_id_user);
            frm.AddField("user_lang", s_lang_user);
            this.carrot.send(frm, done_show_user_buy_id);
        }

        private void done_show_user_buy_id(string data)
        {
            IDictionary login_obj = (IDictionary)Json.Deserialize(data);
            this.carrot.user.show_info_user_by_data(login_obj);
            if (this.carrot.user.act_after_show_view_by_id != null) this.carrot.user.act_after_show_view_by_id(data);
        }

        public void show_user_by_id(string s_id_user, string s_lang_user, UnityAction<string> act_after)
        {
            this.act_after_show_view_by_id = act_after;
            this.show_user_by_id(s_id_user, s_lang_user);
        }

        public void set_data_field_avatar_for_update_user(Texture2D photo)
        {
            this.data_avatar_user = photo.EncodeToPNG();
            this.is_change_avatar = true;
        }

        public string get_data_user_login(string key_data)
        {
            IDictionary data_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            IList list_info = (IList)data_user["list_info"];
            for (int i = 0; i < list_info.Count; i++)
            {
                IDictionary data_item = (IDictionary)list_info[i];
                if (data_item["id_name"].ToString() == key_data) return data_item["val"].ToString();
            }
            return "";
        }

        public string get_lang_user_login()
        {
            IDictionary data_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            return data_user["user_lang"].ToString();
        }

        public GameObject add_item_info(GameObject item_obj)
        {
            GameObject item_box = Instantiate(item_obj);
            item_box.transform.SetParent(this.transform);
            item_box.transform.localPosition = new Vector3(item_box.transform.position.x, item_box.transform.position.y, 0f);
            item_box.transform.localScale = new Vector3(1f, 1f, 1f);
            item_box.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return item_box;
        }

        //Field in user info login curent
        public Carrot_Box_Item create_item_field_user_login(string s_name = "Item_box_info_user")
        {
            GameObject obj_field_info = add_item_info(this.box_info_item_prefab);
            Carrot_Box_Item box_item_new = obj_field_info.GetComponent<Carrot_Box_Item>();
            box_item_new.txt_tip.color = this.carrot.color_highlight;
            box_item_new.name = s_name;
            this.list_field_user_login.Add(obj_field_info);
            return box_item_new;
        }

        //Field in user info default
        public Carrot_Box_Item create_item_field_user_info(string s_name = "Item_box_info_user")
        {
            GameObject obj_field_info = add_item_info(this.box_info_item_prefab);
            Carrot_Box_Item box_item_new = obj_field_info.GetComponent<Carrot_Box_Item>();
            box_item_new.txt_tip.color = this.carrot.color_highlight;
            box_item_new.name = s_name;
            return box_item_new;
        }
    }
}
