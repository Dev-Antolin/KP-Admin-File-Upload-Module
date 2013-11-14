using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Services;
using MySql.Data.MySqlClient;
using log4net;



/// <summary>
/// Summary description for FileUploadSave
/// </summary>
public class FileUploadSave
{
    datatrappings ser = new datatrappings();

    public String[] arrayListofError;
    public String[] arrayListOfSuccess;
    int countererror = 0;
    int counterSuccess = 0;
    int countercorpoSO = 0;
    public string Erromsg = string.Empty;
    public string batchnumber;
    public MySqlTransaction trans = null;
    public MySqlCommand command;
    public String[] arrCorpoSO;
    public int counter = 0;
    public int counterErr = 0;

    public string ermsg { get; set; }

    public string SaveSendOutTrans(String[] Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId)
    {
        int result;
        // int identifier = 0;
        string output = string.Empty;
        string errorname = string.Empty;
        String sessionId = SessionId;
        
        ser.ConnectPartners();
        batchnumber = ser.generateBatchNo();
        using (MySqlConnection con = ser.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
                    
                    trans = con.BeginTransaction();
                    command.Transaction = trans;
                    arrayListofError = new String[Data.Length];
                    arrayListOfSuccess = new String[Data.Length];
                    arrCorpoSO = new String[Data.Length];

                    foreach (String item in Data)
                    {
                        decimal charge;
                        string[] datasplit = item.Split('|');
                        string itemno = datasplit[0].ToString();
                        string kptn = datasplit[1].ToString();
                        string dttiled = datasplit[2].ToString();
                        string ctrlNo = datasplit[3].ToString();
                        decimal principal = Convert.ToDecimal(datasplit[4].ToString());
                        string currency = datasplit[5].ToString();
                        string chg = datasplit[6].ToString();
                        if (chg == "")
                        {

                            charge = 0;
                        }
                        else
                        {
                            charge = Convert.ToDecimal(datasplit[6].ToString());
                        }
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
                        String OperatorID = operatorID;
                        String StationID = stationID;
                        String BranchCode = bcode;
                        String AccountCode = AccountId;
                        int stID = Convert.ToInt16(stationID);
                        int zcode = Convert.ToInt16(zonecode);
                        int zc = Convert.ToInt32(zonecode);
                        //if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true) || ser.checkCreditLimit(AccountCode, principal, currency).Equals(true))
                        if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true))
                        {
                            int ret = ser.checkreference(ctrlNo, AccountId);

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                            if (ret == 0)
                            {
                                command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, Currency, Principal," +
                                                          "Charge, AccountCode, TransDate, " +
                                                          "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                          "ReceiverName, ReceiverGender, ReceiverContactNo,ReceiverBirthDate, " +
                                                          "SenderGender, SenderContactNo, sessionID, OtherDetails, OperatorID," +
                                                          "StationID, ReceiverStreet, ReceiverCountry, KPTN, Message, BranchCode, SenderBirthdate)" +
                                                          "values(@ControlNo, @RefNumber, @Currency, @Principal, @Charge, " +
                                                          "@AccountCode, now(), " +
                                                          "@SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                          "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverGender, @ReceiverContactNo, " +
                                                          "@ReceiverBDate, @SenderGender, @SenderContactNo, " +
                                                          "@sessionID, @OtherDetails, @OperatorId, @StationID, @ReceiverStreet, @ReceiverCountry, " +
                                                          "@KPTN, @Message, @BranchCode, @SenderBdate);";

                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command.Parameters.AddWithValue("KPTN", kptn);
                                command.Parameters.AddWithValue("RefNumber", ctrlNo);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("Charge", charge);
                                command.Parameters.AddWithValue("AccountCode", AccountCode);
                                command.Parameters.AddWithValue("SenderFName", senderfname);
                                command.Parameters.AddWithValue("SenderLName", senderlname);
                                command.Parameters.AddWithValue("SenderMName", sendermname);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverFName", receiverfname);
                                command.Parameters.AddWithValue("ReceiverLName", receiverlname);
                                command.Parameters.AddWithValue("ReceiverMName", receivermname);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("ReceiverGender", receivergender);
                                command.Parameters.AddWithValue("ReceiverContactNo", receiverphonenum);
                                command.Parameters.AddWithValue("ReceiverBDate", receiverbdate);
                                command.Parameters.AddWithValue("SenderGender", sendergender);
                                command.Parameters.AddWithValue("SenderContactNo", senderphone);
                                command.Parameters.AddWithValue("sessionId", sessionId);
                                command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command.Parameters.AddWithValue("OperatorId", operatorID);
                                command.Parameters.AddWithValue("StationID", stationID);
                                command.Parameters.AddWithValue("ReceiverStreet", receiverstreet);
                                command.Parameters.AddWithValue("ReceiverCountry", receiverscountry);
                                command.Parameters.AddWithValue("Message", msg);
                                command.Parameters.AddWithValue("BranchCode", bcode);
                                command.Parameters.AddWithValue("SenderBdate", senderbdate);
                                result = command.ExecuteNonQuery();
                                //**************************************************insert transactionlogs**********************************************//



                                //**************************************inserting sotxnref table********************************************************//

                                DateTime dt = ser.getServerDatePartner(true);
                                string day = dt.ToString("dd");
                                string m = dt.ToString("MM");
                                string tableRef = "sendout" + m + day;
                                string TDate = dt.ToString("yyyy-MM-dd hh:mm:ss");


                                String sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) values (@xrefno,@xtableRef,@xAccountCode,@xBatchNo,@xTDate,@xCurr,@Ttype) ";
                                command.Parameters.Clear();
                                command.CommandText = sql;
                                command.Parameters.AddWithValue("xrefno", ctrlNo);
                                command.Parameters.AddWithValue("xtableref", tableRef);
                                command.Parameters.AddWithValue("xAccountCode", AccountCode);
                                command.Parameters.AddWithValue("xBatchNo", batchnumber);
                                command.Parameters.AddWithValue("xTDate", dt);
                                command.Parameters.AddWithValue("xCurr", currency);
                                command.Parameters.AddWithValue("Ttype", 2);
                                result = command.ExecuteNonQuery();

                                //*******************************************inserting corporatesendouts************************************************//
                                arrCorpoSO[countercorpoSO] = ControlNumber + "|" + kptn + "|" + " " + "|" + ctrlNo + "|" + AccountCode + "|" + currency + "|" + dt + "|" + stID + "|" + "0" + "|" + OperatorID + "|" + BranchCode + "|" + zonecode + "|" + " " + "|" + " " + "|" + "0" + "|" + principal + "|" + " " + "|" + charge + "|0|0|0|" + " " + "|" + sessionId + "|" + senderfname + "|" + senderlname + "|" + sendermname + "|" + receiverfname + "|" + receiverlname + "|" + receivermname + "|" + "1";
                                countercorpoSO = countercorpoSO + 1;

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    arrayListofError[countererror] = item + "|" + errorname;
                                    countererror = countererror + 1;


                                }
                                else
                                {
                                    arrayListOfSuccess[counterSuccess] = item;
                                    counterSuccess = counterSuccess + 1;
                                }


                            }
                            else
                            {

                                errorname = "Reference Number Already Exist";
                                arrayListofError[countererror] = item + "|" + errorname;
                                countererror = countererror + 1;

                            }
                        }
                        else
                        {
                            errorname = "Amount is Greater than the Threshold";
                            arrayListofError[countererror] = item + "|" +errorname;
                            countererror = countererror + 1;

                        }

                    }
                    if (counterSuccess != Data.Length)
                    {

                        trans.Rollback();
                        con.Close();
                        return output = "Failed";
                    }
                    else
                    {
                        int data = ser.insertData(AccountId, batchnumber, counterSuccess, batchnum);
                        if (data == 1)
                        {

                            foreach (string sItem in arrCorpoSO)
                            {
                                string[] datasplit = sItem.Split('|');

                                string controlno = datasplit[0].ToString();
                                string kptn = datasplit[1].ToString();
                                string oldkptn = datasplit[2].ToString();
                                string referenceno = datasplit[3].ToString();
                                string accountid = datasplit[4].ToString();
                                string currency = datasplit[5].ToString();
                                DateTime transdate = Convert.ToDateTime(datasplit[6].ToString());
                                int stationno = Convert.ToInt16(datasplit[7].ToString());
                                int isremote = Convert.ToInt16(datasplit[8].ToString());
                                string operatorid = datasplit[9].ToString();
                                string branchcode = datasplit[10].ToString();
                                int Zcodecorp = Convert.ToInt16(datasplit[11].ToString());
                                string remoteoperatorid = datasplit[12].ToString();
                                string remotebranchcode = datasplit[13].ToString();
                                int remotezonecode = Convert.ToInt16(datasplit[14].ToString());
                                string principalS = datasplit[15].ToString();
                                decimal principal;
                                if (principalS == string.Empty)
                                {
                                    principal = 0;
                                }
                                else
                                {
                                    principal = Convert.ToDecimal(principalS);
                                }
                                string chargeto = datasplit[16].ToString();
                                string chargeamountS = datasplit[17].ToString();
                                decimal chargeamount;
                                if (chargeamountS == string.Empty)
                                {
                                    chargeamount = 0;
                                }
                                else
                                {
                                    chargeamount = Convert.ToDecimal(chargeamountS);
                                }
                                decimal othercharge = Convert.ToDecimal(datasplit[18].ToString());
                                decimal total = Convert.ToDecimal(datasplit[19].ToString());
                                decimal forex = Convert.ToDecimal(datasplit[20].ToString());
                                string traceno = datasplit[21].ToString();
                                string sessionid = datasplit[22].ToString();
                                string senderfname = datasplit[23].ToString();
                                string senderlname = datasplit[24].ToString();
                                string sendermname = datasplit[25].ToString();
                                string receiverfname = datasplit[26].ToString();
                                string receiverlname = datasplit[27].ToString();
                                string receivermname = datasplit[28].ToString();
                                string senderfullname = senderfname + " " + sendermname + " " + senderlname;
                                string receiverfullname = receiverfname + " " + receivermname + " " + receiverlname;
                                int isactive = Convert.ToInt16(datasplit[29].ToString());

                                string sql = ser.InsertCorporateTrans(controlno, kptn, oldkptn, referenceno, accountid, currency, transdate, stationno, isremote, operatorid, branchcode, Zcodecorp, remoteoperatorid, remotebranchcode, remotezonecode, principal, chargeto, chargeamount, othercharge, total, forex, traceno, sessionid, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, isactive);

                              
                                command.CommandText = sql;
                                result = command.ExecuteNonQuery();


                                command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno,isremote)" +
                                     " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo,@isremote);";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("kptnnum", kptn);
                                command.Parameters.AddWithValue("refnum", referenceno);
                                command.Parameters.AddWithValue("action", "SENDOUT");
                                command.Parameters.AddWithValue("bcode", branchcode);
                                command.Parameters.AddWithValue("opID", operatorID);
                                command.Parameters.AddWithValue("scode", stationcode);
                                command.Parameters.AddWithValue("sID", stationID);
                                command.Parameters.AddWithValue("zCode", zonecode);
                                command.Parameters.AddWithValue("txndate", transdate);
                                //9command.Parameters.AddWithValue("bname", bname);
                                command.Parameters.AddWithValue("type", "kppartners");
                                command.Parameters.AddWithValue("AccountCode", accountid);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("ControlNo", controlno);
                                command.Parameters.AddWithValue("isremote", 0);
                                result = command.ExecuteNonQuery();

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    string item = Data[counter].ToString();
                                    arrayListofError[countererror] = item + "|" + errorname;
                                    countererror = countererror + 1;
                                    counterErr += 1;


                                }

                                counter += 1;


                            }
                            if (counterErr == 0)
                            {
                                trans.Commit();
                                con.Close();
                                output = "Success";
                            }
                            else
                            {
                                trans.Rollback();
                                con.Close();
                                output = "Failed";
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            con.Close();
                            output = "Failed";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                output = "error";
                ermsg = ex.Message;
                trans.Rollback();
                con.Close();
                return output;
            }
        }

        return output;

    }

    //***********************************************************EXCEL INSERTION****************************************************************//

    public String SaveSendOutTransExcel(String[] Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String sessionId)
    {
        int result;
        string output = string.Empty;
        String SessionId = sessionId;
        ser.ConnectPartners();
        batchnumber = ser.generateBatchNo();

        using (MySqlConnection con = ser.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
                    trans = con.BeginTransaction();
                    command.Transaction = trans;
                    arrayListofError = new String[Data.Length];
                    arrayListOfSuccess = new String[Data.Length];
                    arrCorpoSO = new String[Data.Length];

                    foreach (string Dataitem in Data)
                    {
                        String[] datasplit = Dataitem.Split('|');
                        string ctrlNo = datasplit[1].ToString();
                        string senderlname = datasplit[2].ToString();
                        string senderfname = datasplit[3].ToString();
                        string sendermname = datasplit[4].ToString();
                        string receiverlname = datasplit[5].ToString();
                        string receiverfname = datasplit[6].ToString();
                        string receivermname = datasplit[7].ToString();
                        string ReceiverAddress = datasplit[8].ToString();
                        string receiverphonenum = datasplit[9].ToString();
                        string kptn = datasplit[13].ToString();
                        string dttiled = datasplit[0].ToString();
                        decimal principal = Convert.ToDecimal(datasplit[10].ToString());
                        string currency = Currency;
                        decimal charge = Convert.ToDecimal(datasplit[11].ToString());
                        Decimal Total = Convert.ToDecimal(datasplit[12].ToString());
                        String sessionID = sessionId;
                        string senderfullname = senderfname + " " + sendermname + " " + senderlname;
                        string receiverfullname = receiverfname + " " + receivermname + " " + receiverlname;
                        string OtherDetails = kptn + "|" + dttiled;
                        String AccountCode = AccountId;
                        String OperatorID = operatorID;
                        String StationID = stationID;
                        String BranchCode = bcode;

                        int zc = Convert.ToInt32(zonecode);
                        if (ser.CheckTierCode(AccountCode, principal, Currency).Equals(true) || ser.checkCreditLimit(AccountCode, principal, Currency).Equals(true))
                        {
                            int ret = ser.checkreference(ctrlNo, AccountId);

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                            if (ret == 0)
                            {

                                command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, Currency, Principal," +
                                                      "Charge, Total, AccountCode, TransDate, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverAddress, ReceiverContactNo, " +
                                                      "sessionID, OtherDetails, OperatorID," +
                                                      "StationID, KPTN,BranchCode)" +
                                                      "values(@ControlNo, @RefNumber, @Currency, @Principal, @Charge, " +
                                                      "@Total,@AccountCode, now(), " +
                                                      "@SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                      "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverAddress, @ReceiverContactNo, " +
                                                      "@sessionID, @OtherDetails, @OperatorId, @StationID, @KPTN,@BranchCode);";

                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command.Parameters.AddWithValue("KPTN", kptn);
                                command.Parameters.AddWithValue("RefNumber", ctrlNo);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("Charge", charge);
                                command.Parameters.AddWithValue("Total", Total);
                                command.Parameters.AddWithValue("AccountCode", AccountCode);
                                command.Parameters.AddWithValue("SenderFName", senderfname);
                                command.Parameters.AddWithValue("SenderLName", senderlname);
                                command.Parameters.AddWithValue("SenderMName", sendermname);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverFName", receiverfname);
                                command.Parameters.AddWithValue("ReceiverLName", receiverlname);
                                command.Parameters.AddWithValue("ReceiverMName", receivermname);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("ReceiverAddress", ReceiverAddress);
                                command.Parameters.AddWithValue("ReceiverContactNo", receiverphonenum);
                                command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command.Parameters.AddWithValue("OperatorId", OperatorID);
                                command.Parameters.AddWithValue("StationID", StationID);
                                command.Parameters.AddWithValue("BranchCode", BranchCode);
                                command.Parameters.AddWithValue("sessionID", sessionID);
                                result = command.ExecuteNonQuery();
                                //**********************************************insert transactionlogs***************************************//



                                //**************************************inserting sotxnref table********************************************************//

                                DateTime dt = ser.getServerDatePartner(true);
                                string day = dt.ToString("dd");
                                string m = dt.ToString("MM");
                                string tableRef = "sendout" + m + day;
                                string TDate = dt.ToString("yyyy-MM-dd hh:mm:ss");

                                String sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) values (@xrefno,@xtableRef,@xAccountCode,@xBatchNo,@xTDate,@xCurr,@Ttype) ";
                                command.Parameters.Clear();
                                command.CommandText = sql;
                                command.Parameters.AddWithValue("xrefno", ctrlNo);
                                command.Parameters.AddWithValue("xtableref", tableRef);
                                command.Parameters.AddWithValue("xAccountCode", AccountCode);
                                command.Parameters.AddWithValue("xBatchNo", batchnumber);
                                command.Parameters.AddWithValue("xTDate", TDate);
                                command.Parameters.AddWithValue("xCurr", Currency);
                                command.Parameters.AddWithValue("Ttype", 2);
                                result = command.ExecuteNonQuery();

                                //*******************************************inserting corporatesendouts************************************************//
                                arrCorpoSO[countercorpoSO] = ControlNumber + "|" + kptn + "|" + " " + "|" + ctrlNo + "|" + AccountCode + "|" + currency + "|" + DateTime.Now + "|" + StationID + "|" + "0" + "|" + OperatorID + "|" + BranchCode + "|" + zonecode + "|" + " " + "|" + " " + "|" + "0" + "|" + principal + "|" + " " + "|" + charge + "|0|0|0|" + " " + "|" + sessionId + "|" + senderfname + "|" + senderlname + "|" + sendermname + "|" + receiverfname + "|" + receiverlname + "|" + receivermname + "|" + "1";
                                countercorpoSO = countercorpoSO + 1;

                                if (result.Equals("-1"))
                                {
                                    arrayListofError[countererror] = Dataitem;
                                    countererror = countererror + 1;


                                }
                                else
                                {
                                    arrayListOfSuccess[counterSuccess] = Dataitem;
                                    counterSuccess = counterSuccess + 1;
                                }
                            }
                            else
                            {
                                arrayListofError[countererror] = Dataitem;
                                countererror = countererror + 1;

                            }
                        }
                        else
                        {
                            arrayListofError[countererror] = Dataitem;
                            countererror = countererror + 1;

                        }
                    }

                    if (counterSuccess == Data.Length)
                    {
                        int data = ser.insertData(AccountId, batchnumber, counterSuccess, batchnum);
                        if (data == 1)
                        {

                            foreach (string sItem in arrCorpoSO)
                            {
                                string[] datasplit = sItem.Split('|');

                                string controlno = datasplit[0].ToString();
                                string kptn = datasplit[1].ToString();
                                string oldkptn = datasplit[2].ToString();
                                string referenceno = datasplit[3].ToString();
                                string accountid = datasplit[4].ToString();
                                string currency = datasplit[5].ToString();
                                DateTime transdate = Convert.ToDateTime(datasplit[6].ToString());
                                int stationno = Convert.ToInt16(datasplit[7].ToString());
                                int isremote = Convert.ToInt16(datasplit[8].ToString());
                                string operatorid = datasplit[9].ToString();
                                string branchcode = datasplit[10].ToString();
                                int Zcodecorp = Convert.ToInt16(datasplit[11].ToString());
                                string remoteoperatorid = datasplit[12].ToString();
                                string remotebranchcode = datasplit[13].ToString();
                                int remotezonecode = Convert.ToInt16(datasplit[14].ToString());
                                string principalS = datasplit[15].ToString();
                                decimal principal;
                                if (principalS == string.Empty)
                                {
                                    principal = 0;
                                }
                                else
                                {
                                    principal = Convert.ToDecimal(principalS);
                                }
                                string chargeto = datasplit[16].ToString();
                                string chargeamountS = datasplit[17].ToString();
                                decimal chargeamount;
                                if (chargeamountS == string.Empty)
                                {
                                    chargeamount = 0;
                                }
                                else
                                {
                                    chargeamount = Convert.ToDecimal(chargeamountS);
                                }
                                decimal othercharge = Convert.ToDecimal(datasplit[18].ToString());
                                decimal total = Convert.ToDecimal(datasplit[19].ToString());
                                decimal forex = Convert.ToDecimal(datasplit[20].ToString());
                                string traceno = datasplit[21].ToString();
                                string sessionid = datasplit[22].ToString();
                                string senderfname = datasplit[23].ToString();
                                string senderlname = datasplit[24].ToString();
                                string sendermname = datasplit[25].ToString();
                                string receiverfname = datasplit[26].ToString();
                                string receiverlname = datasplit[27].ToString();
                                string receivermname = datasplit[28].ToString();
                                string SFname = senderfname + " " + sendermname + " " + senderlname;
                                string RFname = receiverfname + " " + receivermname + " " + " " + receiverlname;
                                int isactive = Convert.ToInt16(datasplit[29].ToString());

                                string sql = ser.InsertCorporateTrans(controlno, kptn, oldkptn, referenceno, accountid, currency, transdate, stationno, isremote, operatorid, branchcode, Zcodecorp, remoteoperatorid, remotebranchcode, remotezonecode, principal, chargeto, chargeamount, othercharge, total, forex, traceno, sessionid, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, isactive);
                                command.CommandText = sql;
                                result = command.ExecuteNonQuery();


                                command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno,isremote)" +
                                   " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo,@isremote);";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("kptnnum", kptn);
                                command.Parameters.AddWithValue("refnum", referenceno);
                                command.Parameters.AddWithValue("action", "SENDOUT");
                                command.Parameters.AddWithValue("bcode", branchcode);
                                command.Parameters.AddWithValue("opID", operatorID);
                                command.Parameters.AddWithValue("scode", stationcode);
                                command.Parameters.AddWithValue("sID", stationID);
                                command.Parameters.AddWithValue("zCode", zonecode);
                                command.Parameters.AddWithValue("txndate", transdate);
                                //9command.Parameters.AddWithValue("bname", bname);
                                command.Parameters.AddWithValue("type", "kppartners");
                                command.Parameters.AddWithValue("AccountCode", accountid);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("SenderName", SFname);
                                command.Parameters.AddWithValue("ReceiverName", RFname);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("ControlNo", controlno);
                                command.Parameters.AddWithValue("isremote", 0);
                                result = command.ExecuteNonQuery();


                                if (result.Equals("-1"))
                                {
                                    string errorname = "Error in Saving";
                                    string item = Data[counter].ToString();
                                    arrayListofError[countererror] = item + "|" + errorname;
                                    countererror = countererror + 1;
                                    counterErr += 1;


                                }

                                counter += 1;




                            }
                            if (counterErr == 0)
                            {
                                trans.Commit();
                                con.Close();
                                output = "Success";
                            }
                            else
                            {
                                trans.Rollback();
                                con.Close();
                                output = "Failed";
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            con.Close();
                            output = "Failed";
                        }

                    }
                    else
                    {
                        trans.Rollback();
                        con.Close();
                        return output = "Failed";
                    }


                }

            }
            catch (Exception ex)
            {
                trans.Rollback();
                con.Close();
                return ex.Message;
            }
        }
        return output;
    }
    //****************************************************INSERTION IN BDO PARTNERS**************************************************//        
    public String SaveSendOutTransBDO(String[] Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String sessionID)
    {
        int result;
        string output = string.Empty;
        string errorname = string.Empty;
        String SessionId = sessionID;
        ser.ConnectPartners();
        batchnumber = ser.generateBatchNo();

        using (MySqlConnection con = ser.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    DateTime txdate = ser.getServerDatePartner(true);
                    String pdate = txdate.ToString("MM-dd").Replace("-", "");
                    trans = con.BeginTransaction();
                    command.Transaction = trans;
                    arrayListofError = new String[Data.Length];
                    arrayListOfSuccess = new String[Data.Length];
                    arrCorpoSO = new String[Data.Length];

                    foreach (String dataitem in Data)
                    {
                        String atay = String.Empty;
                        String benefln;
                        String beneffn;
                        String benefmn;
                        String Remfln;
                        String Remffn;
                        String Remfmn;
                        //int r = 2;
                        String peste = String.Empty;
                        String[] datasplit = dataitem.Split('|');

                        String txtref = datasplit[0].ToString();
                        String kptn = datasplit[1].ToString();
                        String transtype = datasplit[2].ToString();
                        benefln = datasplit[3].ToString();
                        beneffn = datasplit[4].ToString();
                        benefmn = datasplit[5].ToString();
                        //String benefullname = benefln + "," + beneffn + " " + benefmn + ".";
                        String benefullname = beneffn + " " + benefmn + " " + benefln;
                        String Remfullname = datasplit[6].ToString();
                        if (Remfullname != "")
                        {
                            String[] RemNamesplit = Remfullname.Split(' ');
                            Remfln = RemNamesplit[0].ToString();
                            Remffn = RemNamesplit[1].ToString();
                            Remfmn = RemNamesplit[2].ToString();
                        }

                        else
                        {
                            Remfln = "";
                            Remffn = "";
                            Remfmn = "";
                        }
                        String prodname = datasplit[7].ToString();
                         decimal principal;
                        string prncpal = datasplit[8].ToString();
                         if(prncpal == string.Empty)
                         {
                             principal = 0;
                         }
                         else
                         {
                             principal = Convert.ToDecimal(prncpal);
                         }
                        String currency = datasplit[9].ToString();
                        String bankbranch = datasplit[10].ToString();
                        String date = datasplit[11].ToString();

                        String ins1 = datasplit[12].ToString();
                        String ins2 = datasplit[13].ToString();
                        String Rf1 = datasplit[14].ToString();
                        String Rf2 = datasplit[15].ToString();
                        String Rf3 = datasplit[16].ToString();
                        String Rf4 = datasplit[17].ToString();
                        String rCount = datasplit[18].ToString();

                        Decimal T_Amount;
                        String totalamount = datasplit[19].ToString();
                        if (totalamount.Trim() == "")
                        {
                            T_Amount = 0;
                        }
                        else
                        {
                            T_Amount = Convert.ToDecimal(totalamount);
                        }
                        String locatorCode = datasplit[21].ToString();
                        String OtherDetails = prodname + "|" + bankbranch + "|" + date + "|" + ins1 + "|" + ins2 + "|" + Rf1 + "|" + Rf2 + "|" + Rf3 + "|" + Rf4 + "|" + date + "|" + rCount + "|" + transtype + "|" + locatorCode;
                       string chrge = datasplit[20].ToString();
                       decimal charge;
                        if (chrge.Trim() == "")
                       {
                           charge = 0;
                       }
                       else
                       {
                           charge = Convert.ToDecimal(chrge);
                       }
                           

                        String AccountCode = AccountId;
                        String OperatorID = operatorID;
                        String StationID = stationID;
                        String BranchCode = bcode;

                        int zc = Convert.ToInt32(zonecode);
                        //if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true) || ser.checkCreditLimit(AccountCode, principal, currency).Equals(true))
                        if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true))
                        {
                            int ret = ser.checkreference(txtref, AccountId);

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                            if (ret == 0)
                            {
                                command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, Currency, Principal, " +
                                                      "Charge, AccountCode, TransDate, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverContactNo, " +
                                                      "sessionID, OtherDetails, OperatorID, " +
                                                      "StationID, KPTN, BranchCode,Total) " +
                                                      "values(@ControlNo, @ReferenceNO, @Currency, @Principal, @Charge, " +
                                                      "@AccountCode, @txndate1, " +
                                                      "@SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                      "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverContactNo, " +
                                                      "@SessionID, @OtherDetails, @OperatorId, @StationID, @KPTN, @Branchcode, @TotalAmount);";


                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("OperatorID", operatorID);
                                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command.Parameters.AddWithValue("ReferenceNO", txtref);
                                //command1.Parameters.AddWithValue("ReferenceNO", txtref);
                                //command1.Parameters.AddWithValue("Principal", amt == String.Empty ? 0 : Convert.ToDecimal(amt));
                                command.Parameters.AddWithValue("AccountCode", AccountId);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("Charge", charge);
                                command.Parameters.AddWithValue("SenderFName", Remffn);
                                command.Parameters.AddWithValue("SenderLName", Remfln);
                                command.Parameters.AddWithValue("SenderMName", Remfmn);
                                command.Parameters.AddWithValue("SenderName", Remfullname);
                                command.Parameters.AddWithValue("ReceiverFName", beneffn);
                                command.Parameters.AddWithValue("ReceiverLName", benefln);
                                command.Parameters.AddWithValue("ReceiverMName", benefmn);
                                command.Parameters.AddWithValue("ReceiverName", benefullname);
                                command.Parameters.AddWithValue("ReceiverContactNo", "");
                                command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command.Parameters.AddWithValue("StationID", stationID);
                                command.Parameters.AddWithValue("SessionID", SessionId);
                                command.Parameters.AddWithValue("KPTN", kptn);
                                command.Parameters.AddWithValue("Branchcode", bcode);
                                command.Parameters.AddWithValue("txndate1", txdate);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("TotalAmount", T_Amount);
                                result = command.ExecuteNonQuery();


                                //**************************************inserting transactionLogs******************************************************//


                                //**************************************inserting sotxnref table********************************************************//

                                DateTime dt = ser.getServerDatePartner(true);
                                string day = dt.ToString("dd");
                                string m = dt.ToString("MM");
                                string tableRef = "sendout" + m + day;
                                string TDate = dt.ToString("yyyy-MM-dd hh:mm:ss");

                                String sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) values (@xrefno,@xtableRef,@xAccountCode,@xBatchNo,@xTDate,@xCurr,@Ttype) ";
                                command.Parameters.Clear();
                                command.CommandText = sql;
                                command.Parameters.AddWithValue("xrefno", txtref);
                                command.Parameters.AddWithValue("xtableref", tableRef);
                                command.Parameters.AddWithValue("xAccountCode", AccountCode);
                                command.Parameters.AddWithValue("xBatchNo", batchnumber);
                                command.Parameters.AddWithValue("xTDate", dt);
                                command.Parameters.AddWithValue("xCurr", currency);
                                command.Parameters.AddWithValue("Ttype", 2);
                                result = command.ExecuteNonQuery();

                                arrCorpoSO[countercorpoSO] = ControlNumber + "|" + kptn + "|" + " " + "|" + txtref + "|" + AccountCode + "|" + currency + "|" + dt + "|" + StationID + "|" + "0" + "|" + OperatorID + "|" + BranchCode + "|" + zonecode + "|" + " " + "|" + " " + "|" + "0" + "|" + principal + "|" + " " + "|" + charge + "|0|0|0|" + " " + "|" + SessionId + "|" + Remffn + "|" + Remfln + "|" + Remfmn + "|" + beneffn + "|" + benefln + "|" + benefmn + "|" + "1";
                                countercorpoSO = countercorpoSO + 1;

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    arrayListofError[countererror] = dataitem + "|" + errorname;
                                    countererror = countererror + 1;


                                }
                                else
                                {
                                    arrayListOfSuccess[counterSuccess] = dataitem;
                                    counterSuccess = counterSuccess + 1;
                                }



                            }
                            else
                            {
                                errorname = "Reference Number Already exist";
                                arrayListofError[countererror] = dataitem + "|" + errorname;
                                countererror = countererror + 1;

                            }
                        }
                        else
                        {
                            errorname = "Amount is Greater than the Threshold";
                            arrayListofError[countererror] = dataitem + "|" + errorname;
                            countererror = countererror + 1;

                        }
                    }
                    if (counterSuccess == Data.Length)
                    {
                        int data = ser.insertData(AccountId, batchnumber, counterSuccess, batchnum);
                        if (data == 1)
                        {

                            foreach (string sItem in arrCorpoSO)
                            {
                                string[] datasplit = sItem.Split('|');

                                string controlno = datasplit[0].ToString();
                                string kptn = datasplit[1].ToString();
                                string oldkptn = datasplit[2].ToString();
                                string referenceno = datasplit[3].ToString();
                                string accountid = datasplit[4].ToString();
                                string currency = datasplit[5].ToString();
                                DateTime transdate = Convert.ToDateTime(datasplit[6].ToString());
                                int stationno = Convert.ToInt16(datasplit[7].ToString());
                                int isremote = Convert.ToInt16(datasplit[8].ToString());
                                string operatorid = datasplit[9].ToString();
                                string branchcode = datasplit[10].ToString();
                                int Zcodecorp = Convert.ToInt16(datasplit[11].ToString());
                                string remoteoperatorid = datasplit[12].ToString();
                                string remotebranchcode = datasplit[13].ToString();
                                int remotezonecode = Convert.ToInt16(datasplit[14].ToString());
                                string principalS = datasplit[15].ToString();
                                decimal principal;
                                if (principalS == "".Trim())
                                {
                                    principal = 0;
                                }
                                else
                                {
                                    principal = Convert.ToDecimal(principalS);
                                }
                                string chargeto = datasplit[16].ToString();
                                string chargeamountS = datasplit[17].ToString();
                                decimal chargeamount;
                                if (chargeamountS == "")
                                {
                                    chargeamount = 0;
                                }
                                else
                                {
                                    chargeamount = Convert.ToDecimal(chargeamountS);
                                }
                                decimal othercharge = Convert.ToDecimal(datasplit[18].ToString());
                                decimal total = Convert.ToDecimal(datasplit[19].ToString());
                                decimal forex = Convert.ToDecimal(datasplit[20].ToString());
                                string traceno = datasplit[21].ToString();
                                string sessionid = datasplit[22].ToString();
                                string senderfname = datasplit[23].ToString();
                                string senderlname = datasplit[24].ToString();
                                string sendermname = datasplit[25].ToString();
                                string receiverfname = datasplit[26].ToString();
                                string receiverlname = datasplit[27].ToString();
                                string receivermname = datasplit[28].ToString();
                                string SFname = senderfname + " " + sendermname + " " + senderlname;
                                string RFname = receiverfname + " " + receivermname + " " + " " + receiverlname;
                                int isactive = Convert.ToInt16(datasplit[29].ToString());

                                string sql = ser.InsertCorporateTrans(controlno, kptn, oldkptn, referenceno, accountid, currency, transdate, stationno, isremote, operatorid, branchcode, Zcodecorp, remoteoperatorid, remotebranchcode, remotezonecode, principal, chargeto, chargeamount, othercharge, total, forex, traceno, sessionid, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, isactive);
                                command.CommandText = sql;
                                result = command.ExecuteNonQuery();


                                command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno,isremote)" +
                                " values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo,@isremote);";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("kptnnum", kptn);
                                command.Parameters.AddWithValue("refnum", referenceno);
                                command.Parameters.AddWithValue("action", "SENDOUT");
                                command.Parameters.AddWithValue("bcode", branchcode);
                                command.Parameters.AddWithValue("opID", operatorID);
                                command.Parameters.AddWithValue("scode", stationcode);
                                command.Parameters.AddWithValue("sID", stationID);
                                command.Parameters.AddWithValue("zCode", zonecode);
                                command.Parameters.AddWithValue("txndate", transdate);
                                //commad.Parameters.AddWithValue("bname", bname);
                                command.Parameters.AddWithValue("type", "kppartners");
                                command.Parameters.AddWithValue("AccountCode", AccountId);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("SenderName", SFname);
                                command.Parameters.AddWithValue("ReceiverName", RFname);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("ControlNo", controlno);
                                command.Parameters.AddWithValue("isremote", 0);

                                result = command.ExecuteNonQuery();

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    string item = Data[counter].ToString();
                                    arrayListofError[countererror] = item + "|" + errorname;
                                    countererror = countererror + 1;
                                    counterErr += 1;


                                }

                                counter += 1;
                            }
                            if (counterErr == 0)
                            {
                                trans.Commit();
                                con.Close();
                                output = "Success";
                            }
                            else
                            {
                                trans.Rollback();
                                con.Close();
                                output = "Failed";
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            con.Close();
                            output = "Failed";
                        }

                    }
                    else
                    {
                        trans.Rollback();
                        con.Close();
                        return output = "Failed";
                    }
                }

            }
            catch (Exception ex)
            {

                output = "error";
                ermsg = ex.Message;
                trans.Rollback();
                con.Close();
                return output;
            }


        }
        return output;
    }
    //*****************************************************Money Exchange Insertion**********************************************************//
    public String SaveSendOutTransMoneyExchanger(String[] Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String sessionID)
    {
        int result;
        string output = string.Empty;
        string errorname = string.Empty;
        String SessionId = sessionID;
        ser.ConnectPartners();
        batchnumber = ser.generateBatchNo();
        using (MySqlConnection con = ser.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");

                    trans = con.BeginTransaction();
                    command.Transaction = trans;
                    arrayListofError = new String[Data.Length];
                    arrayListOfSuccess = new String[Data.Length];
                    arrCorpoSO = new String[Data.Length];

                    foreach (String dataitem in Data)
                    {
                        String[] datasplit = dataitem.Split('|');
                        string ctrlNo = datasplit[0].ToString();//

                        string senderlname = datasplit[4].ToString().Trim();
                        string senderfname = datasplit[3].ToString().Trim();
                        string sendermname = datasplit[5].ToString().Trim();
                        string receiverlname = datasplit[7].ToString().Trim();
                        string receiverfname = datasplit[6].ToString().Trim();
                        string receivermname = datasplit[8].ToString().Trim();
                        string ReceiverAddress = datasplit[9].ToString().Trim();
                        string receiverstreet = datasplit[10].ToString().Trim();
                        string receiverscountry = datasplit[11].ToString().Trim();
                        string receiverphonenum = datasplit[12].ToString().Trim();
                        string kptn = datasplit[31].ToString();
                        decimal principal = Convert.ToDecimal(datasplit[20].ToString());
                        string currency = datasplit[21].ToString().Trim();
                        string receiversTown = datasplit[11].ToString().Trim();
                        decimal charge;
                        string chg = datasplit[30].ToString().Trim();
                        if (chg == string.Empty)
                        {
                            charge = 0;
                        }
                        else
                        {
                            charge = Convert.ToDecimal(datasplit[11].ToString().Trim());
                        }
                        Decimal Total = Convert.ToDecimal(datasplit[20].ToString());
                        string purpose = datasplit[14].ToString();
                        string msg = "";
                        string senderfullname = senderfname + " " + sendermname + " " + senderlname;
                        string receiverfullname = receiverfname + " " + receivermname + " " + receiverlname;
                        string OtherDetails = kptn + "|" + purpose;
                        String AccountCode = AccountId;
                        String OperatorID = operatorID;
                        String StationID = stationID;
                        String BranchCode = bcode;

                        //if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true) || ser.checkCreditLimit(AccountCode, principal, currency).Equals(true))
                        int zc = Convert.ToInt32(zonecode);
                        if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true))
                        {
                            int ret = ser.checkreference(ctrlNo, AccountId);

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                            if (ret == 0)
                            {
                                command.Parameters.Clear();
                                command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo, Currency, Principal," +
                                                      "Charge, Total, AccountCode, TransDate, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverAddress, ReceiverContactNo, " +
                                                      "sessionID, OtherDetails, OperatorID," +
                                                      "StationID, ReceiverStreet, ReceiverCountry, KPTN,Message, BranchCode)" +
                                                      "values(@ControlNo, @RefNumber, @Currency, @Principal, @Charge, " +
                                                      "@Total, @AccountCode, now(), " +
                                                      "@SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                      "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverAddress, @ReceiverContactNo, " +
                                                      "@sessionID, @OtherDetails, @OperatorId, @StationID, @ReceiverStreet, @ReceiverCountry,@KPTN, @Message, @BranchCode);";

                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command.Parameters.AddWithValue("KPTN", kptn);
                                command.Parameters.AddWithValue("RefNumber", ctrlNo);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("Charge", charge);
                                command.Parameters.AddWithValue("Total", Total);
                                command.Parameters.AddWithValue("AccountCode", AccountCode);
                                command.Parameters.AddWithValue("SenderFName", senderfname);
                                command.Parameters.AddWithValue("SenderLName", senderlname);
                                command.Parameters.AddWithValue("SenderMName", sendermname);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverFName", receiverfname);
                                command.Parameters.AddWithValue("ReceiverLName", receiverlname);
                                command.Parameters.AddWithValue("ReceiverMName", receivermname);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("ReceiverAddress", ReceiverAddress);
                                command.Parameters.AddWithValue("ReceiverContactNo", receiverphonenum);
                                command.Parameters.AddWithValue("sessionId", SessionId);
                                command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command.Parameters.AddWithValue("OperatorId", OperatorID);
                                command.Parameters.AddWithValue("StationID", StationID);
                                command.Parameters.AddWithValue("ReceiverStreet", receiverstreet);
                                command.Parameters.AddWithValue("ReceiverCountry", receiverscountry);
                                command.Parameters.AddWithValue("Message", msg);
                                command.Parameters.AddWithValue("BranchCode", BranchCode);

                                result = command.ExecuteNonQuery();

                                //******************************************************insert transactionlogs****************************************************//



                                //**************************************inserting sotxnref table********************************************************//

                                DateTime dt = ser.getServerDatePartner(true);
                                string day = dt.ToString("dd");
                                string m = dt.ToString("MM");
                                string tableRef = "sendout" + m + day;
                                string TDate = dt.ToString("yyyy-MM-dd hh:mm:ss");

                                String sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) values (@xrefno,@xtableRef,@xAccountCode,@xBatchNo,@xTDate,@xCurr,@Ttype) ";
                                command.Parameters.Clear();
                                command.CommandText = sql;
                                command.Parameters.AddWithValue("xrefno", ctrlNo);
                                command.Parameters.AddWithValue("xtableref", tableRef);
                                command.Parameters.AddWithValue("xAccountCode", AccountCode);
                                command.Parameters.AddWithValue("xBatchNo", batchnumber);
                                command.Parameters.AddWithValue("xTDate", dt);
                                command.Parameters.AddWithValue("xCurr", currency);
                                command.Parameters.AddWithValue("Ttype", 2);
                                result = command.ExecuteNonQuery();

                                arrCorpoSO[countercorpoSO] = ControlNumber + "|" + kptn + "|" + " " + "|" + ctrlNo + "|" + AccountCode + "|" + currency + "|" + dt + "|" + StationID + "|" + "0" + "|" + OperatorID + "|" + BranchCode + "|" + zonecode + "|" + " " + "|" + " " + "|" + "0" + "|" + principal + "|" + " " + "|" + charge + "|0|0|0|" + " " + "|" + SessionId + "|" + "" + "|" + "" + "|" + "" + "|" + receiverfname + "|" + receiverlname + "|" + receivermname + "|" + "1";
                                countercorpoSO = countercorpoSO + 1;

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    arrayListofError[countererror] = dataitem + "|" + errorname;
                                    countererror = countererror + 1;


                                }
                                else
                                {
                                    arrayListOfSuccess[counterSuccess] = dataitem;
                                    counterSuccess = counterSuccess + 1;
                                }

                            }
                            else
                            {
                                errorname = "Reference Number Already Exist";
                                arrayListofError[countererror] = dataitem + "|" + errorname;
                                countererror = countererror + 1;

                            }
                        }
                        else
                        {
                            errorname = "Amount is Greater than the Threshold";
                            arrayListofError[countererror] = dataitem + "|" + errorname;
                            countererror = countererror + 1;

                        }
                    }
                    if (counterSuccess == Data.Length)
                    {
                        int data = ser.insertData(AccountId, batchnumber, counterSuccess, batchnum);
                        if (data == 1)
                        {

                            foreach (string sItem in arrCorpoSO)
                            {
                                string[] datasplit = sItem.Split('|');

                                string controlno = datasplit[0].ToString();
                                string kptn = datasplit[1].ToString();
                                string oldkptn = datasplit[2].ToString();
                                string referenceno = datasplit[3].ToString();
                                string accountid = datasplit[4].ToString();
                                string currency = datasplit[5].ToString();
                                DateTime transdate = Convert.ToDateTime(datasplit[6].ToString());
                                int stationno = Convert.ToInt16(datasplit[7].ToString());
                                int isremote = Convert.ToInt16(datasplit[8].ToString());
                                string operatorid = datasplit[9].ToString();
                                string branchcode = datasplit[10].ToString();
                                int Zcodecorp = Convert.ToInt16(datasplit[11].ToString());
                                string remoteoperatorid = datasplit[12].ToString();
                                string remotebranchcode = datasplit[13].ToString();
                                int remotezonecode = Convert.ToInt16(datasplit[14].ToString());
                                string principalS = datasplit[15].ToString();
                                decimal principal;
                                if (principalS == string.Empty)
                                {
                                    principal = 0;
                                }
                                else
                                {
                                    principal = Convert.ToDecimal(principalS);
                                }
                                string chargeto = datasplit[16].ToString();
                                string chargeamountS = datasplit[17].ToString();
                                decimal chargeamount;
                                if (chargeamountS == string.Empty)
                                {
                                    chargeamount = 0;
                                }
                                else
                                {
                                    chargeamount = Convert.ToDecimal(chargeamountS);
                                }
                                decimal othercharge = Convert.ToDecimal(datasplit[18].ToString());
                                decimal total = Convert.ToDecimal(datasplit[19].ToString());
                                decimal forex = Convert.ToDecimal(datasplit[20].ToString());
                                string traceno = datasplit[21].ToString();
                                string sessionid = datasplit[22].ToString();
                                string senderfname = datasplit[23].ToString();
                                string senderlname = datasplit[24].ToString();
                                string sendermname = datasplit[25].ToString();
                                string receiverfname = datasplit[26].ToString();
                                string receiverlname = datasplit[27].ToString();
                                string receivermname = datasplit[28].ToString();
                                string SFname = senderfname + " " + sendermname + " " + " " + senderlname;
                                string RFname = receiverfname + " " + receivermname + " " + " " + receiverlname;
                                int isactive = Convert.ToInt16(datasplit[29].ToString());

                                string sql = ser.InsertCorporateTrans(controlno, kptn, oldkptn, referenceno, accountid, currency, transdate, stationno, isremote, operatorid, branchcode, Zcodecorp, remoteoperatorid, remotebranchcode, remotezonecode, principal, chargeto, chargeamount, othercharge, total, forex, traceno, sessionid, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, isactive);
                                command.CommandText = sql;
                                result = command.ExecuteNonQuery();


                                command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode," +
                                                     "action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate," +
                                                     "currency,sendername,receivername,principal,controlno,isremote) values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,"
                                                     + "@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo,@isremote);";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("kptnnum", kptn);
                                command.Parameters.AddWithValue("refnum", referenceno);
                                command.Parameters.AddWithValue("action", "SENDOUT");
                                command.Parameters.AddWithValue("bcode", branchcode);
                                command.Parameters.AddWithValue("opID", operatorID);
                                command.Parameters.AddWithValue("scode", stationcode);
                                command.Parameters.AddWithValue("sID", stationID);
                                command.Parameters.AddWithValue("zCode", zonecode);
                                command.Parameters.AddWithValue("txndate", transdate);
                                //9command.Parameters.AddWithValue("bname", bname);
                                command.Parameters.AddWithValue("type", "kppartners");
                                command.Parameters.AddWithValue("AccountCode", accountid);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("SenderName", SFname);
                                command.Parameters.AddWithValue("ReceiverName", RFname);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("ControlNo", controlno);
                                command.Parameters.AddWithValue("isremote", 0);
                                result = command.ExecuteNonQuery();

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    string item = Data[counter].ToString();
                                    arrayListofError[countererror] = item + "|" + errorname;
                                    countererror = countererror + 1;
                                    counterErr += 1;


                                }

                                counter += 1;



                            }
                            if (counterErr == 0)
                            {
                                trans.Commit();
                                con.Close();
                                output = "Success";
                            }
                            else
                            {
                                trans.Rollback();
                                con.Close();
                                output = "Failed";
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            con.Close();
                            output = "Failed";
                        }

                    }
                    else
                    {
                        trans.Rollback();
                        con.Close();
                        return output = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                output = "error";
                ermsg = ex.Message;
                trans.Rollback();
                con.Close();
                return output;
            }


        }
        return output;
    }
    //***************************************************Insert BPI**************************************************//

    public String SaveSendOutTransBPI(String[] Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String sessionID)
    {
        int result;
        string output = string.Empty;
        string errorname = string.Empty;
        String SessionId = sessionID;
        ser.ConnectPartners();
        batchnumber = ser.generateBatchNo();

        using (MySqlConnection con = ser.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    DateTime txndate = ser.getServerDatePartner(true);
                    String pdate = txndate.ToString("MM-dd").Replace("-", "");

                    trans = con.BeginTransaction();
                    command.Transaction = trans;
                    arrayListofError = new String[Data.Length];
                    arrayListOfSuccess = new String[Data.Length];
                    arrCorpoSO = new String[Data.Length];

                    foreach (string dataitem in Data)
                    {

                        String[] datasplit = dataitem.Split('|');

                        string txnappnum = datasplit[2].ToString();
                        string kptn = datasplit[1].ToString();
                        string txtref = datasplit[0].ToString();
                        string arnum = datasplit[3].ToString();
                        string txndistdate = datasplit[4].ToString();
                        string settlement = datasplit[5].ToString();
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
                        string benefcurr = datasplit[22].ToString();
                        string benefnetproceeds = datasplit[23].ToString();
                        string senderfullname = remitrfn + " " + remitrmn + " " + remitrln;
                        string receiverfullname = beneffn + " " + benefmn + " " + benefln;
                        string OtherDetails = txnappnum + "|" + kptn + "|" + txndistdate;
                        String AccountCode = AccountId;
                        String OperatorID = operatorID;
                        String StationID = stationID;
                        String BranchCode = bcode;
                        decimal charge = Convert.ToDecimal(datasplit[24].ToString());
                        int gndr = 3;


                        int zc = Convert.ToInt32(zonecode);

                        //if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true) || ser.checkCreditLimit(AccountCode, principal, currency).Equals(true))
                        decimal principal = Convert.ToDecimal(benefnetproceeds);
                        if (ser.CheckTierCode(AccountCode, principal, benefcurr).Equals(true))
                        {
                            int ret = ser.checkreference(txtref, AccountId);

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

                            if (ret == 0)
                            {
                                command.Parameters.Clear();
                                command.CommandText = "Insert into kppartners.sendout" + pdate + " (ControlNo, ReferenceNo,Currency, Principal," +
                                                      "Charge, AccountCode, TransDate, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverGender, ReceiverContactNo, " +
                                                      "SenderGender, sessionID, OtherDetails, OperatorID," +
                                                      "StationID, KPTN,Message, BranchCode)" +
                                                      "values(@ControlNo, @RefNumber, @Currency, @Principal, @Charge, " +
                                                      "@AccountCode, @txndate, " +
                                                      "@SenderFName, @SenderLName, @SenderMName, @SenderName, @ReceiverFName, " +
                                                      "@ReceiverLName, @ReceiverMName, @ReceiverName, @ReceiverGender, @ReceiverContactNo, " +
                                                      "@SenderGender, " +
                                                      "@sessionID, @OtherDetails, @OperatorId, @StationID, @KPTN, @Message,@BranchCode);";

                                command.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command.Parameters.AddWithValue("KPTN", kptn);
                                command.Parameters.AddWithValue("RefNumber", txtref);
                                command.Parameters.AddWithValue("Currency", benefcurr);
                                command.Parameters.AddWithValue("Principal", benefnetproceeds);
                                command.Parameters.AddWithValue("Charge", charge);
                                command.Parameters.AddWithValue("AccountCode", AccountCode);
                                command.Parameters.AddWithValue("SenderFName", remitrfn);
                                command.Parameters.AddWithValue("SenderLName", remitrln);
                                command.Parameters.AddWithValue("SenderMName", remitrmn);
                                command.Parameters.AddWithValue("SenderName", senderfullname);
                                command.Parameters.AddWithValue("ReceiverFName", beneffn);
                                command.Parameters.AddWithValue("ReceiverLName", benefln);
                                command.Parameters.AddWithValue("ReceiverMName", benefmn);
                                command.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command.Parameters.AddWithValue("ReceiverGender", gndr);
                                command.Parameters.AddWithValue("ReceiverContactNo", benefphonenum);
                                command.Parameters.AddWithValue("SenderGender", gndr);
                                command.Parameters.AddWithValue("sessionId", SessionId);
                                command.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command.Parameters.AddWithValue("OperatorId", OperatorID);
                                command.Parameters.AddWithValue("StationID", StationID);
                                command.Parameters.AddWithValue("Message", msgtorecepient);
                                command.Parameters.AddWithValue("BranchCode", BranchCode);
                                command.Parameters.AddWithValue("txndate", txndate);
                                result = command.ExecuteNonQuery();


                                int stID = Convert.ToInt16(stationID);
                                int zcode = Convert.ToInt16(zonecode);



                                DateTime dt = ser.getServerDatePartner(true);
                                string day = dt.ToString("dd");
                                string m = dt.ToString("MM");
                                string tableRef = "sendout" + m + day;
                                string TDate = dt.ToString("yyyy-MM-dd hh:mm:ss");

                                String sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) values (@xrefno,@xtableRef,@xAccountCode,@xBatchNo,@xTDate,@xCurr,@Ttype) ";
                                command.Parameters.Clear();
                                command.CommandText = sql;
                                command.Parameters.AddWithValue("xrefno", txtref);
                                command.Parameters.AddWithValue("xtableref", tableRef);
                                command.Parameters.AddWithValue("xAccountCode", AccountCode);
                                command.Parameters.AddWithValue("xBatchNo", batchnumber);
                                command.Parameters.AddWithValue("xTDate", dt);
                                command.Parameters.AddWithValue("xCurr", benefcurr);
                                command.Parameters.AddWithValue("Ttype", 2);
                                result = command.ExecuteNonQuery();

                                arrCorpoSO[countercorpoSO] = ControlNumber + "|" + kptn + "|" + " " + "|" + txtref + "|" + AccountCode + "|" + benefcurr + "|" + dt + "|" + StationID + "|" + "0" + "|" + OperatorID + "|" + BranchCode + "|" + zonecode + "|" + " " + "|" + " " + "|" + "0" + "|" + principal + "|" + " " + "|" + charge + "|0|0|0|" + " " + "|" + SessionId + "|" + remitrfn + "|" + remitrln + "|" + remitrmn + "|" + beneffn + "|" + benefln + "|" + benefmn + "|" + "1";
                                countercorpoSO = countercorpoSO + 1;

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    arrayListofError[countererror] = dataitem + "|" + errorname;
                                    countererror = countererror + 1;

                                    

                                }
                                else
                                {
                                    arrayListOfSuccess[counterSuccess] = dataitem;
                                    counterSuccess = counterSuccess + 1;
                                }


                            }
                            else
                            {
                                errorname = "Reference Number Already exist";
                                arrayListofError[countererror] = dataitem + "|" + errorname;
                                countererror = countererror + 1;
                               
                            }

                        }
                        else
                        {
                            errorname = "Amount is Greater than the Threshold";
                            arrayListofError[countererror] = dataitem + "|" + errorname;
                            countererror = countererror + 1;
                           
                        }
                    }//
                    if (counterSuccess == Data.Length)
                    {
                        int data = ser.insertData(AccountId, batchnumber, counterSuccess, batchnum);
                        if (data == 1)
                        {

                            foreach (string sItem in arrCorpoSO)
                            {
                                string[] datasplit = sItem.Split('|');

                                string controlno = datasplit[0].ToString();
                                string kptn = datasplit[1].ToString();
                                string oldkptn = datasplit[2].ToString();
                                string referenceno = datasplit[3].ToString();
                                string accountid = datasplit[4].ToString();
                                string currency = datasplit[5].ToString();
                                DateTime transdate = Convert.ToDateTime(datasplit[6].ToString());
                                int stationno = Convert.ToInt16(datasplit[7].ToString());
                                int isremote = Convert.ToInt16(datasplit[8].ToString());
                                string operatorid = datasplit[9].ToString();
                                string branchcode = datasplit[10].ToString();
                                int Zcodecorp = Convert.ToInt16(datasplit[11].ToString());
                                string remoteoperatorid = datasplit[12].ToString();
                                string remotebranchcode = datasplit[13].ToString();
                                int remotezonecode = Convert.ToInt16(datasplit[14].ToString());
                                string principalS = datasplit[15].ToString();
                                decimal principal;
                                if (principalS == string.Empty)
                                {
                                    principal = 0;
                                }
                                else
                                {
                                    principal = Convert.ToDecimal(principalS);
                                }
                                string chargeto = datasplit[16].ToString();
                                string chargeamountS = datasplit[17].ToString();
                                decimal chargeamount;
                                if (chargeamountS == string.Empty)
                                {
                                    chargeamount = 0;
                                }
                                else
                                {
                                    chargeamount = Convert.ToDecimal(chargeamountS);
                                }
                                decimal othercharge = Convert.ToDecimal(datasplit[18].ToString());
                                decimal total = Convert.ToDecimal(datasplit[19].ToString());
                                decimal forex = Convert.ToDecimal(datasplit[20].ToString());
                                string traceno = datasplit[21].ToString();
                                string sessionid = datasplit[22].ToString();
                                string senderfname = datasplit[23].ToString();
                                string senderlname = datasplit[24].ToString();
                                string sendermname = datasplit[25].ToString();
                                string receiverfname = datasplit[26].ToString();
                                string receiverlname = datasplit[27].ToString();
                                string receivermname = datasplit[28].ToString();
                                string SFname = senderfname + " " + sendermname + " " + " " + senderlname;
                                string RFname = receiverfname + " " + receivermname + " " + " " + receiverlname;
                                int isactive = Convert.ToInt16(datasplit[29].ToString());

                                string sql = ser.InsertCorporateTrans(controlno, kptn, oldkptn, referenceno, accountid, currency, transdate, stationno, isremote, operatorid, branchcode, Zcodecorp, remoteoperatorid, remotebranchcode, remotezonecode, principal, chargeto, chargeamount, othercharge, total, forex, traceno, sessionid, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, isactive);
                                
                                command.CommandText = sql;
                                result = command.ExecuteNonQuery();


                                command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode," +
                                                     "action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate," +
                                                     "currency,sendername,receivername,principal,controlno,isremote) values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,"
                                                     + "@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo,@isremote);";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("kptnnum", kptn);
                                command.Parameters.AddWithValue("refnum", referenceno);
                                command.Parameters.AddWithValue("action", "SENDOUT");
                                command.Parameters.AddWithValue("bcode", branchcode);
                                command.Parameters.AddWithValue("opID", operatorID);
                                command.Parameters.AddWithValue("scode", stationcode);
                                command.Parameters.AddWithValue("sID", stationID);
                                command.Parameters.AddWithValue("zCode", zonecode);
                                command.Parameters.AddWithValue("txndate", transdate);
                                //9command.Parameters.AddWithValue("bname", bname);
                                command.Parameters.AddWithValue("type", "kppartners");
                                command.Parameters.AddWithValue("AccountCode", accountid);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("SenderName", SFname);
                                command.Parameters.AddWithValue("ReceiverName", RFname);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("ControlNo", controlno);
                                command.Parameters.AddWithValue("isremote", 0);
                                result = command.ExecuteNonQuery();

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    string item = Data[counter].ToString();
                                    arrayListofError[countererror] = item + "|" + errorname;
                                    countererror = countererror + 1;
                                    counterErr += 1;


                                }

                                counter += 1;



                            }
                            if (counterErr == 0)
                            {
                                trans.Commit();
                                con.Close();
                                output = "Success";
                            }
                            else
                            {
                                trans.Rollback();
                                con.Close();
                                output = "Failed";
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            con.Close();
                            output = "Failed";
                        }

                    }
                    else
                    {
                        trans.Rollback();
                        con.Close();
                        return output = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                output = "error";
                ermsg = ex.Message;
                trans.Rollback();
                con.Close();
                return output;
            }


        }
        return output;
    }

    public String SaveSendOutTransDBP(String[] Data, String AccountId, String batchnum, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId)
    {
        int result;
        string output = string.Empty;
        string errorname = string.Empty;
        String sessionId = SessionId;
        ser.ConnectPartners();
        batchnumber = ser.generateBatchNo();

        using (MySqlConnection con = ser.dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    String atay = String.Empty;
                    String peste = String.Empty;
                    DateTime txndate = ser.getServerDatePartner(true);
                    String pdate = txndate.ToString("MM-dd").Replace("-", "");
                    trans = con.BeginTransaction();
                    command.Transaction = trans;
                    arrayListofError = new String[Data.Length];
                    arrayListOfSuccess = new String[Data.Length];
                    arrCorpoSO = new String[Data.Length];
                    foreach (string dataitem in Data)
                    {
                        String[] datasplit = dataitem.Split('|');

                        string txtref = datasplit[2].ToString().Trim();
                        string kptn = datasplit[1].ToString().Trim();
                        string appnum = datasplit[0].ToString().Trim();
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
                        decimal amt;
                        if (pamnt == " ")
                        {
                            amt = 0;
                        }
                        else
                        {
                            amt = Convert.ToDecimal(pamnt);
                        }
                        
                        string senderfullname = datasplit[11].ToString().Trim();
                        String[] dsplit1 = datasplit[11].Split(' ');
                        decimal charge = Convert.ToDecimal(datasplit[15].ToString());
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
                        String irnum = "";
                        String curr = Currency;
                        Decimal chrge = charge;
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
                        int zc = Convert.ToInt32(zonecode);
                        //if (ser.CheckTierCode(AccountCode, principal, currency).Equals(true) || ser.checkCreditLimit(AccountCode, principal, currency).Equals(true))
                        if (ser.CheckTierCode(AccountId, amt, Currency).Equals(true))
                        {
                            int ret = ser.checkreference(txtref, AccountId);

                            string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);

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
                                                      "@OperatorID, @Charge, @OtherCharge, @Total, @CancelledDate, @AccountCode, @CancelledbyOperatorID, " +
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
                                command1.Parameters.AddWithValue("ControlNo", ControlNumber);
                                command1.Parameters.AddWithValue("ReferenceNO", txtref);
                                command1.Parameters.AddWithValue("Principal", amt);
                                command1.Parameters.AddWithValue("AccountCode", AccountId);
                                command1.Parameters.AddWithValue("SenderFName", remitrfn);
                                command1.Parameters.AddWithValue("SenderLName", remitrln);
                                command1.Parameters.AddWithValue("SenderMName", remitrmn);
                                command1.Parameters.AddWithValue("SenderName", senderfullname);
                                command1.Parameters.AddWithValue("ReceiverFName", beneffn);
                                command1.Parameters.AddWithValue("ReceiverLName", benefln);
                                command1.Parameters.AddWithValue("ReceiverMName", benefmn);
                                command1.Parameters.AddWithValue("ReceiverName", receiverfullname);
                                command1.Parameters.AddWithValue("ReceiverContactNo", benefcntactlndline + "/" + benefmobilenum);
                                command1.Parameters.AddWithValue("OtherDetails", OtherDetails);
                                command1.Parameters.AddWithValue("StationID", stationID);
                                command1.Parameters.AddWithValue("KPTN", kptn);
                                command1.Parameters.AddWithValue("Message", msg);
                                command1.Parameters.AddWithValue("Branchcode", bcode);
                                command1.Parameters.AddWithValue("txndate", txndate);
                                result = command1.ExecuteNonQuery();



                                int stID = Convert.ToInt16(stationID);
                                int zcode = Convert.ToInt16(zonecode);
                                decimal Principal = Convert.ToDecimal(amt);

                                DateTime dt = ser.getServerDatePartner(true);
                                string day = dt.ToString("dd");
                                string m = dt.ToString("MM");
                                string tableRef = "sendout" + m + day;
                                string TDate = dt.ToString("yyyy-MM-dd hh:mm:ss");

                                String sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) values (@xrefno,@xtableRef,@xAccountCode,@xBatchNo,@xTDate,@xCurr,@Ttype) ";
                                command.Parameters.Clear();
                                command.CommandText = sql;
                                command.Parameters.AddWithValue("xrefno", txtref);
                                command.Parameters.AddWithValue("xtableref", tableRef);
                                command.Parameters.AddWithValue("xAccountCode", AccountId);
                                command.Parameters.AddWithValue("xBatchNo", batchnumber);
                                command.Parameters.AddWithValue("xTDate", dt);
                                command.Parameters.AddWithValue("xCurr", curr);
                                command.Parameters.AddWithValue("Ttype", 2);
                                result = command.ExecuteNonQuery();

                                arrCorpoSO[countercorpoSO] = ControlNumber + "|" + kptn + "|" + " " + "|" + txtref + "|" + AccountId + "|" + curr + "|" + dt + "|" + stationID + "|" + "0" + "|" + operatorID + "|" + bcode + "|" + zonecode + "|" + " " + "|" + " " + "|" + "0" + "|" + amt + "|" + " " + "|" + charge + "|0|0|0|" + " " + "|" + SessionId + "|" + remitrfn + "|" + remitrln + "|" + remitrmn + "|" + beneffn + "|" + benefln + "|" + benefmn + "|" + "1";
                                countercorpoSO = countercorpoSO + 1;

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    arrayListofError[countererror] = dataitem + "|" + errorname;
                                    countererror = countererror + 1;


                                }
                                else
                                {
                                    arrayListOfSuccess[counterSuccess] = dataitem;
                                    counterSuccess = counterSuccess + 1;
                                }



                            }
                            else
                            {
                                errorname = "Reference Number Already Exist";
                                arrayListofError[countererror] = dataitem + "|" + errorname;
                                countererror = countererror + 1;
                            }
                        }
                        else
                        {
                            errorname = "Amount is Greater than the Threshold";
                            arrayListofError[countererror] = dataitem + "|" + errorname;
                            countererror = countererror + 1;
                        }
                    }
                    //
                    if (counterSuccess == Data.Length)
                    {
                        int data = ser.insertData(AccountId, batchnumber, counterSuccess, batchnum);
                        if (data == 1)
                        {

                            foreach (string sItem in arrCorpoSO)
                            {
                                string[] datasplit = sItem.Split('|');

                                string controlno = datasplit[0].ToString();
                                string kptn = datasplit[1].ToString();
                                string oldkptn = datasplit[2].ToString();
                                string referenceno = datasplit[3].ToString();
                                string accountid = datasplit[4].ToString();
                                string currency = datasplit[5].ToString();
                                DateTime transdate = Convert.ToDateTime(datasplit[6].ToString());
                                int stationno = Convert.ToInt16(datasplit[7].ToString());
                                int isremote = Convert.ToInt16(datasplit[8].ToString());
                                string operatorid = datasplit[9].ToString();
                                string branchcode = datasplit[10].ToString();
                                int Zcodecorp = Convert.ToInt16(datasplit[11].ToString());
                                string remoteoperatorid = datasplit[12].ToString();
                                string remotebranchcode = datasplit[13].ToString();
                                int remotezonecode = Convert.ToInt16(datasplit[14].ToString());
                                string principalS = datasplit[15].ToString();
                                decimal principal;
                                if (principalS == string.Empty)
                                {
                                    principal = 0;
                                }
                                else
                                {
                                    principal = Convert.ToDecimal(principalS);
                                }
                                string chargeto = datasplit[16].ToString();
                                string chargeamountS = datasplit[17].ToString();
                                decimal chargeamount;
                                if (chargeamountS == string.Empty)
                                {
                                    chargeamount = 0;
                                }
                                else
                                {
                                    chargeamount = Convert.ToDecimal(chargeamountS);
                                }
                                decimal othercharge = Convert.ToDecimal(datasplit[18].ToString());
                                decimal total = Convert.ToDecimal(datasplit[19].ToString());
                                decimal forex = Convert.ToDecimal(datasplit[20].ToString());
                                string traceno = datasplit[21].ToString();
                                string sessionid = datasplit[22].ToString();
                                string senderfname = datasplit[23].ToString();
                                string senderlname = datasplit[24].ToString();
                                string sendermname = datasplit[25].ToString();
                                string receiverfname = datasplit[26].ToString();
                                string receiverlname = datasplit[27].ToString();
                                string receivermname = datasplit[28].ToString();
                                string SFname = senderfname + " " + sendermname + ". " + " " + senderlname;
                                string RFname = receiverfname + " " + receivermname + ". " + " " + receiverlname;
                                int isactive = Convert.ToInt16(datasplit[29].ToString());

                                string sql = ser.InsertCorporateTrans(controlno, kptn, oldkptn, referenceno, accountid, currency, transdate, stationno, isremote, operatorid, branchcode, Zcodecorp, remoteoperatorid, remotebranchcode, remotezonecode, principal, chargeto, chargeamount, othercharge, total, forex, traceno, sessionid, senderfname, senderlname, sendermname, receiverfname, receiverlname, receivermname, isactive);
                                command.CommandText = sql;
                                result = command.ExecuteNonQuery();


                                command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode," +
                                                     "action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate," +
                                                     "currency,sendername,receivername,principal,controlno,isremote) values(@kptnnum,@refnum,@AccountCode,@action,@type,@bcode,@opID,"
                                                     + "@scode,@zCode,@sID,@txndate,@Currency,@SenderName,@ReceiverName,@Principal,@ControlNo,@isremote);";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("kptnnum", kptn);
                                command.Parameters.AddWithValue("refnum", referenceno);
                                command.Parameters.AddWithValue("action", "SENDOUT");
                                command.Parameters.AddWithValue("bcode", branchcode);
                                command.Parameters.AddWithValue("opID", operatorID);
                                command.Parameters.AddWithValue("scode", stationcode);
                                command.Parameters.AddWithValue("sID", stationID);
                                command.Parameters.AddWithValue("zCode", zonecode);
                                command.Parameters.AddWithValue("txndate", transdate);
                                //9command.Parameters.AddWithValue("bname", bname);
                                command.Parameters.AddWithValue("type", "kppartners");
                                command.Parameters.AddWithValue("AccountCode", accountid);
                                command.Parameters.AddWithValue("Currency", currency);
                                command.Parameters.AddWithValue("SenderName", SFname);
                                command.Parameters.AddWithValue("ReceiverName", RFname);
                                command.Parameters.AddWithValue("Principal", principal);
                                command.Parameters.AddWithValue("ControlNo", controlno);
                                command.Parameters.AddWithValue("isremote", 0);
                                result = command.ExecuteNonQuery();

                                if (result.Equals("-1"))
                                {
                                    errorname = "Error in Saving";
                                    string item = Data[counter].ToString();
                                    arrayListofError[countererror] = item + "|" + errorname;
                                    countererror = countererror + 1;
                                    counterErr += 1;


                                }

                                counter += 1;



                            }
                            if (counterErr == 0)
                            {
                                trans.Commit();
                                con.Close();
                                output = "Success";
                            }
                            else
                            {
                                trans.Rollback();
                                con.Close();
                                output = "Failed";
                            }
                        }
                        else
                        {
                            trans.Rollback();
                            con.Close();
                            output = "Failed";
                        }

                    }
                    else
                    {
                        trans.Rollback();
                        con.Close();
                        return output = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                output = "error";
                ermsg = ex.Message;
                trans.Rollback();
                con.Close();
                return output;
            }


        }

        return output;
      
    }
    
    
}

