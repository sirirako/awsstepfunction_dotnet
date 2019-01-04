using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using s3replication;

namespace s3replication.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
        }

        [Fact]
        public void TestGreeting()
        {
            TestLambdaContext context = new TestLambdaContext();

            StepFunctionTasks functions = new StepFunctionTasks();

            var state = new State
            {
                Name = "MyStepFunctions"
            };


            state = functions.Greeting(state, context);

            Assert.Equal(5, state.WaitInSeconds);
            Assert.Equal("Hello MyStepFunctions", state.Message);
        }
    }
}
