using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_lang_show : MonoBehaviour
    {
        public string[] key;
        public Text[] emp;
        [Tooltip("Image for country")]
        public Image[] List_img_change_icon_lang;

        public void load_lang_emp(Carrot_lang lang)
        {
            for (int i = 0; i < this.emp.Length; i++) 
                if (lang.Val(this.key[i])!= "") this.emp[i].text = lang.Val(this.key[i]);
        }

        public void load_lang_emp(Sprite sp_lang_cur,Carrot_lang lang)
        {
            this.load_lang_emp(lang);
            for (int i = 0; i < this.List_img_change_icon_lang.Length; i++)
            {
                List_img_change_icon_lang[i].sprite = sp_lang_cur;
            }
        }
    }


}
