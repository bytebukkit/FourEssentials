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
using System.Collections.Generic;

public class GetPosCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length > 0)
        {
            var p = H.FindPlayer(sender, args[0]); if (p == null) return true;
            outputPosition(sender, p.getLocation(), sender is Player sp ? sp.getLocation() : null);
        }
        else
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            outputPosition(sender, p.getLocation(), null);
        }
        return true;
    }

    private void outputPosition(CommandSender sender, Location loc, Location? from)
    {
        sender.sendMessage(M.Format(M.CurrentWorld, loc.getWorld()?.getName() ?? "?"));
        sender.sendMessage(M.Format(M.PosX,   loc.getBlockX()));
        sender.sendMessage(M.Format(M.PosY,   loc.getBlockY()));
        sender.sendMessage(M.Format(M.PosZ,   loc.getBlockZ()));
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
            repairHand(p);
        else if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
            repairAll(p);
        else
            sender.sendMessage(M.NotEnoughArgs);
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
        if (item.getAmount() >= max) { p.sendMessage(M.FullStack); return true; }
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
        int total  = (int)(p.getExp() * 100);
        int toNext = 100 - total;
        sender.sendMessage(M.Format(M.Exp, p.getDisplayName(), total, p.getLevel(), toNext));
    }
}