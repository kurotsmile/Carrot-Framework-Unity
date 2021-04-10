using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Carrot.Carrot carrot;
    void Start()
    {
        carrot.Load_Carrot();
    }

    public void show_rate()
    {
        carrot.show_rate();
    }

    public void show_share()
    {
        carrot.show_share();
    }

    public void show_login()
    {
        carrot.show_login();
    }

    public void show_msg()
    {
        carrot.show_msg("Test msg", "Haha Success!!!", Carrot.Msg_Icon.Success);
    }

    public void show_loading()
    {
        carrot.show_loading();
    }

    public void show_user_by_id()
    {
        carrot.show_user_by_id("c18d01f5ecb4c58e7cc35d0e11e3c4f7", "en");
    }

    public void show_list_carrot_app()
    {
        carrot.show_list_carrot_app();
    }
}
