using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{

    [FirestoreData]
    public struct Carrot_Rate_user_data
    {
        [FirestoreProperty]
        public string id {get;set;}
        [FirestoreProperty]
        public string name { get; set; }
        [FirestoreProperty]
        public string avatar { get; set; }
        [FirestoreProperty]
        public string lang {get;set;}
    }

    [FirestoreData]
    public struct Carrot_Rate_data
    {
        [FirestoreProperty]
        public Carrot_Rate_user_data user {get;set;}
        [FirestoreProperty]
        public string star {get;set;}
        [FirestoreProperty]
        public string comment {get;set;}
        [FirestoreProperty]
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

            CollectionReference RateDbRef = this.carrot.db.Collection("app");
            DocumentReference RateRef = RateDbRef.Document(this.carrot.Carrotstore_AppId);
            Carrot_Rate_data rate = new Carrot_Rate_data();
            rate.comment = this.inp_review_feedback.text;
            rate.star = this.index_star_feedback.ToString();
            rate.date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            Carrot_Rate_user_data user_login = new Carrot_Rate_user_data();
            user_login.name = this.carrot.user.get_data_user_login("name");
            user_login.id = this.carrot.user.get_id_user_login();
            user_login.lang = this.carrot.user.get_lang_user_login();
            user_login.avatar = this.carrot.user.get_data_user_login("avatar");
            rate.user = user_login;
            Dictionary<string, object> UpdateData = new Dictionary<string, object>{{"rates",rate}};
            RateRef.UpdateAsync(UpdateData);
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
            DocumentReference AppRateRef =this.carrot.db.Collection("app").Document(this.carrot.Carrotstore_AppId);
            AppRateRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    IDictionary data_app=snapshot.ToDictionary();
                    IList rates =(IList) data_app["rates"];
                    for(int i = 0; i < rates.Count; i++)
                    {
                        IDictionary rate =(IDictionary) rates[i];
                        IDictionary rate_user =(IDictionary) rate["user"];
                        if (rate_user["id"].ToString() == user_id_login)
                        {
                            this.inp_review_feedback.text = rate["comment"].ToString();
                            int index_star = int.Parse(rate["star"].ToString());
                            this.btn_sel_rate(index_star);
                        }
                    }
                }
            });
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