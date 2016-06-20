using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadImageDemo.Processor
{
    /// <summary>
    /// 裁剪策略
    /// </summary>
    public enum ResizePolicy
    {
        /// <summary>
        /// 自动
        /// </summary>
        Auto,

        /// <summary>
        /// 使用快速模式
        /// </summary>
        Quick,

        /// <summary>
        /// 使用最佳裁剪模式
        /// </summary>
        HighQuality
    }
}