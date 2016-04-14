using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using BT.Web.ApiControllers.Base;

namespace BT.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private ILog logger = LogManager.GetLogger((typeof(MvcApplication)));
        protected void Application_Start()
        {
           GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            log4net.Config.XmlConfigurator.Configure();
           
       
            //替换原有的IHttpControllerSelector 为自定义
          

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
         
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            //捕捉和记录网站中出现的所有未处理错误，抛出详细的页面来源和访问ip，调用的接口方法及异常实例(详细说明)。 - 2012-02-13 
            HttpContext ctx = HttpContext.Current;
            if (ctx == null) return;
            try
            {
                Exception erroy = Server.GetLastError();
                if (erroy is HttpException)
                {
                    if (((HttpException)erroy).GetHttpCode() == 404)
                    {
                        Response.StatusCode = 404;
                        Server.ClearError();
                        return;
                    }
                }
                string LogErr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "；出错页面：" + Request.Url.ToString() + "；访问IP：" + Request.UserHostAddress.ToString() + "\r\n";
                LogErr += "异常信息：(" + erroy.Message + ")\r\n";
                LogErr += "异常方法：(" + erroy.TargetSite + ")\r\n";
                LogErr += "异常来源：(" + erroy.Source + ")\r\n";
                LogErr += "异常处理：\r\n" + erroy.StackTrace + "\r\n";
                LogErr += "异常实例：\r\n" + erroy.InnerException + "\r\n";
                LogErr += "//**********************************************************************************************************************" + "\r\n";

                logger.Error(LogErr);
                //**********************************************************************************************************************
                ////Window系统安全日志
                //Platform.Controllers.P_LogInfo.WriteTextLog(LogErr);
                ////文本文件安全日志,带类命名空间
                //Platform.Controllers.P_LogInfo.WriteTextLog("Platform.B2C", LogErr);
                ////文本文件安全日志
                //Platform.Controllers.P_LogInfo.WriteWindowLog(LogErr);
                ////数据库SQL安全日志
                //Platform.Controllers.P_LogInfo.WriteSQLLog("网站登陆", Platform.Controllers.P_LogInfo.LogErrType.B2CLog, LogErr, HttpContext.Current.User);
                //**********************************************************************************************************************
            }
            catch
            {
            }
            finally
            {
                //清除前一个异常
                //  Server.ClearError();

            }
        }
    }
}
