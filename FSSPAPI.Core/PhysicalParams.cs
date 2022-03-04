using System;
using System.Collections.Generic;
using System.Text;

namespace FSSPAPI.Core
{
    public class PhysicalParams: RequestParams
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int region { get; set; }
        public string birthdate { get; set; }
        public PhysicalParams()
        {

        }
        public PhysicalParams(Person p)
        {
            firstname = p.FirstName;
            lastname = p.LastName;
            birthdate = p.BirthDate;
            region = p.Region;
        }
    }
}
