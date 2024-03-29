using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public enum Type_List_Avatar {all,boy,girl}

    public struct user_carrot_address
    {
        public string lon { get; set; }
        public string lat { get; set; }
        public string name { get; set; }
    }

    public struct user_carrot_data
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string sex { get; set; }
        public string password { get; set; }
        public string status_share { get; set; }
        public user_carrot_address address { get; set; }
        public string lang { get; set; }
        public string avatar { get; set; }
        public string date_create { get; set; }
    }

    public class Carrot_User : MonoBehaviour
    {
        private Carrot carrot;
        private Carrot_Box box_list;
        private Carrot_Box box_list_avatar;
        private On_event_change event_after_login_user;
        private Type_List_Avatar type_list_avatar = Type_List_Avatar.all;

        public Sprite icon_user_login_true;
        public Sprite icon_user_login_false;

        public GameObject window_login_prefab;

        private UnityAction<IDictionary> act_after_show_view_by_id = null;
        public UnityAction<string> event_customer_user_field;

        public string[] list_id_field_advanced;

        [Header("Icon Emp user")]
        public Sprite icon_user_name;
        public Sprite icon_user_status;
        public Sprite icon_user_register;
        public Sprite icon_user_edit;
        public Sprite icon_user_info;
        public Sprite icon_user_logout;
        public Sprite icon_user_done;
        public Sprite icon_user_change_password;

        [Header("Infor user")]
        public Color32 color_edit;
        public Color32 color_logout;
        public Color32 color_change_password;

        private string s_data_user_login;
        private string s_id_user_login;
        private string s_password_user_login;

        private bool is_model_nomal = true;

        private Carrot_Box_Item item_name;
        private Carrot_Box_Item item_phone;
        private Carrot_Box_Item item_email;
        private Carrot_Box_Item item_sex;
        private Carrot_Box_Item item_password;
        private Carrot_Box_Item item_rep_password;
        private Carrot_Box_Item item_address;
        private Carrot_Box_Item item_status_share;
        private Carrot_Box_Item item_avatar;

        private Carrot_Box_Btn_Item btn_model_nomal;
        private Carrot_Box_Btn_Item btn_model_advanced;

        user_carrot_address user_address;
        private string s_data_json_avatar_offline = "";

        private Carrot_Window_User_Login cur_window_user_login = null;
        public Carrot_Box_Item user_login_item_setting = null;

        private IDictionary data_user_temp = null;

        public void On_load(Carrot carrot)
        {
            this.carrot = carrot;
            if (this.carrot.is_offline()) this.s_data_json_avatar_offline = PlayerPrefs.GetString("s_data_json_avatar_offline", "");
            this.s_data_user_login = PlayerPrefs.GetString("data_user_login");
            this.s_id_user_login = PlayerPrefs.GetString("id_user_login");
            this.s_password_user_login = PlayerPrefs.GetString("password_user_login");
            this.check_btn_login();
            this.user_address = new user_carrot_address();
        }

        public void show_login(UnityAction act_afte_login = null)
        {
            this.carrot.play_sound_click();
            if (this.s_data_user_login != "")
                this.Show_user_cur_info();
            else
                this.carrot.user.show_window_login(act_afte_login);
        }

        private void Show_user_cur_info()
        {
            Debug.Log("Show_user_cur_info:"+this.s_data_user_login);
            IDictionary data_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            Show_info_user_by_data(data_user);
        }

        public Carrot_Box Show_info_user_by_data(IDictionary data_user)
        {
            if (this.box_list != null) this.box_list.close();
            this.box_list = this.carrot.Create_Box();
            this.box_list.set_title(this.carrot.lang.Val("acc_info", "Account Information"));
            this.box_list.set_icon(this.icon_user_info);

            if (data_user["avatar"] != null)
            {
                if (data_user["avatar"].ToString() != "")
                {
                    string user_id = "";
                    if (data_user["user_id"] != null)
                    {
                        user_id = data_user["user_id"].ToString();
                    }
                    else
                    {
                        if(data_user["id"]!=null) user_id = data_user["id"].ToString();
                    }

                    if (user_id != "")
                    {
                        Carrot_Box_Item info_avatar = this.box_list.create_item("info_avatar");
                        info_avatar.set_icon(this.carrot.icon_carrot_avatar);
                        info_avatar.set_title(this.carrot.lang.Val("user_avatar", "Avatar"));
                        info_avatar.set_tip(data_user["avatar"].ToString());
                        Sprite sp_avatar = this.carrot.get_tool().get_sprite_to_playerPrefs("avatar_user_" + user_id);
                        if (sp_avatar != null) info_avatar.set_icon_white(sp_avatar);
                        else this.carrot.get_img_and_save_playerPrefs(data_user["avatar"].ToString(), info_avatar.img_icon, "avatar_user_" + user_id);
                    }
                }
            }

            Carrot_Box_Item info_name = this.box_list.create_item("info_name");
            info_name.set_icon(this.icon_user_name);
            info_name.set_title(this.carrot.lang.Val("user_name","Full name"));
            if (data_user["name"] != null)
            {
                info_name.set_tip(data_user["name"].ToString());
            }
            else
            {
                if (this.box_list != null) this.box_list.close();
                this.Act_logout();
                return null;
            }

            if (data_user["email"] != null)
            {
                if (data_user["email"].ToString() != "")
                {
                    Carrot_Box_Item info_email = this.box_list.create_item("info_email");
                    info_email.set_icon(this.carrot.icon_carrot_mail);
                    info_email.set_title("Email box (Email)");
                    info_email.set_tip(data_user["email"].ToString());
                }
            }

            if (data_user["sex"] != null)
            {
                Carrot_Box_Item info_sex = this.box_list.create_item("info_sex");
                info_sex.set_icon(this.carrot.icon_carrot_sex);
                info_sex.set_title(this.carrot.lang.Val("user_sex", "Gender"));
                if (data_user["sex"].ToString() == "0")
                    info_sex.set_tip(this.carrot.lang.Val("user_sex_boy", "Boy"));
                else
                    info_sex.set_tip(this.carrot.lang.Val("user_sex_girl", "Girl"));
            }

            if (data_user["phone"]!=null)
            {
                if (data_user["phone"].ToString() != "")
                {
                    Carrot_Box_Item info_phone = this.box_list.create_item("info_phone");
                    info_phone.set_icon(this.carrot.icon_carrot_phone);
                    info_phone.set_title(this.carrot.lang.Val("user_phone", "Phone number"));
                    info_phone.set_tip(data_user["phone"].ToString());
                }
            }

            if (data_user["address"] != null)
            {
                IDictionary us_address = (IDictionary)data_user["address"];
                if (us_address["name"] != null)
                {
                    if (us_address["name"].ToString() != "")
                    {
                        Carrot_Box_Item info_address = this.box_list.create_item("info_address");
                        info_address.set_icon(this.carrot.icon_carrot_address);
                        info_address.set_title(this.carrot.lang.Val("user_address", "Your address"));
                        info_address.set_tip(us_address["name"].ToString());
                    }
                }
            }

            if (data_user["status_share"] != null)
            {
                Carrot_Box_Item info_status = this.box_list.create_item("info_status");
                info_status.set_icon(this.icon_user_status);
                info_status.set_title(this.carrot.lang.Val("user_info_status", "Information status"));
                if (data_user["status_share"].ToString() == "0")
                    info_status.set_tip(this.carrot.lang.Val("user_info_status_yes", "Share information"));
                else
                    info_status.set_tip(this.carrot.lang.Val("user_info_status_no", "Do not share information"));
            }

            if (data_user["user_id"] != null)
            {
                if (this.s_id_user_login == data_user["user_id"].ToString())
                {
                    Carrot_Box_Btn_Panel panel_btn = this.box_list.create_panel_btn();

                    Carrot_Button_Item btn_edit = panel_btn.create_btn("btn_edit");
                    btn_edit.set_icon(this.carrot.icon_carrot_done);
                    btn_edit.set_label(this.carrot.lang.Val("edit", "Edit"));
                    btn_edit.set_label_color(Color.white);
                    btn_edit.set_bk_color(this.carrot.color_highlight);
                    btn_edit.set_act_click(() => Act_show_edit_user(data_user));

                    Carrot_Button_Item btn_logout = panel_btn.create_btn("btn_logout");
                    btn_logout.set_icon(this.icon_user_logout);
                    btn_logout.set_label(this.carrot.lang.Val("logout", "Log out"));
                    btn_logout.set_label_color(Color.white);
                    btn_logout.set_bk_color(this.carrot.color_highlight);
                    btn_logout.set_act_click(() => this.Act_logout());


                    Carrot_Button_Item btn_canel = panel_btn.create_btn("btn_cancel");
                    btn_canel.set_icon(this.carrot.icon_carrot_cancel);
                    btn_canel.set_label(this.carrot.lang.Val("cancel", "Cancel"));
                    btn_canel.set_label_color(Color.white);
                    btn_canel.set_bk_color(this.carrot.color_highlight);
                    btn_canel.set_act_click(() => this.Act_close_box());
                }
            }

            this.box_list.update_color_table_row();
            this.box_list.update_gamepad_cosonle_control();
            if (this.act_after_show_view_by_id != null)
            {
                this.act_after_show_view_by_id.Invoke(data_user);
                this.act_after_show_view_by_id = null;
            }
            return this.box_list;
        }

        private void Act_show_edit_user(IDictionary data_user)
        {
            this.Edit_or_add_by_data(data_user);
        }

        private void Act_logout()
        {
            this.delete_data_user_login();
            this.check_and_show_item_login_setting();
            if (this.box_list != null) this.box_list.close();
        }

        private void show_window_login(UnityAction act_login_success)
        {
            GameObject window_login = this.carrot.create_window(this.window_login_prefab);
            this.cur_window_user_login = window_login.GetComponent<Carrot_Window_User_Login>();
            this.cur_window_user_login.On_load(this.carrot);
            this.cur_window_user_login.act_after_login_success = act_login_success;
            window_login.GetComponent<Carrot_lang_show>().load_lang_emp(this.carrot.lang.Get_sp_lang_cur(),carrot.lang);
            this.cur_window_user_login.Check_mode_login();
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.cur_window_user_login.UI.get_list_btn());
        }

        public Carrot_Window_User_Login get_cur_window_user_login()
        {
            return this.cur_window_user_login;
        }

        public void show_window_lost_password()
        {
            this.box_list = this.carrot.Create_Box();
            this.box_list.set_icon(this.icon_user_change_password);
            this.box_list.set_title(this.carrot.lang.Val("forgot_password", "Forgot password"));

            Carrot_Box_Item item_tip=this.box_list.create_item("item_username");
            item_tip.set_icon(this.icon_user_info);
            item_tip.set_title(this.carrot.lang.Val("forgot_password", "Forgot password"));
            item_tip.set_tip(this.carrot.lang.Val("forgot_password_tip", "Enter your phone number or email to retrieve the password"));

            this.item_email = this.box_list.create_item("item_email");
            this.item_email.set_icon(this.carrot.icon_carrot_mail);
            this.item_email.set_title("Email box (Email)");
            this.item_email.set_tip("Enter your email address (Email)");
            this.item_email.set_lang_data("user_email", "user_email_tip");
            this.item_email.load_lang_data();
            this.item_email.set_type(Box_Item_Type.box_email_input);
            this.item_email.check_type();

            this.item_phone = this.box_list.create_item("item_phone");
            this.item_phone.set_icon(this.carrot.icon_carrot_phone);
            this.item_phone.set_title("Phone number");
            this.item_phone.set_tip("Enter your phone number");
            this.item_phone.set_lang_data("user_phone", "user_phone_tip");
            this.item_phone.load_lang_data();
            this.item_phone.set_type(Box_Item_Type.box_number_input);
            this.item_phone.check_type();

            Carrot_Box_Btn_Panel panel_btn=this.box_list.create_panel_btn();
            Carrot_Button_Item btn_done=panel_btn.create_btn("item_done");
            btn_done.set_icon(this.carrot.icon_carrot_done);
            btn_done.set_label_color(Color.white);
            btn_done.set_bk_color(this.carrot.color_highlight);
            btn_done.set_label(this.carrot.lang.Val("done","Done"));
            btn_done.set_act_click(Act_done_lost_password);

            Carrot_Button_Item btn_cancel = panel_btn.create_btn("item_cancel");
            btn_cancel.set_icon(this.carrot.icon_carrot_done);
            btn_cancel.set_label_color(Color.white);
            btn_cancel.set_bk_color(this.carrot.color_highlight);
            btn_cancel.set_label(this.carrot.lang.Val("cancel", "Cancel"));
            btn_cancel.set_act_click(() => this.box_list.close());
            this.box_list.update_gamepad_cosonle_control();
        }

        private void Act_done_lost_password()
        {
            this.carrot.show_loading();
            StructuredQuery q = new("user-" + this.carrot.lang.Get_key_lang());
            if(this.item_email.get_val()!="")q.Add_where("email",Query_OP.EQUAL,this.item_email.get_val());
            if(this.item_phone.get_val()!="") q.Add_where("phone",Query_OP.EQUAL,this.item_phone.get_val());
            q.Set_limit(1);
            this.carrot.server.Get_doc(q.ToJson(), Act_done_lost_password_done, Act_done_lost_password_fail);
        }

        private void Act_done_lost_password_done(string s_data)
        {
            this.carrot.hide_loading();
            Fire_Collection fc = new(s_data);
            if (!fc.is_null)
            {
                string password =fc.fire_document[0].Get_val("password").ToString();
                if (password!=null) this.carrot.Show_msg(this.carrot.lang.Val("pass_acc_msg", "The password for the account is:")+password);
                return;
            }
            else
            {
                this.carrot.Show_msg(this.carrot.lang.Val("forgot_password", "Forgot password"), this.carrot.lang.Val("acc_no", "This account information is not in the system!"));
            }
        }

        private void Act_done_lost_password_fail(string s_error)
        {
            this.carrot.hide_loading();
            this.carrot.Show_msg(this.carrot.lang.Val("forgot_password", "Forgot password"), this.carrot.lang.Val("acc_no", "This account information is not in the system!"));
        }

        public void set_data_user_login(IDictionary data_user)
        {
            this.s_data_user_login = Json.Serialize(data_user);
            this.s_id_user_login = data_user["user_id"].ToString();
            this.s_password_user_login = data_user["password"].ToString();
            Debug.Log("Save data user:"+this.s_data_user_login);
            PlayerPrefs.SetString("data_user_login", this.s_data_user_login);
            PlayerPrefs.SetString("id_user_login", this.s_id_user_login);
            PlayerPrefs.SetString("password_user_login", this.s_password_user_login);
            if (this.carrot.img_btn_login != null)
            {
                if(data_user["avatar"].ToString()!="") this.carrot.get_img_and_save_playerPrefs(data_user["avatar"].ToString(), this.carrot.img_btn_login, "carrot_user_avatar");
            }
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

        public void show_user_register()
        {
            this.s_id_user_login = "";
            this.Edit_or_add_by_data(null);
        }

        public void check_and_show_item_login_setting()
        {
            if (this.user_login_item_setting != null)
            {
                if (this.get_id_user_login() == "")
                {
                    this.user_login_item_setting.set_title(this.carrot.lang.Val("login", "Login"));
                    this.user_login_item_setting.set_tip(this.carrot.lang.Val("login_tip", "Sign in to your carrot account to manage data, and use many other services"));
                    this.user_login_item_setting.set_lang_data("login", "login_tip");

                    Carrot_Box_Btn_Item item_btn_regiter = this.user_login_item_setting.create_item();
                    item_btn_regiter.set_icon(this.icon_user_register);
                    item_btn_regiter.set_color(this.carrot.color_highlight);
                    item_btn_regiter.set_act(this.show_user_register);

                    Carrot_Box_Btn_Item item_btn_password = this.user_login_item_setting.create_item();
                    item_btn_password.set_icon(this.icon_user_change_password);
                    item_btn_password.set_color(this.carrot.color_highlight);
                    item_btn_password.set_act(this.show_window_lost_password);
                }
                else
                {
                    this.user_login_item_setting.set_title(this.carrot.lang.Val("acc_info", "Account Information"));
                    this.user_login_item_setting.set_tip(this.carrot.lang.Val("acc_edit_tip", "Click this button to update account information"));
                    this.user_login_item_setting.set_lang_data("acc_info", "acc_edit_tip");
                }

                this.user_login_item_setting.set_act(() => this.show_login(this.carrot.Reload_setting));
                this.load_avatar_user_login(this.user_login_item_setting.img_icon);
            }
        }

        private void Edit_or_add_by_data(IDictionary data)
        {
            if (this.box_list != null) this.box_list.close();
            this.box_list = this.carrot.Create_Box();
            this.box_list.set_icon(this.icon_user_register);
            if(data==null)
                this.box_list.set_title(this.carrot.lang.Val("register", "Register Account"));
            else
                this.box_list.set_title(this.carrot.lang.Val("acc_edit", "Update account information"));

            this.btn_model_nomal = this.box_list.create_btn_menu_header(this.carrot.icon_carrot_nomal);
            this.btn_model_nomal.set_act(this.Act_model_nomal_register);
            if (this.is_model_nomal) this.btn_model_nomal.set_icon_color(this.carrot.color_highlight);

            this.btn_model_advanced = this.box_list.create_btn_menu_header(this.carrot.icon_carrot_advanced);
            this.btn_model_advanced.set_act(this.act_model_advanced_register);
            if (!this.is_model_nomal) this.btn_model_advanced.set_icon_color(this.carrot.color_highlight);

            this.item_avatar = this.box_list.create_item("item_avatar");
            this.item_avatar.set_type(Box_Item_Type.box_value_txt);
            this.item_avatar.check_type();
            this.item_avatar.set_icon(this.carrot.icon_carrot_avatar);
            this.item_avatar.set_title("Avatar");
            this.item_avatar.set_tip("Choose your profile picture");
            this.item_avatar.set_lang_data("user_avatar", "user_avatar_tip");
            this.item_avatar.load_lang_data();
            this.item_avatar.set_act(()=>this.Act_show_list_avatar());
            if (data != null)
            {
                if (data["avatar"] != null)
                {
                    this.item_avatar.set_val(data["avatar"].ToString());
                    Sprite sp_avatar = this.carrot.get_tool().get_sprite_to_playerPrefs("carrot_user_avatar");
                    if (sp_avatar != null) this.item_avatar.set_icon_white(sp_avatar);
                    else this.carrot.get_img_and_save_playerPrefs(data["avatar"].ToString(), this.item_avatar.img_icon, "carrot_user_avatar");
                }
            }

            Carrot_Box_Btn_Item btn_list_avatar=this.item_avatar.create_item();
            btn_list_avatar.set_icon(this.carrot.icon_carrot_all_category);
            btn_list_avatar.set_act(()=>this.Act_show_list_avatar());
            btn_list_avatar.set_color(this.carrot.color_highlight);

            this.item_name = this.box_list.create_item("item_name");
            this.item_name.set_type(Box_Item_Type.box_value_input);
            this.item_name.check_type();
            this.item_name.set_icon(this.icon_user_name);
            this.item_name.set_title("Full name");
            this.item_name.set_tip("Enter your full name");
            this.item_name.set_lang_data("user_name", "user_name_tip");
            this.item_name.load_lang_data();
            if (data != null) if (data["name"] != null) this.item_name.set_val(data["name"].ToString());

            this.item_email = this.box_list.create_item("item_email");
            this.item_email.set_type(Box_Item_Type.box_email_input);
            this.item_email.check_type();
            this.item_email.set_icon(this.carrot.icon_carrot_mail);
            this.item_email.set_title("Email box (Email)");
            this.item_email.set_tip("Enter your email address (Email)");
            this.item_email.set_lang_data("user_email", "user_email_tip");
            this.item_email.load_lang_data();
            if (data != null) if (data["email"] != null) this.item_email.set_val(data["email"].ToString());

            this.item_sex = this.box_list.create_item("item_sex");
            this.item_sex.set_type(Box_Item_Type.box_value_dropdown);
            this.item_sex.check_type();
            this.item_sex.set_icon(this.carrot.icon_carrot_sex);
            this.item_sex.set_title("Sex");
            this.item_sex.set_tip("Choose your gender");
            this.item_sex.set_lang_data("user_sex", "user_sex_tip");
            this.item_sex.dropdown_val.ClearOptions();
            this.item_sex.dropdown_val.options.Add(new Dropdown.OptionData { text = this.carrot.lang.Val("user_sex_boy", "boy") });
            this.item_sex.dropdown_val.options.Add(new Dropdown.OptionData { text = this.carrot.lang.Val("user_sex_girl", "girl") });
            if (data != null)
            {
                if (data["sex"] != null)
                    this.item_sex.dropdown_val.value = int.Parse(data["sex"].ToString());
                else
                    this.item_sex.dropdown_val.value = 0;
            }
            this.item_sex.dropdown_val.RefreshShownValue();
            this.item_sex.load_lang_data();

            this.item_phone = this.box_list.create_item("item_phone");
            this.item_phone.set_type(Box_Item_Type.box_number_input);
            this.item_phone.check_type();
            this.item_phone.set_icon(this.carrot.icon_carrot_phone);
            this.item_phone.set_title("Phone number");
            this.item_phone.set_tip("Enter your phone number");
            this.item_phone.set_lang_data("user_phone", "user_phone_tip");
            this.item_phone.load_lang_data();
            if (data != null) if (data["phone"] != null) this.item_phone.set_val(data["phone"].ToString());

            this.item_address = this.box_list.create_item("item_address");
            this.item_address.set_type(Box_Item_Type.box_value_input);
            this.item_address.check_type();
            this.item_address.set_icon(this.carrot.icon_carrot_address);
            this.item_address.set_title("Your address");
            this.item_address.set_tip("Enter your permanent address");
            this.item_address.set_lang_data("user_address", "user_address_tip");
            this.item_address.load_lang_data();
            if (data != null)
            {
                if (data["address"] != null)
                {
                    IDictionary u_address = (IDictionary)data["address"];
                    this.item_address.set_val(u_address["name"].ToString());
                }
            }
            Carrot_Box_Btn_Item btn_get_location=this.item_address.create_item();
            btn_get_location.set_color(this.carrot.color_highlight);
            btn_get_location.set_icon(this.carrot.icon_carrot_location);
            btn_get_location.set_act(()=>this.carrot.location.get_location(this.act_get_location_done));

            if (this.is_model_nomal) this.item_address.gameObject.SetActive(false);

            this.item_status_share = this.box_list.create_item("user_info_status");
            this.item_status_share.set_type(Box_Item_Type.box_value_dropdown);
            this.item_status_share.check_type();
            this.item_status_share.set_icon(this.icon_user_status);
            this.item_status_share.set_title("Information status");
            this.item_status_share.set_tip("Select information sharing status");
            this.item_status_share.set_lang_data("user_info_status", "user_info_status_tip");
            this.item_status_share.dropdown_val.ClearOptions();
            this.item_status_share.dropdown_val.options.Add(new Dropdown.OptionData { text = this.carrot.lang.Val("user_info_status_yes", "Share information") });
            this.item_status_share.dropdown_val.options.Add(new Dropdown.OptionData { text = this.carrot.lang.Val("user_info_status_no", "Do not share information") });
            this.item_status_share.dropdown_val.value = 0;
            this.item_status_share.dropdown_val.RefreshShownValue();
            this.item_status_share.load_lang_data();
            if (data != null)
            {
                if (data["status_share"] != null)
                    this.item_status_share.dropdown_val.value = int.Parse(data["status_share"].ToString());
                else
                    this.item_status_share.dropdown_val.value = 0;
            }
            if (this.is_model_nomal) this.item_status_share.gameObject.SetActive(false);

            this.item_password = this.box_list.create_item("item_password");
            this.item_password.set_type(Box_Item_Type.box_password_input);
            this.item_password.check_type();
            this.item_password.set_icon(this.carrot.user.icon_user_change_password);
            this.item_password.set_title("Password");
            this.item_password.set_tip("Enter your password");
            this.item_password.set_lang_data("user_password", "user_password_tip");
            this.item_password.load_lang_data();
            if (data != null) if (data["password"] != null) this.item_password.set_val(data["password"].ToString());

            this.item_rep_password = this.box_list.create_item("item_rep_password");
            this.item_rep_password.set_type(Box_Item_Type.box_password_input);
            this.item_rep_password.check_type();
            this.item_rep_password.set_icon(this.carrot.user.icon_user_change_password);
            this.item_rep_password.set_title("Re-enter password");
            this.item_rep_password.set_tip("Confirm your password again");
            this.item_rep_password.set_lang_data("user_rep_password", "user_rep_password_tip");
            this.item_rep_password.load_lang_data();
            if (data != null) if (data["password"] != null) this.item_rep_password.set_val(data["password"].ToString());


            Carrot_Box_Btn_Panel panel_btn = this.box_list.create_panel_btn();
            Carrot_Button_Item btn_done = panel_btn.create_btn("btn_done");
            btn_done.set_icon(this.carrot.icon_carrot_done);
            btn_done.set_label(this.carrot.lang.Val("done", "Done"));
            btn_done.set_label_color(Color.white);
            btn_done.set_bk_color(this.carrot.color_highlight);
            btn_done.set_act_click(Act_done_register);

            Carrot_Button_Item btn_canel = panel_btn.create_btn("btn_cancel");
            btn_canel.set_icon(this.carrot.icon_carrot_cancel);
            btn_canel.set_label(this.carrot.lang.Val("cancel", "Cancel"));
            btn_canel.set_label_color(Color.white);
            btn_canel.set_bk_color(this.carrot.color_highlight);
            btn_canel.set_act_click(Act_close_box);

            this.box_list.update_gamepad_cosonle_control();
        }

        private void act_get_location_done(LocationInfo loc)
        {
            this.user_address.lat = loc.latitude.ToString();
            this.user_address.lon = loc.longitude.ToString();
        }

        private void Act_done_register()
        {
            string s_title;
            if (this.s_id_user_login == "")
                s_title = this.carrot.lang.Val("register", "Register Account");
            else
                s_title = this.carrot.lang.Val("acc_edit", "Update account information");

            if (this.item_name.get_val().Trim().Length<3)
            {
                this.carrot.Show_msg(s_title, this.carrot.lang.Val("error_name", "The account name cannot be empty and be greater than 5 characters"));
                return;
            }

            if (this.item_password.get_val().Trim().Length < 3)
            {
                this.carrot.Show_msg(s_title, this.carrot.lang.Val("error_password", "Password cannot be blank and be greater than 6 characters"));
                return;
            }

            if (this.item_password.get_val().Trim()!=this.item_rep_password.get_val().Trim())
            {
                this.carrot.Show_msg(s_title, this.carrot.lang.Val("error_rep_password", "Re-enter the password does not match."));
                return;
            }

            this.user_address.name = this.item_address.get_val();
            user_carrot_data u = new user_carrot_data()
            {
                name = this.item_name.get_val(),
                sex=this.item_sex.get_val(),
                email = this.item_email.get_val(),
                phone = this.item_phone.get_val(),
                address = this.user_address,
                password = this.item_password.get_val(),
                lang = this.carrot.lang.Get_key_lang(),
                status_share = this.item_status_share.get_val(),
                avatar = this.item_avatar.get_val(),
                date_create = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            this.carrot.show_loading();
            string user_id_new = "";
            if(this.s_id_user_login=="")
                user_id_new="user" +this.carrot.generateID();
            else
                user_id_new = this.s_id_user_login;

            this.data_user_temp= (IDictionary)Json.Deserialize(JsonConvert.SerializeObject(u));
            this.carrot.server.Add_Document_To_Collection("user-" + this.carrot.lang.Get_key_lang(), user_id_new, this.carrot.server.Convert_IDictionary_to_json(this.data_user_temp), Act_done_register_done, Act_done_register_fail);
        }

        private void Act_done_register_done(string s_data)
        {
            this.carrot.hide_loading();
            if (this.s_id_user_login == "")
                this.carrot.Show_msg(this.carrot.lang.Val("register", "Register Account"), this.carrot.lang.Val("register_success", "Account registration is successful!"), Msg_Icon.Success);
            else
            {
                this.carrot.Show_msg(this.carrot.lang.Val("register", "Register Account"), this.carrot.lang.Val("acc_edit_success", "Successful account information update!"), Msg_Icon.Success);
                this.data_user_temp["user_id"] = this.s_id_user_login;
                this.set_data_user_login(this.data_user_temp);
            }
            if (this.box_list != null) this.box_list.close();
        }

        private void Act_done_register_fail(string s_error)
        {
            this.carrot.Show_msg(s_error);
        }

        public string get_id_user_login()
        {
            return this.s_id_user_login;
        }

        public void show_user_by_id(string s_id_user, string s_lang_user)
        {
            this.carrot.play_sound_click();
            this.carrot.show_loading();
            this.carrot.server.Get_doc_by_path("user-" + s_lang_user, s_id_user, Act_show_user_by_id_done, Act_show_user_by_id_fail);
        }

        private void Act_show_user_by_id_done(string s_data)
        {
            Debug.Log("user:" + s_data);
            this.carrot.hide_loading();
            Fire_Document fd = new(s_data);
            IDictionary data_user = fd.Get_IDictionary();
            if (data_user!=null)
                this.Show_info_user_by_data(data_user);
            else
                this.carrot.Show_msg(this.carrot.lang.Val("acc_info", "Account Information"), "Account not found", Msg_Icon.Alert);
        }

        private void Act_show_user_by_id_fail(string s_error)
        {
            this.carrot.hide_loading();
            this.carrot.Show_msg(this.carrot.lang.Val("acc_info", "Account Information"), "The operation failed, please try again next time!", Msg_Icon.Error);
        }

        public void show_user_by_id(string s_id_user, string s_lang_user, UnityAction<IDictionary> act_after)
        {
            this.act_after_show_view_by_id = act_after;
            this.show_user_by_id(s_id_user, s_lang_user);
        }

        public string get_data_user_login(string key_data)
        {
            IDictionary data_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            if (data_user[key_data] != null)
                return data_user[key_data].ToString();
            else
                return "";
        }

        public string get_lang_user_login()
        {
            IDictionary data_user = (IDictionary)Json.Deserialize(this.s_data_user_login);
            return data_user["lang"].ToString();
        }

        private void Act_close_box()
        {
            if (this.box_list != null) this.box_list.close();
        }

        private void Act_model_nomal_register()
        {
            this.carrot.play_sound_click();
            this.btn_model_advanced.set_icon_color(Color.gray);
            this.btn_model_nomal.set_icon_color(this.carrot.color_highlight);
            this.is_model_nomal = true;
            this.item_avatar.gameObject.SetActive(true);
            this.item_name.gameObject.SetActive(true);
            this.item_email.gameObject.SetActive(true);
            this.item_sex.gameObject.SetActive(true);
            this.item_phone.gameObject.SetActive(true);
            this.item_address.gameObject.SetActive(false);
            this.item_status_share.gameObject.SetActive(false);
            this.item_password.gameObject.SetActive(true);
            this.item_rep_password.gameObject.SetActive(true);
        }

        private void act_model_advanced_register()
        {
            this.carrot.play_sound_click();
            this.btn_model_advanced.set_icon_color(this.carrot.color_highlight);
            this.btn_model_nomal.set_icon_color(Color.gray);
            this.is_model_nomal = false;
            this.item_avatar.gameObject.SetActive(true);
            this.item_name.gameObject.SetActive(true);
            this.item_email.gameObject.SetActive(true);
            this.item_sex.gameObject.SetActive(true);
            this.item_phone.gameObject.SetActive(true);
            this.item_address.gameObject.SetActive(true);
            this.item_status_share.gameObject.SetActive(true);
            this.item_password.gameObject.SetActive(true);
            this.item_rep_password.gameObject.SetActive(true);
        }

        #region List Avatar
        private void Act_show_list_avatar()
        {
            if (this.box_list_avatar != null) this.box_list_avatar.close();

            if (this.s_data_json_avatar_offline=="")
            {
                this.carrot.show_loading();
                StructuredQuery q = new("user-avatar");
                this.carrot.server.Get_doc(q.ToJson(), Act_show_list_avatar_done, Act_show_list_avatar_fail);
            }
            else
            {
                this.Act_load_list_avatar(this.s_data_json_avatar_offline);
            }
        }

        private void Act_show_list_avatar_done(string s_data)
        {
            this.s_data_json_avatar_offline = s_data;
            PlayerPrefs.SetString("s_data_json_avatar_offline", this.s_data_json_avatar_offline);
            this.Act_load_list_avatar(s_data);
        }

        private void Act_show_list_avatar_fail(string s_error)
        {
            this.carrot.hide_loading();
            if (this.box_list_avatar != null) this.box_list_avatar.close();
        }

        private void Act_load_list_avatar(string s_data)
        {
            this.carrot.hide_loading();
            Fire_Collection fc = new(s_data);
            this.carrot.log("act_get_list_avatar_by_query (" + this.type_list_avatar + ")");

            if (!fc.is_null)
            {
                if (this.box_list_avatar != null) this.box_list_avatar.close();
                this.carrot.hide_loading();
                this.box_list_avatar = this.carrot.Create_Box("list_avatar");
                box_list_avatar.set_title(this.carrot.lang.Val("user_avatar", "List Avatar"));
                box_list_avatar.set_icon(this.carrot.icon_carrot_avatar);
                box_list_avatar.set_type(Carrot_Box_Type.Grid_Box);

                Carrot_Box_Btn_Item btn_all = this.box_list_avatar.create_btn_menu_header(this.carrot.icon_carrot_all_category);
                btn_all.set_act(() => this.Act_set_type_show_list_avatar(Type_List_Avatar.all));
                if (type_list_avatar == Type_List_Avatar.all) btn_all.set_icon_color(this.carrot.color_highlight);

                Carrot_Box_Btn_Item btn_boy = this.box_list_avatar.create_btn_menu_header(this.carrot.icon_carrot_sex_boy);
                btn_boy.set_act(() => this.Act_set_type_show_list_avatar(Type_List_Avatar.boy));
                if (type_list_avatar == Type_List_Avatar.boy) btn_boy.set_icon_color(this.carrot.color_highlight);

                Carrot_Box_Btn_Item btn_girl = this.box_list_avatar.create_btn_menu_header(this.carrot.icon_carrot_sex_girl);
                btn_girl.set_act(() => this.Act_set_type_show_list_avatar(Type_List_Avatar.girl));
                if (type_list_avatar == Type_List_Avatar.girl) btn_girl.set_icon_color(this.carrot.color_highlight);

                for (int i = 0; i < fc.fire_document.Length; i++)
                {
                    IDictionary avatar_data =fc.fire_document[i].Get_IDictionary();
                    if (this.type_list_avatar == Type_List_Avatar.all)
                        this.Add_item_avatar_to_list_box(avatar_data);

                    else
                        if (this.type_list_avatar.ToString().ToLower() == avatar_data["type"].ToString()) this.Add_item_avatar_to_list_box(avatar_data);
                }
            }
        }

        private void Add_item_avatar_to_list_box(IDictionary avatar_data)
        {
            string s_id_avatar = "avt_"+avatar_data["id"].ToString();
            string s_url_icon = avatar_data["icon"].ToString();
            Carrot_Box_Item avatar_item = box_list_avatar.create_item();
            avatar_item.set_icon(this.carrot.icon_carrot_avatar);
            Sprite sp = this.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
            if (sp != null)
                avatar_item.set_icon_white(sp);
            else
                this.carrot.get_img_and_save_playerPrefs(s_url_icon, avatar_item.img_icon, s_id_avatar);
            avatar_item.set_act(() => Select_avatar(s_url_icon, avatar_item.img_icon.sprite));
        }

        private void Act_set_type_show_list_avatar(Type_List_Avatar type_list)
        {
            this.type_list_avatar = type_list;
            this.Act_show_list_avatar();
        }

        private void Select_avatar(string url_icon, Sprite sp_icon)
        {
            this.item_avatar.img_icon.sprite = sp_icon;
            this.item_avatar.img_icon.color = Color.white;
            this.item_avatar.set_val(url_icon);
            if (this.box_list_avatar != null) this.box_list_avatar.close();
        }
        #endregion
    }
}
