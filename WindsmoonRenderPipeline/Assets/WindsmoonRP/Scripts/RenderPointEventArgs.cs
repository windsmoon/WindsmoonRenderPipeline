using System;
using UnityEngine.Rendering;

namespace WindsmoonRP
{
    public class RenderPointEventArgs : EventArgs
    {
        #region fields
        private CommandBuffer commandBuffer;
        #endregion

        #region constructors
        public RenderPointEventArgs(CommandBuffer commandBuffer)
        {
            this.commandBuffer = commandBuffer;
        }
        #endregion

        #region properties

        public CommandBuffer CommandBuffer
        {
            get => commandBuffer;
        }
        #endregion
    }
}