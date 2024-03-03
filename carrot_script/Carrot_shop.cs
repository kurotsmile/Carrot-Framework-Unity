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
                IList list_product = (IList)data_inapp["products"];

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
                this.Check_login_and_buy_product_paypal(this.list_id_product[index_p]);
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
        private void Check_login_and_buy_product_paypal(string s_id_product)
        {
            Carrot_Box box_paypal = this.carrot.Create_Box();
            box_paypal.set_title("Login PayPal");
            box_paypal.set_icon_white(this.carrot.icon_carrot_all_category);

            Carrot_Box_Item item_product = box_paypal.create_item("item_product");
            item_product.set_icon(this.carrot.icon_carrot_database);

            Carrot_Box_Item item_email = box_paypal.create_item("item_mail");
            item_email.set_icon(this.carrot.user.icon_user_name);
            item_email.set_title("Email");
            item_email.set_tip("Email paypal account");
            item_email.set_type(Box_Item_Type.box_email_input);
            item_email.check_type();

            Carrot_Box_Item item_password = box_paypal.create_item("item_password");
            item_password.set_icon(this.carrot.user.icon_user_change_password);
            item_password.set_title("Password");
            item_password.set_tip("Password paypal");
            item_password.set_type(Box_Item_Type.box_password_input);
            item_password.check_type();

            Carrot_Box_Btn_Panel panel_btn = box_paypal.create_panel_btn();
            Carrot_Button_Item btn_login = panel_btn.create_btn("btn_login");
            btn_login.set_icon(this.carrot.icon_carrot_done);
            btn_login.set_label("Login");
        }
        #endregion

    }
}
