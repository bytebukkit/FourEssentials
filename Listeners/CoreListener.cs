// FourEssentials
// Copyright (C) 2026 Byte_HD
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Entity;
using Minecraft.Server.FourKit.Event;
using Minecraft.Server.FourKit.Event.Entity;
using Minecraft.Server.FourKit.Event.Player;
using System;

public class CoreListener : Listener
{
    private static FourEssentials Ess => FourEssentials.Instance;

    [EventHandler]
    public void onJoin(PlayerJoinEvent e)
    {
        var p    = e.getPlayer();
        var data = Ess.Data.Get(p.getName());
        if (!data.SeenSet) { data.FirstSeen = DateTime.UtcNow; data.SeenSet = true; }
        data.LastSeen = DateTime.UtcNow;
        data.IsAfk    = false;
        var nick = Ess.Nicks.Get(p.getName());
        if (nick != null) p.setDisplayName(nick);
        if (Ess.Vanish.IsVanished(p.getName())) e.setJoinMessage("");
    }

    [EventHandler]
    public void onQuit(PlayerQuitEvent e)
    {
        Ess.Data.Get(e.getPlayer().getName()).LastSeen = DateTime.UtcNow;
        if (Ess.Vanish.IsVanished(e.getPlayer().getName())) e.setQuitMessage("");
    }

    [EventHandler]
    public void onChat(PlayerChatEvent e)
    {
        var p    = e.getPlayer();
        var data = Ess.Data.Get(p.getName());
        if (Ess.Mutes.IsMuted(p.getName()))
        {
            e.setCancelled(true);
            p.sendMessage(M.YouMuted);
            return;
        }
        var nick = Ess.Nicks.Get(p.getName());
        if (nick != null) e.setFormat("<" + nick + "> %2$s");
        if (data.IsAfk)
        {
            data.IsAfk = false;
            FourKit.broadcastMessage(M.Format(M.UserIsNotAway, p.getDisplayName()));
        }
    }

    [EventHandler]
    public void onMove(PlayerMoveEvent e)
    {
        var data = Ess.Data.Get(e.getPlayer().getName());
        if (!data.IsAfk) return;
        var f = e.getFrom(); var t = e.getTo();
        if (Math.Abs(f.getX() - t.getX()) < 0.01 && Math.Abs(f.getZ() - t.getZ()) < 0.01) return;
        data.IsAfk = false;
        FourKit.broadcastMessage(M.Format(M.UserIsNotAway, e.getPlayer().getDisplayName()));
    }

    [EventHandler]
    public void onDeath(PlayerDeathEvent e)
    {
        var p    = e.getEntity();
        var data = Ess.Data.Get(p.getName());
        data.LastLoc = p.getLocation();
        p.sendMessage("\u00a76Use the /back command to return to your death point.");
    }

    [EventHandler]
    public void onDamage(EntityDamageEvent e)
    {
        if (e.getEntity() is Player p && Ess.Data.Get(p.getName()).GodMode)
            e.setCancelled(true);
    }
}