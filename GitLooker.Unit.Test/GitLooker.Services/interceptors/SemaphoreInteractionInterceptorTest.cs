﻿using FluentAssertions;
using GitLooker.Core;
using GitLooker.Core.Services;
using GitLooker.Services.interceptors;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GitLooker.Unit.Test.GitLooker.Services.interceptors
{
    public class SemaphoreInteractionInterceptorTest
    {
        private readonly SemaphoreInteractionInterceptor semaphoreInteractionInterceptor;
        private readonly IAppSemaphoreSlim appSemaphoreSlim;
        private readonly Castle.DynamicProxy.IInvocation invocation1;
        private readonly Castle.DynamicProxy.IInvocation invocation2;
        private bool hasExecuted;


        public SemaphoreInteractionInterceptorTest()
        {
            var methodInfo1 = GetType().GetMethod("Execute");
            var methodInfo2 = GetType().GetMethod("NoExecute");
            invocation1 = Mock.Of<Castle.DynamicProxy.IInvocation>();
            Mock.Get(invocation1).Setup(i => i.Method).Returns(methodInfo1);
            Mock.Get(invocation1).Setup(i => i.Proceed()).Callback(Execute);
            invocation2 = Mock.Of<Castle.DynamicProxy.IInvocation>();
            Mock.Get(invocation2).Setup(i => i.Method).Returns(methodInfo2);
            Mock.Get(invocation2).Setup(i => i.Proceed()).Callback(NoExecute);

            appSemaphoreSlim = Mock.Of<IAppSemaphoreSlim>();
            hasExecuted = default;

            semaphoreInteractionInterceptor = new SemaphoreInteractionInterceptor(appSemaphoreSlim);
        }

        [Fact]
        public void Intercept_check_for_Execute_method_should_use_Semaphore()
        {
            semaphoreInteractionInterceptor.Intercept(invocation1);

            Mock.Get(invocation1).Verify(i => i.Proceed(), Times.Once);
            invocation1.ReturnValue.Should().BeNull();
            Mock.Get(appSemaphoreSlim).Verify(s => s.Wait(), Times.Once);
            Mock.Get(appSemaphoreSlim).Verify(s => s.Release(), Times.Once);
            hasExecuted.Should().BeTrue();
        }

        [Fact]
        public void Intercept_check_for_Execute_methodWithException_should_use_Semaphore_and_ReturnValue()
        {
            Mock.Get(invocation1).Setup(i => i.Proceed()).Callback(() => throw new Exception());
            var expectedReturnValue = new AppResult<IEnumerable<string>>(new Exception());

            semaphoreInteractionInterceptor.Intercept(invocation1);
            var returnResult = invocation1.ReturnValue.As<AppResult<IEnumerable<string>>>().Error;

            Mock.Get(invocation1).Verify(i => i.Proceed(), Times.Once);
            returnResult.Should().HaveCount(1)
                .And
                .Contain(e => e.Message == "Exception of type 'System.Exception' was thrown.");
            Mock.Get(appSemaphoreSlim).Verify(s => s.Wait(), Times.Once);
            Mock.Get(appSemaphoreSlim).Verify(s => s.Release(), Times.Once);
            hasExecuted.Should().BeFalse();
        }

        [Fact]
        public void Intercept_check_for_NoExecute_method_should_not_use_Semaphore()
        {
            semaphoreInteractionInterceptor.Intercept(invocation2);

            Mock.Get(invocation2).Verify(i => i.Proceed(), Times.Once);
            invocation2.ReturnValue.Should().BeNull();
            Mock.Get(appSemaphoreSlim).Verify(s => s.Wait(), Times.Never);
            Mock.Get(appSemaphoreSlim).Verify(s => s.Release(), Times.Never);
            hasExecuted.Should().BeTrue();
        }

        public void Execute() => hasExecuted = true;

        public void NoExecute() => hasExecuted = true;
    }
}
