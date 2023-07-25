using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Button_Item : MonoBehaviour
    {
        public Text txt_val;
        public Image img_icon;
        public Image img_bk;

        private UnityAction act;

        public void on_click()
        {
            this.act.Invoke();
        }

        public void set_act_click(UnityAction act_event)
        {
            this.act = act_event;
        }

        public void set_bk_color(Color32 color_bk)
        {
            this.img_bk.color = color_bk;
        }

        public void set_label_color(Color32 color_label)
        {
            this.img_icon.color = color_label;
            this.txt_val.color = color_label;
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

        public void set_label(string s_txt)
        {
            this.txt_val.text = s_txt;
        }
    }

}
