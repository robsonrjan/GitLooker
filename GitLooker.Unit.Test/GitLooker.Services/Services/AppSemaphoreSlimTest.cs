using FluentAssertions;
using GitLooker.Services.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitLooker.Unit.Test.GitLooker.Services.Services
{
    [TestFixture]
    public class AppSemaphoreSlimTest
    {
        private const int maxRepoProcessingCount = 3;
        private AppSemaphoreSlim appSemaphoreSlim;

        [SetUp]
        public void BeforeEach()
        {
            appSemaphoreSlim = new AppSemaphoreSlim(maxRepoProcessingCount);
        }

        [TearDown]
        public void AfterEach()
        {
            appSemaphoreSlim.Dispose();
        }

        [Test]
        public async Task Wait_action_check_should_wait()
        {
            bool reachMax = default;
            List<Task> actionTasksList = new List<Task>();

            appSemaphoreSlim.OnUse = canMore => reachMax = canMore;

            for (int i = 0; i < maxRepoProcessingCount; i++)
            {
                actionTasksList.Add(Task.Run(() =>
                {
                    appSemaphoreSlim.Wait();
                    Task.Delay(500);
                }));
            }
            await Task.WhenAll(actionTasksList);

            reachMax.Should().BeTrue();

            appSemaphoreSlim.Release();

            appSemaphoreSlim.CurrentCount.Should().Be(1);

            appSemaphoreSlim.Release(maxRepoProcessingCount - 1);

            appSemaphoreSlim.CurrentCount.Should().Be(maxRepoProcessingCount);
        }

        [Test]
        public async Task Wait_action_check_should_waitAsync()
        {
            bool reachNone = default;
            List<Task> actionTasksList = new List<Task>();

            appSemaphoreSlim.OnUse = canMore => reachNone = canMore;

            for (int i = 0; i < maxRepoProcessingCount; i++)
            {
                actionTasksList.Add(Task.Run<Task>(async () =>
                {
                    await appSemaphoreSlim.WaitAsync();
                    await Task.Delay(500);
                    return Task.CompletedTask;
                }));
            }
            await Task.WhenAll(actionTasksList);

            appSemaphoreSlim.CurrentCount.Should().Be(0);

            appSemaphoreSlim.Release(maxRepoProcessingCount);

            reachNone.Should().BeTrue();
            appSemaphoreSlim.CurrentCount.Should().Be(maxRepoProcessingCount);
        }
    }
}
