using SodaCraft.Localizations;
using UnityEngine;

namespace PinItems.Utils
{
    internal static class PinLocalization
    {
        private const string PinKey = "Pin";
        private const string UnpinKey = "Unpin";

        internal static string GetPinLabel(bool pinned) => Get(pinned ? UnpinKey : PinKey);

        internal static string Get(string key)
        {
            SystemLanguage language = LocalizationManager.CurrentLanguage;
            return language switch
            {
                SystemLanguage.ChineseSimplified => key switch
                {
                    PinKey => "固定",
                    UnpinKey => "取消固定",
                    _ => key
                },
                SystemLanguage.ChineseTraditional => key switch
                {
                    PinKey => "固定",
                    UnpinKey => "取消固定",
                    _ => key
                },
                SystemLanguage.Japanese => key switch
                {
                    PinKey => "ピン留め",
                    UnpinKey => "ピン留め解除",
                    _ => key
                },
                SystemLanguage.Korean => key switch
                {
                    PinKey => "고정하기",
                    UnpinKey => "고정 해제",
                    _ => key
                },
                SystemLanguage.Russian => key switch
                {
                    PinKey => "Закрепить",
                    UnpinKey => "Открепить",
                    _ => key
                },
                SystemLanguage.German => key switch
                {
                    PinKey => "Anheften",
                    UnpinKey => "Lösen",
                    _ => key
                },
                SystemLanguage.Spanish => key switch
                {
                    PinKey => "Fijar",
                    UnpinKey => "Quitar fijación",
                    _ => key
                },
                SystemLanguage.French => key switch
                {
                    PinKey => "Épingler",
                    UnpinKey => "Désépingler",
                    _ => key
                },
                _ => key switch
                {
                    PinKey => "Pin",
                    UnpinKey => "Unpin",
                    _ => key
                }
            };
        }
    }
}
