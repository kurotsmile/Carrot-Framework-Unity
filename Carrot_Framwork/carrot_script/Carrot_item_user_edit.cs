using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Carrot
{
    public class Carrot_item_user_edit : MonoBehaviour
    {
        public string s_name;
        public string s_type;
        public Text txt_title;
        public InputField inp_val;
        public Dropdown dropdown_val;
        public GameObject button_delete;
        public Image img_view_pass;
        public string val_select;
        public string val_select_en;

        [Header("Avatar Obj")]
        public GameObject panel_avatar;
        public Image img_avatar;

        [Header("Icon")]
        public Sprite icon_view_pass;
        public Sprite icon_no_view_pass;

        private void hide_all_emp()
        {
            this.inp_val.gameObject.SetActive(false);
            this.dropdown_val.gameObject.SetActive(false);
            this.panel_avatar.SetActive(false);
            this.img_view_pass.gameObject.SetActive(false);
        }

        public void set_data(string s_type_field, string s_val)
        {
            this.hide_all_emp();
            this.inp_val.placeholder.GetComponent<Text>().text = PlayerPrefs.GetString("inp_tip", "Enter text...");
            this.s_type = s_type_field;
            if (this.s_type == "2")
            {
                //For select down
            }
            else if (this.s_type == "4")
            {
                this.inp_val.gameObject.SetActive(true);
                this.inp_val.contentType = InputField.ContentType.DecimalNumber;
                this.inp_val.text = s_val;
            }
            else if (this.s_type == "5")
            {
                this.inp_val.gameObject.SetActive(true);
                this.inp_val.contentType = InputField.ContentType.EmailAddress;
                this.inp_val.text = s_val;
            }
            else if (this.s_type == "3")
            {
                this.inp_val.gameObject.SetActive(true);
                this.inp_val.contentType = InputField.ContentType.Password;
                this.inp_val.text = s_val;
                this.img_view_pass.gameObject.SetActive(true);
            }
            else if (this.s_type == "6")
            {
                this.panel_avatar.SetActive(true);
            }
            else if (this.s_type == "8")
            {
                this.inp_val.gameObject.SetActive(true);
                this.inp_val.contentType = InputField.ContentType.DecimalNumber;
                this.inp_val.text = s_val;
            }
            else
            {
                this.inp_val.gameObject.SetActive(true);
                this.inp_val.contentType = InputField.ContentType.Standard;
                this.inp_val.text = s_val;
            }
        }

        public void set_data_select_down(string s_data_val, string s_val_en, string s_val)
        {
            this.hide_all_emp();
            this.s_type = "2";
            this.dropdown_val.gameObject.SetActive(true);
            this.val_select = s_data_val;
            this.val_select_en = s_val_en;
            IList list_data_val = (IList)Json.Deserialize(s_data_val);
            IList list_data_val_en = (IList)Json.Deserialize(s_val_en);
            dropdown_val.ClearOptions();
            for (int i = 0; i < list_data_val.Count; i++)
            {
                dropdown_val.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString(list_data_val[i].ToString(), list_data_val_en[i].ToString()) });
            }
            this.dropdown_val.value = int.Parse(s_val);
            this.dropdown_val.RefreshShownValue();
        }

        public string get_val()
        {
            if (this.s_type == "2")
                return this.dropdown_val.value.ToString();
            else
                return this.inp_val.text;
        }

        public void btn_show_camera()
        {
            GameObject.Find("Carrot").GetComponent<Carrot>().camera_pro.show_camera(done_camera);
        }

        private void done_camera(Texture2D photo)
        {
            img_avatar.sprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0, 0));
            img_avatar.color = Color.white;
            GameObject.Find("Carrot").GetComponent<Carrot>().user.set_data_field_avatar_for_update_user(photo);
        }

        public void btn_view_pass()
        {
            if (this.img_view_pass.sprite == this.icon_view_pass)
            {
                this.img_view_pass.sprite = this.icon_no_view_pass;
                this.inp_val.contentType = InputField.ContentType.Standard;
            }
            else
            {
                this.img_view_pass.sprite = this.icon_view_pass;
                this.inp_val.contentType = InputField.ContentType.Password;
            }
            this.inp_val.gameObject.SetActive(false);
            this.inp_val.gameObject.SetActive(true);
        }

        public void btn_delete()
        {
            Destroy(this.gameObject);
        }
    }
}
