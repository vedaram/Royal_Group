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
using System.Text;
using System.Net;
using System.Threading;
using System.Security.Cryptography;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Services;
using ClosedXML.Excel;
using SecurAX.Logger;

public partial class lan_information : System.Web.UI.Page
{

    public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
    const string page = "DEVICE_INFORMATION";

    protected void Page_Load(object sender, EventArgs e) {

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string message = string.Empty;
        
        try {
            if (Session["username"] == null) {
                Response.Redirect("~/logout.aspx", true);
            }
        }
        catch (Exception ex) {

            Logger.LogException(ex, page, "PAGE_LOAD");

            message = "An error occurred while loading Device Information page. Please try again. If the error persists, please contact Support.";
            
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
    public static ReturnObject GetDeviceData(int page_number) {

        ReturnObject return_object  = new ReturnObject();
        DBConnection db_connection  = new DBConnection();
        DataTable device_data_table = new DataTable();
        int start_row               = (page_number - 1) * 30;
        int number_of_record        = (page_number * 30) + 1;
        string query                = string.Empty;

        try {
            
            query = "select deviceid as device_id, deviceip as device_ip, communation as communication_type, devicename as device_name, devicetype as device_type, devicemodel as device_model, category, status from (select deviceid, deviceip, communation, devicename, devicetype, devicemodel, category, status, ROW_NUMBER() OVER (ORDER BY deviceid) as row from deviceinfo) a where row > " + start_row + " and row < " + number_of_record;
            device_data_table = db_connection.ReturnDataTable(query);

            return_object.status = "success";
            return_object.return_data = JsonConvert.SerializeObject(device_data_table, Formatting.Indented);
        }
        catch (Exception ex) {

            Logger.LogException(ex, page, "GET_DEVICE_DATA");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    public void UpdateDatabase(int device_id, string device_name, string device_ip, string device_type, string communication_type, string device_model, string device_category, string mode) {
        
        Hashtable device_data      = new Hashtable();
        DBConnection db_connection = new DBConnection();

        device_data.Add("mode", mode);
        device_data.Add("DeviceId", device_id);
        device_data.Add("Communication", communication_type);
        device_data.Add("DeviceIP", device_ip);
        device_data.Add("DeviceName", device_name);
        device_data.Add("Category", device_category);
        device_data.Add("devicetype", device_type);
        device_data.Add("devicemodel", device_model);
        // Commenting the below 2 lines of code. This will come in to use when Device Location and SNO are also passed from the frontend.
        // Please change the arguments for function before uncommenting these 2 lines.
        /* 
            device_data.Add("Location", device_location);
            device_data.Add("deviceaddr", device_sno);
        */
        db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateDevice", device_data);
    }

    protected string GetDeviceTypeFromDeviceModel(string selected_device_model) {

        string query               = string.Empty;
        string device_type         = string.Empty;
        DBConnection db_connection = new DBConnection();

        query = "select devicetype from devicemodel where devicemodel = '" + selected_device_model + "' ";
        device_type = db_connection.ExecuteQuery_WithReturnValueString(query);

        return device_type;
    }

    protected string GetDeviceType() {

        DBConnection db_connection = new DBConnection();
        string query               = string.Empty;
        string device_type         = string.Empty;

        query = "select devicetype from devicetype";
        device_type = db_connection.ExecuteQuery_WithReturnValueString(query);

        return device_type;
    }

    protected string GetDeviceCategory(int is_finger, int is_card, int is_pin)
    {
        string device_category = "";
        
        if (is_finger == 1) device_category = "F";

        if (is_card == 1) device_category += "C";

        if (is_pin == 1) device_category += "P";

        return device_category;
    }

    [WebMethod]
    public static ReturnObject AddDeviceInformation(string current) {

        lan_information page_object = new lan_information();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string 
            device_name = string.Empty, device_ip = string.Empty, communication_type = string.Empty,
            device_model = string.Empty, device_category = string.Empty, device_type = string.Empty,
            query = string.Empty;
        int 
            device_id = 0, count = 0, is_finger = 0, is_card = 0, is_pin = 0;

        try {

            JObject current_data = JObject.Parse(current);
            device_name = current_data["device_name"].ToString();
            device_ip = current_data["device_ip"].ToString();
            communication_type = current_data["communication_type"].ToString();
            device_type = current_data["device_type"].ToString();
            device_model = current_data["device_model"].ToString();
            device_id = Convert.ToInt32(current_data["device_id"]);
            is_finger = Convert.ToInt32(current_data["finger_print"]);
            is_card = Convert.ToInt32(current_data["card_number"]);
            is_pin = Convert.ToInt32(current_data["pin_number"]);

            device_category = page_object.GetDeviceCategory(is_finger, is_card, is_pin);

            query = "select count(*) from deviceinfo where deviceid = '" + device_id + "' ";
            count = db_connection.GetRecordCount(query);
            if (count > 0) {
                return_object.status      = "error";
                return_object.return_data = "Device ID has been taken. Please try again with a different Device ID.";

                return return_object;
            }

            query = "select count(*) from DeviceInfo where DeviceIP = '" + device_ip + "' and deviceip!='' ";
            count = db_connection.GetRecordCount(query);
            if (count > 0) {
                return_object.status      = "error";
                return_object.return_data = "Device IP has been taken. Please try again with a different IP.";

                return return_object;
            }

            page_object.UpdateDatabase(device_id, device_name, device_ip, device_type, communication_type, device_model, device_category, "I");
                    
            return_object.status      = "success";
            return_object.return_data = "New Device added successfully!";
        } 
        catch (Exception ex) {

            Logger.LogException(ex, page, "ADD_DEVICE_INFORMATION");

            return_object.status      = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";
            
            throw;
        }
        finally {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject EditDeviceInformation(string current, string previous) {

        lan_information page_object = new lan_information();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();

        string 
            original_device_ip = string.Empty, device_name = string.Empty, device_ip = string.Empty, 
            communication_type = string.Empty, device_model = string.Empty, device_category = string.Empty, 
            device_type = string.Empty, query = string.Empty;
        int 
            device_id = 0, count = 0, is_finger = 0, is_card = 0, is_pin = 0;

        try {

            JObject current_data = JObject.Parse(current);
            device_name          = current_data["device_name"].ToString();
            device_ip            = current_data["device_ip"].ToString();
            communication_type   = current_data["communication_type"].ToString();
            device_type          = current_data["device_type"].ToString();
            device_model         = current_data["device_model"].ToString();
            device_id            = Convert.ToInt32(current_data["device_id"]);
            is_finger            = Convert.ToInt32(current_data["finger_print"]);
            is_card              = Convert.ToInt32(current_data["card_number"]);
            is_pin               = Convert.ToInt32(current_data["pin_number"]);

            JObject previous_data = JObject.Parse(previous);
            original_device_ip    = previous_data["device_ip"].ToString();

            if (original_device_ip != device_ip) {
                query = "select count(*) from DeviceInfo where DeviceIP = '" + device_ip + "' ";
                count = db_connection.GetRecordCount(query);

                if (count > 0) {
                    return_object.status = "error";
                    return_object.return_data = "Deivce IP has been taken. Please try again with a different IP Address.";

                    return return_object;
                }
            }

            device_category = page_object.GetDeviceCategory(is_finger, is_card, is_pin);

            page_object.UpdateDatabase(device_id, device_name, device_ip, device_type, communication_type, device_model, device_category, "U");

            return_object.status = "success";
            return_object.return_data = "Device Information edited successfully!";
        }
        catch (Exception ex) {

            Logger.LogException(ex, page, "EDIT_DEVICE_INFORMATION");

            return_object.status = "error";
            return_object.return_data = "An error occurred while performing this operation. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject DeleteDeviceInformation(string current) {

        DBConnection db_connection = new DBConnection();
        Hashtable device_details   = new Hashtable();
        ReturnObject return_object = new ReturnObject();
        int device_id              = 0;

        try {

            JObject current_data = JObject.Parse(current);
            device_id = Convert.ToInt32(current_data["device_id"]);

            device_details.Add("mode", "D");
            device_details.Add("DeviceId", device_id);

            db_connection.ExecuteStoredProcedureWithHashtable_WithoutReturn("ManipulateDevice", device_details);

            return_object.status = "success";
            return_object.return_data = "Device Information deleted successfully!";
        }
        catch (Exception ex) {

            Logger.LogException(ex, page, "DELETE_DEVICE_INFORMATION");

            return_object.status = "error";
            return_object.return_data = "An error occurred while deleting Device Information details. Please try again. If the error persists, please contact Support.";

            throw;
        }

        return return_object;
    }

    private int ConnectAnvizDevice(int device_id, string device_ip, string communication_type) {

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

    [WebMethod]
    public static ReturnObject GetDeviceTime(string current) {

        lan_information page_object = new lan_information();
        ReturnObject return_object                     = new ReturnObject();
        string communication_type                      = string.Empty;
        string device_type                             = string.Empty;
        string device_ip                               = string.Empty;
        string device_time                             = string.Empty;
        bool is_connected                              = false;
        int idwErrorCode                               = 0;
        int idwYear                                    = 0;
        int idwMonth                                   = 0;
        int idwDay                                     = 0;
        int idwHour                                    = 0;
        int idwMinute                                  = 0;
        int idwSecond                                  = 0;
        int device_id                                  = 0;
        int status                                     = 0;

        try {

            JObject current_data = JObject.Parse(current);
            device_id            = Convert.ToInt32(current_data["device_id"]);
            device_ip            = current_data["device_ip"].ToString();
            device_type          = current_data["device_type"].ToString();
            communication_type   = current_data["communication_type"].ToString();

            switch (device_type) {

            case "Anviz":
                status = 0;
                status = page_object.ConnectAnvizDevice(device_id, device_ip, communication_type);

                if (status != 0) {

                    AnvizNew.DATETIMEINFO PTime = new AnvizNew.DATETIMEINFO();
                    if (AnvizNew.CKT_GetDeviceClock(device_id, ref PTime) == 1) {

                        device_time = PTime.Year.ToString() + "-" + (PTime.Month).ToString("00") + "-" + (PTime.Day).ToString("00") + " " + (PTime.Hour).ToString("00") + ":" + (PTime.Minute).ToString("00") + ":" + (PTime.Second).ToString("00");
                        
                        return_object.status = "success";
                        return_object.return_data = device_time;
                    }
                    else {
                        return_object.status = "error";
                        return_object.return_data = "Getting Device Time failed. Please try again.";
                    }
                }
                else {
                    return_object.status = "error";
                    return_object.return_data = "Unable to communicate with the Device. Please try again.";
                }
                break;

            case "BioSecurity":
                is_connected = page_object.axCZKEM1.Connect_Net(device_ip, Convert.ToInt32(4370));
                if (is_connected == true) {
                    //Cursor = Cursors.WaitCursor;
                    if (page_object.axCZKEM1.GetDeviceTime(device_id, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond)) {

                        device_time = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();

                        return_object.status = "success";
                        return_object.return_data = device_time;
                    }
                    else {
                        return_object.status = "error";
                        return_object.return_data = "Set Device Clock failed. Please try again.";
                    }
                }
                else {
                    return_object.status = "error";
                    return_object.return_data = "Unable to communicate with the Device. Please try again.";
                }
                page_object.axCZKEM1.Disconnect();
                //Cursor = Cursors.Default;
                break;
            }
        }
        catch (Exception ex) {

            Logger.LogException(ex, page, "GET_DEVICE_TIME");

            return_object.status = "error";
            return_object.return_data = "An error occurred when getting Device Time. Please try again. If the error persists, please contact Support";
        }
        finally {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject SetDeviceTime(string current) {

        lan_information page_object = new lan_information();
        ReturnObject return_object                     = new ReturnObject();
        string communication_type                      = string.Empty;
        string device_ip                               = string.Empty;
        string device_type                             = string.Empty;
        string device_time                             = string.Empty;
        bool is_connected                              = false;
        int idwErrorCode                               = 0;
        int idwYear                                    = 0;
        int idwMonth                                   = 0;
        int idwDay                                     = 0;
        int idwHour                                    = 0;
        int idwMinute                                  = 0;
        int idwSecond                                  = 0;
        int device_id                                  = 0;
        int status                                     = 0;

        try {

            JObject current_data = JObject.Parse(current);
            device_ip            = current_data["device_ip"].ToString();
            device_type          = current_data["device_type"].ToString();
            communication_type   = current_data["communication_type"].ToString();
            device_id            = Convert.ToInt32(current_data["device_id"]);

            switch (device_type) {

            case "Anviz":
                status = page_object.ConnectAnvizDevice(device_id, device_ip, communication_type);

                if (status != 0) {
                        //int ReaderNo = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text);
                        AnvizNew.DATETIMEINFO PTime = new AnvizNew.DATETIMEINFO();
                        PTime.Year = Convert.ToUInt16(DateTime.Now.Year);
                        PTime.Month = Convert.ToByte(DateTime.Now.Month);
                        PTime.Day = Convert.ToByte(DateTime.Now.Day);
                        PTime.Hour = Convert.ToByte(DateTime.Now.Hour);
                        PTime.Minute = Convert.ToByte(DateTime.Now.Minute);
                        PTime.Second = Convert.ToByte(DateTime.Now.Second);
                        if (AnvizNew.CKT_SetDeviceClock(device_id, ref PTime) == 1) {

                            device_time = PTime.Year.ToString() + "-" + (PTime.Month).ToString("00") + "-" + (PTime.Day).ToString("00") + " " + (PTime.Hour).ToString("00") + ":" + (PTime.Minute).ToString("00") + ":" + (PTime.Second).ToString("00");
                            
                            return_object.status = "success";
                            return_object.return_data = device_time;
                        }
                        else {
                            return_object.status = "error";
                            return_object.return_data = "Setting Device Clock failed. Please try again.";
                        }
                }
                else {
                    return_object.status = "error";
                    return_object.return_data = "Unable to communicate with the Device. Please try again.";
                }
                break;
            case "BioSecurity":
                    is_connected = page_object.axCZKEM1.Connect_Net(device_ip, Convert.ToInt32(4370));
                    if (is_connected == true) {

                        idwErrorCode = 0;
                        idwYear      = Convert.ToInt32(DateTime.Now.Year);
                        idwMonth     = Convert.ToInt32(DateTime.Now.Month);
                        idwDay       = Convert.ToInt32(DateTime.Now.Day);
                        idwHour      = Convert.ToInt32(DateTime.Now.Hour);
                        idwMinute    = Convert.ToInt32(DateTime.Now.Minute);
                        idwSecond    = Convert.ToInt32(DateTime.Now.Second);

                        //Cursor = Cursors.WaitCursor;
                        if (page_object.axCZKEM1.SetDeviceTime2(device_id, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond)) {

                            page_object.axCZKEM1.RefreshData(device_id);//the data in the device should be refreshed
                            
                            return_object.status = "success";
                            return_object.return_data = device_time;
                        }
                        else {
                            return_object.status = "error";
                            return_object.return_data = "Setting Device Clock failed. Please try again.";
                        }
                    }
                    else {
                        return_object.status = "error";
                        return_object.return_data = "Unable to communicate with the Device. Please try again.";
                    }
                    page_object.axCZKEM1.Disconnect();
                break;
            }
        }
        catch (Exception ex) {

            Logger.LogException(ex, page, "SET_DEVICE_TIME");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Setting Device Clock. Please try again. If the error persists, please contact Support";

            throw;
        }
        finally {
            page_object.Dispose();
        }

        return return_object;
    }

    [WebMethod]
    public static ReturnObject TestDeviceConnection(string current) {

        lan_information page_object = new lan_information();
        DBConnection db_connection = new DBConnection();
        ReturnObject return_object = new ReturnObject();
        string communication_type = string.Empty;
        string device_type = string.Empty;
        string device_ip = string.Empty;
        string query = string.Empty;
        int status = 0;
        int device_id = 0;

        try {

            JObject current_data = JObject.Parse(current);
            device_id            = Convert.ToInt32(current_data["device_id"]);
            device_type          = current_data["device_type"].ToString();
            device_ip            = current_data["device_ip"].ToString();
            communication_type   = current_data["communication_type"].ToString();

            switch (device_type)
            {

            case "Anviz":
                status = page_object.ConnectAnvizDevice(device_id, device_ip, communication_type);
                if (status != 0) {

                    query = "update deviceinfo set Status='connected' where deviceip='" + device_ip + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    return_object.status = "success";
                    return_object.return_data = "connected";
                }
                else {

                    query = "update deviceinfo set Status='disconnected' where deviceip='" + device_ip + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    return_object.status = "success";
                    return_object.return_data = "disconnected";
                }
                break;

            case "BioSecurity":
                if (page_object.axCZKEM1.Connect_Net(device_ip, 4370)) {

                    query = "update deviceinfo set Status='connected' where deviceip='" + device_ip + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    return_object.status = "success";
                    return_object.return_data = "connected";
                }
                else
                {
                    query = "update deviceinfo set Status='disconnected' where deviceip='" + device_ip + "'";
                    db_connection.ExecuteQuery_WithOutReturnValue(query);

                    return_object.status = "success";
                    return_object.return_data = "disconnected";
                }
                break;
            }
        } 
        catch (Exception ex) {

            Logger.LogException(ex, page, "TEST_DEVICE_CONNECTION");

            return_object.status = "error";
            return_object.return_data = "An error occurred while Testing Connection with the Device. Please try again. If the error persists, please contact Support.";

            throw;
        }
        finally {
            page_object.Dispose();
        }

        return return_object;
    }
}