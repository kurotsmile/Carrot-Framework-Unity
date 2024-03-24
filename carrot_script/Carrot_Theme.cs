using UnityEngine;
using UnityEngine.Events;

namespace Carrot
{
    public enum Carrot_Theme_List_Type {list,grid,mix}

    public class Carrot_Theme : MonoBehaviour
    {
        private Carrot carrot;
        private Carrot_Box box_list;
        private Color32 color_ads_rewarded;
        private UnityAction<Color32> Act_done=null;
        private bool is_ads_rewarded = false;

        [Header("Color Mix")]
        private Carrot_Box_Item item_color_preview;
        private Carrot_Box_Item item_color_r;
        private Carrot_Box_Item item_color_g;
        private Carrot_Box_Item item_color_b;

        private Color color_mix;
        private Carrot_Theme_List_Type type = Carrot_Theme_List_Type.list;

        public void On_load(Carrot cr)
        {
            this.carrot = cr;

            string theme_box_color_type = PlayerPrefs.GetString("theme_box_color_type", "");
            if (theme_box_color_type == "list") this.type = Carrot_Theme_List_Type.list;
            if (theme_box_color_type == "grid") this.type = Carrot_Theme_List_Type.grid;
            if (theme_box_color_type == "mix") this.type = Carrot_Theme_List_Type.mix;
        }

        public void show_list_theme()
        {
            this.box_list = this.carrot.Create_Box("Theme");
        }

        public Carrot_Box Show_box_change_color(UnityAction<Color32> act_done,string s_color_default="#ffffff")
        {
            if (this.type == Carrot_Theme_List_Type.list)
                return Show_box_list_item_color(act_done);
            else if (this.type == Carrot_Theme_List_Type.grid)
                return Show_box_grid_item_color(act_done);
            else if(this.type==Carrot_Theme_List_Type.mix)
                return Show_mix_color(act_done, s_color_default);
            else
                return Show_mix_color(act_done, s_color_default);
        }

        public Carrot_Box Show_box_list_item_color(UnityAction<Color32> act_done)
        {
            this.Act_done = act_done;
            if (this.box_list != null) this.box_list.close();
            box_list = this.carrot.Create_Box("list_color");
            box_list.set_item_size(new Vector2(50f, 50f));
            box_list.set_icon(this.carrot.sp_icon_picker_color);
            box_list.set_title(this.carrot.lang.Val("setting_color", "Color Select"));

            this.Header_menu_box(this.box_list);

            for (int i = 1; i <= 25; i++)
            {
                Color32 color_mix = new((byte)Random.Range(5, 255), (byte)Random.Range(5, 255), (byte)Random.Range(5, 255), 255);
                var c = color_mix;
                Carrot_Box_Item item_color = box_list.create_item("item_color_" + i);
                item_color.set_icon(this.carrot.sp_icon_theme_color);
                item_color.set_title("Color " + i);
                item_color.set_tip(color_mix.ToString()+" - #"+ColorUtility.ToHtmlStringRGBA(color_mix));
                item_color.img_icon.color = color_mix;

                if (this.carrot.ads.get_status_ads())
                {
                    if (i % 2 == 0)
                    {
                        Carrot_Box_Btn_Item btn_watch_ads = item_color.create_item();
                        btn_watch_ads.set_color(this.carrot.color_highlight);
                        btn_watch_ads.set_icon(this.carrot.icon_carrot_ads);
                        btn_watch_ads.set_act(() => this.Act_watch_ads_get_color(color_mix));
                        item_color.set_act(() => this.Act_watch_ads_get_color(color_mix));
                    }
                    else
                    {
                        item_color.set_act(() => Act_select_color_item(c));
                    }
                }
                else
                {
                    item_color.set_act(() => Act_select_color_item(c));
                }
            }
            return box_list;
        }

        public Carrot_Box Show_box_grid_item_color(UnityAction<Color32> act_done)
        {
            this.Act_done = act_done;
            if (this.box_list != null) this.box_list.close();
            box_list = this.carrot.show_grid();
            box_list.set_item_size(new Vector2(60f, 60f));
            box_list.set_icon(this.carrot.sp_icon_picker_color);
            box_list.set_title(this.carrot.lang.Val("setting_color", "Color Select"));
            box_list.Set_grid_col(7);
             
            this.Header_menu_box(box_list);

            int step = 35;
            int max_val = 220;

            for (int i = max_val; i > 0; i -= step) Create_item_grid_color(new Color32(255, (byte)i, (byte)i, 255));
            for (int i = max_val; i > 0; i -= step) Create_item_grid_color(new Color32((byte)i,255, (byte)i, 255));
            for (int i = max_val; i > 0; i -= step) Create_item_grid_color(new Color32((byte)i, (byte)i,255, 255));
            for (int i = max_val; i > 0; i -= step) Create_item_grid_color(new Color32(255,255, (byte)i, 255));
            for (int i = max_val; i > 0; i -= step) Create_item_grid_color(new Color32((byte)i, 255,255, 255));
            for (int i = max_val; i > 0; i -= step) Create_item_grid_color(new Color32(255, (byte)i, 255, 255));
            for (int i = max_val; i > 0; i -= step) Create_item_grid_color(new Color32((byte)i, (byte)i, (byte)i, 255));

            return box_list;
        }

        private void Create_item_grid_color(Color32 color_mix)
        {
            Carrot_Box_Item item_color = box_list.create_item("item_color");
            item_color.set_icon(null);
            item_color.img_icon.color = color_mix;
            item_color.set_act(() => this.Act_select_color_item(color_mix));
        }

        private void Act_select_color_item(Color32 color_new)
        {
            if (this.box_list != null) this.box_list.close();
            this.Act_done?.Invoke(color_new);
        }

        private void Act_watch_ads_get_color(Color32 color_new)
        {
            this.carrot.ads.show_ads_Rewarded();
            this.is_ads_rewarded = true;
            this.color_ads_rewarded = color_new;
        }

        public Carrot_Box Show_mix_color(UnityAction<Color32> act_done,string s_color_default)
        {
            this.Act_done = act_done;
            string s_color_mix = s_color_default;
            ColorUtility.TryParseHtmlString(s_color_mix,out this.color_mix);
            if (this.box_list != null) this.box_list.close();
            box_list = this.carrot.Create_Box("list_color");
            box_list.set_item_size(new Vector2(50f, 50f));
            box_list.set_icon(this.carrot.sp_icon_picker_color);
            box_list.set_title(this.carrot.lang.Val("setting_color", "Color Select"));

            this.Header_menu_box(box_list);

            this.item_color_preview = this.box_list.create_item("preview");
            item_color_preview.set_icon(this.carrot.sp_icon_theme_color);
            item_color_preview.set_title("Preview");
            item_color_preview.set_tip("Color preview");
            item_color_preview.set_type(Box_Item_Type.box_value_txt);
            item_color_preview.set_val("#" + s_color_mix);
            item_color_preview.txt_val.color = this.Get_color_by_string(s_color_mix);

            this.item_color_r = this.box_list.create_item("R");
            this.item_color_r.set_icon(this.carrot.sp_icon_picker_color);
            this.item_color_r.set_title("R");
            this.item_color_r.set_type(Box_Item_Type.box_value_slider);
            this.item_color_r.set_fill_color(this.carrot.color_highlight);
            this.item_color_r.slider_val.wholeNumbers = true;
            this.item_color_r.slider_val.maxValue = 255;
            this.item_color_r.slider_val.minValue = 0;
            this.item_color_r.slider_val.value = color_mix.r;
            this.item_color_r.slider_val.onValueChanged.AddListener(Change_val_color_mix);
            item_color_r.set_tip("Red attribute");

            this.item_color_g = this.box_list.create_item("G");
            this.item_color_g.set_icon(this.carrot.sp_icon_picker_color);
            this.item_color_g.set_title("G");
            this.item_color_g.set_type(Box_Item_Type.box_value_slider);
            this.item_color_g.set_fill_color(this.carrot.color_highlight);
            this.item_color_g.slider_val.wholeNumbers = true;
            this.item_color_g.slider_val.maxValue = 255;
            this.item_color_g.slider_val.minValue = 0;
            this.item_color_g.slider_val.value = color_mix.g;
            this.item_color_g.slider_val.onValueChanged.AddListener(Change_val_color_mix);
            this.item_color_g.set_tip("Green attribute");

            this.item_color_b = this.box_list.create_item("B");
            this.item_color_b.set_icon(this.carrot.sp_icon_picker_color);
            this.item_color_b.set_title("B");
            this.item_color_b.set_type(Box_Item_Type.box_value_slider);
            this.item_color_b.set_fill_color(this.carrot.color_highlight);
            this.item_color_b.slider_val.wholeNumbers = true;
            this.item_color_b.slider_val.maxValue = 255;
            this.item_color_b.slider_val.minValue = 0;
            this.item_color_b.slider_val.value = color_mix.b;
            this.item_color_b.slider_val.onValueChanged.AddListener(Change_val_color_mix);
            this.item_color_b.set_tip("Blue attribute");

            Carrot_Box_Btn_Panel obj_panel_btn = this.box_list.create_panel_btn();

            string s_done_label_txt = this.carrot.lang.Val("done", "Done");
            Carrot_Button_Item obj_btn_done = obj_panel_btn.create_btn("btn_done");
            obj_btn_done.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            obj_btn_done.set_bk_color(this.carrot.color_highlight);
            obj_btn_done.set_act_click(Act_done_color_mix);
            obj_btn_done.set_label_color(Color.white);
            obj_btn_done.set_label(s_done_label_txt);
            obj_btn_done.set_icon(this.carrot.icon_carrot_done);

            string s_cancel_label_txt = this.carrot.lang.Val("cancel", "Cancel");
            Carrot_Button_Item obj_btn_cancel = obj_panel_btn.create_btn("btn_cancel");
            obj_btn_cancel.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            obj_btn_cancel.set_act_click(act_close_box);
            obj_btn_cancel.set_bk_color(this.carrot.color_highlight);
            obj_btn_cancel.set_label_color(Color.white);
            obj_btn_cancel.set_label(s_cancel_label_txt);
            obj_btn_cancel.set_icon(this.carrot.icon_carrot_cancel);
            return box_list;
        }

        private void Change_val_color_mix(float f_data)
        {
            byte r = byte.Parse(this.item_color_r.get_val());
            byte g = byte.Parse(this.item_color_g.get_val());
            byte b = byte.Parse(this.item_color_b.get_val());
            this.color_mix = new Color32(r, g, b, 255);
            string s_color_sys = ColorUtility.ToHtmlStringRGBA(this.color_mix);
            this.item_color_preview.set_val("#" + s_color_sys);
            this.item_color_preview.txt_val.color = this.color_mix;
        }

        private void Act_done_color_mix()
        {
            this.Act_done?.Invoke(this.color_mix);
            if (this.box_list != null) this.box_list.close();
        }

        private void act_close_box()
        {
            this.Act_done = null;
            if (this.box_list != null) this.box_list.close();
        }

        public Color32 Get_color_by_string(string s_color)
        {
            Color newCol;
            if (!s_color.Contains("#")) s_color = "#" + s_color;
            if (ColorUtility.TryParseHtmlString(s_color, out newCol))
                return newCol;
            else
                return this.carrot.color_highlight;
        }

        public void OnRewardedSuccess()
        {
            if (this.is_ads_rewarded)
            {
                this.Act_done?.Invoke(this.color_ads_rewarded);
                this.is_ads_rewarded = false;
            }
        }

        private void Header_menu_box(Carrot_Box box)
        {
            Carrot_Box_Btn_Item btn_get_new_color = box_list.create_btn_menu_header(this.carrot.sp_icon_restore,false);
            btn_get_new_color.set_act(() =>this.Show_box_change_color(this.Act_done));

            Carrot_Box_Btn_Item btn_menu_list = box.create_btn_menu_header(this.carrot.sp_icon_theme_color);
            if (this.type == Carrot_Theme_List_Type.list) btn_menu_list.set_icon_color(carrot.color_highlight);
            btn_menu_list.set_act(() => this.Act_head_btn_change_type_show( Carrot_Theme_List_Type.list));

            Carrot_Box_Btn_Item btn_menu_mixer = box.create_btn_menu_header(this.carrot.sp_icon_mixer_color);
            if (this.type == Carrot_Theme_List_Type.mix) btn_menu_mixer.set_icon_color(carrot.color_highlight);
            btn_menu_mixer.set_act(() => this.Act_head_btn_change_type_show(Carrot_Theme_List_Type.mix));

            Carrot_Box_Btn_Item btn_menu_table = box.create_btn_menu_header(this.carrot.sp_icon_table_color);
            if (this.type == Carrot_Theme_List_Type.grid) btn_menu_table.set_icon_color(carrot.color_highlight);
            btn_menu_table.set_act(() => this.Act_head_btn_change_type_show(Carrot_Theme_List_Type.grid));
        }

        private void Act_head_btn_change_type_show(Carrot_Theme_List_Type type)
        {
            this.type = type;
            PlayerPrefs.SetString("theme_box_color_type", this.type.ToString());
            this.Show_box_change_color(this.Act_done, "#FFFFFF");
        }
    }
}
