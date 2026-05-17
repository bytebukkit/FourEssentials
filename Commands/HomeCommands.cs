using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System;

public class SetHomeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        string name = (args.Length > 0 ? args[0] : "home").ToLower();
        if (name == "bed") { p.sendMessage(M.InvalidHomeName); return true; }
        FourEssentials.Instance.Homes.Set(p.getName(), name, p.getLocation());
        p.sendMessage(M.HomeSet);
        return true;
    }
}

public class HomeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        string homeName   = args.Length > 0 ? args[0].ToLower() : "";
        string playerName = p.getName();
        if (homeName.Contains(':'))
        {
            var parts = homeName.Split(':', 2);
            playerName = parts[0];
            homeName   = parts[1];
        }
        if (homeName.Length == 0)
        {
            var list = FourEssentials.Instance.Homes.All(playerName);
            if (list.Count == 0) { p.sendMessage("\u00a76Player has not set a home."); return true; }
            homeName = list[0];
        }
        var loc = FourEssentials.Instance.Homes.Get(playerName, homeName);
        if (loc == null) { p.sendMessage(M.Format(M.InvalidHome, homeName)); return true; }
        H.TeleportWithBack(p, loc);
        p.sendMessage(M.Format(M.WarpingTo, homeName));
        return true;
    }
}

public class DelHomeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        string playerName;
        string homeName;
        var p   = sender as Player;
        var raw = args[0];
        if (raw.Contains(':'))
        {
            var parts = raw.Split(':', 2);
            playerName = parts[0];
            homeName   = parts[1];
        }
        else
        {
            if (p == null) { sender.sendMessage(M.NotEnoughArgs); return true; }
            playerName = p.getName();
            homeName   = raw;
        }
        if (homeName.Equals("bed", StringComparison.OrdinalIgnoreCase))
        { sender.sendMessage(M.InvalidHomeName); return true; }
        if (!FourEssentials.Instance.Homes.Delete(playerName, homeName.ToLower()))
        { sender.sendMessage(M.Format(M.InvalidHome, homeName)); return true; }
        sender.sendMessage(M.Format(M.DeleteHome, homeName));
        return true;
    }
}

public class HomesCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        var list = FourEssentials.Instance.Homes.All(p.getName());
        if (list.Count == 0) { p.sendMessage("\u00a76Player has not set a home."); return true; }
        p.sendMessage(M.Format(M.Homes, string.Join(", ", list)));
        return true;
    }
}