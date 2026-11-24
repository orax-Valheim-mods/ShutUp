using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ShutUp;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
internal class ShutUp : BaseUnityPlugin
{
    public const string PluginGUID = "orax.ShutUp";
    public const string PluginName = "ShutUp";
    public const string PluginVersion = "0.1.0";

    internal static ManualLogSource Log;

    private static readonly Regex CloneSuffixRegex = new(@"(?:\(Clone\))+$", RegexOptions.Compiled);

    private static ConfigEntry<bool> _enabled;
    private static ConfigEntry<string> _mutedSounds;
    private static ConfigEntry<bool> _logSoundNames;

    // Sound names the player asked to mute, compared case-sensitively as whole names.
    private static readonly HashSet<string> MutedNames = new(StringComparer.Ordinal);

    private Harmony _harmony;

    private void Awake()
    {
        Log = base.Logger;

        _enabled = Config.Bind(
            "General",
            "Enabled",
            true,
            "Set to false to disable the mod without uninstalling it."
        );

        _mutedSounds = Config.Bind(
            "Sounds",
            "Muted sounds",
            "sfx_chicken_eat,sfx_chicken_idle_vocal,sfx_deer_alerted,sfx_deer_idle,sfx_wolf_haul",
            "Comma separated list of the exact sound names to mute.\n"
                + "Names are matched case-sensitively, and the whole name has to match: 'sfx_wolf' does not mute 'sfx_wolf_haul'.\n"
                + "The '(Clone)' suffix is ignored, so names copied from the log work as they are.\n"
                + "Turn on 'Log sound names' to find the names the game uses: they are written to the BepInEx log."
        );

        _logSoundNames = Config.Bind(
            "Debug",
            "Log sound names",
            false,
            "Write the name of every sound effect created by the game to the BepInEx log.\n"
                + "Useful to find out which names to put in 'Muted sounds'."
        );

        _enabled.SettingChanged += OnSettingsChanged;
        _mutedSounds.SettingChanged += OnSettingsChanged;
        RebuildMuteList();

        _harmony = new Harmony(PluginGUID);
        _harmony.PatchAll();

        Log.LogInfo($"{PluginName} has landed. {MutedNames.Count} sound name(s) muted.");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }

    private static void OnSettingsChanged(object sender, EventArgs e)
    {
        RebuildMuteList();
    }

    private static void RebuildMuteList()
    {
        MutedNames.Clear();

        if (!_enabled.Value || string.IsNullOrEmpty(_mutedSounds.Value))
        {
            return;
        }

        foreach (string entry in _mutedSounds.Value.Split(','))
        {
            string name = entry.Trim();
            if (name.Length > 0)
            {
                MutedNames.Add(name);
            }
        }
    }

    /// <summary>
    /// Every sound effect in Valheim is a ZSFX component, so muting its AudioSource is enough
    /// to silence it while leaving the rest of the game's audio untouched.
    /// </summary>
    [HarmonyPatch(typeof(ZSFX), nameof(ZSFX.Awake))]
    private static class ZSFXMutePatch
    {
        private static void Postfix(ZSFX __instance)
        {
            // Removes the "(Clone)" suffix Unity adds to instances, plus surrounding whitespace,
            // so the name shown in the log, the one typed in the config and the one compared match.
            string soundName = CloneSuffixRegex
                .Replace(__instance.gameObject.name.Trim(), string.Empty)
                .Trim();

            if (_logSoundNames.Value)
            {
                Log.LogInfo($"Sound effect name: {soundName}");
            }

            if (MutedNames.Contains(soundName))
            {
                __instance.GetComponent<AudioSource>().mute = true;
                Log.LogDebug($"Muted sound effect: {soundName}");
            }
        }
    }
}
