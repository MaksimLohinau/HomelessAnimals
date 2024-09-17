using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface ICaptchaValidationService
    {
        Task<bool> IsHuman(string token);
    }
}
