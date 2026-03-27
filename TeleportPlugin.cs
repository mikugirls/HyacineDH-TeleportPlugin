using System;
using HyacineCore.Server.GameServer.Plugin.Constructor;
using HyacineCore.Server.Util;
using HyacineCore.Server.Command.Command;
using TeleportPlugin;

namespace TeleportPlugin
{
    [PluginInfo("TeleportExtended", "多功能传送插件", "1.0.0")]
    public class TeleportPluginEntry : IPlugin
    {
        private static readonly Logger _logger = new Logger("TeleportExtended");
        
        public void OnLoad()
        {
            try
            {
                var cmdManager = CommandManager.Instance;
                if (cmdManager != null)
                {
                    cmdManager.RegisterCommand(typeof(TeleportCommand));
                    cmdManager.RegisterCommand(typeof(GetPositionCommand));
                    cmdManager.RegisterCommand(typeof(OffsetTeleportCommand));
                }
                
                _logger.Info("多功能传送插件已加载！/tp, /getpos, /offsettp 指令已注册！");
            }
            catch (Exception ex)
            {
                _logger.Error("加载多功能传送插件时发生错误。", ex);
            }
        }

        public void OnUnload()
        {
            _logger.Info("多功能传送插件已卸载。");
        }
    }
}