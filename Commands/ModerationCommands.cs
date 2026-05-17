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
using System.Linq;

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