I'm creating a Mod for the game Escape From Duckov
- The game mod should be written in `PinItems` folder
- the mod allows players to right-click on an item in the inventory, then click on `Pin` button 
- the `Pin` button does the following:
  - displays a pin-icon on top of the item sprite(this icon I can provider)
  - the item is now in a `pined` status, dis-allowing it from being moved from player inventory to storage when the player clicks `StoreAll` button. though the player is allowed to move it with other methods.

# Your Task
- Analyze my requirements and come up with a detailed implementation plan
- write your plan as a file called pinItemsPlan.md

# Notable Things & Contexts
## General Contexts
- the decompilation(api reference) lives in `duckovAPI/Decompilation
- you can have a look at `README_EN.md` for general context
- the `DisplayItemValue` is an example Mod you can learn from

## Item Operation Menu(the right-click menu)
- The right-click “Mark / Add to wishlist” entry lives in TeamSoda.Duckov.Core/Duckov/UI/
ItemOperationMenu.cs:90. During Initialize() the field btn_Wishlist has its click handler wired up to Wishlist()
(this.btn_Wishlist.onClick.AddListener(new UnityAction(this.Wishlist));), so this is the actual button definition in
the context menu that appears when you right-click an item.
- The handler itself is ItemOperationMenu.Wishlist() in the same file (TeamSoda.Duckov.Core/
  Duckov/UI/ItemOperationMenu.cs:95). It toggles the manual wishlist entry for the currently
  selected item by calling ItemWishlist.RemoveFromWishlist(typeID) if already marked, otherwise
  ItemWishlist.AddToWishList(this.TargetItem.TypeID).

## The Store-All button
- In TeamSoda.Duckov.Core/Duckov/UI/LootView.cs:84 the storeAllButton’s onClick handler is registered during Awake,
  pointing at OnStoreAllButtonClicked(). This view is the shared inventory/loot UI panel that shows both the player’s
  inventory and the currently open storage/loot container.
- The serialized button itself is declared at TeamSoda.Duckov.Core/Duckov/UI/LootView.cs:626. Unity wires this field
  to the actual UI button prefab, so activating/deactivating it is handled in the same class (LootView.Setup enables
  the button only when the current TargetInventory is the global PlayerStorage.Inventory).
- OnStoreAllButtonClicked (same file lines 88‑126) implements the “dump all” behaviour: it iterates through
  the player character’s inventory, skipping locked slots, and moves each item into the target storage via
  TargetInventory.AddAndMerge.

## Patching
- consider using harmony patching to achieve this feature if its better