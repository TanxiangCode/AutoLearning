using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AutoLearning
{
    /// <summary>
    /// 文 件 名：UserInfo
    /// 创建人员：谭翔
    /// 创建时间：2018/10/23 11:16:39
    /// 说明描述：
    /// 修改记录：
    /// <para>R1、……</para>
    /// <para>R2、……</para>
    /// </summary>
    public class UserInfo
    {
        public string user_id { get; set; }
        
        public string login_name { get; set; }
        
        public string user_name { get; set; }
        public string EnrollCode { get; set; }

        public string Competent { get; set; }
        public CookieContainer LoginCookie { get; set; }
        public CookieContainer LearnEduCookies { get; set; }
    }
}
