using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Carrot
{
    public class Carrot_Box_Btn_Item : MonoBehaviour
    {
        public Image icon;
        private UnityAction act_click = null;
        public void click()
        {
            if (this.act_click != null) this.act_click();
        }

        public void set_act(UnityAction act_click)
        {
            this.act_click = act_click;
        }

        public void set_icon(Sprite sp)
        {
            this.icon.sprite = sp;
        }

        public void set_icon_color(Color32 color)
        {
            this.icon.color = color;
        }

        public void set_color(Color32 color)
        {
            this.GetComponent<Image>().color = color;
        }
    }
}
