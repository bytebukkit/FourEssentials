using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System.Linq;
using System.Text.RegularExpressions;

public class NickCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        if (args.Length == 0) { sender.sendMessage(M.NotEnoughArgs); return true; }
        Player? target; string nickArg;
        if (args.Length >= 2 && FourKit.getPlayer(args[0]) != null)
        { target = FourKit.getPlayer(args[0])!; nickArg = string.Join(" ", args.Skip(1)); }
        else
        { target = H.RequirePlayer(sender); if (target == null) return true; nickArg = string.Join(" ", args); }

        if (nickArg.Equals("off", System.StringComparison.OrdinalIgnoreCase) ||
            nickArg.Equals(target.getName(), System.StringComparison.OrdinalIgnoreCase))
        {
            FourEssentials.Instance.Nicks.Remove(target.getName());
            target.setDisplayName(null);
            target.sendMessage(M.NickNoMore);
        }
        else
        {
            if (!Regex.IsMatch(nickArg, @"^[a-zA-Z_0-9\u00a7]+$"))
            { sender.sendMessage(M.NickAlpha); return true; }
            if (nickArg.Length > 30) { sender.sendMessage(M.NickTooLong); return true; }
            string stripped = Regex.Replace(nickArg, "\u00a7.", "").ToLower();
            foreach (var online in FourKit.getOnlinePlayers())
            {
                if (online.getName() == target.getName()) continue;
                string onlineStripped = Regex.Replace(online.getDisplayName(), "\u00a7.", "").ToLower();
                if (onlineStripped == stripped || online.getName().ToLower() == stripped)
                { sender.sendMessage(M.NickInUse); return true; }
            }
            FourEssentials.Instance.Nicks.Set(target.getName(), nickArg);
            target.setDisplayName(nickArg);
            target.sendMessage(M.Format(M.NickSet, nickArg));
        }
        if (sender is Player sp && sp.getName() != target.getName())
            sender.sendMessage(M.NickChanged);
        return true;
    }
}