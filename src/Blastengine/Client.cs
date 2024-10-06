using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blastengine
{
    public class Client
    {
        public string UserId { get; }
        public string ApiKey { get; }
        public string Token { get; set; }
        public string EndPoint = "https://app.engn.jp/api";

        public Client(string UserId, string ApiKey)
        {
            this.UserId = UserId;
            this.ApiKey = ApiKey;
            GenerateToken();
            Transaction.Client = this;
        }

        private void GenerateToken()
        {
            // 入力文字列をバイト配列に変換
            byte[] inputBytes = Encoding.UTF8.GetBytes(UserId + ApiKey);

            // SHA256を使用してハッシュ化
            SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // ハッシュを16進数表現の小文字文字列に変換
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            string lowerCaseHash = sb.ToString();

            // 小文字のハッシュをバイト配列に再変換
            byte[] hashBytesLower = Encoding.UTF8.GetBytes(lowerCaseHash);

            // Base64エンコードして結果を返す
            Token = Convert.ToBase64String(hashBytesLower);
        }

        public async Task<BlastengineObject> PostText(string Path, string Data)
        {
            var Client = new HttpClient();
            var Content = new StringContent(Data, Encoding.UTF8, "application/json");
            Client.DefaultRequestHeaders.Add("Authorization", $@"Bearer {Token}");
            var Url = $@"{EndPoint}{Path}";
            var Response = await Client.PostAsync(Url, Content);
            var ResponseBody = await Response.Content.ReadAsStringAsync();

            // ステータスコードのチェック（成功しているかどうか）
            if (Response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new Exception(ResponseBody);
            }
            return JsonConvert.DeserializeObject<BlastengineObject>(ResponseBody);
        }
    }

    public struct From
    {
        [JsonProperty("email")]
        public string Email { get; }
        [JsonProperty("name")]
        public string Name { get; set; }

        public From(string Email, string Name = "")
        {
            this.Email = Email;
            this.Name = Name;
        }
    }

    public struct ListUnsubscribe
    {
        [JsonProperty("mailto")]
        public string MailTo { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }

        public ListUnsubscribe(string Email = null, string Url = null)
        {
            if (Email == null)
            {
                MailTo = "";
            }
            else
            {
                MailTo = $@"mailto:{Email}";
            }
            this.Url = Url;
        }
    }

    public struct InsertCode
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

        public InsertCode(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}
