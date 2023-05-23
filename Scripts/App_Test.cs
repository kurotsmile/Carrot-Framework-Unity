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
}
