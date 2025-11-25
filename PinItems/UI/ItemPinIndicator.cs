using Duckov.UI;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace PinItems.UI
{
    internal class ItemPinIndicator : MonoBehaviour
    {
        private ItemDisplay? _display;
        private Item? _item;
        private Image? _icon;

        internal void Bind(ItemDisplay display)
        {
            _display = display;
            _item = display?.Target;
            EnsureIcon();
            Refresh();
        }

        private void OnEnable()
        {
            PinnedItemRegistry.PinStateChanged += OnPinStateChanged;
            Refresh();
        }

        private void OnDisable()
        {
            PinnedItemRegistry.PinStateChanged -= OnPinStateChanged;
        }

        private void OnDestroy()
        {
            PinnedItemRegistry.PinStateChanged -= OnPinStateChanged;
        }

        private void OnPinStateChanged(Item changedItem, bool _)
        {
            if (_item == changedItem || _display?.Target == changedItem)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            if (_display == null)
            {
                return;
            }

            _item = _display.Target;
            bool shouldShow = _item != null && PinnedItemRegistry.IsPinned(_item);
            if (_icon == null)
            {
                EnsureIcon();
            }
            if (_icon != null)
            {
                Sprite? sprite = ModBehaviour.PinSprite;
                _icon.sprite = sprite;
                _icon.gameObject.SetActive(shouldShow && sprite != null);
            }
        }

        private void EnsureIcon()
        {
            if (_icon != null || _display == null)
            {
                return;
            }

            GameObject indicatorObject = new GameObject("PinIndicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            indicatorObject.transform.SetParent(_display.transform, false);
            RectTransform rect = indicatorObject.GetComponent<RectTransform>();
            rect.pivot = new Vector2(1f, 1f);
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-4f, -4f);
            rect.sizeDelta = new Vector2(18f, 18f);

            _icon = indicatorObject.GetComponent<Image>();
            _icon.raycastTarget = false;
            _icon.gameObject.SetActive(false);
        }
    }
}
