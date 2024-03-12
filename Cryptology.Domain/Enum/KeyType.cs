using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptology.Domain.Enum
{
    public enum KeyType
    {
        [Display(Name = "Linear Equation Coefficients")]
        LinearEquation = 1,

        [Display(Name = "Nonlinea Equation Coefficients")]
        NonlinearEquation = 2,

        [Display(Name = "Password")]
        Password = 3
    }
}
