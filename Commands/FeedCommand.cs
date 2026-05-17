using Minecraft.Server.FourKit.Command;

public class FeedCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length > 0)
        {
            var t = H.FindPlayer(sender, args[0]); if (t == null) return true;
            t.setFoodLevel(20);
            t.setSaturation(10f);
            t.setExhaustion(0f);
            t.sendMessage(M.Feed);
            sender.sendMessage(M.Format(M.FeedOther, t.getDisplayName()));
        }
        else
        {
            var p = H.RequirePlayer(sender); if (p == null) return true;
            p.setFoodLevel(20);
            p.setSaturation(10f);
            p.setExhaustion(0f);
            p.sendMessage(M.Feed);
        }
        return true;
    }
}