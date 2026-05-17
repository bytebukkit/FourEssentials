using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

public class PTimeCommand : CommandExecutor
{
    private static readonly HashSet<string> GetAliases = new(StringComparer.OrdinalIgnoreCase)
        { "get", "list", "show", "display" };

    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0)
        {
            var self = H.RequirePlayer(sender); if (self == null) return true;
            showTime(sender, new[] { self });
            return true;
        }

        IEnumerable<Player> targets;
        if (args.Length >= 2)
        {
            if (args[1].Equals("*", StringComparison.OrdinalIgnoreCase) ||
                args[1].Equals("all", StringComparison.OrdinalIgnoreCase))
                targets = FourKit.getOnlinePlayers();
            else
            {
                var t = H.FindPlayer(sender, args[1]); if (t == null) return true;
                targets = new[] { t };
            }
        }
        else
        {
            var self = H.RequirePlayer(sender); if (self == null) return true;
            targets = new[] { self };
        }

        string timeArg = args[0];

        if (GetAliases.Contains(timeArg))
        {
            showTime(sender, targets);
            return true;
        }

        if (timeArg.Equals("reset", StringComparison.OrdinalIgnoreCase))
        {
            var names = string.Join(", ", targets.Select(t => t.getName()));
            foreach (var t in targets)
                t.getLocation().getWorld()?.setTime(FourKit.getWorld(0).getTime());
            sender.sendMessage(M.Format(M.PTimeReset, names));
            return true;
        }

        bool relative = true;
        if (timeArg.StartsWith("@")) { relative = false; timeArg = timeArg.Substring(1); }

        if (!H.TryParseTicks(timeArg, out long ticks))
        { sender.sendMessage(M.NotEnoughArgs); return true; }

        var playerNames = new List<string>();
        foreach (var t in targets)
        {
            var world = t.getLocation().getWorld();
            if (world != null)
            {
                long worldTime = world.getTime();
                long newTime   = (worldTime - worldTime % 24000) + 24000 + ticks;
                if (relative) newTime -= worldTime;
                world.setTime(newTime % 24000);
            }
            playerNames.Add(t.getName());
        }

        string nameStr = string.Join(", ", playerNames);
        string timeStr = H.FormatTicks(ticks);
        if (!relative)
            sender.sendMessage(M.Format(M.PTimeSetFixed, timeStr, nameStr));
        else
            sender.sendMessage(M.Format(M.PTimeSet, timeStr, nameStr));
        return true;
    }

    private void showTime(CommandSender sender, IEnumerable<Player> targets)
    {
        var list = targets.ToList();
        if (list.Count > 1) sender.sendMessage(M.PTimePlayers);
        foreach (var t in list)
            sender.sendMessage(M.Format(M.PTimeNormal, t.getName()));
    }
}