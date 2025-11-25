using System;
using Duckov.UI;
using HarmonyLib;
using ItemStatsSystem;
using PinItems.Utils;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PinItems.UI
{
    internal class PinMenuButtonController : MonoBehaviour
    {
        private ItemOperationMenu? _menu;
        private Button? _pinButton;
        private TextMeshProUGUI? _label;
        private bool _lastPinnedState;
        private static readonly Func<ItemOperationMenu, Item?> TargetGetter = BuildTargetGetter();

        internal void EnsureButton(ItemOperationMenu menu, Button wishlistButton)
        {
            _menu = menu;
            if (_pinButton != null || wishlistButton == null)
            {
                if (_pinButton != null)
                {
                    PinLogger.Info("PinMenuButtonController.EnsureButton called but pin button already exists.");
                }
                else
                {
                    PinLogger.Warn("PinMenuButtonController.EnsureButton missing wishlist button reference.");
                }
                return;
            }

            _pinButton = Instantiate(wishlistButton, wishlistButton.transform.parent);
            _pinButton.name = "btn_Pin";
            _pinButton.transform.SetSiblingIndex(wishlistButton.transform.GetSiblingIndex() + 1);
            StripLocalizationComponents(_pinButton);
            _pinButton.onClick.RemoveAllListeners();
            _pinButton.onClick.AddListener(OnPinButtonClicked);
            _label = _pinButton.GetComponentInChildren<TextMeshProUGUI>(true);
            UpdateLabelText();
            _pinButton.gameObject.SetActive(false);
            PinLogger.Info($"Pin button cloned for menu '{menu.name}'.");
        }

        internal void RefreshButton()
        {
            if (_menu == null || _pinButton == null)
            {
                PinLogger.Warn("PinMenuButtonController.RefreshButton missing menu or pin button instance.");
                return;
            }

            Item? item = GetTargetItem();
            bool eligible = PinnedItemRegistry.IsEligible(item);
            bool visible = eligible;
            _pinButton.gameObject.SetActive(visible);
            if (!visible)
            {
                if (item == null)
                {
                    PinLogger.Warn("Pin button hidden because ItemOperationMenu target item is null.");
                }
                else if (!eligible)
                {
                    PinLogger.Info($"Pin button hidden: target '{item.DisplayName}' is not eligible.");
                }
                return;
            }

            if (item == null)
            {
                PinLogger.Warn("Pin button refresh aborted because target item disappeared unexpectedly.");
                _pinButton.gameObject.SetActive(false);
                return;
            }

            bool pinned = PinnedItemRegistry.IsPinned(item);
            _lastPinnedState = pinned;
            UpdateLabelText();
            _pinButton.interactable = true;
            PinLogger.Info($"Pin button shown for '{item.DisplayName}' (pinned = {pinned}).");
        }

        private void OnEnable()
        {
            PinnedItemRegistry.PinStateChanged += OnPinStateChanged;
            LocalizationManager.OnSetLanguage += OnLanguageChanged;
        }

        private void OnDisable()
        {
            PinnedItemRegistry.PinStateChanged -= OnPinStateChanged;
            LocalizationManager.OnSetLanguage -= OnLanguageChanged;
        }

        private void OnPinStateChanged(Item changedItem, bool _)
        {
            Item? target = GetTargetItem();
            if (changedItem == target)
            {
                RefreshButton();
            }
        }

        private void OnLanguageChanged(SystemLanguage _)
        {
            UpdateLabelText();
        }

        private void OnPinButtonClicked()
        {
            Item? item = GetTargetItem();
            if (item == null)
            {
                PinLogger.Warn("Pin button clicked but no target item was found.");
                return;
            }
            PinLogger.Info($"Pin button clicked for '{item.DisplayName}'.");
            PinnedItemRegistry.Toggle(item);
            RefreshButton();
        }

        private Item? GetTargetItem()
        {
            if (_menu == null)
            {
                return null;
            }

            try
            {
                return TargetGetter(_menu);
            }
            catch (Exception ex)
            {
                PinLogger.Error($"Failed to read ItemOperationMenu target: {ex.Message}");
                return null;
            }
        }

        private static Func<ItemOperationMenu, Item?> BuildTargetGetter()
        {
            var getter = AccessTools.PropertyGetter(typeof(ItemOperationMenu), "TargetItem");
            if (getter == null)
            {
                return _ => null;
            }

            try
            {
                return (Func<ItemOperationMenu, Item?>)Delegate.CreateDelegate(
                    typeof(Func<ItemOperationMenu, Item?>), getter);
            }
            catch (Exception ex)
            {
                PinLogger.Error($"Failed to cache TargetItem getter: {ex.Message}");
                return _ => null;
            }
        }

        private void StripLocalizationComponents(Button button)
        {
            MonoBehaviour[] components = button.GetComponentsInChildren<MonoBehaviour>(true);
            bool removed = false;
            foreach (MonoBehaviour component in components)
            {
                if (component == null)
                {
                    continue;
                }

                Type type = component.GetType();
                string? ns = type.Namespace;
                if (!string.IsNullOrEmpty(ns) && ns.StartsWith("SodaCraft.Localizations", StringComparison.Ordinal))
                {
                    Destroy(component);
                    removed = true;
                }
            }

            if (!removed)
            {
                PinLogger.Warn("PinMenuButtonController could not find localization component on pin button clone.");
            }
        }

        private void UpdateLabelText()
        {
            if (_label == null)
            {
                return;
            }

            _label.text = PinLocalization.GetPinLabel(_lastPinnedState);
        }
    }
}
