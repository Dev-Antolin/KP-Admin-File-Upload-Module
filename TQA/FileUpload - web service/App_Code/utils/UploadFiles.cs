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
using System.IO;
using MySql.Data.MySqlClient;
using log4net;
using System.Collections.Generic;
/// <summary>
/// Summary description for UploadFiles
/// </summary>
public class UploadFiles
{
    string[] column;
    int numdata = 1000;
    string File = "";
    string filename = "";
    string tinuodngaFilename = "";
    string mappath = AppDomain.CurrentDomain.BaseDirectory;
    datatrappings ser = new datatrappings();
    public MySqlTransaction trans = null;
    public MySqlCommand command;
    string MLbatchNum = "";
    string errorname = "";
    public List<string> ListSuccess = new List<string>();
    public List<string> ListError = new List<string>();
    public String ermsg = "";
    int counterError = 0;
    int counterSuccess = 0;
    int arraycount = 0;
    //int counter = 0;
    int arraytotal = 0;
    //string temptableP2P = "kppartners.tmpp2ptable";
    //FOR Insert In SoUploadHeader Table
    public string BatchNo { get; set; }
    public string AccountCode { get; set; }
    public int NoOfTrans { get; set; }
    public string UploadDate { get; set; }
    public string CorporateBatchNo { get; set; }
    public string MLbNumber = "";
    
    #region "DBP"

    private int InsertDBPTable365(ref MySqlConnection conn, string sessionID, string temptable)
    {
        int i = 0;
        //int DeleteTmpTbl = 0;
        int soUploadInserted = 0;
        int CorporateInserted = 0;
        int TransLogInserted = 0;
        int soTransRefInserted = 0;
        bool UpdateTmpTbl = false;
        using (command = conn.CreateCommand())
        {
            DateTime serverdate;
            string table;
            command.CommandText = "Select NOW() as serverdt;";
            using (MySqlDataReader Reader = command.ExecuteReader())
            {
                Reader.Read();
                serverdate = Convert.ToDateTime(Reader["serverdt"]);
                Reader.Close();

            }

            table = serverdate.ToString("MM-dd").Replace("-", "");

            //SELECT Fields under Temp Table                   
            string sql = "INSERT INTO kppartners.sendout" + table + " (ControlNo, ReferenceNo, Principal," +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, OtherDetails, StationID, KPTN, Message, BranchCode, ReceiverContactNo, Transdate,IRNo, "+
                                                      "Currency, OperatorID, Charge, OtherCharge, Total, CancelledDate, AccountCode, CancelledByOperatorID," +
                                                      "CancelledByBranchCode, CancelledByZoneCode, CancelledByStationID, CancelReason, CancelDetails, " +
                                                      "ReceiverAddress, ReceiverGender, ReceiverBirthDate, CancelCharge,ChargeTo, Forex, Traceno, SenderStreet, SenderGender,SenderContactNo, sessionID, " +
                                                      "ReceiverProvince, ReceiverCountry, Redeem, SenderBirthdate) " +
                          "Select ControlNumber,refno,principal,substring_index(SenderName,' ',1) as SenderFName,substring_index(substring_index(SenderName,' ',3),' ',-1) as SenderLName,"+
                          "substring_index(substring_index(SenderName,' ',2),' ',-1) as SenderMName,SenderName,substring_index(ReceiverName,' ',1) as ReceiverFName,substring_index(substring_index(ReceiverName,' ',3),' ',-1) as ReceiverLName, " +
                          "substring_index(substring_index(ReceiverName,' ',2),' ',-1) as ReceiverMName,ReceiverName,CONCAT(appnum ,'|', ReceiverAddress ,'|',  ReceiverAddress) AS OtherDetail, " +
                          "StationId,kptn,Message, BranchCode,CONCAT(ReceiverLandline,'/',ReceiverMobile) AS ReceiverNo,NOW(),'',Currency,OperatorId,Charge, "+
                          "'','','',AccountId,'','','','','','',ReceiverAddress,'','','','','','','','','',SessionId,'','','',''"+
                          "FROM " + temptable + " where sessionId = '" + sessionID + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
            NoOfTrans = i;
            if (i <= 0)//Error Insert in 365
            {
                //i = 0;
                trans.Rollback();
                conn.Close();
                return i = 0;
            }
            else
            {
                

                sql = "SELECT DISTINCT Currency,AccountId,batchNumPartners,MlbatchNum FROM " + temptable + " WHERE sessionId = '" + sessionID + "'";
                command.CommandText = sql;
                List<string> Currency = new List<string>();

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Currency.Add(rdr[0].ToString().Replace("\r",""));
                            AccountCode = rdr[1].ToString();
                            CorporateBatchNo = rdr[2].ToString();
                            BatchNo = rdr[3].ToString();
                        }
                        rdr.Close();
                        foreach (string item in Currency)
                        {
                            UpdateTmpTbl = getNewRB(AccountCode, SumAmount(item, sessionID, ref conn, temptable), item, ref conn);
                            if (!UpdateTmpTbl)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                        }
                        //i = 1;
                    }
                    else
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                }
                if (UpdateTmpTbl)
                {
                    soUploadInserted = InsertSoUploadHeader(ref conn);
                    if (soUploadInserted <= 0)
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                    else
                    {
                        CorporateInserted = InsertDBPCorporateSendout(ref conn, temptable, sessionID);
                        if (CorporateInserted <= 0)
                        {
                            trans.Rollback();
                            conn.Close();
                            return i = 0;
                        }
                        else
                        {
                            TransLogInserted = InsertDBPTransactionslog(ref conn, sessionID, temptable);
                            if (TransLogInserted <= 0)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                            else
                            {
                                /// String pdate = "0802";//ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
                                soTransRefInserted = InsertDBPSotxnRef(ref conn, table, sessionID, temptable);
                                if (soTransRefInserted <= 0)
                                {
                                    trans.Rollback();
                                    conn.Close();
                                    return i = 0;
                                }
                                else
                                {
                                    bool result = true;
                                    sql = "INSERT INTO kpUploadArchive.tmpDBPtable(ControlNumber, AccountId, batchNumPartners, MLbatchNum, BranchName, BranchCode, StationId, StationNumber, OperatorId, ZoneCode, ";
                                    sql = sql + "SessionId, appnum, kptn, refno, ReceiverName, Index4, Index5, ReceiverAddress, ReceiverAddress2, ReceiverLandline, ReceiverMobile, ";
                                    sql = sql + "Principal, SenderName, Index12, Index13, Message, Charge, Currency) ";

                                    sql = sql + "select ControlNumber, AccountId, batchNumPartners, MLbatchNum, BranchName, BranchCode, StationId, StationNumber, OperatorId, ZoneCode, ";
                                    sql = sql + "SessionId, appnum, kptn, refno, ReceiverName, Index4, Index5, ReceiverAddress, ReceiverAddress2, ReceiverLandline, ReceiverMobile, ";
                                    sql = sql + "Principal, SenderName, Index12, Index13, Message, Charge, Currency from	kppartners.tmpDBPtable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }

                                    sql = "DELETE FROM kppartners.tmpDBPtable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }
                                    if (result)
                                    {

                                        trans.Commit();
                                        conn.Close();
                                        return i = 1;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        return i;
    }

    private int InsertDBPCorporateSendout(ref MySqlConnection conn, string table, string sessionId)
    {
        int i = 0;
        string sql;
        using (command = conn.CreateCommand())
        {
            sql = "insert into kppartnerstransactions.corporatesendouts (controlno,kptn,oldkptn,referenceno," + //4
                    "accountid,currency,transdate,stationno,isremote,operatorid,branchcode,zonecode,remoteoperatorid," + //9
                    "remotebranchcode,remotezonecode,principal,chargeto,chargeamount,othercharge,total,forex,traceno," + //9
                    "sessionid,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,isactive)" +//8

                    "select ControlNumber,kptn,'',refno,accountId,Currency,NOW(),stationnumber,'0',OperatorId,branchcode,zonecode,'','','', " + //15
                    "principal,'',charge,'','0.00','0.00','',sessionId,substring_index(SenderName,' ',1) as SenderFName,substring_index(substring_index(SenderName,' ',3),' ',-1) as SenderLName, " +
                    "substring_index(substring_index(SenderName,' ',3),' ',-1) as SenderMName,substring_index(ReceiverName,' ',1) as ReceiverFName,substring_index(substring_index(ReceiverName,' ',3),' ',-1) as ReceiverLName, " +
                    "substring_index(substring_index(ReceiverName,' ',2),' ',-1) as ReceiverMName,'1' " +
                    "FROM " + table + " WHERE sessionId = '" + sessionId + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertDBPTransactionslog(ref MySqlConnection con, string sessionId, string table)
    {
        int result = 0;
        string sql;
        using (command = con.CreateCommand())
        {
            sql = "Insert into kpadminpartnerslog.transactionslogs (kptnno,refno,AccountCode,action,type,branchcode" + //6
                     ",operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno,isremote) " + //11

                     "Select kptn,refno,AccountId,'SENDOUT','kppartners',branchcode,OperatorID,stationnumber,zonecode,stationId,now(),Currency," +
                     "SenderName,ReceiverName, principal,ControlNumber,0 from " + table + " WHERE sessionId = '" + sessionId + "'";
            command.CommandText = sql;
            result = command.ExecuteNonQuery();
            //Double Check stationCode = StationId/stationnumber
        }
        return result = result <= 0 ? 0 : 1;
    }

    private int InsertDBPSotxnRef(ref MySqlConnection con, string pdate, string sessionId, string table)
    {
        int result = 0;
        string sql;
        string tblref = "sendout" + pdate;
        using (command = con.CreateCommand())
        {
            sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) " +
                  "select refno,'" + tblref + "',AccountId,MlbatchNum,now(),Currency,'2' FROM " + table + " where sessionid = '" + sessionId + "'";
            command.CommandText = sql;
            result = command.ExecuteNonQuery();
        }
        return result = result <= 0 ? 0 : 1;
    }

    #endregion

    #region "BPI"

    private int InsertBPITable365(ref MySqlConnection conn, string sessionID, string temptable)
    {
        int i = 0;
        //int DeleteTmpTbl = 0;
        int soUploadInserted = 0;
        int CorporateInserted = 0;
        int TransLogInserted = 0;
        int soTransRefInserted = 0;
        bool UpdateTmpTbl = false;
        using (command = conn.CreateCommand())
        {
            DateTime serverdate;
            string table;
            command.CommandText = "Select NOW() as serverdt;";
            using (MySqlDataReader Reader = command.ExecuteReader())
            {
                Reader.Read();
                serverdate = Convert.ToDateTime(Reader["serverdt"]);
                Reader.Close();

            }

            table = serverdate.ToString("MM-dd").Replace("-", "");
            string sql;
            //SELECT Fields under Temp Table  
            sql = "Insert into kppartners.sendout" + table + " (ControlNo, ReferenceNo,Currency, Principal," +
                                                               "Charge, AccountCode, TransDate, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverGender, ReceiverContactNo, " +
                                                      "SenderGender, sessionID, OtherDetails, OperatorID," +
                                                      "StationID, KPTN,Message, BranchCode) " +
                  "SELECT ControlNumber,refno,Currency,Principal,charge,AccountId,NOW(),SenderFName,SenderLName,SenderMName,CONCAT(SenderFName,' ',SenderMName,' ',SenderLName) AS SenderName ,ReceiverFName,ReceiverLName, " +
                  "ReceiverMName, CONCAT(ReceiverFName,' ',ReceiverMName,' ',ReceiverLName) AS ReceiverName,3,benefphonenum,3,SessionId,CONCAT(txnappnum,' ',kptn,' ',txndistdate) As OtherDetail, " +
                  "OperatorId,StationId,kptn,msgtorecepient,BranchCode FROM " + temptable + " WHERE sessionId = '"+ sessionID +"'";
            //txnappnum + "|" + kptn + "|" + txndistdate
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
            NoOfTrans = i;
            if (i <= 0)//Error Insert in 365
            {
                //i = 0;
                trans.Rollback();
                conn.Close();
                return i = 0;
            }
            else
            {
                sql = "SELECT DISTINCT Currency,AccountId,batchNumPartners,MlbatchNum FROM " + temptable + " WHERE sessionId = '" + sessionID + "'";
                command.CommandText = sql;
                List<string> Currency = new List<string>();

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Currency.Add(rdr[0].ToString());
                            AccountCode = rdr[1].ToString();
                            CorporateBatchNo = rdr[2].ToString();
                            BatchNo = rdr[3].ToString();
                        }
                        rdr.Close();
                        foreach (string item in Currency)
                        {
                            UpdateTmpTbl = getNewRB(AccountCode, SumAmount(item, sessionID, ref conn, temptable), item, ref conn);
                            if (!UpdateTmpTbl)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                        }
                        //i = 1;
                    }
                    else
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                }
                if (UpdateTmpTbl)
                {
                    soUploadInserted = InsertSoUploadHeader(ref conn);
                    if (soUploadInserted <= 0)
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                    else
                    {
                        CorporateInserted = InsertCorporateSendout(ref conn, temptable, sessionID);
                        if (CorporateInserted <= 0)
                        {
                            trans.Rollback();
                            conn.Close();
                            return i = 0;
                        }
                        else
                        {
                            TransLogInserted = InsertTransactionslog(ref conn, sessionID, temptable);
                            if (TransLogInserted <= 0)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                            else
                            {
                                /// String pdate = "0802";//ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
                                soTransRefInserted = InsertSotxnRef(ref conn, table, sessionID, temptable);
                                if (soTransRefInserted <= 0)
                                {
                                    trans.Rollback();
                                    conn.Close();
                                    return i = 0;
                                }
                                else
                                {
                                    bool result = true;
                                    sql = String.Empty;
                                    sql = "INSERT INTO kpUploadArchive.tmpBPITable(ControlNumber,AccountId,batchNumPartners, MLbatchNum, branchName, branchCode, StationId, StationNumber, OperatorId, ZoneCode, SessionId, ";
                                    sql = sql + "refno, kptn, txnappnum, arnum, txndistdate, settlement, SenderLName, SenderFName, SenderMName, ReceiverLName, ReceiverFName,ReceiverMName, ";
                                    sql = sql + "altrnaterecepient, addrs1, addr2, addr3, benefcode, benefphonenum, benefmobilenum, benefofficenum, beneflandmrk, msgtorecepient, Currency, ";
                                    sql = sql + "Principal, charge) ";

                                    sql = sql + "SELECT ControlNumber,AccountId,batchNumPartners, MLbatchNum, branchName, branchCode, StationId, StationNumber, OperatorId, ZoneCode, SessionId, ";
                                    sql = sql + "refno, kptn, txnappnum, arnum, txndistdate, settlement, SenderLName, SenderFName, SenderMName, ReceiverLName, ReceiverFName,ReceiverMName, ";
                                    sql = sql + "altrnaterecepient, addrs1, addr2, addr3, benefcode, benefphonenum, benefmobilenum, benefofficenum, beneflandmrk, msgtorecepient, Currency, ";
                                    sql = sql + "Principal, charge from kppartners.tmpBPITable where sessionId = '" + sessionID + "';";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }

                                    sql = "DELETE FROM kppartners.tmpBPITable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }
                                    if (result)
                                    {

                                        trans.Commit();
                                        conn.Close();
                                        return i = 1;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        return i;
    }
    
    #endregion    

    #region "BDO"

    private int InsertBDOTable365(ref MySqlConnection conn, string sessionID, string temptable)
    {
        int i = 0;
        //int DeleteTmpTbl = 0;
        int soUploadInserted = 0;
        int CorporateInserted = 0;
        int TransLogInserted = 0;
        int soTransRefInserted = 0;
        bool UpdateTmpTbl = false;
        using (command = conn.CreateCommand())
        {
            DateTime serverdate;
            string table;
            command.CommandText = "Select NOW() as serverdt;";
            using (MySqlDataReader Reader = command.ExecuteReader())
            {
                Reader.Read();
                serverdate = Convert.ToDateTime(Reader["serverdt"]);
                Reader.Close();

            }

            table = serverdate.ToString("MM-dd").Replace("-", "");

            //SELECT Fields under Temp Table                   
            string sql = "Insert into kppartners.sendout" + table + " (ControlNo, ReferenceNo, Currency, Principal, " +
                                                      "Charge, AccountCode, TransDate, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverContactNo, " +
                                                      "sessionID, OtherDetails, OperatorID, " +
                                                      "StationID, KPTN, BranchCode,Total )" +
                          "Select ControlNo,refno,currency,principal,charge,Accountid,now(),substring_index(SenderFname,' ',1) as FirstName," +
                          "substring_index(substring_index(SenderFname,' ',-2),' ',1) as FamilyName,substring_index(substring_index(SenderFname,' ',-3),' ',1) as MiddleName,SenderFName," +
                          "beneffn,benefln,benefmn,CONCAT(beneffn,' ',benefmn,' ',benefln) AS ReceiverName,'',sessionid, " +
                          "CONCAT(prodname,'|',bankbranch,'|',LocatorCode)as Otherdetails, " +
                          " OperatorId,StationNum,kptn,branchcode,TotalAmount " +
                          "FROM " + temptable + " where sessionId = '" + sessionID + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
            NoOfTrans = i;
            if (i <= 0)//Error Insert in 365
            {
                //i = 0;
                trans.Rollback();
                conn.Close();
                return i = 0;
            }
            else
            {
                sql = "SELECT DISTINCT Currency,AccountId,batchNumPartner,MlbatchNum FROM " + temptable + " WHERE sessionId = '" + sessionID + "'";
                command.CommandText = sql;
                List<string> Currency = new List<string>();

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Currency.Add(rdr[0].ToString());
                            AccountCode = rdr[1].ToString();
                            CorporateBatchNo = rdr[2].ToString();
                            BatchNo = rdr[3].ToString();
                        }
                        rdr.Close();
                        foreach (string item in Currency)
                        {
                            UpdateTmpTbl = getNewRB(AccountCode, SumAmount(item, sessionID, ref conn, temptable), item, ref conn);
                            if (!UpdateTmpTbl)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                        }
                        //i = 1;
                    }
                    else
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                }
                if (UpdateTmpTbl)
                {
                    soUploadInserted = InsertSoUploadHeader(ref conn);//No need to modify SELECT Query
                    if (soUploadInserted <= 0)
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                    else
                    {
                        CorporateInserted = InsertBDOCorporateSendout(ref conn, temptable, sessionID);
                        if (CorporateInserted <= 0)
                        {
                            trans.Rollback();
                            conn.Close();
                            return i = 0;
                        }
                        else
                        {
                            TransLogInserted = InsertBDOTransactionslog(ref conn, sessionID, temptable);
                            if (TransLogInserted <= 0)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                            else
                            {
                                /// String pdate = "0802";//ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
                                soTransRefInserted = InsertSotxnRef(ref conn, table, sessionID, temptable); //No need to modify SELECT Query
                                if (soTransRefInserted <= 0)
                                {
                                    trans.Rollback();
                                    conn.Close();
                                    return i = 0;
                                }
                                else
                                {
                                    bool result = true;
                                    sql = "INSERT INTO kpUploadArchive.tmpBDOTable(ControlNo,AccountId,batchNumPartner,Mlbatchnum,branchName,branchCode,StationId,StationNum,OperatorId,ZoneCode,SessionId,refno,kptn,";
                                    sql = sql + "`type`,benefln,beneffn,benefmn,senderFName,ProdName,Principal,Currency,BankBranch,TransDate,Ins1,Ins2,Rf1,Rf2,Rf3,Rf4,rCount,TotalAmount,";
                                    sql = sql + "charge,LocatorCode,message) ";

                                    sql = sql + "select ControlNo,AccountId,batchNumPartner,Mlbatchnum,branchName,branchCode,StationId,StationNum,OperatorId,ZoneCode,SessionId,refno,kptn,";
                                    sql = sql + "`type`,benefln,beneffn,benefmn,senderFName,ProdName,Principal,Currency,BankBranch,TransDate,Ins1,Ins2,Rf1,Rf2,Rf3,Rf4,rCount,TotalAmount,";
                                    sql = sql + "charge,LocatorCode,message from	kppartners.tmpBDOTable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }

                                    sql = "DELETE FROM kppartners.tmpBDOTable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }
                                    if (result)
                                    {

                                        trans.Commit();
                                        conn.Close();
                                        return i = 1;
                                    }

                                }
                            }
                        }
                    }

                }
            }
        }
        return i;
    }

    private int InsertBDOCorporateSendout(ref MySqlConnection conn, string table, string sessionId)
    {
        int i = 0;
        string sql;
        using (command = conn.CreateCommand())
        {
            sql = "insert into kppartnerstransactions.corporatesendouts (controlno,kptn,oldkptn,referenceno," + //4
                    "accountid,currency,transdate,stationno,isremote,operatorid,branchcode,zonecode,remoteoperatorid," + //9
                    "remotebranchcode,remotezonecode,principal,chargeto,chargeamount,othercharge,total,forex,traceno," + //9
                    "sessionid,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,isactive)" +//8                                              

                    "select ControlNo,kptn,'',refno,accountId,currency,NOW(),StationNum,'0',OperatorId,branchcode,zonecode,'','','', " + //15
                    "principal,'',charge,'','0.00','0.00','',sessionId,substring_index(SenderFname,' ',1) as FirstName,substring_index(substring_index(SenderFname,' ',-2),' ',1) as FamilyName, " +
                    "substring_index(substring_index(SenderFname,' ',-3),' ',1) as MiddleName,beneffn,benefln,benefmn,'1' " +
                    "FROM " + table + " WHERE sessionId = '" + sessionId + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;

    }

    private int InsertBDOTransactionslog(ref MySqlConnection con, string sessionId, string table)
    {
        int i = 0;
        string sql;
        using (command = con.CreateCommand())
        {
            sql = "Insert into kpadminpartnerslog.transactionslogs (kptnno,refno,AccountCode,action,type,branchcode" + //6
                  ",operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno,isremote) " + //11
                  "Select kptn,refno,AccountId,'SENDOUT','kppartners',branchcode,OperatorID,stationnum,zonecode,stationId,now(),Currency," +
                  "senderFName,CONCAT(beneffn,' ',benefmn,' ',benefln) AS ReceiverName," +
                  "principal,ControlNo,0 from " + table + " WHERE sessionId = '" + sessionId + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();            
        }
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertBDOSotxnRef(ref MySqlConnection con, string pdate, string sessionId, string table)
    {
        int i = 0;
        string sql;
        string tblref = "sendout" + pdate;
        using (command = con.CreateCommand())
        {
            sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) " +
                  "select refno,'" + tblref + "',AccountId,MlbatchNum,now(),Currency,'2' FROM " + table + " where sessionid = '" + sessionId + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    #endregion

    #region "P2P"

    private int InsertTable365(ref MySqlConnection conn, string sessionID, string temptable)
    {
        int i = 0;
        //int DeleteTmpTbl = 0;
        int soUploadInserted = 0;
        int CorporateInserted = 0;
        int TransLogInserted = 0;
        int soTransRefInserted = 0;
        bool UpdateTmpTbl = false;
        using (command = conn.CreateCommand())
        {
            DateTime serverdate;
            string table;
            command.CommandText = "Select NOW() as serverdt;";
            using (MySqlDataReader Reader = command.ExecuteReader())
            {
                Reader.Read();
                serverdate = Convert.ToDateTime(Reader["serverdt"]);
                Reader.Close();

            }

            table = serverdate.ToString("MM-dd").Replace("-", "");

            //SELECT Fields under Temp Table                   
            string sql = "INSERT INTO kppartners.sendout" + table + " (ControlNo, ReferenceNo, Currency, Principal," +
                                                          "Charge, AccountCode, TransDate, " +
                                                          "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                          "ReceiverName, ReceiverGender, ReceiverContactNo,ReceiverBirthDate, " +
                                                          "SenderGender, SenderContactNo, sessionID, OtherDetails, OperatorID," +
                                                          "StationID, ReceiverStreet, ReceiverCountry, KPTN, Message, BranchCode, SenderBirthdate) " +
                          "Select ControlNumber,refno,currency,principal,charge,AccountId,now(),senderfname,senderlname,sendermname,CONCAT(senderfname,' ',sendermname,' ',SenderLName) AS SenderName, " +
                          "receiverfname,receiverlname,receivermname,CONCAT(ReceiverFName,' ',Receivermname,' ',ReceiverLName) AS ReceiverName, receivergender, receiverphonenum,receiverbdate,sendergender,senderphone, " +
                          "sessionId,CONCAT (itemno,'|',kptn,'|',dttiled,'|',sourceoffund,'|',relationtoreceiver,'|',purpose,'|',senderIdtype,'|',senderIdnum,'|',senderidexpiredate) AS OtherDetails, " +
                          "OperatorID,stationID,receiverstreet,receiverscountry,kptn,message,branchcode,senderbirthdate " +
                          "FROM " + temptable + " where sessionId = '" + sessionID + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
            NoOfTrans = i;
            if (i <= 0)//Error Insert in 365
            {
                //i = 0;
                trans.Rollback();
                conn.Close();
                return i = 0;
            }
            else
            {
                sql = "SELECT DISTINCT Currency,AccountId,batchNumPartners,MlbatchNum FROM " + temptable + " WHERE sessionId = '" + sessionID + "'";
                command.CommandText = sql;
                List<string> Currency = new List<string>();

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Currency.Add(rdr[0].ToString());
                            AccountCode = rdr[1].ToString();
                            CorporateBatchNo = rdr[2].ToString();
                            BatchNo = rdr[3].ToString();
                        }
                        rdr.Close();
                        foreach (string item in Currency)
                        {
                            UpdateTmpTbl = getNewRB(AccountCode, SumAmount(item, sessionID, ref conn, temptable), item, ref conn);
                            if (!UpdateTmpTbl)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                        }
                        //i = 1;
                    }
                    else
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                }
                if (UpdateTmpTbl)
                {
                    soUploadInserted = InsertSoUploadHeader(ref conn);
                    if (soUploadInserted <= 0)
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                    else
                    {
                        CorporateInserted = InsertCorporateSendout(ref conn, temptable, sessionID);
                        if (CorporateInserted <= 0)
                        {
                            trans.Rollback();
                            conn.Close();
                            return i = 0;
                        }
                        else
                        {
                            TransLogInserted = InsertTransactionslog(ref conn, sessionID, temptable);
                            if (TransLogInserted <= 0)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                            else
                            {                                
                                soTransRefInserted = InsertSotxnRef(ref conn, table, sessionID, temptable);
                                if (soTransRefInserted <= 0)
                                {
                                    trans.Rollback();
                                    conn.Close();
                                    return i = 0;
                                }
                                else
                                {
                                    bool result = true;
                                    sql = "INSERT INTO kpUploadArchive.tmpp2ptable(ControlNumber, AccountId, batchNumPartners, MlbatchNum, branchName, branchcode, stationID, stationnumber, OperatorID, ";
                                    sql = sql + "zonecode, sessionId, itemno, kptn, dttiled, refno, principal, currency, charge, sourceoffund, relationtoreceiver, ";
                                    sql = sql + "purpose, senderlname, senderfname, sendermname, senderIdtype, senderIdnum, senderidexpiredate, senderbirthdate, sendergender, ";
                                    sql = sql + "senderstreet, senderprovince, sendercountry, senderphone, receiverlname, receiverfname, receivermname, receiverstreet, receiversprovince, ";
                                    sql = sql + "receiverscountry, receiverbdate, receivergender, receiverphonenum, message)";

                                    sql = sql + "select ControlNumber, 	AccountId, batchNumPartners, MlbatchNum, branchName, branchcode, stationID, stationnumber, OperatorID, ";
                                    sql = sql + "zonecode, sessionId, itemno, kptn, dttiled, refno, principal, currency, charge, sourceoffund, relationtoreceiver, ";
                                    sql = sql + "purpose, senderlname, senderfname, sendermname, senderIdtype, senderIdnum, senderidexpiredate, senderbirthdate, sendergender,  ";
                                    sql = sql + "senderstreet, senderprovince, sendercountry, senderphone, receiverlname, receiverfname, receivermname, receiverstreet, receiversprovince,";
                                    sql = sql + "receiverscountry, receiverbdate, receivergender, receiverphonenum, message from	kppartners.tmpp2ptable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }

                                    sql = "DELETE FROM kppartners.tmpp2ptable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }
                                    if (result)
                                    {

                                        trans.Commit();
                                        conn.Close();
                                        return i = 1;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        return i;
    }

    private int InsertCorporateSendout(ref MySqlConnection conn, string table, string sessionId)
    {
        int i = 0;
        string sql;
        using (command = conn.CreateCommand())
        {
            sql = "insert into kppartnerstransactions.corporatesendouts (controlno,kptn,oldkptn,referenceno," + //4
                    "accountid,currency,transdate,stationno,isremote,operatorid,branchcode,zonecode,remoteoperatorid," + //9
                    "remotebranchcode,remotezonecode,principal,chargeto,chargeamount,othercharge,total,forex,traceno," + //9
                    "sessionid,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,isactive)" +//8

                    "select ControlNumber,kptn,'',refno,accountId,currency,NOW(),stationnumber,'0',OperatorId,branchcode,zonecode,'','','', " + //15
                    "principal,'',charge,'','0.00','0.00','',sessionId,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,'1' " +
                    "FROM " + table + " WHERE sessionId = '" + sessionId + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertTransactionslog(ref MySqlConnection con, string sessionId, string table)
    {
        int i = 0;
        string sql;
        using (command = con.CreateCommand())
        {
            sql = "Insert into kpadminpartnerslog.transactionslogs (kptnno,refno,AccountCode,action,type,branchcode" + //6
                  ",operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno,isremote) " + //11

                  "Select kptn,refno,AccountId,'SENDOUT','kppartners',branchcode,OperatorID,stationnumber,zonecode,stationId,now(),Currency," +
                  "CONCAT(senderfname,' ',sendermname,' ',SenderLName) AS SenderName,CONCAT(ReceiverFName,' ',Receivermname,' ',ReceiverLName) AS ReceiverName," +
                  "principal,ControlNumber,0 from " + table + " WHERE sessionId = '" + sessionId + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertSotxnRef(ref MySqlConnection con, string pdate, string sessionId, string table)
    {
        int i = 0;
        string sql;
        string tblref = "sendout" + pdate;
        using (command = con.CreateCommand())
        {
            sql = "Insert into kppartners.sotxnref (ReferenceNo,tablereference,AccountCode,BatchNo,TransDate,currency,TransactionType) " +
                  "select refno,'" + tblref + "',AccountId,MlbatchNum,now(),Currency,'2' FROM " + table + " where sessionid = '" + sessionId + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    private int InsertSoUploadHeader(ref MySqlConnection con)
    {
        int i = 0;
        string sql;
        using (command = con.CreateCommand())
        {
            sql = "INSERT INTO kppartners.souploadheader (BatchNo,AccountCode,NoOfTrxn,UploadDate,CorporateBatchNo)" +
                  "VALUES('" + BatchNo + "','" + AccountCode + "','" + NoOfTrans + "',NOW(),'" + CorporateBatchNo + "')";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
        }
        return i = i <= 0 ? 0 : 1;
    }

    #endregion

    #region "Money Express"

    private int InsertMOEXTable365(ref MySqlConnection conn, string sessionID, string temptable)
    {
        int i = 0;
        //int DeleteTmpTbl = 0;
        int soUploadInserted = 0;
        int CorporateInserted = 0;
        int TransLogInserted = 0;
        int soTransRefInserted = 0;
        bool UpdateTmpTbl = false;
        using (command = conn.CreateCommand())
        {
            DateTime serverdate;
            string table;
            command.CommandText = "Select NOW() as serverdt;";
            using (MySqlDataReader Reader = command.ExecuteReader())
            {
                Reader.Read();
                serverdate = Convert.ToDateTime(Reader["serverdt"]);
                Reader.Close();

            }

            table = serverdate.ToString("MM-dd").Replace("-", "");

            //SELECT Fields under Temp Table                   
            string sql = "INSERT INTO kppartners.sendout" + table + " (ControlNo, ReferenceNo, Currency, Principal," +
                                                      "Charge, Total, AccountCode, TransDate, " +
                                                      "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
                                                      "ReceiverName, ReceiverAddress, ReceiverContactNo, " +
                                                      "sessionID, OtherDetails, OperatorID," +
                                                      "StationID, ReceiverStreet, ReceiverCountry, KPTN,Message, BranchCode) " +
                          "Select ControlNumber,refno,Currency,Principal,Charge,Principal,AccountId,now(),SenderFname,SenderLname,SenderMName,CONCAT(SenderFName,' ',SenderMName,' ',SenderLName) AS SenderName, " +
                          "ReceiverFName,ReceiverLName,ReceiverMName,CONCAT(ReceiverFName,' ',ReceiverMName,' ',ReceiverLName) AS ReceiverName, ReceiverAddress, ReceiverMobile, " +                          
                          "SessionId,CONCAT (kptn,'|',purpose) AS OtherDetails, OperatorID,StationId,ReceiverStreet,ReceiverCountry,kptn,Message,BranchCode " +
                          "FROM " + temptable + " where sessionId = '" + sessionID + "'";
            command.CommandText = sql;
            i = command.ExecuteNonQuery();
            NoOfTrans = i;
            if (i <= 0)//Error Insert in 365
            {
                //i = 0;
                trans.Rollback();
                conn.Close();
                return i = 0;
            }
            else
            {
                sql = "SELECT DISTINCT Currency,AccountId,batchNumPartners,MlbatchNum FROM " + temptable + " WHERE sessionId = '" + sessionID + "'";
                command.CommandText = sql;
                List<string> Currency = new List<string>();

                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Currency.Add(rdr[0].ToString());
                            AccountCode = rdr[1].ToString();
                            CorporateBatchNo = rdr[2].ToString();
                            BatchNo = rdr[3].ToString();
                        }
                        rdr.Close();
                        foreach (string item in Currency)
                        {
                            UpdateTmpTbl = getNewRB(AccountCode, SumAmount(item, sessionID, ref conn, temptable), item, ref conn);
                            if (!UpdateTmpTbl)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                        }
                        //i = 1;
                    }
                    else
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                }
                if (UpdateTmpTbl)
                {
                    soUploadInserted = InsertSoUploadHeader(ref conn);
                    if (soUploadInserted <= 0)
                    {
                        //i = 0;
                        trans.Rollback();
                        conn.Close();
                        return i = 0;
                    }
                    else
                    {
                        CorporateInserted = InsertCorporateSendout(ref conn, temptable, sessionID);
                        if (CorporateInserted <= 0)
                        {
                            trans.Rollback();
                            conn.Close();
                            return i = 0;
                        }
                        else
                        {
                            TransLogInserted = InsertTransactionslog(ref conn, sessionID, temptable);
                            if (TransLogInserted <= 0)
                            {
                                trans.Rollback();
                                conn.Close();
                                return i = 0;
                            }
                            else
                            {
                                /// String pdate = "0802";//ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
                                soTransRefInserted = InsertSotxnRef(ref conn, table, sessionID, temptable);
                                if (soTransRefInserted <= 0)
                                {
                                    trans.Rollback();
                                    conn.Close();
                                    return i = 0;
                                }
                                else
                                {
                                    bool result = true;
                                    sql = "INSERT INTO kpUploadArchive.tmpMOEXTable(ControlNumber, 	AccountId, batchNumPartners, MLbatchNum, BranchName, BranchCode, StationId, StationNumber, OperatorId, ZoneCode, ";
                                    sql = sql + "SessionId, refno, Index1, Index2, SenderFName, SenderLName, SenderMName, ReceiverFName, ReceiverLName, ReceiverMName, ReceiverAddress, ";
                                    sql = sql + "ReceiverStreet, ReceiverCountry, ReceiverMobile, Index13, Purpose, Index15, Index16, Index17, Index18, Index19, Principal, ";
                                    sql = sql + "Currency, Index22, Index23, Index24, Index25, Index26, Index27, Index28, Index29, Charge, kptn, OtherDetails, Message)";

                                    sql = sql + "select ControlNumber, 	AccountId, batchNumPartners, MLbatchNum, BranchName, BranchCode, StationId, StationNumber, OperatorId, ZoneCode, ";
                                    sql = sql + "SessionId, refno, Index1, Index2, SenderFName, SenderLName, SenderMName, ReceiverFName, ReceiverLName, ReceiverMName, ReceiverAddress, ";
                                    sql = sql + "ReceiverStreet, ReceiverCountry, ReceiverMobile, Index13, Purpose, Index15, Index16, Index17, Index18, Index19, Principal, ";
                                    sql = sql + "Currency, Index22, Index23, Index24, Index25, Index26, Index27, Index28, Index29, Charge, kptn, OtherDetails, Message ";
                                    sql = sql + "from	kppartners.tmpMOEXTable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }

                                    sql = "DELETE FROM kppartners.tmpMOEXTable where sessionId = '" + sessionID + "'";
                                    command.CommandText = sql;
                                    i = command.ExecuteNonQuery();
                                    if (i <= 0)//Error Insert in 365
                                    {
                                        //i = 0;
                                        trans.Rollback();
                                        conn.Close();
                                        result = false;
                                        return i = 0;
                                    }
                                    if (result)
                                    {

                                        trans.Commit();
                                        conn.Close();
                                        return i = 1;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        return i;
    }

    #endregion

    

    public string SubmitDBPDatabase(string sessionId)
    {
        string Msg;
        string tmpTable = "kppartners.tmpDBPtable";
        ser.ConnectPartners();
        MySqlConnection con = ser.dbconkp.getConnection();
        try
        {
            con.Open();
            using (command = con.CreateCommand())
            {
                trans = con.BeginTransaction();
                command.Transaction = trans;
                if (InsertDBPTable365(ref con, sessionId, tmpTable) == 0)
                {
                    Msg = "Error";
                    return Msg;
                }
                return Msg = "Success";
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            con.Close();
            throw ex;
        }
    }
    public string SubmitBPIDatabase(string sessionId)
    {
        string Msg;
        string tmpTable = "kppartners.tmpBPITable";
        ser.ConnectPartners();
        MySqlConnection con = ser.dbconkp.getConnection();
        try
        {
            con.Open();
            using (command = con.CreateCommand())
            {
                trans = con.BeginTransaction();
                command.Transaction = trans;
                if (InsertBPITable365(ref con, sessionId, tmpTable) == 0)
                {
                    Msg = "Error";
                    return Msg;
                }
                return Msg = "Success";
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            con.Close();
            throw ex;
        }
    }
    public string SubmitBDOToDatabase(string sessionId)
    {
        string Msg;
        string tmpTable = "kppartners.tmpBDOTable";
        ser.ConnectPartners();
        MySqlConnection con = ser.dbconkp.getConnection();
        try
        {
            con.Open();
            using (command = con.CreateCommand())
            {
                trans = con.BeginTransaction();
                command.Transaction = trans;
                if (InsertBDOTable365(ref con, sessionId, tmpTable) == 0)
                {
                    Msg = "Error";
                    return Msg;
                }
                return Msg = "Success";
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            con.Close();
            throw ex;
        }
    }
    public string SubmitToDatabase(string sessionId)
    {
        string Msg;
        string tmpTable = "kppartners.tmpp2ptable";
        ser.ConnectPartners();
        MySqlConnection con = ser.dbconkp.getConnection();
        try
        {
            con.Open();
            using (command = con.CreateCommand())
            {             
                trans = con.BeginTransaction();
                command.Transaction = trans;
                if (InsertTable365(ref con, sessionId, tmpTable) == 0)
                {
                    Msg = "Error";
                    return Msg;
                }
                return Msg = "Success";
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            con.Close();
            throw ex;
        }
    }
    public string SubmitMOEXToDatabase(string sessionId)
    {
        string Msg;
        string tmpTable = "kppartners.tmpMOEXTable";
        ser.ConnectPartners();
        MySqlConnection con = ser.dbconkp.getConnection();
        try
        {
            con.Open();
            //command = con.CreateCommand();
            //String pate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
            using (command = con.CreateCommand())
            {

                //String pdate = "0802";
                trans = con.BeginTransaction();
                command.Transaction = trans;
                if (InsertMOEXTable365(ref con, sessionId, tmpTable) == 0)
                {
                    Msg = "Error";
                    return Msg;
                }
                return Msg = "Success";
            }
        }
        catch (Exception ex)
        {
            trans.Rollback();
            con.Close();
            throw ex;
        }
    }
    
    
    public string SaveTransactionDBPUpload(String[] Data, String AccountId, String batchnumforPartners, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId)
    {
        string output = "";
       
        int Inserted = 0;
        filename = "NOWDBP.csv";
        string ItemToCSV = "";
        ser.ConnectPartners();
        MLbatchNum = ser.generateBatchNo();
        MLbNumber = MLbatchNum;
        tinuodngaFilename = mappath + filename;
        string tmpTable = "kppartners.tmpDBPtable";
        MySqlConnection conn = ser.dbconkp.getConnection();
        

        try
        {
            conn.Open();
            command = conn.CreateCommand();
            String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
            trans = conn.BeginTransaction();
            command.Transaction = trans;
            foreach (string item in Data)
            {
                column = item.Split('|');
                ItemToCSV = item.Replace(",", "");
                decimal principal = Convert.ToDecimal(column[10].ToString());
                string reference = column[2].ToString();
                //string curr = column[5].ToString();
                string curr = Currency;
                int zc = Convert.ToInt32(zonecode);
                arraytotal += 1;
                if (ser.CheckTierCode(AccountId, principal, curr).Equals(true))
                {
                    int ret = ser.checkreference(reference, AccountId);
                    if (ret == 0)
                    {
                        arraycount += 1;
                        string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                        if (ControlNumber == "ErrorOnControlNumber")
                        {
                            ermsg = "Error on generating ControlNumber";
                            trans.Rollback();
                            conn.Close();
                            return output = "error Control";
                        }
                        File = File + "," + ControlNumber + "," + AccountId + "," + batchnumforPartners + "," + MLbatchNum + "," + bname + "," + bcode + "," + stationID + "," +
                               stationcode + "," + operatorID + "," + zonecode + "," + SessionId + "," +
                               ItemToCSV.Replace('|', ',') + Environment.NewLine;
                        if (numdata == arraycount || Data.Length == arraytotal)
                        {
                            File = File.Substring(0, File.Length - 2);
                            Inserted = Inserted + InsertSuccess(arraycount, ref conn, tmpTable, File);
                            if (Inserted == 0)
                            {
                                output = "error Upload";
                                ermsg = "error in uploading";
                                trans.Rollback();
                                conn.Close();
                                return output;
                            }
                            File = "";
                        }
                        
                        counterSuccess += 1;
                        ListSuccess.Add(item + "|" + "Success");
                    }
                    else
                    {
                        errorname = "reference no. already exist";
                        ListError.Add(item + "|" + errorname);
                        
                    }
                }
                else
                {
                    errorname = "Amount is Greater than the Threshold";
                    ListError.Add(item + "|" + errorname);
                    
                }

                ItemToCSV = "";
            }
            if (Inserted == Data.Length)
            {
                output = "success";
                trans.Commit();
                conn.Close();
                return output;
            }
            else
            {
                output = "failed";
                trans.Rollback();
                conn.Close();
                return output;
            }
        }
        catch (Exception ex)
        {
            output = ex.ToString();
            ermsg = ex.ToString();
            trans.Rollback();
            conn.Close();
            return output;
        }
    }
    public string SaveTransactionBDOUpload(String[] Data, String AccountId, String batchnumforPartners, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId)
    {
        string output = "";
        string ermsg = "";
        int Inserted = 0;
        filename = "NOWBDO.csv";
        string ItemToCSV = "";
        ser.ConnectPartners();
        MLbatchNum = ser.generateBatchNo();
        MLbNumber = MLbatchNum;
        tinuodngaFilename = mappath + filename;
        string tmpTable = "kppartners.tmpBDOTable";
        MySqlConnection conn = ser.dbconkp.getConnection();

        try
        {
            conn.Open();
            command = conn.CreateCommand();
            String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
            trans = conn.BeginTransaction();
            command.Transaction = trans;
            foreach (string item in Data)
            {
                column = item.Split('|');
                ItemToCSV = item.Replace(",", "");
                decimal principal = Convert.ToDecimal(column[8].ToString());
                string reference = column[0].ToString();
                string curr = column[9].ToString();
                int zc = Convert.ToInt32(zonecode);

                arraytotal += 1;
                if (ser.CheckTierCode(AccountId, principal, curr).Equals(true))
                {
                    int ret = ser.checkreference(reference, AccountId);
                    if (ret == 0)
                    {
                        arraycount += 1;
                        string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                        if (ControlNumber == "ErrorOnControlNumber")
                        {
                            ermsg = "Error on generating ControlNumber";
                            trans.Rollback();
                            conn.Close();
                            return output = "error Control";
                        }
                        File = File + "," + ControlNumber + "," + AccountId + "," + batchnumforPartners + "," + MLbatchNum + "," + bname + "," + bcode + "," + stationID + "," +
                               stationcode + "," + operatorID + "," + zonecode + "," + SessionId + "," +
                               ItemToCSV.Replace('|', ',') + "\r\n";
                        if (numdata == arraycount || Data.Length == arraytotal)
                        {
                            File = File.Substring(0, File.Length - 2);
                            Inserted = Inserted + InsertSuccess(arraycount, ref conn, tmpTable, File);
                            if (Inserted == 0)
                            {
                                output = "error Upload";
                                ermsg = "error in uploading";
                                trans.Rollback();
                                conn.Close();
                                return output;
                            }
                            File = "";
                        }
                        ListSuccess.Add(item + "|" + "Success"); 
                    }
                    else
                    {
                        errorname = "reference no. already exist";
                        ListError.Add(item + "|" + errorname);
                        
                    }
                }
                else
                {
                    errorname = "Amount is Greater than the Threshold";
                    ListError.Add(item + "|" + errorname);
                    
                }
                ItemToCSV = "";
            }
            if (Inserted == Data.Length)
            {
                output = "success";
                trans.Commit();
                conn.Close();
                return output;
            }
            else
            {
                output = "failed";
                trans.Rollback();
                conn.Close();
                return output;
            }
        }
        catch (Exception ex)
        {
            output = ex.ToString();// "error Try Catch";
            ermsg = ex.ToString();
            trans.Rollback();
            conn.Close();
            return output;
        }                 
    }
    public string SaveTransactionBPIUpload(String[] Data, String AccountId, String batchnumforPartners, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId)
    {
        string output = "";
        string ermsg = "";
        int Inserted = 0;
        filename = "NOWBPI.csv";
        string ItemToCSV = "";
        ser.ConnectPartners();
        MLbatchNum = ser.generateBatchNo();
        MLbNumber = MLbatchNum;
        tinuodngaFilename = mappath + filename;
        string tmpTable = "kppartners.tmpBPITable"; 
        MySqlConnection conn = ser.dbconkp.getConnection();

        try
        {
            conn.Open();
            command = conn.CreateCommand();
            String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
            trans = conn.BeginTransaction();
            command.Transaction = trans;
            foreach (string item in Data)
            {
                column = item.Split('|');
                ItemToCSV = item.Replace(",", "");
                decimal principal = Convert.ToDecimal(column[23].ToString());
                string reference = column[0].ToString();
                string curr = column[22].ToString();
                int zc = Convert.ToInt32(zonecode);

                arraytotal += 1;
                if (ser.CheckTierCode(AccountId, principal, curr).Equals(true))
                {
                    int ret = ser.checkreference(reference, AccountId);
                    if (ret == 0)
                    {
                        arraycount += 1;
                        string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                        if (ControlNumber == "ErrorOnControlNumber")
                        {
                            ermsg = "Error on generating ControlNumber";
                            trans.Rollback();
                            conn.Close();
                            return output = "error Control";
                        }
                        File = File + "," + ControlNumber + "," + AccountId + "," + batchnumforPartners + "," + MLbatchNum + "," + bname + "," + bcode + "," + stationID + "," +
                               stationcode + "," + operatorID + "," + zonecode + "," + SessionId + "," +
                               ItemToCSV.Replace('|', ',') + "\r\n";
                        if (numdata == arraycount || Data.Length == arraytotal)
                        {
                            File = File.Substring(0, File.Length - 2);
                            Inserted = Inserted + InsertSuccess(arraycount, ref conn, tmpTable, File);
                            if (Inserted == 0)
                            {
                                output = "error in upload";
                                ermsg = "error in uploading";
                                trans.Rollback();
                                conn.Close();
                                return output;
                            }
                            File = "";
                        }
                        ListSuccess.Add(item + "|Success");
                    }
                    else
                    {
                        errorname = "reference no. already exist";
                        ListError.Add(item + "|" + errorname);
                        
                    }
                }
                else
                {
                    errorname = "Amount is Greater than the Threshold";
                    ListError.Add(item + "|" + errorname);
                    
                }
                ItemToCSV = "";
            }
            if (Inserted == Data.Length)
            {
                output = "success";
                trans.Commit();
                conn.Close();
                return output;
            }
            else
            {
                output = "failed";
                trans.Rollback();
                conn.Close();
                return output;
            }
        }
        catch (Exception ex)
        {
            output = ex.ToString();
            ermsg = ex.ToString();
            trans.Rollback();
            conn.Close();
            return output;
        }
    }
    public string SaveTransactionP2PUpload(String[] Data, String AccountId, String batchnumforPartners, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId)
    {
        string output = "";
        string ermsg = "";
        int Inserted = 0;
        filename = DateTime.Now.Ticks.ToString()   + ".csv";
        string ItemToCSV = "";
        ser.ConnectPartners();
        MLbatchNum = ser.generateBatchNo();
        MLbNumber = MLbatchNum;
        tinuodngaFilename = mappath + filename;
        string tmpTable = "kppartners.tmpp2ptable";
        MySqlConnection conn = ser.dbconkp.getConnection();

        try
        {
            conn.Open();
            command = conn.CreateCommand();
            String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
            trans = conn.BeginTransaction();
            command.Transaction = trans;
            foreach (string item in Data)
            {
                column = item.Split('|');
                ItemToCSV = item.Replace(",", "");

                decimal principal = Convert.ToDecimal(column[4].ToString());
                string reference = column[3].ToString();
                string curr = column[5].ToString();
                int zc = Convert.ToInt32(zonecode);

                arraytotal += 1;
                if (ser.CheckTierCode(AccountId, principal, curr).Equals(true))
                {
                    int ret = ser.checkreference(reference, AccountId);
                    if (ret == 0)
                    {
                        arraycount += 1;

                        string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                        if (ControlNumber == "ErrorOnControlNumber")
                        {
                            ermsg = "Error on generating ControlNumber";
                            trans.Rollback();
                            conn.Close();
                            return output = "error on Control Number";
                        }
                        File = File + "," + ControlNumber + "," + AccountId + "," + batchnumforPartners + "," + MLbatchNum + "," + bname + "," + bcode + "," + stationID + "," +
                               stationcode + "," + operatorID + "," + zonecode + "," + SessionId + "," +
                               ItemToCSV.Replace('|', ',') + "\r\n";
                        if (numdata == arraycount || Data.Length == arraytotal)
                        {
                            File = File.Substring(0, File.Length - 2);
                            Inserted = Inserted + InsertSuccess(arraycount, ref conn, tmpTable, File);
                            if (Inserted == 0)
                            {
                                output = "error Upload";
                                ermsg = "error in uploading";
                                trans.Rollback();
                                conn.Close();
                                return output;
                            }
                            File = "";
                        }
                        ListSuccess.Add(item + "|Success");
                    }
                    else
                    {
                        errorname = "reference no. already exist";
                        ListError.Add(item + "|" + errorname);
                        
                    }
                }
                else
                {
                    errorname = "Amount is Greater than the Threshold";
                    ListError.Add(item + "|" + errorname);
                }

                ItemToCSV = "";
            }
            if (Inserted == Data.Length)
            {
                output = "success";
                trans.Commit();
                conn.Close();
                return output;
            }
            else
            {
                output = "failed";
                trans.Rollback();
                conn.Close();
                return output;
            }
        }
        catch (Exception ex)
        {
            output = ex.ToString();
            ermsg = ex.ToString();
            trans.Rollback();
            conn.Close();
            return output;
        }
    }
    public string SaveTransactionMOEXUpload(String[] Data, String AccountId, String batchnumforPartners, String bname, String bcode, String stationcode, String zonecode, String operatorID, String stationID, String Currency, String SessionId)
    {
        string output = "";
        string ermsg = "";
        int Inserted = 0;
        filename = "NOWMOEX.csv";
        string ItemToCSV = "";
        ser.ConnectPartners();
        MLbatchNum = ser.generateBatchNo();
        MLbNumber = MLbatchNum;
        tinuodngaFilename = mappath + filename;
        string tmpTable = "kppartners.tmpMOEXTable";

        //CorporateBatchNo = batchnumforPartners;
        //BatchNo = MLbatchNum;

        MySqlConnection conn = ser.dbconkp.getConnection();


        try
        {
            conn.Open();
            command = conn.CreateCommand();
            String pdate = ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
            trans = conn.BeginTransaction();
            command.Transaction = trans;
            foreach (string item in Data)
            {
                column = item.Split('|');
                ItemToCSV = item.Replace(",", "");

                decimal principal = Convert.ToDecimal(column[20].ToString());
                string reference = column[0].ToString();
                string curr = column[21].ToString();
                int zc = Convert.ToInt32(zonecode);

                arraytotal += 1;
                if (ser.CheckTierCode(AccountId, principal, curr).Equals(true))
                {

                    int ret = ser.checkreference(reference, AccountId);
                    if (ret == 0)
                    {

                        arraycount += 1;

                        string ControlNumber = ser.generateControlGlobal(bcode, 0, operatorID, zc, stationID, 1.2, stationcode);
                        if (ControlNumber == "ErrorOnControlNumber")
                        {
                            ermsg = "Error on generating ControlNumber";
                            trans.Rollback();
                            conn.Close();
                            return output = "error Control Number";                            
                        }
                        File = File + "," + ControlNumber + "," + AccountId + "," + batchnumforPartners + "," + MLbatchNum + "," + bname + "," + bcode + "," + stationID + "," +
                               stationcode + "," + operatorID + "," + zonecode + "," + SessionId + "," +
                               ItemToCSV.Replace('|', ',') + "\r\n";

                        if (numdata == arraycount || Data.Length == arraytotal)
                        {
                            File = File.Substring(0, File.Length - 2);
                            Inserted = Inserted + InsertSuccess(arraycount, ref conn, tmpTable, File);
                            if (Inserted == 0)
                            {
                                output = "error in Upload";
                                ermsg = "error in uploading";
                                trans.Rollback();
                                conn.Close();
                                return output;

                            }
                            File = "";
                        }

                        ListSuccess.Add(item + "|Success");
                    }
                    else
                    {
                        errorname = "reference no. already exist";
                        ListError.Add(item + "|" + errorname);
                    }
                }
                else
                {
                    errorname = "Amount is Greater than the Threshold";
                    ListError.Add(item + "|" + errorname);
                }

                ItemToCSV = "";
            }

            if (Inserted == Data.Length)
            {
                output = "success";
                trans.Commit();
                conn.Close();
                return output;
            }
            else
            {
                output = "failed";
                trans.Rollback();
                conn.Close();
                return output;
            }


        }
        catch (Exception ex)
        {
            output = ex.ToString();
            ermsg = ex.ToString();
            trans.Rollback();
            conn.Close();
            return output;
        }
        //return output;
    }
    //private int SubmitBDOtoDatabase(String SessionId)
    //{

    //}
    // private int InsertBDOTable365(ref MySqlConnection conn, string sessionID, string temptable)
    //{
    //    int i = 0;
    //    //int DeleteTmpTbl = 0;
    //    int soUploadInserted = 0;
    //    int CorporateInserted = 0;
    //    int TransLogInserted = 0;
    //    int soTransRefInserted = 0;
    //    bool UpdateTmpTbl = false;
    //    using (command = conn.CreateCommand())
    //    {
    //        DateTime serverdate;
    //        string table;
    //        command.CommandText = "Select NOW() as serverdt;";
    //        using (MySqlDataReader Reader = command.ExecuteReader())
    //        {
    //            Reader.Read();
    //            serverdate = Convert.ToDateTime(Reader["serverdt"]);
    //            Reader.Close();

    //        }

    //        table = serverdate.ToString("MM-dd").Replace("-", "");

    //        //SELECT Fields under Temp Table                   
    //        string sql = "Insert into kppartners.sendout" + table + " (ControlNo, ReferenceNo, Currency, Principal, " +
    //                                                  "Charge, AccountCode, TransDate, " +
    //                                                  "SenderFName, SenderLName, SenderMName, SenderName, ReceiverFName, ReceiverLName, ReceiverMName, " +
    //                                                  "ReceiverName, ReceiverContactNo, " +
    //                                                  "sessionID, OtherDetails, OperatorID, " +
    //                                                  "StationID, KPTN, BranchCode,Total )" +
    //                      "Select ControlNo,refno,currency,principal,charge,Accountid,now(),substring_index(SenderFname,' ',1) as FirstName," +
    //                      "substring_index(substring_index(SenderFname,' ',-3),' ',1) as MiddleName,substring_index(substring_index(SenderFname,' ',-2),' ',1) as FamilyName,"+
    //                      "beneffn,benefln,benefmn,CONCAT(beneffn,' ',benefmn,' ',benefln) AS ReceiverName,'',sessionid, " +
    //                      "SenderFName,CONCAT(prodname,'|',bankbranch,'|',LocatorCode)as Otherdetails, " + 
    //                      " OperatorId,StationNum,kptn,branchcode,TotalAmount " +
    //                      "FROM " + temptable + " where sessionId = '" + sessionID + "'";
    //        command.CommandText = sql;
    //        i = command.ExecuteNonQuery();
    //        NoOfTrans = i;
    //        if (i <= 0)//Error Insert in 365
    //        {
    //            //i = 0;
    //            trans.Rollback();
    //            conn.Close();
    //            return i = 0;
    //        }
    //        else
    //        {
    //            sql = "SELECT DISTINCT Currency,AccountId,batchNumPartner,MlbatchNum FROM " + temptable + " WHERE sessionId = '" + sessionID + "'";
    //            command.CommandText = sql;
    //            List<string> Currency = new List<string>();

    //            using (MySqlDataReader rdr = command.ExecuteReader())
    //            {
    //                if (rdr.HasRows)
    //                {
    //                    while (rdr.Read())
    //                    {
    //                        Currency.Add(rdr[0].ToString());
    //                        AccountCode = rdr[1].ToString();
    //                        CorporateBatchNo = rdr[2].ToString();
    //                        BatchNo = rdr[3].ToString();
    //                    }
    //                    rdr.Close();
    //                    foreach (string item in Currency)
    //                    {
    //                        UpdateTmpTbl = getNewRB(AccountCode, SumAmount(item, sessionID, ref conn, temptable), item, ref conn);
    //                        if (!UpdateTmpTbl)
    //                        {
    //                            trans.Rollback();
    //                            conn.Close();
    //                            return i = 0;
    //                        }
    //                    }
    //                    //i = 1;
    //                }
    //                else
    //                {
    //                    //i = 0;
    //                    trans.Rollback();
    //                    conn.Close();
    //                    return i = 0;
    //                }
    //            }
    //            if (UpdateTmpTbl)
    //            {
    //                soUploadInserted = InsertSoUploadHeader(ref conn);//No need to modify SELECT Query
    //                if (soUploadInserted <= 0)
    //                {
    //                    //i = 0;
    //                    trans.Rollback();
    //                    conn.Close();
    //                    return i = 0;
    //                }
    //                else
    //                {
    //                    CorporateInserted = InsertBDOCorporateSendout(ref conn, temptable, sessionID);
    //                    if (CorporateInserted <= 0)
    //                    {
    //                        trans.Rollback();
    //                        conn.Close();
    //                        return i = 0;
    //                    }
    //                    else
    //                    {
    //                        TransLogInserted = InsertBDOTransactionslog(ref conn, sessionID, temptable);
    //                        if (TransLogInserted <= 0)
    //                        {
    //                            trans.Rollback();
    //                            conn.Close();
    //                            return i = 0;
    //                        }
    //                        else
    //                        {
    //                            /// String pdate = "0802";//ser.getServerDatePartner(true).ToString("MM-dd").Replace("-", "");
    //                            soTransRefInserted = InsertSotxnRef(ref conn, table, sessionID, temptable); //No need to modify SELECT Query
    //                            if (soTransRefInserted <= 0)
    //                            {
    //                                trans.Rollback();
    //                                conn.Close();
    //                                return i = 0;
    //                            }
    //                            else
    //                            {
    //                                trans.Commit();
    //                                conn.Close();
    //                                return i = 1;
    //                            }
    //                        }
    //                    }
    //                }

    //            }
    //        }
    //    }
    //    return i;
    //}

    //----------------------------------------------Function------------------------------------------------------------------

    private int ClearTmpTable(ref MySqlConnection con,string table,string SessionID)
    {
      int i = 0;
      string sql;
      using(command = con.CreateCommand())
      {
          sql = "DELETE FROM " + table + "WHERE sessionID = '" + SessionID + "' ";
          command.CommandText = sql;
          i = command.ExecuteNonQuery();
      }
      return i = i <= 0 ? 0 : 1;
    }

    private int InsertSuccess(int arraycount,ref MySqlConnection conn,string pDate,string str)
    {        
            CreateLog(str, filename);
            int insert = BulkInsert(ref conn, pDate, tinuodngaFilename);
            int Reply = 0;


            if (insert != arraycount)
            {
                Reply = 0;
                return Reply;
            }
            System.IO.File.Delete(mappath + filename);
            Reply = insert;
            arraycount = 0;
            return Reply;
    }

    private void CreateLog(string str, string filename)
    {
        filename = mappath + filename;
        FileStream objFile;
        System.IO.StreamWriter objWriter;
        if (System.IO.File.Exists(filename))
        {
            System.IO.File.Delete(filename);
        }

        objFile = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write);

        objWriter = new System.IO.StreamWriter(objFile);
        objWriter.BaseStream.Seek(0, SeekOrigin.End);
        objWriter.WriteLine(str);
        objWriter.Close();
        
    }

    public bool getNewRB(String accid, Decimal TotalAmount, String currency,ref MySqlConnection conn)
    {
        decimal NeWr;
        bool result = false;
        //Int16 ret = 0;
        NeWr = getRBalance(accid, TotalAmount, currency,ref conn);
        result = UpdateRunningBalance(NeWr, accid, currency,ref conn);
        return result;
    }

    public decimal SumAmount(string Currency, string sessionid,ref MySqlConnection con,string tableName)
    {
        decimal Sum;
        using(command = con.CreateCommand())
        {
            string sql = "select (sum(principal) + sum(charge)) as totalPrincipal from " + tableName + " where Currency = '" + Currency + "' and sessionid = '" + sessionid + "'";
            command.CommandText = sql;
            using (MySqlDataReader rdr = command.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    rdr.Read();
                    Sum = Convert.ToDecimal(rdr[0]);
                }
                else
                {
                    Sum = 0;
                }                
            }
        }
        return Sum;
    }

    private int BulkInsert(ref MySqlConnection conn,string table,string fname)
    {
        int insert = 0;
        
        var bl = new MySqlBulkLoader(conn);
        bl.TableName = table;
        bl.FieldTerminator = ",";
        bl.LineTerminator = "\n";
        bl.FileName = fname;
        bl.NumberOfLinesToSkip = 0;
        insert = bl.Load();

        return insert;
    }

    private bool UpdateRunningBalance(Decimal NewRbalance, String AccId, String Currency,ref MySqlConnection conn)
    {
        bool res = false;               
            try
            {               
                using (command = conn.CreateCommand())
                {
                    string query = " UPDATE kpadminpartners.accountdetail Set runningbalance = @NewRbalance where accountid = @AccId and currency = @Currency";
                    command.CommandText = query;
                    command.Parameters.AddWithValue("NewRbalance", NewRbalance);
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Currency", Currency);
                    int res1 = command.ExecuteNonQuery();
                    if (res1 <= 0)
                    {
                        res = false;
                    }
                    else
                    {
                        res = true;
                    }
                }                
            }
            catch (Exception ex)
            {
                res = false;
            }      
        return res;
    }

    private decimal getRBalance(String AccId, Decimal TINAmount, String Currency, ref MySqlConnection con)
    {
        decimal NewBalance = 0;
                using (command = con.CreateCommand())
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
        return NewBalance;
    }
    
}
//------------------------------------------------