# FourEssentials

A FourKit server plugin written in C#. Provides the essential commands every server needs — teleportation, homes, warps, moderation, chat tools, and more.

**Version:** 1.0.1
**Author:** Byte_HD

---

## Building

Requires the `Minecraft.Server.FourKit.dll` placed at `../runtime/Minecraft.Server.FourKit.dll` relative to the project root.

```
dotnet build -c Release
```

Output lands in `bin/Release/net10.0/FourEssentials.dll`.

---

## Commands

### Player

<img width="556" height="102" alt="fly" src="https://github.com/user-attachments/assets/a5ec2cb2-e168-4407-9379-0814440d1104" />

| Command | Aliases | Description |
|---|---|---|
| `/heal [player]` | | Heals you or the target player to full health. |
| `/feed [player]` | | Restores hunger and saturation. |
| `/fly [player] [on\|off]` | | Toggles flight for yourself or another player. |
| `/god [player] [on\|off]` | | Toggles god mode (damage immunity). |
| `/suicide` | | Kills yourself immediately. |
| `/more` | | Fills your held item stack to 64. |
| `/repair [hand\|all]` | | Repairs durability of held item or entire inventory. |
| `/speed [fly\|walk] <value> [player]` | | Sets walk or fly speed (0.0001–10). |
| `/getpos [player]` | `pos`, `coords`, `position` | Shows world, coordinates, yaw, pitch, and distance to target. |
| `/exp [give\|set\|show] [player] [amount]` | `xp` | Manages player experience. |

<img width="556" height="102" alt="fly" src="https://github.com/user-attachments/assets/080f18be-4afc-4dd9-97d4-6a97fe7ee8a2" />

### Gamemode

<img width="700" height="415" alt="gmc" src="https://github.com/user-attachments/assets/affae8a7-389f-4057-8bbb-a36305e0423d" />

| Command | Aliases | Description |
|---|---|---|
| `/gamemode <mode> [player]` | `gm` | Sets gamemode by name or number. |
| `/gms [player]` | | Survival. |
| `/gmc [player]` | | Creative. |
| `/gma [player]` | | Adventure. |

Mode values accepted: `survival`, `s`, `0`, `creative`, `c`, `1`, `adventure`, `a`, `2`.

### Teleportation

<img width="592" height="159" alt="teleporting" src="https://github.com/user-attachments/assets/6d646fba-2a7b-4e74-8273-4bbd8b4831a2" />

| Command | Aliases | Description |
|---|---|---|
| `/tp <player>` | | Teleport to a player. |
| `/tp <playerA> <playerB>` | | Teleport A to B. |
| `/tp <x> <y> <z>` | | Teleport to coordinates. |
| `/tphere <player>` | `tph` | Bring a player to you. |
| `/tppos <x> <y> <z>` | | Teleport yourself to coordinates. |
| `/tpall [player]` | | Teleport all online players to a player (or yourself). |
| `/spawn` | | Teleport to world spawn (0, 64, 0). |
| `/back` | | Return to your previous location before the last teleport or death. |

Coordinate values are capped at ±30,000,000.

### Homes

<img width="494" height="235" alt="home" src="https://github.com/user-attachments/assets/0657a064-2798-494a-b7fc-5ffc6f14f08a" />

| Command | Description |
|---|---|
| `/sethome [name]` | Set a home at your current location. Defaults to `home`. |
| `/home [name]` | Teleport to a home. Supports `player:home` syntax. |
| `/delhome <name>` | Delete a home. Supports `player:home` syntax. |
| `/homes` | List all your homes. |

The name `bed` is reserved and cannot be used.

### Warps

<img width="509" height="294" alt="warping" src="https://github.com/user-attachments/assets/0c374d81-76ee-4e71-a96a-e00b0ce38836" />

| Command | Description |
|---|---|
| `/setwarp <name>` | Create a warp at your current location. |
| `/warp <name>` | Warp to the specified location. |
| `/warp [page]` | List all warps, paginated at 20 per page. |
| `/delwarp <name>` | Delete a warp. |
| `/warps` | List all warps. |

Warp names cannot be purely numeric.

### Chat & Social

<img width="718" height="146" alt="messaging" src="https://github.com/user-attachments/assets/2e9ad1e8-3a64-4081-ae76-1f39af2de6e4" />

<img width="750" height="93" alt="broadcasting" src="https://github.com/user-attachments/assets/b908d7f7-7e71-4330-ac18-74af2061e5cf" />

| Command | Aliases | Description |
|---|---|---|
| `/msg <player> <message>` | `tell`, `w`, `whisper`, `pm` | Send a private message. |
| `/r <message>` | `reply` | Reply to the last player who messaged you. |
| `/me <action>` | | Broadcast an action message. |
| `/broadcast <message>` | `bcast`, `bc` | Broadcast a message to the whole server. |
| `/afk [player]` | | Toggle AFK status. Auto-clears on movement or chat. |
| `/nick [player] <name\|off>` | | Set or remove a nickname. Supports color codes (`§`). |

<img width="613" height="188" alt="Nick" src="https://github.com/user-attachments/assets/a4fef104-0165-4114-aa21-875f3e7c4d84" />

Nick rules: alphanumeric + color codes only, max 30 characters, must not match another online player's name or nick.

### Player Info

<img width="726" height="150" alt="info" src="https://github.com/user-attachments/assets/30899722-5af4-4317-8862-a75be32b0570" />

| Command | Aliases | Description |
|---|---|---|
| `/list` | `who`, `online` | List online (non-vanished) players with AFK tags. |
| `/seen <player>` | | Shows when a player was last online, or their join date if online now. |
| `/whois <player>` | | Full info: health, hunger, exp, location, IP, gamemode, god, fly, AFK, muted. |
| `/realname <nick>` | | Find the real username behind a nickname. |
| `/ping` | | Replies with `Pong!` (or echoes your arguments back). |
| `/gc` | | Shows server uptime, TPS, and memory usage. |

### Moderation
> I have not found any operator system for this yet! Clone the source and remove if you don't want these.

<img width="714" height="310" alt="banUsers" src="https://github.com/user-attachments/assets/616f6bbc-0cd0-4548-849c-a3a2c0100d81" />

| Command | Description |
|---|---|
| `/kick <player> [reason]` | Kicks a player. Broadcasts the reason. |
| `/kickall [reason]` | Kicks all players except the sender. |
| `/ban <player> [reason]` | Bans and kicks a player. |
| `/mute <player>` | Mutes or unmutes a player (toggles). Blocks chat while muted. |
| `/unmute <player>` | Unmutes a player. |
| `/kill <player>` | Instantly kills a player. |
| `/vanish [player] [on\|off]` | `v` | Hides a player from other players and command output. |

### Time



| Command | Description |
|---|---|
| `/ptime <time\|reset\|get> [player\|*]` | Set, reset, or view a player's client-side time. |

Named time values: `day` (1000), `noon` (6000), `night` (13000), `midnight` (18000), `dawn`/`sunrise` (23000), `dusk`/`sunset` (12500). Prefix with `@` to set an absolute time rather than relative.

---

## Stores

All data is held in memory. Nothing is persisted to disk across restarts.

| Store | Holds |
|---|---|
| `PlayerDataStore` | God mode, AFK state, last location, reply target, first/last seen timestamps |
| `WarpStore` | Named warp locations |
| `HomeStore` | Per-player named home locations |
| `NickStore` | Player nicknames |
| `MuteStore` | Muted player names |
| `VanishStore` | Vanished player names |

---

## Events (CoreListener)

| Event | Behaviour |
|---|---|
| `PlayerJoinEvent` | Restores nickname, records first/last seen, hides join message if vanished |
| `PlayerQuitEvent` | Updates last seen, hides quit message if vanished |
| `PlayerChatEvent` | Blocks chat if muted, applies nickname to chat format, clears AFK on chat |
| `PlayerMoveEvent` | Clears AFK state on significant XZ movement |
| `PlayerDeathEvent` | Saves death location for `/back`, notifies player |
| `EntityDamageEvent` | Cancels damage if the entity is a player in god mode |

---

## Utilities

### `H` (Helpers)

| Method | Description |
|---|---|
| `H.FindPlayer(sender, name)` | Looks up an online player by name, sends `PlayerNotFound` on failure |
| `H.RequirePlayer(sender)` | Returns the sender as a Player or sends `OnlyPlayers` and returns null |
| `H.TeleportWithBack(player, location)` | Saves current location to `LastLoc` then teleports |
| `H.ParseMode(string)` | Parses a gamemode string to `GameMode?` |
| `H.DN(player)` | Returns display name (nick if set, otherwise username) |
| `H.SN(sender)` | Returns sender display name, or `Console` |
| `H.TryParseTicks(string, out long)` | Parses a named time or raw tick count |
| `H.FormatTicks(long)` | Converts tick count back to a named time if possible |

### `M` (Messages)

All player-facing strings are defined as constants in `M`. Formatting is done via `M.Format(template, args...)`, which wraps `string.Format` with a fallback for malformed templates.

### `FourEssentials` (Binary helpers)

| Method | Description |
|---|---|
| `WriteFloat(buffer, offset, value)` | Writes a big-endian float into a byte array |
| `WriteInt(buffer, offset, value)` | Writes a big-endian int into a byte array |
| `WriteShort(buffer, offset, value)` | Writes a big-endian short into a byte array |

These are used by `FlyCommand` and `SpeedCommand` to send raw ability packets to the client.
