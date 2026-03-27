using System.Threading.Tasks;
using HyacineCore.Server.Command;
using HyacineCore.Server.Command.Command;
using HyacineCore.Server.GameServer.Game.Player;
using HyacineCore.Server.Util;

namespace TeleportPlugin
{
    [CommandInfo("getpos", "获取你当前的坐标信息", "用法: /getpos")]
    public class GetPositionCommand : ICommand
    {
        private static readonly Logger _logger = new Logger("GetPositionCommand");

        [CommandDefault]
        public async ValueTask HandleGetPosition(CommandArg arg)
        {
            var player = arg.Target?.Player as PlayerInstance;
            if (player == null || player.Data == null) 
            {
                await arg.SendMsg("未找到有效的玩家数据。");
                return;
            }

            try
            {
                Position? currentPos = player.Data.Pos;
                if (currentPos == null)
                {
                    await arg.SendMsg("无法获取坐标信息，位置数据为空。");
                    return;
                }
                
                string posString = $"当前位置: X={currentPos.X}, Y={currentPos.Y}, Z={currentPos.Z}";
                _logger.Info($"玩家 {player.Data.Name} 查询了坐标: ({currentPos.X}, {currentPos.Y}, {currentPos.Z})");
                await arg.SendMsg(posString);
            }
            catch (System.Exception ex)
            {
                _logger.Error("获取坐标时发生错误。", ex);
                await arg.SendMsg("获取坐标失败，发生了一个内部错误。");
            }
        }
    }
}
