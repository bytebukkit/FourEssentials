using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;

public class GamemodeCommand : CommandExecutor
{
    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        GameMode? mode;
        string?   playerArg;

        mode = H.ParseMode(label);
        if (mode == null)
        {
            if (args.Length == 0) { sender.sendMessage(M.GameModeInv); return true; }
            mode = H.ParseMode(args[0]);
            if (mode == null) { sender.sendMessage(M.GameModeInv); return true; }
            playerArg = args.Length > 1 ? args[1] : null;
        }
        else
        {
            playerArg = args.Length > 0 ? args[0] : null;
        }

        Player? target;
        if (playerArg != null)
        {
            target = H.FindPlayer(sender, playerArg); if (target == null) return true;
        }
        else
        {
            target = H.RequirePlayer(sender); if (target == null) return true;
        }

        target.setGameMode(mode.Value);
        string modeName = mode.Value.ToString().ToLower();
        sender.sendMessage(M.Format(M.GameMode, modeName, target.getDisplayName()));
        if (sender is Player sp && sp.getName() != target.getName())
            target.sendMessage(M.Format(M.GameMode, modeName, target.getDisplayName()));
        return true;
    }
}