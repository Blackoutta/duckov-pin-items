using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace PinItems
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony? _harmony;
        private Sprite? _pinSprite;
        private Texture2D? _pinTexture;

        internal static ModBehaviour? Instance { get; private set; }
        internal static Sprite? PinSprite => Instance?._pinSprite;

        private void Awake()
        {
            PinLogger.Info("PinItems Awakened!");
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            PinLogger.Info("Loading Sprite...");
            LoadPinSprite();
            PinLogger.Info("Patching with Harmony...");
            _harmony = new Harmony("pinitems.mod");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            PinLogger.Info("PinItems loaded");
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            Instance = null;
            try
            {
                if (_harmony != null)
                {
                    _harmony.UnpatchAll(_harmony.Id);
                    _harmony = null;
                }
            }
            catch (Exception ex)
            {
                PinLogger.Error($"Failed to unpatch: {ex}");
            }

            PinnedItemRegistry.Clear();
            if (_pinSprite != null)
            {
                Destroy(_pinSprite);
                _pinSprite = null;
            }
            if (_pinTexture != null)
            {
                Destroy(_pinTexture);
                _pinTexture = null;
            }
        }

        private void LoadPinSprite()
        {
            PinLogger.Info("Loading pin sprite resources.");
            try
            {
                string asmLocation = Assembly.GetExecutingAssembly().Location;
                string? folder = Path.GetDirectoryName(asmLocation);
                if (!string.IsNullOrEmpty(folder))
                {
                    string candidate = Path.Combine(folder, "pin.png");
                    if (File.Exists(candidate))
                    {
                        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        texture.name = "PinItems.PinTexture";
                        byte[] bytes = File.ReadAllBytes(candidate);
                        if (texture.LoadImage(bytes))
                        {
                            _pinTexture = texture;
                            _pinSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f));
                            _pinSprite.name = "PinItems.PinSprite";
                            PinLogger.Info($"Loaded custom pin icon from '{candidate}'.");
                            return;
                        }
                        Destroy(texture);
                        PinLogger.Warn($"pin.png was found but failed to load, falling back to default sprite.");
                    }
                }
            }
            catch (Exception ex)
            {
                PinLogger.Warn($"Failed to load external pin icon: {ex.Message}");
            }

            // Fallback sprite (simple yellow circle)
            Texture2D fallbackTex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
            Color32 gold = new Color32(255, 211, 78, 255);
            Color32 clear = new Color32(0, 0, 0, 0);
            Vector2 center = new Vector2(7.5f, 7.5f);
            float radius = 7f;
            for (int y = 0; y < fallbackTex.height; y++)
            {
                for (int x = 0; x < fallbackTex.width; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    fallbackTex.SetPixel(x, y, Vector2.Distance(pos, center) <= radius ? gold : clear);
                }
            }
            fallbackTex.Apply();
            fallbackTex.name = "PinItems.DefaultPinTexture";
            _pinTexture = fallbackTex;
            _pinSprite = Sprite.Create(fallbackTex, new Rect(0, 0, fallbackTex.width, fallbackTex.height),
                new Vector2(0.5f, 0.5f));
            _pinSprite.name = "PinItems.DefaultPinSprite";
            PinLogger.Info("Initialized fallback pin icon sprite.");
        }
    }
}
