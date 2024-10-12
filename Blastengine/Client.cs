using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace Blastengine
{
    public class Client
    {
        public string UserId { get; }
        public string ApiKey { get; }
        public string? Token { get; set; }
        public string EndPoint = "https://app.engn.jp/api";

        public Client(string UserId, string ApiKey)
        {
            this.UserId = UserId;
            this.ApiKey = ApiKey;
            GenerateToken();
            Base.Client = this;
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

        public async Task<BEObject?> PostText(string Path, string Data)
        {
            var Response = await Exec("POST", Path, null, Data);
            var ResponseBody = await Response.Content.ReadAsStringAsync();
            // ステータスコードのチェック（成功しているかどうか）
            if (Response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new Exception(ResponseBody);
            }
            return JsonSerializer.Deserialize<BEObject>(json: ResponseBody!);
        }

        public async Task<BEObject?> PutText(string Path, string Data)
        {
            var Response = await Exec("PUT", Path, null, Data);
            var ResponseBody = await Response.Content.ReadAsStringAsync();
            // ステータスコードのチェック（成功しているかどうか）
            if (Response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(ResponseBody);
            }
            return JsonSerializer.Deserialize<BEObject>(json: ResponseBody!);
        }

        public async Task<BEObject?> PatchText(string Path, string? Data)
        {
            var Response = await Exec("PATCH", Path, null, Data);
            var ResponseBody = await Response.Content.ReadAsStringAsync();
            // ステータスコードのチェック（成功しているかどうか）
            if (Response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(ResponseBody);
            }
            return JsonSerializer.Deserialize<BEObject>(json: ResponseBody!);
        }

        public HttpClient CreateClient()
        {
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("Authorization", $@"Bearer {Token}");
            return Client;
        }

        public async Task<BEObject?> GetText(string Path, System.Collections.Specialized.NameValueCollection? Query)
        {
            var Response = await Exec("GET", Path, Query, null);
            var ResponseBody = await Response.Content.ReadAsStringAsync();
            // ステータスコードのチェック（成功しているかどうか）
            if (Response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(ResponseBody);
            }
            return JsonSerializer.Deserialize<BEObject>(json: ResponseBody!);
        }

        public async Task<BEObject?> DeleteText(string Path)
        {
            var Response = await Exec("DELETE", Path, null, null);
            var ResponseBody = await Response.Content.ReadAsStringAsync();
            // ステータスコードのチェック（成功しているかどうか）
            if (Response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(ResponseBody);
            }
            return JsonSerializer.Deserialize<BEObject>(json: ResponseBody!);
        }

        public async Task<HttpResponseMessage> Exec(
            string Method,
            string Path,
            System.Collections.Specialized.NameValueCollection? Query,
            string? Data)
        {
            var Client = CreateClient();
            var QueryString = Query != null ? $@"?{Query.ToString()}" : "";
            var Url = $@"{EndPoint}{Path}{QueryString}";
            if (Method == "POST" || Method == "PUT" || Method == "PATCH")
            {
                var Content = Data == null ? null : new StringContent(Data, Encoding.UTF8, "application/json");
                if (Method == "POST") return await Client.PostAsync(Url, Content);
                if (Method == "PUT") return await Client.PutAsync(Url, Content);
                return await Client.PatchAsync(Url, Content);
            }
            if (Method == "DELETE")
            {
                return await Client.DeleteAsync(Url);
            }
            return await Client.GetAsync(Url);
        }

    }

}
