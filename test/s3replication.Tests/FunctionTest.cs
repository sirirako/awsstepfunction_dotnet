using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using s3replication;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace s3replication.Tests
{
    public class FunctionTest
    {
        IAmazonS3 s3Client;
        public FunctionTest()
        {
            
        }

        [Fact]
        public void TestGreeting()
        {
            TestLambdaContext context = new TestLambdaContext();
            IAmazonS3 s3Client = new AmazonS3Client(RegionEndpoint.USEast1);
            StepFunctionTasks functions = new StepFunctionTasks(s3Client);

            var state = new State();

            CWEvent cwevent = new CWEvent
            {
                region = "us-east-1",
                account = "573575823092",
                detail = new CWEvent.Detail
                {
                    userIdentity = new CWEvent.Detail.UserIdentity
                    {
                        account = "",
                        userName = "",
                        arn = ""
                    },
                    requestParameters = new CWEvent.Detail.RequestParameters
                    {
                        bucketName = "siri-sandbox-bucket-1-us-east-1",
                        key = "booksSchemaFail.xml"                    
                    }
                }
                
            };

            state = functions.XMLValidate(cwevent, context);

            Assert.Equal(5, state.WaitInSeconds);
            Assert.Equal(false, state.XMLValid);
        }
    }
}
