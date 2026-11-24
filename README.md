# ShutUp

ShutUp is a mod that mutes the sound effects you pick, and only those. Volume settings, music,
ambient loops and every other sound stay untouched.

# Configuration

The first launch creates `BepInEx/config/orax.ShutUp.cfg`.

## Finding the name of a sound

1. Set `Log sound names` to `true` and start the game.
2. Reproduce the sound you want to get rid of.
3. Look for `Sound effect name: <name>` lines in `BepInEx/LogOutput.log`.
4. Put the name in `Muted sounds`, one per entry, for example:
   `sfx_seagull_idle,sfx_deer_idle,sfx_shieldgenerator_powered_loop`

# Installation

This mod is client-side only.

## Using a mod manager (recommended)

You can install the mod using Vortex or Gale.

## Manual

Install BepInExPack for Valheim. Copy the DLL file to
`Steam\steamapps\common\Valheim\BepInEx\plugins\`

# Other mods

- [SilencePlease](https://thunderstore.io/c/valheim/p/Vismeneer/SilencePlease/) by Vismeneer

This mod is very similar to mine, but as of version 1.1.2 (2026-11-25), it is flagged as
incompatible with Valheim Plus. While I don't know the exact reason for that incompatibility, my mod
should work alongside Valheim Plus without issues.

# Misc.

My Valheim mods: [Nexus Mods](https://www.nexusmods.com/profile/0R4X/mods?gameId=3667) |
[Thunderstore](https://thunderstore.io/c/valheim/p/orax/) |
[Hexium](https://valheim.hexium.gg/?q=orax)

Source code: https://github.com/orax-Valheim-mods/

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/S6S1LBNIQ)
