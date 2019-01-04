using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

using Amazon.S3;
using Amazon.Lambda.Core;
using Amazon.S3.Util;
using Amazon.S3.Model;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace s3replication
{
    public class StepFunctionTasks
    {
        public AmazonS3Client S3Client { get; set; }
        
        string stxsd {get; set;}

        const string BUCKET_NAME = "";
        const string SCHEMA_FILENAME = "";
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public StepFunctionTasks()
        {
            S3Client = new AmazonS3Client();
            //var bucketName = System.Environment.GetEnvironmentVariable(BUCKET_NAME);
            //var schemaName = System.Environment.GetEnvironmentVariable(SCHEMA_FILENAME);
            string bucketName = "siri-lambda-test";
            string schemaName = "books.xsd";
            

            stxsd = GetObject(bucketName,schemaName).Result;
            //stxsd = GetObject("siri-lambda-test","books.xsd").Result;
            System.IO.File.WriteAllText("/tmp/books.xsd", stxsd);
        }


        public State Greeting(CWEvent cwevent, ILambdaContext context)
        {
            State state = new State();

            context.Logger.LogLine("Start with " +cwevent.detail.requestParameters.bucketName);
            state.Bucket = cwevent.detail.requestParameters.bucketName;
            state.Key = cwevent.detail.requestParameters.key;

            try
            {

                var s3object = GetObject(cwevent.detail.requestParameters.bucketName, cwevent.detail.requestParameters.key).Result;
                XmlReaderSettings settings = new XmlReaderSettings();
                byte[] byteArray = Encoding.ASCII.GetBytes(s3object);
                MemoryStream stream = new MemoryStream( byteArray );
                XmlReader xmlReaderS3object = XmlReader.Create(stream);
                string curFile = "/tmp/books.xsd";
                context.Logger.LogLine(File.Exists(curFile) ? "File exists." : "File does not exist.");
                settings.Schemas.Add("urn:bookstore-schema","/tmp/books.xsd");
                settings.ValidationType = ValidationType.Schema;
                //context.Logger.LogLine(s3object);

                XmlReader reader =XmlReader.Create(xmlReaderS3object, settings); 
                XmlDocument document = new XmlDocument();
                document.Load(reader);
                ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationCallBack);
                    
                document.Validate(eventHandler);
                state.XMLValid = true;
            }
            catch(Exception e)
            {
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                state.XMLValid = false;
                state.StateException = true;
            }

            //

           

            // Tell Step Function to wait 5 seconds before calling 
            
            state.WaitInSeconds = 5;
            state.TargetBucket = "siri-sandbox-bucket-1-us-east-2";

            return state;
        }

        public State Salutations(State state, ILambdaContext context)
        {
            state.Message += ", Goodbye";

            if (!string.IsNullOrEmpty(state.Bucket))
            {
                state.Message += " " + state.Bucket;
            }
 
            try {
                // Create a CopyObject request
                CopyObjectRequest request = new CopyObjectRequest
                {
                    SourceBucket = state.Bucket,
                    SourceKey = state.Key,
                    DestinationBucket = state.TargetBucket,
                    DestinationKey = state.Key
                };
 
                // Issue request
                S3Client.CopyObjectAsync(request);
            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                state.StateException = true;                
            }

            return state;
        }

        public async Task<string> GetObject(string bucket, string key)
        {
            var response = await S3Client.GetObjectAsync(bucket,key);
            using (var reader = new StreamReader(response.ResponseStream))
            {
                String s3object = await reader.ReadToEndAsync();
                return s3object;
            }
            
        }
      

        private static void ValidationCallBack(object sender, ValidationEventArgs e) {
            Console.WriteLine("Validation Error: {0}", e.Message);
        }

    }
}
