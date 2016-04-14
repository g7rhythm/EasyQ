using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Async;

namespace BT.Contexts
{
     public  class TestContext:QueueBaseContext
    {
         private readonly int _userId;
         private object user;

         public TestContext(int userId)
         {
             _userId = userId;
         }

         public override bool Interact()
         {
             //逻辑
             StatusCode= "ResponseCode.Ok";
             return true;
          
         }

         public override void Persist()
         {
             //持久化

            
         }

         public override void InitData()
         {
              user = new {_userId = 1, Name = "XXX"}; // GetById(_userId);
         }
    }
}
