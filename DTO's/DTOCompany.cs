using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SGMP_Client.DTO_s
{
    public  class DTOCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public int BeneficiaryId { get; set; }
    }
}
