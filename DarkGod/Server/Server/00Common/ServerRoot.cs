

namespace Server
{
    class ServerRoot
    {
        private static ServerRoot instance = null;

        /// <summary>
        /// singleton
        /// </summary>
        public static ServerRoot Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ServerRoot();
                }
                return instance;
            }
        }

        public void InitServerRoot()
        {
            //数据库TODO
            DBMgr.Instance.InitDBmgr();
            //服务层
            CfgSvc.Instance.InitCfgSvc();
            CacheSvc.Instance.InitCache();
            NetSvc.Instance.InitNetSvc();
            TimerSvc.Instance.InitTimerSvc();


            //业务系统
            LoginSys.Instance.InitLoginSys();
            GuideSys.Instance.InitGuideSys();
            StrongSys.Instance.InitStrongSys();
            ChatSys.Instance.InitChatSys();
            BuySys.Instance.InitBuySys();
            PowerSys.Instance.Init();
            taskSys.Instance.InitTaskSys();
            FubenSys.Instance.InitFubenSys();
        }

        public void Update()
        {
            NetSvc.Instance.Update();
            TimerSvc.Instance.Update();
        }

        private int SessionID = 0;
        public int GetSessionID()
        {
            if(SessionID > int.MaxValue)
            {
                SessionID = 0;
            }
            return SessionID += 1;
        }
    }
}
