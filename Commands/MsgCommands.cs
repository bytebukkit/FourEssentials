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
        string msg   = string.Join(" ", args);
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
        FourKit.broadcastMessage(M.Format(M.Action, H.SN(sender), string.Join(" ", args)));
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