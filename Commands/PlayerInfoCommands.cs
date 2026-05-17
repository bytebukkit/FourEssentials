// FourEssentials
// Copyright (C) 2026 Byte_HD
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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

public class SeenCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var online = FourKit.getPlayer(args[0]);
        if (online != null)
        {
            var data = FourEssentials.Instance.Data.Get(online.getName());
            sender.sendMessage(M.Format(M.SeenOnline, online.getDisplayName(), FormatTimeAgo(data.FirstSeen)));
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
        if (diff.TotalSeconds < 60) return (int)diff.TotalSeconds + " seconds ago";
        if (diff.TotalMinutes < 60) return (int)diff.TotalMinutes + " minutes ago";
        if (diff.TotalHours   < 24) return (int)diff.TotalHours   + " hours ago";
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
        sender.sendMessage(M.Format(M.WhoisTop,      p.getName()));
        sender.sendMessage(M.Format(M.WhoisNick,     p.getDisplayName()));
        sender.sendMessage(M.Format(M.WhoisHealth,   p.getHealth().ToString("0.#")));
        sender.sendMessage(M.Format(M.WhoisHunger,   p.getFoodLevel(), p.getSaturation().ToString("0.#")));
        sender.sendMessage(M.Format(M.WhoisExp,      (int)(p.getExp() * 100), p.getLevel()));
        sender.sendMessage(M.Format(M.WhoisLocation,
            loc.getWorld()?.getName() ?? "?",
            loc.getBlockX(), loc.getBlockY(), loc.getBlockZ()));
        var addr = p.getAddress()?.getAddress()?.getHostAddress() ?? "unknown";
        sender.sendMessage(M.Format(M.WhoisIP,       addr));
        sender.sendMessage(M.Format(M.WhoisGamemode, p.getGameMode().ToString().ToLower()));
        sender.sendMessage(M.Format(M.WhoisGod,      data.GodMode ? M.TrueStr : M.FalseStr));
        sender.sendMessage(M.Format(M.WhoisOp,       M.FalseStr));

        bool allowFlight = p.getAllowFlight();
        FourEssentials.Instance.flying.TryGetValue(p.getName(), out bool storedFlying);
        sender.sendMessage(M.Format(M.WhoisFly,
            allowFlight ? M.TrueStr : M.FalseStr,
            storedFlying ? M.Flying : M.NotFlying));

        sender.sendMessage(M.Format(M.WhoisAFK,   data.IsAfk ? M.TrueStr : M.FalseStr));
        sender.sendMessage(M.Format(M.WhoisJail,  M.FalseStr));
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
            string dn = Regex.Replace(online.getDisplayName(), "\u00a7.", "").ToLower();
            if (dn.Contains(search))
            {
                sender.sendMessage(online.getDisplayName() + " is " + online.getName());
                found = true;
            }
        }
        if (!found) sender.sendMessage(M.PlayerNotFound);
        return true;
    }
}