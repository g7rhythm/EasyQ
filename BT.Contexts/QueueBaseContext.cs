using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc.Async;
using HQFramework;
using log4net;
using Newtonsoft.Json;


namespace BT.Contexts
{
   public abstract class  QueueBaseContext:BaseContext
    {
       private ILog log = LogManager.GetLogger(typeof(QueueBaseContext));
       private static ConcurrentDictionary<string, ConcurrentQueue<AsyncManager>> killQueues = new ConcurrentDictionary<string, ConcurrentQueue<AsyncManager>>();
        private static ConcurrentDictionary<string, Task> taskDic = new ConcurrentDictionary<string, Task>();
        private AsyncManager AsyncManager;
        private string quenekey;

       public void SetAsync(AsyncManager _AsyncManager)
       {
           SetAsync(_AsyncManager, "");
       }
       public void SetAsync(AsyncManager _AsyncManager, string _quenekey)
       {
           quenekey = _quenekey;
           this.AsyncManager = _AsyncManager;
       }

       public abstract void InitData();
       public override string Execute()
       {
           if (AsyncManager == null)
           {
               throw new Exception("必须调用SetAsync 设置AsyncManager对象");

           }
         var runtimeType=  this.GetType();
           var qKey = runtimeType.FullName + quenekey;
          // typeof().
           AsyncManager.OutstandingOperations.Increment();
           //开一个队列 判断是否有队列 
           if (killQueues.ContainsKey(qKey) == false)
           {
               killQueues.TryAdd(qKey, new ConcurrentQueue<AsyncManager>(new[] {AsyncManager}));
           }
           else
           {
               killQueues[qKey].Enqueue(AsyncManager);

           }
           Action ac = () =>
           {
              
               while (killQueues[qKey].IsEmpty == false)
               {
                 //  Thread.Sleep(15000);
                   log.DebugFormat("while 进来了  killQueueitemCount length:{0} ,Q num{1}", killQueues[qKey].Count, killQueues.Count);
                   AsyncManager item;
                   killQueues[qKey].TryDequeue(out item);//取出队列的一个进行处理
                   try
                   {

                       InitData();
                       if (Interact())//对应业务逻辑
                           Persist();

                       AsyncManager.Parameters["response"] = new { Code = this.StatusCode};
                       AsyncManager.OutstandingOperations.Decrement();
                   }
                   catch (Exception e)
                   {
                       log.ErrorFormat("出错，e msg:{0} ,trace:{1}", e.Message, e.StackTrace);
                       AsyncManager.Parameters["response"] = new { Code = "ResponseCode.DataError", Description = "服务器错误，请重试" };
                       AsyncManager.OutstandingOperations.Decrement();
                   }
               }
               //remove q
           };
           if (taskDic.ContainsKey(qKey) == false)
           {
               taskDic.TryAdd(qKey, Task.Factory.StartNew(ac));
           }
           if (taskDic[qKey].IsCompleted || taskDic[qKey].IsFaulted)
           {
               taskDic[qKey] = Task.Factory.StartNew(ac);
           }
           return "";
       }
     

    }
}
