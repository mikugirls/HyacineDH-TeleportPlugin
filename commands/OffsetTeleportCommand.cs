using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyacineCore.Server.Command;
using HyacineCore.Server.Command.Command;
using HyacineCore.Server.GameServer.Game.Player;
using HyacineCore.Server.Util;

namespace TeleportPlugin
{
    [CommandInfo("offsettp", "相对位移传送", "用法: /offsettp <set1|set2|go|status|clear>")]
    public class OffsetTeleportCommand : ICommand
    {
        private static readonly Logger _logger = new Logger("OffsetTeleport");
        private static readonly Dictionary<int, Position> _pos1Store = new();
        private static readonly Dictionary<int, Position> _pos2Store = new();

        [CommandMethod("set1")]
        public async ValueTask HandleSet1(CommandArg arg)
        {
            var player = GetPlayerAndData(arg);
            if (player == null) 
            {
                await arg.SendMsg("未找到有效的玩家数据。");
                return;
            }

            if (player.Data.Pos == null)
            {
                await arg.SendMsg("无法记录坐标点1，当前位置数据为空。");
                return;
            }

            var currentPos = new Position(player.Data.Pos); 
            _pos1Store[player.Data.Uid] = currentPos;
            _logger.Info($"玩家 {player.Data.Name} 设置了坐标点1: ({currentPos.X}, {currentPos.Y}, {currentPos.Z})");
            await arg.SendMsg($"坐标点1已记录为你的当前位置。");
        }

        [CommandMethod("set2")]
        public async ValueTask HandleSet2(CommandArg arg)
        {
            var player = GetPlayerAndData(arg);
            if (player == null)
            {
                await arg.SendMsg("未找到有效的玩家数据。");
                return;
            }
            
            if (player.Data.Pos == null)
            {
                await arg.SendMsg("无法记录坐标点2，当前位置数据为空。");
                return;
            }

            var currentPos = new Position(player.Data.Pos);
            _pos2Store[player.Data.Uid] = currentPos;
            _logger.Info($"玩家 {player.Data.Name} 设置了坐标点2: ({currentPos.X}, {currentPos.Y}, {currentPos.Z})");
            await arg.SendMsg($"坐标点2已记录为你的当前位置。");
        }
        
        [CommandMethod("go")]
        public async ValueTask HandleGo(CommandArg arg)
        {
            var player = GetPlayerAndData(arg);
            if (player == null)
            {
                await arg.SendMsg("未找到有效的玩家数据。");
                return;
            }

            int uid = player.Data.Uid;
            if (!_pos1Store.TryGetValue(uid, out var p1) || !_pos2Store.TryGetValue(uid, out var p2))
            {
                await arg.SendMsg("错误: 你必须先通过 /offsettp set1 和 /offsettp set2 设置两个坐标点。");
                return;
            }

            var currentPos = player.Data.Pos;
            if (currentPos == null)
            {
                await arg.SendMsg("无法执行传送，你的当前位置数据为空。");
                return;
            }

            try
            {
                var offsetX = p2.X - p1.X;
                var offsetY = p2.Y - p1.Y;
                var offsetZ = p2.Z - p1.Z;
                var destination = new Position(currentPos.X + offsetX, currentPos.Y + offsetY, currentPos.Z + offsetZ);
                
                await player.MoveTo(destination);
                _logger.Info($"玩家 {player.Data.Name} 执行了相对位移传送。");
                await arg.SendMsg($"执行传送！");
            }
            catch (Exception ex)
            {
                _logger.Error("执行相对位移传送时出错。", ex);
                await arg.SendMsg("传送失败，发生内部错误。");
            }
        }

        [CommandMethod("status")]
        public async ValueTask HandleStatus(CommandArg arg)
        {
            var player = GetPlayerAndData(arg);
            if (player == null)
            {
                await arg.SendMsg("未找到有效的玩家数据。");
                return;
            }

            string p1Status = _pos1Store.TryGetValue(player.Data.Uid, out var p1) ? $"({p1.X},{p1.Y},{p1.Z})" : "未设置";
            string p2Status = _pos2Store.TryGetValue(player.Data.Uid, out var p2) ? $"({p2.X},{p2.Y},{p2.Z})" : "未设置";

            await arg.SendMsg($"当前状态:\n坐标点1: {p1Status}\n坐标点2: {p2Status}");
        }

        [CommandMethod("clear")]
        public async ValueTask HandleClear(CommandArg arg)
        {
            var player = GetPlayerAndData(arg);
            if (player == null)
            {
                await arg.SendMsg("未找到有效的玩家数据。");
                return;
            }
            
            _pos1Store.Remove(player.Data.Uid);
            _pos2Store.Remove(player.Data.Uid);
            await arg.SendMsg("已清除为你记录的所有坐标点。");
        }

        [CommandDefault]
        public async ValueTask HandleDefault(CommandArg arg)
        {
            await arg.SendMsg("相对位移传送指令用法:\n/offsettp set1 - 记录坐标点1\n/offsettp set2 - 记录坐标点2\n/offsettp go - 执行传送\n/offsettp status - 查看已记录的坐标\n/offsettp clear - 清除记录");
        }
        private PlayerInstance? GetPlayerAndData(CommandArg arg)
        {
            var player = arg.Target?.Player as PlayerInstance;
            if(player == null || player.Data == null)
            {
                return null;
            }
            return player;
        }
    }
}