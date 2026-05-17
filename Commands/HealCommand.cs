using Minecraft.Server.FourKit.Command;

public class HealCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length > 0)
        {
            var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
            if (t.getHealth() == 0) { sender.sendMessage(M.HealDead); return true; }
            t.setHealth(t.getMaxHealth());
            t.setFoodLevel(20);
            t.sendMessage(M.Heal);
            sender.sendMessage(M.Format(M.HealOther, t.getDisplayName()));
        }
        else
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            if (p.getHealth() == 0) { p.sendMessage(M.HealDead); return true; }
            p.setHealth(p.getMaxHealth());
            p.setFoodLevel(20);
            p.sendMessage(M.Heal);
        }
        return true;
    }
}