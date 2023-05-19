using UnityEngine;
using UnityEngine.UI;
namespace Carrot
{
    public class Carrot_item_app : MonoBehaviour
    {
        public Text txt_name;
        public Image img_icon;
        public string link;
        public string link_carrot_app;

        public void click()
        {
            Application.OpenURL(this.link);
        }

        public void go_link_carrot_app()
        {
            Application.OpenURL(this.link_carrot_app);
        }

        public void share_link_carrot_app()
        {
            GameObject.Find("Carrot").GetComponent<Carrot>().show_share(this.link_carrot_app,PlayerPrefs.GetString("share_tip", "Choose the platform below to share this great app with your friends or others"));
        }
    }
}