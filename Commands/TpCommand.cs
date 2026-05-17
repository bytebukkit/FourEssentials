using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System;

public class TpCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }

        if (args.Length == 1)
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
            H.TeleportWithBack(p, t.getLocation());
            p.sendMessage(M.Format("\u00a76Teleporting to \u00a7c{0}\u00a76.", t.getDisplayName()));
        }
        else if (args.Length == 2)
        {
            var mover = H.FindPlayer(sender, args[0]); if (mover == null) return true;
            var dest  = H.FindPlayer(sender, args[1]); if (dest == null) return true;
            H.TeleportWithBack(mover, dest.getLocation());
            mover.sendMessage(M.Format(M.TeleportAtoB, H.SN(sender), dest.getDisplayName()));
        }
        else if (args.Length >= 3)
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            bool hasPlayer = !double.TryParse(args[0], out _);
            Player? movee = hasPlayer ? H.FindPlayer(sender, args[0]) : p;
            if (movee == null) return true;
            int xi = hasPlayer ? 1 : 0;
            if (!double.TryParse(args[xi], out double x) ||
                !double.TryParse(args[xi+1], out double y) ||
                !double.TryParse(args[xi+2], out double z))
            { sender.sendMessage(M.NotEnoughArgs); return true; }
            if (Math.Abs(x) > 30_000_000 || Math.Abs(y) > 30_000_000 || Math.Abs(z) > 30_000_000)
            { sender.sendMessage(M.TeleportInvalid); return true; }
            var loc = new Location(movee.getLocation().getWorld(), x, y, z);
            H.TeleportWithBack(movee, loc);
            movee.sendMessage(M.Teleporting);
        }
        return true;
    }
}