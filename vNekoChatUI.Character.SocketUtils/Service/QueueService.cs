using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace vNekoChatUI.Character.SocketUtils.Service
{
    public interface IQueueService
    {
        public void EnqueueTask(Action<Action> task);
    }

    public class QueueService : IQueueService
    {
        private bool firstEntry = false;
        private readonly Queue<Action<Action>> taskQueue = new Queue<Action<Action>>();
        private readonly SemaphoreSlim taskDone = new SemaphoreSlim(0, 1);
        //server 1   个
        //client 1*n 个
        //这里   1   个，然后UI线程就顶不顺，添加新角色直接卡炸。弃用，留着以后复制粘贴

        public QueueService()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    //等个信号
                    taskDone.Wait();

                    lock (taskQueue)
                    {
                        // 取出任务
                        Action<Action>? task = null;

                        if (taskQueue.Count >= 1)
                        {
                            task = taskQueue.Dequeue();
                        }

                        // 执行任务
                        task?.Invoke(() =>
                        {
                            //任务执行完毕后再给个信号
                            taskDone.Release();
                        });
                    }
                }
            });
        }

        public void EnqueueTask(Action<Action> task)
        {
            lock (taskQueue)
            {
                // 将任务添加到队列中
                taskQueue.Enqueue(task);

                // 首个任务给个信号
                if (taskQueue.Count == 1)
                {
                    taskDone.Release();
                }
            }
        }
    }
}
