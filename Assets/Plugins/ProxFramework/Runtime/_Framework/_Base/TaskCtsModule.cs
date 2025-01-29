using System.Collections.Generic;
using System.Threading;

namespace ProxFramework.Base
{
    public static class TaskCtsModule
    {
        public class CtsInfo
        {
            public int id;
            public CancellationTokenSource cts;
        }
        
        public static List<CtsInfo> ctsList = new List<CtsInfo>();
        private static int _id = 0;

        public static CtsInfo GetCts()
        {
            var cts = new CancellationTokenSource();
            var ctsInfo = new CtsInfo
            {
                id = _id++,
                cts = cts
            };
            ctsList.Add(ctsInfo);
            return ctsInfo;
        }
        
        public static void CancelCts(int id)
        {
            var ctsInfo = ctsList.Find(x => x.id == id);
            if (ctsInfo == null) return;
            ctsInfo.cts.Cancel();
            ctsList.Remove(ctsInfo);
        }
        
        public static void CancelAllCts()
        {
            foreach (var ctsInfo in ctsList)
            {
                ctsInfo.cts.Cancel();
            }
            ctsList.Clear();
        }
    }
}