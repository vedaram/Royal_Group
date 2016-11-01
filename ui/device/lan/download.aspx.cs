using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Configuration;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class lan_download : System.Web.UI.Page
{
    const string page = "LAN_TEMPLATE_DOWNLOAD";

    protected void Page_Load(object sender, EventArgs e)
    {

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string message = string.Empty;

        try
        {

            if (Session["username"] == null)
            {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex)
        {

            message = "An error occurred while loading Template Download. Please try again. If the error persists, please contact Support.";

            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    public static zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
    [DllImport("Kernel32.dll")]
    public static extern bool RtlMoveMemory(ref AnvizNew.CLOCKINGRECORD Destination, int Source, int Length);
    [DllImport("Kernel32.dll")]
    public static extern bool RtlMoveMemory(ref AnvizNew.PERSONINFO Destination, int Source, int Length);
    [DllImport("Kernel32.dll")]
    public static extern bool RtlMoveMemory(ref int Destination, int Source, int Length);
    [DllImport("Kernel32.dll")]
    public static extern bool RtlMoveMemory(ref byte Destination, int Source, int Length);
    [DllImport("Kernel32.dll")]
    public static extern void GetLocalTime(ref AnvizNew.SYSTEMTIME lpSystemTime);

    [WebMethod]
    public static ReturnObject getDeviceData(int page_number)
    {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable device_data = new DataTable();
        int start_row = (page_number - 1) * 30;
        int number_of_record = (page_number * 30) + 1;
        string query = string.Empty;

        try
        {

            query = "select deviceid, deviceip, communation, devicename, devicemodel, devicetype, category, status from ( select deviceid, deviceip, communation, devicename, devicemodel, devicetype, category, status, ROW_NUMBER() OVER (ORDER BY deviceid) as row from deviceinfo) a where row > " + start_row + " and row < " + number_of_record;

            device_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(device_data, Formatting.Indented);
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_DEVICE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occured while loading Device Data. Please refresh the page and try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    public int connectanvizdevice(int device_id, string device_ip, string communication_type)
    {

        int status = 0;

        switch (communication_type)
        {

            case "LAN":
                if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) { }
                status = AnvizNew.CKT_RegisterNet(device_id, device_ip);
                break;

            case "WAN":
                int pLongRun = new int();
                if (AnvizNew.CKT_ChangeConnectionMode(1) != 1) { }
                status = AnvizNew.CKT_NetDaemonWithPort(5010);
                if (status == 1)
                {
                    Thread.Sleep(5000);
                    status = AnvizNew.CKT_GetClockingRecordEx(device_id, ref pLongRun);
                }
                break;

            case "USB":
                if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) { }
                status = AnvizNew.CKT_RegisterUSB(device_id, 0);
                break;

            case "DNS":
                if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) { }
                IPAddress[] addresslist = Dns.GetHostAddresses(device_ip);
                device_ip = Convert.ToString(addresslist[0]);
                status = AnvizNew.CKT_RegisterNet(device_id, device_ip);
                break;
        }

        return status;
    }

    private DataTable getEnrollment(int device_id)
    {

        AnvizNew.PERSONINFO person = new AnvizNew.PERSONINFO();
        DBConnection db_connection = new DBConnection();
        DataTable employee_details = new DataTable();
        DataTable enrollments = new DataTable();
        string query = string.Empty;
        string employee_name = string.Empty;
        int i = 0;
        int RecordCount = new int();
        int RetCount = new int();
        int pPersons = new int();
        int pLongRun = new int();
        int ptemp = 0;
        int status = AnvizNew.CKT_ListPersonInfoEx(device_id, ref pLongRun);

        enrollments.Columns.Add("status");
        enrollments.Columns.Add("EnrollId");
        enrollments.Columns.Add("Employeename");

        query = "truncate table exportenrollid";
        db_connection.ExecuteQuery_WithReturnValueString(query);

        if (status == 1)
        {

            while (true)
            {
                status = AnvizNew.CKT_ListPersonProgress(pLongRun, ref RecordCount, ref RetCount, ref pPersons);

                if (status != 0)
                {
                    ptemp = Marshal.SizeOf(person);
                    for (i = 0; i < RetCount; i++)
                    {

                        RtlMoveMemory(ref person, pPersons, ptemp);
                        pPersons = pPersons + ptemp;

                        query = "select emp_name from Employeemaster where emp_card_no = '" + person.PersonID.ToString() + "'";
                        employee_details = db_connection.ReturnDataTable(query);

                        if (employee_details.Rows.Count > 0) employee_name = employee_details.Rows[0]["emp_name"].ToString();

                        string[] hu = { "", person.PersonID.ToString(), employee_name };

                        query = "insert into exportenrollid(enrollid, empname)values('" + person.PersonID.ToString() + "','" + employee_name + "')";
                        db_connection.ExecuteQuery_WithReturnValueString(query);

                        enrollments.Rows.Add(hu);
                    } // end of for loop
                } // end of if condition

                if (status == 1) break;
            } // end of while
        }

        return enrollments;
    }

    private DataTable getBioSecurityEnrollment(int device_id, string device_ip)
    {

        DBConnection db_connection = new DBConnection();
        DataTable enrollments = new DataTable();
        DataTable employee_details = new DataTable();
        string employee_name = string.Empty;
        string query = string.Empty;
        string sCardnumber = string.Empty;
        string sdwEnrollNumber = string.Empty;
        string sName = string.Empty;
        string sPassword = string.Empty;
        bool bEnabled = false;
        int iMachineNumber = device_id;
        int iPrivilege = 0;
        int i = 0;

        enrollments.Columns.Add("status");
        enrollments.Columns.Add("EnrollId");
        enrollments.Columns.Add("Employeename");

        query = "truncate table exportenrollid";
        db_connection.ExecuteQuery_WithReturnValueString(query);

        //bool flag = axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory

        //get user information from memory
        while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
        {

            if (axCZKEM1.GetStrCardNumber(out sCardnumber))
            { //get the card number from the memory 

                query = "select emp_name from Employeemaster where emp_card_no='" + sdwEnrollNumber.ToString() + "'";
                employee_details = db_connection.ReturnDataTable(query);

                if (employee_details.Rows.Count > 0) employee_name = employee_details.Rows[0]["emp_name"].ToString();

                string[] hu = { "", sdwEnrollNumber.ToString(), employee_name };
                query = "insert into exportenrollid(enrollid, empname)values('" + sdwEnrollNumber.ToString() + "','" + employee_name + "')";
                db_connection.ExecuteQuery_WithReturnValueString(query);

                enrollments.Rows.Add(hu);
            }
        }

        return enrollments;
    }

    [WebMethod]
    public static ReturnObject getEnrollmentData(string current)
    {

        lan_download page_object = new lan_download();
        DBConnection db_connection = new DBConnection();
        DataTable enrollment_details = new DataTable();
        ReturnObject return_object = new ReturnObject();
        string communication_type = string.Empty;
        string device_ip = string.Empty;
        string device_type = string.Empty;
        string device_model = string.Empty;
        bool is_connected = false;
        int device_id = 0;
        int status = 0;
        int i = 0;

        try
        {

            enrollment_details.Clear();

            JObject current_data = JObject.Parse(current);
            device_id = Convert.ToInt32(current_data["deviceid"]);
            device_ip = current_data["deviceip"].ToString();
            device_type = current_data["devicetype"].ToString();
            device_model = current_data["devicemodel"].ToString();
            communication_type = current_data["communation"].ToString();

            switch (device_type)
            {

                case "Anviz":
                    if (device_model == "T5") device_id = 0;

                    status = page_object.connectanvizdevice(device_id, device_ip, communication_type);
                    if (status != 0)
                    {
                        enrollment_details = page_object.getEnrollment(device_id);

                        return_object.status = "success";
                        return_object.return_data = JsonConvert.SerializeObject(enrollment_details, Formatting.Indented);
                    }
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;

                case "BioSecurity":
                    is_connected = axCZKEM1.Connect_Net(device_ip, Convert.ToInt32(4370));
                    if (is_connected == true)
                    {
                        enrollment_details = page_object.getBioSecurityEnrollment(device_id, device_ip);

                        return_object.status = "success";
                        return_object.return_data = JsonConvert.SerializeObject(enrollment_details, Formatting.Indented);
                    }
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;
            }
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "GET_ENROLLMENT_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while getting enrollment data for the Device. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    public string getDrives()
    {

        string d = "";

        foreach (var drive in DriveInfo.GetDrives())
        {
            int index = 0;
            int flag = 0;
            string drivename = drive.Name;
            DriveType drivetypes = drive.DriveType;
            switch (drivetypes)
            {
                case DriveType.Fixed:
                    d = drivename;
                    index = d.IndexOf('C');
                    if (index == -1)
                    {
                        d = drivename;
                        flag = 1;
                    }
                    break;
            }
            if (flag == 1)
            {
                break;
            }
        }
        return d;
    }

    private void downloadBioSecurity(int device_id, string employees)
    {

        DBConnection db_connection = new DBConnection();
        List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);
        Hashtable enrollment_details = new Hashtable();
        DataTable temp_data_table = new DataTable();
        string query = string.Empty;
        string sdwEnrollNumber = string.Empty;
        string sName = string.Empty;
        string sPassword = string.Empty;
        string sCardnumber = string.Empty;
        string FaceTemp = string.Empty;
        string data = string.Empty;
        string sTmpData = string.Empty;
        string TempData = string.Empty;
        bool Update = false;
        bool bEnabled = false;
        int iFaceIndex = 50;
        int iLength = 0;
        int iFlag = 0;
        int Length = 0;
        int iMachineNumber = device_id;
        int FaceIndx = 0;
        int FaceLength = 0;
        int iPrivilege = 0;
        int enroll_id = 0;
        int count = 0;
        int i = 0;

        for (i = 0; i < employees_list.Count; i++)
        {

            enroll_id = Convert.ToInt32(employees_list[i]);
            query = "select COUNT(*) from downloadtemp where enrollid='" + enroll_id + "' ";
            count = db_connection.GetRecordCount(query);

            if (count <= 0)
            {
                query = "insert into downloadtemp(EnrollId,Name,CardNo,Password,Privilege,FaceIndx,FaceTempData,FingerTempData1,FingerTempData2,Length,Enabled)values('" + enroll_id + "','','','',0,0,'','','',0,'')";
                db_connection.ExecuteQuery_WithOutReturnValue(query);
            }

            axCZKEM1.ReadAllUserID(device_id);//read all the user information to the memory
            while (axCZKEM1.SSR_GetAllUserInfo(device_id, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            { //get user information from memory

                query = "Select * from DownloadTemp where EnrollId = " + sdwEnrollNumber;
                count = db_connection.GetRecordCount(query);

                if (count > 0)
                {
                    Update = true;
                    query = "delete downloadtemp where EnrollId = '" + sdwEnrollNumber + "' ";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                    //get the card number from the memory
                    if (axCZKEM1.GetStrCardNumber(out sCardnumber)) enrollment_details.Add("CardNo", Convert.ToInt32(sCardnumber));

                    enrollment_details.Add("EnrollId", Convert.ToInt32(sdwEnrollNumber));
                    enrollment_details.Add("DeviceName", sName);
                    enrollment_details.Add("password", sPassword);
                    enrollment_details.Add("previlage", iPrivilege);

                    if (axCZKEM1.GetUserFaceStr(device_id, sdwEnrollNumber, iFaceIndex, ref sTmpData, ref iLength))
                    { //get the face templates from the memory 
                        TempData = sTmpData.ToString();
                        query = "select count(*) from DownloadTemp where Enrollid='" + sdwEnrollNumber + "'";
                        count = db_connection.GetRecordCount(query);
                        if (count == 0)
                        {
                            query = "insert into DownloadTemp(EnrollId,Name,CardNo,Password,Privilege,FaceIndx,FaceTempData,Length,Enabled) values('" + sdwEnrollNumber.ToString() + "','" + sName.ToString() + "','" + sCardnumber.ToString() + "','" + sPassword.ToString() + "','" + iPrivilege + "','" + iFaceIndex + "','" + TempData.ToString() + "','" + iLength + "','" + bEnabled + "')";
                            db_connection.ExecuteQuery_WithOutReturnValue(query);
                        }
                        else
                        {
                            query = "update DownloadTemp set Name='" + sName.ToString() + "',Password='" + sPassword.ToString() + "',Privilege='" + iPrivilege + "',FaceIndx='" + iFaceIndex + "',FaceTempData='" + sTmpData.ToString() + "',Length='" + iLength + "',Enabled='" + bEnabled + "' where EnrollId='" + sdwEnrollNumber.ToString() + "'";
                            db_connection.ExecuteQuery_WithOutReturnValue(query);
                        }
                    }

                    query = "Select * from DownloadTemp where EnrollId = " + sdwEnrollNumber;
                    temp_data_table = db_connection.ReturnDataTable(query);
                    
                    if (temp_data_table.Rows.Count > 0)
                    {
                        if (temp_data_table.Rows[0]["FaceIndx"].ToString() != "")
                            FaceIndx = Convert.ToInt32(temp_data_table.Rows[0]["FaceIndx"].ToString());
                        else
                            FaceIndx = 0;

                        if (temp_data_table.Rows[0]["FaceTempData"].ToString() != "")
                            FaceTemp = temp_data_table.Rows[0]["FaceTempData"].ToString();
                        else
                            FaceTemp = string.Empty;

                        if (temp_data_table.Rows[0]["Length"].ToString() != "")
                            FaceLength = Convert.ToInt32(temp_data_table.Rows[0]["Length"].ToString());
                        else
                            FaceLength = 0;
                    }

                    enrollment_details.Add("faceindex", FaceIndx);
                    enrollment_details.Add("faceTempData", FaceTemp);

                    for (int idwFingerIndex = 0; idwFingerIndex < 2; idwFingerIndex++)
                    {
                        //get the corresponding templates string and length from the memory
                        if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out data, out Length))
                            enrollment_details.Add("Finger" + (idwFingerIndex + 1).ToString(), data);
                        else
                            enrollment_details.Add("Finger" + (idwFingerIndex + 1).ToString(), string.Empty);
                    }

                    enrollment_details.Add("length", FaceLength);

                    if (bEnabled == true)
                        enrollment_details.Add("Enabled", "True");
                    else
                        enrollment_details.Add("Enabled", "false");

                    db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("sp_DownloadData", enrollment_details);

                    axCZKEM1.RefreshData(iMachineNumber);

                    enrollment_details.Clear();
                }
            }
        }
    }

    private void downloadCard(int device_id, string device_ip, string employees)
    {
        List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);
        DBConnection db_connection = new DBConnection();
        AnvizNew.PERSONINFO person = new AnvizNew.PERSONINFO();
        DataTable employee_details = new DataTable();
        string enroll_id = string.Empty;
        string employee_name = string.Empty;
        string employee_id = string.Empty;
        string password = string.Empty;
        string card_id = string.Empty;
        string query = string.Empty;
        int RecordCount = new int();
        int RetCount = new int();
        int pPersons = new int();
        int pLongRun = new int();
        int cardcount = 0;
        long Cardlong = 0;
        int i = 0;
        int status = 0;
        int ptemp;

        if (AnvizNew.CKT_ListPersonInfoEx(device_id, ref pLongRun) == 1)
        {

            while (true)
            {

                status = AnvizNew.CKT_ListPersonProgress(pLongRun, ref RecordCount, ref RetCount, ref pPersons);
                if (status != 0)
                {
                    //if (RecordCount > 0) ProgressBar1.Maximum = RetCount;
                    ptemp = Marshal.SizeOf(person);
                    for (i = 0; i < RetCount; i++)
                    {
                        RtlMoveMemory(ref person, pPersons, ptemp);
                        pPersons = pPersons + ptemp;

                        string[] hu = { i.ToString(), person.PersonID.ToString(), Encoding.Default.GetString(person.Name).ToString(), Encoding.Default.GetString(person.Password).ToString(), person.CardNo.ToString() };
                        enroll_id = person.PersonID.ToString();
                        if (employees_list.Exists(element => element == enroll_id))
                        {
                            employee_name = Encoding.Default.GetString(person.Name).ToString();
                            password = Encoding.Default.GetString(person.Password).ToString();
                            password = System.Text.RegularExpressions.Regex.Replace(password, "[^0-9_.]+", "", System.Text.RegularExpressions.RegexOptions.Compiled);
                            password = password.TrimEnd('0');

                            card_id = person.CardNo.ToString();
                            Cardlong = Convert.ToInt64(card_id);

                            if (Cardlong < -1)
                            {
                                Cardlong = Cardlong + 4294967296;
                                card_id = Convert.ToString(Cardlong);
                            }

                            if (card_id == "-1") card_id = "";
                            if (employee_name.StartsWith("\0")) employee_name = "";

                            cardcount = cardcount + 1;

                            if (db_connection.RecordExist("select count(emp_card_no) from employeemaster where emp_card_no='" + enroll_id + "' "))
                            {

                                query = "select emp_code,emp_name as Name from Employeemaster where emp_card_no='" + enroll_id + "'";
                                employee_details.Rows.Clear();
                                employee_details = db_connection.ReturnDataTable(query);
                                employee_id = employee_details.Rows[0]["emp_code"].ToString();
                                employee_name = employee_details.Rows[0]["Name"].ToString();

                            }

                            if (db_connection.RecordExist("select count(*) from Enrollmaster where enrollid='" + enroll_id + "'"))
                            {

                                query = "update Enrollmaster set cardid='" + card_id + "',pin='" + password + "',empid='" + employee_id + "',name='" + employee_name + "' where enrollid='" + enroll_id + "'";
                                db_connection.ExecuteQuery_WithOutReturnValue(query);
                            }
                            else
                            {
                                if (password == "\0\0\0\0\0\0\0\0") password = "";

                                query = "Insert into Enrollmaster(enrollid,cardid,pin,empid,name) values('" + enroll_id + "','" + card_id + "','" + password + "','" + employee_id + "','" + employee_name + "')";
                                db_connection.ExecuteQuery_WithOutReturnValue(query);
                            }
                        }
                    }

                    if (status == 1) break;
                }
                else
                {
                    // Add logic for logging error to log file.
                }
            }
        }
    }

    private void saveFingerPrint(int device_id, string employees)
    {

        lan_download page_object = new lan_download();
        List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);
        string drive = page_object.getDrives();
        string keyname = "FingerPrintLocation";
        string pat = ConfigurationManager.AppSettings["FingerPrintLocation"];
        string pat1 = pat.Substring(0, 3);
        string pat2 = pat.Substring(3);
        int enroll_id = 0;
        int i = 0;

        string path = string.Empty, path1 = string.Empty, path2 = string.Empty, path3 = string.Empty, path4 = string.Empty, path5 = string.Empty, path6 = string.Empty, path7 = string.Empty, path8 = string.Empty, path9 = string.Empty;
        int status = 0, status1 = 0, status2 = 0, status3 = 0, status4 = 0, status5 = 0, status6 = 0, status7 = 0, status8 = 0, status9 = 0;

        if (drive == pat1)
        {
            if (!Directory.Exists(pat))
            {
                Directory.CreateDirectory(pat);
            }
        }
        else
        {
            string newdrive = drive + pat2;
            if (!Directory.Exists(newdrive))
            {
                Directory.CreateDirectory(newdrive);
            }
            Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");

            // var section = WebConfigurationManager.GetSection("appSettings");
            //webConfigApp = section as Configuration;
            string Key_c = keyname;
            string Value_c = newdrive;

            webConfigApp.AppSettings.Settings[Key_c].Value = Value_c;

            webConfigApp.Save();
        }

        for (i = 0; i < employees_list.Count; i++)
        {

            enroll_id = Convert.ToInt32(employees_list[i]);
            path = pat + enroll_id + ".anv";
            path1 = pat + enroll_id + "_" + 1 + ".anv";
            path2 = pat + enroll_id + "_" + 2 + ".anv";
            path3 = pat + enroll_id + "_" + 3 + ".anv";
            path4 = pat + enroll_id + "_" + 4 + ".anv";
            path5 = pat + enroll_id + "_" + 5 + ".anv";
            path6 = pat + enroll_id + "_" + 6 + ".anv";
            path7 = pat + enroll_id + "_" + 7 + ".anv";
            path8 = pat + enroll_id + "_" + 8 + ".anv";
            path9 = pat + enroll_id + "_" + 9 + ".anv";

            var file = Directory.GetFiles(pat, enroll_id + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file1 = Directory.GetFiles(pat, enroll_id + "_" + 1 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file2 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 2 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file3 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 3 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file4 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 4 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file5 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 5 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file6 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 6 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file7 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 7 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file8 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 8 + ".anv", SearchOption.TopDirectoryOnly).Length;
            var file9 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 9 + ".anv", SearchOption.TopDirectoryOnly).Length;

            if (file == 0)
            {
                status = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 0, path);

                if (file1 == 0) status1 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 1, path1);
                if (file2 == 0) status2 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 2, path2);
                if (file3 == 0) status3 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 3, path3);
                if (file4 == 0) status4 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 4, path4);
                if (file5 == 0) status5 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 5, path5);
                if (file6 == 0) status6 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 6, path6);
                if (file7 == 0) status7 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 7, path7);
                if (file8 == 0) status8 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 8, path8);
                if (file9 == 0) status9 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 9, path9);
            }
            else
            {

                status = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 0, path);
                status1 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 1, path1);
                status2 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 2, path2);
                status3 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 3, path3);
                status4 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 4, path4);
                status5 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 5, path5);
                status6 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 6, path6);
                status7 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 7, path7);
                status8 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 8, path8);
                status9 = AnvizNew.CKT_GetFPTemplateSaveFile(device_id, enroll_id, 9, path9);
            }
        }

    }

    [WebMethod]
    public static ReturnObject downloadTemplates(string device, string employees)
    {

        lan_download page_object = new lan_download();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string device_ip = string.Empty;
        string device_type = string.Empty;
        string device_model = string.Empty;
        string communication_type = string.Empty;
        string device_category = string.Empty;
        string query = string.Empty;
        bool is_connected = false;
        int device_id = 0;
        int status = 0;
        int i = 0;

        try
        {

            JObject device_data = JObject.Parse(device);
            device_id = Convert.ToInt32(device_data["deviceid"]);
            device_ip = device_data["deviceip"].ToString();
            device_type = device_data["devicetype"].ToString();
            device_category = device_data["category"].ToString();
            device_model = device_data["devicemodel"].ToString();
            communication_type = device_data["communation"].ToString();

            switch (device_type)
            {

                case "Anviz":
                    status = page_object.connectanvizdevice(device_id, device_ip, communication_type);

                    if (status != 0)
                    {

                        if (device_model == "T5") device_id = 0;

                        switch (device_category)
                        {

                            case "FCP":
                                page_object.saveFingerPrint(device_id, employees);
                                page_object.downloadCard(device_id, device_ip, employees);
                                break;
                            case "FP":
                                page_object.saveFingerPrint(device_id, employees);
                                page_object.downloadCard(device_id, device_ip, employees);
                                break;
                            case "FC":
                                page_object.saveFingerPrint(device_id, employees);
                                page_object.downloadCard(device_id, device_ip, employees);
                                break;
                            case "CP":
                                page_object.downloadCard(device_id, device_ip, employees);
                                break;
                            case "F":
                                page_object.saveFingerPrint(device_id, employees);
                                break;
                            case "C":
                                page_object.downloadCard(device_id, device_ip, employees);
                                break;
                            case "P":
                                page_object.downloadCard(device_id, device_ip, employees);
                                break;
                        }

                        return_object.status = "success";
                        return_object.return_data = "Downloaded Templates successfully!";
                    }
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;
                case "BioSecurity":
                    is_connected = false;
                    //zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
                    is_connected = axCZKEM1.Connect_Net(device_ip, 4370);
                    if (is_connected == true)
                    {
                        page_object.downloadBioSecurity(device_id, employees);

                        return_object.status = "success";
                        return_object.return_data = "Downloaded Templates successfully!";
                    }
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;
            }

        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "DOWNLOAD_TEMPLATES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while getting Enrollment data for the Device. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }

    private ReturnObject deleteZKTemplate(string device, string employees)
    {

        List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);
        ReturnObject return_object = new ReturnObject();
        string communication_type = string.Empty;
        string device_ip = string.Empty;
        string device_type = string.Empty;
        string deletion_errors = string.Empty;
        int device_id = 0;
        int enroll_id = 0;
        int i = 0;

        try
        {
            JObject device_data = JObject.Parse(device);
            communication_type = device_data["communation"].ToString();
            device_ip = device_data["deviceip"].ToString();
            device_id = Convert.ToInt32(device_data["deviceid"]);

            if (communication_type == "DNS")
            {
                string temp_device_ip = device_ip;
                IPAddress[] addresslist = Dns.GetHostAddresses(temp_device_ip);
                device_ip = Convert.ToString(addresslist[0]);
            }

            if (axCZKEM1.Connect_Net(device_ip, Convert.ToInt32(4370)) != false)
            {

                for (i = 0; i < employees_list.Count; i++)
                {
                    enroll_id = Convert.ToInt32(employees_list[i].ToString());
                    if (!(axCZKEM1.DeleteEnrollData(device_id, enroll_id, device_id, 12)))
                        deletion_errors += " " + enroll_id.ToString() + " ";
                }
            }
        }
        catch (Exception ex)
        {
            // Add logic for logging.
        }

        if (deletion_errors != "")
        {
            return_object.status = "error";
            return_object.return_data = deletion_errors;
        }
        else
        {
            return_object.status = "success";
            return_object.return_data = "success";
        }
        axCZKEM1.Disconnect();

        return return_object;
    }

    [WebMethod]
    public static ReturnObject deleteEnrollment(string device, string employees)
    {

        lan_download page_object = new lan_download();
        List<string> employees_list = JsonConvert.DeserializeObject<List<string>>(employees);
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        ReturnObject zk_status = new ReturnObject();
        string communication_type = string.Empty;
        string device_ip = string.Empty;
        string device_type = string.Empty;
        string device_model = string.Empty;
        bool download_status = false;
        bool is_connected = false;
        int enroll_id = 0;
        int device_id = 0;
        int status = 0;
        int i = 0;

        try
        {

            JObject device_data = JObject.Parse(device);
            communication_type = device_data["communation"].ToString();
            device_model = device_data["devicemodel"].ToString();
            device_type = device_data["devicetype"].ToString();
            device_ip = device_data["deviceip"].ToString();
            device_id = Convert.ToInt32(device_data["deviceid"]);

            if (device_model == "T5") device_id = 0;

            switch (device_type)
            {

                case "Anviz":
                    status = page_object.connectanvizdevice(device_id, device_ip, communication_type);
                    if (status != 0)
                    {

                        for (i = 0; i < employees_list.Count; i++)
                        {

                            enroll_id = Convert.ToInt32(employees_list[i].ToString());
                            AnvizNew.CKT_DeletePersonInfo(device_id, enroll_id, 0xFF);
                            download_status = true;
                        }
                    }
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;
                case "BioSecurity":
                    is_connected = axCZKEM1.Connect_Net(device_ip, Convert.ToInt32(4370));
                    if (is_connected == true)
                    {

                        for (i = 0; i < employees_list.Count; i++)
                        {
                            enroll_id = Convert.ToInt32(employees_list[i].ToString());
                            download_status = axCZKEM1.SSR_DeleteEnrollData(device_id, enroll_id.ToString(), 12);
                        }

                        axCZKEM1.RefreshData(device_id);
                        axCZKEM1.Disconnect();
                    }
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;

                case "Zk":
                    zk_status = page_object.deleteZKTemplate(device, employees);

                    if (zk_status.status == "success")
                        download_status = true;
                    else
                    {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;
            }

            if (download_status)
            {
                return_object.status = "success";
                return_object.return_data = "Templates deleted successfully!";
            }
        }
        catch (Exception ex)
        {

            Logger.LogException(ex, page, "DELETE_ENROLLMENT");

            return_object.status = "error";
            return_object.return_data = "An error occurred while deleting Enrollments. Please try again. If the error persists, please contact Support.";
        }
        finally
        {
            page_object.Dispose();
        }

        return return_object;
    }
}