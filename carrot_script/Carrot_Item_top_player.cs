using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Item_top_player : MonoBehaviour
    {
        public Image img_user;
        public Image img_rank;
        public Text txt_user_name;
        public Text txt_user_scores;
        public string user_id;
        public string user_lang;

        public void click()
        {
            GameObject.Find("Carrot").GetComponent<Carrot>().user.show_user_by_id(this.user_id, this.user_lang);
        }
    }
}
