// FourEssentials
// Copyright (C) 2026 Byte_HD
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerDataStore
{
    private readonly Dictionary<string, PlayerData> _d = new(StringComparer.OrdinalIgnoreCase);

    public PlayerData Get(string n)
    {
        if (!_d.TryGetValue(n, out var d)) { d = new(); _d[n] = d; }
        return d;
    }
}

public class PlayerData
{
    public bool      GodMode   { get; set; }
    public bool      IsAfk     { get; set; }
    public Location? LastLoc   { get; set; }
    public string?   ReplyTo   { get; set; }
    public DateTime  LastSeen  { get; set; } = DateTime.UtcNow;
    public DateTime  FirstSeen { get; set; } = DateTime.UtcNow;
    public bool      SeenSet   { get; set; }
}

public class WarpStore
{
    private readonly Dictionary<string, Location> _w = new(StringComparer.OrdinalIgnoreCase);

    public void      Set(string n, Location l) => _w[n] = l;
    public bool      Delete(string n)           => _w.Remove(n);
    public Location? Get(string n)              => _w.TryGetValue(n, out var l) ? l : null;
    public IReadOnlyList<string> All()          => _w.Keys.OrderBy(k => k).ToList();
}

public class HomeStore
{
    private readonly Dictionary<string, Dictionary<string, Location>> _h = new(StringComparer.OrdinalIgnoreCase);

    private Dictionary<string, Location> Of(string p)
    {
        if (!_h.TryGetValue(p, out var d)) { d = new(StringComparer.OrdinalIgnoreCase); _h[p] = d; }
        return d;
    }

    public void      Set(string p, string n, Location l) => Of(p)[n] = l;
    public bool      Delete(string p, string n)           => Of(p).Remove(n);
    public Location? Get(string p, string n)              => Of(p).TryGetValue(n, out var l) ? l : null;
    public IReadOnlyList<string> All(string p)            => Of(p).Keys.OrderBy(k => k).ToList();
    public int       Count(string p)                      => Of(p).Count;
}

public class NickStore
{
    private readonly Dictionary<string, string> _n = new(StringComparer.OrdinalIgnoreCase);

    public void    Set(string p, string nick) => _n[p] = nick;
    public void    Remove(string p)           => _n.Remove(p);
    public string? Get(string p)              => _n.TryGetValue(p, out var n) ? n : null;

    public IEnumerable<(string real, string display)> FindByNick(string search)
        => _n.Where(kv => kv.Value.Replace("\u00a7", "§").ToLower().Contains(search.ToLower()))
             .Select(kv => (kv.Key, kv.Value));
}

public class MuteStore
{
    private readonly HashSet<string> _m = new(StringComparer.OrdinalIgnoreCase);

    public void Mute(string p)    => _m.Add(p);
    public void Unmute(string p)  => _m.Remove(p);
    public bool IsMuted(string p) => _m.Contains(p);
}

public class VanishStore
{
    private readonly HashSet<string> _v = new(StringComparer.OrdinalIgnoreCase);

    public void Set(string p, bool on) { if (on) _v.Add(p); else _v.Remove(p); }
    public bool IsVanished(string p)   => _v.Contains(p);
}