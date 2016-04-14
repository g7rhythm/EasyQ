using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Filters;
using log4net;
using Microsoft.Owin.Security.Infrastructure;

namespace BT.Web.ApiControllers.Base
{
    /// <summary>
    /// 异常拦截器
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private ILog logger = LogManager.GetLogger(typeof(ExceptionFilter));

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Exception erroy = actionExecutedContext.Exception;
            if (erroy is HttpException)
            {
                if (((HttpException)erroy).GetHttpCode() == 404)
                {
                  return;
                }
            }
            string LogErr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "；出错页面：" +
                            actionExecutedContext.Request.RequestUri.ToString();
            LogErr += "异常信息：(" + erroy.Message + ")\r\n";
            LogErr += "异常方法：(" + erroy.TargetSite + ")\r\n";
            LogErr += "异常来源：(" + erroy.Source + ")\r\n";
            LogErr += "异常处理：\r\n" + erroy.StackTrace + "\r\n";
            LogErr += "异常实例：\r\n" + erroy.InnerException + "\r\n";
            LogErr += "//**********************************************************************************************************************" + "\r\n";

            logger.Error(LogErr);

            //记录错误日志

            base.OnException(actionExecutedContext);
        }
    }
}