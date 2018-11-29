using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebApi.Models.ViewModel
{
    /// <summary>
    /// 签证办理视图模型
    /// </summary>
    public class VisaIndoViewModel
    {
        /// <summary>
        ///团号
        /// </summary>]
        public string GroupNo { get; set; }
        /// <summary>
        /// 实际送签时间
        /// </summary>
        public DateTime? RealTime { get; set; }
        /// <summary>
        /// 出签时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string client { get; set; }
        /// <summary>
        /// 未描述
        /// </summary>
        public DateTime? InTime { get; set; }
        /// <summary>
        /// 未描述
        /// </summary>
        public DateTime? OutTime { get; set; }
        /// <summary>
        /// 客人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 客人英文姓名
        /// </summary>
        public string EnglishName { get; set; }
        /// <summary>
        /// 客人性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 客人出生年月
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 客人护照号
        /// </summary>
        public string PassportNo { get; set; }
        /// <summary>
        /// 办理时间
        /// </summary>
        public DateTime? EntryTime { get; set; }
        /// <summary>
        /// 签证类型(团签\个签\其他)
        /// </summary>
        public string Types { get; set; }
        /// <summary>
        /// 办理国家
        /// </summary>
        public string Country { get; set; }
    }
}