using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace Carrot
{
    interface Carrot_shop_event
    {
        void Carrot_by_success(string s_id_product);
        void Carrot_restore_success(string[] arr_id);
    }

    public class Carrot_shop : MonoBehaviour, IStoreListener
    {
        IStoreController m_StoreController;
        IExtensionProvider extensions;

        private List<string> list_id_product;
        private Carrot carrot;
        public UnityAction<string> onCarrotPaySuccess;
        public UnityAction<string[]> onCarrotRestoreSuccess;

        private IList list_product;
        private Carrot_Box box_shop;

        private string user_id_pay = "";
        private string product_id_pay= "";
        private string order_id_pay = "";
        private string order_type_pay = "";

        public void On_load(Carrot carrot)
        {
            this.carrot = carrot;
            list_id_product = new List<string>();

            if (this.carrot.pay_app == PayApp.UnitySDKPay)
            {
                var catalog = ProductCatalog.LoadDefaultCatalog();


                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                if (catalog.allProducts.Count > 0)
                {
                    foreach (var product in catalog.allProducts)
                    {
                        builder.AddProduct(product.id, product.type);
                        this.list_id_product.Add(product.id);
                    }
                    UnityPurchasing.Initialize(this, builder);
                }
            }

            if (this.carrot.pay_app == PayApp.CarrotPay)
            {
                var asset = Resources.Load("IAPProductCatalog") as TextAsset;
                Debug.Log(asset);
                IDictionary data_inapp = (IDictionary)Json.Deserialize(asset.text);
                list_product = (IList)data_inapp["products"];

                foreach (IDictionary product in list_product)
                {
                    this.list_id_product.Add(product["id"].ToString());
                }
            }
        }

        public void buy_product(int index_p)
        {
            this.carrot.play_sound_click();
            if (this.carrot.pay_app == PayApp.CarrotPay)
            {
                this.Check_login_and_buy_product_paypal((IDictionary)this.list_product[index_p]);
            }
            else
            {
                this.carrot.show_loading();
                m_StoreController.InitiatePurchase(this.list_id_product[index_p]);
            }
        }

        public void restore_product()
        {
            this.carrot.play_sound_click();
            if (this.carrot.pay_app == PayApp.CarrotPay)
                this.Restrore_Carrot_pay();
            else
                this.act_restore_unity_pay();
        }

        #region Unity_Pay
        private void act_restore_unity_pay()
        {
            if (Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerARM)
            {
                extensions.GetExtension<IMicrosoftExtensions>().RestoreTransactions();
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS)
            {

            }
            else if (Application.platform == RuntimePlatform.Android && StandardPurchasingModule.Instance().appStore == AppStore.GooglePlay)
            {

            }
            else
            {
                this.carrot.log(Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button");
            }
        }


        void OnTransactionsRestored(bool success)
        {
            if (success)
                this.carrot.Show_msg(this.carrot.lang.Val("shop", "Shop"), this.carrot.lang.Val("shop_restore_success", "Successful recovery!"), Msg_Icon.Success);
            else
                this.carrot.Show_msg(this.carrot.lang.Val("shop", "Shop"), this.carrot.lang.Val("shop_restore_fail", "Restore failed!"), Msg_Icon.Error);
        }

        public string get_id_by_index(int index)
        {
            return this.list_id_product[index];
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            this.carrot.log($"In-App Purchasing initialize failed: {error}");
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args)
        {
            var product = args.purchasedProduct;
            this.carrot.hide_loading();
            this.onCarrotPaySuccess.Invoke(product.definition.id);
            this.carrot.log($"Purchase Complete - Product: {product.definition.id}");
            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            this.carrot.hide_loading();
            this.carrot.Show_msg(this.carrot.lang.Val("shop", "Shop"), this.carrot.lang.Val("shop_buy_fail", "Purchase failed, Please check your account balance, or try again at another time"), Msg_Icon.Error);
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider exten)
        {
            this.extensions = exten;
            this.carrot.log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Carrot_Paypal
        private void Check_login_and_buy_product_paypal(IDictionary data_product)
        {
            this.order_id_pay = "order" + carrot.generateID();

            string user_id = carrot.user.get_id_user_login();
            string user_lang = carrot.lang.Get_key_lang();
            string user_name = "";
           
            if (user_id != "")
            {
                user_lang = carrot.user.get_lang_user_login();
                user_name = carrot.user.get_data_user_login("name");
            }  
            else
                user_id = SystemInfo.deviceUniqueIdentifier;

            this.user_id_pay = user_id;
            this.product_id_pay = data_product["id"].ToString();
            this.order_type_pay = data_product["type"].ToString();

            IDictionary defaultDescription = (IDictionary)data_product["defaultDescription"];
            IDictionary googlePrice = (IDictionary)data_product["googlePrice"];
            string price_product = googlePrice["num"].ToString();
            price_product = price_product.Insert(1, ".");

            string name_product = defaultDescription["title"].ToString();

            box_shop=this.carrot.Create_Box();
            box_shop.set_title(name_product);
            box_shop.set_icon(this.carrot.icon_carrot_buy);

            Carrot_Box_Btn_Item btn_history = box_shop.create_btn_menu_header(carrot.sp_icon_restore);
            btn_history.set_act(() => Show_history_pay(user_id));

            Carrot_Box_Item item_product = box_shop.create_item("item_product");
            item_product.set_icon(this.carrot.icon_carrot_database);
            item_product.set_title(defaultDescription["title"].ToString());
            item_product.set_tip(defaultDescription["description"].ToString());

            Carrot_Box_Item item_price = box_shop.create_item("item_price");
            item_price.set_icon(carrot.icon_carrot_price);
            item_price.set_title("Price of product");
            item_price.set_tip(price_product+"$");

            Carrot_Box_Item item_type= box_shop.create_item("item_type");
            item_type.set_icon(carrot.icon_carrot_all_category);
            item_type.set_title("Type");
            if (this.order_type_pay== "0")
                item_type.set_tip("Consumable");
            else
                item_type.set_tip("No Consumable");

            var url_paypal = carrot.mainhost + "?page=pay&id=" + data_product["id"].ToString() + "&title=" + defaultDescription["title"].ToString() + "&description=" + defaultDescription["description"].ToString();
            url_paypal += "&price="+price_product;
            url_paypal += "&user_id="+user_id;
            url_paypal += "&user_lang="+user_lang;
            url_paypal += "&type="+order_type_pay;
            url_paypal += "&id_order="+order_id_pay;
            if (user_name != "") url_paypal += "&user_name=" + user_name;

            Carrot_Box_Btn_Panel panel_btn = box_shop.create_panel_btn();
            Carrot_Button_Item btn_paypal = panel_btn.create_btn();
            btn_paypal.set_icon_white(carrot.icon_carrot_link);
            btn_paypal.set_label("Pay now");
            btn_paypal.set_label_color(Color.white);
            btn_paypal.set_bk_color(carrot.color_highlight);
            btn_paypal.set_act_click(() => On_paypal(url_paypal));

            Carrot_Button_Item btn_share = panel_btn.create_btn();
            btn_share.set_icon_white(carrot.sp_icon_share);
            btn_share.set_label(this.carrot.lang.Val("share","Share"));
            btn_share.set_label_color(Color.white);
            btn_share.set_bk_color(carrot.color_highlight);
            btn_share.set_act_click(() => carrot.show_share(url_paypal, "Ask someone else to buy this product for you!"));

            Carrot_Button_Item btn_check = panel_btn.create_btn();
            btn_check.set_icon_white(carrot.icon_carrot_advanced);
            btn_check.set_label("Check Again!");
            btn_check.set_label_color(Color.white);
            btn_check.set_bk_color(carrot.color_highlight);
            btn_check.set_act_click(() => Check_pay());

            Carrot_Button_Item btn_cancel = panel_btn.create_btn();
            btn_cancel.set_icon_white(carrot.icon_carrot_cancel);
            btn_cancel.set_label(this.carrot.lang.Val("cancel","Cancel"));
            btn_cancel.set_label_color(Color.white);
            btn_cancel.set_bk_color(carrot.color_highlight);
            btn_cancel.set_act_click(() => Close_box_carrot_pay());

            this.On_paypal(url_paypal);
        }

        private void Show_history_pay(string s_id_user)
        {
            carrot.show_loading();
            StructuredQuery q = new("order");
            q.Add_where("user_id", Query_OP.EQUAL, s_id_user);
            carrot.server.Get_doc(q.ToJson(),Act_get_list_history_done,Act_server_fail);
        }

        private void Act_get_list_history_done(string s_data)
        {
            carrot.hide_loading();
            Fire_Collection fc = new(s_data);
            if (!fc.is_null)
            {
                Carrot_Box box_history = carrot.Create_Box();
                box_history.set_title("History Pay");
                box_history.set_icon(carrot.sp_icon_restore);

                for(int i=0;i<fc.fire_document.Length;i++)
                {
                    IDictionary data_history = fc.fire_document[i].Get_IDictionary();
                    var id_product = data_history["id_product"].ToString();
                    Carrot_Box_Item item_history = box_history.create_item("item_history_" + i);
                    if (data_history["status"].ToString() == "COMPLETED")
                    {
                        item_history.set_icon(carrot.icon_carrot_done);

                        if (data_history["type_product"].ToString() == "1")
                        {
                            if (onCarrotPaySuccess != null)
                            {
                                Carrot_Box_Btn_Item btn_download = item_history.create_item();
                                btn_download.set_icon(carrot.icon_carrot_download);
                                btn_download.set_color(carrot.color_highlight);
                                btn_download.set_act(() => onCarrotPaySuccess(id_product));
                            }
                        }
                    }
                    else
                    {
                        item_history.set_icon(carrot.icon_carrot_cancel);
                    }
                    item_history.set_title(data_history["id_product"].ToString());
                    item_history.set_tip(data_history["update_time"].ToString());
                }
            }
            else
            {
                carrot.Show_msg(this.carrot.lang.Val("shop", "Shop"), "You have not purchased any products yet!", Msg_Icon.Alert);
            }
        }

        private void Act_server_fail(string s_error)
        {
            carrot.hide_loading();
            carrot.Show_msg("Error", s_error, Msg_Icon.Error);
        }

        private void Close_box_carrot_pay()
        {
            if(carrot!=null) carrot.play_sound_click();
            product_id_pay = "";
            user_id_pay = "";
            if (box_shop != null) box_shop.close();
        }

        private void On_paypal(string url_pay)
        {
            carrot.play_sound_click();
            Application.OpenURL(url_pay);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus==true&&this.product_id_pay!="") Check_pay();
        }

        private void Check_pay()
        {
            this.carrot.show_loading();
            Debug.Log("Check pay (" + this.product_id_pay + " - "+this.user_id_pay+") from server...");
            StructuredQuery q = new("order");
            if (this.order_type_pay == "1")
            {
                q.Add_where("user_id", Query_OP.EQUAL, user_id_pay);
                q.Add_where("id_product", Query_OP.EQUAL, this.product_id_pay);
            }
            else
            {
                q.Add_where("id_order", Query_OP.EQUAL, this.order_id_pay);
            }
            carrot.server.Get_doc(q.ToJson(), Check_pay_done, Act_server_fail);
        }

        private void Check_pay_done(string s_data)
        {
            this.carrot.hide_loading();
            Fire_Collection fc = new(s_data);
            if (!fc.is_null)
            {
                this.Reset_session_carrot_pay();
                IDictionary data_pay = fc.fire_document[0].Get_IDictionary();
                onCarrotPaySuccess?.Invoke(data_pay["id_product"].ToString());
                if (box_shop != null) box_shop.close();
            }
            else
            {
                this.carrot.Show_msg(this.carrot.lang.Val("shop", "Shop"), this.carrot.lang.Val("shop_buy_fail", "Purchase failed, Please check your account balance, or try again at another time"), Msg_Icon.Error);
                if (box_shop == null) this.Reset_session_carrot_pay();
            }
        }

        private void Restrore_Carrot_pay()
        {
            carrot.show_loading();
            string user_id = carrot.user.get_id_user_login();
            if (user_id=="") user_id = SystemInfo.deviceUniqueIdentifier;
            StructuredQuery q = new("order");
            q.Add_where("user_id",Query_OP.EQUAL,user_id);
            carrot.server.Get_doc(q.ToJson(), Act_restore_carrot_pay_done, Act_server_fail);
        }

        private void Act_restore_carrot_pay_done(string s_data)
        {
            carrot.hide_loading();
            Fire_Collection fc = new(s_data);

            if (!fc.is_null)
            {
                this.OnTransactionsRestored(true);
                IList list_inapp_restore = (IList)Json.Deserialize("[]");

                for(int i = 0; i < fc.fire_document.Length; i++)
                {
                    IDictionary data_in_app = fc.fire_document[i].Get_IDictionary();
                    if (data_in_app["status"].ToString() == "COMPLETED")
                    {
                        if (data_in_app["type_product"].ToString() == "1") list_inapp_restore.Add(data_in_app["id_product"].ToString());
                    }
                }

                string[] arr_id = new string[list_inapp_restore.Count];
                for (int i = 0; i < list_inapp_restore.Count; i++)
                {
                    arr_id[i] = list_inapp_restore[i].ToString();
                }
                this.onCarrotRestoreSuccess.Invoke(arr_id);
            }
            else
            {
                this.OnTransactionsRestored(false);
            }

        }

        private void Reset_session_carrot_pay()
        {
            user_id_pay = "";
            product_id_pay = "";
            order_type_pay = "";
            order_id_pay = "";
        }
        #endregion

    }
}
