// FourEssentials
// Copyright (C) 2026 Byte_HD
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
using System;

public static class M
{
    public const string Heal         = "\u00a76You have been healed.";
    public const string HealOther    = "\u00a76Healed\u00a7c {0}\u00a76.";
    public const string HealDead     = "\u00a74You cannot heal someone who is dead!";
    public const string Feed         = "\u00a76Your appetite was sated.";
    public const string FeedOther    = "\u00a76You satiated the appetite of \u00a7c{0}\u00a76.";
    public const string FlyMode      = "\u00a76Set fly mode\u00a7c {0} \u00a76for {1}\u00a76.";
    public const string GodMode      = "\u00a76God mode\u00a7c {0}\u00a76.";
    public const string GodEnabled   = "\u00a7aenabled\u00a76 for\u00a7c {0}";
    public const string GodDisabled  = "\u00a7cdisabled\u00a76 for\u00a7c {0}";
    public const string GameMode     = "\u00a76Set game mode\u00a7c {0} \u00a76for \u00a7c{1}\u00a76.";
    public const string GameModeInv  = "\u00a74You need to specify a valid player/mode.";
    public const string Enabled      = "enabled";
    public const string Disabled     = "disabled";
    public const string Flying       = "flying";
    public const string Walking      = "walking";
    public const string Teleporting  = "\u00a76Teleporting...";
    public const string TeleportAtoB = "\u00a7c{0}\u00a76 teleported you to \u00a7c{1}\u00a76.";
    public const string TeleportDisabled = "\u00a7c{0} \u00a74has teleportation disabled.";
    public const string TeleportInvalid  = "Value of coordinates cannot be over 30000000";
    public const string BackUsage    = "\u00a76Returning to previous location.";
    public const string NoLocation   = "\u00a74No valid location found.";
    public const string HomeSet      = "\u00a76Home set to current location.";
    public const string InvalidHome  = "\u00a74Home\u00a7c {0} \u00a74doesn''t exist!";
    public const string InvalidHomeName = "\u00a74Invalid home name!";
    public const string DeleteHome   = "\u00a76Home\u00a7c {0} \u00a76has been removed.";
    public const string Homes        = "\u00a76Homes:\u00a7r {0}";
    public const string MaxHomes     = "\u00a74You cannot set more than\u00a7c {0} \u00a74homes.";
    public const string WarpSet      = "\u00a76Warp\u00a7c {0} \u00a76set.";
    public const string WarpingTo    = "\u00a76Warping to\u00a7c {0}\u00a76.";
    public const string WarpNotExist = "\u00a74That warp does not exist.";
    public const string WarpOverwrite= "\u00a74You cannot overwrite that warp.";
    public const string DeleteWarp   = "\u00a76Warp\u00a7c {0} \u00a76has been removed.";
    public const string Warps        = "\u00a76Warps:\u00a7r {0}";
    public const string WarpsCount   = "\u00a76There are\u00a7c {0} \u00a76warps. Showing page \u00a7c{1} \u00a76of \u00a7c{2}\u00a76.";
    public const string NoWarpsDef   = "\u00a76No warps defined.";
    public const string InvalidWarp  = "\u00a74Invalid warp name!";
    public const string NickSet      = "\u00a76Your nickname is now \u00a7c{0}\u00a76.";
    public const string NickNoMore   = "\u00a76You no longer have a nickname.";
    public const string NickChanged  = "\u00a76Nickname changed.";
    public const string NickInUse    = "\u00a74That name is already in use.";
    public const string NickAlpha    = "\u00a74Nicknames must be alphanumeric.";
    public const string NickTooLong  = "\u00a74That nickname is too long.";
    public const string MsgFormat    = "\u00a76[\u00a7c{0}\u00a76 -> \u00a7c{1}\u00a76] \u00a7r{2}";
    public const string ForeverAlone = "\u00a74You have nobody to whom you can reply.";
    public const string UserAFK      = "\u00a77{0} \u00a75is currently AFK and may not respond.";
    public const string Action       = "\u00a75* {0} \u00a75{1}";
    public const string Broadcast    = "\u00a7r\u00a76[\u00a74Broadcast\u00a76]\u00a7a {0}";
    public const string KickDefault  = "Kicked from server.";
    public const string KickExempt   = "\u00a74You cannot kick that person.";
    public const string KickedAll    = "\u00a74Kicked all players from server.";
    public const string PlayerKicked = "\u00a76Player\u00a7c {0} \u00a76kicked {1} for {2}.";
    public const string BanFormat    = "\u00a7cYou have been banned:\n\u00a7r{0}";
    public const string BanExempt    = "\u00a74You cannot ban that player.";
    public const string BanExemptOff = "\u00a74You may not ban offline players.";
    public const string DefaultBan   = "The Ban Hammer has spoken!";
    public const string PlayerBanned = "\u00a76Player\u00a7c {0} \u00a76banned\u00a7c {1} \u00a76for: \u00a7c{2}\u00a76.";
    public const string MutedPlayer  = "\u00a76Player\u00a7c {0} \u00a76muted.";
    public const string MutedFor     = "\u00a76Player\u00a7c {0} \u00a76muted for\u00a7c {1}\u00a76.";
    public const string YouMuted     = "\u00a76You have been muted!";
    public const string YouMutedFor  = "\u00a76You have been muted for\u00a7c {0}.";
    public const string MuteExempt   = "\u00a74You may not mute that player.";
    public const string MuteExemptOff= "\u00a74You may not mute offline players.";
    public const string MuteNotify   = "\u00a7c{0} \u00a76has muted player \u00a7c{1}\u00a76.";
    public const string UnmutedPlayer= "\u00a76Player\u00a7c {0} \u00a76unmuted.";
    public const string YouUnmuted   = "\u00a76You have been unmuted.";
    public const string Kill         = "\u00a76Killed\u00a7c {0}\u00a76.";
    public const string KillExempt   = "\u00a74You cannot kill \u00a7c{0}\u00a74.";
    public const string CurrentWorld = "\u00a76Current World:\u00a7c {0}";
    public const string PosX         = "\u00a76X: {0} (+East <-> -West)";
    public const string PosY         = "\u00a76Y: {0} (+Up <-> -Down)";
    public const string PosZ         = "\u00a76Z: {0} (+South <-> -North)";
    public const string PosYaw       = "\u00a76Yaw: {0} (Rotation)";
    public const string PosPitch     = "\u00a76Pitch: {0} (Head angle)";
    public const string Distance     = "\u00a76Distance: {0}";
    public const string MoveSpeed    = "\u00a76Set {0} speed to\u00a7c {1} \u00a76for \u00a7c{2}\u00a76.";
    public const string Repair       = "\u00a76You have successfully repaired your: \u00a7c{0}\u00a76.";
    public const string RepairNone   = "\u00a74There were no items that needed repairing.";
    public const string RepairFixed  = "\u00a74This item does not need repairing.";
    public const string RepairInvalid= "\u00a74This item cannot be repaired.";
    public const string RepairEnch   = "\u00a74You are not allowed to repair enchanted items.";
    public const string FullStack    = "\u00a74You already have a full stack.";
    public const string CantSpawn    = "\u00a74You are not allowed to spawn the item\u00a7c {0}\u00a74.";
    public const string SuicideMsg   = "\u00a76Goodbye cruel world...";
    public const string SuicideSucc  = "\u00a76Player \u00a7c{0} \u00a76took their own life.";
    public const string UserIsAway   = "\u00a77* {0} \u00a77is now AFK.";
    public const string UserIsNotAway= "\u00a77* {0} \u00a77is no longer AFK.";
    public const string ListAmount   = "\u00a76There are \u00a7c{0}\u00a76 out of maximum \u00a7c{1}\u00a76 players online.";
    public const string ListAfkTag   = "\u00a77[AFK]\u00a7r";
    public const string ConnPlayers  = "\u00a76Connected players\u00a7r";
    public const string Pong         = "Pong!";
    public const string Uptime       = "\u00a76Uptime:\u00a7c {0}";
    public const string TPS          = "\u00a76Current TPS = {0}";
    public const string GcMax        = "\u00a76Maximum memory:\u00a7c {0} MB.";
    public const string GcTotal      = "\u00a76Allocated memory:\u00a7c {0} MB.";
    public const string GcFree       = "\u00a76Free memory:\u00a7c {0} MB.";
    public const string GcWorld      = "\u00a76{0} \"\u00a7c{1}\u00a76\": \u00a7c{2}\u00a76 chunks, \u00a7c{3}\u00a76 entities, \u00a7c{4}\u00a76 tiles.";
    public const string Exp          = "\u00a7c{0} \u00a76has\u00a7c {1} \u00a76exp (level\u00a7c {2}\u00a76) and needs\u00a7c {3} \u00a76more exp to level up.";
    public const string ExpSet       = "\u00a7c{0} \u00a76now has\u00a7c {1} \u00a76exp.";
    public const string SeenOnline   = "\u00a76Player\u00a7c {0} \u00a76has been \u00a7aonline\u00a76 since \u00a7c{1}\u00a76.";
    public const string SeenOffline  = "\u00a76Player\u00a7c {0} \u00a76has been \u00a74offline\u00a76 since \u00a7c{1}\u00a76.";
    public const string UserUnknown  = "\u00a74Warning: The user ''\u00a7c{0}\u00a74'' has never joined this server.";
    public const string WhoisTop     = "\u00a76 ====== WhoIs:\u00a7c {0} \u00a76======";
    public const string WhoisNick    = "\u00a76 - Nick:\u00a7r {0}";
    public const string WhoisHealth  = "\u00a76 - Health:\u00a7r {0}/20";
    public const string WhoisHunger  = "\u00a76 - Hunger:\u00a7r {0}/20 (+{1} saturation)";
    public const string WhoisExp     = "\u00a76 - Exp:\u00a7r {0} (Level {1})";
    public const string WhoisLocation= "\u00a76 - Location:\u00a7r ({0}, {1}, {2}, {3})";
    public const string WhoisIP      = "\u00a76 - IP Address:\u00a7r {0}";
    public const string WhoisGamemode= "\u00a76 - Gamemode:\u00a7r {0}";
    public const string WhoisGod     = "\u00a76 - God mode:\u00a7r {0}";
    public const string WhoisOp      = "\u00a76 - OP:\u00a7r {0}";
    public const string WhoisFly     = "\u00a76 - Fly mode:\u00a7r {0} ({1})";
    public const string WhoisAFK     = "\u00a76 - AFK:\u00a7r {0}";
    public const string WhoisJail    = "\u00a76 - Jail:\u00a7r {0}";
    public const string WhoisMuted   = "\u00a76 - Muted:\u00a7r {0}";
    public const string TrueStr      = "\u00a7atrue\u00a7r";
    public const string FalseStr     = "\u00a74false\u00a7r";
    public const string NotFlying    = "not flying";
    public const string Vanished     = "\u00a76You are now completely invisible to normal users, and hidden from in-game commands.";
    public const string VanishMsg    = "\u00a76Vanish for {0}\u00a76: {1}";
    public const string PTimeNormal  = "\u00a7c{0}\u00a76's time is normal and matches the server.";
    public const string PTimeCurrent = "\u00a7c{0}\u00a76's time is\u00a7c {1}\u00a76.";
    public const string PTimeFixed   = "\u00a7c{0}\u00a76's time is fixed to\u00a7c {1}\u00a76.";
    public const string PTimeSet     = "\u00a76Player time is set to \u00a7c{0}\u00a76 for: \u00a7c{1}.";
    public const string PTimeSetFixed= "\u00a76Player time is fixed to \u00a7c{0}\u00a76 for: \u00a7c{1}.";
    public const string PTimeReset   = "\u00a76Player time has been reset for: \u00a7c{0}";
    public const string PTimePlayers = "\u00a76These players have their own time:\u00a7r";
    public const string PTimeOthPerm = "\u00a74You are not authorized to set other players' time.";
    public const string RealName     = "{0} is {1}";
    public const string SudoRun      = "\u00a76Forcing\u00a7c {0} \u00a76to run:\u00a7r /{1}";
    public const string SudoExempt   = "\u00a74You cannot sudo this user.";
    public const string NotEnoughArgs= "\u00a74Not enough arguments.";
    public const string PlayerNotFound="\u00a74Player not found.";
    public const string OnlyPlayers  = "\u00a74Only in-game players can use \u00a7c{0}\u00a74.";

    public static string Format(string template, params object[] args)
    {
        if (string.IsNullOrEmpty(template)) return string.Empty;
        try
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, template, args);
        }
        catch (FormatException)
        {
            string s = template;
            for (int i = 0; i < args.Length; i++)
                s = s.Replace("{" + i + "}", args[i]?.ToString() ?? "");
            return s;
        }
    }
}

public static class Short
{
    public const int MaxValue = 32767;
}