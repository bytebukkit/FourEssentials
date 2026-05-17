using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Plugin;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

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