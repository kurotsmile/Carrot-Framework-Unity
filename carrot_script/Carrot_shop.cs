using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
namespace Carrot
{
    interface Carrot_shop_event
    {
        void carrot_by_success(string s_id_product);
        void carrot_restore_success(string[] arr_id);
    }

    public class Carrot_shop : MonoBehaviour, IStoreListener
    {
        IStoreController m_StoreController;
        IExtensionProvider extensions;

        private List<string> list_id_product;
        private string id_product_check = "";
        private Carrot carrot;
        public UnityAction<string> onCarrotPaySuccess;
        public UnityAction<string[]> onCarrotRestoreSuccess;

        public void load(Carrot carrot)
        {
            this.carrot = carrot;
            var catalog = ProductCatalog.LoadDefaultCatalog();
            list_id_product = new List<string>();

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

        public void buy_product(int index_p)
        {
            this.carrot.play_sound_click();
            this.carrot.show_loading();
            if (this.carrot.pay_app == PayApp.CarrotPay)
            {
                this.id_product_check = this.list_id_product[index_p];
                string user_id = this.carrot.user.get_id_user_login();
                string user_lang = PlayerPrefs.GetString("lang", "en");
                if (user_id == "")
                    user_id = SystemInfo.deviceUniqueIdentifier;
                else
                    user_lang = this.carrot.user.get_lang_user_login();
                //string link_pay =this.carrot.get_url_host() + "/pay_inapp/" + this.list_id_product[index_p] + "/" + user_id + "/" + user_lang;
                //Application.OpenURL(link_pay);
            }
            else
            {
                m_StoreController.InitiatePurchase(this.list_id_product[index_p]);
            }
        }

        public void restore_product()
        {
            this.carrot.play_sound_click();
            if (this.carrot.pay_app == PayApp.CarrotPay)
            {
                WWWForm frm_inapp = carrot.frm_act("restore_inapp");
                string user_id = this.carrot.user.get_id_user_login();
                string user_lang = PlayerPrefs.GetString("lang", "en");
                if (user_id == "")
                    user_id = SystemInfo.deviceUniqueIdentifier;
                else
                    user_lang = this.carrot.user.get_lang_user_login();
                frm_inapp.AddField("user_id", user_id);
                frm_inapp.AddField("user_lang", user_lang);
                for (int i = 0; i < this.list_id_product.Count; i++)
                {
                    frm_inapp.AddField("inapp_id[]", this.list_id_product[i]);
                }
                //this.carrot.send(frm_inapp, act_restore_inapp_carrot);
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
                IList list_inapp =(IList) data_restore["inapp_success"];
                string[] arr_id = new string[list_inapp.Count];
                for(int i = 0; i < list_inapp.Count; i++)
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

        void OnApplicationFocus(bool hasFocus)
        {
            if (this.id_product_check != "" && hasFocus&&this.carrot.pay_app==PayApp.CarrotPay)
            {
                WWWForm frm_inapp = carrot.frm_act("check_inapp");
                string user_id = this.carrot.user.get_id_user_login();
                string user_lang = PlayerPrefs.GetString("lang", "en");
                if (user_id == "")
                    user_id = SystemInfo.deviceUniqueIdentifier;
                else
                    user_lang = this.carrot.user.get_lang_user_login();
                frm_inapp.AddField("inapp_id", this.id_product_check);
                frm_inapp.AddField("user_id", user_id);
                frm_inapp.AddField("user_lang", user_lang);
                //this.carrot.send(frm_inapp, act_check_inapp);
            }
        }

        private void act_check_inapp(string s_data)
        {
            this.carrot.hide_loading();
            IDictionary data_inapp = (IDictionary)Json.Deserialize(s_data);
            if (data_inapp["error"].ToString() == "1")
                this.carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString(data_inapp["msg"].ToString(), data_inapp["msg_en"].ToString()), Msg_Icon.Alert);
            else
                this.onCarrotPaySuccess.Invoke(this.id_product_check);
            this.id_product_check = "";
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
    }
}
