using System.Collections.Generic;

namespace FSSPAPI.Core
{
    internal class GroupRequest
    {
        public GroupRequest()
        {
        }

        public string token { get; internal set; }
        public List<SingleRequest> request { get; internal set; }
    }
}