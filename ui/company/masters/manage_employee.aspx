<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="manage_employee.aspx.cs" Inherits="masters_manage_employee" Title="Employee Master | SecurTime" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">MANAGE EMPLOYEE</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a href="employee.aspx" class="btn btn-blue">
                    <span class="fa fa-long-arrow-left"></span>Back
                </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" runat="server">

<div class="container">
            <input type="hidden" id="tabname" name="Language" value="emp"/>
         <div class="card no-padding">
			<div class="card-header">
				<ul class="tablist clearfix">
		            <li class="tab" id="empDetailsTabOption"> 
                        <!-- javascript:void(0) -->
		                <a href="#empDetailsTab;" data-control="toggle" data-toggle="tab" data-target="#empDetailsTab" data-id="empdetail">
		                    Employee Details
		                </a>
		            </li>
		            <li class="tab" id="empDataTransactionTabOption">
		                <a href="#edtTab" data-control="toggle" data-toggle="tab" data-target="#edtTab" data-id="edt">
		                    Employee Data Transactions
		                </a>
		            </li>
		        </ul>
	        </div>
                

   <div class="card-body">
	        	<div class="tab-content">
		            <div class="tab-panel" id="empDetailsTab">
                        <form class="form" method="" id="saveForm">
                              <!--  <div class="card">
                                    <div class="card-header">
                                        <h4>MANAGE EMPLOYEE DETAILS</h4>
                                    </div> -->
                    
                    <!-- /.card-header -->
                    <div class="card-body">
                        <div class="container">
                            <div class="col-offset-2 col-10">
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Employee Code <span style="color: red">*</span></label>
                                        <%--<label class="control-label">Test</label>--%>
                                        <input type="text" class="form-control" id="employee_code" name="employee_code" maxlength="14">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Date of Joining <span style="color: red">*</span></label>
                                        <input type="text" class="form-control datepicker" id="date_of_joining" name="date_of_joining">
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Employee Name <span style="color: red">*</span></label>
                                        <input type="text" class="form-control" id="employee_name" name="employee_name">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Company <span style="color: red">*</span></label>
                                        <select class="form-control" id="company_code" name="company_code">
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Date of Birth</label>
                                        <input type="text" class="form-control datepicker" id="date_of_birth" name="date_of_birth">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Branch</label>
                                        <select class="form-control" id="branch_code" name="branch_code">
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Gender</label>
                                        <select class="form-control" id="gender" name="gender">
                                            <option value="male">Male</option>
                                            <option value="female">Female</option>
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Department</label>
                                        <select class="form-control" id="department_code" name="department_code">
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Category</label>
                                        <select class="form-control" id="user_type" name="user_type">
                                            <option value="2">Employee</option>
                                            <option value="0">Admin</option>
                                            <option value="1">Manager</option>
                                            <option value="3">HR</option>
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Manager ID and Name</label>
                                        <select class="form-control" id="manager_id" name="manager_id">
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Address</label>
                                        <textarea class="form-control" id="address" name="address"></textarea>
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <div class="form-group" style="padding-top: 10px;">
                                            <label class="text-blue"><small><strong>Designation</strong></small></label>
                                            <select class="form-control" id="designation_code" name="designation_code">
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label class="text-blue"><small><strong>Shift</strong></small></label>
                                            <select class="form-control" id="shift_code" name="shift_code">
                                            </select>
                                        </div>
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Phone Number</label>
                                        <input type="text" class="form-control" id="phone_number" name="phone_number">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Employee Category <span style="color: red">*</span></label>
                                        <select class="form-control" id="employee_category_code" name="employee_category_code">
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Email ID</label>
                                        <input type="text" class="form-control" id="email_address" name="email_address">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Enrollment ID <span style="color: red">*</span></label>
                                        <input type="text" class="form-control" id="enroll_id" name="enroll_id">
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Passport Number</label>
                                        <input type="text" class="form-control" id="passport_number" name="passport_number">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Passport Expiry Date</label>
                                        <input type="text" class="form-control datepicker" id="passport_expiry" name="passport_expiry">
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Emirates Number</label>
                                        <input type="text" class="form-control date-picker" id="emirates_number" name="emirates_number">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Nationality</label>
                                        <input type="text" class="form-control" id="nationality" name="nationality">
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Emergency Contact Number</label>
                                        <input type="text" class="form-control" id="emergency_contact_number" name="emergency_contact_number">
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Visa Expiry Date</label>
                                        <input type="text" class="form-control datepicker" id="visa_expiry" name="visa_expiry">
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Employee Status</label>
                                        <select class="form-control" id="employee_status" name="employee_status">
                                            <option value="1">Active</option>
                                            <option value="2">Terminated</option>
                                            <option value="3">Resigned</option>
                                            <option value="4">Absconding</option>
                                        </select>
                                    </div>
                                    <!-- /.col-4 -->
                                    <div class="col-4">
                                        <label class="control-label">Resigned/Terminated Date</label>
                                        <input type="text" class="form-control datepicker" id="date_of_leaving" name="date_of_leaving">
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <!-- /.form-group -->


                                <div class="form-group">
                                    <div class="col-4">
                                        <input type="checkbox" id="auto_checked" name="auto_checked">
                                        <span class="form-control1">Is Auto Shift Eligible?</span>
                                    </div>
                                    <div class="col-4">
                                        <input type="checkbox" id="overtime_checked" disabled name="overtime_checked">
                                        <span class="form-control1">Overtime Eligibility</span>
                                    </div>
                                    <!-- /.col-4 -->
                                </div>
                                <div class="form-group">
                                    <div class="col-4">
                                        <input type="checkbox" id="ramadan_checked" disabled name="ramadan_checked">
                                        <span class="form-control1">Ramadan Timing Eligibility</span>
                                    </div>
                                    <div class="col-4">
                                        <input type="checkbox" id="punch_exception_checked" disabled name="punch_exception_checked">
                                        <span class="form-control1">Punch Exception</span>
                                    </div>
                                    <!-- /.col-4 -->

                                </div>
                                <!-- /.form-group -->
                                <!-- /.col-4 -->

                                <div class="form-group" id="maternity" style="display: none">
                                    <div class="col-4" style="display: none">
                                        <input type="checkbox" id="maternity_break_hours_checked" name="maternity_break_hours_checked">
                                        <span class="form-control1" id="maternity_break_hours_checked_span">Maternity Break Hours</span>
                                    </div>
                                    <div class="col-4" style="display: none">
                                        <label class="control-label">Child Date of Birth</label>
                                        <input type="text" class="form-control datepicker" id="child_date_of_birth" name="child_date_of_birth">
                                    </div>


                                    <div class="col-4" style="display: none">
                                        <input type="checkbox" id="maternity_eligibility_checked" name="maternity_eligibility_checked">
                                        <span class="form-control1">Maternity Eligibility</span>
                                    </div>

                                </div>
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Employee Religion</label>
                                        <select class="form-control" id="employee_religion" name="employee_religion">
                                            <option value="">Select</option>
                                            <option value="Muslim">Muslim</option>
                                            <option value="Agnostic">Agnostic</option>
                                            <option value="Orthodox">Orthodox</option>
                                            <option value="Catholic">Catholic</option>
                                            <option value="Evangelical">Evangelical</option>
                                            <option value="Protestant">Protestant</option>
                                            <option value="Spiritist">Spiritist</option>
                                            <option value="Methodist">Methodist</option>
                                            <option value="Christian">Christian</option>
                                            <option value="Hindu">Hindu</option>
                                            <option value="Buddhist">Buddhist</option>
                                            <option value="Sikh">Sikh</option>
                                            <option value="Qadiani">Qadiani</option>
                                            <option value="Bahai">Bahai</option>
                                            <option value="Jewish">Jewish</option>
                                            <option value="Greek Orthodox">Greek Orthodox</option>
                                            <option value="Unknown">Unknown</option>
                                        </select>
                                    </div>
                                    <div class="col-4">
                                        <label class="control-label">Work Hours Per Day</label>
                                        <input type="text" class="form-control" id="work_hours_day" name="work_hours_day">
                                    </div>

                                </div>
                                <div class="form-group">
                                    <div class="col-4">
                                        <label class="control-label">Work Hours Per Week</label>
                                        <input type="text" class="form-control" id="work_hours_week" name="work_hours_week">
                                    </div>
                                    <div class="col-4">
                                        <label class="control-label">Work Hours Per Month</label>
                                        <input type="text" class="form-control" id="work_hours_month" name="work_hours_month">
                                    </div>
                                </div>
                                <!-- /.form-group -->


                                <!-- /.col-8 -->
                            </div>
                            <!-- /.container -->
                        </div>
                        <!-- /.card-body -->
                        <div class="card-footer text-center">
                            <button class="btn btn-blue" id="saveButton" data-control="button" data-role="employee/save" data-loading-text="Saving ...">
                                Save Employee
                            </button>
                        </div>
                        <!-- /.card-footer -->
                    </div>
                    <!-- /.card -->
            </form>
        <!-- /.form -->
        </div>
        
         <div class="tab-panel" id="edtTab">
            
            <!-- Employee Data Transaction starts here
            
            <div class="modal-body">

                    <!-- Employee Transaction Form -->
                   
 
                    <form method="post" id="EmployeeDataTransactionForm">
                        <div class="form-group eligible_class">
                            <div class="col-4">
                                <label class="form-control1" style="display:none">Employee Code</label>
                                <input type="text" class="form-control" style="display:none" id="employee_data" name="employee_data" />
                             </div>
                            <div class="col-4">
                               <%--<button class="btn btn-blue" id="filteremployeebutton"   data-role="employee-filter" data-loading-text="Saving ..."  >Filter Employee</button>--%>
                                 <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="employee-filter" style="display:none" id="filteremployeebutton">
                                     <span class="fa fa-filter"></span>Filters
                                 </a>
                  
                            </div>
                        <div class="col-4" style="display: none">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="to_date" name="to_date" />
                            </div>
                        </div>
                        <table id="shift" class="table" width="900" cellspacing="0" cellpadding="20">
                            <tbody>
                   
                              <!--  <div class="form-group eligible_class"> -->
                                <tr>
                                    
                                        <th width="150">
                                            <label class="text-blue"><strong>Shift</strong></label></th>
                                    
                                        <th width="150">
                                            <label class="text-blue"><strong>From Date</strong></label></th>
                                   
                                        <th width="150">
                                            <label class="text-blue"><strong>End Date</strong></label></th>
                                    
                                        <th width="350">
                                            <label class="text-blue"><strong>Shift Name</strong></label></th>
                                    
                                
                                        <th width="20">
                                            <label class="text-blue"><strong>Action</strong></label></th>
                                        <th width="20"/>
                                    
                                </tr>
                                
                               
                            </tbody>
                    </table>
                    
                       
                        <table class="table" width="900" cellspacing="0" cellpadding="20" id="ot">
                            <tbody>
                            <tr>
                                <th width="150"> <label class="text-blue"><strong>OT</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>From Date</strong></label></th> 
                                <th width="150"> <label class="text-blue"><strong>End Date</strong></label></th> 
                                <th width="350"></th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr>
                                
                           </tbody>
                    </table>
                      
                         <table class="table" width="900" cellspacing="0" cellpadding="20" id="ramadan">
                         <tbody>
                            <tr>
                                <th width="150"> <label class="text-blue"><strong>Ramadan</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>From Date</strong></label></th> 
                                <th width="150"> <label class="text-blue"><strong>End Date</strong></label></th>
                                <th width="350"></th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr>
                          
                            </tbody>
                    </table>
                    
                       
                         <table class="table" width="900" cellspacing="0" cellpadding="0" id="punchexception">
                        <tbody>                 
                            <tr>
                                <th width="150"> <label class="text-blue"><strong>Punch Exception</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>From Date</strong></label></th> 
                                <th width="150"> <label class="text-blue"><strong>End Date</strong></label></th>
                                <th width="350"></th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr> 
                        </tbody>
                            
                    </table>
                       
                    <table class="table" width="900" cellspacing="0" cellpadding="0" id="workhourperday">
                        <tbody>
                             <tr>
                                <th width="150"> <label class="text-blue"><strong>Work Hour Per Day</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>From Date</strong></label></th> 
                                <th width="150"> <label class="text-blue"><strong>End Date</strong></label></th>
                                <th width="350"> <label class="text-blue"><strong>Hours</strong></label></th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr> 
                         </tbody>
                       </table>
                       

                    <table class="table" width="900" cellspacing="0" cellpadding="0" id="workhourperweek">
                        <tbody>
                              <tr>
                                <th width="150"> <label class="text-blue"><strong>Work Hour Per Week</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>From Date</strong></label></th> 
                                <th width="150"> <label class="text-blue"><strong>End Date</strong></label></th>
                                <th width="350"> <label class="text-blue"><strong>Hours</strong></label></th> 
                               <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr> 
                         </tbody>
                        
                    </table>
                        
                    <table class="table" width="900" cellspacing="0" cellpadding="0" id="workhourpermonth">
                        <tbody>
                              <tr>
                                <th width="150"> <label class="text-blue"><strong>Work Hour Per Month</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>From Date</strong></label></th> 
                                <th width="150"> <label class="text-blue"><strong>End Date</strong></label></th>
                                <th width="350"> <label class="text-blue"><strong>Hours</strong></label></th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr> 
                            </tbody>
                        
                    </table>
                       
                    <table class="table" width="900" cellspacing="0" cellpadding="0" id="Maternity">
                        <tbody>
                            
                             <tr>
                                <th width="150"> <label class="text-blue"><strong>Maternity</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>Child date of birth</strong></label></th> 
                                <th width="150"> </th>
                                <th width="350"> </th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr> 
                                
                        </tbody>
                            
                    </table>
                       
                    <table class="table" width="900" cellspacing="0" cellpadding="0" id="termination">
                        <tr>
                                <th width="150"> <label class="text-blue"><strong>Termination</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>Date of Termination</strong></label></th> 
                                <th width="150"> </th>
                                <th width="350"> </th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                         </tr> 
                        </tbody>
                    </table>
                       
                    <table class="table" width="900" cellspacing="0" cellpadding="20" id="manager">
                         <tbody>
                              <tr>
                                <th width="150"> <label class="text-blue"><strong>Manager</strong></label></th>  
                                <th width="150"> <label class="text-blue"><strong>From Date</strong></label></th> 
                                <th width="150"> <label class="text-blue"><strong>End Date</strong></label></th>
                                <th width="350"> <label class="text-blue"><strong>Manager Id</strong></label></th> 
                                <th width="20"> <label class="text-blue"><strong>Action</strong></label></th> 
                                <th width="20"/> 
                            </tr> 
                         </tbody>
                        
                    </table> 
				<div class="card-footer text-center">
                    <!--<a href="javascript:void(0);" class="text-red" data-control="close" style="display:none" data-dismiss="modal" data-target="#saveDialog">Cancel</a> -->
                    <button class="btn btn-blue" id="Button1" data-control="button" data-role="employee-save" data-loading-text="Saving ...">
                        Save Employee Transaction</button>
                </div>					
                    </form>
					
                    <!-- End Employee Transaction Form -->
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                
                <!-- /.modal-footer 
            </div>
            
            
            
            
            
            
            Employee Data Transaction starts here --> 
        </div>
      </div>
   </div>
</div>
    <!-- /.container -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script src="http://code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css">
    <script type="text/javascript" src="../../../resources/js/company/manage_employee.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
</asp:Content>
