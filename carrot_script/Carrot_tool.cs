using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Carrot
{
    public class Carrot_tool
    {
        public Sprite Texture2DtoSprite(Texture2D tex2d)
        {
            return Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
        }

        public IEnumerator get_img_form_url_and_save_playerPrefs(string s_url_img, Image img, string s_key,UnityAction<Texture2D> act_done=null, UnityAction<string> act_fail = null)
        {
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(s_url_img);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                if (act_fail != null) act_fail(www.error);
            }
            else
            {
                Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (img != null)
                {
                    img.sprite = this.Texture2DtoSprite(tex);
                    img.color = Color.white;
                }
                this.PlayerPrefs_Save_texture2D(s_key, tex);
                act_done?.Invoke(tex);
            }
        }

        public void PlayerPrefs_Save_texture2D(string s_key, Texture2D tex)
        {
            PlayerPrefs.SetString(s_key, System.Convert.ToBase64String(tex.EncodeToPNG()));
        }

        public void PlayerPrefs_Save_by_data(string s_key, byte[] data)
        {
            PlayerPrefs.SetString(s_key, System.Convert.ToBase64String(data));
        }

        public IEnumerator get_img_form_url_and_save_file(string s_url_img, Image img, string s_path_save)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(s_url_img))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    img.sprite = this.Texture2DtoSprite(tex);
                    img.color = Color.white;
                    this.save_file(s_path_save, tex.EncodeToPNG());
                }
            }
        }

        public IEnumerator get_img_form_url(string s_url_img, Image img,UnityAction<Texture2D> act_done = null,UnityAction<string> act_fail = null)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(s_url_img))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    if (act_fail != null) act_fail(www.error);
                }
                else
                {
                    Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    if (img != null)
                    {
                        if (tex.width > 0)
                        {
                            img.sprite = this.Texture2DtoSprite(tex);
                            img.color = Color.white;
                        }
                    }

                    if (act_done != null) act_done(tex);
                }
            }
        }

        public IEnumerator download_img_form_url(string s_url_img, UnityAction<Texture2D> act_after_done = null)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(s_url_img))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    if (act_after_done != null) act_after_done(null);
                }
                else
                {
                    Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    if (act_after_done != null) act_after_done(tex);
                }
            }
        }

        public void load_file_img(string name_file, Image img, int size_crop)
        {
            string name_file_bk;
            if (Application.isEditor)
                name_file_bk = Application.dataPath + "/" + name_file;
            else
                name_file_bk = Application.persistentDataPath + "/" + name_file;

            if (System.IO.File.Exists(name_file_bk))
            {
                Texture2D load_s01_texture;
                byte[] bytes;
                bytes = System.IO.File.ReadAllBytes(name_file_bk);
                load_s01_texture = new Texture2D(1, 1);
                load_s01_texture.LoadImage(bytes);
                load_s01_texture = this.ResampleAndCrop(load_s01_texture, size_crop, size_crop);
                Sprite sprite = Sprite.Create(load_s01_texture, new Rect(0, 0, load_s01_texture.width, load_s01_texture.height), new Vector2(0, 0));
                img.sprite = sprite;
                img.color = Color.white;
            }
        }

        public Texture2D ResampleAndCrop(Texture2D source, int targetWidth, int targetHeight)
        {
            int sourceWidth = source.width;
            int sourceHeight = source.height;
            float sourceAspect = (float)sourceWidth / sourceHeight;
            float targetAspect = (float)targetWidth / targetHeight;
            int xOffset = 0;
            int yOffset = 0;
            float factor = 1;
            if (sourceAspect > targetAspect)
            {
                factor = (float)targetHeight / sourceHeight;
                xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
            }
            else
            {
                factor = (float)targetWidth / sourceWidth;
                yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
            }
            Color32[] data = source.GetPixels32();
            Color32[] data2 = new Color32[targetWidth * targetHeight];
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                    var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                    data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
                }
            }

            var tex = new Texture2D(targetWidth, targetHeight);
            tex.SetPixels32(data2);
            tex.Apply(true);
            return tex;
        }

        public void save_file(string name_file_save, byte[] data_file)
        {
            if (Application.isEditor)
                System.IO.File.WriteAllBytes(Application.dataPath + "/" + name_file_save, data_file);
            else
                System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + name_file_save, data_file);
        }

        public void create_folder(string s_name_folder)
        {
            if (Application.isEditor)
                s_name_folder = Application.dataPath + "/" + s_name_folder;
            else
                s_name_folder = Application.persistentDataPath + "/" + s_name_folder;

            if (!System.IO.Directory.Exists(s_name_folder)) System.IO.Directory.CreateDirectory(s_name_folder);
        }

        public Sprite get_sprite_to_playerPrefs(string s_key)
        {
            string s_data_sp = PlayerPrefs.GetString(s_key, "");
            if (s_data_sp != "")
            {
                byte[] imageBytes = Convert.FromBase64String(s_data_sp);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
            else
            {
                return null;
            }
        }

        public Texture2D get_texture2D_to_playerPrefs(string s_key)
        {
            string s_data_sp = PlayerPrefs.GetString(s_key, "");
            if (s_data_sp != "")
            {
                byte[] imageBytes = Convert.FromBase64String(s_data_sp);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                return tex;
            }
            else
            {
                return null;
            }
        }

        public byte[] get_data_to_playerPrefs(string s_key)
        {
            string s_data= PlayerPrefs.GetString(s_key, "");
            if (s_data != "")
            {
                byte[] dataBytes = Convert.FromBase64String(s_data);
                return dataBytes;
            }
            else
            {
                return null;
            }
        }

        public void load_file_img(string name_file, Image img)
        {
            string name_file_bk;
            if (Application.isEditor)
                name_file_bk = Application.dataPath + "/" + name_file;
            else
                name_file_bk = Application.persistentDataPath + "/" + name_file;

            if (System.IO.File.Exists(name_file_bk))
            {
                Texture2D load_s01_texture;
                byte[] bytes;
                bytes = System.IO.File.ReadAllBytes(name_file_bk);
                load_s01_texture = new Texture2D(1, 1);
                load_s01_texture.LoadImage(bytes);

                Sprite sprite = Sprite.Create(load_s01_texture, new Rect(0, 0, load_s01_texture.width, load_s01_texture.height), new Vector2(0, 0));
                img.sprite = sprite;
                img.color = Color.white;
            }
        }

        public Sprite load_file_img(string name_file)
        {
            string name_file_bk;
            if (Application.isEditor)
                name_file_bk = Application.dataPath + "/" + name_file;
            else
                name_file_bk = Application.persistentDataPath + "/" + name_file;

            if (System.IO.File.Exists(name_file_bk))
            {
                Texture2D load_s01_texture;
                byte[] bytes;
                bytes = System.IO.File.ReadAllBytes(name_file_bk);
                load_s01_texture = new Texture2D(1, 1);
                load_s01_texture.LoadImage(bytes);
                return Sprite.Create(load_s01_texture, new Rect(0, 0, load_s01_texture.width, load_s01_texture.height), new Vector2(0, 0));
            }
            else
            {
                return null;
            }
        }

        public void load_file_img(string name_file, SpriteRenderer img)
        {
            string name_file_bk;
            if (Application.isEditor)
                name_file_bk = Application.dataPath + "/" + name_file;
            else
                name_file_bk = Application.persistentDataPath + "/" + name_file;

            if (System.IO.File.Exists(name_file_bk))
            {
                Texture2D load_s01_texture;
                byte[] bytes;
                bytes = System.IO.File.ReadAllBytes(name_file_bk);
                load_s01_texture = new Texture2D(1, 1);
                load_s01_texture.LoadImage(bytes);

                Sprite sprite = Sprite.Create(load_s01_texture, new Rect(0, 0, load_s01_texture.width, load_s01_texture.height), new Vector2(0, 0));
                img.sprite = sprite;
                img.color = Color.white;
            }
        }

        public void delete_file(string name_file)
        {
            if (Application.isEditor)
                name_file = Application.dataPath + "/" + name_file;
            else
                name_file = Application.persistentDataPath + "/" + name_file;

            if (System.IO.File.Exists(name_file)) System.IO.File.Delete(name_file);
        }

        public bool check_file_exist(string name_file)
        {
            if (Application.isEditor)
                name_file = Application.dataPath + "/" + name_file;
            else
                name_file = Application.persistentDataPath + "/" + name_file;
            if (System.IO.File.Exists(name_file))
                return true;
            else
                return false;
        }

        public string get_file_path(string name_file)
        {
            if (Application.isEditor)
                return Application.dataPath + "/" + name_file;
            else
                return Application.persistentDataPath + "/" + name_file;
        }

        public IList<IDictionary> Shuffle_Ilist(IList<IDictionary> list)
        {
            System.Random rng = new System.Random();
            for (int i = list.Count - 1; i > 0; i--)
            { 
                int j = rng.Next(0, i + 1);
                IDictionary temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
            return list;
        }
    }
}
