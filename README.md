# EasyQ
### 
		EasyQ是一个轻量级的专门用来处理高并发HTTP请求的框架。
		应用MVC异步Action机制实现了相同业务单线程排队处理，没有用任何读写锁。
		可以指定某一种业务创建一条队列，也可以指定某一种业务+某一种数据ID作为一条队列。
		如秒杀商品业务，可以指定所有秒杀业务都使用单线程排队处理，避免脏读 脏写。
		但是这样做的话，所有秒杀商品都会进入排队，显然是不科学的。
		所以扩展一种方式是： 秒杀业务+商品ID 作为队列名。
		当然不止商品ID，也可以是用户ID，商品分类等任意字符串作为队列名的后缀。
		
### 代码解析
		HomeController 是入口页面，需要继承AsyncController，使用MVC的异步Action
		BT.Contexts项目放置业务代码，所有Context需要继承抽象类QueueBaseContext，并且实现3个方法
		1，InitData 初始化数据，数据库获取数据的方法应该写在此处
		2，Interact 交互操作，数据模型之间的交互，业务代码的各种计算、判断等
		3，Persist 持久化操作，数据保存到数据库的操作应当写在此处。
		这3个方法的默认执行步骤非常简单 1=》2=》3
		在HomeController使用Context时，首先应该分开2个Action 如  TestAsync TestCompleted。这是MVC异步Action的机制决定
		TestAsync用来启动异步，TestCompleted是异步完成后的回调操作。这2个方法必须成对出现。具体原理请参考MSDN
			
		调用是URL为:{host}/home/text  注意Async后缀在路由时会被去掉。
		SetAsync方法必须传入AsyncManager对象，key是可选参数，如上所述是用来细分队列的，如果想根据商品ID生成队列，不同商品的秒杀行为在不同的队列中排队，就在此处用SetAsync传入key是商品ID
		
		public void TestAsync(string key)
        {
            //GET DATA
            TestContext context = new TestContext(1);
            context.SetAsync(AsyncManager, key);//参数为产品队列标识
            context.Execute();
        }
		再看回调方法
		public ActionResult TestCompleted()
        {
           var result =   AsyncManager.Parameters["response"];

         return Content(JsonConvert.SerializeObject( result));
        }
		所有Context执行后的结果以Parameters["response"]返回;

# 存在缺陷
###
		1 目前是单点设计，只能在单机上运行，还在研究横向扩展。
		2 性能还需要优化
		3 由于使用异步Action 导致每个Action必须一分为二。
		
		