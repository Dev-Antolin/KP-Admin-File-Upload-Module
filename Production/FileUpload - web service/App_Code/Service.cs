using System;
using System.Linq;
using System.Web.Services;
using System.Data;
using MySql.Data.MySqlClient;
using log4net;
using System.Collections.Generic;



[WebService(Namespace = "http://localhost", Description = "MLKP FileUpload Web Service", Name = "MLhuillier")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Service : System.Web.Services.WebService
{
 
    
    public DBConnection dbconkp;
    private String pathdomestic;
    public MySqlCommand command;
    public MySqlTransaction trans = null;
   
    private Decimal chargeValue;
    public FileUploadSave fquery = new FileUploadSave();
    public UploadFiles upnew = new UploadFiles();
    UploadFile UF = new UploadFile();

    InsertTransaction.InsertPOTransaction r = new InsertTransaction.InsertPOTransaction();
    

    private static readonly ILog kplog = LogManager.GetLogger(typeof(Service));
    public Service()
    {

        try
        {
           
            pathdomestic = "C:\\kpconfig\\DBPartner.ini";
            IniFile iniDomestic = new IniFile(pathdomestic);
           // r.ConnectDB();
            ConnectPartners();
           
             log4net.Config.XmlConfigurator.Configure();

             StorePartnerFunc = new Dictionary<String, Func<String, Object>>();
             StorePartnerFunc.Clear();
             StorePartnerFunc.Add("BDO", String => TestForBDO(PartnerSession));
             StorePartnerFunc.Add("P2P", String => TestForP2P(PartnerSession));
             StorePartnerFunc.Add("BPI", String => TestForBPI(PartnerSession));
             StorePartnerFunc.Add("MOEX", String => TestForMOEX(PartnerSession));
             StorePartnerFunc.Add("DBP", String => TestForDBP(PartnerSession));
        }
        catch (Exception ex)
        {
            kplog.Fatal(ex.Message, ex);
            throw new Exception(ex.Message);
        }


    }
    Dictionary<String, Func<String, Object>> StorePartnerFunc;// = new Dictionary<String, Func<String, Object>>();
    
    private String PartnerName { get; set; }    
    private String PartnerSession { get; set; }
    
    [WebMethod]
    public Testing SubmitTempTable(string Partner, string Session)
    {        
        PartnerName = Partner;
        PartnerSession = Session;
        Func<String, Object> func = StorePartnerFunc[PartnerName];
        return (Testing)func(PartnerSession);
    }

   // [WebMethod]

    //public UploadFile FileUpload(String[] listdata, String AccountId, String Bnumber, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String file, Decimal TotalAmount, String Currency)
    //{
    //    //String[] listdata, String AccountId, String Bnumber, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String file, Decimal TotalAmount,String Currency
    //    //branchname, branchcode, stationcode, zonecode, operatorID, stationID)                                  
    //    //String[] listdata,
    //    //String[] listdata, String AccountId, String Bnumber, String bcode, String bname
    //    //String[] listdata,String AccountId
    //    //String[] listdata, String AccountId, String Bnumber
    //    //String bcode = "";
    //    //String bname = "";
    //    //String[] listdata = { "00001| |2011-08-09 10:00:00|TXNB09-0016|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg", "00002| |2011-08-09 10:00:00|TXNB09-0011|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg" };
    //    //String AccountId = "MLBPP120001";
    //    //String Bnumber = "1231616-5456";

    //    //file = "BPI";
    //    //String[] listdata = { "MLKP0000000062|001291861429655801|PICK-UP|||200|PHP|||PAID||||||20130118|11:54:11||027" };
    //    //String[] listdata = { "860090000236|001271138064706001|86020090623_0000000002|9997|02/05/2009|0|TESTREMITTER,|INDIVIDUAL|TEST|TESTBENE,|PKOUTUSD|T||123 PKOUTUSD ST CEBUANA LHUILLIER PICKUP OUTLET|||9997|||||                                                                                                                                                                                                        |USD|184" };
    //    //String[] listdata = { "ALBI20110204_00007|12101212104|10347083295979|ELIZABETH V. LAUSIN|||ML KWARTA PADALA|Cash Remittance|||6569.77|FROILAN HEGINA LAUSIN|||", "ALBI20110204_00008|12101212104|10347083295979|ELIZABETH V. LAUSIN|||ML KWARTA PADALA|Cash Remittance|||6569.77|FROILAN HEGINA LAUSIN|||", "ALBI20110204_00009|12101212104|10347083295979|ELIZABETH V. LAUSIN|||ML KWARTA PADALA|Cash Remittance|||6569.77|FROILAN HEGINA LAUSIN|||" };
    //    //String[] listdata = { "ALBI20110204_00007|000001|10347083295979|ELIZABETH V. LAUSIN|||ML KWARTA PADALA|Cash Remittance|||6569.77|FROILAN HEGINA LAUSIN|||", "ALBI20110204_00008|000002|10347088884205|LILIA C. GARCIA|||ML-KWARTA PADALA|Cash Remittance|09216185252||9151.09|WILLIE M REOXON JR|||", "ALBI20110204_00010|000003|10347062706508|MELANIE S SORIANO|||ML KWARTA PADALA|Cash Remittance|09198827251||2908.95|JONALYN SALES NAVIGAR|||", "ALBI20110204_00009|000004|10347096037611|VERONICA ORTIZ|||ML KWARTA PADALA|Cash Remittance|||13858.49|SHAHEEDUL ISLAM ISLAM|||", "ALBI20110204_00001|000005|10347000709279|DAISY FERNANDEZ|||M LHUILLIER|Cash Remittance|||9421.96|ROLLY CARO FERNANDEZ|||", "ALBI20110204_00002|000006|10347008765133|ROXANNE P MATEO|||ML KWARTA PADALA|Cash Remittance|09064886030||14456.85|VICTOR SALANO .|||", "ALBI20110204_00003|000007|10347021440984|ALVIN VILLACASTIN|||ML-KUWARTA PADALA|Cash Remittance|||2041.49|WILFREDO DIMACULANGAN LESCANO JR|||", "ALBI20110204_00004|10347021517286|VIRGINIA NICOLAS|||ML-KUWARTA PADALA|Cash Remittance|||2279.43|ALBERTO ARCA NICOLAS JR|||", "ALBI20110204_00005|000008|10347096000956|AURORA MASUPIL|||ML KWARTA PADALA|Cash Remittance|||6576.8|NINA D CABRERA|||", "ALBI20110204_00006|000009|10347083074244|HAZRAT MUDZMAR UTAL|||ML KWARTA PADALA|Cash Remittance|||3869.05|TIMHAR JUDLI ADORI|||" };    

    //    //String[] listdata = { "41611|ROMe-06091238|fdsafsad|dfsdffffdsffd|MI|dfsafsa|fdfdffdsf|d|dffffd|fd|900000|9000|90000|001091155884847403" };
    //    //String AccountId = "MLCIP13011";
    //    //String Bnumber = "";
    //    //String bname = "Cebu Head Office";
    //    //String bcode = "001";
    //    //String stationcode = "-414968788";
    //    //String zonecode = "1";
    //    //String operatorID = "REBU11056660";
    //    //String stationID = "1";
    //    //String file = "EXCEL";
    //    //Decimal TotalAmount = 2600;
    //    //String Currency = "PHP";
    //    int counter = 0;
    //    int ret = 1;
    //    string batchnum = generateBatchNo();
    //    //string TierCode = getTierCode(AccountId);
    //    if (file == "BPI")
    //    {

    //        int counterDuplicate = 0;
    //        int counterErrorInserting = 0;
    //        int counterErrorTier = 0;
    //        int counterSuccess = 0;
    //        decimal TotalInsertedTransaction = 0;

    //        //int i = 0;
    //        arrayListofduplicate = new String[listdata.Length];
    //        arrayListofErrorInserting = new String[listdata.Length];
    //        arrayListofErrorTier = new String[listdata.Length];
    //        arrayListOfSuccess = new String[listdata.Length];


    //        if (CheckTotalAmount(AccountId, TotalAmount, Currency).Equals(true))
    //        {
    //            foreach (String item in listdata)
    //            {
    //                String[] arraysplit = item.Split('|');
    //                String controlNo = arraysplit[2].ToString();

    //                decimal principal = Convert.ToDecimal(arraysplit[23].ToString());

    //                if (CheckTierCode(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransBPI(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else if (checkCreditLimit(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransBPI(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else
    //                {
    //                    arrayListofErrorTier[counterErrorTier] = item;
    //                    counterErrorTier = counterErrorTier + 1;
    //                    arrayListofduplicate[counterDuplicate] = "0";
    //                    counterDuplicate = counterDuplicate + 1;
    //                    arrayListofErrorInserting[counterErrorInserting] = "0";
    //                    counterErrorInserting = counterErrorInserting + 1;
    //                    arrayListOfSuccess[counterSuccess] = "0";
    //                    counterSuccess = counterSuccess + 1;
    //                }

    //            }

    //            if (counter > 0)
    //            {
    //                insertData(AccountId, batchnum, counter, Bnumber);
    //                decimal tAmount = getRBalance(AccountId, TotalInsertedTransaction, Currency);
    //                string data = UpdateRunningBalance(tAmount, AccountId, Currency);
    //                if (data == "Success")
    //                {
    //                    ret = 1;
    //                }
    //            }
    //            if (counter == listdata.Length)
    //            {
    //                ret = 0;
    //            }


    //            return new UploadFile { response = ret, BatchNumber = batchnum, dataDuplicate = arrayListofduplicate, dataErrorInserting = arrayListofErrorInserting, dataErrorTier = arrayListofErrorTier, dataSuccessFull = arrayListOfSuccess }; 
    //        }
    //        else
    //        {
    //            return new UploadFile { response = 1, msg = "The Account is not Active or The Total Amount of Transaction is Less than the Running Balance" };
    //        }


    //    }
    //    else if (file == "MOEX")
    //    {
    //        int counterDuplicate = 0;
    //        int counterErrorInserting = 0;
    //        int counterErrorTier = 0;
    //        int counterSuccess = 0;
    //        decimal TotalInsertedTransaction = 0;

    //        //int i = 0;
    //        arrayListofduplicate = new String[listdata.Length];
    //        arrayListofErrorInserting = new String[listdata.Length];
    //        arrayListofErrorTier = new String[listdata.Length];
    //        arrayListOfSuccess = new String[listdata.Length];
    //        if (CheckTotalAmount(AccountId, TotalAmount, Currency).Equals(true))
    //        {
    //            foreach (String item in listdata)
    //            {
    //                String[] arraysplit = item.Split('|');
    //                String controlNo = arraysplit[0].ToString();
    //                string blah = arraysplit[20].ToString();

    //                decimal principal = Convert.ToDecimal(arraysplit[20].ToString());

    //                if (CheckTierCode(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransMoneyExchanger(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else if (checkCreditLimit(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransMoneyExchanger(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else
    //                {
    //                    arrayListofErrorTier[counterErrorTier] = item;
    //                    counterErrorTier = counterErrorTier + 1;
    //                    arrayListofduplicate[counterDuplicate] = "0";
    //                    counterDuplicate = counterDuplicate + 1;
    //                    arrayListofErrorInserting[counterErrorInserting] = "0";
    //                    counterErrorInserting = counterErrorInserting + 1;
    //                    arrayListOfSuccess[counterSuccess] = "0";
    //                    counterSuccess = counterSuccess + 1;
    //                }

    //            }

    //            if (counter > 0)
    //            {
    //                insertData(AccountId, batchnum, counter, Bnumber);
    //                decimal tAmount = getRBalance(AccountId, TotalInsertedTransaction, Currency);
    //                string data = UpdateRunningBalance(tAmount, AccountId, Currency);
    //                if (data == "Success")
    //                {
    //                    ret = 1;
    //                }
    //            }
    //            if (counter == listdata.Length)
    //            {
    //                ret = 0;
    //            }


    //            return new UploadFile { response = ret, BatchNumber = batchnum, dataDuplicate = arrayListofduplicate, dataErrorInserting = arrayListofErrorInserting, dataErrorTier = arrayListofErrorTier, dataSuccessFull = arrayListOfSuccess };
    //        }
    //        else
    //        {
    //            return new UploadFile { response = 1, msg = "The Account is not Active or The Total Amount of Transaction is Less than the Running Balance" };
    //        }
    //    }
    //    else if (file == "DBP")
    //    {

    //        int counterDuplicate = 0;
    //        int counterErrorInserting = 0;
    //        int counterErrorTier = 0;
    //        int counterSuccess = 0;
    //        decimal TotalInsertedTransaction = 0;
    //        //int i = 0;
    //        arrayListofduplicate = new String[listdata.Length];
    //        arrayListofErrorInserting = new String[listdata.Length];
    //        arrayListofErrorTier = new String[listdata.Length];
    //        arrayListOfSuccess = new String[listdata.Length];
    //        if (CheckTotalAmount(AccountId, TotalAmount, Currency).Equals(true))
    //        {
    //            foreach (String item in listdata)
    //            {
    //                String[] arraysplit = item.Split('|');
    //                String controlNo = arraysplit[0].ToString();

    //                decimal principal = Convert.ToDecimal(arraysplit[10].ToString());

    //                if (CheckTierCode(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransDBP(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else if (checkCreditLimit(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransDBP(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else
    //                {
    //                    arrayListofErrorTier[counterErrorTier] = item;
    //                    counterErrorTier = counterErrorTier + 1;
    //                    arrayListofduplicate[counterDuplicate] = "0";
    //                    counterDuplicate = counterDuplicate + 1;
    //                    arrayListofErrorInserting[counterErrorInserting] = "0";
    //                    counterErrorInserting = counterErrorInserting + 1;
    //                    arrayListOfSuccess[counterSuccess] = "0";
    //                    counterSuccess = counterSuccess + 1;
    //                }

    //            }
    //            if (counter > 0)
    //            {
    //                insertData(AccountId, batchnum, counter, Bnumber);
    //                decimal tAmount = getRBalance(AccountId, TotalInsertedTransaction, Currency);
    //                string data = UpdateRunningBalance(tAmount, AccountId, Currency);
    //                if (data == "Success")
    //                {
    //                    ret = 1;
    //                }
    //            }
    //            if (counter == listdata.Length)
    //            {
    //                ret = 0;
    //            }

    //            return new UploadFile { response = ret, BatchNumber = batchnum, dataDuplicate = arrayListofduplicate, dataErrorInserting = arrayListofErrorInserting, dataErrorTier = arrayListofErrorTier, dataSuccessFull = arrayListOfSuccess };
    //        }
    //        else
    //        {
    //            return new UploadFile { response = 1, msg = "The Account is not Active or The Total Amount of Transaction is Less than the Running Balance" };
    //        }

    //    }

    //    else if (file == "BDO")
    //    {

    //        int counterDuplicate = 0;
    //        int counterErrorInserting = 0;
    //        int counterErrorTier = 0;
    //        int counterSuccess = 0;
    //        decimal TotalInsertedTransaction = 0;
    //        //int i = 0;
    //        arrayListofduplicate = new String[listdata.Length];
    //        arrayListofErrorInserting = new String[listdata.Length];
    //        arrayListofErrorTier = new String[listdata.Length];
    //        arrayListOfSuccess = new String[listdata.Length];
    //        if (CheckTotalAmount(AccountId, TotalAmount, Currency).Equals(true))
    //        {
    //            foreach (String item in listdata)
    //            {
    //                String[] arraysplit = item.Split('|');
    //                String controlNo = arraysplit[0].ToString();

    //                decimal principal = Convert.ToDecimal(arraysplit[5].ToString());

    //                if (CheckTierCode(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransBDO(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else if (checkCreditLimit(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransBDO(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(controlNo, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else
    //                {
    //                    arrayListofErrorTier[counterErrorTier] = item;
    //                    counterErrorTier = counterErrorTier + 1;
    //                    arrayListofduplicate[counterDuplicate] = "0";
    //                    counterDuplicate = counterDuplicate + 1;
    //                    arrayListofErrorInserting[counterErrorInserting] = "0";
    //                    counterErrorInserting = counterErrorInserting + 1;
    //                    arrayListOfSuccess[counterSuccess] = "0";
    //                    counterSuccess = counterSuccess + 1;
    //                }

    //            }
    //            if (counter > 0)
    //            {
    //                insertData(AccountId, batchnum, counter, Bnumber);
    //                decimal tAmount = getRBalance(AccountId, TotalInsertedTransaction, Currency);
    //                string data = UpdateRunningBalance(tAmount, AccountId, Currency);
    //                if (data == "Success")
    //                {
    //                    ret = 1;
    //                }
    //            }
    //            if (counter == listdata.Length)
    //            {
    //                ret = 0;
    //            }

    //            return new UploadFile { response = ret,BatchNumber = batchnum, dataDuplicate = arrayListofduplicate, dataErrorInserting = arrayListofErrorInserting, dataErrorTier = arrayListofErrorTier, dataSuccessFull = arrayListOfSuccess};
    //        }
    //        else
    //        {
    //            return new UploadFile { response = 1, msg = "The Account is not Active or The Total Amount of Transaction is Less than the Running Balance" };
    //        }

    //    }
    //    else if (file == "EXCEL")
    //    {
    //        int counterDuplicate = 0;
    //        int counterErrorInserting = 0;
    //        int counterErrorTier = 0;
    //        int counterSuccess = 0;
    //        decimal TotalInsertedTransaction = 0;
    //        int lengthdata = listdata.Length;
    //        //int i = 0;

    //        arrayListofduplicate = new String[lengthdata];
    //        arrayListofErrorInserting = new String[lengthdata];
    //        arrayListofErrorTier = new String[lengthdata];
    //        arrayListOfSuccess = new String[lengthdata];
    //        if (CheckTotalAmount(AccountId, TotalAmount, Currency).Equals(true))
    //        {
    //            foreach (String item in listdata)
    //            {
    //                String[] arraysplit = item.Split('|');
    //                string refno = arraysplit[1].ToString();
    //                decimal principal = Convert.ToDecimal(arraysplit[10].ToString());

    //                if (CheckTierCode(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransExcel(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(refno, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;
    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else if (checkCreditLimit(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTransExcel(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(refno, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;

    //                    }
    //                    else if (res == "2")
    //                    {
    //                        //arrayListofduplicate[counterDuplicate] = item;
    //                        //counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                    else if (res == "3")
    //                    {
    //                        //arrayListofErrorInserting[counterErrorInserting] = item;
    //                        //counterErrorInserting = counterErrorInserting + 1;

    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                    }
    //                }
    //                else
    //                {
    //                    arrayListofErrorTier[counterErrorTier] = item;
    //                    counterErrorTier = counterErrorTier + 1;
    //                    arrayListofduplicate[counterDuplicate] = "0";
    //                    counterDuplicate = counterDuplicate + 1;
    //                    arrayListofErrorInserting[counterErrorInserting] = "0";
    //                    counterErrorInserting = counterErrorInserting + 1;
    //                    arrayListOfSuccess[counterSuccess] = "0";
    //                    counterSuccess = counterSuccess + 1;
    //                }

    //            }
    //            if (counter > 0)
    //            {
    //                insertData(AccountId, batchnum, counter, Bnumber);
    //                decimal tAmount = getRBalance(AccountId, TotalInsertedTransaction, Currency);
    //                string data = UpdateRunningBalance(tAmount, AccountId, Currency);
    //                if (data == "Success")
    //                {
    //                    ret = 1;
    //                }
    //            }
    //            if (counter == listdata.Length)
    //            {
    //                ret = 0;
    //            }

    //            return new UploadFile { response = ret, BatchNumber = batchnum, dataDuplicate = arrayListofduplicate, dataErrorInserting = arrayListofErrorInserting, dataErrorTier = arrayListofErrorTier, dataSuccessFull = arrayListOfSuccess };
    //        }
    //        else
    //        {
    //            return new UploadFile { response = 1, msg = "The Account is not Active or The Total Amount of Transaction is Less than the Running Balance" };
    //        }

    //    }

    //    else
    //    {

    //        int counterDuplicate = 0;
    //        int counterErrorInserting = 0;
    //        int counterErrorTier = 0;
    //        int counterSuccess = 0;
    //        decimal TotalInsertedTransaction = 0;
    //        //int i = 0;

    //        arrayListofduplicate = new String[listdata.Length];
    //        arrayListofErrorInserting = new String[listdata.Length];
    //        arrayListofErrorTier = new String[listdata.Length];
    //        arrayListOfSuccess = new String[listdata.Length];
    //        if (CheckTotalAmount(AccountId, TotalAmount, Currency).Equals(true))
    //        {
    //            foreach (String item in listdata)
    //            {
    //                String[] arraysplit = item.Split('|');
    //                string refno = arraysplit[3].ToString();
    //                decimal principal = Convert.ToDecimal(arraysplit[4].ToString());

    //                if (CheckTierCode(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTrans(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {


    //                        Insertsotxnref(refno, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;


    //                    }
    //                    else if (res == "2")
    //                    {
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;


    //                    }
    //                    else if (res == "3")
    //                    {
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;


    //                    }

    //                }
    //                else if (checkCreditLimit(AccountId, principal,Currency).Equals(true))
    //                {
    //                    String res = SaveSendOutTrans(item, AccountId, batchnum, bname, bcode, stationcode, zonecode, operatorID, stationID,Currency);
    //                    if (res == "1")
    //                    {
    //                        Insertsotxnref(refno, AccountId, batchnum,Currency);
    //                        arrayListOfSuccess[counterSuccess] = item;
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;
    //                        TotalInsertedTransaction = TotalInsertedTransaction + principal;
    //                        counter = counter + 1;

    //                    }
    //                    else if (res == "2")
    //                    {
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = item;
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = "0";
    //                        counterErrorInserting = counterErrorInserting + 1;

    //                    }
    //                    else if (res == "3")
    //                    {
    //                        arrayListOfSuccess[counterSuccess] = "0";
    //                        counterSuccess = counterSuccess + 1;
    //                        arrayListofErrorTier[counterErrorTier] = "0";
    //                        counterErrorTier = counterErrorTier + 1;
    //                        arrayListofduplicate[counterDuplicate] = "0";
    //                        counterDuplicate = counterDuplicate + 1;
    //                        arrayListofErrorInserting[counterErrorInserting] = item;
    //                        counterErrorInserting = counterErrorInserting + 1;


    //                    }
    //                }
    //                else
    //                {
    //                    arrayListOfSuccess[counterSuccess] = "0";
    //                    counterSuccess = counterSuccess + 1;
    //                    arrayListofErrorTier[counterErrorTier] = item;
    //                    counterErrorTier = counterErrorTier + 1;
    //                    arrayListofduplicate[counterDuplicate] = "0";
    //                    counterDuplicate = counterDuplicate + 1;
    //                    arrayListofErrorInserting[counterErrorInserting] = "0";
    //                    counterErrorInserting = counterErrorInserting + 1;

    //                }

    //            }

    //            if (counter > 0)
    //            {
    //                insertData(AccountId, batchnum, counter, Bnumber);
    //                decimal tAmount = getRBalance(AccountId, TotalInsertedTransaction, Currency);
    //                string data = UpdateRunningBalance(tAmount, AccountId,Currency);
    //                if (data == "Success")
    //                {
    //                    ret = 1;
    //                }
    //            }
    //            if (counter == listdata.Length)
    //            {
    //                ret = 0;
    //            }

    //            return new UploadFile { response = ret, BatchNumber = batchnum, dataDuplicate = arrayListofduplicate, dataErrorInserting = arrayListofErrorInserting, dataErrorTier = arrayListofErrorTier, dataSuccessFull = arrayListOfSuccess };
    //        }
    //        else
    //        {
    //            return new UploadFile { response = 1, msg = "The Account is not Active or The Total Amount of Transaction is Less than the Running Balance" };
    //        }

    //    }


    //}    
    

    [WebMethod]
    public UploadFile FileUpload(String[] listdata, String AccountId, String Bnumber, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String file, Decimal TotalAmount, String Currency, Decimal charge)
    {
        //
        //String[] listdata, String AccountId, String Bnumber, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String file, Decimal TotalAmount, String Currency, Decimal charge
        //String[] listdata, String AccountId, String Bnumber, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String file, Decimal TotalAmount, String Currency, Decimal charge
        //String[] listdata, String AccountId, String Bnumber, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String file, Decimal TotalAmount, String Currency, Decimal charge
        string ret;
        string error = string.Empty;
        int resp = 0;
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        String sessionId = manager.CreateSessionID(Context);
        //String[] listdata = { "00003|001151116620326403|2012-12-05 14:45:13|42768|34,465.50|PHP|0.00||||Pay2Home Remittance|-|.|OTHERS|200105808G|N/A||3|37C Tanjong Pagar Road|Singapore                |Singapore|+6563722473|GARCIA|JOHNNIE|JAY|N/A|N/A|Philippines||3|N/A|" };
        //String[] listdata = { "DUB6139589|001153336720326403|Pick-up|DION|NOVA D|.|CARTA SATURNINO VILLENA .|ML KWARTA PADALA|5036.00|PHP|000|20120919|||||||52|365961.02|817|", "MAD198422|001151116620326493|Pick-up|GENIL|JAYSON B.|.|CALINISAN ALONA DALISAY .|ML KWARTA PADALA|10992.00|PHP|000|20120919|||||||52|365961.02|836|", "MAD666655|001151116620326405|Pick-up|MANDIADE|REMEDIOS OPINA|.|ARTELLASO ZENAIDA TOTICA .|ML KWARTA PADALA|5036.00|PHP|000|20120919|||||||52|365961.02|836|" };
        //String[] listdata = { "833110001998|001153666720326403|83320110305_0000000039|MLHBPI0000017462|03/05/2011|4|BROWN,|ROSALIE|D|DELA PENA,|ERLINDA|CABALLERO|||SAN CARLOS NEGROS OCCIDENTAL MLHUILLIER MLHUILLIER|9777||09109234864|||||PHP|3400|35.00" };
        //DBP
       // String[] listdata = { "ALBI78444894_77996|001153666720326403|17895914494489|ELIZABETH V. LAUSIN|||ML KWARTA PADALA|Cash Remittance|||1569.77|FROILAN HEGINA LAUSIN||||45.00" };
        //MOEX
        //String[] listdata = { "4218-00418281|20130208|1305|ANTONIO                  |LUSOTAN             |                    |HELENIA                  |CEBRIAN             |                    |                                                                                |CAGAYAN                                           |PHL|09127726465         |                    |                                                                                                                                                                                                        |8204-0710|DIVISORIA DE CAGAYAN                              |8204|MLHUILLIER                                        |PHL|2550800.00|PHP|N|                              |                                                  |                                        |6I8JH062KAQ3|4218-00018155            |NA |                    |                    |001041131123293709|0" };
        //String[] listdata = { "00001|001061153000951308|2013-03-14 10:14:29|HE13031474434|24,845.00|PHP|100.00|SALARY|Friend||Ableitinger|Markus|.|OTHERS|XXX|N/A|2000-11-11|0|.|New Territories          |Hong Kong||Elllorango|Gina|E|.|SARANGANI|Philippines||1||" };
        //String AccountId = "MLCIP130007";
        //String Bnumber = "";
        //String bname = "Cebu Head Office";
        //String bcode = "001";
        //String stationcode = "895118844";
        //String zonecode = "1";
        //String operatorID = "BELA09043084";
        //String stationID = "1";
        //String file = "TESTING";
        //Decimal TotalAmount = 992500;
        //String Currency = "PHP";
        //string batchnum = string.Empty;
        
        if (file == "TESTING")
        {
            ret = upnew.SaveTransactionP2PUpload(listdata, AccountId, Bnumber, bname, bcode, stationcode, zonecode, operatorID, stationID, Currency, sessionId);
            if (ret == "success")
            {
                resp = 1;
                UF.data = upnew.ListSuccess;

            }
            else if (ret == "failed")
            {
                resp = 0;
                UF.data = upnew.ListError;
            }
            else
            {
                resp = 2;
                return new UploadFile { response = resp, msg = ret };
                //return new UploadFile { response = resp, msg = upnew.ermsg };
            }
        }
        else if (file == "TESTINGBDO")
        {
            ret = upnew.SaveTransactionBDOUpload(listdata, AccountId, Bnumber, bname, bcode, stationcode, zonecode, operatorID, stationID, Currency, sessionId);
            if (ret == "success")
            {
                resp = 1;
                UF.data = upnew.ListSuccess;

            }
            else if (ret == "failed")
            {
                resp = 0;
                UF.data = upnew.ListError;
            }
            else
            {
                resp = 2;
                return new UploadFile { response = resp, msg = ret };
                //return new UploadFile { response = resp, msg = upnew.ermsg };
            }
        }
        else if (file == "TESTINGBPI")
        {
            ret = upnew.SaveTransactionBPIUpload(listdata, AccountId, Bnumber, bname, bcode, stationcode, zonecode, operatorID, stationID, Currency, sessionId);
            if (ret == "success")
            {
                resp = 1;
                UF.data = upnew.ListSuccess;

            }
            else if (ret == "failed")
            {
                resp = 0;
                UF.data = upnew.ListError;
            }
            else
            {
                resp = 2;
                return new UploadFile { response = resp, msg = ret };
                //return new UploadFile { response = resp, msg = upnew.ermsg };
            }
        }
        else if (file == "TESTINGDBP")
        {
            ret = upnew.SaveTransactionDBPUpload(listdata, AccountId, Bnumber, bname, bcode, stationcode, zonecode, operatorID, stationID, Currency, sessionId);
            if (ret == "success")
            {
                resp = 1;
                UF.data = upnew.ListSuccess;

            }
            else if (ret == "failed")
            {
                resp = 0;
                UF.data = upnew.ListError;
            }
            else
            {
                resp = 2;
                return new UploadFile { response = resp, msg = ret };
                //return new UploadFile { response = resp, msg = upnew.ermsg };
            }
        }
        else if (file == "TESTINGMOEX")
        {
            ret = upnew.SaveTransactionMOEXUpload(listdata, AccountId, Bnumber, bname, bcode, stationcode, zonecode, operatorID, stationID, Currency, sessionId);
            if (ret == "success")
            {
                resp = 1;
                UF.data = upnew.ListSuccess;

            }
            else if (ret == "failed")
            {
                resp = 0;
                UF.data = upnew.ListError;
            }
            else
            {
                resp = 2;
                return new UploadFile { response = resp, msg = ret };
                //return new UploadFile { response = resp, msg = upnew.ermsg };
            }
        }


        return new UploadFile { response = resp, array = UF.data, BatchNumber = upnew.MLbNumber, session = sessionId };
        
    }
    

   [WebMethod]
    public getIntegrationType IntegrationType(String data)
   {
       string intType = "";
       using(MySqlConnection connection1 = dbconkp.getConnection())
       {
           connection1.Open();
           try
           {
               using (command = connection1.CreateCommand())
               {
                   string query = "select integrationtype from kpadminpartners.accountintegration where accountid = @data";
                   command.CommandText = query;
                   command.Parameters.AddWithValue("data",data);
                   using(MySqlDataReader rdr = command.ExecuteReader())
                   {
                       if(rdr.HasRows)
                       {
                           rdr.Read();
                           intType = rdr["integrationtype"].ToString();
                       }
                   }
                   connection1.Close();
                   return new getIntegrationType { response = 0, IntegrationType = intType, msg = "Successful"};
                  
               }
               
           }
           catch(Exception ex)
           {
               connection1.Close();
               return new getIntegrationType { response = 1, msg = ex.Message };
           }
       }
   }

    [WebMethod]
        public SearchPartners ListofPartners()
    {

       
        using (MySqlConnection conPart = dbconkp.getConnection())
        {
            try
            {
                //getArrayClass[] RetData = new getArrayClass();
                
                conPart.Open();
                using (command = conPart.CreateCommand())
                {
                    int counter = 0;

                    String sql = "select distinct t1.accountname as AccountName,t1.accountid as AccountID, t1.address as Address,t3.integrationtype as IntegrationType, " +
                                 "t1.contactno as ContactNo,t1.accounttype as AccountType,t1.oldaccountid as OldAccountID " +
                                 "from kpadminpartners.accountlist t1 " +
                                 "inner join kpadminpartners.accountdetail t2 on t1.accountid = t2.accountid and t1.isactive = t2.isactive " +
                                 "inner join kpadminpartners.accountintegration t3 on t1.accountid = t3.accountid " +
                                 "where t2.isactive = '1' and t3.integrationtype = '2'";
                    
                    

                   // String sql = "select distinct t1.accountname as AccountName,t1.accountId as AccountID, t1.Address as Address," +
                    //   " t1.ContactNo as ContactNo,t1.currency as Currency,t1.Threshold as Threshold,t1.OldAccountID as OldAccountID from kpadminpartners.accountlist t1," +
                     //  " kpadminpartners.accountdetails t2 where t1.accountid = t2.accountid and t1.isactive = t2.isactive and t2.integrationtype in (2,4) and t1.isactive = 1";
                    //command.Parameters.AddWithValue("Currency", Currency);
                    command.CommandText = sql;
                    //here just try
                    using (MySqlDataReader ReaderCounter = command.ExecuteReader())
                    {
                        while (ReaderCounter.Read())
                        {
                            counter++;
                        }

                    }   

                   // getArrayClass[] RetData = new getArrayClass[counter];
                   
                    String[] RetData = new String[counter];
                    using (MySqlDataReader read = command.ExecuteReader())
                    {
                        int i = 0;
                            //String[] Data;
                            
                            while (read.Read())
                            {

                                RetData[i] = read["AccountName"].ToString() + "|" + read["AccountID"].ToString() + "|" + read["Address"].ToString() + "|" + read["ContactNo"].ToString() + "|" + read["AccountType"].ToString() + "|" + read["OldAccountID"].ToString();
                                i = i + 1;
                            }
                            
                            //RetData = Data.ToString();
                        
                    }
                    conPart.Close();
                    return new SearchPartners { ReturnData = RetData};


                    
                }

            }
            catch (MySqlException ex)
            {

                 
                conPart.Close();
                return new SearchPartners { respcode = 1, ErrorDetail = ex.ToString() };

            }
        }
       
              
    }
    [WebMethod]
    public SearchCurrency Currency()
    {
        int i = 0;
        int counter = 0;
        string[] curr;
       using(MySqlConnection con = dbconkp.getConnection())
       {
           try
           {
               con.Open();
               using(command = con.CreateCommand())
               {
                   string query = "Select distinct currency from kpadminpartners.accountdetail";
                   command.CommandText = query;

                   using(MySqlDataReader rdr = command.ExecuteReader())
                   {
                       if (rdr.HasRows)
                       {
                           while (rdr.Read())
                           {
                               counter = counter + 1;
                           }
                       }

                       else
                       {
                           con.Close();
                           return new SearchCurrency { msg = "no currency available"};
                       }
                   }
                   curr = new string[counter];
                   using (MySqlDataReader read = command.ExecuteReader())
                   {
                       if (read.HasRows)
                       {
                           while(read.Read())
                           {
                               curr[i] = read["currency"].ToString();
                               i = i + 1;
                           }
                       }
                   }
                   con.Close();
                   return new SearchCurrency {  ChooseCurrency = curr};


               }
           }
           catch (Exception ex)
           {
               con.Close();
               throw new Exception(ex.Message);
           }
       }
    }
    [WebMethod]
    public getCurrency GetPartnersCurrency(String AccountID)
    {

        String[] ds;
        int counter = 0;
        int i = 0;
        
        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    string query = "select currency from kpadminpartners.accountdetail where accountid = @AccountID";
                    command.Parameters.AddWithValue("AccountID", AccountID);
                    command.CommandText = query;
                   using(MySqlDataReader rdr = command.ExecuteReader())
                   {
                      
                       if (rdr.HasRows)
                       {
                          

                           while (rdr.Read())
                           {
                               counter = counter + 1;
                           }
                           
                       }
                       else
                       {
                           con.Close();
                           return new getCurrency { msg = "no currency found on that partners" };
                       }

                   }

                   ds = new String[counter];
                  
                    using(MySqlDataReader read = command.ExecuteReader())
                    {
                        while (read.Read())
                        {
                            ds[i] = read["currency"].ToString();
                            i = i + 1;
                        }   
                    }

                }
                con.Close();
                return new getCurrency { Currency = ds };

            }
            catch (Exception ex)
            {
                con.Close();
                throw new Exception(ex.ToString());
            }
        }
    }
    [WebMethod]
    public ChargeValue ChargesPerLine(String AccId, Decimal amount, String Currency)
    {
        decimal value;
        string data;
        
        data = getChargeType(AccId, Currency);
        string[] datasplit = data.Split('|');
        string Ctype = datasplit[0].ToString();
        string charge = datasplit[1].ToString();
        if (Ctype == "Tier Bracket")
        {
            value = GetChargeValue(AccId, Currency, amount);
        }
        else
        {
            value = Convert.ToDecimal(charge);
        }
       

        return new ChargeValue { response = 0, Charges = value };
    }

    [WebMethod]
    public getserverDate GetDate()
    {
        String fromserverdate;
        using (MySqlConnection dcon = dbconkp.getConnection())
        {
            try
            {
                dcon.Open();
                using (command = dcon.CreateCommand())
                {
                    String query = "Select NOW() as serverdt";
                    command.CommandText = query;
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        rdr.Read();
                        fromserverdate = rdr["serverdt"].ToString();
                        
                        rdr.Close();
                        dcon.Close();
                        return new getserverDate { getfromDateServer = fromserverdate };
                        
                    }
                }
            }
            catch (Exception ex)
            {
                kplog.Fatal(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }
    }
    [WebMethod]
    public CheckTotalAmountInFile CheckTheTotalAmount(String AccId, Decimal totalAmount, String Currency)
    {
        bool result = false;
        result = CheckTotalAmount(AccId, totalAmount, Currency);

        return new CheckTotalAmountInFile { identifier = result };
    }
    [WebMethod]
    public UpdateNewRbalance UpdateNewRunningBalance(String accid,Decimal TotalAmount, String currency)
    {
        decimal NeWr;
        string result;
        Int16 ret = 0;
        NeWr = getRBalance(accid, TotalAmount, currency);
        result = UpdateRunningBalance(NeWr, accid, currency);
        if (result == "Failed")
        {
            ret = 1;
        }
        else if (result == "Success")
        {
            ret = 0;
        }
        return new UpdateNewRbalance { result = ret };
    }

    public string getChargeType(string Accountid, string Currency)
    {
        string value = string.Empty;
            using(MySqlConnection con = dbconkp.getConnection())
            {
                try
                {
                    con.Open();
                    using(command = con.CreateCommand())
                    {
                        string sql = "select chargetype,chargeamount from kpadminpartners.accountdetail where accountid = '" + Accountid + "' and currency = '" + Currency + "' ";
                        command.CommandText = sql;
                        using(MySqlDataReader rdr = command.ExecuteReader())
                        {
                            if(rdr.HasRows)
                            {
                                rdr.Read();
                                value = rdr["chargetype"].ToString() + "|" + rdr["chargeamount"].ToString();
                            }
                        }
                        con.Close();
                    }   
                }
                catch (Exception ex)
                {
                    con.Close();
                    throw new Exception(ex.Message);
                }
            }
            return value;
    }
   

    private decimal getRBalance(String AccId, Decimal TINAmount, String Currency)
    {
        decimal NewBalance = 0;   
        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                    using(command = con.CreateCommand())
                    {
                        string query = "Select runningbalance,creditlimit from kpadminpartners.accountdetail where accountid = @AccId and currency = @Currency";
                        command.CommandText = query;
                        command.Parameters.AddWithValue("AccId", AccId);
                        command.Parameters.AddWithValue("Currency", Currency);

                        using (MySqlDataReader rdr = command.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                decimal rB = Convert.ToDecimal(rdr["runningbalance"].ToString());
                                decimal cL = Convert.ToDecimal(rdr["creditlimit"].ToString());

                                decimal result = rB - TINAmount;
                                NewBalance = result;
                                if (result < 0)
                                {
                                    result = (rB + cL) - TINAmount;
                                }

                                
                            }
                        }
                 
                    }

                    con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                throw new Exception(ex.ToString());
            }
        }
        return NewBalance;
    }
    
    private string UpdateRunningBalance(Decimal NewRbalance, String AccId, String Currency)
    {
        int res;
        string result;
        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using(command = con.CreateCommand())
                {
                    string query = " UPDATE kpadminpartners.accountdetail Set runningbalance = @NewRbalance where accountid = @AccId and currency = @Currency";
                    command.CommandText = query;
                    command.Parameters.AddWithValue("NewRbalance", NewRbalance);
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Currency", Currency);
                    res = command.ExecuteNonQuery();

                    if (res.Equals("-1"))
                    {
                        result = "Failed";
                        con.Close();
                        return result;
                    }
                    result = "Success";
                }
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                return ex.Message;
            }
        }

        return result;
    }

    private Boolean CheckTotalAmount(String AccId, Decimal totalAmount,String Currency)
    {
        int credLimstat;
        decimal rBalance;
        decimal newRbalance;
        decimal credLim;
        bool ret = false;
        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    string query = "Select creditActivation,runningbalance,creditlimit from kpadminpartners.accountdetail where accountid = @AccId and currency = @Currency";
                    command.CommandText = query;
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Currency", Currency);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            credLimstat = Convert.ToInt32(reader["creditActivation"]);
                            rBalance = Convert.ToDecimal(reader["runningbalance"]);
                            credLim = Convert.ToDecimal(reader["creditlimit"]);
                            
                            if (totalAmount <= rBalance)
                            {
                                ret = true;
                            }

                            else 
                            {

                                if (credLimstat == 1)
                                {
                                    newRbalance = (rBalance + credLim) - totalAmount;
                                    if (newRbalance >= 0)
                                    {
                                        ret = true;
                                    }

                                }
                                else 
                                {

                                    ret = false;
                                }
                            
                            }
                            
                            
                            //if (credLimstat == 1)
                            //{
                                
                            //    if (totalAmount <= rBalance)
                            //    {
                            //        ret = true;
                            //    }
                            //    else
                            //    {
                            //        newRbalance = (rBalance + credLim) - totalAmount;
                            //        if(newRbalance >= 0)
                            //        {
                            //            ret = true;
                            //        }
                            //    }
                            //}                           
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

        return ret;
    }

    public Boolean CheckTierCode(String AccId, Decimal amount, String Currency)
    {
        decimal minAmount, maxAmount, charge, thold, Rbalance, credlim;
        string Tcode;
        int ret = 0;
        bool datareturn = false;
        using (MySqlConnection dbcon = dbconkp.getConnection())
        {
            try
            {
                dbcon.Open();
                using (command = dbcon.CreateCommand())
                {
                    //string query = "select b.Minimum,b.Maximum,b.TierCode,b.Charge,c.Threshold,c.runningbalance,c.creditlimit from kpadminpartners.tierdetails b " +
                    //                "inner join kpadminpartners.accountcharging a on b.TierCode = a.BracketTierCode " +
                    //                "inner join kpadminpartners.accountlist c on a.AccountID = c.AccountID " +
                    //                "where a.AccountID = @AccId";
                    string query = "select b.Minimum,b.Maximum,b.TierCode,b.Chargeamount,a.thresholdamount,a.runningbalance,a.creditlimit from kpadminpartners.tierdetails b " +
                                   "inner join kpadminpartners.accountdetail a on b.TierCode = a.BracketTierCode " +
                                   "where a.accountid = @AccId and a.currency = @Currency";

                    command.CommandText = query;
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Currency", Currency);
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                            maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                            Tcode = rdr["TierCode"].ToString();
                            charge = Convert.ToDecimal(rdr["Chargeamount"].ToString());
                            thold = maxAmount;//Convert.ToDecimal(rdr["thresholdamount"]);
                            Rbalance = Convert.ToDecimal(rdr["runningbalance"].ToString());
                            credlim = Convert.ToDecimal(rdr["creditlimit"].ToString());
                            if (amount <= thold && (amount >= minAmount && amount <= maxAmount))
                            {
                                    datareturn = true;
                                    chargeValue = charge;
                            }
                            ret = 1;
                        }
                    }
                    dbcon.Close();
                    if(ret == 0)
                    {
                        datareturn = checkTiercodeBillsPay(AccId, amount,Currency);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        return datareturn;
    }

    private Decimal GetChargeValue(String AccId,String Curr,Decimal amount)
    {
        decimal charge = 0;
        int result = 0;
        
        using(MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using(command = con.CreateCommand())
                {
                    string sql = "select b.Minimum,b.Maximum,b.TierCode,b.Chargeamount,a.thresholdamount from kpadminpartners.tierdetails b " +
                                   "inner join kpadminpartners.accountdetail a on b.TierCode = a.BracketTierCode " +
                                   "where a.accountid = @AccId and Currency = @Curr";
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Curr", Curr);
                    command.CommandText = sql;
                    using(MySqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            decimal minAmount = Convert.ToDecimal(rdr["Minimum"]);
                            decimal maxAmount = Convert.ToDecimal(rdr["Maximum"]);
                            decimal thold = maxAmount; //Convert.ToDecimal(rdr["thresholdamount"]);
                            decimal chargeAmount = Convert.ToDecimal(rdr["Chargeamount"]);
                            if ((amount >= minAmount && amount <= maxAmount))
                            {
                                charge = chargeAmount;
                            }
                            result = 1;
                        }
                    }
                    con.Close();
                    if (result == 0)
                    {
                        charge = getchargeBillspay(AccId, amount, Curr);
                    }
                }
            }
            catch (Exception ex)
            {
                con.Close();
                throw new Exception(ex.ToString());
            }

        }
        return charge;
    }

    private Decimal getchargeBillspay(String AccId, Decimal amount, String Currency)
    {
        decimal result = 0;
        decimal minAmount, maxAmount, Thold, charge;
        using (MySqlConnection dcon = dbconkp.getConnection())
        {
            dcon.Open();
            try
            {
                using (command = dcon.CreateCommand())
                {
                    string query = "select b.Minimum, b.Maximum, b.ChargeCode, b.chargeamount, a.thresholdamount from kpadminpartners.billspaychargedetails b " +
                                    "inner join kpadminpartners.accountdetail a on b.ChargeCode = a.BracketTierCode " +
                                    "where a.AccountID = @AccId and a.currency = @Currency";
                    command.CommandText = query;
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Currency", Currency);
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                            maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                            Thold = Convert.ToDecimal(rdr["thresholdamount"]);
                            charge = Convert.ToDecimal(rdr["chargeamount"].ToString());
                            if ((amount > minAmount && amount < maxAmount))
                            {
                                result = charge;
                            }
                        }
                    }
                }
                dcon.Close();
            }
            catch (Exception ex)
            {
                dcon.Close();
                throw new Exception(ex.ToString());
            }
        }
        return result;
    }

    private Boolean checkTiercodeBillsPay(String AccId, Decimal amount, String Currency)
    {
        bool ret = false;
        decimal minAmount,maxAmount,Thold,charge;
            using(MySqlConnection con = dbconkp.getConnection())
            {
                con.Open();
                try
                {
                    using(command = con.CreateCommand())
                    {
                        string query = "select b.Minimum, b.Maximum, b.ChargeCode, b.chargeamount, a.thresholdamount,a.runningbalance,a.creditlimit from kpadminpartners.billspaychargedetails b " +
                                        "inner join kpadminpartners.accountdetail a on b.ChargeCode = a.BracketTierCode " +
                                        "where a.AccountID = @AccId and a.currency = @Currency";
                        command.CommandText = query;
                        command.Parameters.AddWithValue("AccId", AccId);
                        command.Parameters.AddWithValue("Currency", Currency);
                        using(MySqlDataReader rdr = command.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                                maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                                Thold = Convert.ToDecimal(rdr["thresholdamount"]);
                                charge = Convert.ToDecimal(rdr["chargeamount"].ToString());
                                if (amount <= maxAmount && (amount > minAmount && amount < maxAmount))
                                {
                                    ret = true;
                                    chargeValue = charge;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.ToString());
                }
            }
        return ret;
    }

    public Boolean checkCreditLimit(String AccId, Decimal Amount, String Currency)
    {
        int credLimstat;
        decimal credLim;
        bool ret = false;
        using(MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    string query = "Select creditlimit,creditactivation from kpadminpartners.accountdetail where AccountID = @AccId and currency = @Currency";
                    command.CommandText = query;
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Currency", Currency);
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            credLim = Convert.ToDecimal(rdr["creditlimit"].ToString());
                            credLimstat = Convert.ToInt32(rdr["creditactivation"]);
                            if (credLimstat == 1 && Amount <= credLim)
                            {
                                ret = true;
                            }

                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }

        return ret;
    }

    

    public void ConnectPartners()
    {
        try
        {
            //string path = httpcontext.current.server.mappath("boskpws.ini");
            IniFile ini = new IniFile(pathdomestic);

            String Serv = ini.IniReadValue("DBConfig Partner", "Server");
            String DB = ini.IniReadValue("DBConfig Partner", "Database"); ;
            String UID = ini.IniReadValue("DBConfig Partner", "UID"); ;
            String Password = ini.IniReadValue("DBConfig Partner", "Password"); ;
          // String pool = ini.IniReadValue("DBConfig Partner", "Pool");
            Int32 maxcon = Convert.ToInt32(ini.IniReadValue("DBConfig Partner", "MaxCon"));
            Int32 mincon = Convert.ToInt32(ini.IniReadValue("DBConfig Partner", "MinCon"));
            Int32 tout = Convert.ToInt32(ini.IniReadValue("DBConfig Partner", "Tout"));
            dbconkp = new DBConnection(Serv, DB, UID, Password, "", maxcon, mincon, tout);


        }
        catch (Exception ex)
        {

            throw new Exception(ex.ToString());
        }
    }
   

    public void insertData(String AccountId,String batchnum,Int32 numtrxn,String Bnumber)
    {
        string fromserverdate;
        using (MySqlConnection con = dbconkp.getConnection())
        {

            try
            {
                con.Open();

                using (command = con.CreateCommand())
                {
                    String query = "Select NOW() as serverdt";
                    command.CommandText = query;
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        rdr.Read();
                        fromserverdate = rdr["serverdt"].ToString();
                        rdr.Close();
                        using (command = con.CreateCommand())
                        {


                            String sql = "insert into kppartners.souploadheader (BatchNo,AccountCode,NoOfTrxn,UploadDate,CorporateBatchNo) values (@batchnum,@AccountId,@numtrxn,@Date,@Bnumber)";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("batchnum", batchnum);
                            command.Parameters.AddWithValue("AccountId", AccountId);
                            command.Parameters.AddWithValue("numtrxn", numtrxn);
                            command.Parameters.AddWithValue("Date",Convert.ToDateTime(fromserverdate));
                            command.Parameters.AddWithValue("Bnumber", Bnumber);
                            command.ExecuteNonQuery();
                        }

                        con.Close();

                       

                        

                    }
                }
            
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }
    }

    public String generateBatchNo()
    {
        string batchNum;
        string tempNum = "";
        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                int btyear = 0;
                int btmonth = 0;
                int ctr;
                DateTime bn = DateTime.Now;
                int year = bn.Year;
                string month = bn.ToString("MM");

                
                con.Open();
                using (command = con.CreateCommand())
                {
                    String query = "Select BatchNo from kppartners.souploadheader ORDER BY UploadDate DESC LIMIT 1";
                    command.CommandText = query;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                     
                        if (reader.Read())
                        {
                           string bnum = reader["BatchNo"].ToString();
                           
                           reader.Close();
                           String storedMonth = "call kppartners.selectMaxMonth";
                            command.CommandText = storedMonth;
                            using (MySqlDataReader read = command.ExecuteReader())
                            {
                                if (read.Read())
                                {
                                    btmonth = Convert.ToInt32(read["month"].ToString());
                                }
                                read.Close();
                            }
                            
                            String storedYear = "call kppartners.selectMaxYear";
                            command.CommandText = storedYear;

                            using (MySqlDataReader rd = command.ExecuteReader())
                            {
                                if (rd.Read())
                                {
                                    btyear = Convert.ToInt32(rd["year"].ToString());
                                }

                                rd.Close();
                            }
                            

                            if ((btmonth < Convert.ToInt32(month) && btyear == year) || (btyear < year))
                            {
                                tempNum = "00001";
                            }
                            else
                            {


                                ctr = Convert.ToInt32(bnum.Substring(6, 5));
                                ctr = ctr + 1;

                                if (Convert.ToString(ctr).Length == 1)
                                {
                                    tempNum = "0000" + Convert.ToString(ctr);
                                }
                                else if (Convert.ToString(ctr).Length == 2)
                                {
                                    tempNum = "000" + Convert.ToString(ctr);
                                }
                                else if (Convert.ToString(ctr).Length == 3)
                                {
                                    tempNum = "00" + Convert.ToString(ctr);
                                }
                                else if (Convert.ToString(ctr).Length == 4)
                                {
                                    tempNum = "0" + Convert.ToString(ctr);
                                }
                                else if (Convert.ToString(ctr).Length == 5)
                                {
                                    tempNum = Convert.ToString(ctr);
                                }
                            }
                           batchNum = Convert.ToString(year) + month + tempNum;                                              
                        }
                        else
                        {
                            batchNum = Convert.ToString(year) + month + "00001";
                        }
                    }

                }

                con.Close();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }
        return batchNum;
    }

    public Int32 checkreference(String refno,String AccountCode)
    {
      
        int flag;


                String query = "Select ReferenceNo from kppartners.sotxnref where ReferenceNo = @refnumber and AccountCode = @AccountCode";
                command.CommandText = query;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("refnumber", refno);
                command.Parameters.AddWithValue("AccountCode", AccountCode);
                using( MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        flag = 1;
                    }
                    else
                    {
                        flag = 0;
                    }
                }
          
        
        return flag;
    }


    public void Insertsotxnref(String refno, String AccountCode, String BatchNo, String Currency)
    {
        MySqlConnection dcon = dbconkp.getConnection();
       
            dcon.Open();
            DateTime dt = getServerDatePartner(true);
            dcon.Close();
        
        string day = dt.ToString("dd");
        string m = dt.ToString("MM");
        string tableRef = "sendout" + m + day;

        string TDate = dt.ToString("yyyy-MM-dd hh:mm:ss");
        using (MySqlConnection connection = dbconkp.getConnection())
        {
            try
            {
                connection.Open();
                using (command = connection.CreateCommand())
                {
                    try
                    {
                        String sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency) values (@xrefno,@xtableRef,@xAccountCode,@xBatchNo,@xTDate,@xCurr) ";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("xrefno", refno);
                        command.Parameters.AddWithValue("xtableref", tableRef);
                        command.Parameters.AddWithValue("xAccountCode", AccountCode);
                        command.Parameters.AddWithValue("xBatchNo", BatchNo);
                        command.Parameters.AddWithValue("xTDate", TDate);
                        command.Parameters.AddWithValue("xCurr", Currency);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.ToString());
                    }


                }
                connection.Close();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }
    }

    public String SaveSendOutTransDBP(String Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID,String Currency)
    {
        // String[] Data,
        //String[] Data = { "00001| |2011-08-09 10:00:00|TXNB09-0016|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg", "00002| |2011-08-09 10:00:00|TXNB09-0011|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg" };
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        String sessionId = manager.CreateSessionID(Context);
        Int32 result = 0;
        String Return = "";
        int i = 1;
        //string batchnum = generateBatchNo();

        ConnectPartners();

        String ControlNumber;

        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = getServerDatePartner(true).ToString("MM-dd").Replace("-", "");

                    String atay = String.Empty;
                    //String benefln;
                    //String beneffn;
                    //int r = 2;
                    String peste = String.Empty;
                 
                        String[] datasplit = Data.Split('|');

                        string txtref = datasplit[0].ToString().Trim();
                        string kptn = datasplit[1].ToString().Trim();
                        string appnum = datasplit[2].ToString().Trim();
                        string receiverfullname = datasplit[3].ToString().Trim();
                        String[] dsplit = datasplit[3].Split(' ');
                        if (dsplit.Count() == 4)
                        {
                            atay = dsplit[3].ToString(); ;
                        }
                        else
                        {
                            atay = String.Empty;
                        }
                        string benefmn = dsplit.Count() == 2 ? String.Empty : dsplit.Count() == 1 ? String.Empty : dsplit[1].ToString();
                        string benefln = dsplit.Count() == 2 ? dsplit[1].ToString() : dsplit.Count() == 1 ? String.Empty : dsplit[2].ToString() + " " + atay;
                        string beneffn = dsplit[0].ToString();
                        string benefadr1 = datasplit[6].ToString().Trim();
                        string benefadr2 = datasplit[7].ToString().Trim();
                        string benefcntactlndline = datasplit[8].ToString().Trim();
                        string benefmobilenum = datasplit[9].ToString().Trim();
                        string pamnt = datasplit[10].ToString().Trim();
                        string amt = pamnt;
                        string senderfullname = datasplit[11].ToString().Trim();
                        String[] dsplit1 = datasplit[11].Split(' ');
                        if (dsplit1.Count() == 4)
                        {
                            peste = dsplit1[3].ToString();
                        }
                        else
                        {
                            peste = String.Empty;
                        }
                        string remitrln = dsplit1.Count() == 2 ? dsplit1[1].ToString() : dsplit1.Count() == 1 ? String.Empty : dsplit1[2].ToString() + " " + peste;
                        string remitrfn = dsplit1[0].ToString();
                        string remitrmn = dsplit1.Count() == 2 ? String.Empty : dsplit1.Count() == 1 ? String.Empty : dsplit1[1].ToString();
                        string msg = datasplit[14].ToString().Trim();
                        string OtherDetails = appnum + "," + benefadr1 + "," + benefadr2;
                        String  irnum = "";
                        String curr = Currency;
                        Decimal chrge = chargeValue;
                        String othrchrge = "";
                        String total = "";
                        String cancelleddate = "";
                        String cancelledbyoperationid = "";
                        String cancelledbybranchcode = "";
                        String cancelledbyzonecode = "";
                        String cancelledbystationid = "";
                        String cancelreason = "";
                        String canceldetails = "";
                        String receiveraddress = "";
                        String receivergender = "";
                        String receiverbdate = "";
                        String cancelcharge = "";
                        String chrgeto = "";
                        String forex = "";
                        String traceno = "";
                        String senderaddress = "";
                        String sendergender = "";
                        String sendercontactnum = "";
                        String sessionid = "";
                        String receiverstreet = "";
                        String receiverprovince = "";
                        String receivercountry = "";
                        String redeem = "";
                        String senderbdate = "";
                        int ret = checkreference(txtref,AccountId);
                        int zc = Convert.ToInt32(zonecode);
                        ControlNumber = generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                        if (ret == 0)
                        {

                            MySqlCommand command1 = con.CreateCommand();
                            command1.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, Principal," +
                                                "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                  "ReceiverName," +
                                                  "OtherDetails," +
                                                  "StationID, KPTN, Message, BranchCode, ReceiverContactNo, Transdate,IRNo,Currency, OperatorID," +
                                                  "Charge, OtherCharge, Total, CancelledDate, AccountCode, CancelledByOperatorID," +
                                                  "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                                                  "ReceiverAddress, ReceiverGender, ReceiverBirthDate, CancelCharge,ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, " +
                                                  "ReceiverProvince, ReceiverCountry, Redeem, SenderBirthdate) " +
                                                  "values(@ControlNo, @ReferenceNO, @Principal, @SenderFName, @SenderLName," +
                                                  " @SenderMName, @SenderName, @ReceiverFName, @ReceiverLName, @ReceiverMName, @ReceiverName," +
                                                  "@OtherDetails, @StationID, @KPTN, @Message, @Branchcode, @ReceiverContactNo, @txndate, @IRNo, @Currency," +
                                                  "@OperatorID, @Charge, @OtherCharge, @Total, @CancelledDate, @AccountCode, @CancelledbyOperatorID, "+
                                                  "@CancelledbyBranchcode, @CancelledbyZonecode, @CancelledbyStationID, @CancelReason, @CancelDetails, " +
                                                  "@ReceiverAddress, @ReceiverGender, @ReceiverBirthdate, @CancelCharge, @ChargeTo, @Forex, " +
                                                  "@TraceNo, @SenderAddress, @SenderGender, @SenderContactNo, @SessionID, @ReceiverProvince, " +
                                                  "@ReceiverCountry, @Redeem, @SenderBirthdate);";
                            command1.Parameters.Clear();
                            command1.Parameters.AddWithValue("IRNo", irnum);
                            command1.Parameters.AddWithValue("Currency", curr);
                            command1.Parameters.AddWithValue("Charge", chrge);
                            command1.Parameters.AddWithValue("OtherCharge", othrchrge);
                            command1.Parameters.AddWithValue("Total", total);
                            command1.Parameters.AddWithValue("CancelledDate", cancelleddate);
                            command1.Parameters.AddWithValue("CancelledbyOperatorID", cancelledbyoperationid);
                            command1.Parameters.AddWithValue("CancelledbyBranchcode", cancelledbybranchcode);
                            command1.Parameters.AddWithValue("CancelledbyZonecode", cancelledbyzonecode);
                            command1.Parameters.AddWithValue("CancelledbyStationID", cancelledbystationid);
                            command1.Parameters.AddWithValue("CancelReason", cancelreason);
                            command1.Parameters.AddWithValue("CancelDetails", canceldetails);
                            command1.Parameters.AddWithValue("ReceiverAddress", receiveraddress);
                            command1.Parameters.AddWithValue("ReceiverGender", receivergender);
                            command1.Parameters.AddWithValue("ReceiverBirthdate", receiverbdate);
                            command1.Parameters.AddWithValue("CancelCharge", cancelcharge);
                            command1.Parameters.AddWithValue("ChargeTo", chrgeto);
                            command1.Parameters.AddWithValue("Forex", forex);
                            command1.Parameters.AddWithValue("TraceNo", traceno);
                            command1.Parameters.AddWithValue("SenderAddress", senderaddress);
                            command1.Parameters.AddWithValue("SenderGender", sendergender);
                            command1.Parameters.AddWithValue("SenderContactNo", sendercontactnum);
                            command1.Parameters.AddWithValue("SessionID", sessionid);
                            command1.Parameters.AddWithValue("ReceiverStreet", receiverstreet);
                            command1.Parameters.AddWithValue("ReceiverProvince", receiverprovince);
                            command1.Parameters.AddWithValue("ReceiverCountry", receivercountry);
                            command1.Parameters.AddWithValue("Redeem", redeem);
                            command1.Parameters.AddWithValue("SenderBirthdate", senderbdate);
                            command1.Parameters.AddWithValue("OperatorID", operatorID);
                            command1.Parameters.AddWithValue("ControlNo",ControlNumber);
                            command1.Parameters.AddWithValue("ReferenceNO",txtref);
                            command1.Parameters.AddWithValue("Principal", amt == String.Empty ? 0 : Convert.ToDecimal(amt));
                            command1.Parameters.AddWithValue("AccountCode",AccountId);
                            command1.Parameters.AddWithValue("SenderFName",remitrfn);
                            command1.Parameters.AddWithValue("SenderLName",remitrln);
                            command1.Parameters.AddWithValue("SenderMName",remitrmn);
                            command1.Parameters.AddWithValue("SenderName", senderfullname);
                            command1.Parameters.AddWithValue("ReceiverFName",beneffn);
                            command1.Parameters.AddWithValue("ReceiverLName",benefln);
                            command1.Parameters.AddWithValue("ReceiverMName",benefmn);
                            command1.Parameters.AddWithValue("ReceiverName",receiverfullname);
                            command1.Parameters.AddWithValue("ReceiverContactNo",benefcntactlndline + "/" + benefmobilenum);
                            command1.Parameters.AddWithValue("OtherDetails",OtherDetails);
                            command1.Parameters.AddWithValue("StationID",stationID);
                            command1.Parameters.AddWithValue("KPTN",kptn);
                            command1.Parameters.AddWithValue("Message", msg);
                            command1.Parameters.AddWithValue("Branchcode",bcode);
                            command1.Parameters.AddWithValue("txndate", DateTime.Now);
                            result = command1.ExecuteNonQuery();

                            command1.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno)"+
                                " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo);";
                            command1.Parameters.Clear();
                            command1.Parameters.AddWithValue("kptnnum", kptn);
                            command1.Parameters.AddWithValue("refnum", txtref);
                            command1.Parameters.AddWithValue("action", "SENDOUT");
                            command1.Parameters.AddWithValue("bcode", bcode);
                            command1.Parameters.AddWithValue("opID", operatorID);
                            command1.Parameters.AddWithValue("scode", stationcode);
                            command1.Parameters.AddWithValue("sID", stationID);
                            command1.Parameters.AddWithValue("zCode", zonecode);
                            command1.Parameters.AddWithValue("txndate", DateTime.Now);
                            //9command.Parameters.AddWithValue("bname", bname);
                            command1.Parameters.AddWithValue("type", "kppartners");
                            command1.Parameters.AddWithValue("AccountCode", AccountId);
                            command1.Parameters.AddWithValue("Currency", Currency);
                            command.Parameters.AddWithValue("SenderName", senderfullname);
                            command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                            command.Parameters.AddWithValue("Principal", amt);
                            command.Parameters.AddWithValue("ControlNo", ControlNumber);
                            result = command1.ExecuteNonQuery();
                            decimal Total;
                            decimal OtherCharge;
                            decimal frex;
                            int stID = Convert.ToInt16(stationID);
                            int zcode = Convert.ToInt16(zonecode);
                            decimal Principal = Convert.ToDecimal(amt);
                            
                           if(total == "")
                            {
                                Total = 0;
                            }
                            else
                            {
                                Total = Convert.ToDecimal(total);
                            }

                            if(othrchrge == "")
                            {
                                OtherCharge = 0;
                            }
                            else
                            {
                              OtherCharge = Convert.ToDecimal(othrchrge);
                            }

                            if(forex == "")
                            {
                                frex = 0;
                            }
                            else
                            {
                             frex = Convert.ToDecimal(forex); 
                            }


                            result = InsertCorporateTrans(ControlNumber, kptn, "", txtref, AccountId, curr, DateTime.Now, stID, 0, operatorID, bcode, zcode, "", "", 0, Principal, chrgeto, chrge, OtherCharge, Total, frex, traceno, sessionid, "", "", "", beneffn, benefln, benefmn, 1);


                            if (result.Equals("-1"))
                            {

                                con.Close();
                                return Return = "3";
                            }
                            

                            i = i + 1;
                        }
                        else
                        {
                           
                            con.Close();
                            return Return = "2";
                        }



                  
                    con.Close();
                    Return = "1";

                }
            }
            catch (Exception ex)
            {
               
                con.Close();
                return ex.Message;
            }


        }


        return Return;
    }



    public String SaveSendOutTransBDO(String Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID,String Currency)
    {
        // String[] Data,
        //String[] Data = { "00001| |2011-08-09 10:00:00|TXNB09-0016|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg", "00002| |2011-08-09 10:00:00|TXNB09-0011|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg" };
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        String sessionId = manager.CreateSessionID(Context);
        Int32 result = 0;
        String Return = "";
        int i = 1;
        //string batchnum = generateBatchNo();

        ConnectPartners();

        String ControlNumber;

        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = getServerDatePartner(true).ToString("MM-dd").Replace("-", "");

                    String atay = String.Empty;
                    String benefln;
                    String beneffn;
                    String benefmn;
                    //int r = 2;
                    String peste = String.Empty;

                    String[] datasplit = Data.Split('|');

                        String txtref = datasplit[0].ToString();
                        String kptn = datasplit[1].ToString();
                        String transtype = datasplit[2].ToString();
                        String benefullname = datasplit[3].ToString();
                        if (benefullname != "")
                        {
                            String[] beneNamesplit = benefullname.Split(' ');
                            benefln = beneNamesplit[0].ToString();
                            beneffn = beneNamesplit[1].ToString();
                            benefmn = beneNamesplit[2].ToString();
                        }

                        else 
                        {
                            benefln = "";
                            beneffn = "";
                            benefmn = "";
                        }

                        String prodname = datasplit[4].ToString();
                        String principal = datasplit[5].ToString();
                        String currency = Currency; //datasplit[6].ToString();
                        String bankbranch = datasplit[7].ToString();
                        String usrid = datasplit[8].ToString();
                        String status = datasplit[9].ToString();
                        String docid = datasplit[10].ToString();
                        String doctype = datasplit[11].ToString();
                        String docissuedate = datasplit[12].ToString();
                        String docexpirydate = datasplit[13].ToString();
                        String remarks = datasplit[14].ToString();
                        String date = datasplit[15].ToString();
                        String time = datasplit[16].ToString();
                        String accountno = datasplit[17].ToString();
                        String conduit = datasplit[18].ToString();



                        //string txtref = datasplit[0].ToString();
                        //string kptn = datasplit[1].ToString();
                        //string appnum = datasplit[2].ToString();
                        //string receiverfullname = datasplit[3].ToString();
                        //String[] dsplit = datasplit[3].Split(' ');
                        //if (dsplit.Count() == 4)
                        //{
                        //    atay = dsplit[3].ToString(); ;
                        //}
                        //else
                        //{
                        //    atay = String.Empty;
                        //}
                        //string benefmn = dsplit.Count() == 2 ? String.Empty : dsplit.Count() == 1 ? String.Empty : dsplit[1].ToString();
                        //string benefln = dsplit.Count() == 2 ? dsplit[1].ToString() : dsplit.Count() == 1 ? String.Empty : dsplit[2].ToString() + " " + atay;
                        //string beneffn = dsplit[0].ToString();
                        //string benefadr1 = datasplit[6].ToString();
                        //string benefadr2 = datasplit[7].ToString();
                        //string benefcntactlndline = datasplit[8].ToString();
                        //string benefmobilenum = datasplit[9].ToString();
                        //string pamnt = datasplit[10].ToString();
                        //string amt = pamnt;
                        //string senderfullname = datasplit[11].ToString();
                        //String[] dsplit1 = datasplit[11].Split(' ');
                        //if (dsplit1.Count() == 4)
                        //{
                        //    peste = dsplit1[3].ToString();
                        //}
                        //else
                        //{
                        //    peste = String.Empty;
                        //}
                        //string remitrln = dsplit1.Count() == 2 ? dsplit1[1].ToString() : dsplit1.Count() == 1 ? String.Empty : dsplit1[2].ToString() + " " + peste;
                        //string remitrfn = dsplit1[0].ToString();
                        //string remitrmn = dsplit1.Count() == 2 ? String.Empty : dsplit1.Count() == 1 ? String.Empty : dsplit1[1].ToString();
                        //string msg = datasplit[14].ToString();
                        //string OtherDetails = appnum + "," + benefadr1 + "," + benefadr2;
                        //prodname + "," +	bankbranch + "," + usrid + "," + status + "," +	docid + "," + doctype + "," + docissuedate + "," +	docexpirydate + "," + remarks + "," + date + "," +	time + "," + transtype
                        String OtherDetails = prodname + "," + bankbranch + "," + usrid + "," + status + "," + docid + "," + doctype + "," + docissuedate + "," + docexpirydate + "," + remarks + "," + date + "," + time + "," + transtype;
                        String irnum = "";
                        //String curr = "";
                        String msg = "";
                        decimal chrge = chargeValue;
                        String othrchrge = "";
                        String total = "";
                        String cancelleddate = "";
                        String cancelledbyoperationid = "";
                        String cancelledbybranchcode = "";
                        String cancelledbyzonecode = "";
                        String cancelledbystationid = "";
                        String cancelreason = "";
                        String canceldetails = "";
                        String receiveraddress = "";
                        String receivergender = "";
                        String receiverbdate = "";
                        String cancelcharge = "";
                        String chrgeto = "";
                        String forex = "";
                        String traceno = "";
                        String senderaddress = "";
                        String sendergender = "";
                        String sendercontactnum = "";
                        String sessionid = "";
                        String receiverstreet = "";
                        String receiverprovince = "";
                        String receivercountry = "";
                        String redeem = "";
                        String senderbdate = "";
                       
                

                        int ret = checkreference(txtref,AccountId);
                        int zc = Convert.ToInt32(zonecode);
                        ControlNumber = generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                           
                        

                        if (ret == 0)
                        {
                            command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno)"+
                                " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo);";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("kptnnum", kptn);
                            command.Parameters.AddWithValue("refnum", txtref);
                            command.Parameters.AddWithValue("action", "SENDOUT");
                            command.Parameters.AddWithValue("bcode", bcode);
                            command.Parameters.AddWithValue("opID", operatorID);
                            command.Parameters.AddWithValue("scode", stationcode);
                            command.Parameters.AddWithValue("sID", stationID);
                            command.Parameters.AddWithValue("zCode", zonecode);
                            command.Parameters.AddWithValue("txndate", DateTime.Now);
                            //commad.Parameters.AddWithValue("bname", bname);
                            command.Parameters.AddWithValue("type", "kppartners");
                            command.Parameters.AddWithValue("AccountCode", AccountId);
                            command.Parameters.AddWithValue("Currency", Currency);
                            command.Parameters.AddWithValue("SenderName", "");
                            command.Parameters.AddWithValue("ReceiverName", benefullname);
                            command.Parameters.AddWithValue("Principal", principal);
                            command.Parameters.AddWithValue("ControlNo", ControlNumber);

                            result = command.ExecuteNonQuery();

                           // MySqlCommand command1 = con.CreateCommand();
                            //command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, Principal," +
                            //                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                            //                      "ReceiverName," +
                            //                      "OtherDetails," +
                            //                      "StationID, KPTN, Message, BranchCode, ReceiverContactNo, Transdate,IRNo,Currency, OperatorID," +
                            //                      "Charge, OtherCharge, Total, CancelledDate, AccountCode, CancelledByOperatorID," +
                            //                      "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                            //                      "ReceiverAddress, ReceiverGender, ReceiverBirthDate, CancelCharge,ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, " +
                            //                      "ReceiverProvince, ReceiverCountry, Redeem, SenderBirthdate) " +
                            //                      "values(@ControlNo, @ReferenceNO, @Principal, @SenderFName, @SenderLName, " +
                            //                      "@SenderMName, @SenderName, @ReceiverFName, @ReceiverLName, @ReceiverMName, @ReceiverName, " +
                            //                      "@OtherDetails, @StationID, @KPTN, @Message, @Branchcode, @ReceiverContactNo, @txndate1, @IRNo, @Currency, " +
                            //                      "@OperatorID, @Charge, @OtherCharge, @Total, @CancelledDate, @AccountId, @CancelledbyOperatorID, " +
                            //                      "@CancelledbyBranchcode, @CancelledbyZonecode, @CancelledbyStationID, @CancelReason, @CancelDetails, " +
                            //                      "@ReceiverAddress, @ReceiverGender, @ReceiverBirthdate, @CancelCharge, @ChargeTo, @Forex, " +
                            //                      "@TraceNo, @SenderAddress, @SenderGender, @SenderContactNo, @SessionID, @ReceiverProvince, " +
                            //                      "@ReceiverCountry, @Redeem, @SenderBirthdate);";
                            command.Parameters.Clear();
                            command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, IRNo,Currency, Principal, " +
                                                  "Charge, OtherCharge, Total, CancelledDate, AccountCode, TransDate, CancelledByOperatorID, " +
                                                  "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                                                  "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                  "ReceiverName, ReceiverAddress, ReceiverGender, ReceiverContactNo, ReceiverBirthDate, CancelCharge, " +
                                                  "ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, OtherDetails, OperatorID, " +
                                                  "StationID, ReceiverStreet, ReceiverProvince, ReceiverCountry, KPTN,Message, Redeem, BranchCode, SenderBirthdate) " +
                                                  "values(@ControlNo, @ReferenceNO, @IRNo, @Currency, @Principal, @Charge, " +
                                                  "@OtherCharge, @Total, @CancelledDate, @AccountCode, @txndate1, @CancelledbyOperatorID, " +
                                                  "@CancelledbyBranchcode, @CancelledbyZonecode, @CancelledbyStationID, @CancelReason, " +
                                                  "@CancelDetails, @SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                  "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverAddress, @ReceiverGender, @ReceiverContactNo, " +
                                                  "@ReceiverBirthdate, @CancelCharge, @ChargeTo, @Forex, @TraceNo, @SenderAddress, @SenderGender, @SenderContactNo, " +
                                                  "@SessionID, @OtherDetails, @OperatorId, @StationID, @ReceiverStreet, @ReceiverProvince, @ReceiverCountry,@KPTN, @Message, @Redeem, @Branchcode, @SenderBdate);";

                            command.Parameters.AddWithValue("IRNo", irnum); //empty
                            command.Parameters.AddWithValue("Currency", currency);
                            command.Parameters.AddWithValue("Charge", chrge);//empty
                            command.Parameters.AddWithValue("OtherCharge", othrchrge);//empty
                            command.Parameters.AddWithValue("Total", total);//empty
                            command.Parameters.AddWithValue("CancelledDate", cancelleddate);//empty
                            command.Parameters.AddWithValue("CancelledbyOperatorID", cancelledbyoperationid);//empty
                            command.Parameters.AddWithValue("CancelledbyBranchcode", cancelledbybranchcode);//empty
                            command.Parameters.AddWithValue("CancelledbyZonecode", cancelledbyzonecode);//empty
                            command.Parameters.AddWithValue("CancelledbyStationID", cancelledbystationid);//empty
                            command.Parameters.AddWithValue("CancelReason", cancelreason);//empty
                            command.Parameters.AddWithValue("CancelDetails", canceldetails);//empty
                            command.Parameters.AddWithValue("ReceiverAddress", receiveraddress);//empty
                            command.Parameters.AddWithValue("ReceiverGender", receivergender);//empty
                            command.Parameters.AddWithValue("ReceiverBirthdate", receiverbdate);//empty
                            command.Parameters.AddWithValue("CancelCharge", cancelcharge);//empty
                            command.Parameters.AddWithValue("ChargeTo", chrgeto);//empty
                            command.Parameters.AddWithValue("Forex", forex);//empty
                            command.Parameters.AddWithValue("TraceNo", traceno);//empty
                            command.Parameters.AddWithValue("SenderAddress", senderaddress);//empty
                            command.Parameters.AddWithValue("SenderGender", sendergender);//empty
                            command.Parameters.AddWithValue("SenderContactNo", sendercontactnum);//empty
                            command.Parameters.AddWithValue("SessionID", sessionid);//empty
                            command.Parameters.AddWithValue("ReceiverStreet", receiverstreet);//empty
                            command.Parameters.AddWithValue("ReceiverProvince", receiverprovince);//empty
                            command.Parameters.AddWithValue("ReceiverCountry", receivercountry);//empty
                            command.Parameters.AddWithValue("Redeem", redeem);//empty
                            command.Parameters.AddWithValue("SenderBdate", senderbdate);//empty
                            command.Parameters.AddWithValue("OperatorID", operatorID);
                            command.Parameters.AddWithValue("ControlNo", ControlNumber);
                            command.Parameters.AddWithValue("ReferenceNO", txtref);
                            //command1.Parameters.AddWithValue("ReferenceNO", txtref);
                            //command1.Parameters.AddWithValue("Principal", amt == String.Empty ? 0 : Convert.ToDecimal(amt));
                            command.Parameters.AddWithValue("AccountCode", AccountId);
                            command.Parameters.AddWithValue("SenderFName", "");
                            command.Parameters.AddWithValue("SenderLName", "");
                            command.Parameters.AddWithValue("SenderMName", "");
                            command.Parameters.AddWithValue("SenderName", "");
                            command.Parameters.AddWithValue("ReceiverFName", beneffn);
                            command.Parameters.AddWithValue("ReceiverLName", benefln);
                            command.Parameters.AddWithValue("ReceiverMName", benefmn);
                            command.Parameters.AddWithValue("ReceiverName", benefullname);
                            command.Parameters.AddWithValue("ReceiverContactNo","");
                            command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                            command.Parameters.AddWithValue("StationID", stationID);
                            command.Parameters.AddWithValue("KPTN", kptn);
                            command.Parameters.AddWithValue("Message", msg);
                            command.Parameters.AddWithValue("Branchcode", bcode);
                            command.Parameters.AddWithValue("txndate1", DateTime.Now);
                            command.Parameters.AddWithValue("Principal",principal);
                            
                            result = command.ExecuteNonQuery();

                            
                            decimal Total;
                            decimal OtherCharge;
                            decimal frex;
                            int stID = Convert.ToInt16(stationID);
                            int zcode = Convert.ToInt16(zonecode);
                            decimal Principal = Convert.ToDecimal(principal);
                            if(total == "")
                            {
                                Total = 0;
                            }
                            else
                            {
                                Total = Convert.ToDecimal(total);
                            }

                            if(othrchrge == "")
                            {
                                OtherCharge = 0;
                            }
                            else
                            {
                              OtherCharge = Convert.ToDecimal(othrchrge);
                            }

                            if(forex == "")
                            {
                                frex = 0;
                            }
                            else
                            {
                             frex = Convert.ToDecimal(forex); 
                            }


                                                                                                                                                                                                                                 
                            result = InsertCorporateTrans(ControlNumber, kptn, "", txtref, AccountId, currency, DateTime.Now, stID, 0, operatorID, bcode, zcode, "", "", 0,Principal,chrgeto,chrge,OtherCharge,Total,frex,traceno,sessionid,"","","",beneffn,benefln,benefmn,1);

                            if (result.Equals("-1"))
                            {

                                con.Close();
                                return Return = "3";
                            }
                           

                            i = i + 1;
                        }
                        else
                        {
                          
                            con.Close();
                            return Return = "2";
                        }



                   
                   
                    con.Close();
                    Return = "1";

                }
            }
            catch (Exception ex)
            {
                
                con.Close();
                Return = ex.ToString();
            }


        }


        return Return;
    }



    public String SaveSendOutTransBPI(String Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID,String Currency)
    {
        // String[] Data,
        //String[] Data = { "00001| |2011-08-09 10:00:00|TXNB09-0016|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg", "00002| |2011-08-09 10:00:00|TXNB09-0011|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg" };
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        String sessionId = manager.CreateSessionID(Context);
        Int32 result = 0;
        String Return = "";
        int i = 1;
        //string batchnum = generateBatchNo();

        ConnectPartners();

        String ControlNumber;

        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = getServerDatePartner(true).ToString("MM-dd").Replace("-", "");



                    String[] datasplit = Data.Split('|');

                        string txnappnum = datasplit[0].ToString();
                        string kptn = datasplit[1].ToString();
                        string txtref = datasplit[2].ToString();
                        string arnum = datasplit[3].ToString();
                        string txndistdate = datasplit[4].ToString();
                        string settlement = datasplit[5].ToString();
                        //decimal principal = Convert.ToDecimal(datasplit[4].ToString());
                        //string currency = datasplit[5].ToString();
                        string remitrln = datasplit[6].ToString();
                        string remitrfn = datasplit[7].ToString();
                        string remitrmn = datasplit[8].ToString();
                        string benefln = datasplit[9].ToString();
                        string beneffn = datasplit[10].ToString();
                        string benefmn = datasplit[11].ToString();
                        string altrnaterecepient = datasplit[12].ToString();
                        string addrs1 = datasplit[13].ToString();
                        string addr2 = datasplit[14].ToString();
                        string addr3 = datasplit[15].ToString();
                        string benefcode = datasplit[16].ToString();
                        string benefphonenum = datasplit[17].ToString();
                        string benefmobilenum = datasplit[18].ToString();
                        string benefofficenum = datasplit[19].ToString();
                        string beneflandmrk = datasplit[20].ToString();
                        string msgtorecepient = datasplit[21].ToString();
                        string benefcurr = Currency; //datasplit[22].ToString();
                        string benefnetproceeds = datasplit[23].ToString();
                        //string receivermname = datasplit[24].ToString();
                        //string receiverstreet = datasplit[25].ToString();
                        //string receiversprovince = datasplit[26].ToString();
                        //string receiverscountry = datasplit[27].ToString();
                        //string receiverbdate = datasplit[28].ToString();
                        //string receivergender = datasplit[29].ToString();
                        //string receiverphonenum = datasplit[30].ToString();
                        //string msg = datasplit[31].ToString();
                        string senderfullname = remitrfn + " " + remitrmn + " " + remitrln;
                        string receiverfullname = beneffn + " " + benefmn + " " + benefln;
                        string OtherDetails = txnappnum + "|" + kptn + "|" + txndistdate;
                        String IRNumber = "";
                        Decimal OtherCharge = 0;
                        Decimal Total = 0;
                        String CancelledDate = "";
                        String AccountCode = AccountId;
                        String CancelledByOperatorID = "";
                        String CancelledByBranchCode = "";
                        String CancelledByZoneCode = "";
                        String CancelledByStationID = "";
                        String CancelReason = "";
                        String CancelDetails = "";
                        String ReceiverAddress = "";
                        Decimal CancelCharge = 0;
                        String ChargeTo = "";
                        Decimal Forex = 0;
                        String TraceNumber = "";
                        String SenderAddress = "";
                        String OperatorID = operatorID;
                        String StationID = stationID;
                        decimal Redeem = 0;
                        String receiverprovince = "";
                        String BranchCode = bcode;
                        decimal charge = chargeValue;
                        int gndr = 3;
                        String benefdate = "";
                        String senderphone = "";
                        String rcvrstrt = "";
                        String rcvrscountry = "";
                        String senderbdate = "";
                        int ret = checkreference(txtref,AccountId);
                        int zc = Convert.ToInt32(zonecode);
                        ControlNumber = generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                        if (ret == 0)
                        {
                            command.Parameters.Clear();
                            command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, IRNo,Currency, Principal," +
                                                  "Charge, OtherCharge, Total, CancelledDate, AccountCode, TransDate, CancelledByOperatorID," +
                                                  "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                                                  "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                  "ReceiverName, ReceiverAddress, ReceiverGender, ReceiverContactNo, ReceiverBirthDate, CancelCharge, " +
                                                  "ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, OtherDetails, OperatorID," +
                                                  "StationID, ReceiverStreet, ReceiverProvince, ReceiverCountry, KPTN,Message, Redeem, BranchCode, SenderBirthdate)" +
                                                  "values(@ControlNo, @RefNumber, @IRnum, @Currency, @Principal, @Charge, " +
                                                  "@OtherCharge, @Total, @CancelledDate, @AccountCode, @txndate, @CancelledByOperatorID, " +
                                                  "@CancelledByBranchCode, @CancelledByZoneCode, @CancelledByStationID, @CancelReason, " +
                                                  "@CancelDetails, @SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                  "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverAddress, @ReceiverGender, @ReceiverContactNo, " +
                                                  "@ReceiverBDate, @CancelCharge, @ChargeTo, @Forex, @TraceNo, @SenderAddress, @SenderGender, @SenderContactNo, " +
                                                  "@sessionID, @OtherDetails, @OperatorId, @StationID, @ReceiverStreet, @ReceiverProvince, @ReceiverCountry,@KPTN, @Message, @Redeem, @BranchCode, @SenderBdate);";
                            
                            command.Parameters.AddWithValue("ControlNo", ControlNumber);
                            command.Parameters.AddWithValue("KPTN", kptn);
                            command.Parameters.AddWithValue("RefNumber", txtref);
                            command.Parameters.AddWithValue("IRnum", IRNumber);
                            command.Parameters.AddWithValue("Currency", benefcurr);
                            command.Parameters.AddWithValue("Principal", benefnetproceeds);
                            command.Parameters.AddWithValue("Charge", charge);
                            command.Parameters.AddWithValue("OtherCharge", OtherCharge);
                            command.Parameters.AddWithValue("Total", Total);
                            command.Parameters.AddWithValue("CancelledDate", CancelledDate);
                            command.Parameters.AddWithValue("AccountCode", AccountCode);
                            command.Parameters.AddWithValue("CancelledByOperatorID", CancelledByOperatorID);
                            command.Parameters.AddWithValue("CancelledByBranchCode", CancelledByBranchCode);
                            command.Parameters.AddWithValue("CancelledByZoneCode", CancelledByZoneCode);
                            command.Parameters.AddWithValue("CancelledByStationID", CancelledByStationID);
                            command.Parameters.AddWithValue("CancelReason", CancelReason);
                            command.Parameters.AddWithValue("CancelDetails", CancelDetails);
                            command.Parameters.AddWithValue("SenderFName", remitrfn);
                            command.Parameters.AddWithValue("SenderLName", remitrln);
                            command.Parameters.AddWithValue("SenderMName", remitrmn);
                            command.Parameters.AddWithValue("SenderName", senderfullname);
                            command.Parameters.AddWithValue("ReceiverFName", beneffn);
                            command.Parameters.AddWithValue("ReceiverLName", benefln);
                            command.Parameters.AddWithValue("ReceiverMName", benefmn);
                            command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                            command.Parameters.AddWithValue("ReceiverAddress", ReceiverAddress);
                            command.Parameters.AddWithValue("ReceiverGender", gndr);
                            command.Parameters.AddWithValue("ReceiverContactNo", benefphonenum);
                            command.Parameters.AddWithValue("ReceiverBDate", benefdate);
                            command.Parameters.AddWithValue("CancelCharge", CancelCharge);
                            command.Parameters.AddWithValue("ChargeTo", ChargeTo);
                            command.Parameters.AddWithValue("Forex", Forex);
                            command.Parameters.AddWithValue("TraceNo", TraceNumber);
                            command.Parameters.AddWithValue("SenderAddress", SenderAddress);
                            command.Parameters.AddWithValue("SenderGender", gndr);
                            command.Parameters.AddWithValue("SenderContactNo", senderphone);
                            command.Parameters.AddWithValue("sessionId", sessionId);
                            command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                            command.Parameters.AddWithValue("OperatorId", OperatorID);
                            command.Parameters.AddWithValue("StationID", StationID);
                            command.Parameters.AddWithValue("ReceiverStreet", rcvrstrt);
                            command.Parameters.AddWithValue("ReceiverProvince", receiverprovince);
                            command.Parameters.AddWithValue("ReceiverCountry", rcvrscountry);
                            command.Parameters.AddWithValue("Message", msgtorecepient);
                            command.Parameters.AddWithValue("Redeem", Redeem);
                            command.Parameters.AddWithValue("BranchCode", BranchCode);
                            command.Parameters.AddWithValue("SenderBdate", senderbdate);
                            command.Parameters.AddWithValue("txndate", DateTime.Now);
                            result = command.ExecuteNonQuery();
                            
                            command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno)"+
                                " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo);";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("kptnnum", kptn);
                            command.Parameters.AddWithValue("refnum", txtref);
                            command.Parameters.AddWithValue("action", "SENDOUT");
                            command.Parameters.AddWithValue("bcode", bcode);
                            command.Parameters.AddWithValue("opID", operatorID);
                            command.Parameters.AddWithValue("scode", stationcode);
                            command.Parameters.AddWithValue("sID", stationID);
                            command.Parameters.AddWithValue("zCode", zonecode);
                            command.Parameters.AddWithValue("txndate", DateTime.Now);
                            //9command.Parameters.AddWithValue("bname", bname);
                            command.Parameters.AddWithValue("type", "kppartners");
                            command.Parameters.AddWithValue("AccountCode", AccountCode);
                            command.Parameters.AddWithValue("Currency", Currency);
                            command.Parameters.AddWithValue("SenderName", senderfullname);
                            command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                            command.Parameters.AddWithValue("Principal", benefnetproceeds);
                            command.Parameters.AddWithValue("ControlNo", ControlNumber);
                            result = command.ExecuteNonQuery();
                            int stID = Convert.ToInt16(stationID);
                            int zcode = Convert.ToInt16(zonecode);
                            decimal principal = Convert.ToDecimal(benefnetproceeds);
                                                                                                                                
                            result = InsertCorporateTrans(ControlNumber, kptn, "", txtref, AccountCode, benefcurr, DateTime.Now, stID, 0, OperatorID, BranchCode, zcode, "", "", 0,principal,ChargeTo,charge,OtherCharge,Total,Forex,TraceNumber,sessionId,remitrfn, remitrln, remitrmn, beneffn, benefln, benefmn, 1); 

                            if (result.Equals("-1"))
                            {

                                con.Close();
                                return Return = "3";
                            }
                           
                                i = i + 1;
                        }
                        else
                        {
                            
                            con.Close();
                            return Return = "2";
                        }



                   
                    con.Close();
                    Return = "1";

                }
            }
            catch (Exception ex)
            {
                
                con.Close();
                return ex.Message;
            }


        }


        return Return;
    }

    public String SaveSendOutTrans(String Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode , String operatorID , String stationID,String Currency)
    {
        // String[] Data,
        //String[] Data = { "00001| |2011-08-09 10:00:00|TXNB09-0016|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg", "00002| |2011-08-09 10:00:00|TXNB09-0011|000000100.00|PHP| |SALARY|FRIEND|ELECTRIC BILL|DELA CRUZ|JUAN|MARIO|SCHOOLID|UC#1234567890|2011-12-31| |1|CAWAYAN ST.|CEBU|PHILIPPINES| |MONICO|JOSE|ANTONIO|PALOMA ST.|CEBU|PHILIPPINES| |1| |msg" };
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        String sessionId = manager.CreateSessionID(Context);
        Int32 result = 0;
        String Return = "";
        int i = 1;
        //string batchnum = generateBatchNo();
      
            ConnectPartners();

            String ControlNumber;
           
                using (MySqlConnection con = dbconkp.getConnection())
                {
                  try 
                   {
                    con.Open();
                    
                    using (command = con.CreateCommand())
                    {
                        String pdate = getServerDatePartner(true).ToString("MM-dd").Replace("-", "");



                        String[] datasplit = Data.Split('|');

                            string itemno = datasplit[0].ToString();
                            string kptn = datasplit[1].ToString();
                            string dttiled = datasplit[2].ToString();
                            string ctrlNo = datasplit[3].ToString();
                            decimal principal = Convert.ToDecimal(datasplit[4].ToString());
                            string currency = Currency; //datasplit[5].ToString();
                            decimal charge = chargeValue; //datasplit[6].ToString();
                            string sourceoffund = datasplit[7].ToString();
                            string relationtoreceiver = datasplit[8].ToString();
                            string purpose = datasplit[9].ToString();
                            string senderlname = datasplit[10].ToString();
                            string senderfname = datasplit[11].ToString();
                            string sendermname = datasplit[12].ToString();
                            string senderIdtype = datasplit[13].ToString();
                            string senderIdnum = datasplit[14].ToString();
                            string senderidexpdate = datasplit[15].ToString();
                            string senderbdate = datasplit[16].ToString();
                            string sendergender = datasplit[17].ToString();
                            string senderstreet = datasplit[18].ToString();
                            string senderprovince = datasplit[19].ToString();
                            string sendercountry = datasplit[20].ToString();
                            string senderphone = datasplit[21].ToString();
                            string receiverlname = datasplit[22].ToString();
                            string receiverfname = datasplit[23].ToString();
                            string receivermname = datasplit[24].ToString();
                            string receiverstreet = datasplit[25].ToString();
                            string receiversprovince = datasplit[26].ToString();
                            string receiverscountry = datasplit[27].ToString();
                            string receiverbdate = datasplit[28].ToString();
                            string receivergender = datasplit[29].ToString();
                            string receiverphonenum = datasplit[30].ToString();
                            string msg = datasplit[31].ToString();
                            string senderfullname = senderfname + " " + sendermname + " " + senderlname;
                            string receiverfullname = receiverfname + " " + receivermname + " " + receiverlname;
                            string OtherDetails = itemno + "|" + kptn + "|" + dttiled + "|" + sourceoffund + "|" + relationtoreceiver + "|" + purpose + "|" + senderIdtype + "|" + senderIdnum + "|" + senderidexpdate;
                            String IRNumber = "";
                            Decimal OtherCharge = 0;
                            Decimal Total = 0;
                            String CancelledDate = "";
                            String AccountCode = AccountId;
                            String CancelledByOperatorID = "";
                            String CancelledByBranchCode = "";
                            String CancelledByZoneCode = "";
                            String CancelledByStationID = "";
                            String CancelReason = "";
                            String CancelDetails = "";
                            String ReceiverAddress = "";
                            Decimal CancelCharge = 0;
                            String ChargeTo = "";
                            Decimal Forex = 0;
                            String TraceNumber = "";
                            String SenderAddress = "";
                            String OperatorID = operatorID;
                            String StationID = stationID;
                            decimal Redeem = 0;
                            String receiverprovince = "";
                            String BranchCode = bcode;
                            int zc = Convert.ToInt32(zonecode);
                            int ret = checkreference(ctrlNo,AccountId);
                            ControlNumber = generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                           
                            if (ret == 0)
                            {
                                command.Parameters.Clear();
                                command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, IRNo,Currency, Principal," +
                                                      "Charge, OtherCharge, Total, CancelledDate, AccountCode, TransDate, CancelledByOperatorID," +
                                                      "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverAddress, ReceiverGender, ReceiverContactNo, ReceiverBirthDate, CancelCharge, " +
                                                      "ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, OtherDetails, OperatorID," +
                                                      "StationID, ReceiverStreet, ReceiverProvince, ReceiverCountry, KPTN,Message, Redeem, BranchCode, SenderBirthdate)" +
                                                      "values(@ControlNo, @RefNumber, @IRnum, @Currency, @Principal, @Charge, " +
                                                      "@OtherCharge, @Total, @CancelledDate, @AccountCode, now(), @CancelledByOperatorID, " +
                                                      "@CancelledByBranchCode, @CancelledByZoneCode, @CancelledByStationID, @CancelReason, " +
                                                      "@CancelDetails, @SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                      "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverAddress, @ReceiverGender, @ReceiverContactNo, " +
                                                      "@ReceiverBDate, @CancelCharge, @ChargeTo, @Forex, @TraceNo, @SenderAddress, @SenderGender, @SenderContactNo, " +
                                                      "@sessionID, @OtherDetails, @OperatorId, @StationID, @ReceiverStreet, @ReceiverProvince, @ReceiverCountry,@KPTN, @Message, @Redeem, @BranchCode, @SenderBdate);";

                                command.Parameters.Clear();
				                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command.Parameters.AddWithValue("KPTN", kptn);
                                command.Parameters.AddWithValue("RefNumber", ctrlNo);
                                command.Parameters.AddWithValue("IRnum", IRNumber);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("Charge", charge);
                                command.Parameters.AddWithValue("OtherCharge", OtherCharge);
                                command.Parameters.AddWithValue("Total", Total);
                                command.Parameters.AddWithValue("CancelledDate", CancelledDate);
                                command.Parameters.AddWithValue("AccountCode", AccountCode);
                                command.Parameters.AddWithValue("CancelledByOperatorID", CancelledByOperatorID);
                                command.Parameters.AddWithValue("CancelledByBranchCode", CancelledByBranchCode);
                                command.Parameters.AddWithValue("CancelledByZoneCode", CancelledByZoneCode);
                                command.Parameters.AddWithValue("CancelledByStationID", CancelledByStationID);
                                command.Parameters.AddWithValue("CancelReason", CancelReason);
                                command.Parameters.AddWithValue("CancelDetails", CancelDetails);
                                command.Parameters.AddWithValue("SenderFName", senderfname);
                                command.Parameters.AddWithValue("SenderLName", senderlname);
                                command.Parameters.AddWithValue("SenderMName", sendermname);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverFName", receiverfname);
                                command.Parameters.AddWithValue("ReceiverLName", receiverlname);
                                command.Parameters.AddWithValue("ReceiverMName", receivermname);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("ReceiverAddress", ReceiverAddress);
                                command.Parameters.AddWithValue("ReceiverGender", receivergender);
                                command.Parameters.AddWithValue("ReceiverContactNo", receiverphonenum);
                                command.Parameters.AddWithValue("ReceiverBDate", receiverbdate);
                                command.Parameters.AddWithValue("CancelCharge", CancelCharge);
                                command.Parameters.AddWithValue("ChargeTo", ChargeTo);
                                command.Parameters.AddWithValue("Forex", Forex);
                                command.Parameters.AddWithValue("TraceNo", TraceNumber);
                                command.Parameters.AddWithValue("SenderAddress", SenderAddress);
                                command.Parameters.AddWithValue("SenderGender", sendergender);
                                command.Parameters.AddWithValue("SenderContactNo", senderphone);
                                command.Parameters.AddWithValue("sessionId", sessionId);
                                command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command.Parameters.AddWithValue("OperatorId", operatorID);
                                command.Parameters.AddWithValue("StationID", stationID);
                                command.Parameters.AddWithValue("ReceiverStreet", receiverstreet);
                                command.Parameters.AddWithValue("ReceiverProvince", receiverprovince);
                                command.Parameters.AddWithValue("ReceiverCountry", receiverscountry);
                                command.Parameters.AddWithValue("Message", msg);
                                command.Parameters.AddWithValue("Redeem", Redeem);
                                command.Parameters.AddWithValue("BranchCode", bcode);
                                command.Parameters.AddWithValue("SenderBdate", senderbdate);
                                result = command.ExecuteNonQuery();
                                int stID = Convert.ToInt16(stationID);
                                int zcode = Convert.ToInt16(zonecode);
                                                                                
                                result = InsertCorporateTrans(ControlNumber, kptn,"",ctrlNo, AccountCode, currency,DateTime.Now,stID,0,OperatorID,BranchCode,zcode,"","",0,principal,ChargeTo,charge,OtherCharge,Total,Forex,TraceNumber,sessionId,senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, 1); 
                                    

                                 command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno)"+
                                     " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo);";
                                 command.Parameters.Clear(); 
                                 command.Parameters.AddWithValue("kptnnum", kptn);
                                 command.Parameters.AddWithValue("refnum", ctrlNo);
                                 command.Parameters.AddWithValue("action", "SENDOUT");
                                 command.Parameters.AddWithValue("bcode", bname);
                                 command.Parameters.AddWithValue("opID", operatorID);
                                 command.Parameters.AddWithValue("scode", stationcode);
                                 command.Parameters.AddWithValue("sID", stationID);
                                 command.Parameters.AddWithValue("zCode", zonecode);
                                 command.Parameters.AddWithValue("txndate", DateTime.Now);
                                 //9command.Parameters.AddWithValue("bname", bname);
                                 command.Parameters.AddWithValue("type", "kppartners");
                                 command.Parameters.AddWithValue("AccountCode", AccountCode);
                                 command.Parameters.AddWithValue("Currency", Currency);
                                 command.Parameters.AddWithValue("SenderName", senderfullname);
                                 command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                 command.Parameters.AddWithValue("Principal", principal);
                                 command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                 result = command.ExecuteNonQuery();

                                 if (result.Equals("-1"))
                                 {

                                     con.Close();
                                     return Return = "3";
                                 }
                                

                                i = i + 1;
                            }
                            else
                            {
                                
                                con.Close();
                                return Return = "2";
                            }


                            
                            con.Close();
                            Return = "1";
                       
                    }
                }
              catch (Exception ex)
                {
                     
                      con.Close();
                      return ex.Message;
                }

            
          }
        

        return  Return;
    }
   
    public String SaveSendOutTransExcel(String Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID,String Currency)
    {
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        String sessionId = manager.CreateSessionID(Context);
        Int32 result = 0;
        String Return = "";
        int i = 1;
        
        //string batchnum = generateBatchNo();

        ConnectPartners();

        String ControlNumber;
        

        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = getServerDatePartner(true).ToString("MM-dd").Replace("-", "");


                    String[] datasplit = Data.Split('|');
                        string ctrlNo = datasplit[1].ToString();

                         string senderlname = datasplit[2].ToString();
                        string senderfname = datasplit[3].ToString();
                        string sendermname = datasplit[4].ToString();
                        string senderIdtype = "";
                        string senderIdnum = "";
                        string senderidexpdate =""; 
                        string senderbdate = "";
                        string sendergender = "";
                       
                        string senderphone = "";
                        
                         string receiverlname = datasplit[5].ToString();
                        string receiverfname = datasplit[6].ToString();
                        string receivermname = datasplit[7].ToString();
                        string ReceiverAddress = datasplit[8].ToString();
                        string receiverstreet = "";
                        
                        string receiverscountry = "";
                        string receiverbdate = "";
                        string receivergender = "";
                        string receiverphonenum = datasplit[9].ToString();

                        string itemno = "";
                        string kptn = datasplit[13].ToString();
                        string dttiled = datasplit[0].ToString();
                        
                        decimal principal =Convert.ToDecimal(datasplit[10].ToString());
                        string currency = Currency;
                        decimal charge = chargeValue;//datasplit[11].ToString();
                         Decimal Total = Convert.ToDecimal(datasplit[12].ToString());
                         string sourceoffund = ""; 
                        string relationtoreceiver = "";
                        string purpose = "";
                       
                       
                        string msg = "";
                        string senderfullname = senderfname + " " + sendermname + " " + senderlname;
                        string receiverfullname = receiverfname + " " + receivermname + " " + receiverlname;
                        string OtherDetails = itemno + "|" + kptn + "|" + dttiled + "|" + sourceoffund + "|" + relationtoreceiver + "|" + purpose + "|" + senderIdtype + "|" + senderIdnum + "|" + senderidexpdate;
                        String IRNumber = "";
                        Decimal OtherCharge = 0;
                       
                        String CancelledDate = "";
                        String AccountCode = AccountId;
                        String CancelledByOperatorID = "";
                        String CancelledByBranchCode = "";
                        String CancelledByZoneCode = "";
                        String CancelledByStationID = "";
                        String CancelReason = "";
                        String CancelDetails = "";
                        
                        Decimal CancelCharge = 0;
                        String ChargeTo = "";
                        Decimal Forex = 0;
                        String TraceNumber = "";
                        String SenderAddress = "";
                        String OperatorID = operatorID;
                        String StationID = stationID;
                        decimal Redeem = 0;
                        String receiverprovince = "";
                        String BranchCode = bcode;

                      
                            int ret = checkreference(ctrlNo,AccountId);
                            int zc = Convert.ToInt32(zonecode);
                            ControlNumber = generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                            if (ret == 0)
                            {
                                command.Parameters.Clear();
                                command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, IRNo,Currency, Principal," +
                                                      "Charge, OtherCharge, Total, CancelledDate, AccountCode, TransDate, CancelledByOperatorID," +
                                                      "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverAddress, ReceiverGender, ReceiverContactNo, ReceiverBirthDate, CancelCharge, " +
                                                      "ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, OtherDetails, OperatorID," +
                                                      "StationID, ReceiverStreet, ReceiverProvince, ReceiverCountry, KPTN,Message, Redeem, BranchCode, SenderBirthdate)" +
                                                      "values(@ControlNo, @RefNumber, @IRnum, @Currency, @Principal, @Charge, " +
                                                      "@OtherCharge, @Total, @CancelledDate, @AccountCode, now(), @CancelledByOperatorID, " +
                                                      "@CancelledByBranchCode, @CancelledByZoneCode, @CancelledByStationID, @CancelReason, " +
                                                      "@CancelDetails, @SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                      "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverAddress, @ReceiverGender, @ReceiverContactNo, " +
                                                      "@ReceiverBDate, @CancelCharge, @ChargeTo, @Forex, @TraceNo, @SenderAddress, @SenderGender, @SenderContactNo, " +
                                                      "@sessionID, @OtherDetails, @OperatorId, @StationID, @ReceiverStreet, @ReceiverProvince, @ReceiverCountry,@KPTN, @Message, @Redeem, @BranchCode, @SenderBdate);";

                                
                                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command.Parameters.AddWithValue("KPTN", kptn);
                                command.Parameters.AddWithValue("RefNumber", ctrlNo);
                                command.Parameters.AddWithValue("IRnum", IRNumber);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("Charge", charge);
                                command.Parameters.AddWithValue("OtherCharge", OtherCharge);
                                command.Parameters.AddWithValue("Total", Total);
                                command.Parameters.AddWithValue("CancelledDate", CancelledDate);
                                command.Parameters.AddWithValue("AccountCode", AccountCode);
                                command.Parameters.AddWithValue("CancelledByOperatorID", CancelledByOperatorID);
                                command.Parameters.AddWithValue("CancelledByBranchCode", CancelledByBranchCode);
                                command.Parameters.AddWithValue("CancelledByZoneCode", CancelledByZoneCode);
                                command.Parameters.AddWithValue("CancelledByStationID", CancelledByStationID);
                                command.Parameters.AddWithValue("CancelReason", CancelReason);
                                command.Parameters.AddWithValue("CancelDetails", CancelDetails);
                                command.Parameters.AddWithValue("SenderFName", senderfname);
                                command.Parameters.AddWithValue("SenderLName", senderlname);
                                command.Parameters.AddWithValue("SenderMName", sendermname);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverFName", receiverfname);
                                command.Parameters.AddWithValue("ReceiverLName", receiverlname);
                                command.Parameters.AddWithValue("ReceiverMName", receivermname);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("ReceiverAddress", ReceiverAddress);
                                command.Parameters.AddWithValue("ReceiverGender", receivergender);
                                command.Parameters.AddWithValue("ReceiverContactNo", receiverphonenum);
                                command.Parameters.AddWithValue("ReceiverBDate", receiverbdate);
                                command.Parameters.AddWithValue("CancelCharge", CancelCharge);
                                command.Parameters.AddWithValue("ChargeTo", ChargeTo);
                                command.Parameters.AddWithValue("Forex", Forex);
                                command.Parameters.AddWithValue("TraceNo", TraceNumber);
                                command.Parameters.AddWithValue("SenderAddress", SenderAddress);
                                command.Parameters.AddWithValue("SenderGender", sendergender);
                                command.Parameters.AddWithValue("SenderContactNo", senderphone);
                                command.Parameters.AddWithValue("sessionId", sessionId);
                                command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command.Parameters.AddWithValue("OperatorId", OperatorID);
                                command.Parameters.AddWithValue("StationID", StationID);
                                command.Parameters.AddWithValue("ReceiverStreet", receiverstreet);
                                command.Parameters.AddWithValue("ReceiverProvince", receiverprovince);
                                command.Parameters.AddWithValue("ReceiverCountry", receiverscountry);
                                command.Parameters.AddWithValue("Message", msg);
                                command.Parameters.AddWithValue("Redeem", Redeem);
                                command.Parameters.AddWithValue("BranchCode", BranchCode);
                                command.Parameters.AddWithValue("SenderBdate", senderbdate);
                                result = command.ExecuteNonQuery();



                                command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno)"+
                                    " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo);";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("kptnnum", kptn);
                                command.Parameters.AddWithValue("refnum", ctrlNo);
                                command.Parameters.AddWithValue("action", "SENDOUT");
                                command.Parameters.AddWithValue("bcode", bname);
                                command.Parameters.AddWithValue("opID", operatorID);
                                command.Parameters.AddWithValue("scode", stationcode);
                                command.Parameters.AddWithValue("sID", stationID);
                                command.Parameters.AddWithValue("zCode", zonecode);
                                command.Parameters.AddWithValue("txndate", DateTime.Now);
                                //9command.Parameters.AddWithValue("bname", bname);
                                command.Parameters.AddWithValue("type", "kppartners");
                                command.Parameters.AddWithValue("AccountCode", AccountCode);
                                command.Parameters.AddWithValue("Currency", Currency);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                result = command.ExecuteNonQuery();

                                int stID = Convert.ToInt16(stationID);
                                int zcode = Convert.ToInt16(zonecode);
                                //decimal Principal = Convert.ToDecimal(principal);
                               // decimal Total = Convert.ToDecimal(total);
                                //decimal OtherCharge = Convert.ToDecimal(othrchrge);
                                //decimal frex = Convert.ToDecimal(forex);

                                result = InsertCorporateTrans(ControlNumber, kptn, "", ctrlNo, AccountId, currency, DateTime.Now, stID, 0, operatorID, bcode, zcode, "", "", 0, principal, ChargeTo, charge, OtherCharge, Total, Forex, TraceNumber, sessionId, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, 1);


                                if (result.Equals("-1"))
                                {
                                   
                                    con.Close();
                                    return Return = "3";//Error in Inserting
                                }
                               
                                i = i + 1;
                            }
                            else
                            {
                               
                                con.Close();
                                return Return = "2";//duplicate data in database
                            }

                    con.Close();
                    Return = "1"; //successfuly Insert

                }
            }
            catch (Exception ex)
            {
                
                con.Close();
                return ex.Message;
            }


        }


        return Return;
    }
    public String SaveSendOutTransMoneyExchanger(String Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID,String Currency)
    {
        System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
        String sessionId = manager.CreateSessionID(Context);
        Int32 result = 0;
        String Return = "";
        int i = 1;

        //string batchnum = generateBatchNo();

        ConnectPartners();

        String ControlNumber;


        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = getServerDatePartner(true).ToString("MM-dd").Replace("-", "");


                    String[] datasplit = Data.Split('|');
                    string ctrlNo = datasplit[0].ToString();

                    string senderlname = datasplit[4].ToString().Trim();
                    string senderfname = datasplit[3].ToString().Trim();
                    string sendermname = datasplit[5].ToString().Trim();
                    string senderIdtype = "";
                    string senderIdnum = "";
                    string senderidexpdate = "";
                    string senderbdate = "";
                    string sendergender = "";

                    string senderphone = "";

                    string receiverlname = datasplit[7].ToString().Trim();
                    string receiverfname = datasplit[6].ToString().Trim();
                    string receivermname = datasplit[8].ToString().Trim();
                    string ReceiverAddress = datasplit[9].ToString().Trim();
                    string receiverstreet = datasplit[10].ToString().Trim();

                    string receiverscountry = datasplit[11].ToString().Trim();
                    string receiverbdate = "";
                    string receivergender = "";
                    string receiverphonenum = datasplit[12].ToString().Trim();

                    string itemno = "";
                    string kptn = datasplit[31].ToString();
                    string dttiled = "";

                    decimal principal = Convert.ToDecimal(datasplit[20].ToString());
                    string currency = Currency; //datasplit[21].ToString().Trim();
                    decimal charge = chargeValue;//datasplit[11].ToString().Trim();
                    Decimal Total = Convert.ToDecimal(datasplit[20].ToString());
                    string sourceoffund = "";
                    string relationtoreceiver = "";
                    string purpose = datasplit[14].ToString();


                    string msg = "";
                    string senderfullname = senderfname + " " + sendermname + " " + senderlname;
                    string receiverfullname = receiverfname + " " + receivermname + " " + receiverlname;
                    string OtherDetails = itemno + "|" + kptn + "|" + dttiled + "|" + sourceoffund + "|" + relationtoreceiver + "|" + purpose + "|" + senderIdtype + "|" + senderIdnum + "|" + senderidexpdate;
                    String IRNumber = "";
                    Decimal OtherCharge = 0;

                    String CancelledDate = "";
                    String AccountCode = AccountId;
                    String CancelledByOperatorID = "";
                    String CancelledByBranchCode = "";
                    String CancelledByZoneCode = "";
                    String CancelledByStationID = "";
                    String CancelReason = "";
                    String CancelDetails = "";

                    Decimal CancelCharge = 0;
                    String ChargeTo = "";
                    Decimal Forex = 0;
                    String TraceNumber = "";
                    String SenderAddress = "";
                    String OperatorID = operatorID;
                    String StationID = stationID;
                    decimal Redeem = 0;
                    String receiverprovince = "";
                    String BranchCode = bcode;


                    int ret = checkreference(ctrlNo,AccountId);
                    int zc = Convert.ToInt32(zonecode);
                    ControlNumber = generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                    if (ret == 0)
                    {
                        command.Parameters.Clear();
                        command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, IRNo,Currency, Principal," +
                                              "Charge, OtherCharge, Total, CancelledDate, AccountCode, TransDate, CancelledByOperatorID," +
                                              "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                                              "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                              "ReceiverName, ReceiverAddress, ReceiverGender, ReceiverContactNo, ReceiverBirthDate, CancelCharge, " +
                                              "ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, OtherDetails, OperatorID," +
                                              "StationID, ReceiverStreet, ReceiverProvince, ReceiverCountry, KPTN,Message, Redeem, BranchCode, SenderBirthdate)" +
                                              "values(@ControlNo, @RefNumber, @IRnum, @Currency, @Principal, @Charge, " +
                                              "@OtherCharge, @Total, @CancelledDate, @AccountCode, now(), @CancelledByOperatorID, " +
                                              "@CancelledByBranchCode, @CancelledByZoneCode, @CancelledByStationID, @CancelReason, " +
                                              "@CancelDetails, @SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                              "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverAddress, @ReceiverGender, @ReceiverContactNo, " +
                                              "@ReceiverBDate, @CancelCharge, @ChargeTo, @Forex, @TraceNo, @SenderAddress, @SenderGender, @SenderContactNo, " +
                                              "@sessionID, @OtherDetails, @OperatorId, @StationID, @ReceiverStreet, @ReceiverProvince, @ReceiverCountry,@KPTN, @Message, @Redeem, @BranchCode, @SenderBdate);";

                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("ControlNo", ControlNumber);
                        command.Parameters.AddWithValue("KPTN", kptn);
                        command.Parameters.AddWithValue("RefNumber", ctrlNo);
                        command.Parameters.AddWithValue("IRnum", IRNumber);
                        command.Parameters.AddWithValue("Currency", currency);
                        command.Parameters.AddWithValue("Principal", principal);
                        command.Parameters.AddWithValue("Charge", charge);
                        command.Parameters.AddWithValue("OtherCharge", OtherCharge);
                        command.Parameters.AddWithValue("Total", Total);
                        command.Parameters.AddWithValue("CancelledDate", CancelledDate);
                        command.Parameters.AddWithValue("AccountCode", AccountCode);
                        command.Parameters.AddWithValue("CancelledByOperatorID", CancelledByOperatorID);
                        command.Parameters.AddWithValue("CancelledByBranchCode", CancelledByBranchCode);
                        command.Parameters.AddWithValue("CancelledByZoneCode", CancelledByZoneCode);
                        command.Parameters.AddWithValue("CancelledByStationID", CancelledByStationID);
                        command.Parameters.AddWithValue("CancelReason", CancelReason);
                        command.Parameters.AddWithValue("CancelDetails", CancelDetails);
                        command.Parameters.AddWithValue("SenderFName", senderfname);
                        command.Parameters.AddWithValue("SenderLName", senderlname);
                        command.Parameters.AddWithValue("SenderMName", sendermname);
                        command.Parameters.AddWithValue("SenderName", senderfullname);
                        command.Parameters.AddWithValue("ReceiverFName", receiverfname);
                        command.Parameters.AddWithValue("ReceiverLName", receiverlname);
                        command.Parameters.AddWithValue("ReceiverMName", receivermname);
                        command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                        command.Parameters.AddWithValue("ReceiverAddress", ReceiverAddress);
                        command.Parameters.AddWithValue("ReceiverGender", receivergender);
                        command.Parameters.AddWithValue("ReceiverContactNo", receiverphonenum);
                        command.Parameters.AddWithValue("ReceiverBDate", receiverbdate);
                        command.Parameters.AddWithValue("CancelCharge", CancelCharge);
                        command.Parameters.AddWithValue("ChargeTo", ChargeTo);
                        command.Parameters.AddWithValue("Forex", Forex);
                        command.Parameters.AddWithValue("TraceNo", TraceNumber);
                        command.Parameters.AddWithValue("SenderAddress", SenderAddress);
                        command.Parameters.AddWithValue("SenderGender", sendergender);
                        command.Parameters.AddWithValue("SenderContactNo", senderphone);
                        command.Parameters.AddWithValue("sessionId", sessionId);
                        command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                        command.Parameters.AddWithValue("OperatorId", OperatorID);
                        command.Parameters.AddWithValue("StationID", StationID);
                        command.Parameters.AddWithValue("ReceiverStreet", receiverstreet);
                        command.Parameters.AddWithValue("ReceiverProvince", receiverprovince);
                        command.Parameters.AddWithValue("ReceiverCountry", receiverscountry);
                        command.Parameters.AddWithValue("Message", msg);
                        command.Parameters.AddWithValue("Redeem", Redeem);
                        command.Parameters.AddWithValue("BranchCode", BranchCode);
                        command.Parameters.AddWithValue("SenderBdate", senderbdate);
                        result = command.ExecuteNonQuery();



                        command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,"+
                                              "action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,"+
                                              "currency,sendername,receivername,principal,controlno) values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,"
                                              + "@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo);";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("kptnnum", kptn);
                        command.Parameters.AddWithValue("refnum", ctrlNo);
                        command.Parameters.AddWithValue("action", "SENDOUT");
                        command.Parameters.AddWithValue("bcode", bname);
                        command.Parameters.AddWithValue("opID", operatorID);
                        command.Parameters.AddWithValue("scode", stationcode);
                        command.Parameters.AddWithValue("sID", stationID);
                        command.Parameters.AddWithValue("zCode", zonecode);
                        command.Parameters.AddWithValue("txndate", DateTime.Now);
                        //9command.Parameters.AddWithValue("bname", bname);
                        command.Parameters.AddWithValue("type", "kppartners");
                        command.Parameters.AddWithValue("AccountCode", AccountCode);
                        command.Parameters.AddWithValue("Currency", Currency);
                        command.Parameters.AddWithValue("SenderName", senderfullname);
                        command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                        command.Parameters.AddWithValue("Principal", principal);
                        command.Parameters.AddWithValue("ControlNo", ControlNumber);
                        result = command.ExecuteNonQuery();


                        int stID = Convert.ToInt16(stationID);
                        int zcode = Convert.ToInt16(zonecode);
                        result = InsertCorporateTrans(ControlNumber, kptn, "", ctrlNo, AccountId, currency, DateTime.Now, stID, 0, operatorID, bcode, zcode, "", "", 0, principal, ChargeTo, charge, OtherCharge, Total, Forex, TraceNumber, sessionId, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, 1);

                        if (result.Equals("-1"))
                        {
                            
                            con.Close();
                            return Return = "3";//Error in Inserting
                        }

                        i = i + 1;
                    }
                    else
                    {

                        con.Close();
                        return Return = "2";//duplicate data in database
                    }

                    con.Close();
                    Return = "1"; //successfuly Insert

                }
            }
            catch (Exception ex)
            {

                con.Close();
                return ex.Message;
            }


        }


        return Return;
    }
    private String generateControlGlobal(String branchcode, Int32 type, String OperatorID, Int32 ZoneCode, String StationNumber, Double version, String stationcode)
    {

        String controlNumber = "";
        String controlno = "";
        try
        {

            DateTime dt = getServerDatePartner(true);

            String control = "";

            command.CommandText = "Select station, bcode, userid, nseries, zcode, type from kpadminpartners.control where station = @st and bcode = @bcode and zcode = @zcode and `type` = @tp FOR UPDATE";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("st", StationNumber);
            command.Parameters.AddWithValue("bcode", branchcode);
            command.Parameters.AddWithValue("zcode", ZoneCode);
            command.Parameters.AddWithValue("tp", type);
            MySqlDataReader Reader = command.ExecuteReader();

            if (Reader.HasRows)
            {
                //throw new Exception("Invalid type value");
                Reader.Read();
                //throw new Exception(Reader["station"].ToString() + " " + Reader["bcode"].ToString() + " " + Reader["type"].ToString());
                if (type == 0)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 1)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 2)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else if (type == 3)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else
                {
                    kplog.Error("Invalid type value");
                    throw new Exception("Invalid type value");
                }

                String s = Reader["Station"].ToString();
                String nseries = Reader["nseries"].ToString().PadLeft(6, '0');
                Int32 seriesno = Convert.ToInt32(nseries) + 1;
                nseries = seriesno.ToString().PadLeft(6, '0');
                Reader.Close();

                if (isSameYear2(dt))
                {
                    controlno = control + "-" + dt.ToString("yy") + "-" + nseries;
                }
                else
                {
                    controlno = control + "-" + dt.ToString("yy") + "-" + "000001";
                }
                command.Parameters.Clear();
                command.CommandText = "Update kpadminpartners.control set nseries='" + nseries + "' where " +
                                      "station = @stu and bcode = @bcodeu and zcode = @zcodeu and `type` = @tpu;";
                command.Parameters.AddWithValue("stu", StationNumber);
                command.Parameters.AddWithValue("bcodeu", branchcode);
                command.Parameters.AddWithValue("zcodeu", ZoneCode);
                command.Parameters.AddWithValue("tpu", type);
                command.ExecuteNonQuery();

                command.Dispose();

            }
            else
            {
                Reader.Close();
                command.CommandText = "Insert into kpadminpartners.control (`station`,`bcode`,`userid`,`nseries`,`zcode`, `type`) values (@station,@branchcode,@uid,1,@zonecode,@type)";
                if (type == 0)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 1)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-" + branchcode;
                }
                else if (type == 2)
                {
                    control += "S0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else if (type == 3)
                {
                    control += "P0" + ZoneCode.ToString() + "-" + StationNumber + "-R" + branchcode;
                }
                else
                {
                    kplog.Error("Invalid type value");
                    throw new Exception("Invalid type value");
                }
                command.Parameters.AddWithValue("station", StationNumber);
                command.Parameters.AddWithValue("branchcode", branchcode);
                command.Parameters.AddWithValue("uid", OperatorID);
                command.Parameters.AddWithValue("zonecode", ZoneCode);
                command.Parameters.AddWithValue("type", type);
                int x = command.ExecuteNonQuery();
                //if (x < 1) {
                //    conn.Close();
                //    throw new Exception("asdfsadfds");
                //}

                controlno = control + "-" + dt.ToString("yy") + "-" + "000001";
                command.Dispose();
            }
        }
        catch (Exception ex)
        {

            return ex.Message.ToString();
        }



        return controlNumber = controlno;
    }
    private Boolean isSameYear2(DateTime date)
    {
        try
        {
            //throw new Exception(date.Year.ToString());
            if (GetYesterday2(date).Year.Equals(date.Year))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            kplog.Fatal(ex.ToString());
            throw new Exception(ex.ToString());
        }
    }
    private DateTime GetYesterday2(DateTime date)
    {
        return date.AddDays(-1);
    }
    public DateTime getServerDatePartner(Boolean isOpenConnection)
    {
        try
        {
            if (!isOpenConnection)
            {
                using (MySqlConnection conn = dbconkp.getConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        DateTime serverdate;
                        command.CommandText = "Select NOW() as serverdt;";
                        using (MySqlDataReader Reader = command.ExecuteReader())
                        {
                            Reader.Read();
                            serverdate = Convert.ToDateTime(Reader["serverdt"]);
                            //Reader.Close();
                            //conn.Close();
                            return serverdate;
                        }
                    }
                }
            }
            else
            {
                DateTime serverdate;
                command.CommandText = "Select NOW() as serverdt;";
                using (MySqlDataReader Reader = command.ExecuteReader())
                {
                    Reader.Read();
                    serverdate = Convert.ToDateTime(Reader["serverdt"]);
                    Reader.Close();
                    return serverdate;
                }
            }
        }
        catch (Exception ex)
        {
            kplog.Fatal(ex.ToString());
            throw new Exception(ex.Message);
        }
    }

    public int InsertCorporateTrans(string controlno,string kptn,string oldkptn,string referenceno,string accountid,string currency,DateTime transdate,int stationno,int isremote,string operatorid,string branchcode,int zonecode,string remoteoperatorid,string remotebranchcode,int remotezonecode,decimal principal,string chargeto,decimal chargeamount,decimal othercharge,decimal total,decimal forex,string traceno,string sessionid,string senderfname,string senderlname,string sendermname,string receiverfname,string receiverlname,string receivermname,int isactive)
    {
        
        //using (MySqlConnection con = dbconkp.getConnection())
        //{
        //    con.Open();
            try
            {
                //using(command = con.CreateCommand())
                //{
                    string query = "insert into kppartnerstransactions.corporatesendouts (controlno,kptn,oldkptn,referenceno," +
                        "accountid,currency,transdate,stationno,isremote,operatorid,branchcode,zonecode,remoteoperatorid,"+
                        "remotebranchcode,remotezonecode,principal,chargeto,chargeamount,othercharge,total,forex,traceno,"+
                        "sessionid,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,isactive)"+ 
                        " values(@controlno,@kptn,@oldkptn,@referenceno,"+
                        "@accountid,@currency,@transdate,@stationno,@isremote,@operatorid,@branchcode,@zonecode,@remoteoperatorid,"+
                        "@remotebranchcode,@remotezonecode,@principal,@chargeto,@chargeamount,@othercharge,@total,@forex,@traceno,"+
                        "@sessionid,@senderfname,@senderlname,@sendermname,@receiverfname,@receiverlname,@receivermname,@isactive)";
                    command.CommandText = query;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("controlno",controlno);
                    command.Parameters.AddWithValue("kptn",kptn);
                    command.Parameters.AddWithValue("oldkptn",oldkptn);
                    command.Parameters.AddWithValue("referenceno",referenceno);
                    command.Parameters.AddWithValue("accountid",accountid);
                    command.Parameters.AddWithValue("currency",currency);
                    command.Parameters.AddWithValue("transdate",transdate);
                    command.Parameters.AddWithValue("stationno",stationno);
                    command.Parameters.AddWithValue("isremote",isremote);
                    command.Parameters.AddWithValue("operatorid",operatorid);
                    command.Parameters.AddWithValue("branchcode",branchcode);
                    command.Parameters.AddWithValue("zonecode",zonecode);
                    command.Parameters.AddWithValue("remoteoperatorid",remoteoperatorid);
                    command.Parameters.AddWithValue("remotebranchcode",remotebranchcode);
                    command.Parameters.AddWithValue("remotezonecode",remotezonecode);
                    command.Parameters.AddWithValue("principal",principal);
                    command.Parameters.AddWithValue("chargeto",chargeto);
                    command.Parameters.AddWithValue("chargeamount",chargeamount);
                    command.Parameters.AddWithValue("othercharge",othercharge);
                    command.Parameters.AddWithValue("total",total);
                    command.Parameters.AddWithValue("forex",forex);
                    command.Parameters.AddWithValue("traceno",traceno);
                    command.Parameters.AddWithValue("sessionid",sessionid);
                    command.Parameters.AddWithValue("senderfname",senderfname);
                    command.Parameters.AddWithValue("senderlname",senderlname);
                    command.Parameters.AddWithValue("sendermname",sendermname);
                    command.Parameters.AddWithValue("receiverfname",receiverfname);
                    command.Parameters.AddWithValue("receiverlname",receiverlname);
                    command.Parameters.AddWithValue("receivermname",receivermname);
                    command.Parameters.AddWithValue("isactive",isactive);
                    int ret = command.ExecuteNonQuery();
                    if (ret == -1)
                    {
                       
                        return ret;
                    }
                    else
                    {
                        
                        return ret;
                    }

                    
                //}
            }
            catch (Exception ex)
            {
               
                throw new Exception(ex.Message);
            }
       // }
    }

    public Object TestForDBP(string sessionId)
    {

        if (upnew.SubmitDBPDatabase(sessionId) == "Success")
        {
            return new Testing { respcode = 1, msg = "Successfully submit" };
        }
        else
        {
            return new Testing { respcode = 0, msg = "Error in submitting" };
        }
    }

    public Object TestForP2P(string sessionId)
    {
        if (upnew.SubmitToDatabase(sessionId) == "Success")
        {
            return new Testing { respcode = 1, msg = "Successfully submit" };
        }
        else
        {
            return new Testing { respcode = 0, msg = "Error in submitting" };
        }
    }

    public Object TestForMOEX(string sessionId)
    {
        if (upnew.SubmitMOEXToDatabase(sessionId) == "Success")
        {
            return new Testing { respcode = 1, msg = "Successfully submit" };
        }
        else
        {
            return new Testing { respcode = 0, msg = "Error in submitting" };
        }
    }

    public Object TestForBPI(string sessionId)
    {
        if (upnew.SubmitBPIDatabase(sessionId) == "Success")
        {
            return new Testing { respcode = 1, msg = "Successfully submit" };
        }
        else
        {
            return new Testing { respcode = 0, msg = "Error in submitting" };
        }
    }

    public Object TestForBDO(string sessionId)
    {
        if (upnew.SubmitBDOToDatabase(sessionId) == "Success")
        {
            return new Testing { respcode = 1, msg = "Successfully submit" };
        }
        else
        {
            return new Testing { respcode = 0, msg = "Error in submitting" };
        }
    }
    

   
    
   
}
