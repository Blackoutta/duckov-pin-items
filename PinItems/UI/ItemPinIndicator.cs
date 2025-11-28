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
        private int _targetTypeId = -1;
        private InventoryEntry? _entry;
        private bool _isPlayerStorageDisplay;

        internal void Bind(ItemDisplay display)
        {
            _display = display;
            CacheInventoryEntry();
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
            _display = null;
            _entry = null;
            _item = null;
            _isPlayerStorageDisplay = false;
            _targetTypeId = -1;
        }

        private void OnPinStateChanged(int changedTypeId, bool _)
        {
            if (_display == null)
            {
                return;
            }

            if (_targetTypeId != changedTypeId)
            {
                Item? current = _display.Target;
                if (current == null || current.TypeID != changedTypeId)
                {
                    return;
                }
            }

            Refresh();
        }

        private void Refresh()
        {
            if (_display == null)
            {
                return;
            }

            UpdateStorageContext();
            _item = _display.Target;
            _targetTypeId = _item?.TypeID ?? -1;
            bool validateOwnership = true;
            if (_isPlayerStorageDisplay)
            {
                validateOwnership = false;
            }
            else if (_item != null && _item.IsInPlayerStorage())
            {
                validateOwnership = false;
            }
            bool shouldShow = _item != null && PinnedItemRegistry.IsPinned(_item, validateOwnership);
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
            rect.pivot = new Vector2(0f, 1f);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(4f, -4f);
            rect.sizeDelta = new Vector2(18f, 18f);

            _icon = indicatorObject.GetComponent<Image>();
            _icon.raycastTarget = false;
            _icon.gameObject.SetActive(false);
        }

        private void CacheInventoryEntry()
        {
            if (_display == null)
            {
                _entry = null;
                _isPlayerStorageDisplay = false;
                return;
            }

            _entry = _display.GetComponentInParent<InventoryEntry>();
            UpdateStorageContext();
        }

        private void UpdateStorageContext()
        {
            if (_entry == null)
            {
                _isPlayerStorageDisplay = false;
                return;
            }

            InventoryDisplay? master = _entry.Master;
            Inventory? target = master?.Target;
            _isPlayerStorageDisplay = target != null && target == PlayerStorage.Inventory;
        }
    }
}
