// FourEssentials
// Copyright (C) 2026 Byte_HD
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
using Minecraft.Server.FourKit.Command;

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