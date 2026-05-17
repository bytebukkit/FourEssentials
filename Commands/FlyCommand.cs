using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;
using System;

public class FlyCommand : CommandExecutor
{
    private static void SendPlayerAbilities(Player p, bool canFly, bool flying, float flySpeed, float walkSpeed)
    {
        byte[] buf = new byte[10];
        buf[0] = 202;
        byte flags = 0;
        if (flying) flags |= 0x02;
        if (canFly) flags |= 0x04;
        buf[1] = flags;
        FourEssentials.WriteFloat(buf, 2, flySpeed);
        FourEssentials.WriteFloat(buf, 6, walkSpeed);
        p.getConnection().send(buf);
    }

    public bool onCommand(CommandSender sender, Command cmd, string label, string[] args)
    {
        Player? target = null;
        bool other = false;
        bool? force = null;

        if (args.Length == 0)
        {
            target = sender as Player;
            if (target == null) { sender.sendMessage(M.Format(M.OnlyPlayers, "/fly")); return true; }
        }
        else if (args.Length == 1)
        {
            if (args[0].Equals("on", StringComparison.OrdinalIgnoreCase)) force = true;
            else if (args[0].Equals("off", StringComparison.OrdinalIgnoreCase)) force = false;
            else
            {
                target = H.FindPlayer(sender, args[0]);
                if (target == null) return true;
                other = true;
            }

            if (target == null && force.HasValue)
            {
                target = sender as Player;
                if (target == null) { sender.sendMessage(M.Format(M.OnlyPlayers, "/fly")); return true; }
            }
        }
        else
        {
            target = H.FindPlayer(sender, args[0]); if (target == null) return true;
            other = true;
            if (args[1].Equals("on", StringComparison.OrdinalIgnoreCase)) force = true;
            else if (args[1].Equals("off", StringComparison.OrdinalIgnoreCase)) force = false;
            else { sender.sendMessage(M.NotEnoughArgs); return true; }
        }

        if (target == null) { sender.sendMessage(M.NotEnoughArgs); return true; }

        FourEssentials.Instance.flying.TryGetValue(target.getName(), out bool current);
        bool enabled = force ?? !current;

        FourEssentials.Instance.flying[target.getName()] = enabled;

        SendPlayerAbilities(target, canFly: enabled, flying: enabled, flySpeed: 0.05f, walkSpeed: 0.1f);

        target.setFallDistance(0f);

        string state = enabled ? M.Enabled : M.Disabled;
        target.sendMessage(M.Format(M.FlyMode, state, target.getDisplayName()));
        if (other || sender is not Player sp2 || sp2.getName() != target.getName())
            sender.sendMessage(M.Format(M.FlyMode, state, target.getDisplayName()));

        return true;
    }
}