using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public enum Box_Item_Type {
        box_nomal,
        box_value_txt,
        box_value_input,
        box_number_input,
        box_email_input,
        box_password_input,
        box_value_slider,
        box_value_dropdown
    }

    public class Carrot_Box_Item : MonoBehaviour
    {
        public Image img_icon;
        public Image img_fill_bar;
        public Text txt_name;
        public Text txt_tip;
        public Text txt_val;
        public Text txt_placeholder_tip;
        public InputField inp_val;
        public Dropdown dropdown_val;
        public Slider slider_val;
        public Transform area_all_btn_extension;
        public GameObject item_extension_prefab;

        private Carrot carrot;
        private UnityAction act_click=null;
        private string s_lang_title;
        private string s_lang_tip;
        private bool is_change = false;
        private Box_Item_Type type=Box_Item_Type.box_nomal;
        private Carrot_Box_Btn_Item btn_password_visible;

        public void on_load(Carrot cr)
        {
            this.carrot = cr;
        }

        public void check_type()
        {
            RectTransform r = this.transform.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0f, 1f);
            r.pivot = new Vector2(0.5f, 1f);
            r.offsetMin = new Vector2(0, r.offsetMin.y);
            r.offsetMax = new Vector2(0, r.offsetMax.y);
            r.offsetMax = new Vector2(r.offsetMax.x, 0f);

            if (type == Box_Item_Type.box_nomal)
            {
                this.txt_val.gameObject.SetActive(false);
                this.inp_val.gameObject.SetActive(false);
                this.dropdown_val.gameObject.SetActive(false);
                this.slider_val.gameObject.SetActive(false);
                r.offsetMin = new Vector2(r.offsetMin.x, -50.0f);
            }
            else
            {
                if(type==Box_Item_Type.box_value_txt)this.txt_val.gameObject.SetActive(true);
                if (type == Box_Item_Type.box_value_input)
                {
                    this.inp_val.gameObject.SetActive(true);
                    this.txt_placeholder_tip.text =this.carrot.lang.Val("inp_tip", "Enter your data here ...");
                    Destroy(this.GetComponent<Button>());
                }

                if (type == Box_Item_Type.box_number_input)
                {
                    this.inp_val.gameObject.SetActive(true);
                    this.inp_val.contentType = InputField.ContentType.IntegerNumber;
                    this.txt_placeholder_tip.text = this.carrot.lang.Val("inp_tip", "Enter your data here ...");
                    Destroy(this.GetComponent<Button>());
                }

                if (type == Box_Item_Type.box_email_input)
                {
                    this.inp_val.gameObject.SetActive(true);
                    this.inp_val.contentType = InputField.ContentType.EmailAddress;
                    this.txt_placeholder_tip.text = this.carrot.lang.Val("inp_tip", "Enter your data here ...");
                    Destroy(this.GetComponent<Button>());
                }

                if (type == Box_Item_Type.box_password_input)
                {
                    this.inp_val.gameObject.SetActive(true);
                    this.inp_val.contentType = InputField.ContentType.Password;
                    this.txt_placeholder_tip.text = this.carrot.lang.Val("inp_tip", "Enter your data here ...");
                    if (btn_password_visible != null) Destroy(btn_password_visible.gameObject);
                    btn_password_visible = this.create_item();
                    btn_password_visible.set_icon(this.carrot.icon_carrot_visible_off);
                    btn_password_visible.set_color(this.carrot.color_highlight);
                    btn_password_visible.set_act(this.on_or_off_visible_password);
                    Destroy(this.GetComponent<Button>());
                }

                if (type == Box_Item_Type.box_value_dropdown)
                {
                    this.dropdown_val.gameObject.SetActive(true);
                    Destroy(this.GetComponent<Button>());
                }

                if(type==Box_Item_Type.box_value_slider){
                    this.slider_val.gameObject.SetActive(true);
                    Destroy(this.GetComponent<Button>());
                }
                r.offsetMin = new Vector2(r.offsetMin.x, -100.0f);
            }
        }

        public void click()
        {
            if(this.act_click!=null) act_click();
        }

        public void set_title(string s_title)
        {
            this.txt_name.text = s_title;
        }

        public void set_tip(string s_tip)
        {
            this.txt_tip.text = s_tip;
        }

        public void set_key_lang_title(string s_key_lang)
        {
            this.s_lang_title = s_key_lang;
        }

        public void set_key_lang_tip(string s_key_lang)
        {
            this.s_lang_tip = s_key_lang;
        }

        public void set_lang_data(string s_key_title,string s_key_tip)
        {
            this.s_lang_title = s_key_title;
            this.s_lang_tip = s_key_tip;
        }

        public void load_lang_data()
        {
            if (this.s_lang_title != "")
            {
                string lang_title = carrot.lang.Val(this.s_lang_title);
                if (lang_title != "") this.txt_name.text = lang_title;
            }

            if (this.s_lang_tip != "")
            {
                string lang_tip = carrot.lang.Val(this.s_lang_tip);
                if (lang_tip != "") this.txt_tip.text = lang_tip;
            }
        }

        public void set_icon(Sprite sp_icon)
        {
            this.img_icon.sprite = sp_icon;
        }

        public void set_icon_white(Sprite sp_icon)
        {
            this.img_icon.sprite = sp_icon;
            this.img_icon.color = Color.white;
        }

        public void set_act(UnityAction act)
        {
            this.act_click = act;
        }

        public GameObject add_item_extension(GameObject item_obj)
        {
            GameObject item_box = Instantiate(item_obj);
            item_box.transform.SetParent(this.area_all_btn_extension);
            item_box.transform.localPosition = new Vector3(item_box.transform.position.x, item_box.transform.position.y, 0f);
            item_box.transform.localScale = new Vector3(1f, 1f, 1f);
            item_box.transform.localRotation = Quaternion.Euler(Vector3.zero);
            return item_box;
        }

        public Carrot_Box_Btn_Item create_item()
        {
            return this.add_item_extension(this.item_extension_prefab).GetComponent<Carrot_Box_Btn_Item>();
        }

        public bool get_change_status()
        {
            return this.is_change;
        }

        public void set_change_status(bool is_update)
        {
            this.is_change = is_update;
        }

        public UnityAction get_act_click()
        {
            return this.act_click;
        }

        public void set_type(Box_Item_Type type_set)
        {
            this.type = type_set;
            this.check_type();
        }

        public void set_type(string s_type)
        {
            if (s_type == "1") this.type = Box_Item_Type.box_value_input;
            if (s_type == "3") this.type = Box_Item_Type.box_password_input;
            if (s_type == "4") this.type = Box_Item_Type.box_number_input;
            if (s_type == "5") this.type = Box_Item_Type.box_email_input;
            if (s_type == "2") this.type = Box_Item_Type.box_value_dropdown;
            this.check_type();
        }

        public void set_val(string s_val)
        {
            if(this.type==Box_Item_Type.box_value_txt) this.txt_val.text = s_val;
            if(this.type==Box_Item_Type.box_value_input) this.inp_val.text = s_val;
            if(this.type==Box_Item_Type.box_password_input) this.inp_val.text = s_val;
            if(this.type==Box_Item_Type.box_number_input) this.inp_val.text = s_val;
            if(this.type==Box_Item_Type.box_email_input) this.inp_val.text = s_val;
            if (this.type == Box_Item_Type.box_value_dropdown) this.dropdown_val.value = int.Parse(s_val);
            if (this.type == Box_Item_Type.box_value_slider) this.slider_val.value = int.Parse(s_val);
        }

        public string get_val()
        {
            if (this.type == Box_Item_Type.box_value_txt) return this.txt_val.text;
            if (this.type == Box_Item_Type.box_value_input) return this.inp_val.text;
            if (this.type == Box_Item_Type.box_password_input) return this.inp_val.text;
            if (this.type == Box_Item_Type.box_number_input) return this.inp_val.text;
            if (this.type == Box_Item_Type.box_email_input) return this.inp_val.text;
            if (this.type == Box_Item_Type.box_value_dropdown) return this.dropdown_val.value.ToString();
            if (this.type == Box_Item_Type.box_value_slider) return this.slider_val.value.ToString();
            return "";
        }

        public void set_fill_color(Color32 color_fill)
        {
            this.img_fill_bar.color = color_fill;
        }

        public void on_or_off_visible_password()
        {
            if (this.inp_val.contentType == InputField.ContentType.Password)
            {
                this.inp_val.contentType = InputField.ContentType.Standard;
                this.btn_password_visible.set_icon(this.carrot.icon_carrot_visible_on);
            }
            else
            {
                this.inp_val.contentType = InputField.ContentType.Password;
                this.btn_password_visible.set_icon(this.carrot.icon_carrot_visible_off);
            } 
        }

        public void set_val_dropdown(IList list_val, IList list_en,int index_sel)
        {
            if (this.type == Box_Item_Type.box_value_dropdown)
            {
                this.dropdown_val.ClearOptions();
                for(int i=0;i< list_val.Count; i++)
                {
                    string val =list_val[i].ToString();
                    string val_en = list_en[i].ToString();
                    string s_val = PlayerPrefs.GetString(val, val_en);
                    this.dropdown_val.options.Add(new Dropdown.OptionData(s_val));
                }
                this.dropdown_val.value = index_sel;
                this.dropdown_val.RefreshShownValue();
            }
        }
    }
}
