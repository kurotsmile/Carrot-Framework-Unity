using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
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

        public Text login_username_title;
        public InputField inp_login_username;
        public InputField inp_login_password;
        public Image img_icon_mode_login;

        public UnityAction act_after_login_success;

        private bool is_model_login_email = true;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.UI.set_theme(this.carrot.color_highlight);
            this.check_mode_login();
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
            Query UserQuery = this.carrot.db.Collection("user-" + this.carrot.lang.get_key_lang());
            UserQuery=UserQuery.WhereEqualTo("password", inp_login_password.text);
            if(this.is_model_login_email)
                UserQuery=UserQuery.WhereEqualTo("email", this.inp_login_username.text);
            else
                UserQuery=UserQuery.WhereEqualTo("phone", this.inp_login_username.text);
           UserQuery.Limit(1).GetSnapshotAsync().ContinueWithOnMainThread(task => {
                QuerySnapshot AllUserQuerySnapshot = task.Result;
                if (task.IsFaulted)
                {
                    this.carrot.show_msg(PlayerPrefs.GetString("login", "Login"), PlayerPrefs.GetString("login_fail", "Login failed, please try again!"));
                }

                if (task.IsCompleted)
                {
                    if (AllUserQuerySnapshot.Count > 0)
                    {
                       if (this.is_model_login_email)
                           PlayerPrefs.SetString("login_username_mail", this.inp_login_username.text);
                       else
                           PlayerPrefs.SetString("login_username_phone", this.inp_login_username.text);

                       foreach (DocumentSnapshot documentSnapshot in AllUserQuerySnapshot.Documents)
                        {
                            IDictionary u = documentSnapshot.ToDictionary();
                            u["user_id"] = documentSnapshot.Id;
                            this.carrot.user.set_data_user_login(u);
                            this.carrot.user.show_info_user_by_data(u);
                            if (this.act_after_login_success != null) this.act_after_login_success();
                            this.close();
                            return; 
                        };
                    }
                    else
                    {
                        this.carrot.show_msg(PlayerPrefs.GetString("login", "Login"), PlayerPrefs.GetString("acc_no", "This account information is not in the system!"));
                    }
                }
            });
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

        public void btn_change_mode_login()
        {
            if (this.is_model_login_email)
                this.is_model_login_email = false;
            else
                this.is_model_login_email = true;
            this.check_mode_login();
        }

        public void check_mode_login()
        {
            Debug.Log("check_mode_login");
            if (this.is_model_login_email)
            {
                this.inp_login_username.text = PlayerPrefs.GetString("login_username_mail");
                this.img_icon_mode_login.sprite = this.carrot.icon_carrot_mail;
                this.login_username_title.text = PlayerPrefs.GetString("user_email", "Email");
            }
            else
            {
                this.inp_login_username.text = PlayerPrefs.GetString("login_username_phone");
                this.img_icon_mode_login.sprite = this.carrot.icon_carrot_phone;
                this.login_username_title.text = PlayerPrefs.GetString("user_phone", "Phone");
            }
                
        }
    }
}
