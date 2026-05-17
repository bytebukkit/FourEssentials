using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;

public class TpAllCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? dest;
        if (args.Length > 0) { dest = H.FindPlayer(sender, args[0]); if (dest == null) return true; }
        else { dest = H.RequirePlayer(sender); if (dest == null) return true; }
        sender.sendMessage("\u00a76Teleporting all players...");
        var loc = dest.getLocation();
        foreach (var pl in FourKit.getOnlinePlayers())
            if (pl.getName() != dest.getName())
                H.TeleportWithBack(pl, loc);
        return true;
    }
}