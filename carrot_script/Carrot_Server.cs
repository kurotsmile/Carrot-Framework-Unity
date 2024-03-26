using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Carrot
{
    public enum Query_OP
    {
        OPERATOR_UNSPECIFIED,
        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        EQUAL,
        NOT_EQUAL,
        ARRAY_CONTAINS,
        IN,
        ARRAY_CONTAINS_ANY,
        NOT_IN
    }

    public enum Query_Order_Direction
    {
        DIRECTION_UNSPECIFIED,
        ASCENDING,
        DESCENDING
    }

    public class Carrot_Server : MonoBehaviour
    {
        [Header("Obj Main")]
        public Carrot carrot;
        private readonly string projectId = "carrotstore";

        public void Get_doc(string query, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            StartCoroutine(this.Act_get_doc(query, act_done, act_fail));
        }

        IEnumerator Act_get_doc(string query, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            string url = "https://firestore.googleapis.com/v1/projects/" + projectId + "/databases/(default)/documents:runQuery";

            var request = new UnityWebRequest(url, "POST");

            TextAsset txtAsset = (TextAsset)Resources.Load("serviceAccountKey", typeof(TextAsset));

            request.uploadHandler = new UploadHandlerRaw(txtAsset.bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(query);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw)
            {
                contentType = "application/json"
            };

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                act_fail?.Invoke(request.error);
            else
                act_done?.Invoke(request.downloadHandler.text);
        }

        public void Get_doc_by_path(string id_Collection, string id_document, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            StartCoroutine(this.Act_get_doc_by_path(id_Collection, id_document, act_done, act_fail));
        }

        IEnumerator Act_get_doc_by_path(string id_Collection, string id_document, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            string url = "https://firestore.googleapis.com/v1/projects/" + projectId + "/databases/(default)/documents/" + id_Collection + "/" + id_document + "?key=" + this.carrot.key_api_rest_firestore;
            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
                act_fail?.Invoke(www.error);
            else
                act_done?.Invoke(www.downloadHandler.text);
        }

        public void Add_Document_To_Collection(string collectionId, string documentId, string jsonData, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            Debug.Log("jsonData:" + jsonData);
            StartCoroutine(Act_add_document(collectionId, documentId, jsonData, act_done, act_fail));
        }

        private IEnumerator Act_add_document(string collectionId, string documentId, string jsonData, UnityAction<string> act_done, UnityAction<string> act_fail)
        {
            string url = "https://firestore.googleapis.com/v1/projects/" + projectId + "/databases/(default)/documents/" + collectionId + "/" + documentId;

            var request = new UnityWebRequest(url, "PATCH");

            TextAsset txtAsset = (TextAsset)Resources.Load("serviceAccountKey", typeof(TextAsset));

            request.uploadHandler = new UploadHandlerRaw(txtAsset.bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw)
            {
                contentType = "application/json"
            };

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                act_fail?.Invoke(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                act_done?.Invoke(request.downloadHandler.text);
            }
        }

        public void Update_Field_Document(string collectionId, string documentId, string fieldID, string jsonData, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            StartCoroutine(Act_update_document(collectionId, documentId, fieldID, jsonData, act_done, act_fail));
        }

        private IEnumerator Act_update_document(string collectionId, string documentId,string fieldID, string jsonData, UnityAction<string> act_done, UnityAction<string> act_fail)
        {
            string url = "https://firestore.googleapis.com/v1/projects/" + projectId + "/databases/(default)/documents/" + collectionId + "/" + documentId + "?updateMask.fieldPaths="+ fieldID;

            var request = new UnityWebRequest(url, "PATCH");

            TextAsset txtAsset = (TextAsset)Resources.Load("serviceAccountKey", typeof(TextAsset));

            request.uploadHandler = new UploadHandlerRaw(txtAsset.bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw)
            {
                contentType = "application/json"
            };

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                act_fail?.Invoke(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                act_done?.Invoke(request.downloadHandler.text);
            }
        }

        [ContextMenu("Add test")]
        public void Test()
        {
            string jsonPayload = "{\"fields\": {\"fieldName\": {\"stringValue\": \"fieldValue\"}}}";
            this.Add_Document_To_Collection("app", "a", jsonPayload);
        }

        [ContextMenu("Del test")]
        public void Delete_test()
        {
            this.Delete_Doc("app", "a");
        }

        public string Convert_IDictionary_to_json(IDictionary obj_IDictionary)
        {
            string s_json = "{";
            s_json += "\"fields\":{";
            foreach (var key in obj_IDictionary.Keys)
            {
                if (key.ToString() == "user")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IDictionary data_obj = (IDictionary)obj_IDictionary[key];
                        s_json += "\"user\":{\"mapValue\":"+this.Get_Mapvalue(data_obj) +"},";
                    }
                }
                else if(key.ToString()== "address")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IDictionary data_obj = (IDictionary)obj_IDictionary[key];
                        s_json += "\"address\":{\"mapValue\":"+this.Get_Mapvalue(data_obj) + "},";
                    }
                }
                else if(key.ToString()== "reports")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IList list_obj = (IList)obj_IDictionary[key];
                        s_json += "\"reports\":{\"arrayValue\":" + this.Get_ArrayValue(list_obj) + "},";
                    }
                }
                else if (key.ToString() == "rank")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IList list_obj = (IList)obj_IDictionary[key];
                        s_json += "\"rank\":{\"arrayValue\":" + this.Get_ArrayValue(list_obj) + "},";
                    }
                }
                else if (key.ToString() == "rates")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IList list_obj = (IList)obj_IDictionary[key];
                        s_json += "\"rates\":{\"arrayValue\":" + this.Get_ArrayValue(list_obj) + "},";
                    }
                }
                else if (key.ToString() == "datas")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IList list_obj = (IList)obj_IDictionary[key];
                        s_json += "\"datas\":{\"arrayValue\":" + this.Get_ArrayValue(list_obj) + "},";
                    }
                }
                else
                {
                    s_json += "\"" + key + "\":{\"stringValue\":\"" + obj_IDictionary[key] + "\"},";
                }
            }
            s_json = s_json[..^1];
            return s_json += "}}";
        }

        private string Get_Mapvalue(IDictionary obj_IDictionary)
        {
            string s_json = "{";
            s_json += "\"fields\":{";
            foreach (var key in obj_IDictionary.Keys)
            {
                if (key.ToString() == "user")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IDictionary data_obj = (IDictionary)obj_IDictionary[key];
                        s_json += "\"user\":{\"mapValue\":" + this.Get_Mapvalue(data_obj) + "},";
                    }
                }
                else if (key.ToString() == "address")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IDictionary data_obj = (IDictionary)obj_IDictionary[key];
                        s_json += "\"address\":{\"mapValue\":" + this.Get_Mapvalue(data_obj) + "},";
                    }
                }
                else if (key.ToString() == "reports")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IList list_obj = (IList)obj_IDictionary[key];
                        s_json += "\"reports\":{\"arrayValue\":" + this.Get_ArrayValue(list_obj) + "},";
                    }
                }
                else if (key.ToString() == "rank")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IList list_obj = (IList)obj_IDictionary[key];
                        s_json += "\"rank\":{\"arrayValue\":" + this.Get_ArrayValue(list_obj) + "},";
                    }
                }
                else if (key.ToString() == "datas")
                {
                    if (obj_IDictionary[key] != null)
                    {
                        IList list_obj = (IList)obj_IDictionary[key];
                        s_json += "\"datas\":{\"arrayValue\":" + this.Get_ArrayValue(list_obj) + "},";
                    }
                }
                else
                {
                    s_json += "\"" + key + "\":{\"stringValue\":\"" + obj_IDictionary[key] + "\"},";
                }
            }
            s_json = s_json[..^1];
            return s_json += "}}";
        }

        public string Get_ArrayValue(IList datas)
        {
            string s_json = "{";
            s_json += "\"values\":[";
            for(int i=0;i<datas.Count;i++)
            {
                IDictionary obj_data = (IDictionary) datas[i];
                s_json += "{\"mapValue\":" +this.Get_Mapvalue(obj_data) + "},";
            }
            s_json = s_json[..^1];
            return s_json += "]}";
        }

        public void Delete_Doc(string id_Collection, string id_document, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            StartCoroutine(DeleteDoc(id_Collection, id_document, act_done, act_fail));
        }

        IEnumerator DeleteDoc(string id_Collection, string id_document, UnityAction<string> act_done = null, UnityAction<string> act_fail = null)
        {
            string url = "https://firestore.googleapis.com/v1/projects/" + projectId + "/databases/(default)/documents/" + id_Collection + "/" + id_document + "?key=" + this.carrot.key_api_rest_firestore;

            using UnityWebRequest www = UnityWebRequest.Delete(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
                act_fail?.Invoke(www.error);
            }
            else
            {
                Debug.Log("Delete successful!");
                act_done?.Invoke("");
            }
        }
    }

    public class Fire_Document
    {
        readonly IDictionary doc;

        public Fire_Document(IDictionary doc)
        {
            this.doc = doc;
        }

        public Fire_Document(string s_data)
        {
            this.doc = (IDictionary) Json.Deserialize(s_data);
        }

        public string Get_path()
        {
            return doc["name"].ToString();
        }

        public string Get_id()
        {
            string[] parts = this.Get_path().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return parts[^1];
        }

        public object Get_val(string key)
        {
            IDictionary fields = (IDictionary)this.doc["fields"];
            if (fields[key] != null)
            {
                IDictionary val = (IDictionary)fields[key];
                if(val["stringValue"]!=null) return val["stringValue"];
                if(val["mapValue"] !=null) return val["mapValue"];
                if(val["arrayValue"] !=null) return val["arrayValue"];
            }
            return null;
        }

        public IDictionary Get_IDictionary()
        {
            IDictionary obj_d = (IDictionary)Json.Deserialize("{}");
            if (this.doc["fields"] != null)
            {
                IDictionary fields = (IDictionary)this.doc["fields"];
                foreach (var key in fields.Keys)
                {
                    IDictionary val = (IDictionary)fields[key];
                    if (val["stringValue"] != null)
                    {
                        obj_d[key] = val["stringValue"];
                    }

                    if (val["mapValue"] != null)
                    {
                        IDictionary mapValue = (IDictionary)val["mapValue"];
                        obj_d[key] = this.Get_mapValue(mapValue);
                    }

                    if (val["arrayValue"] != null)
                    {
                        IDictionary values = (IDictionary)val["arrayValue"];
                        IList arrayValue = (IList)values["values"];
                        obj_d[key] = this.Get_ArrayValue(arrayValue);
                    }
                }
            }
            obj_d["id"] = this.Get_id();
            return obj_d;
        }

        public IDictionary Get_mapValue(IDictionary datamap)
        {
            IDictionary fields = (IDictionary)datamap["fields"];
            IDictionary obj_d = (IDictionary)Json.Deserialize("{}");
            foreach (var key in fields.Keys)
            {
                IDictionary val = (IDictionary)fields[key];
                if (val["stringValue"] != null)
                {
                    string s_val = val["stringValue"].ToString();
                    obj_d[key] = s_val;
                }

                if (val["mapValue"] != null)
                {
                    IDictionary mapValue = (IDictionary)val["mapValue"];
                    obj_d[key] = this.Get_mapValue(mapValue);
                }

                if (val["arrayValue"] != null)
                {
                    IDictionary values = (IDictionary)val["arrayValue"];
                    IList arrayValue = (IList)values["values"];
                    obj_d[key] = this.Get_ArrayValue(arrayValue);
                }
            }
            return obj_d;
        }

        public IList Get_ArrayValue(IList datamap)
        {
            IList list = (IList)Json.Deserialize("[]");
            for(int i=0;i<datamap.Count;i++)
            {
                IDictionary val = (IDictionary)datamap[i];
                if (val["stringValue"] != null)
                {
                    string s_val = val["stringValue"].ToString();
                    list.Add(s_val);
                }

                if (val["mapValue"] != null)
                {
                    IDictionary mapValue = (IDictionary)val["mapValue"];
                    list.Add(this.Get_mapValue(mapValue));
                }

                if (val["arrayValue"] != null)
                {
                    IDictionary values=(IDictionary)val["arrayValue"];
                    IList arrayValue = (IList)values["values"];
                    list.Add(this.Get_ArrayValue(arrayValue));
                }
            }
            return list;
        }
    }

    public class Fire_Collection
    {
        public Fire_Document[] fire_document;
        public bool is_null = false;

        public Fire_Collection(string s_data)
        {
            IList Documents = (IList)Json.Deserialize(s_data);
            fire_document = new Fire_Document[Documents.Count];

            if (Documents.Count == 1)
            {
                IDictionary readTime = (IDictionary)Documents[0];
                if (readTime["document"] == null)
                {
                    this.is_null = true;
                    return;
                }
            }

            for (int i = 0; i < Documents.Count; i++)
            {
                IDictionary all_Document = (IDictionary)Documents[i];
                IDictionary d = (IDictionary)all_Document["document"];
                this.fire_document[i] = new Fire_Document(d);
            }
        }

        public Fire_Document Get_doc_random()
        {
            if (this.fire_document.Length > 0)
            {
                int index_rand = UnityEngine.Random.Range(0, this.fire_document.Length);
                return this.fire_document[index_rand];
            }
            else
            {
                return this.fire_document[0];
            }
        }
    }

    [System.Serializable]
    public class StructuredQuery
    {
        private string collectionId = "";
        private string s_where = "";
        private string s_order = "";
        private string s_type_where = "AND";

        private int limit = -1;
        private readonly List<string> list_select = new();
        private readonly List<string> list_where = new();

        public StructuredQuery(string s_collectionId)
        {
            this.collectionId = s_collectionId;
        }

        public void set_type_where(string s_type)
        {
            this.s_type_where = s_type;
        }

        public void Set_from(string s_collectionId)
        {
            this.collectionId = s_collectionId;
        }

        public void Set_limit(int limit)
        {
            this.limit = limit;
        }

        public void Add_select(string s_field)
        {
            this.list_select.Add("{\"fieldPath\":\"" + s_field + "\"}");
        }

        public void Set_where(string key, Query_OP op = Query_OP.EQUAL, string val = "")
        {
            this.s_where = ",\"where\":{\"fieldFilter\":{\"field\":{\"fieldPath\":\"" + key + "\"},\"op\":\"" + op.ToString() + "\",\"value\":{\"stringValue\":\"" + val + "\"}}}";
        }

        public void Add_where(string key, Query_OP op = Query_OP.EQUAL, string val = "")
        {
            this.list_where.Add("{\"fieldFilter\":{\"field\":{\"fieldPath\":\"" + key + "\"},\"op\":\"" + op.ToString() + "\",\"value\":{\"stringValue\":\"" + val + "\"}}}");
        }

        public void Add_order(string s_field_orderby, Query_Order_Direction direction = Query_Order_Direction.ASCENDING)
        {
            this.s_order = ",\"orderBy\":[{\"field\":{\"fieldPath\":\"" + s_field_orderby + "\"},\"direction\":\""+direction.ToString()+"\"}]";
        }

        public string ToJson()
        {
            string s_json = "{\"structuredQuery\":{\"from\":[{\"collectionId\":\"" + this.collectionId + "\"}]";

            if (this.list_select.Count > 0)
            {
                s_json += ",\"select\":{\"fields\":[";
                for (int i = 0; i < this.list_select.Count; i++)
                {
                    s_json += this.list_select[i];
                    if (i != this.list_select.Count - 1) s_json += ",";
                }
                s_json += "]}";
            }

            if (this.s_where != "")
            {
                s_json += this.s_where;
            }
            else
            {
                if (this.list_where.Count > 0)
                {
                    string s_q_where = ",\"where\":{\"compositeFilter\":{\"op\":\""+this.s_type_where+"\",\"filters\":[";
                    for (int i = 0; i < this.list_where.Count; i++)
                    {
                        s_q_where += this.list_where[i];
                        if (i != this.list_where.Count - 1) s_q_where += ",";
                    }
                    s_q_where += "]}}";
                    s_json += s_q_where;
                }
            }

            if (this.s_order != "") s_json += this.s_order;
            if (limit != -1) s_json += ",\"limit\":"+this.limit;

            s_json += "}}";
            return s_json;
        }
    }
}