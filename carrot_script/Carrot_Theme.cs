using UnityEngine;
using UnityEngine.Events;

namespace Carrot
{
    public class Carrot_Theme : MonoBehaviour
    {
        private Carrot carrot;
        private Carrot_Box box_list;
        private Color32 color_ads_rewarded;
        private UnityAction<Color32> act_box_list_color_done=null;
        private bool is_ads_rewarded = false;

        [Header("Color Mix")]
        private Carrot_Box_Item item_color_preview;
        private Carrot_Box_Item item_color_r;
        private Carrot_Box_Item item_color_g;
        private Carrot_Box_Item item_color_b;

        private Color32 color_mix;

        public void on_load(Carrot cr)
        {
            this.carrot = cr;
        }

        public void show_list_theme()
        {
            this.box_list = this.carrot.Create_Box("Theme");
        }

        public Carrot_Box show_list_color(UnityAction<Color32> act_done)
        {
            this.act_box_list_color_done = act_done;
            if (this.box_list != null) this.box_list.close();
            box_list = this.carrot.Create_Box("list_color");
            box_list.set_item_size(new Vector2(50f, 50f));
            box_list.set_icon(this.carrot.sp_icon_theme_color);
            box_list.set_title(PlayerPrefs.GetString("setting_color", "Color Select"));

            Carrot_Box_Btn_Item btn_get_new_color = box_list.create_btn_menu_header(this.carrot.sp_icon_restore);
            btn_get_new_color.set_act(() => this.show_list_color(act_done));

            for (int i = 1; i <= 25; i++)
            {
                Color32 color_mix = new Color32((byte)Random.Range(5, 255), (byte)Random.Range(5, 255), (byte)Random.Range(5, 255), 255);
                var c = color_mix;
                Carrot_Box_Item item_color = box_list.create_item("item_color_" + i);
                item_color.set_icon(this.carrot.sp_icon_theme_color);
                item_color.set_title("Color " + i);
                item_color.set_tip("R:" + color_mix.r + " G:" + color_mix.g + " B:" + color_mix.b);
                item_color.img_icon.color = color_mix;

                if (this.carrot.ads.get_status_ads())
                {
                    if (i % 2 == 0)
                    {
                        Carrot_Box_Btn_Item btn_watch_ads = item_color.create_item();
                        btn_watch_ads.set_color(this.carrot.color_highlight);
                        btn_watch_ads.set_icon(this.carrot.icon_carrot_ads);
                        btn_watch_ads.set_act(() => this.act_watch_ads_get_color(color_mix));
                        item_color.set_act(() => this.act_watch_ads_get_color(color_mix));
                    }
                    else
                    {
                        item_color.set_act(() => act_select_color_item(c));
                    }
                }
                else
                {
                    item_color.set_act(() => act_select_color_item(c));
                }
            }
            return box_list;
        }

        public void act_select_color_item(Color32 color_new)
        {
            if (this.box_list != null) this.box_list.close();
            if (this.act_box_list_color_done != null) this.act_box_list_color_done(color_new);
        }

        public void act_watch_ads_get_color(Color32 color_new)
        {
            this.carrot.ads.show_ads_Rewarded();
            this.is_ads_rewarded = true;
            this.color_ads_rewarded = color_new;
        }

        public void show_mix_color(Color32 color_show,UnityAction<Color32> act_done)
        {
            this.act_box_list_color_done = act_done;
            this.color_mix = color_show;
            string s_color_mix = ColorUtility.ToHtmlStringRGBA(color_show);
            if (this.box_list != null) this.box_list.close();
            box_list = this.carrot.Create_Box("list_color");
            box_list.set_item_size(new Vector2(50f, 50f));
            box_list.set_icon(this.carrot.sp_icon_picker_color);
            box_list.set_title(PlayerPrefs.GetString("setting_color", "Color Select"));

            this.item_color_preview = this.box_list.create_item("preview");
            item_color_preview.set_icon(this.carrot.sp_icon_theme_color);
            item_color_preview.set_title("Preview");
            item_color_preview.set_tip("Color preview");
            item_color_preview.set_type(Box_Item_Type.box_value_txt);
            item_color_preview.set_val("#" + s_color_mix);
            item_color_preview.txt_val.color = this.get_color_by_string(s_color_mix);

            this.item_color_r = this.box_list.create_item("R");
            this.item_color_r.set_icon(this.carrot.sp_icon_picker_color);
            this.item_color_r.set_title("R");
            this.item_color_r.set_type(Box_Item_Type.box_value_slider);
            this.item_color_r.set_fill_color(this.carrot.color_highlight);
            this.item_color_r.slider_val.wholeNumbers = true;
            this.item_color_r.slider_val.maxValue = 255;
            this.item_color_r.slider_val.minValue = 0;
            this.item_color_r.slider_val.value = color_show.r;
            this.item_color_r.slider_val.onValueChanged.AddListener(change_val_color_mix);
            item_color_r.set_tip("Red attribute");

            this.item_color_g = this.box_list.create_item("G");
            this.item_color_g.set_icon(this.carrot.sp_icon_picker_color);
            this.item_color_g.set_title("G");
            this.item_color_g.set_type(Box_Item_Type.box_value_slider);
            this.item_color_g.set_fill_color(this.carrot.color_highlight);
            this.item_color_g.slider_val.wholeNumbers = true;
            this.item_color_g.slider_val.maxValue = 255;
            this.item_color_g.slider_val.minValue = 0;
            this.item_color_g.slider_val.value = color_show.g;
            this.item_color_g.slider_val.onValueChanged.AddListener(change_val_color_mix);
            this.item_color_g.set_tip("Green attribute");

            this.item_color_b = this.box_list.create_item("B");
            this.item_color_b.set_icon(this.carrot.sp_icon_picker_color);
            this.item_color_b.set_title("B");
            this.item_color_b.set_type(Box_Item_Type.box_value_slider);
            this.item_color_b.set_fill_color(this.carrot.color_highlight);
            this.item_color_b.slider_val.wholeNumbers = true;
            this.item_color_b.slider_val.maxValue = 255;
            this.item_color_b.slider_val.minValue = 0;
            this.item_color_b.slider_val.value = color_show.b;
            this.item_color_b.slider_val.onValueChanged.AddListener(change_val_color_mix);
            this.item_color_b.set_tip("Blue attribute");

            Carrot_Box_Btn_Panel obj_panel_btn = this.box_list.create_panel_btn();

            string s_done_label_txt = PlayerPrefs.GetString("done", "Done");
            Carrot_Button_Item obj_btn_done = obj_panel_btn.create_btn("btn_done");
            obj_btn_done.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            obj_btn_done.set_bk_color(this.carrot.color_highlight);
            obj_btn_done.set_act_click(act_done_color_mix);
            obj_btn_done.set_label_color(Color.white);
            obj_btn_done.set_label(s_done_label_txt);
            obj_btn_done.set_icon(this.carrot.icon_carrot_done);

            string s_cancel_label_txt = PlayerPrefs.GetString("cancel", "Cancel");
            Carrot_Button_Item obj_btn_cancel = obj_panel_btn.create_btn("btn_cancel");
            obj_btn_cancel.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            obj_btn_cancel.set_act_click(act_close_box);
            obj_btn_cancel.set_bk_color(this.carrot.color_highlight);
            obj_btn_cancel.set_label_color(Color.white);
            obj_btn_cancel.set_label(s_cancel_label_txt);
            obj_btn_cancel.set_icon(this.carrot.icon_carrot_cancel);
        }

        private void change_val_color_mix(float f_data)
        {
            byte r = byte.Parse(this.item_color_r.get_val());
            byte g = byte.Parse(this.item_color_g.get_val());
            byte b = byte.Parse(this.item_color_b.get_val());
            this.color_mix = new Color32(r, g, b, 255);
            string s_color_sys = ColorUtility.ToHtmlStringRGBA(this.color_mix);
            this.item_color_preview.set_val("#" + s_color_sys);
            this.item_color_preview.txt_val.color = this.color_mix;
        }

        private void act_done_color_mix()
        {
            if (this.act_box_list_color_done != null) this.act_box_list_color_done(this.color_mix);
            if (this.box_list != null) this.box_list.close();
        }

        private void act_close_box()
        {
            this.act_box_list_color_done = null;
            if (this.box_list != null) this.box_list.close();
        }

        public Color32 get_color_by_string(string s_color)
        {
            Color newCol;
            if (!s_color.Contains("#")) s_color = "#" + s_color;
            if (ColorUtility.TryParseHtmlString(s_color, out newCol))
                return newCol;
            else
                return this.carrot.color_highlight;
        }

        public void onRewardedSuccess()
        {
            if (this.is_ads_rewarded)
            {
                if (this.act_box_list_color_done != null) this.act_box_list_color_done(this.color_ads_rewarded);
                this.is_ads_rewarded = false;
            }
        }
    }
}
