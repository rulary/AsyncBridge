using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncBridge.Tests
{
    [TestClass]
    public class WhenAllTests
    {
        [TestMethod]
        public async Task GenericIEnumerableWithSomeContents()
        {
            var taskCompletionSources = new []
                                        {
                                            new TaskCompletionSource<int>(),
                                            new TaskCompletionSource<int>(),
                                            new TaskCompletionSource<int>()
                                        };
            var whenAllTask = TaskUtils.WhenAll(taskCompletionSources.Select(tcs => tcs.Task));
            taskCompletionSources[0].SetResult(1);
            taskCompletionSources[1].SetResult(2);
            taskCompletionSources[2].SetResult(3);
            var results = await whenAllTask;
            CollectionAssert.AreEquivalent(new[] {1, 2, 3}, results.ToList());
        }

        [TestMethod]
        public void DontCompleteOne()
        {
            var taskCompletionSources = new []
                                        {
                                            new TaskCompletionSource<int>(), 
                                            new TaskCompletionSource<int>()
                                        };
            var whenAllTask = TaskUtils.WhenAll(taskCompletionSources.Select(tcs => tcs.Task));
            taskCompletionSources[0].SetResult(1);
            Assert.IsFalse(whenAllTask.IsCompleted);
        }

        [TestMethod]
        public async Task NonGenericVersion()
        {
            var taskCompletionSources = new []
                                        {
                                            new TaskCompletionSource<int>(), 
                                            new TaskCompletionSource<int>()
                                        };
            var whenAllTask = TaskUtils.WhenAll(taskCompletionSources.Select(tcs => tcs.Task).Cast<Task>());
            taskCompletionSources[0].SetResult(1);
            Assert.IsFalse(whenAllTask.IsCompleted);
            taskCompletionSources[1].SetResult(1);
            await whenAllTask;
        }

        [TestMethod]
        public async Task ArrayVersion()
        {
            var taskCompletionSources = new[]
                                        {
                                            new TaskCompletionSource<int>(), 
                                            new TaskCompletionSource<int>()
                                        };
            Task whenAllTask = TaskUtils.WhenAll(taskCompletionSources.Select(tcs => tcs.Task).ToArray());
            taskCompletionSources[0].SetResult(1);
            Assert.IsFalse(whenAllTask.IsCompleted);
            taskCompletionSources[1].SetResult(1);
            await whenAllTask;
        }
    }
}