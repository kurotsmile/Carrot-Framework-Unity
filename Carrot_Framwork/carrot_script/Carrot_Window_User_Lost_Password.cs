using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_User_Lost_Password : MonoBehaviour
    {
        public Carrot_UI UI;
        private Carrot carrot;
        public InputField inp_lost_password;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.inp_lost_password.text = PlayerPrefs.GetString("login_username");
        }

        public void close()
        {
            this.UI.close();
        }

        public void get_passowrd()
        {
            WWWForm frm = this.carrot.frm_act("get_password");
            frm.AddField("inp_info", this.inp_lost_password.text);
            this.carrot.send(frm, get_password_done);
        }

        void get_password_done(string data)
        {
            IDictionary alert = (IDictionary)Json.Deserialize(data);
            if (alert["error"].ToString() == "1")
                this.carrot.show_msg(PlayerPrefs.GetString("forgot_password", "Forgot password"), PlayerPrefs.GetString(alert["msg"].ToString(), alert["msg_en"].ToString()), Msg_Icon.Error);
            else
            {
                this.close();
                string s_password_msg = PlayerPrefs.GetString(alert["msg"].ToString(), alert["msg_en"].ToString()) + " " + alert["password"].ToString();
                this.carrot.show_msg(PlayerPrefs.GetString("forgot_password", "Forgot password"), s_password_msg, Msg_Icon.Success);
            }
        }

        public void set_enable_gamepad_console(bool is_user_console)
        {
            if (this.carrot.type_control != TypeControl.None)this.carrot.game.set_enable_gamepad_console(is_user_console);
        }
    }
}
