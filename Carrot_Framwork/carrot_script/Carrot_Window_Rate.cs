using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Rate : MonoBehaviour
    {
        private Carrot carrot;
        public Carrot_UI UI;
        public GameObject panel_rate_rating;
        public GameObject panel_rate_feedback;
        public GameObject button_rate_feeedback;
        public Image[] img_star_feedback;
        public InputField inp_review_feedback;
        private int index_star_feedback;
        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.btn_sel_rate(-1);
            this.panel_rate_rating.SetActive(true);
            this.panel_rate_feedback.SetActive(false);

            if (this.carrot.Carrotstore_AppId != "" && this.carrot.user.get_id_user_login() != "")
                this.button_rate_feeedback.SetActive(true);
            else
                this.button_rate_feeedback.SetActive(false);

            if (this.carrot.auto_open_rate_store) this.app_rate();
            if(this.carrot.type_control!=TypeControl.None) this.carrot.game.set_list_button_gamepad_console(UI.get_list_btn());
            this.GetComponent<Carrot_lang_show>().load_lang_emp();
            this.UI.set_theme(this.carrot.color_highlight);
        }

        public void app_rate()
        {
            this.act_rate();
        }

        private void act_rate()
        {
            if (this.carrot.type_rate == TypeRate.Link_Share_CarrotApp) Application.OpenURL(this.carrot.link_share_app);
            if (this.carrot.type_rate == TypeRate.Market_Android) Application.OpenURL("market://details?id=" + Application.identifier);
            if (this.carrot.type_rate == TypeRate.Ms_Windows_Store) Application.OpenURL("ms-windows-store://review/?ProductId=" + this.carrot.WindowUWP_ProductId);
            if (this.carrot.type_rate == TypeRate.Amazon_app_store) Application.OpenURL("amzn://apps/android?p=" + Application.identifier);
        }

        public void btn_show_rate_feedback()
        {
            this.inp_review_feedback.text = "";
            this.load_rate_by_user();
            this.panel_rate_rating.SetActive(false);
            this.panel_rate_feedback.SetActive(true);
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(UI.get_list_btn());
        }

        public void btn_close_rate_feedback()
        {
            this.panel_rate_rating.SetActive(true);
            this.panel_rate_feedback.SetActive(false);
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(UI.get_list_btn());
        }

        public void btn_submit_rate_feedback()
        {
            if (this.index_star_feedback == -1 && this.inp_review_feedback.text.Trim() == "") return;

            WWWForm frm_rate = this.carrot.frm_act("submit_rate");
            frm_rate.AddField("app_id", this.carrot.Carrotstore_AppId);
            frm_rate.AddField("inp_review", this.inp_review_feedback.text);
            frm_rate.AddField("star_feedback", this.index_star_feedback);
            frm_rate.AddField("user_id", this.carrot.user.get_id_user_login());
            frm_rate.AddField("user_lang", this.carrot.user.get_lang_user_login());
            this.carrot.send_hide(frm_rate, done_sumbit_feedback);
        }

        private void done_sumbit_feedback(string s_data)
        {
            this.carrot.show_msg(PlayerPrefs.GetString("send_feedback", "Send Feedback"), PlayerPrefs.GetString("rate_thanks", "Send your comments to the successful developer. Thanks for your feedback!"), Msg_Icon.Success);
        }

        public void btn_sel_rate(int index_star)
        {
            this.index_star_feedback = index_star;
            for (int i = 0; i < this.img_star_feedback.Length; i++)
            {
                if (i <= index_star)
                    this.img_star_feedback[i].color = this.carrot.color_highlight;
                else
                    this.img_star_feedback[i].color = Color.white;
            }
        }

        private void load_rate_by_user()
        {
            string user_id_login = this.carrot.user.get_id_user_login();
            if (user_id_login != "")
            {
                WWWForm frm_rate = this.carrot.frm_act("get_rate");
                frm_rate.AddField("user_id", user_id_login);
                frm_rate.AddField("user_lang", this.carrot.user.get_lang_user_login());
                frm_rate.AddField("app_id", this.carrot.Carrotstore_AppId);
                this.carrot.send_hide(frm_rate, act_done_get_rate);
            }
        }

        private void act_done_get_rate(string s_data)
        {
            if (s_data == "") return;
            IDictionary data_rate = (IDictionary)Json.Deserialize(s_data);
            if (data_rate["comment"] != null) this.inp_review_feedback.text = data_rate["comment"].ToString();
            if (data_rate["star"] != null)
            {
                int index_star = int.Parse(data_rate["star"].ToString());
                index_star--;
                this.btn_sel_rate(index_star);
            }
            if (this.carrot.model_app == ModelApp.Develope) Debug.Log("Get rate:" + s_data);
        }

        public void close()
        {
            this.UI.close();
        }

        public void set_enable_gamepad_console(bool is_user_console)
        {
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_enable_gamepad_console(is_user_console);
        }
    }
}
