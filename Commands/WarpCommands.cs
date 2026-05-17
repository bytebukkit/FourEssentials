using Minecraft.Server.FourKit.Command;
using System;
using System.Linq;

public class SetWarpCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        if (args[0].Length == 0 || int.TryParse(args[0], out _))
        { p.sendMessage(M.InvalidWarp); return true; }
        FourEssentials.Instance.Warps.Set(args[0], p.getLocation());
        p.sendMessage(M.Format(M.WarpSet, args[0]));
        return true;
    }
}

public class WarpCommand : CommandExecutor
{
    private const int PerPage = 20;

    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var allWarps = FourEssentials.Instance.Warps.All();
        int pageNum  = 1;

        if (args.Length == 0 || int.TryParse(args[0], out pageNum))
        {
            if (allWarps.Count == 0) { sender.sendMessage(M.NoWarpsDef); return true; }
            if (pageNum <= 0) pageNum = 1;
            int maxPage = (int)Math.Ceiling(allWarps.Count / (double)PerPage);
            if (pageNum > maxPage) pageNum = maxPage;
            var slice = allWarps.Skip((pageNum - 1) * PerPage).Take(PerPage).ToList();
            if (allWarps.Count > PerPage)
            {
                sender.sendMessage(M.Format(M.WarpsCount, allWarps.Count, pageNum, maxPage));
                sender.sendMessage(string.Join(", ", slice));
            }
            else
            {
                sender.sendMessage(M.Format(M.Warps, string.Join(", ", slice)));
            }
            return true;
        }

        var p = H.RequirePlayer(sender); if (p == null) return true;
        var loc = FourEssentials.Instance.Warps.Get(args[0]);
        if (loc == null) { p.sendMessage(M.WarpNotExist); return true; }
        H.TeleportWithBack(p, loc);
        p.sendMessage(M.Format(M.WarpingTo, args[0]));
        return true;
    }
}

public class DelWarpCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        if (!FourEssentials.Instance.Warps.Delete(args[0])) { sender.sendMessage(M.WarpNotExist); return true; }
        sender.sendMessage(M.Format(M.DeleteWarp, args[0]));
        return true;
    }
}

public class WarpsCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var all = FourEssentials.Instance.Warps.All();
        if (all.Count == 0) { sender.sendMessage(M.NoWarpsDef); return true; }
        sender.sendMessage(M.Format(M.Warps, string.Join(", ", all)));
        return true;
    }
}