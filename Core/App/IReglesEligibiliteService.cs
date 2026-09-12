using FDL.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace FDL.Core.App
{
    public interface IReglesEligibiliteService
    {
        EligibiliteResult Evaluer(Menage menage);
    }
}
