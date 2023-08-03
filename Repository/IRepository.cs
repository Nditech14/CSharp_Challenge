using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Core.Repository
{
    public interface IRepository
    {
        object GenerateToken(string username,
          string email, IConfiguration config, string[] roles);

    }
}
