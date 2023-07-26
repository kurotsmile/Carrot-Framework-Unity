using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Carrot
{
    public class Carrot_Window_Ads : MonoBehaviour
    {
        private Carrot carrot;
        public Carrot_UI UI;
        [Header("Ads Obj")]
        public Slider slider_timer_ads;
        public Image pic_img_ads;
        private bool is_show_ads = false;
        public GameObject button_close_ads;
        public Text txt_ads_product_title;
        public Text txt_ads_product_tip;
        public string url_ads_download;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            this.UI.set_theme(this.carrot.color_highlight);
        }

        public void load_data_ads(IDictionary data_ads)
        {
            this.pic_img_ads.GetComponent<Animator>().enabled = true;
            this.is_show_ads = true;
            this.slider_timer_ads.value = 0;
            this.button_close_ads.SetActive(false);
            this.txt_ads_product_title.text = data_ads["name_"+this.carrot.lang.get_key_lang()].ToString();
            this.txt_ads_product_tip.text = data_ads["describe_"+this.carrot.lang.get_key_lang()].ToString();
            this.url_ads_download = data_ads[this.carrot.store_public.ToString().ToLower()].ToString();
            Sprite icon_app = this.carrot.get_tool().get_sprite_to_playerPrefs(data_ads["id"].ToString());
            if (icon_app != null)
            {
                this.pic_img_ads.sprite = icon_app;
                this.pic_img_ads.color = Color.white;
            }
            else
            {
                this.carrot.get_img(data_ads["icon"].ToString(), this.pic_img_ads);
            }     
        }

        private void Update()
        {
            if (this.is_show_ads)
            {
                this.slider_timer_ads.value += 0.1f * Time.deltaTime;
                if (this.slider_timer_ads.value >= 1f)
                {
                    this.name = "window_ads_doe";
                    this.pic_img_ads.GetComponent<Animator>().enabled = false;
                    this.button_close_ads.SetActive(true);
                    this.is_show_ads = false;
                    if(this.carrot.type_control!=TypeControl.None) this.carrot.game.set_list_button_gamepad_console(this.UI.get_list_btn());
                }
            }
        }

        public void btn_ads_download_app()
        {
            Application.OpenURL(this.url_ads_download);
        }

        public void btn_remove_ads()
        {
            this.carrot.buy_inapp_removeads();
        }

        public void close()
        {
            this.carrot.play_sound_click();
            this.UI.close();
        }
    }
}
