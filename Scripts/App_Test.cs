using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App_Test : MonoBehaviour
{

    public Carrot.Carrot carrot;

    void Start()
    {
        this.carrot.Load_Carrot();
    }

    public void bnt_camera()
    {
        this.carrot.camera_pro.show_camera();
    }

    public void btn_box_list()
    {
        Carrot.Carrot_Box box_list = this.carrot.Create_Box("Box List");

        for(int i = 0; i < 10; i++)
        {
            Carrot.Carrot_Box_Item box_item = box_list.create_item("item_" + i);
            box_item.set_title("Box item " + i);
            box_item.set_tip("Box tip " + i);
        }
    }

    public void btn_list_photo_camera()
    {
        this.carrot.camera_pro.show_list_img(null);
    }

    public void btn_get_location()
    {
        this.carrot.location.get_location(act_get_location_success);
    }

    private void act_get_location_success(LocationInfo info)
    {
        this.carrot.show_msg("Get Location", "latitude:" + info.latitude+ ",longitude:" + info.longitude, Carrot.Msg_Icon.Success);
    }

    public void btn_setting()
    {
        this.carrot.Create_Setting();
    }

    public void btn_msg()
    {
        this.carrot.show_msg("App test", "Test Msg",Carrot.Msg_Icon.Alert);
    }

    public void btn_list_color()
    {
        this.carrot.theme.show_list_color(act_sel_color);
    }

    private void act_sel_color(Color32 col)
    {
        this.carrot.show_msg("Color select:" + col.ToString());
    }

    public void btn_mix_color()
    {
        this.carrot.theme.show_mix_color(this.carrot.color_highlight,act_sel_color);
    }

    public void btn_list_app_other()
    {
        this.carrot.show_list_carrot_app();
    }

    public void btn_rate()
    {
        this.carrot.show_rate();
    }

    public void btn_share()
    {
        this.carrot.show_share();
    }

    public void btn_search()
    {
        this.carrot.show_search(null);
    }

    public void btn_login()
    {
        this.carrot.show_login();
    }

    public void btn_register()
    {
        this.carrot.user.show_register();
    }

    public void btn_list_lang()
    {
        this.carrot.show_list_lang();
    }

    public void btn_loading()
    {
        this.carrot.show_loading();
    }
}
