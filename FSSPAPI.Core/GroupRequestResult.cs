using System;
using System.Collections.Generic;
using System.Text;

namespace FSSPAPI.Core
{
    class GroupRequestResult
    {
        //{"status":"success","code":0,"exception":"","response":{"task":"2c10999c-b800-4a4b-a88d-0be44edd0281"}}
        public string Status { get; set; }
        public string Code { get; set; }
        public string Exception { get; set; }
        public TaskResponse Response { get; set; }
    }

    public class TaskResponse
    {
        public string Task { get; set; }
    }
}
