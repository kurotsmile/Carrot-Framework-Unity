using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public enum Window_Input_value_Type {input_field,slider}
    public class Carrot_Window_Input : MonoBehaviour
    {
        public Carrot_UI UI;

        public Carrot_Box box;
        public Text tip;
        public InputField inp_text;
        public Slider inp_slider;

        private UnityAction<string> act_done;
        private Carrot carrot;
        private Window_Input_value_Type type_input;
        

        public void load(Carrot cr)
        {
            this.carrot = cr;
            this.UI.set_theme(cr.color_highlight);
            this.set_inp_type(Window_Input_value_Type.input_field);
            this.GetComponent<Carrot_lang_show>().load_lang_emp(carrot.lang);
        }

        public void set_icon(Sprite sp_icon)
        {
            this.box.set_icon(sp_icon);
        }

        public void set_title(string s_title)
        {
            this.box.set_title(s_title);
        }

        public void set_tip(string s_tip)
        {
            this.tip.text = s_tip;
        }

        public void set_inp_type(Window_Input_value_Type t)
        {
            this.type_input = t;
            this.inp_text.gameObject.SetActive(false);
            this.inp_slider.gameObject.SetActive(false);

            if (t == Window_Input_value_Type.input_field) this.inp_text.gameObject.SetActive(true);
            if (t == Window_Input_value_Type.slider) this.inp_slider.gameObject.SetActive(true);
        }

        public void set_val(string s_val)
        {
            if (this.type_input == Window_Input_value_Type.input_field) this.inp_text.text = s_val;
            if (this.type_input == Window_Input_value_Type.slider) this.inp_slider.value = float.Parse(s_val);
        }

        public string get_val()
        {
            if (this.type_input == Window_Input_value_Type.input_field) return this.inp_text.text;
            if (this.type_input == Window_Input_value_Type.slider) return this.inp_slider.value.ToString();
            return this.inp_text.text;
        }

        public void inp_done_act()
        {
            string s_val = this.get_val();
            if (this.act_done != null) this.act_done(s_val);
        }

        public void set_act_done(UnityAction<string> act)
        {
            this.act_done = act;
        }

        public void close()
        {
            this.carrot.play_sound_click();
            this.UI.close();
        }

    }
}