using Minecraft.Server.FourKit.Command;

public class TpHereCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        var p = H.RequirePlayer(sender); if (p == null) return true;
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
        H.TeleportWithBack(t, p.getLocation());
        t.sendMessage(M.Format(M.TeleportAtoB, H.DN(p), p.getDisplayName()));
        return true;
    }
}