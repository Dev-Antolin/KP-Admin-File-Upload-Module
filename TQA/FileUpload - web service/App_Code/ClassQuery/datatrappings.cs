using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MySql.Data.MySqlClient;
using log4net;





public class datatrappings
{
    public DBConnection dbconkp;
    private String pathdomestic;
    public MySqlCommand command;
    public MySqlTransaction trans = null;
    public String batchnumber;

    private static readonly ILog kplog = LogManager.GetLogger(typeof(Service));
    private decimal getRBalance(String AccId, Decimal TINAmount, String Currency)
    {
        decimal NewBalance = 0;
        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
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
                using (command = con.CreateCommand())
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

    private Boolean CheckTotalAmount(String AccId, Decimal totalAmount, String Currency)
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

    public string getChargeTypeforTiercode(string Accountid, string Currency)
    {
        string value = string.Empty;
     
            try
            {
               
                    string sql = "select chargetype,thresholdamount from kpadminpartners.accountdetail where accountid = '" + Accountid + "' and currency = '" + Currency + "' ";
                    command.CommandText = sql;
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            value = rdr["chargetype"].ToString() + "|" + rdr["thresholdamount"].ToString();
                        }
                    }
               
            }
            catch (Exception ex)
            {
               
                throw new Exception(ex.Message);
            }
       
        return value;
    }


    public Boolean CheckTierCode(String AccId, Decimal amount, String Currency)
    {
        decimal minAmount, maxAmount, charge, thold, Rbalance, credlim;
        string Tcode,type,cType,thold2;
        int ret = 0;
        bool datareturn = false;
        type = getChargeTypeforTiercode(AccId, Currency);
        string[] datasplit = type.Split('|');
        cType = datasplit[0].ToString();
        thold2 = datasplit[1].ToString();
       
        if (cType == "Tier Bracket")
        {
            try
            {

                //string query = "select b.Minimum,b.Maximum,b.TierCode,b.Charge,c.Threshold,c.runningbalance,c.creditlimit from kpadminpartners.tierdetails b " +
                //                "inner join kpadminpartners.accountcharging a on b.TierCode = a.BracketTierCode " +
                //                "inner join kpadminpartners.accountlist c on a.AccountID = c.AccountID " +
                //                "where a.AccountID = @AccId";
                string query = "select b.Minimum,b.Maximum,b.TierCode,b.Chargeamount,a.thresholdamount,a.runningbalance,a.creditlimit from kpadminpartners.tierdetails b " +
                               "inner join kpadminpartners.accountdetail a on b.TierCode = a.BracketTierCode " +
                               "where a.accountid = @AccId and a.currency = @Currency";

                command.Parameters.Clear();
                command.Parameters.AddWithValue("AccId", AccId);
                command.Parameters.AddWithValue("Currency", Currency);
                command.CommandText = query;
                using (MySqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                        maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                        Tcode = rdr["TierCode"].ToString();
                        charge = Convert.ToDecimal(rdr["Chargeamount"].ToString());
                        //thold = maxAmount; //Convert.ToDecimal(rdr["thresholdamount"]);
                        Rbalance = Convert.ToDecimal(rdr["runningbalance"].ToString());
                        credlim = Convert.ToDecimal(rdr["creditlimit"].ToString());
                        if (amount <= maxAmount && (amount >= minAmount && amount <= maxAmount))
                        {
                            datareturn = true;

                        }
                        ret = 1;
                    }
                }

                if (ret == 0)
                {
                    datareturn = checkTiercodeBillsPay(AccId, amount, Currency);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        else
        {
            if (string.IsNullOrEmpty(thold2) & thold2 == null) 
            {
                thold = 0;
            }
            else
            {
                thold = Convert.ToDecimal(thold2);
            }
            if (amount <= thold)
            {
                datareturn = true;

            }
        }
        return datareturn;
    }

    private Decimal GetChargeValue(String AccId, String Curr, Decimal amount)
    {
        decimal charge = 0;
        int result = 0;

        using (MySqlConnection con = dbconkp.getConnection())
        {
            try
            {
                con.Open();
                using (command = con.CreateCommand())
                {
                    string sql = "select b.Minimum,b.Maximum,b.TierCode,b.Chargeamount,a.thresholdamount from kpadminpartners.tierdetails b " +
                                   "inner join kpadminpartners.accountdetail a on b.TierCode = a.BracketTierCode " +
                                   "where a.accountid = @AccId and Currency = @Curr";
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Curr", Curr);
                    command.CommandText = sql;
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            decimal minAmount = Convert.ToDecimal(rdr["Minimum"]);
                            decimal maxAmount = Convert.ToDecimal(rdr["Maximum"]);
                            decimal thold = Convert.ToDecimal(rdr["thresholdamount"]);
                            decimal chargeAmount = Convert.ToDecimal(rdr["Chargeamount"]);
                            if (amount <= thold && (amount >= minAmount && amount <= maxAmount))
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
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("AccId", AccId);
                    command.Parameters.AddWithValue("Currency", Currency);
                    command.CommandText = query;
                    using (MySqlDataReader rdr = command.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            minAmount = Convert.ToDecimal(rdr["Minimum"].ToString());
                            maxAmount = Convert.ToDecimal(rdr["Maximum"].ToString());
                            Thold = Convert.ToDecimal(rdr["thresholdamount"]);
                            charge = Convert.ToDecimal(rdr["chargeamount"].ToString());
                            if (amount <= Thold && (amount > minAmount && amount < maxAmount))
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
        decimal minAmount, maxAmount, Thold, charge;
       
            try
            {
              
                    string query = "select b.Minimum, b.Maximum, b.ChargeCode, b.chargeamount, a.thresholdamount,a.runningbalance,a.creditlimit from kpadminpartners.billspaychargedetails b " +
                                    "inner join kpadminpartners.accountdetail a on b.ChargeCode = a.BracketTierCode " +
                                    "where a.AccountID = @AccId and a.currency = @Currency";
                    command.Parameters.Clear();
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
                            if (amount <= Thold && (amount > minAmount && amount < maxAmount))
                            {
                                ret = true;
                               
                            }
                        }
                    }
              
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
       
        return ret;
    }

    public Boolean checkCreditLimit(String AccId, Decimal Amount, String Currency)
    {
        int credLimstat;
        decimal credLim;
        bool ret = false;
     
            try
            {
               
                    string query = "Select creditlimit,creditactivation from kpadminpartners.accountdetail where AccountID = @AccId and currency = @Currency";
                    command.Parameters.Clear();
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
                   
                }
           
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
       

        return ret;
    }



    public void ConnectPartners()
    {
        try
        {
            //string path = httpcontext.current.server.mappath("boskpws.ini");
            pathdomestic = "C:\\kpconfig\\DBPartner.ini";
            IniFile ini = new IniFile(pathdomestic);

            String Serv = ini.IniReadValue("DBConfig Partner", "Server");
            String DB = ini.IniReadValue("DBConfig Partner", "Database"); ;
            String UID = ini.IniReadValue("DBConfig Partner", "UID"); ;
            String Password = ini.IniReadValue("DBConfig Partner", "Password"); 
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


    public int insertData(String AccountId, String batchnum, Int32 numtrxn, String Bnumber)
    {
        string fromserverdate;
        int ret = 0;
        try
        {
            String query = "Select NOW() as serverdt";
            command.CommandText = query;
            using (MySqlDataReader rdr = command.ExecuteReader())
            {
                rdr.Read();
                fromserverdate = rdr["serverdt"].ToString();
                rdr.Close();



                String sql = "insert into kppartners.souploadheader (BatchNo,AccountCode,NoOfTrxn,UploadDate,CorporateBatchNo) values (@batchnum,@AccountId,@numtrxn,@Date,@Bnumber)";
                command.CommandText = sql;
                command.Parameters.AddWithValue("batchnum", batchnum);
                command.Parameters.AddWithValue("AccountId", AccountId);
                command.Parameters.AddWithValue("numtrxn", numtrxn);
                command.Parameters.AddWithValue("Date", Convert.ToDateTime(fromserverdate));
                command.Parameters.AddWithValue("Bnumber", Bnumber);
                ret = command.ExecuteNonQuery();





            }


        }
        catch (Exception ex)
        {
            return ret = -1;
            throw new Exception(ex.ToString());
        }
        return ret;
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

    public Int32 checkreference(String refno, String AccountCode)
    {

        int flag;


        String query = "Select ReferenceNo from kppartners.sotxnref where ReferenceNo = @refnumber and AccountCode = @AccountCode and CancelledDate is null";
        command.CommandText = query;
        command.Parameters.Clear();
        command.Parameters.AddWithValue("refnumber", refno);
        command.Parameters.AddWithValue("AccountCode", AccountCode);
        using (MySqlDataReader rdr = command.ExecuteReader())
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


    

   
    public String generateControlGlobal(String branchcode, Int32 type, String OperatorID, Int32 ZoneCode, String StationNumber, Double version, String stationcode)
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

            return "ErrorOnControlNumber";
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
                   
                        conn.Open();
                 

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

    public int insertTransactionslogs(String kptn, String ctrlNo, String stationcode, String ControlNumber, String bname, String operatorID, String stationID, String zonecode, String AccountCode, String Currency, String senderfullname, String receiverfullname, Decimal principal)
    {
        int ret = 0;
        try
        {

            command.CommandText = "insert into kpadminpartnerslog.transactionslogs(kptnno,refno,AccountCode,action,type,branchcode,operatorid,stationcode,zonecode,stationno,txndate,currency,sendername,receivername,principal,controlno)" +
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
            int result = command.ExecuteNonQuery();
            if (result == -1)
            {
                ret = result;
            }
            else
            {
                ret = result;
            }

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
        return ret;
    }

    

    public string InsertCorporateTrans(string controlno, string kptn, string oldkptn, string referenceno, string accountid, string currency, DateTime transdate, int stationno, int isremote, string operatorid, string branchcode, int zonecode, string remoteoperatorid, string remotebranchcode, int remotezonecode, decimal principal, string chargeto, decimal chargeamount, decimal othercharge, decimal total, decimal forex, string traceno, string sessionid, string senderfname, string senderlname, string sendermname, string receiverfname, string receiverlname, string receivermname, int isactive)
    {

       

            string query = "insert into kppartnerstransactions.corporatesendouts (controlno,kptn,oldkptn,referenceno," +
                "accountid,currency,transdate,stationno,isremote,operatorid,branchcode,zonecode,remoteoperatorid," +
                "remotebranchcode,remotezonecode,principal,chargeto,chargeamount,othercharge,total,forex,traceno," +
                "sessionid,senderfname,senderlname,sendermname,receiverfname,receiverlname,receivermname,isactive)" +
                " values('" + controlno + "','" + kptn + "','" + oldkptn + "','" + referenceno + "'," +
                "'" + accountid + "','" + currency + "','" + transdate.ToString("yyyy-MM-dd HH:mm:ss") + "','" + stationno + "','" + isremote + "','" + operatorid + "','" + branchcode + "','" + zonecode + "','" + remoteoperatorid + "'," +
                "'" + remotebranchcode + "','" + remotezonecode + "','" + principal + "','" + chargeto + "','" + chargeamount + "','" + othercharge + "','" + total + "','" + forex + "','" + traceno + "'," +
                "'" + sessionid + "','" + senderfname + "','" + senderlname + "','" + sendermname + "','" + receiverfname + "','" + receiverlname + "','" + receivermname + "','" + isactive + "')";

 
            return query;
    }

  
 
}
