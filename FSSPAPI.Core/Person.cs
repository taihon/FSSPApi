using System.Collections.Generic;

namespace FSSPAPI.Core
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public string BirthDate { get; set; }
        public int Region { get; set; }
        public string BatchId { get; set; }
        public List<PhysicalResponse> Results { get; set; } = new List<PhysicalResponse>();
    }
}