using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SahoAcs.WebService
{
    /// <summary>
    ///Fileupload 的摘要描述
    /// </summary>
    [WebService(Namespace = "SahoWeb")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class Fileupload : System.Web.Services.WebService
    {
        public Fileupload()
        {

        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
