using System;
using System.Collections.Generic;
using System.Text;

namespace s3replication
{
    /// <summary>
    /// The state passed between the step function executions.
    /// </summary>
    public class State
    {
        public string Bucket { get; set; }
        public string TargetBucket { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        public bool XMLValid { get; set; }
        public bool StateException { get; set; }        
        public int WaitInSeconds { get; set; } 
    }
}
