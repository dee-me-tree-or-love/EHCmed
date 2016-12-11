using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_Write_App
{
    public enum DiagnosisAndProcedures
    {
        Diagnosis,
        Procedures
    }

    public enum DAndRSubCat
    {
        Diseases,
        Allergies,
        Vaccines,
        Medication
    }

    static class MedTerms
    {
        public static Dictionary<DiagnosisAndProcedures, List<DAndRSubCat>> DandRCategorization = new Dictionary<DiagnosisAndProcedures, List<DAndRSubCat>>()
        {
            { DiagnosisAndProcedures.Diagnosis, new List<DAndRSubCat>() { DAndRSubCat.Allergies, DAndRSubCat.Diseases } },
            { DiagnosisAndProcedures.Procedures, new List<DAndRSubCat>() { DAndRSubCat.Vaccines, DAndRSubCat.Medication } }
        };


    }
}
