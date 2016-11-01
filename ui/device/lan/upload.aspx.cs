using System;
using System.Collections;
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
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.Net;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurAX.Logger;

public partial class lan_upload : System.Web.UI.Page
{

    public static int mpiRet = 0;

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

    const string page = "LAN_TEMPLATE_UPLOAD";

    protected void Page_Load(object sender, EventArgs e) {

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string message = string.Empty;
        
        try {

            if (Session["username"] == null) {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex) {

            message = "An error occurred while loading Template Upload. Please try again. If the error persists, please contact Support.";
            
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("SAXAlert.show({'type': error','message': '");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");

            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }

    [WebMethod]
    public static ReturnObject getDeviceData(int page_number) {

        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        DataTable device_data = new DataTable();
        string query = string.Empty;

        try {

            query = "select deviceid, deviceip, communation, devicename, devicemodel, devicetype, status from deviceinfo";
            device_data = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(device_data, Formatting.Indented);
        }
        catch (Exception ex) {

        	Logger.LogException(ex, page, "GET_DEVICE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Device Data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    private void prepareEnrollmentData() {

        DBConnection db_connection = new DBConnection();
        string pat                 = ConfigurationManager.AppSettings["FingerPrintLocation"];
        string enrollment_file     = string.Empty;
        string path                = string.Empty;
        string finger1             = string.Empty;
        string finger2             = string.Empty;
        string file_extension      = string.Empty;
        string query               = string.Empty;
        int count_finger_print     = 0;
        int enroll_id              = 0;
        int index                  = 0;
    
        DirectoryInfo di       = new DirectoryInfo(pat);
           
        foreach (FileInfo fileInfo in di.GetFiles())
        {
            count_finger_print = 0;
            finger1 = "";
            finger2 = "";
            file_extension = Path.GetExtension(fileInfo.Name).ToString();

            if (file_extension == ".anv") {

                enrollment_file = Path.GetFileNameWithoutExtension(fileInfo.Name).ToString();
                
                if (enrollment_file.IndexOf('_') < 0) {
                    enroll_id = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileInfo.Name));
                }
                else {
                    enroll_id = Convert.ToInt32(enrollment_file.Substring(0, enrollment_file.IndexOf('_')));
                }

                path = pat + enroll_id + ".anv";
                string path1 = pat + enroll_id + "_" + 1 + ".anv";
                string path2 = pat + enroll_id + "_" + 2 + ".anv";
                string path3 = pat + enroll_id + "_" + 3 + ".anv";
                string path4 = pat + enroll_id + "_" + 4 + ".anv";
                string path5 = pat + enroll_id + "_" + 5 + ".anv";
                string path6 = pat + enroll_id + "_" + 6 + ".anv";
                string path7 = pat + enroll_id + "_" + 7 + ".anv";
                string path8 = pat + enroll_id + "_" + 8 + ".anv";
                string path9 = pat + enroll_id + "_" + 9 + ".anv";

                var file = Directory.GetFiles(pat, enroll_id + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file1 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 1 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file2 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 2 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file3 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 3 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file4 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 4 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file5 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 5 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file6 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 6 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file7 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 7 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file8 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 8 + ".anv", SearchOption.TopDirectoryOnly).Length;
                var file9 = Directory.GetFiles(pat, enroll_id.ToString() + "_" + 9 + ".anv", SearchOption.TopDirectoryOnly).Length;

                if (file == 1) count_finger_print = count_finger_print + 1;
                if (file1 == 1) count_finger_print = count_finger_print + 1;
                if (file2 == 1) count_finger_print = count_finger_print + 1;
                if (file3 == 1) count_finger_print = count_finger_print + 1;
                if (file4 == 1) count_finger_print = count_finger_print + 1;
                if (file5 == 1) count_finger_print = count_finger_print + 1;
                if (file6 == 1) count_finger_print = count_finger_print + 1;
                if (file7 == 1) count_finger_print = count_finger_print + 1;
                if (file8 == 1) count_finger_print = count_finger_print + 1;
                if (file9 == 1) count_finger_print = count_finger_print + 1;

                if (count_finger_print == 1) {
                    finger1 = "Yes";
                    finger2 = "";
                }
                else if (count_finger_print >= 2) {
                    finger1 = "Yes";
                    finger2 = "Yes";
                }

                if (db_connection.RecordExist("select count(*) from Enrollmaster where enrollid='" + Convert.ToString(enroll_id) + "'")) {
                    query = "update Enrollmaster set fingerp1='" + finger1 + "',fingerp2='" + finger2 + "' where enrollid='" + Convert.ToString(enroll_id) + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
                else {
                    query = "Insert into Enrollmaster(enrollid,fingerp1,fingerp2) values('" + Convert.ToString(enroll_id) + "','" + finger1 + "','" + finger2 + "')";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);
                }
            }
        }
    }

    private DataTable getAnvizEnrollmentData() {

        DBConnection db_connection = new DBConnection();
        DataTable return_data      = new DataTable();
        DataTable enrollment_data  = new DataTable();
        string query               = string.Empty;
        string enroll_id           = string.Empty;
        string card_id             = string.Empty;
        string finger_print1       = string.Empty;
        string finger_print2       = string.Empty;
        string employee_id         = string.Empty;
        string employee_name       = string.Empty;
        string pin                 = string.Empty;
        object db_enroll_id, db_finger_print1, db_finger_print2, db_card_id, db_pin, db_employee_id, db_employee_name;

        try {

            return_data.Columns.Add("Enrollid");
            return_data.Columns.Add("Cardid");
            return_data.Columns.Add("pin");
            return_data.Columns.Add("Empid");
            return_data.Columns.Add("Name");
            return_data.Columns.Add("fingerp1");
            return_data.Columns.Add("fingerp2");

            query = "select * from Enrollmaster order by Cast(Enrollid as int)";
            enrollment_data = db_connection.ReturnDataTable(query);

            foreach (DataRow row in enrollment_data.Rows) {

                db_enroll_id     = row["Enrollid"];
                db_finger_print1 = row["fingerp1"];
                db_finger_print2 = row["fingerp2"];
                db_card_id       = row["Cardid"];
                db_pin           = row["pin"];
                db_employee_id   = row["Empid"];
                db_employee_name = row["Name"];

                if ( db_enroll_id != DBNull.Value) 
                    enroll_id = db_enroll_id.ToString();
                
                if ( db_finger_print1 != DBNull.Value) 
                    finger_print1 = db_finger_print1.ToString();
                
                if ( db_finger_print2 != DBNull.Value) 
                    finger_print2 = db_finger_print2.ToString();
                
                if ( db_card_id != DBNull.Value) 
                    card_id = db_card_id.ToString();

                if ( db_pin != DBNull.Value) 
                    pin = db_pin.ToString();
                
                if ( db_employee_id != DBNull.Value) 
                    employee_id = db_employee_id.ToString();

                if ( db_employee_name != DBNull.Value) 
                    employee_name = db_employee_name.ToString();

                if (db_enroll_id != DBNull.Value) 
                    return_data.Rows.Add(enroll_id, card_id, pin, employee_id, employee_name, finger_print1, finger_print2);

                finger_print1 = "";
                finger_print2 = "";
                employee_name = "";
                employee_id = "";
                card_id = "";
                pin = "";
                enroll_id = "";
            }
        }
        catch (Exception ex) {
            // TODO: Add logic for Logging.
        }

        return return_data;
    }

    private DataTable getBioSecurityEnrollmentData() {

        DBConnection db_connection = new DBConnection();
        DataTable enrollment_data  = new DataTable();
        DataTable return_data      = new DataTable();
        string finger_print1       = string.Empty;
        string finger_print2       = string.Empty;
        string employee_name       = string.Empty;
        string employee_id         = string.Empty;
        string password            = string.Empty;
        string face                = string.Empty;
        string query               = string.Empty;
        string enroll_id           = string.Empty;
        int card_number            = 0;

        try {

            return_data.Columns.Add("Enrollid");
            return_data.Columns.Add("Cardid");
            return_data.Columns.Add("pin");
            return_data.Columns.Add("Empid");
            return_data.Columns.Add("Name");
            return_data.Columns.Add("fingerp1");
            return_data.Columns.Add("fingerp2");
            return_data.Columns.Add("face");

            query = "select dt.EnrollId, dt.FaceTempData, dt.FingerTempData1, dt.FingerTempData2, dt.CardNo, dt.Password, em.Emp_Code, em.Emp_Name from DownloadTemp as DT left join EmployeeMaster as em on em.Emp_Card_No = dt.EnrollId";
            enrollment_data = db_connection.ReturnDataTable(query);

            foreach (DataRow row in enrollment_data.Rows) {

                enroll_id = row["Enrollid"].ToString();

                if (row["FaceTempData"].ToString() != "") 
                    face = "Yes";
                else
                    face = "";

                if (row["Password"].ToString() != "")
                    password = "Yes";
                else
                    password = "";

                if (row["FingerTempData1"].ToString() != "")
                    finger_print1 = "Yes";
                else
                    finger_print1 = "";

                if (row["FingerTempData2"].ToString() != "")
                    finger_print2 = "Yes";
                else
                    finger_print2 = "";

                int CardNo_Int = 0;
                if (Int32.TryParse(row["CardNo"].ToString(), out CardNo_Int) && (CardNo_Int != 0))
                    card_number = CardNo_Int;
                else
                    card_number = 0;

                employee_name = row["Emp_Name"].ToString();
                employee_id = row["Emp_Code"].ToString();
                return_data.Rows.Add(enroll_id, finger_print1, finger_print2, face, card_number, password, employee_id, employee_name);
            }
        }
        catch (Exception ex) {
            //TODO: Add logic for Logging.
        }

        return return_data;
    }

    [WebMethod]
    public static ReturnObject getEnrollmentData() {

        lan_upload page_object = new lan_upload();
        DBConnection db_connection                  = new DBConnection();
        DataTable device_data                       = new DataTable();
        DataTable enrollment_data                   = new DataTable();
        ReturnObject return_object                  = new ReturnObject();
        string device_type                          = string.Empty;
        string query                                = string.Empty;

        try {

            query = "select devicetype from devicetype";
            device_data = db_connection.ReturnDataTable(query);

            device_type = device_data.Rows[0]["devicetype"].ToString();

            if (device_type == "Anviz") {
                page_object.prepareEnrollmentData();
                enrollment_data = page_object.getAnvizEnrollmentData();
            }
            else if (device_type == "BioSecurity") {
                enrollment_data = page_object.getBioSecurityEnrollmentData();
            }

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(enrollment_data, Formatting.Indented);
        }
        catch (Exception ex) {

        	Logger.LogException(ex, page, "GET_ENROLLMENT_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while loading Enrollment Data. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }

    public int connectanvizdevice(int device_id, string device_ip, string communication_type) {

        int status = 0;

        switch (communication_type) {

        case "LAN":
            if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) {}
            status = AnvizNew.CKT_RegisterNet(device_id, device_ip);
            break;

        case "WAN":
            int pLongRun = new int();
            if (AnvizNew.CKT_ChangeConnectionMode(1) != 1) {}
            status = AnvizNew.CKT_NetDaemonWithPort(5010);
            if (status == 1) {
                Thread.Sleep(5000);
                status = AnvizNew.CKT_GetClockingRecordEx(device_id, ref pLongRun);
            }
            break;

        case "USB":
            if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) {}
            status = AnvizNew.CKT_RegisterUSB(device_id, 0);
            break;

        case "DNS":
            if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) {}
            IPAddress[] addresslist = Dns.GetHostAddresses(device_ip);
            device_ip = Convert.ToString(addresslist[0]);
            status = AnvizNew.CKT_RegisterNet(device_id, device_ip);
            break;
        }

        return status;
    }

    private int ModifyPersonInfoLAN(int enroll_id, int device_id, string device_ip, long card_string, string password, string employee_name) {

        AnvizNew.PERSONINFO person = new AnvizNew.PERSONINFO();
        DBConnection db_connection = new DBConnection();
        byte[] nms = Encoding.ASCII.GetBytes(password);
        byte[] pss = Encoding.Default.GetBytes(employee_name);
        int status = 1, card_number, final_status = 0, i = 0;
        long card_long = 0;
        bool is_admin = false;
        string query = string.Empty;

        status = AnvizNew.CKT_RegisterNet(device_id, device_ip);
        if (status == 1) {

            if (card_string > 0)
                 card_long = Convert.ToInt64(card_string);
            else
                card_long = 0;
            if (card_long > 2147483647) {
                card_long = card_long - 4294967296;
                card_number = Convert.ToInt32(card_long);
            }
            else {
                card_number = Convert.ToInt32(card_long);
            }

            person.CardNo = card_number;
            person.Name = new byte[12];

            for (i = 0; i < 12; i++)
            {
                if (i < pss.Length)
                {
                    person.Name[i] = pss[i];
                    continue;
                }
                person.Name[i] = 0;
            }

            person.Password = new byte[8];
            for (i = 0; i < 8; i++)
            {
                if (i < nms.Length)
                {
                    person.Password[i] = nms[i];
                    continue;
                }
                person.Password[i] = 0;
            }
            person.PersonID = enroll_id;
            person.KQOption = 6;
            person.Group = 1;

            query = "select count(*) from Admin where EnrollId = " + enroll_id + " and Deviceid = " + device_id + " ";
            if (db_connection.RecordExist(query))
                is_admin = true;

            if (is_admin == false)
                final_status = AnvizNew.CKT_ModifyPersonInfo(device_id, ref person);

            // NOTE: Below section has been commented on purpose. Please uncomment after adding the code for LOGGING errors.
            /*switch (final_status) {

            case 1: 
            case -1:
            default:
                //TODO: Add logic for LOGGING here.
                break;
            }*/
        }

        return final_status;
    }

    private int ModifyPersonInfoDNS(int enroll_id, int device_id, string device_ip, long card_string, string password, string employee_name) {

        int final_status = 0;
        string temp_device_ip = device_ip;

        if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) {}
    
        System.Net.IPAddress[] addresslist = System.Net.Dns.GetHostAddresses(temp_device_ip);
        device_ip = Convert.ToString(addresslist[0]);
        final_status = ModifyPersonInfoLAN(enroll_id, device_id, device_ip, card_string, password, employee_name);

        return final_status;
    }

    private int ModifyPersonInfoUSB(int enroll_id, int device_id) {

        AnvizNew.PERSONINFO person = new AnvizNew.PERSONINFO();
        byte[] nms = Encoding.ASCII.GetBytes("");
        byte[] pss = Encoding.Default.GetBytes("");
        int final_status = 0, status = 0, i = 0;
        int pLongRun = new int();

        if (AnvizNew.CKT_ChangeConnectionMode(0) != 1) {}

        status = AnvizNew.CKT_RegisterUSB(device_id, 0);
        if (status != 0) {

            person.CardNo = 5216455;
            person.Name = new byte[12];

            for (i = 0; i < 12; i++) {
                if (i < pss.Length) {
                    person.Name[i] = pss[i];
                    continue;
                }
                person.Name[i] = 0;
            }

            person.Password = new byte[8];

            for (i = 0; i < 8; i++) {
                if (i < nms.Length) {
                    person.Password[i] = nms[i];
                    continue;
                }
                person.Password[i] = 0;
            }
            person.PersonID = enroll_id;
            person.KQOption = 6;
            person.Group = 1;

            final_status = AnvizNew.CKT_ModifyPersonInfo(device_id, ref person);

            // NOTE: Below section has been commented on purpose. Please uncomment after adding the code for LOGGING errors.
            /*switch (final_status) {

            case 1: 
            case -1:
            default:
                //TODO: Add logic for LOGGING here.
                break;
            }*/
        }
        
        return final_status;
    }

    private void uploadFingerPrint(int device_id, string device_ip, string communication_type, string employees) {

        JArray employees_list = new JArray();
        string[] path         = new string[10];
        int[] file            = new int[10];
        int[] device_status   = new int[10];
        long card_string      = 0;

        string employee_name = string.Empty, password = string.Empty;
        int enroll_id = 0, status = 0, i, j;

        try {

            employees_list = JArray.Parse(employees);

            for (i = 0; i < employees_list.Count; i++) {

                password = string.Empty;
                card_string = 0;

                enroll_id = Convert.ToInt32(employees_list[i]["Enrollid"]);

                if ( !string.IsNullOrEmpty( employees_list[i]["Cardid"].ToString() ) )
                    card_string = Convert.ToInt64(employees_list[i]["Cardid"]);

                if ( !string.IsNullOrEmpty( employees_list[i]["pin"].ToString() ) )
                    password = employees_list[i]["pin"].ToString();

                if ( !string.IsNullOrEmpty( employees_list[i]["Name"].ToString() ) )
                    employee_name = employees_list[i]["Name"].ToString();

                switch (communication_type) {

                case "LAN":
                    status = ModifyPersonInfoLAN(enroll_id, device_id, device_ip, card_string, password, employee_name);
                    break;
                case "DNS":
                    status = ModifyPersonInfoDNS(enroll_id, device_id, device_ip, card_string, password, employee_name);
                    break;
                case "USB":
                    status = ModifyPersonInfoUSB(enroll_id, device_id);
                    break;
                }

                if (status == 1) {

                    string pat = ConfigurationManager.AppSettings["FingerPrintLocation"];
                    
                    if(!Directory.Exists(pat))
                        Directory.CreateDirectory(pat);

                    path[0] = pat + enroll_id + ".anv";
                    for (j = 1; j < path.Length; j++) {
                        path[j] = pat + enroll_id + "_" + j + ".anv";
                    }

                    file[0] = Convert.ToInt32( Directory.GetFiles(pat, enroll_id + ".anv", SearchOption.TopDirectoryOnly).Length );
                    for (j = 1; j < file.Length; j++) {
                        file[j] = Convert.ToInt32( Directory.GetFiles(pat, enroll_id.ToString() + "_" + j + ".anv", SearchOption.TopDirectoryOnly).Length );
                    }

                    for (j = 0; j < file.Length; j++) {
                        if (file[j] == 1) {
                            device_status[j] = AnvizNew.CKT_PutFPTemplateLoadFile(device_id, enroll_id, j, path[j]);
                            // TODO: Add logic for LOGGING the return value of the above statement
                        }
                        else {
                            // TODO: Add logic for LOGGING.
                        }
                    }
                }
            }
        }
        catch (Exception ex) {
            // TODO: Add logic for LOGGING.
            throw;
        }
    }

    private void uploadCard_Face(int device_id, string device_ip, string employees) {

        DBConnection db_connection   = new DBConnection();
        DataTable enrollment_details = new DataTable();
        JArray employees_list        = new JArray();
        string query                 = string.Empty;
        bool is_enabled              = false;
        string 
            s_enroll_id, s_employee_name, s_temp_data, s_password, s_enabled, s_card_number, s_user_id,
            finger_print1, finger_print2;
        int 
            error_code = 0, face_index = 0, status = 0, length = 0, privilege = 0, i, enroll_id,
            face = 0, face1 = 0;

        for (i = 0; i < employees_list.Count; i++) {

            enroll_id = Convert.ToInt32(employees_list[i]["Enrollid"]);

            query = "select * from DownloadTemp where EnrollId = '" + enroll_id + "' ";
            enrollment_details = db_connection.ReturnDataTable(query);

            if (enrollment_details.Rows.Count > 0) {

                s_user_id = enroll_id.ToString();
                s_employee_name = enrollment_details.Rows[0]["Name"].ToString();
                s_password = enrollment_details.Rows[0]["Password"].ToString();
                privilege = Convert.ToInt32(enrollment_details.Rows[0]["Privilege"].ToString());
                face_index = Convert.ToInt32(enrollment_details.Rows[0]["FaceIndx"].ToString());
                s_temp_data = enrollment_details.Rows[0]["FaceTempData"].ToString();
                length = Convert.ToInt32(enrollment_details.Rows[0]["Length"].ToString());
                s_enabled = enrollment_details.Rows[0]["Enabled"].ToString();
                finger_print1 = enrollment_details.Rows[0]["FingerTempData1"].ToString();
                finger_print2 = enrollment_details.Rows[0]["FingerTempData2"].ToString();
                s_card_number = enrollment_details.Rows[0]["CardNo"].ToString();//Upload card Details

                if (Convert.ToInt32(s_card_number) != 0)
                    axCZKEM1.SetStrCardNumber(s_card_number);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

                if (s_enabled == "True")
                    is_enabled = true;
                else
                    is_enabled = false;

                if (axCZKEM1.SSR_SetUserInfo(device_id, s_user_id, s_employee_name, s_password, privilege, is_enabled)) { //face templates are part of users' information 
                    if (s_temp_data != "") {
                        axCZKEM1.SetUserFaceStr(device_id, s_user_id, face_index, s_temp_data, length);//upload face templates information to the device
                        status = 1;
                        face1 = 1;
                    }

                    if (finger_print1 != "") {
                        axCZKEM1.SetUserTmpExStr(device_id, s_user_id, 0, 1, finger_print1);
                        status = 1;
                        face = 2;
                    }

                    if (finger_print2 != "") {
                        axCZKEM1.SetUserTmpExStr(device_id, s_user_id, 1, 1, finger_print2);
                        status = 1;
                        face = 2;
                    }
                }
                else {
                    axCZKEM1.GetLastError(ref error_code);
                    // TODO: Add logic for LOGGING.
                    return;
                }
            }
        }
    }

    [WebMethod]
    public static ReturnObject uploadTemplates(string employees, string devices) {

        lan_upload page_object = new lan_upload();
        JArray device_list                          = new JArray();
        DBConnection db_connection                  = new DBConnection();
        ReturnObject return_object                  = new ReturnObject();
        string device_ip                            = string.Empty;
        string device_type                          = string.Empty;
        string communication_type                   = string.Empty;
        string query                                = string.Empty;
        bool is_connected                           = false;
        int device_id                               = 0;
        int status                                  = 0;
        int i                                       = 0;

        try {

            device_list = JArray.Parse(devices);

            for (i = 0; i < device_list.Count; i++) {

                device_id          = Convert.ToInt32(device_list[i]["deviceid"]);
                device_ip          = device_list[i]["deviceip"].ToString();
                device_type        = device_list[i]["devicetype"].ToString();
                communication_type = device_list[i]["communation"].ToString();

                switch (device_type) {

                case "Anviz":
                    status = page_object.connectanvizdevice(device_id, device_ip, communication_type);
                    if (status != 0) {

                        page_object.uploadFingerPrint(device_id, device_ip, communication_type, employees);

                        return_object.status = "success";
                        return_object.return_data = "Templates uploaded successfully!";
                    }
                    else {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;
                case "BioSecurity":
                    is_connected = axCZKEM1.Connect_Net(device_ip, Convert.ToInt32(4370));
                    if (is_connected == true) {

                        page_object.uploadCard_Face(device_id, device_ip, employees);
                        
                        return_object.status = "success";
                        return_object.return_data = "Templates uploaded successfully!";   
                    }
                    else {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    break;
                }

            }
        }
        catch (Exception ex) {

        	Logger.LogException(ex, page, "LAN_UPLOAD_TEMPLATES");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Uploading Templates to the Devices. Please try again. If the error persists, please contact Support.";
        }

        return return_object;
    }
}