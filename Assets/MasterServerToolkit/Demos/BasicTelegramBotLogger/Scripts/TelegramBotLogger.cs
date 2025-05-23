using MasterServerToolkit.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace MasterServerToolkit.Examples.BasicTelegramBotLogger
{
    public delegate void TelegramBotWebRequestHandler(JObject response, string error);

    public class TelegramBotLogger : SingletonBehaviour<TelegramBotLogger>
    {
        [Header("Settings"), SerializeField]
        private string botApiToken;
        [SerializeField]
        private string[] chatIds;

        private void Start()
        {
            CheckBot((response, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    logger.Error(error);
                    return;
                }

                // if (response.ContainsKey("ok") && response.Value<bool>("ok") == true)
                // {
                //     Application.logMessageReceived += Application_logMessageReceived;
                // }
                // else
                // {
                //     logger.Error(response.Value<string>("description"));
                // }
            });
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            string logString = $"{condition} \n {stackTrace}";

            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    foreach (string userId in chatIds)
                    {
                        SendMessageToChat(userId, logString);
                    }
                    break;
            }
        }

        private string ApiRequest(string method)
        {
            return $"https://api.telegram.org/bot{botApiToken}/{method}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void CheckBot(TelegramBotWebRequestHandler callback)
        {
            StartCoroutine(GetCoroutine(ApiRequest("getMe"), callback));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="text"></param>
        public void SendMessageToChat(string chatId, string text)
        {
            WWWForm form = new WWWForm();
            form.AddField("chat_id", chatId);
            form.AddField("text", text);

            StartCoroutine(PostCoroutine(ApiRequest("sendMessage"), form, (response, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    logger.Error(error);
                    return;
                }

                logger.Info(response);
            }));
        }

        private IEnumerator GetCoroutine(string url, TelegramBotWebRequestHandler callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ProtocolError
                    || www.result == UnityWebRequest.Result.ProtocolError
                     || www.result == UnityWebRequest.Result.DataProcessingError)
                {
                    callback?.Invoke(null, www.error);
                    Debug.LogError(www.error);
                }
                else if (www.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(JObject.Parse(www.downloadHandler.text), string.Empty);
                }
            }
        }

        private IEnumerator PostCoroutine(string url, WWWForm form, TelegramBotWebRequestHandler callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ProtocolError
                        || www.result == UnityWebRequest.Result.ProtocolError
                         || www.result == UnityWebRequest.Result.DataProcessingError)
                {
                    callback?.Invoke(null, www.error);
                    Debug.LogError(www.error);
                }
                else if (www.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(JObject.Parse(www.downloadHandler.text), string.Empty);
                }
            }
        }
    }
}
