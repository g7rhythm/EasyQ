namespace HQFramework
{
    /*
     场景编码规则
     * 把需要的数据输入到构造函数
     * Interact方法只负责主要业务逻辑，与所有依赖分离（包括WEB,数据库等）
     * Persist负责把需要保持的数据持久化，不包含任何逻辑判断
     * 完成一个场景之后需要的各种返回结果使用public成员公开出去给外部使用
     */

    public abstract class BaseContext
    {
        /// <summary>
        /// 场景交互时状态码，以NULL表示未初始化
        /// </summary>
        public string StatusCode;
        /// <summary>
        ///每个场景有唯一的交互触发点
        /// </summary>
        /// <returns>返回交互的结果CODE</returns>
        public virtual string Execute()
        {
            if (Interact())
                Persist();
            return StatusCode;
        }
        /// <summary>
        ///  开始交互（主要业务逻辑执行，与数据库分离）
        /// </summary>
        /// <returns>返回是否成功，用于判断是否运行后续持久化操作</returns>
        public abstract bool Interact();

        /// <summary>
        /// 业务逻辑完成后执行持久化操作
        /// </summary>
        /// <returns></returns>
        public abstract void Persist();
    }
}
