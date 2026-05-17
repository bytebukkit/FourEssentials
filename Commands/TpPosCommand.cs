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