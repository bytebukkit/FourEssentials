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