using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIKeyValidatorMiddelware.Models
{
    public class SystemApiServiceRow
    {
        public int Id { get; set; }
        public string API_Service { get; set; }
        public string API_Key { get; set; }

    }
}
