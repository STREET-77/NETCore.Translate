using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Translate.Core.Translator.Entities;
using Translate.Core.Translator.Enums;
using Translate.Core.Translator.Google.Entities;

namespace Translate.Core.Translator.Google
{
    public enum GoogleTranslateLanguage
    {
        /// <summary>
        /// 阿拉伯语
        /// </summary>
        Arabic,
        /// <summary>
        /// 白俄罗斯语
        /// </summary>
        Belarusian,
        /// <summary>
        /// 保加利亚语
        /// </summary>
        Bulgarian,
        ChineseSimplified,
        ChineseTraditional,
        /// <summary>
        /// 克罗地亚语
        /// </summary>
        Croatian,
        /// <summary>
        /// 捷克语
        /// </summary>
        Czech,
        /// <summary>
        /// 丹麦语
        /// </summary>
        Danish,
        /// <summary>
        /// 荷兰语
        /// </summary>
        Dutch,
        English,
        /// <summary>
        /// 菲律宾语
        /// </summary>
        Filipino,
        /// <summary>
        /// 芬兰语
        /// </summary>
        Finnish,
        French,
        /// <summary>
        /// 德语
        /// </summary>
        German,
        /// <summary>
        /// 希腊语
        /// </summary>
        Greek,
        /// <summary>
        /// 希伯来语
        /// </summary>
        Hebrew,
        /// <summary>
        /// 印地语
        /// </summary>
        Hindi,
        /// <summary>
        /// 匈牙利语
        /// </summary>
        Hungarian,
        /// <summary>
        /// 冰岛语
        /// </summary>
        Icelandic,
        /// <summary>
        /// 印度尼西亚语
        /// </summary>
        Indonesian,
        /// <summary>
        /// 爱尔兰语
        /// </summary>
        Irish,
        /// <summary>
        /// 意大利语
        /// </summary>
        Italian,
        Japanese,
        /// <summary>
        /// 朝鲜语
        /// </summary>
        Korean,
        /// <summary>
        /// 拉丁语
        /// </summary>
        Latin,
        /// <summary>
        /// 马来语
        /// </summary>
        Malay,
        /// <summary>
        /// 蒙古语
        /// </summary>
        Mongolian,
        /// <summary>
        /// 波斯语
        /// </summary>
        Persian,
        /// <summary>
        /// 波兰语
        /// </summary>
        Polish,
        /// <summary>
        /// 葡萄牙语
        /// </summary>
        Portugese,
        /// <summary>
        /// 罗马尼亚语
        /// </summary>
        Romanian,
        /// <summary>
        /// 俄语
        /// </summary>
        Russian,
        /// <summary>
        /// 西班牙语
        /// </summary>
        Spanish,
        /// <summary>
        /// 瑞典语
        /// </summary>
        Swedish,
        /// <summary>
        /// 泰语
        /// </summary>
        Thai,
        /// <summary>
        /// 土耳其语
        /// </summary>
        Turkish,
        /// <summary>
        /// 乌克兰语
        /// </summary>
        Ukranian,
        /// <summary>
        /// 越南语
        /// </summary>
        Vietnamese,
        /// <summary>
        /// 威尔士语
        /// </summary>
        Welsh,
    }

    public class GoogleTranslator
    {
        private static readonly Dictionary<GoogleTranslateLanguage, string> languageCodes;

        static GoogleTranslator()
        {
            languageCodes = new Dictionary<GoogleTranslateLanguage, string>()
            {
                { GoogleTranslateLanguage.Arabic, "ar" },
                { GoogleTranslateLanguage.Belarusian, "be" },
                { GoogleTranslateLanguage.Bulgarian, "bg" },
                { GoogleTranslateLanguage.ChineseSimplified, "zh-CN" },
                { GoogleTranslateLanguage.ChineseTraditional, "zh-TW" },
                { GoogleTranslateLanguage.Croatian, "hr" },
                { GoogleTranslateLanguage.Czech, "cs" },
                { GoogleTranslateLanguage.Danish, "da" },
                { GoogleTranslateLanguage.Dutch, "nl" },
                { GoogleTranslateLanguage.English, "en" },
                { GoogleTranslateLanguage.Filipino, "tl" },
                { GoogleTranslateLanguage.Finnish, "fi" },
                { GoogleTranslateLanguage.French, "fr" },
                { GoogleTranslateLanguage.German, "de" },
                { GoogleTranslateLanguage.Greek, "el" },
                { GoogleTranslateLanguage.Hebrew, "iw" },
                { GoogleTranslateLanguage.Hindi, "hi" },
                { GoogleTranslateLanguage.Hungarian, "hu" },
                { GoogleTranslateLanguage.Icelandic, "is" },
                { GoogleTranslateLanguage.Indonesian, "id" },
                { GoogleTranslateLanguage.Irish, "ga" },
                { GoogleTranslateLanguage.Italian, "it" },
                { GoogleTranslateLanguage.Japanese, "ja" },
                { GoogleTranslateLanguage.Korean, "ko" },
                { GoogleTranslateLanguage.Latin, "la" },
                { GoogleTranslateLanguage.Malay, "ms" },
                { GoogleTranslateLanguage.Mongolian, "mn" },
                { GoogleTranslateLanguage.Persian, "fa" },
                { GoogleTranslateLanguage.Polish, "pl" },
                { GoogleTranslateLanguage.Portugese, "pt" },
                { GoogleTranslateLanguage.Romanian, "ro" },
                { GoogleTranslateLanguage.Russian, "ru" },
                { GoogleTranslateLanguage.Spanish, "es" },
                { GoogleTranslateLanguage.Swedish, "sv" },
                { GoogleTranslateLanguage.Thai, "th" },
                { GoogleTranslateLanguage.Turkish, "tr" },
                { GoogleTranslateLanguage.Ukranian, "uk" },
                { GoogleTranslateLanguage.Vietnamese, "vi" },
                { GoogleTranslateLanguage.Welsh, "cy" }
            };
        }

        public static TranslationResult Translate(string text, GoogleTranslateLanguage from = GoogleTranslateLanguage.ChineseSimplified, GoogleTranslateLanguage to = GoogleTranslateLanguage.English)
        {
            TranslationResult result = new TranslationResult()
            {
                SourceLanguage = languageCodes[from],
                TargetLanguage = languageCodes[to],
                SourceText = text,
                TargetText = "",
                FailedReason = ""
            };
            try
            {
                result.TranslationResultTypes = TranslationResultTypes.Successed;
                var googleTransResult = PrivateTranslate(text, result.SourceLanguage, result.TargetLanguage);
                result.SourceLanguage = googleTransResult.From;
                result.TargetText = googleTransResult.TargetText;
            }
            catch (Exception exception)
            {
                result.FailedReason = exception.Message;
                result.TranslationResultTypes = TranslationResultTypes.Failed;
            }
            return result;
        }

        private static GoogleTransResult PrivateTranslate(string text, string from = "en", string to = "zh-CN")
        {
            if (!(text.Length > 0 && text.Length < 5000))
            {
                return new GoogleTransResult()
                {
                    From = "Unknown",
                    TargetText = "Only 5000 letters!"
                };
            }
            var url = $"https://translate.google.cn/m?sl={from}&tl={to}&hl={to}&q={HttpUtility.UrlEncode(text)}";
            var html = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = "Mozilla/5.0 (Linux; Android 10; GM1910) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.85 Mobile Safari/537.36";
            using (var response = httpWebRequest.GetResponse())
            {
                using Stream stream = response.GetResponseStream();
                if (stream == null)
                {
                    return null;
                }
                using var sr = new StreamReader(stream);
                html = sr.ReadToEnd();
            }

            var match = new Regex(@"<div[^>]*?class=""result-container""[^>]*>([\s\S]*?)<\/div>").Match(html);
            if (match.Success)
            {
                string matchText = new Regex(@"(<\/?[^>]+>)").Replace(match.Groups[1].Value, string.Empty);
                return new GoogleTransResult()
                {
                    From = from,
                    TargetText = HttpUtility.HtmlDecode(matchText)
                };
            }
            return new GoogleTransResult()
            {
                From = "Unknown",
                TargetText = "No Result!"
            };
        }
    }
}
