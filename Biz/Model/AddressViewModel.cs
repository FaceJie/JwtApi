using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biz.Model
{
    public class AddressViewModel : IEquatable<AddressViewModel>
    {
        public string city { get; set; }
        public string address { get; set; }

        public bool Equals(AddressViewModel other)
        {
            return (this.address == other.address && this.city == other.city );
        }
    }
}
