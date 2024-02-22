using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Item_top_player : MonoBehaviour
    {
        public Image img_user;
        public Image img_rank;
        public Text txt_user_name;
        public Text txt_user_scores;

        private UnityAction act_click=null;

        public void click()
        {
            if (this.act_click != null) this.act_click.Invoke();
        }

        public void set_act_click(UnityAction act_click)
        {
            this.act_click = act_click;
        }
    }
}
