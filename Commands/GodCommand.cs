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