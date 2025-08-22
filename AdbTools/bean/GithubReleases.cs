using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdbTools.bean
{
    public class GithubReleases
    {
        /// <summary>
        /// 发布的名称
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 上传构件
        /// </summary>
        public List<GithubReleasesAssets> Assets { get; set; }

        /// <summary>
        /// 发布的说明，Markdown
        /// </summary>
        public string Body { get; set; }

    }
}
