using System;
using System.Collections.Generic;
using System.Text;

namespace s3replication
{
    public class CWEvent
    {
        public string region { get; set; }

        public string account { get; set; }

        public Detail detail { get; set; }

        public class Detail 
        {
            public UserIdentity userIdentity { get; set; }

            public class UserIdentity
            {
                public string account { get; set; }
                public string userName { get; set; }
                public string arn { get; set; }
            };

            public RequestParameters requestParameters { get; set; }
            public class RequestParameters
            {
                public string bucketName { get; set; }
                public string key { get; set; }
            }
        }
    }

}