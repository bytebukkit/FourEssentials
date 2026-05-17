using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System;
using System.Collections.Generic;

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