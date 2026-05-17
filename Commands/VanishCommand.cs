using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System;

public class VanishCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? target = null; bool other = false; bool? force = null;
        foreach (var a in args)
        {
            if (a.Equals("on",  StringComparison.OrdinalIgnoreCase)) { force = true;  continue; }
            if (a.Equals("off", StringComparison.OrdinalIgnoreCase)) { force = false; continue; }
            var f = FourKit.getPlayer(a);
            if (f != null) { target = f; other = true; }
        }
        target ??= H.RequirePlayer(sender);
        if (target == null) return true;

        var store    = FourEssentials.Instance.Vanish;
        bool enabled = force ?? !store.IsVanished(target.getName());
        store.Set(target.getName(), enabled);

        string stateStr = enabled ? M.Enabled : M.Disabled;
        target.sendMessage(M.Format(M.VanishMsg, target.getDisplayName(), stateStr));
        if (enabled) target.sendMessage(M.Vanished);
        if (other || (sender is Player sp && sp.getName() != target.getName()))
            sender.sendMessage(M.Format(M.VanishMsg, target.getDisplayName(), stateStr));
        return true;
    }
}