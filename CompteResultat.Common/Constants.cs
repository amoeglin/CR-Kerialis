using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompteResultat.Common
{
    public sealed class C
    {
        public enum eCompanyProperties
        {
            Id = 1,
            Name = 2,
            Address = 3,
            Email = 4,
            ContactName = 5,
            Telephone = 6,
            Logo = 7,
            Type = 8,
            ParentId = 9
        }

        public enum eExcelGroupsGaranties
        {
            Type,
            GroupName,
            GarantyName,
            CodeActe,
            OrderNumber
        }

        public enum eExcelTypePrev
        {
            CodeSinistre,
            LabelSinistre
        }

        public enum eExcelCadencier
        {
            Year,
            DebutSurvenance,
            FinSurvenance,
            Month,
            Cumul
        }

        public enum eExcelGroupTypes
        {
            Sante,
            Prev
        }

        public enum eReportTypes
        {
            Standard,
            GlobalEnt,
            GlobalSubsid,
            GlobalSynthese
        }

        public enum eTypeComptes
        {
            Survenance,
            Comptable                        
        }

        public enum eUserRoles
        {
            Administrateur,
            Utilisateur
        }

        public enum eTVNodeTypes
        {
            Assureur,
            ParentCompany,
            Subsid,
            Contract,
            CompteResultat
        }

        #region FILE IMPORT

        public enum eImportFile
        {
            PrestaSante,
            CotisatSante,
            CotisatPrev,
            DecompPrev,
            Provisions,
            SinistrePrev,
            Demography,
            Exp
        }

        public enum eMOGImportFile
        {
            PrestationsEntMOG,
            PrestationsProdMOG,
            CotisationsEntMOG,
            CotisationsProdMOG,
            DemographyEntMOG,
            DemographyProdMOG,
            OtherFieldsMOG,
            CotPrevMOG,
            SinistrePrevMOG,
            DecompPrevMOG,
            ProvMOG,
            ExpMOG
        }

        public enum eConfigStrings
        {
            PrestSante,
            CotisatSante,
            Demography,
            OtherFields,
            CotisatPrev,
            SinistrePrev,
            DecomptePrev,
            Provisions,
            Experience
        }

        public enum eUploadSessionVar
        {
            UploadPrestFile,
            UploadCotFile,
            UploadDemoFile,
            UploadCotPrevFile,
            UploadSinistrePrevFile,
            UploadDecompPrevFile,
            UploadProvFile,
            UploadExpFile
        }

        public enum eDBTempTables
        {
            _TempPrestation,
            _TempCotisation,
            _TempDemography,
            _TempOtherFields
        }

        public enum eDBImportTables
        {
            PrestSante,
            CotisatSante,
            Demography,
            CotisatPrev,
            DecomptePrev,
            SinistrePrev,
            ProvPrev,
            _TempExpData
        }

        public enum eDBTempOtherFieldsColumns
        {
            ImportId,
            AssureurName,
            ContractId,
            Company,
            Subsid,
            Gestionnaire,
            Apporteur
        }

        public enum eReportFileTypes
        {
            Excel,
            Powerpoint,
            PDF
        }

        public enum eReportTemplateTypes
        {
            SANTE,
            PREV,
            PREV_GLOBAL,
            SANTE_GLOBAL,
            SANTE_SYNT,
            PREV_SYNTH
        }

        public enum eExcelSheetPrestaData
        {
            Prestation,
            Experience,
            Provision
        }

        #endregion

        public enum eResult
        {
            Success,
            Failure
        }

        public enum eFileType
        {
            Excel,
            ExcelMacro,
            CSV,
            PPT,
            PPTMacro,
            Other
        }

        public enum ePrevProv
        {
            Prev,
            Prov
        }
        
        //Constants required for File Transformation
        public const string cPRESTAFILE = "Prestations.csv";
        public const string cCOTISATFILE = "Cotisations.csv";
        public const string cDEMOFILE = "Demos.csv";

        public const string cREQ = "[[REQ]]";
        public const string cMAP = "=>";
        public const string cEQUAL = "==";
        public const string cVALSEP = ";";
        public const string cEMPTY = "EMPTY";
        public const int cEMPTYVAL = -1;

        //Global Folders - those pathe are updated with values defined in the Web.config
        public static string imageFolder = "Images";
        public static string imageRelFolder = "~/Images/";
        public static string logoFolder = "Logos";
        public static string logoRelFolder = "~/Logos/";
        public static string excelFolder = "Excel";
        public static string excelRelFolder = "~/Excel/";
        public static string uploadFolder = "Upload";
        public static string uploadRelFolder = "~/Upload/";
        public static string reportTemplateFolder = "ReportTemplates";
        public static string reportTemplateRelFolder = "~/ReportTemplates/";
        public static string excelCRFolder = "ExcelCR";
        public static string excelCRRelFolder = "~/ExcelCR/";

        public static string ExcelTemplateGlobalCompanySante = "ReportTemplateGlobalSante.xlsm";
        public static string ExcelTemplateGlobalCompanyPrevoyance = "ReportTemplateGlobalPrevoyance.xlsm";
        //public static string ExcelTemplateGlobalSubsid = "ReportTemplateGlobalFiliale.xlsm";

        public static int maxPrestaLines = 100000;
        public static int maxPrestaIterations = 10;

        //Helper Files
        public static string cPrevProvisionFileName = "PrevProvision.xlsm";

        public static int CO_BulkInsertMaxRows = 100000;
        public static int cNUMBROWSDELETEEXCEL = 100000;
        public static int cDEFAULTID = -1; // used for example for AssureurId in Group & Garanty Tables
        public static string cDEFAULTASSUREUR = "Paramètres par défaut"; // "DEFAULTASSUREUR"; // used in GroupGaranty Tables, when there is no specific Assureur specified
        public static string cDEFAULTCADENCIER = "Paramètres par défaut"; // "DEFAULTCADENCIER"; // used in GroupGaranty Tables, when there is no specific Assureur specified
        public static int cINVALIDID = -1;
        public static string cINVALIDSTRING = "";
        public static string cSTRINGSEP = "&&";
        public static string cISODATE = "yyyy-MM-dd";
        public static string cEXCELEXTENSION = ".xlsm";
        public static string cPPTEXTENSION = ".pptm";


        //Excel Sheets
        public static string cEXCELPREV = "DATA PREV";
        public static string cEXCELPREVINCAP = "DATA INCAP";
        public static string cEXCELPREVINVAL = "DATA INVAL";
        public static string cEXCELPREVDECES = "DATA DECES";
        public static string cEXCELPREVAUTRES = "DATA AUTRES";
        public static string cEXCELPREVPROV = "DATA PROV";
        public static string cEXCELPREVPROVINCAP = "DATA PROV INCAP";
        public static string cEXCELPREVPROVINVAL = "DATA PROV INVAL";
        public static string cEXCELPREVPROVDECES = "DATA PROV DECES";
        public static string cEXCELPREVPROVAUTRES = "DATA PROV AUTRES";
        public static string cEXCELDEMO = "DATA DEMO";
        public static string cEXCELCOT = "DATA COT";
        public static string cEXCELPREST = "DATA PREST";
        public static string cEXCELPRESTWITHOPTION = "DATA PREST2";
        public static string cEXCELEXP = "DATA EXP";
        public static string cEXCELPROV = "DATA PROV";
        public static string cEXCELQUARTILE = "DATA QUARTILE";
        public static string cEXCELGROUPGARANT = "AFFICHAGE";
        public static string cEXCELPAGEGARDE = "Page de garde";

        public static string cEXCELGLOBAL = "DATA";
        public static string cEXCELGLOBALGARANTIE = "DATA_GARANTIE";        
        public static string cEXCELGLOBALCUMUL = "DATA_CUMUL";
        public static string cEXCELGLOBALCUMULGARANTIE = "DATA_CUMUL_GARANTIE";

        //Synthese: data Produit & Data Entreprise
        public static string cEXCELSYNTHESEPROD = "DATA PRODUIT";
        public static string cEXCELSYNTHESEENT = "DATA ENTREPRISE";

        //Quartile Values in percent
        public static int cQuartile1 = 25;
        public static int cQuartile2 = 50;
        public static int cQuartile3 = 75;
        public static int cQuartile4 = 90;

        //Assureur Types
        public static string cASSTYPEPRODUCT = "_PRODUIT";
        public static string cASSTYPEENTERPRISE = "_ENTREPRISE";
        public static string cASSTYPEPRODUCTPREV = "_PREVOYANCE_PRODUIT";
        public static string cASSTYPEENTERPRISEPREV = "_PREVOYANCE_ENTREPRISE";

        #region SQL Server Constants

        //The value used to represent a null DateTime value
        public static DateTime NullDateTime = DateTime.MinValue;
        
        //The value used to represent a null decimal value
        public static decimal NullDecimal = decimal.MinValue;
        
        public static double NullDouble = double.MinValue;
        
        public static Guid NullGuid = Guid.Empty;
        
        public static int NullInt = int.MinValue;
       
        public static long NullLong = long.MinValue;

        public static float NullFloat = float.MinValue;
       
        public static string NullString = string.Empty;

        //Maximum DateTime value allowed by SQL Server
        public static DateTime SqlMaxDate = new DateTime(9999, 1, 3, 23, 59, 59);
       
        public static DateTime SqlMinDate = new DateTime(1753, 1, 1, 00, 00, 00);

        #endregion


    }
}
