using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;

public class SpawnCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        H.TeleportWithBack(p, new Location(FourKit.getWorld(0), 0, 64, 0));
        p.sendMessage(M.Teleporting);
        return true;
    }
}