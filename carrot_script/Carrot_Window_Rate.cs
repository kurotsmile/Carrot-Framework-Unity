using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public struct Carrot_Rate_user_data
    {
        public string id {get;set;}
        public string name { get; set; }
        public string avatar { get; set; }
        public string lang {get;set;}
    }

    public struct Carrot_Rate_data
    {
        public Carrot_Rate_user_data user {get;set;}
        public string star {get;set;}
        public string comment {get;set;}
        public string date {get;set;}
    }

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
        private int index_rate_edit = -1;

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
            if (this.carrot.type_control != TypeControl.None) this.carrot.game.set_list_button_gamepad_console(UI.get_list_btn());
            this.GetComponent<Carrot_lang_show>().load_lang_emp(carrot.lang);
            this.UI.set_theme(this.carrot.color_highlight);
        }

        public void app_rate()
        {
            this.act_rate();
        }

        private void act_rate()
        {
            if (this.carrot.type_rate == TypeRate.Link_Share_CarrotApp) Application.OpenURL(this.carrot.mainhost + "?p=app&id" + this.carrot.Carrotstore_AppId);
            if (this.carrot.type_rate == TypeRate.Market_Android) Application.OpenURL("market://details?id=" + Application.identifier);
            if (this.carrot.type_rate == TypeRate.Ms_Windows_Store) Application.OpenURL("ms-windows-store://review/?ProductId=" + this.carrot.WindowUWP_ProductId);
            if (this.carrot.type_rate == TypeRate.Amazon_app_store) Application.OpenURL("amzn://apps/android?p=" + Application.identifier);
        }

        public void btn_show_rate_feedback()
        {
            this.inp_review_feedback.text = "";
            this.Load_rate_by_user();
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
            this.carrot.show_loading();
            StructuredQuery q = new("app");
            q.Add_where("name_en", Query_OP.EQUAL, this.carrot.Carrotstore_AppId);
            q.Add_select("rates");
            q.Set_limit(1);
            this.carrot.server.Get_doc(q.ToJson(), Act_load_data_feedback_done, Act_submit_rate_feedback_fail);
        }

        private void Act_load_data_feedback_done(string s_data)
        {
            this.carrot.hide_loading();
            Fire_Collection fc = new(s_data);
            if (fc.is_null) return;

            IDictionary app = fc.fire_document[0].Get_IDictionary();
            IList rates;
            if (app["rates"] != null) rates = (IList)app["rates"];
            else rates = (IList)Json.Deserialize("[]");

            this.index_star_feedback++;
            Carrot_Rate_data rate = new();
            rate.comment = this.inp_review_feedback.text;
            rate.star = this.index_star_feedback.ToString();
            rate.date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            Carrot_Rate_user_data user_login = new()
            {
                name = this.carrot.user.get_data_user_login("name"),
                id = this.carrot.user.get_id_user_login(),
                lang = this.carrot.user.get_lang_user_login(),
                avatar = this.carrot.user.get_data_user_login("avatar")
            };
            rate.user = user_login;

            if (this.index_rate_edit != -1)
                rates[this.index_rate_edit] = rate;
            else
                rates.Add(rate);

            app["rates"] = rates;
            IDictionary app_data = (IDictionary)Json.Deserialize(JsonConvert.SerializeObject(app));
            string s_json = this.carrot.server.Convert_IDictionary_to_json(app_data);
            this.carrot.server.Update_Field_Document("app", this.carrot.Carrotstore_AppId, "rates", s_json, Act_submit_rate_feedback_done, Act_submit_rate_feedback_fail);
        }

        private void Act_submit_rate_feedback_done(string s_data)
        {
            this.carrot.hide_loading();
            this.carrot.Show_msg(this.carrot.lang.Val("send_feedback", "Send Feedback"), this.carrot.lang.Val("rate_thanks", "Send your comments to the successful developer. Thanks for your feedback!"), Msg_Icon.Success);
        }

        private void Act_submit_rate_feedback_fail(string s_error)
        {
            this.carrot.hide_loading();
        }

        public void btn_sel_rate(int index_star)
        {
            this.index_star_feedback = index_star;
            if (this.index_star_feedback < 0) this.index_star_feedback = 0;
            for (int i = 0; i < this.img_star_feedback.Length; i++)
            {
                if (i <= index_star)
                    this.img_star_feedback[i].color = this.carrot.color_highlight;
                else
                    this.img_star_feedback[i].color = Color.white;
            }
        }

        private void Load_rate_by_user()
        {
            this.carrot.show_loading();
            StructuredQuery q = new("app");
            q.Add_where("name_en", Query_OP.EQUAL, this.carrot.Carrotstore_AppId);
            q.Add_select("rates");
            q.Set_limit(1);
            this.carrot.server.Get_doc(q.ToJson(), Act_Load_rate_by_user_done, Act_Load_rate_by_user_fail);
        }

        private void Act_Load_rate_by_user_done(string s_data)
        {
            this.carrot.hide_loading();
            Fire_Collection fc = new(s_data);

            if (fc.is_null) return;

            this.index_rate_edit = -1;
            string user_id_login = this.carrot.user.get_id_user_login();
            IDictionary data_app = fc.fire_document[0].Get_IDictionary();
            if (data_app["rates"] != null)
            {
                IList rates = (IList)data_app["rates"];
                for (int i = 0; i < rates.Count; i++)
                {
                    IDictionary rate = (IDictionary)rates[i];
                    if (rate["user"] != null)
                    {
                        IDictionary rate_user = (IDictionary)rate["user"];
                        if (rate_user["id"].ToString() == user_id_login)
                        {
                            index_rate_edit = i;
                            if (rate["comment"] != null)
                            {
                                inp_review_feedback.text = rate["comment"].ToString();
                            }

                            if (rate["star"] != null)
                            {
                                int index_star = int.Parse(rate["star"].ToString());
                                index_star--;
                                btn_sel_rate(index_star);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void Act_Load_rate_by_user_fail(string s_error)
        {
            this.carrot.hide_loading();
            this.carrot.Show_msg(s_error);
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
