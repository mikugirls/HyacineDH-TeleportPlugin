using System.Threading.Tasks;
using HyacineCore.Server.Command;
using HyacineCore.Server.Command.Command;
using HyacineCore.Server.Database.Player;
using HyacineCore.Server.GameServer.Game.Player;
using HyacineCore.Server.Util;

namespace TeleportPlugin
{
    [CommandInfo("tp", "传送到指定坐标", "/tp <x> <y> <z>")]
    public class TeleportCommand : ICommand
    {
        private static readonly Logger _logger = new Logger("TeleportCommand");

        [CommandDefault]
        public async ValueTask HandleTeleport(CommandArg arg)
        {
            var player = arg.Target?.Player as PlayerInstance; 
            if (player == null) 
            {
                await arg.SendMsg("未找到目标玩家。");
                return;
            }
            if (arg.BasicArgs.Count < 3)
            {
                await arg.SendMsg("用法错误: /tp <X坐标> <Y坐标> <Z坐标>");
                return;
            }
            try 
            {
                int x = arg.GetInt(0);
                int y = arg.GetInt(1);
                int z = arg.GetInt(2);
                Position newPosition = new Position(x, y, z);
                await player.MoveTo(newPosition);
                _logger.Info($"已将玩家 {player.Data.Name} (UID: {player.Data.Uid}) 传送到 ({x}, {y}, {z})。");
                await arg.SendMsg($"传送成功！新位置: ({x}, {y}, {z})。");
            }
            catch (System.Exception ex)
            {
                _logger.Error("执行/tp指令时发生严重异常。", ex);
                await arg.SendMsg("指令执行失败，发生了一个内部错误。");
            }
        }
    }
}