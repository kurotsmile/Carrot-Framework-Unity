using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_User_Login : MonoBehaviour
    {
        public Carrot_UI UI;
        private Carrot carrot;

        public InputField inp_login_username;
        public InputField inp_login_password;
        public UnityAction act_after_login_success;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.inp_login_username.text = PlayerPrefs.GetString("login_username");
            this.UI.set_theme(this.carrot.color_highlight);
        }

        public void btn_show_list_lang()
        {
            this.carrot.lang.show_list_lang(this.after_select_lang);
        }

        public void btn_show_register()
        {
            this.carrot.user.show_user_register();
        }

        private void after_select_lang(string s_data)
        {
            this.GetComponent<Carrot_lang_show>().load_lang_emp(this.carrot.lang.get_sp_lang_cur());
        }

        public void btn_show_lost_password()
        {
            this.carrot.user.show_window_lost_password();
        }

        public void btn_user_login()
        {
            WWWForm frm = this.carrot.frm_act("login");
            frm.AddField("login_username", this.inp_login_username.text.Trim());
            frm.AddField("login_password", this.inp_login_password.text);
            this.carrot.send(frm, act_user_login);
        }

        private void act_user_login(string data)
        {
            IDictionary login_obj = (IDictionary)Json.Deserialize(data);
            if (login_obj["error"].ToString() == "1")
            {
                this.carrot.show_msg(PlayerPrefs.GetString("login", "Login"), PlayerPrefs.GetString(login_obj["msg"].ToString(), login_obj["msg_en"].ToString()), Msg_Icon.Error);
            }
            else
            {
                this.carrot.user.set_data_user_login(login_obj);
                if (PlayerPrefs.GetString("lang") != login_obj["user_lang"].ToString()) this.carrot.lang.dowwnload_lang_by_key(login_obj["user_lang"].ToString());
                PlayerPrefs.SetString("login_username", this.inp_login_username.text);
                this.carrot.user.check_btn_login();
                this.close();
                if (act_after_login_success != null)
                    this.act_after_login_success();
                else
                    this.carrot.user.show_info_user_by_data(login_obj);
            }
        } 

        public void close()
        {
            this.carrot.play_sound_click();
            this.UI.close();
        }

        public void set_enable_gamepad_console(bool is_user_console)
        {
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_enable_gamepad_console(is_user_console);
        }
    }
}
