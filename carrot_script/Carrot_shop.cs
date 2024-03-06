using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
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

        public void load(Carrot carrot)
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
            {

            }
            else
            {
                this.act_restore_unity_pay();
            }
        }

        private void act_restore_inapp_carrot(string s_data)
        {
            IDictionary data_restore = (IDictionary)Json.Deserialize(s_data);
            if (data_restore["error"].ToString() == "0")
            {
                this.OnTransactionsRestored(true);
                IList list_inapp = (IList)data_restore["inapp_success"];
                string[] arr_id = new string[list_inapp.Count];
                for (int i = 0; i < list_inapp.Count; i++)
                {
                    arr_id[i] = list_inapp[i].ToString();
                }
                this.onCarrotRestoreSuccess.Invoke(arr_id);
            }
            else
            {
                this.OnTransactionsRestored(false);
            }
            this.carrot.log("Restore inapp:" + s_data);
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
                this.carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("shop_restore_success", "Successful recovery!"), Msg_Icon.Success);
            else
                this.carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("shop_restore_fail", "Restore failed!"), Msg_Icon.Error);
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
            this.carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("shop_buy_fail", "Purchase failed, Please check your account balance, or try again at another time"), Msg_Icon.Error);
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
            string user_id = carrot.user.get_id_user_login();
            string user_lang = carrot.lang.get_key_lang();
           
            if (user_id != "")
                user_lang = carrot.user.get_lang_user_login();
            else
                user_id = SystemInfo.deviceUniqueIdentifier;

            IDictionary defaultDescription = (IDictionary)data_product["defaultDescription"];
            IDictionary googlePrice = (IDictionary)data_product["googlePrice"];
            string price_product = googlePrice["num"].ToString();
            price_product = price_product.Insert(1, ".");

            string name_product = defaultDescription["title"].ToString();

            box_shop=this.carrot.Create_Box();
            box_shop.set_title(name_product);
            box_shop.set_icon(this.carrot.icon_carrot_buy);

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
            if (data_product["type"].ToString()=="0")
                item_type.set_tip("Consumable");
            else
                item_type.set_tip("No Consumable");

            var url_paypal = carrot.mainhost + "?page=pay&id=" + data_product["id"].ToString() + "&title=" + defaultDescription["title"].ToString() + "&description=" + defaultDescription["description"].ToString();
            url_paypal  +="&price="+price_product;
            url_paypal += "&user_id="+user_id;
            url_paypal += "&user_lang="+user_lang;
            url_paypal += "&type"+data_product["type"].ToString();

            Carrot_Box_Btn_Panel panel_btn = box_shop.create_panel_btn();
            Carrot_Button_Item btn_paypal = panel_btn.create_btn();
            btn_paypal.set_icon_white(carrot.icon_carrot_link);
            btn_paypal.set_label("Pay pal");
            btn_paypal.set_label_color(Color.white);
            btn_paypal.set_bk_color(carrot.color_highlight);
            btn_paypal.set_act_click(() => On_paypal(url_paypal));

            Carrot_Button_Item btn_share = panel_btn.create_btn();
            btn_share.set_icon_white(carrot.sp_icon_share);
            btn_share.set_label(PlayerPrefs.GetString("share","Share"));
            btn_share.set_label_color(Color.white);
            btn_share.set_bk_color(carrot.color_highlight);
            btn_share.set_act_click(() => carrot.show_share(url_paypal, "Ask someone else to buy this product for you!"));

            Carrot_Button_Item btn_cancel = panel_btn.create_btn();
            btn_cancel.set_icon_white(carrot.icon_carrot_cancel);
            btn_cancel.set_label(PlayerPrefs.GetString("cancel","Cancel"));
            btn_cancel.set_label_color(Color.white);
            btn_cancel.set_bk_color(carrot.color_highlight);
            btn_cancel.set_act_click(() => Close_box_carrot_pay());

            this.On_paypal(url_paypal);
        }

        private void Close_box_carrot_pay()
        {
            carrot.play_sound_click();
            if (box_shop != null) box_shop.close();
        }

        private void On_paypal(string url_pay)
        {
            carrot.play_sound_click();
            Application.OpenURL(url_pay);
        }

        #endregion

    }
}
