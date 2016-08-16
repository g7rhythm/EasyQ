using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BT.Contexts;
using log4net;
using Newtonsoft.Json;

namespace BT.Web.Controllers
{
    public class HomeController : AsyncController
    {
        private ILog log = LogManager.GetLogger(typeof(QueueBaseContext));
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Heartbeat()
        {
            return Content("OK");
        }



        public void OrderAsync(string key)
        {
            //GET DATA
            TestContext context = new TestContext(1);
            context.SetAsync(AsyncManager, key);//参数为产品队列标识
            context.Execute();
        }
        public ActionResult OrderCompleted()
        {
            log.Debug("IndexCompleted 进来了");
            var result = AsyncManager.Parameters["response"];

            return Content(JsonConvert.SerializeObject(result));
        }
    }
}