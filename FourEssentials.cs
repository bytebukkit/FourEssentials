using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using Minecraft.Server.FourKit.Event;
using Minecraft.Server.FourKit.Event.Entity;
using Minecraft.Server.FourKit.Event.Player;
using Minecraft.Server.FourKit.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Buffers.Binary;

public class FourEssentials : ServerPlugin
{

    internal readonly Dictionary<string, bool> flying = new(StringComparer.OrdinalIgnoreCase);

    public override string name    => "FourEssentials";
    public override string version => "1.0.0";
    public override string author  => "Byte_HD";

    internal static FourEssentials Instance { get; private set; } = null!;

    internal PlayerDataStore Data   { get; } = new();
    internal WarpStore       Warps  { get; } = new();
    internal HomeStore       Homes  { get; } = new();
    internal NickStore       Nicks  { get; } = new();
    internal MuteStore       Mutes  { get; } = new();
    internal VanishStore     Vanish { get; } = new();

    internal static void WriteFloat(byte[] buffer, int offset, float value)
    {
        Span<byte> tmp = stackalloc byte[4];
        BinaryPrimitives.WriteSingleBigEndian(tmp, value);
        tmp.CopyTo(buffer.AsSpan(offset, 4));
    }
    internal static void WriteInt(byte[] buffer, int offset, int value)
    {
        Span<byte> tmp = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(tmp, value);
        tmp.CopyTo(buffer.AsSpan(offset, 4));
    }

    internal static void WriteShort(byte[] buffer, int offset, short value)
    {
        Span<byte> tmp = stackalloc byte[2];
        BinaryPrimitives.WriteInt16BigEndian(tmp, value);
        tmp.CopyTo(buffer.AsSpan(offset, 2));
    }



    public override void onEnable()
    {
        Instance = this;
        FourKit.addListener(new CoreListener());

        Reg("heal",      "Heals you or the given player.",                         new HealCommand());
        Reg("feed",      "Satisfy the hunger.",                                    new FeedCommand());
        Reg("fly",       "Take off, and soar!",                                    new FlyCommand());
        Reg("god",       "Enables your godly powers.",                             new GodCommand());
        Reg("suicide",   "Causes you to perish.",                                  new SuicideCommand());
        Reg("more",      "Fills the item stack in hand to maximum size.",          new MoreCommand());
        Reg("repair",    "Repairs the durability of one or all items.",            new RepairCommand());
        Reg("speed",     "Change your speed limits.",                              new SpeedCommand());
        Reg("getpos",    "Get your current coordinates or those of a player.",     new GetPosCommand(),   "pos", "coords", "position");
        Reg("exp",       "Give, set, reset, or look at a players experience.",     new ExpCommand(),      "xp");
        Reg("gc",        "Reports memory, uptime and tick info.",                  new GcCommand());
        Reg("ping",      "Pong!",                                                  new PingCommand());

        Reg("gamemode",  "Change player gamemode.",                                new GamemodeCommand(), "gm");
        Reg("gms",       "Set gamemode to survival.",                              new GamemodeCommand());
        Reg("gmc",       "Set gamemode to creative.",                              new GamemodeCommand());
        Reg("gma",       "Set gamemode to adventure.",                             new GamemodeCommand());

        Reg("tp",        "Teleport to a player.",                                  new TpCommand());
        Reg("tphere",    "Teleport a player to you.",                              new TpHereCommand(),   "tph");
        Reg("tppos",     "Teleport to coordinates.",                               new TpPosCommand());
        Reg("tpall",     "Teleport all online players to another player.",         new TpAllCommand());
        Reg("back",      "Teleports you to your location prior to tp/spawn/warp.", new BackCommand());
        Reg("spawn",     "Teleport to world spawn.",                               new SpawnCommand());

        Reg("sethome",   "Set your home to your current location.",                new SetHomeCommand());
        Reg("home",      "Teleport to your home.",                                 new HomeCommand());
        Reg("delhome",   "Removes a home.",                                        new DelHomeCommand());
        Reg("homes",     "List your homes.",                                       new HomesCommand());

        Reg("setwarp",   "Creates a new warp.",                                    new SetWarpCommand());
        Reg("warp",      "List all warps or warp to the specified location.",      new WarpCommand());
        Reg("delwarp",   "Deletes the specified warp.",                            new DelWarpCommand());
        Reg("warps",     "List all warps.",                                        new WarpsCommand());

        Reg("nick",      "Change your nickname.",                                  new NickCommand());
        Reg("msg",       "Sends a private message.",                               new MsgCommand(),      "tell", "w", "whisper", "pm");
        Reg("r",         "Quickly reply to the last player to message you.",       new ReplyCommand(),    "reply");
        Reg("me",        "Describes an action in the context of the player.",      new MeCommand());
        Reg("broadcast", "Broadcasts a message to the entire server.",             new BroadcastCommand(),"bcast", "bc");
        Reg("afk",       "Marks you as away-from-keyboard.",                       new AfkCommand());
        Reg("list",      "List all online players.",                               new ListCommand(),     "who", "online");
        Reg("seen",      "Shows the last logout time of a player.",                new SeenCommand());
        Reg("whois",     "Determine basic information about the specified player.",new WhoisCommand());
        Reg("realname",  "Displays the username of a user based on nick.",         new RealNameCommand());
        Reg("vanish",    "Hide yourself from other players.",                      new VanishCommand(),   "v");
        Reg("ptime",     "Adjust player's client time.",                           new PTimeCommand());

        Reg("kick",      "Kicks a specified player with a reason.",                new KickCommand());
        Reg("kickall",   "Kicks all players off the server.",                      new KickAllCommand());
        Reg("ban",       "Bans a player.",                                         new BanCommand());
        Reg("mute",      "Mutes or unmutes a player.",                             new MuteCommand());
        Reg("unmute",    "Unmutes a player.",                                      new UnmuteCommand());
        Reg("kill",      "Kills specified player.",                                new KillCommand());

        Console.WriteLine("[FourEssentials] v" + version + " enabled.");
    }

    public override void onDisable() =>
        Console.WriteLine("[FourEssentials] Disabled.");

    private static void Reg(string name, string desc, CommandExecutor exec, params string[] aliases)
    {
        var c = FourKit.getCommand(name);
        c.setDescription(desc);
        c.setExecutor(exec);
        if (aliases.Length > 0) c.setAliases(new List<string>(aliases));
    }
}


public static class M
{
    public const string Heal         = "\u00a76You have been healed.";
    public const string HealOther    = "\u00a76Healed\u00a7c {0}\u00a76.";
    public const string HealDead     = "\u00a74You cannot heal someone who is dead!";
    public const string Feed         = "\u00a76Your appetite was sated.";
    public const string FeedOther    = "\u00a76You satiated the appetite of \u00a7c{0}\u00a76.";
    public const string FlyMode      = "\u00a76Set fly mode\u00a7c {0} \u00a76for {1}\u00a76.";
    public const string GodMode      = "\u00a76God mode\u00a7c {0}\u00a76.";
    public const string GodEnabled   = "\u00a7aenabled\u00a76 for\u00a7c {0}";
    public const string GodDisabled  = "\u00a7cdisabled\u00a76 for\u00a7c {0}";
    public const string GameMode     = "\u00a76Set game mode\u00a7c {0} \u00a76for \u00a7c{1}\u00a76.";
    public const string GameModeInv  = "\u00a74You need to specify a valid player/mode.";
    public const string Enabled      = "enabled";
    public const string Disabled     = "disabled";
    public const string Flying       = "flying";
    public const string Walking      = "walking";
    public const string Teleporting  = "\u00a76Teleporting...";
    public const string TeleportAtoB = "\u00a7c{0}\u00a76 teleported you to \u00a7c{1}\u00a76.";
    public const string TeleportDisabled = "\u00a7c{0} \u00a74has teleportation disabled.";
    public const string TeleportInvalid  = "Value of coordinates cannot be over 30000000";
    public const string BackUsage    = "\u00a76Returning to previous location.";
    public const string NoLocation   = "\u00a74No valid location found.";
    public const string HomeSet      = "\u00a76Home set to current location.";
    public const string InvalidHome  = "\u00a74Home\u00a7c {0} \u00a74doesn''t exist!";
    public const string InvalidHomeName = "\u00a74Invalid home name!";
    public const string DeleteHome   = "\u00a76Home\u00a7c {0} \u00a76has been removed.";
    public const string Homes        = "\u00a76Homes:\u00a7r {0}";
    public const string MaxHomes     = "\u00a74You cannot set more than\u00a7c {0} \u00a74homes.";
    public const string WarpSet      = "\u00a76Warp\u00a7c {0} \u00a76set.";
    public const string WarpingTo    = "\u00a76Warping to\u00a7c {0}\u00a76.";
    public const string WarpNotExist = "\u00a74That warp does not exist.";
    public const string WarpOverwrite= "\u00a74You cannot overwrite that warp.";
    public const string DeleteWarp   = "\u00a76Warp\u00a7c {0} \u00a76has been removed.";
    public const string Warps        = "\u00a76Warps:\u00a7r {0}";
    public const string WarpsCount   = "\u00a76There are\u00a7c {0} \u00a76warps. Showing page \u00a7c{1} \u00a76of \u00a7c{2}\u00a76.";
    public const string NoWarpsDef   = "\u00a76No warps defined.";
    public const string InvalidWarp  = "\u00a74Invalid warp name!";
    public const string NickSet      = "\u00a76Your nickname is now \u00a7c{0}\u00a76.";
    public const string NickNoMore   = "\u00a76You no longer have a nickname.";
    public const string NickChanged  = "\u00a76Nickname changed.";
    public const string NickInUse    = "\u00a74That name is already in use.";
    public const string NickAlpha    = "\u00a74Nicknames must be alphanumeric.";
    public const string NickTooLong  = "\u00a74That nickname is too long.";
    public const string MsgFormat    = "\u00a76[\u00a7c{0}\u00a76 -> \u00a7c{1}\u00a76] \u00a7r{2}";
    public const string ForeverAlone = "\u00a74You have nobody to whom you can reply.";
    public const string UserAFK      = "\u00a77{0} \u00a75is currently AFK and may not respond.";
    public const string Action       = "\u00a75* {0} \u00a75{1}";
    public const string Broadcast    = "\u00a7r\u00a76[\u00a74Broadcast\u00a76]\u00a7a {0}";
    public const string KickDefault  = "Kicked from server.";
    public const string KickExempt   = "\u00a74You cannot kick that person.";
    public const string KickedAll    = "\u00a74Kicked all players from server.";
    public const string PlayerKicked = "\u00a76Player\u00a7c {0} \u00a76kicked {1} for {2}.";
    public const string BanFormat    = "\u00a7cYou have been banned:\n\u00a7r{0}";
    public const string BanExempt    = "\u00a74You cannot ban that player.";
    public const string BanExemptOff = "\u00a74You may not ban offline players.";
    public const string DefaultBan   = "The Ban Hammer has spoken!";
    public const string PlayerBanned = "\u00a76Player\u00a7c {0} \u00a76banned\u00a7c {1} \u00a76for: \u00a7c{2}\u00a76.";
    public const string MutedPlayer  = "\u00a76Player\u00a7c {0} \u00a76muted.";
    public const string MutedFor     = "\u00a76Player\u00a7c {0} \u00a76muted for\u00a7c {1}\u00a76.";
    public const string YouMuted     = "\u00a76You have been muted!";
    public const string YouMutedFor  = "\u00a76You have been muted for\u00a7c {0}.";
    public const string MuteExempt   = "\u00a74You may not mute that player.";
    public const string MuteExemptOff= "\u00a74You may not mute offline players.";
    public const string MuteNotify   = "\u00a7c{0} \u00a76has muted player \u00a7c{1}\u00a76.";
    public const string UnmutedPlayer= "\u00a76Player\u00a7c {0} \u00a76unmuted.";
    public const string YouUnmuted   = "\u00a76You have been unmuted.";
    public const string Kill         = "\u00a76Killed\u00a7c {0}\u00a76.";
    public const string KillExempt   = "\u00a74You cannot kill \u00a7c{0}\u00a74.";
    public const string CurrentWorld = "\u00a76Current World:\u00a7c {0}";
    public const string PosX         = "\u00a76X: {0} (+East <-> -West)";
    public const string PosY         = "\u00a76Y: {0} (+Up <-> -Down)";
    public const string PosZ         = "\u00a76Z: {0} (+South <-> -North)";
    public const string PosYaw       = "\u00a76Yaw: {0} (Rotation)";
    public const string PosPitch     = "\u00a76Pitch: {0} (Head angle)";
    public const string Distance     = "\u00a76Distance: {0}";
    public const string MoveSpeed    = "\u00a76Set {0} speed to\u00a7c {1} \u00a76for \u00a7c{2}\u00a76.";
    public const string Repair       = "\u00a76You have successfully repaired your: \u00a7c{0}\u00a76.";
    public const string RepairNone   = "\u00a74There were no items that needed repairing.";
    public const string RepairFixed  = "\u00a74This item does not need repairing.";
    public const string RepairInvalid= "\u00a74This item cannot be repaired.";
    public const string RepairEnch   = "\u00a74You are not allowed to repair enchanted items.";
    public const string FullStack    = "\u00a74You already have a full stack.";
    public const string CantSpawn    = "\u00a74You are not allowed to spawn the item\u00a7c {0}\u00a74.";
    public const string SuicideMsg   = "\u00a76Goodbye cruel world...";
    public const string SuicideSucc  = "\u00a76Player \u00a7c{0} \u00a76took their own life.";
    public const string UserIsAway   = "\u00a77* {0} \u00a77is now AFK.";
    public const string UserIsNotAway= "\u00a77* {0} \u00a77is no longer AFK.";
    public const string ListAmount   = "\u00a76There are \u00a7c{0}\u00a76 out of maximum \u00a7c{1}\u00a76 players online.";
    public const string ListAfkTag   = "\u00a77[AFK]\u00a7r";
    public const string ConnPlayers  = "\u00a76Connected players\u00a7r";
    public const string Pong         = "Pong!";
    public const string Uptime       = "\u00a76Uptime:\u00a7c {0}";
    public const string TPS          = "\u00a76Current TPS = {0}";
    public const string GcMax        = "\u00a76Maximum memory:\u00a7c {0} MB.";
    public const string GcTotal      = "\u00a76Allocated memory:\u00a7c {0} MB.";
    public const string GcFree       = "\u00a76Free memory:\u00a7c {0} MB.";
    public const string GcWorld      = "\u00a76{0} \"\u00a7c{1}\u00a76\": \u00a7c{2}\u00a76 chunks, \u00a7c{3}\u00a76 entities, \u00a7c{4}\u00a76 tiles.";
    public const string Exp          = "\u00a7c{0} \u00a76has\u00a7c {1} \u00a76exp (level\u00a7c {2}\u00a76) and needs\u00a7c {3} \u00a76more exp to level up.";
    public const string ExpSet       = "\u00a7c{0} \u00a76now has\u00a7c {1} \u00a76exp.";
    public const string SeenOnline   = "\u00a76Player\u00a7c {0} \u00a76has been \u00a7aonline\u00a76 since \u00a7c{1}\u00a76.";
    public const string SeenOffline  = "\u00a76Player\u00a7c {0} \u00a76has been \u00a74offline\u00a76 since \u00a7c{1}\u00a76.";
    public const string UserUnknown  = "\u00a74Warning: The user ''\u00a7c{0}\u00a74'' has never joined this server.";
    public const string WhoisTop     = "\u00a76 ====== WhoIs:\u00a7c {0} \u00a76======";
    public const string WhoisNick    = "\u00a76 - Nick:\u00a7r {0}";
    public const string WhoisHealth  = "\u00a76 - Health:\u00a7r {0}/20";
    public const string WhoisHunger  = "\u00a76 - Hunger:\u00a7r {0}/20 (+{1} saturation)";
    public const string WhoisExp     = "\u00a76 - Exp:\u00a7r {0} (Level {1})";
    public const string WhoisLocation= "\u00a76 - Location:\u00a7r ({0}, {1}, {2}, {3})";
    public const string WhoisIP      = "\u00a76 - IP Address:\u00a7r {0}";
    public const string WhoisGamemode= "\u00a76 - Gamemode:\u00a7r {0}";
    public const string WhoisGod     = "\u00a76 - God mode:\u00a7r {0}";
    public const string WhoisOp      = "\u00a76 - OP:\u00a7r {0}";
    public const string WhoisFly     = "\u00a76 - Fly mode:\u00a7r {0} ({1})";
    public const string WhoisAFK     = "\u00a76 - AFK:\u00a7r {0}";
    public const string WhoisJail    = "\u00a76 - Jail:\u00a7r {0}";
    public const string WhoisMuted   = "\u00a76 - Muted:\u00a7r {0}";
    public const string TrueStr      = "\u00a7atrue\u00a7r";
    public const string FalseStr     = "\u00a74false\u00a7r";
    public const string NotFlying    = "not flying";
    public const string Vanished     = "\u00a76You are now completely invisible to normal users, and hidden from in-game commands.";
    public const string VanishMsg    = "\u00a76Vanish for {0}\u00a76: {1}";
    public const string PTimeNormal  = "\u00a7c{0}\u00a76's time is normal and matches the server.";
    public const string PTimeCurrent = "\u00a7c{0}\u00a76's time is\u00a7c {1}\u00a76.";
    public const string PTimeFixed   = "\u00a7c{0}\u00a76's time is fixed to\u00a7c {1}\u00a76.";
    public const string PTimeSet     = "\u00a76Player time is set to \u00a7c{0}\u00a76 for: \u00a7c{1}.";
    public const string PTimeSetFixed= "\u00a76Player time is fixed to \u00a7c{0}\u00a76 for: \u00a7c{1}.";
    public const string PTimeReset   = "\u00a76Player time has been reset for: \u00a7c{0}";
    public const string PTimePlayers = "\u00a76These players have their own time:\u00a7r";
    public const string PTimeOthPerm = "\u00a74You are not authorized to set other players' time.";
    public const string RealName     = "{0} is {1}";
    public const string SudoRun      = "\u00a76Forcing\u00a7c {0} \u00a76to run:\u00a7r /{1}";
    public const string SudoExempt   = "\u00a74You cannot sudo this user.";
    public const string NotEnoughArgs= "\u00a74Not enough arguments.";
    public const string PlayerNotFound="\u00a74Player not found.";
    public const string OnlyPlayers  = "\u00a74Only in-game players can use \u00a7c{0}\u00a74.";

    public static string Format(string template, params object[] args)
    {
        if (string.IsNullOrEmpty(template)) return string.Empty;
        try
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, template, args);
        }
        catch (FormatException)
        {
            string s = template;
            for (int i = 0; i < args.Length; i++)
                s = s.Replace("{" + i + "}", args[i]?.ToString() ?? "");
            return s;
        }
    }

}

public class PlayerDataStore
{
    private readonly Dictionary<string, PlayerData> _d = new(StringComparer.OrdinalIgnoreCase);
    public PlayerData Get(string n)
    {
        if (!_d.TryGetValue(n, out var d)) { d = new(); _d[n] = d; }
        return d;
    }
}

public class PlayerData
{
    public bool      GodMode   { get; set; }
    public bool      IsAfk     { get; set; }
    public Location? LastLoc   { get; set; }
    public string?   ReplyTo   { get; set; }
    public DateTime  LastSeen  { get; set; } = DateTime.UtcNow;
    public DateTime  FirstSeen { get; set; } = DateTime.UtcNow;
    public bool      SeenSet   { get; set; }
}

public class WarpStore
{
    private readonly Dictionary<string, Location> _w = new(StringComparer.OrdinalIgnoreCase);
    public void    Set(string n, Location l) => _w[n] = l;
    public bool    Delete(string n)           => _w.Remove(n);
    public Location? Get(string n)            => _w.TryGetValue(n, out var l) ? l : null;
    public IReadOnlyList<string> All()         => _w.Keys.OrderBy(k => k).ToList();
}

public class HomeStore
{
    private readonly Dictionary<string, Dictionary<string, Location>> _h = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, Location> Of(string p)
    {
        if (!_h.TryGetValue(p, out var d)) { d = new(StringComparer.OrdinalIgnoreCase); _h[p] = d; }
        return d;
    }
    public void    Set(string p, string n, Location l) => Of(p)[n] = l;
    public bool    Delete(string p, string n)           => Of(p).Remove(n);
    public Location? Get(string p, string n)            => Of(p).TryGetValue(n, out var l) ? l : null;
    public IReadOnlyList<string> All(string p)          => Of(p).Keys.OrderBy(k => k).ToList();
    public int     Count(string p)                      => Of(p).Count;
}

public class NickStore
{
    private readonly Dictionary<string, string> _n = new(StringComparer.OrdinalIgnoreCase);
    public void    Set(string p, string nick) => _n[p] = nick;
    public void    Remove(string p)           => _n.Remove(p);
    public string? Get(string p)              => _n.TryGetValue(p, out var n) ? n : null;
    public IEnumerable<(string real, string display)> FindByNick(string search)
        => _n.Where(kv => kv.Value.Replace("\u00a7", "§").ToLower().Contains(search.ToLower()))
             .Select(kv => (kv.Key, kv.Value));
}

public class MuteStore
{
    private readonly HashSet<string> _m = new(StringComparer.OrdinalIgnoreCase);
    public void Mute(string p)    => _m.Add(p);
    public void Unmute(string p)  => _m.Remove(p);
    public bool IsMuted(string p) => _m.Contains(p);
}

public class VanishStore
{
    private readonly HashSet<string> _v = new(StringComparer.OrdinalIgnoreCase);
    public void Set(string p, bool on) { if (on) _v.Add(p); else _v.Remove(p); }
    public bool IsVanished(string p)   => _v.Contains(p);
}

public class CoreListener : Listener
{
    private static FourEssentials Ess => FourEssentials.Instance;

    [EventHandler]
    public void onJoin(PlayerJoinEvent e)
    {
        var p    = e.getPlayer();
        var data = Ess.Data.Get(p.getName());
        if (!data.SeenSet) { data.FirstSeen = DateTime.UtcNow; data.SeenSet = true; }
        data.LastSeen = DateTime.UtcNow;
        data.IsAfk    = false;
        var nick = Ess.Nicks.Get(p.getName());
        if (nick != null) p.setDisplayName(nick);
        if (Ess.Vanish.IsVanished(p.getName())) e.setJoinMessage("");
    }

    [EventHandler]
    public void onQuit(PlayerQuitEvent e)
    {
        Ess.Data.Get(e.getPlayer().getName()).LastSeen = DateTime.UtcNow;
        if (Ess.Vanish.IsVanished(e.getPlayer().getName())) e.setQuitMessage("");
    }

    [EventHandler]
    public void onChat(PlayerChatEvent e)
    {
        var p    = e.getPlayer();
        var data = Ess.Data.Get(p.getName());
        if (Ess.Mutes.IsMuted(p.getName()))
        {
            e.setCancelled(true);
            p.sendMessage(M.YouMuted);
            return;
        }
        var nick = Ess.Nicks.Get(p.getName());
        if (nick != null) e.setFormat("<" + nick + "> %2$s");
        if (data.IsAfk)
        {
            data.IsAfk = false;
            FourKit.broadcastMessage(M.Format(M.UserIsNotAway, p.getDisplayName()));
        }
    }

    [EventHandler]
    public void onMove(PlayerMoveEvent e)
    {
        var data = Ess.Data.Get(e.getPlayer().getName());
        if (!data.IsAfk) return;
        var f = e.getFrom(); var t = e.getTo();
        if (Math.Abs(f.getX() - t.getX()) < 0.01 && Math.Abs(f.getZ() - t.getZ()) < 0.01) return;
        data.IsAfk = false;
        FourKit.broadcastMessage(M.Format(M.UserIsNotAway, e.getPlayer().getDisplayName()));
    }

    [EventHandler]
    public void onDeath(PlayerDeathEvent e)
    {
        var p = e.getEntity();
        var data = Ess.Data.Get(p.getName());
        data.LastLoc = p.getLocation();
        p.sendMessage("\u00a76Use the /back command to return to your death point.");
    }

    [EventHandler]
    public void onDamage(EntityDamageEvent e)
    {
        if (e.getEntity() is Player p && Ess.Data.Get(p.getName()).GodMode)
            e.setCancelled(true);
    }
}

public static class H
{
    public static Player? FindPlayer(CommandSender sender, string name)
    {
        var p = FourKit.getPlayer(name);
        if (p == null) sender.sendMessage(M.PlayerNotFound);
        return p;
    }

    public static Player? RequirePlayer(CommandSender sender)
    {
        if (sender is Player p) return p;
        sender.sendMessage(M.Format(M.OnlyPlayers, "this command"));
        return null;
    }

    public static void TeleportWithBack(Player p, Location to)
    {
        FourEssentials.Instance.Data.Get(p.getName()).LastLoc = p.getLocation();
        p.teleport(to);
    }

    public static GameMode? ParseMode(string s) => s.ToLower() switch
    {
        var x when x.Contains("creat") || x == "gmc" || x == "egmc" || x == "1" || x == "c" => GameMode.CREATIVE,
        var x when x.Contains("survi") || x == "gms" || x == "egms" || x == "0" || x == "s" => GameMode.SURVIVAL,
        var x when x.Contains("advent")|| x == "gma" || x == "egma" || x == "2" || x == "a" => GameMode.ADVENTURE,
        _ => null
    };

    public static string DN(Player p) =>
        FourEssentials.Instance.Nicks.Get(p.getName()) ?? p.getName();

    public static string SN(CommandSender s) =>
        s is Player sp ? DN(sp) : "Console";

    private static readonly Dictionary<string, long> NamedTimes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["day"]      = 1000, ["noon"] = 6000, ["night"] = 13000,
        ["midnight"] = 18000, ["dawn"] = 23000, ["dusk"] = 12500,
        ["sunrise"]  = 23000, ["sunset"] = 12500,
    };

    public static bool TryParseTicks(string s, out long ticks)
    {
        if (NamedTimes.TryGetValue(s, out ticks)) return true;
        return long.TryParse(s, out ticks);
    }

    public static string FormatTicks(long ticks)
    {
        foreach (var kv in NamedTimes)
            if (kv.Value == ticks) return kv.Key;
        return ticks.ToString();
    }
}


public class HealCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length > 0)
        {
            var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
            if (t.getHealth() == 0) { sender.sendMessage(M.HealDead); return true; }
            t.setHealth(t.getMaxHealth());
            t.setFoodLevel(20);
            t.sendMessage(M.Heal);
            sender.sendMessage(M.Format(M.HealOther, t.getDisplayName()));
        }
        else
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            if (p.getHealth() == 0) { p.sendMessage(M.HealDead); return true; }
            p.setHealth(p.getMaxHealth());
            p.setFoodLevel(20);
            p.sendMessage(M.Heal);
        }
        return true;
    }
}

public class FeedCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length > 0)
        {
            var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
            t.setFoodLevel(20);
            t.setSaturation(10f);
            t.setExhaustion(0f);
            t.sendMessage(M.Feed);
            sender.sendMessage(M.Format(M.FeedOther, t.getDisplayName()));
        }
        else
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            p.setFoodLevel(20);
            p.setSaturation(10f);
            p.setExhaustion(0f);
            p.sendMessage(M.Feed);
        }
        return true;
    }
}

public class FlyCommand : CommandExecutor
{
    private static void SendPlayerAbilities(Player p, bool canFly, bool flying, float flySpeed, float walkSpeed)
    {
        byte[] buf = new byte[10];
        buf[0] = 202;
        byte flags = 0;
        if (flying) flags |= 0x02;
        if (canFly) flags |= 0x04;
        buf[1] = flags;
        FourEssentials.WriteFloat(buf, 2, flySpeed);
        FourEssentials.WriteFloat(buf, 6, walkSpeed);
        p.getConnection().send(buf);
    }

    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? target = null;
        bool other = false;
        bool? force = null;

        if (args.Length == 0)
        {
            target = sender as Player;
            if (target == null) { sender.sendMessage(M.Format(M.OnlyPlayers, "/fly")); return true; }
        }
        else if (args.Length == 1)
        {
            if (args[0].Equals("on", StringComparison.OrdinalIgnoreCase)) force = true;
            else if (args[0].Equals("off", StringComparison.OrdinalIgnoreCase)) force = false;
            else
            {
                target = H.FindPlayer(sender, args[0]);
                if (target == null) return true;
                other = true;
            }

            if (target == null && force.HasValue)
            {
                target = sender as Player;
                if (target == null) { sender.sendMessage(M.Format(M.OnlyPlayers, "/fly")); return true; }
            }
        }
        else
        {
            target = H.FindPlayer(sender, args[0]); if (target == null) return true;
            other = true;
            if (args[1].Equals("on", StringComparison.OrdinalIgnoreCase)) force = true;
            else if (args[1].Equals("off", StringComparison.OrdinalIgnoreCase)) force = false;
            else { sender.sendMessage(M.NotEnoughArgs); return true; }
        }

        if (target == null) { sender.sendMessage(M.NotEnoughArgs); return true; }

        FourEssentials.Instance.flying.TryGetValue(target.getName(), out bool current);
        bool enabled = force ?? !current;

        FourEssentials.Instance.flying[target.getName()] = enabled;

        SendPlayerAbilities(target, canFly: enabled, flying: enabled, flySpeed: 0.05f, walkSpeed: 0.1f);

        target.setFallDistance(0f);

        string state = enabled ? M.Enabled : M.Disabled;
        target.sendMessage(M.Format(M.FlyMode, state, target.getDisplayName()));
        if (other || sender is not Player sp2 || sp2.getName() != target.getName())
            sender.sendMessage(M.Format(M.FlyMode, state, target.getDisplayName()));

        return true;
    }
}


public class GodCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? target = null; bool other = false; bool? force = null;
        foreach (var a in args)
        {
            if (a.Equals("on",  StringComparison.OrdinalIgnoreCase)) { force = true;  continue; }
            if (a.Equals("off", StringComparison.OrdinalIgnoreCase)) { force = false; continue; }
            var f = FourKit.getPlayer(a);
            if (f != null) { target = f; other = true; }
        }
        target ??= H.RequirePlayer(sender);
        if (target == null) return true;

        var data = FourEssentials.Instance.Data.Get(target.getName());
        data.GodMode = force ?? !data.GodMode;

        if (data.GodMode && target.getHealth() != 0)
        {
            target.setHealth(target.getMaxHealth());
            target.setFoodLevel(20);
        }

        target.sendMessage(M.Format(M.GodMode, data.GodMode ? M.Enabled : M.Disabled));
        if (other || sender is not Player sp2 || sp2.getName() != target.getName())
            sender.sendMessage(M.Format(M.GodMode,
                data.GodMode
                    ? M.Format(M.GodEnabled,  target.getDisplayName())
                    : M.Format(M.GodDisabled, target.getDisplayName())));
        return true;
    }
}

public class GamemodeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        GameMode? mode;
        string?   playerArg;

        mode = H.ParseMode(label);
        if (mode == null)
        {
            if (args.Length == 0) { sender.sendMessage(M.GameModeInv); return true; }
            mode = H.ParseMode(args[0]);
            if (mode == null) { sender.sendMessage(M.GameModeInv); return true; }
            playerArg = args.Length > 1 ? args[1] : null;
        }
        else
        {
            playerArg = args.Length > 0 ? args[0] : null;
        }

        Player? target;
        if (playerArg != null)
        {
            target = H.FindPlayer(sender, playerArg); if (target == null) return true;
        }
        else
        {
            target = H.RequirePlayer(sender); if (target == null) return true;
        }

        target.setGameMode(mode.Value);
        string modeName = mode.Value.ToString().ToLower();
        sender.sendMessage(M.Format(M.GameMode, modeName, target.getDisplayName()));
        if (sender is Player sp && sp.getName() != target.getName())
            target.sendMessage(M.Format(M.GameMode, modeName, target.getDisplayName()));
        return true;
    }
}

public class TpCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }

        if (args.Length == 1)
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
            H.TeleportWithBack(p, t.getLocation());
            p.sendMessage(M.Format("\u00a76Teleporting to \u00a7c{0}\u00a76.", t.getDisplayName()));
        }
        else if (args.Length == 2)
        {
            var mover = H.FindPlayer(sender, args[0]); if (mover == null) return true;
            var dest  = H.FindPlayer(sender, args[1]); if (dest == null) return true;
            H.TeleportWithBack(mover, dest.getLocation());
            mover.sendMessage(M.Format(M.TeleportAtoB, H.SN(sender), dest.getDisplayName()));
        }
        else if (args.Length >= 3)
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            bool hasPlayer = !double.TryParse(args[0], out _);
            Player? movee = hasPlayer ? H.FindPlayer(sender, args[0]) : p;
            if (movee == null) return true;
            int xi = hasPlayer ? 1 : 0;
            if (!double.TryParse(args[xi], out double x) ||
                !double.TryParse(args[xi+1], out double y) ||
                !double.TryParse(args[xi+2], out double z))
            { sender.sendMessage(M.NotEnoughArgs); return true; }
            if (Math.Abs(x) > 30_000_000 || Math.Abs(y) > 30_000_000 || Math.Abs(z) > 30_000_000)
            { sender.sendMessage(M.TeleportInvalid); return true; }
            var loc = new Location(movee.getLocation().getWorld(), x, y, z);
            H.TeleportWithBack(movee, loc);
            movee.sendMessage(M.Teleporting);
        }
        return true;
    }
}

public class TpHereCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        H.TeleportWithBack(t, p.getLocation());
        t.sendMessage(M.Format(M.TeleportAtoB, H.DN(p), p.getDisplayName()));
    return true;
    }
}

public class TpPosCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        if (args.Length < 3) { sender.sendMessage(M.NotEnoughArgs); return true; }
        if (!double.TryParse(args[0], out double x) ||
            !double.TryParse(args[1], out double y) ||
            !double.TryParse(args[2], out double z))
        { sender.sendMessage(M.NotEnoughArgs); return true; }
        if (Math.Abs(x) > 30_000_000 || Math.Abs(y) > 30_000_000 || Math.Abs(z) > 30_000_000)
        { sender.sendMessage(M.TeleportInvalid); return true; }
        var loc = new Location(p.getLocation().getWorld(), x, y, z);
        H.TeleportWithBack(p, loc);
        p.sendMessage(M.Teleporting);
        return true;
    }
}

public class TpAllCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? dest;
        if (args.Length > 0) { dest = H.FindPlayer(sender, args[0]); if (dest == null) return true; }
        else { dest = H.RequirePlayer(sender); if (dest == null) return true; }
        sender.sendMessage("\u00a76Teleporting all players...");
        var loc = dest.getLocation();
        foreach (var pl in FourKit.getOnlinePlayers())
            if (pl.getName() != dest.getName())
                H.TeleportWithBack(pl, loc);
        return true;
    }
}

public class SpawnCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        H.TeleportWithBack(p, new Location(FourKit.getWorld(0), 0, 64, 0));
        p.sendMessage(M.Teleporting);
        return true;
    }
}

public class BackCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        var data = FourEssentials.Instance.Data.Get(p.getName());
        if (data.LastLoc == null) { p.sendMessage(M.NoLocation); return true; }
        var dest = data.LastLoc;
        data.LastLoc = p.getLocation();
        p.teleport(dest);
        p.sendMessage(M.BackUsage);
        return true;
    }
}

public class SetHomeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        string name = (args.Length > 0 ? args[0] : "home").ToLower();
        if (name == "bed") { p.sendMessage(M.InvalidHomeName); return true; }
        FourEssentials.Instance.Homes.Set(p.getName(), name, p.getLocation());
        p.sendMessage(M.HomeSet);
        return true;
    }
}

public class HomeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        string homeName = args.Length > 0 ? args[0].ToLower() : "";
        string playerName = p.getName();
        if (homeName.Contains(':'))
        {
            var parts = homeName.Split(':', 2);
            playerName = parts[0];
            homeName   = parts[1];
        }
        if (homeName.Length == 0)
        {
            var list = FourEssentials.Instance.Homes.All(playerName);
            if (list.Count == 0) { p.sendMessage("\u00a76Player has not set a home."); return true; }
            homeName = list[0];
        }
        var loc = FourEssentials.Instance.Homes.Get(playerName, homeName);
        if (loc == null) { p.sendMessage(M.Format(M.InvalidHome, homeName)); return true; }
        H.TeleportWithBack(p, loc);
        p.sendMessage(M.Format(M.WarpingTo, homeName));
        return true;
    }
}

public class DelHomeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        string playerName;
        string homeName;
        var p = sender as Player;
        var raw = args[0];
        if (raw.Contains(':'))
        {
            var parts = raw.Split(':', 2);
            playerName = parts[0];
            homeName   = parts[1];
        }
        else
        {
            if (p == null) { sender.sendMessage(M.NotEnoughArgs); return true; }
            playerName = p.getName();
            homeName   = raw;
        }
        if (homeName.Equals("bed", StringComparison.OrdinalIgnoreCase))
        { sender.sendMessage(M.InvalidHomeName); return true; }
        if (!FourEssentials.Instance.Homes.Delete(playerName, homeName.ToLower()))
        { sender.sendMessage(M.Format(M.InvalidHome, homeName)); return true; }
        sender.sendMessage(M.Format(M.DeleteHome, homeName));
        return true;
    }
}

public class HomesCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        var list = FourEssentials.Instance.Homes.All(p.getName());
        if (list.Count == 0) { p.sendMessage("\u00a76Player has not set a home."); return true; }
        p.sendMessage(M.Format(M.Homes, string.Join(", ", list)));
        return true;
    }
}

public class SetWarpCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        if (args[0].Length == 0 || int.TryParse(args[0], out _))
        { p.sendMessage(M.InvalidWarp); return true; }
        FourEssentials.Instance.Warps.Set(args[0], p.getLocation());
        p.sendMessage(M.Format(M.WarpSet, args[0]));
        return true;
    }
}

public class WarpCommand : CommandExecutor
{
    private const int PerPage = 20;

    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var allWarps = FourEssentials.Instance.Warps.All();
        int pageNum = 1;

        if (args.Length == 0 || int.TryParse(args[0], out pageNum))
        {
            if (allWarps.Count == 0)
            {
                sender.sendMessage(M.NoWarpsDef);
                return true;
            }

            if (pageNum <= 0) pageNum = 1;

            int maxPage = (int)Math.Ceiling(allWarps.Count / (double)PerPage);
            if (pageNum > maxPage) pageNum = maxPage;

            var slice = allWarps
                .Skip((pageNum - 1) * PerPage)
                .Take(PerPage)
                .ToList();

            if (allWarps.Count > PerPage)
            {
                sender.sendMessage(M.Format(M.WarpsCount, allWarps.Count, pageNum, maxPage));
                sender.sendMessage(string.Join(", ", slice));
            }
            else
            {
                sender.sendMessage(M.Format(M.Warps, string.Join(", ", slice)));
            }

            return true;
        }

        var p = H.RequirePlayer(sender);
        if (p == null) return true;

        var loc = FourEssentials.Instance.Warps.Get(args[0]);
        if (loc == null)
        {
            p.sendMessage(M.WarpNotExist);
            return true;
        }

        H.TeleportWithBack(p, loc);
        p.sendMessage(M.Format(M.WarpingTo, args[0]));
        return true;
    }
}


public class DelWarpCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        if (!FourEssentials.Instance.Warps.Delete(args[0])) { sender.sendMessage(M.WarpNotExist); return true; }
        sender.sendMessage(M.Format(M.DeleteWarp, args[0]));
        return true;
    }
}

public class WarpsCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var all = FourEssentials.Instance.Warps.All();
        if (all.Count == 0) { sender.sendMessage(M.NoWarpsDef); return true; }
        sender.sendMessage(M.Format(M.Warps, string.Join(", ", all)));
        return true;
    }
}

public class NickCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        Player? target; string nickArg;
        if (args.Length >= 2 && FourKit.getPlayer(args[0]) != null)
        { target = FourKit.getPlayer(args[0])!; nickArg = string.Join(" ", args.Skip(1)); }
        else
        { target = H.RequirePlayer(sender); if (target == null) return true; nickArg = string.Join(" ", args); }

        if (nickArg.Equals("off", StringComparison.OrdinalIgnoreCase) ||
            nickArg.Equals(target.getName(), StringComparison.OrdinalIgnoreCase))
        {
            FourEssentials.Instance.Nicks.Remove(target.getName());
            target.setDisplayName(null);
            target.sendMessage(M.NickNoMore);
        }
        else
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(nickArg, @"^[a-zA-Z_0-9\u00a7]+$"))
            { sender.sendMessage(M.NickAlpha); return true; }
            if (nickArg.Length > 30) { sender.sendMessage(M.NickTooLong); return true; }
            string stripped = System.Text.RegularExpressions.Regex.Replace(nickArg, "\u00a7.", "").ToLower();
            foreach (var online in FourKit.getOnlinePlayers())
            {
                if (online.getName() == target.getName()) continue;
                string onlineStripped = System.Text.RegularExpressions.Regex.Replace(online.getDisplayName(), "\u00a7.", "").ToLower();
                if (onlineStripped == stripped || online.getName().ToLower() == stripped)
                { sender.sendMessage(M.NickInUse); return true; }
            }
            FourEssentials.Instance.Nicks.Set(target.getName(), nickArg);
            target.setDisplayName(nickArg);
            target.sendMessage(M.Format(M.NickSet, nickArg));
        }
        if (sender is Player sp && sp.getName() != target.getName())
            sender.sendMessage(M.NickChanged);
        return true;
    }
}

public class MsgCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length < 2) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        string msg  = string.Join(" ", args.Skip(1));
        string from = H.SN(sender);
        string to   = H.DN(t);
        if (FourEssentials.Instance.Data.Get(t.getName()).IsAfk)
            sender.sendMessage(M.Format(M.UserAFK, t.getDisplayName()));
        string line = M.Format(M.MsgFormat, from, to, msg);
        sender.sendMessage(line);
        t.sendMessage(M.Format(M.MsgFormat, from, to, msg));
        FourEssentials.Instance.Data.Get(t.getName()).ReplyTo = from;
        if (sender is Player sp) FourEssentials.Instance.Data.Get(sp.getName()).ReplyTo = t.getName();
        return true;
    }
}

public class ReplyCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        if (args.Length == 0) { p.sendMessage(M.NotEnoughArgs); return true; }
        var data = FourEssentials.Instance.Data.Get(p.getName());
        if (data.ReplyTo == null) { p.sendMessage(M.ForeverAlone); return true; }
        var t = FourKit.getPlayer(data.ReplyTo);
        if (t == null) { p.sendMessage(M.ForeverAlone); return true; }
        string msg  = string.Join(" ", args);
        string line1 = M.Format(M.MsgFormat, "me", t.getDisplayName(), msg);
        string line2 = M.Format(M.MsgFormat, p.getDisplayName(), "me", msg);
        p.sendMessage(line1);
        t.sendMessage(line2);
        data.ReplyTo = t.getName();
        FourEssentials.Instance.Data.Get(t.getName()).ReplyTo = p.getName();
        return true;
    }
}

public class MeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        string message = string.Join(" ", args);
        FourKit.broadcastMessage(M.Format(M.Action, H.SN(sender), message));
        return true;
    }
}

public class BroadcastCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        FourKit.broadcastMessage(M.Format(M.Broadcast, string.Join(" ", args)));
        return true;
    }
}

public class AfkCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? target;
        if (args.Length > 0 && FourKit.getPlayer(args[0]) != null)
            target = FourKit.getPlayer(args[0])!;
        else
        { target = H.RequirePlayer(sender); if (target == null) return true; }

        var data = FourEssentials.Instance.Data.Get(target.getName());
        data.IsAfk = !data.IsAfk;
        string msg = data.IsAfk
            ? M.Format(M.UserIsAway,    target.getDisplayName())
            : M.Format(M.UserIsNotAway, target.getDisplayName());
        if (!FourEssentials.Instance.Vanish.IsVanished(target.getName()))
            FourKit.broadcastMessage(msg);
        return true;
    }
}

public class ListCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var ess     = FourEssentials.Instance;
        var online  = FourKit.getOnlinePlayers();
        var visible = online.Where(p => !ess.Vanish.IsVanished(p.getName())).ToList();
        sender.sendMessage(M.Format(M.ListAmount, visible.Count, "?"));
        var names = visible.Select(p =>
            p.getDisplayName() + (ess.Data.Get(p.getName()).IsAfk ? M.ListAfkTag : ""));
        sender.sendMessage(M.ConnPlayers + ": " + string.Join(", ", names));
        return true;
    }
}

public class KickCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        string reason = args.Length > 1 ? string.Join(" ", args.Skip(1)) : M.KickDefault;
        t.kickPlayer();
        string sn = H.SN(sender);
        sender.sendMessage(M.Format(M.PlayerKicked, sn, t.getName(), reason));
        FourKit.broadcastMessage(M.Format(M.PlayerKicked, sn, t.getName(), reason));
        return true;
    }
}

public class KickAllCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        string reason = args.Length > 0 ? string.Join(" ", args) : M.KickDefault;
        string sn = H.SN(sender);
        foreach (var p in FourKit.getOnlinePlayers())
            if (!(sender is Player sp) || p.getName() != sp.getName())
                p.kickPlayer();
        sender.sendMessage(M.KickedAll);
        return true;
    }
}

public class BanCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        string reason     = args.Length > 1 ? string.Join(" ", args.Skip(1)) : M.DefaultBan;
        string banDisplay = M.Format(M.BanFormat, reason, H.SN(sender));
        t.banPlayer(reason);
        t.kickPlayer();
        string sn = H.SN(sender);
        sender.sendMessage(M.Format(M.PlayerBanned, sn, t.getName(), banDisplay));
        FourKit.broadcastMessage(M.Format(M.PlayerBanned, sn, t.getName(), reason));
        return true;
    }
}

public class MuteCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        bool wasMuted = FourEssentials.Instance.Mutes.IsMuted(t.getName());
        if (wasMuted)
        {
            FourEssentials.Instance.Mutes.Unmute(t.getName());
            sender.sendMessage(M.Format(M.UnmutedPlayer, t.getDisplayName()));
            t.sendMessage(M.YouUnmuted);
        }
        else
        {
            FourEssentials.Instance.Mutes.Mute(t.getName());
            sender.sendMessage(M.Format(M.MutedPlayer, t.getDisplayName()));
            t.sendMessage(M.YouMuted);
            FourKit.broadcastMessage(M.Format(M.MuteNotify, H.SN(sender), t.getName(), ""));
        }
        return true;
    }
}

public class UnmuteCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        FourEssentials.Instance.Mutes.Unmute(t.getName());
        sender.sendMessage(M.Format(M.UnmutedPlayer, t.getDisplayName()));
        t.sendMessage(M.YouUnmuted);
        return true;
    }
}

public class KillCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        t.damage(Short.MaxValue);
        if (t.getHealth() > 0) t.setHealth(0);
        sender.sendMessage(M.Format(M.Kill, t.getDisplayName()));
        return true;
    }
}

public class GetPosCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? p;
        Location loc;
        if (args.Length > 0)
        {
            p = H.FindPlayer(sender, args[0]); if (p == null) return true;
            loc = p.getLocation();
            outputPosition(sender, loc, sender is Player sp ? sp.getLocation() : null);
        }
        else
        {
            p = H.RequirePlayer(sender); if (p == null) return true;
            outputPosition(sender, p.getLocation(), null);
        }
        return true;
    }

    private void outputPosition(CommandSender sender, Location loc, Location? from)
    {
        sender.sendMessage(M.Format(M.CurrentWorld, loc.getWorld()?.getName() ?? "?"));
        sender.sendMessage(M.Format(M.PosX, loc.getBlockX()));
        sender.sendMessage(M.Format(M.PosY, loc.getBlockY()));
        sender.sendMessage(M.Format(M.PosZ, loc.getBlockZ()));
        sender.sendMessage(M.Format(M.PosYaw, ((loc.getYaw() + 180 + 360) % 360).ToString("0.##")));
        sender.sendMessage(M.Format(M.PosPitch, loc.getPitch().ToString("0.##")));
        if (from != null && loc.getWorld() == from.getWorld())
        {
            double dx = loc.getX() - from.getX(), dy = loc.getY() - from.getY(), dz = loc.getZ() - from.getZ();
            sender.sendMessage(M.Format(M.Distance, Math.Sqrt(dx*dx + dy*dy + dz*dz).ToString("0.##")));
        }
    }
}

public class SpeedCommand : CommandExecutor
{
    private static void SendPlayerAbilities(Player p, bool canFly, bool flying, float flySpeed, float walkSpeed)
    {
        byte[] buf = new byte[10];
        buf[0] = 202;
        byte flags = 0;
        if (flying) flags |= 0x02;
        if (canFly) flags |= 0x04;
        buf[1] = flags;
        FourEssentials.WriteFloat(buf, 2, flySpeed);
        FourEssentials.WriteFloat(buf, 6, walkSpeed);
        p.getConnection().send(buf);
    }

    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }

        var p = H.RequirePlayer(sender); if (p == null) return true;
        bool isFly; float speed;
        Player? target = p;

        if (args.Length == 1)
        {
            FourEssentials.Instance.flying.TryGetValue(p.getName(), out bool storedFlying);
            isFly = storedFlying;
            speed = ParseSpeed(args[0]);
            if (float.IsNaN(speed)) { sender.sendMessage(M.NotEnoughArgs); return true; }
        }
        else
        {
            isFly = args[0].ToLower().Contains("fly") || args[0].ToLower() == "f";
            speed = ParseSpeed(args[1]);
            if (float.IsNaN(speed)) { sender.sendMessage(M.NotEnoughArgs); return true; }
            if (args.Length > 2)
            {
                target = H.FindPlayer(sender, args[2]); if (target == null) return true;
            }
        }

        float realSpeed = GetRealSpeed(speed, isFly);

        FourEssentials.Instance.flying.TryGetValue(target.getName(), out bool stored);
        SendPlayerAbilities(target, canFly: stored, flying: stored, flySpeed: isFly ? realSpeed : 0.05f, walkSpeed: isFly ? 0.1f : realSpeed);

        sender.sendMessage(M.Format(M.MoveSpeed, isFly ? M.Flying : M.Walking, speed.ToString("0.##"), target.getDisplayName()));
        return true;
    }

    private float ParseSpeed(string s)
    {
        if (!float.TryParse(s, out float v)) return float.NaN;
        return Math.Clamp(v, 0.0001f, 10f);
    }

    private float GetRealSpeed(float userSpeed, bool isFly)
    {
        float defaultSpeed = isFly ? 0.1f : 0.2f;
        float maxSpeed     = 1f;
        if (userSpeed < 1f) return defaultSpeed * userSpeed;
        return ((userSpeed - 1) / 9) * (maxSpeed - defaultSpeed) + defaultSpeed;
    }
}

public class RepairCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;

        if (args.Length == 0 || args[0].Equals("hand", StringComparison.OrdinalIgnoreCase))
        {
            repairHand(p);
        }
        else if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            repairAll(p);
        }
        else
        {
            sender.sendMessage(M.NotEnoughArgs);
        }
        return true;
    }

    private void repairHand(Player p)
    {
        var item = p.getItemInHand();
        if (item == null) { p.sendMessage(M.RepairInvalid); return; }
        if (item.getDurability() == 0) { p.sendMessage(M.RepairFixed); return; }
        item.setDurability(0);
        p.sendMessage(M.Format(M.Repair, item.getType().ToString().ToLower().Replace("_", " ")));
    }

    private void repairAll(Player p)
    {
        var inv      = p.getInventory();
        var contents = inv.getContents();
        bool any     = false;
        var repaired = new List<string>();
        foreach (var item in contents)
        {
            if (item == null || item.getDurability() == 0) continue;
            item.setDurability(0);
            repaired.Add(item.getType().ToString().ToLower().Replace("_", " "));
            any = true;
        }
        if (!any) { p.sendMessage(M.RepairNone); return; }
        p.sendMessage(M.Format(M.Repair, string.Join(", ", repaired)));
    }
}

public class MoreCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        var item = p.getItemInHand();
        if (item == null) { p.sendMessage(M.Format(M.CantSpawn, "Air")); return true; }
int max = 64;

if (item.getAmount() >= max)
{
    p.sendMessage(M.FullStack);
    return true;
}

item.setAmount(max);

        return true;
    }
}

public class SuicideCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        p.damage(Short.MaxValue);
        if (p.getHealth() > 0) p.setHealth(0);
        p.sendMessage(M.SuicideMsg);
        FourKit.broadcastMessage(M.Format(M.SuicideSucc, p.getDisplayName()));
        return true;
    }
}

public class PingCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0)
            sender.sendMessage(M.Pong);
        else
            sender.sendMessage(string.Join(" ", args));
        return true;
    }
}

public class GcCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        long max   = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024;
        long total = Environment.WorkingSet / 1024 / 1024;
        long free  = max - total;
        long ticks = FourKit.getServerTick();

        sender.sendMessage(M.Format(M.Uptime, Environment.TickCount64 / 1000 + "s"));
        sender.sendMessage(M.Format(M.TPS,    ticks.ToString()));
        sender.sendMessage(M.Format(M.GcMax,  max));
        sender.sendMessage(M.Format(M.GcTotal,total));
        sender.sendMessage(M.Format(M.GcFree, free));
        return true;
    }
}

public class ExpCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0)
        {
            var self = H.RequirePlayer(sender); if (self == null) return true;
            showExp(sender, self);
            return true;
        }
        switch (args[0].ToLower())
        {
            case "give" when args.Length >= 3:
            {
                var t = H.FindPlayer(sender, args[1]); if (t == null) return true;
                if (!long.TryParse(args[2], out long amt)) { sender.sendMessage(M.NotEnoughArgs); return true; }
                t.giveExp((int)amt);
                sender.sendMessage(M.Format(M.ExpSet, t.getDisplayName(), amt));
                break;
            }
            case "set" when args.Length >= 3:
            {
                var t = H.FindPlayer(sender, args[1]); if (t == null) return true;
                if (!long.TryParse(args[2], out long amt)) { sender.sendMessage(M.NotEnoughArgs); return true; }
                t.setLevel(0); t.setExp(0); t.giveExp((int)amt);
                sender.sendMessage(M.Format(M.ExpSet, t.getDisplayName(), amt));
                break;
            }
            case "show" when args.Length >= 2:
            {
                var t = H.FindPlayer(sender, args[1]); if (t == null) return true;
                showExp(sender, t);
                break;
            }
            default:
            {
                if (long.TryParse(args[0], out long quickAmt))
                {
                    var t = args.Length >= 2 ? H.FindPlayer(sender, args[1]) : H.RequirePlayer(sender);
                    if (t == null) return true;
                    t.giveExp((int)quickAmt);
                    sender.sendMessage(M.Format(M.ExpSet, t.getDisplayName(), quickAmt));
                }
                else if (args.Length >= 1)
                {
                    var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
                    showExp(sender, t);
                }
                break;
            }
        }
        return true;
    }

    private void showExp(CommandSender sender, Player p)
    {
        int total   = (int)(p.getExp() * 100);
        int toNext  = 100 - total;
        sender.sendMessage(M.Format(M.Exp, p.getDisplayName(), total, p.getLevel(), toNext));
    }
}

public class SeenCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var online = FourKit.getPlayer(args[0]);
        if (online != null)
        {
            var data = FourEssentials.Instance.Data.Get(online.getName());
            sender.sendMessage(M.Format(M.SeenOnline, online.getDisplayName(),
                FormatTimeAgo(data.FirstSeen)));
            return true;
        }
        var offData = FourEssentials.Instance.Data.Get(args[0]);
        if (!offData.SeenSet) { sender.sendMessage(M.Format(M.UserUnknown, args[0])); return true; }
        sender.sendMessage(M.Format(M.SeenOffline, args[0], FormatTimeAgo(offData.LastSeen)));
        return true;
    }

    private string FormatTimeAgo(DateTime dt)
    {
        var diff = DateTime.UtcNow - dt;
        if (diff.TotalSeconds < 60)   return (int)diff.TotalSeconds + " seconds ago";
        if (diff.TotalMinutes < 60)   return (int)diff.TotalMinutes + " minutes ago";
        if (diff.TotalHours < 24)     return (int)diff.TotalHours   + " hours ago";
        return (int)diff.TotalDays + " days ago";
    }
}

public class WhoisCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var p = H.FindPlayer(sender, args[0]); if (p == null) return true;
        var data = FourEssentials.Instance.Data.Get(p.getName());
        var loc  = p.getLocation();
        sender.sendMessage(M.Format(M.WhoisTop, p.getName()));
        sender.sendMessage(M.Format(M.WhoisNick,    p.getDisplayName()));
        sender.sendMessage(M.Format(M.WhoisHealth,  p.getHealth().ToString("0.#")));
        sender.sendMessage(M.Format(M.WhoisHunger,  p.getFoodLevel(), p.getSaturation().ToString("0.#")));
        sender.sendMessage(M.Format(M.WhoisExp,     (int)(p.getExp() * 100), p.getLevel()));
        sender.sendMessage(M.Format(M.WhoisLocation,
            loc.getWorld()?.getName() ?? "?",
            loc.getBlockX(), loc.getBlockY(), loc.getBlockZ()));
        var addr = p.getAddress()?.getAddress()?.getHostAddress() ?? "unknown";
        sender.sendMessage(M.Format(M.WhoisIP,      addr));
        sender.sendMessage(M.Format(M.WhoisGamemode,p.getGameMode().ToString().ToLower()));
        sender.sendMessage(M.Format(M.WhoisGod,     data.GodMode ? M.TrueStr : M.FalseStr));
        sender.sendMessage(M.Format(M.WhoisOp, M.FalseStr));

        bool allowFlight = p.getAllowFlight();
        FourEssentials.Instance.flying.TryGetValue(p.getName(), out bool storedFlying);
        sender.sendMessage(M.Format(M.WhoisFly,
            allowFlight ? M.TrueStr : M.FalseStr,
            storedFlying ? M.Flying : M.NotFlying));

        sender.sendMessage(M.Format(M.WhoisAFK,    data.IsAfk  ? M.TrueStr : M.FalseStr));
        sender.sendMessage(M.Format(M.WhoisJail,   M.FalseStr));
        sender.sendMessage(M.Format(M.WhoisMuted,
            FourEssentials.Instance.Mutes.IsMuted(p.getName()) ? M.TrueStr : M.FalseStr));
        return true;
    }
}


public class RealNameCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        string search = args[0].ToLower();
        bool found = false;
        foreach (var online in FourKit.getOnlinePlayers())
        {
            if (FourEssentials.Instance.Vanish.IsVanished(online.getName())) continue;
            string dn = System.Text.RegularExpressions.Regex.Replace(
                online.getDisplayName(), "\u00a7.", "").ToLower();
            if (dn.Contains(search))
            {
                sender.sendMessage(online.getDisplayName() + " " + "is" + " " + online.getName());
                found = true;
            }
        }
        if (!found) sender.sendMessage(M.PlayerNotFound);
        return true;
    }
}

public class VanishCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? target = null; bool other = false; bool? force = null;
        foreach (var a in args)
        {
            if (a.Equals("on",  StringComparison.OrdinalIgnoreCase)) { force = true;  continue; }
            if (a.Equals("off", StringComparison.OrdinalIgnoreCase)) { force = false; continue; }
            var f = FourKit.getPlayer(a);
            if (f != null) { target = f; other = true; }
        }
        target ??= H.RequirePlayer(sender);
        if (target == null) return true;

        var store    = FourEssentials.Instance.Vanish;
        bool enabled = force ?? !store.IsVanished(target.getName());
        store.Set(target.getName(), enabled);

        string stateStr = enabled ? M.Enabled : M.Disabled;
        target.sendMessage(M.Format(M.VanishMsg, target.getDisplayName(), stateStr));
        if (enabled) target.sendMessage(M.Vanished);
        if (other || (sender is Player sp && sp.getName() != target.getName()))
            sender.sendMessage(M.Format(M.VanishMsg, target.getDisplayName(), stateStr));
        return true;
    }
}

public class PTimeCommand : CommandExecutor
{
    private static readonly HashSet<string> GetAliases = new(StringComparer.OrdinalIgnoreCase)
        { "get", "list", "show", "display" };

    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0)
        {
            var self = H.RequirePlayer(sender); if (self == null) return true;
            showTime(sender, new[] { self });
            return true;
        }

        IEnumerable<Player> targets;
        if (args.Length >= 2)
        {
            if (args[1].Equals("*", StringComparison.OrdinalIgnoreCase) ||
                args[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                targets = FourKit.getOnlinePlayers();
            else
            {
                var t = H.FindPlayer(sender, args[1]); if (t == null) return true;
                targets = new[] { t };
            }
        }
        else
        {
            var self = H.RequirePlayer(sender); if (self == null) return true;
            targets = new[] { self };
        }

        string timeArg = args[0];

        if (GetAliases.Contains(timeArg))
        {
            showTime(sender, targets);
            return true;
        }

        if (timeArg.Equals("reset", StringComparison.OrdinalIgnoreCase))
        {
            var names = string.Join(", ", targets.Select(t => t.getName()));
            foreach (var t in targets)
                t.getLocation().getWorld()?.setTime(FourKit.getWorld(0).getTime());
            sender.sendMessage(M.Format(M.PTimeReset, names));
            return true;
        }

        bool relative = true;
        if (timeArg.StartsWith("@")) { relative = false; timeArg = timeArg.Substring(1); }

        if (!H.TryParseTicks(timeArg, out long ticks))
        { sender.sendMessage(M.NotEnoughArgs); return true; }

        var playerNames = new List<string>();
        foreach (var t in targets)
        {
            var world = t.getLocation().getWorld();
            if (world != null)
            {
                long worldTime = world.getTime();
                long newTime   = (worldTime - worldTime % 24000) + 24000 + ticks;
                if (relative) newTime -= worldTime;
                world.setTime(newTime % 24000);
            }
            playerNames.Add(t.getName());
        }

        string nameStr  = string.Join(", ", playerNames);
        string timeStr  = H.FormatTicks(ticks);
        if (!relative)
            sender.sendMessage(M.Format(M.PTimeSetFixed, timeStr, nameStr));
        else
            sender.sendMessage(M.Format(M.PTimeSet, timeStr, nameStr));
        return true;
    }

    private void showTime(CommandSender sender, IEnumerable<Player> targets)
    {
        var list = targets.ToList();
        if (list.Count > 1) sender.sendMessage(M.PTimePlayers);
        foreach (var t in list)
        {
            long worldTime = t.getLocation().getWorld()?.getTime() ?? 0;
            sender.sendMessage(M.Format(M.PTimeNormal, t.getName()));
        }
    }
}


public static class Short
{
    public const int MaxValue = 32767;
}