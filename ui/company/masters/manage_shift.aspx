<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="manage_shift.aspx.cs" Inherits="masters_manage_shift" Title="Manage Shift | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
        <div class="pull-left">
        	<div class="current-page-outer">
            	<small class="current-section">MASTERS</small> / <span class="current-page">MANAGE SHIFT</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        	<div class="action-bar">
	        	<a href="shift.aspx" class="btn btn-blue">
	        		<span class="fa fa-long-arrow-left"></span> Back
	    		</a>
    		</div>
    		<!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<form class="form" method="" id="saveForm">
			<div class="card no-padding">
				<div class="card-header">
					SHIFT DETAILS
				</div>
				<div class="card-body">
					<br />
					<div class="form-group">
			            <div class="col-4">
			                <select class="form-control" id="company_code" name="company_code" >
			                    <option value="select">Select Company</option>
			                </select>
			                <label class="control-label">Company Name</label>
			            </div>
			            
			            <div class="col-4">
			                        <label class="control-label container">
		                        		Branch			                        	
		                        	</label>
			                        <select class="form-control" name="branch" id="branch"  >
			                            <option value="select">Select a Branch</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                    
			                    <div class="col-4">
			                       <label class="control-label">Category</label>
			                        <select class="form-control" name="category" id="category"  >
			                            <option value="select">Select Category</option>
			                        </select>
			                    </div></div>
			                    
			                    
			                <div class="form-group">
			                
			                <div class="col-3">
			                    <input type="text" class="form-control" id="shift_code" name="shift_code" />
			                    <label class="control-label">Shift Code</label>
			                </div>
			            
			                <div class="col-3">
			                    <input type="text" class="form-control" id="shift_desc" name="shift_desc" />
			                    <label class="control-label">Shift Name</label>
			                </div>
			                
			                <div class="col-3">
			                    <input type="checkbox" class="" id="auto_checked" name="auto_checked" /> <span class="form-control1">Is Auto Shift Eligible?</span> 
			                </div>
			                
			                <div class="col-3">
			                    <input type="checkbox" class="" id="is_active" name="is_active" /> <span class="form-control1">Is Active Shift?</span> 
			                </div>
			            
			        </div>
			        <!-- /.form-group -->
		       	</div>
		       	<!-- /.card-body -->
	       	</div>
	       	<!-- /.card -->
	       	<div class="card no-padding">
		        <div class="card-header">
		        	<label>
	                    <input type="checkbox" id="check_if_normal_shift" name="check_if_normal_shift" checked/>
	                     IS NORMAL SHIFT REQUIRED?
	                </label>
	            </div>
	            <!-- /.card-header-->
	        	<div class="card-body">
	        		<br />
	                <div class="form-group">
	                	<div class="col-3">
	                        <input type="text" class="form-control timepicker" id="in_time" name="in_time">
	                        <label class="control-label">Start Time</label>
	                    </div>

	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker normal-shift-disable" id="break_out" name="break_out" disabled />
	                        <label class="control-label">Break Out</label>
	                    </div>

	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker normal-shift-disable" id="break_in" name="break_in" disabled />
	                        <label class="control-label">Break In</label>
	                    </div>

	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker" id="out_time" name="out_time">
	                        <label class="control-label">End Time</label>
	                    </div>
	                </div>
	                <!-- /.form-group -->
	            </div>
	            <!-- /.card-body -->
	        </div>
	        <!-- /.card -->
	        <div class="card no-padding">
	            <div class="card-header">
	                <p>CUT OFF</p>
	            </div>
	            <!-- /.card-header -->
	            <div class="card-body">
	            	<br />
	                <div class="form-group">
	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker normal-shift-disable" id="in_grace" name="in_grace" disabled />
	                        <label class="control-label">Start Time</label>
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker normal-shift-disable" id="break_out_grace" name="break_out_grace" disabled />
	                        <label class="control-label">Break Out</label>
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker normal-shift-disable" id="break_in_grace" name="break_in_grace" disabled />
	                        <label class="control-label">Break In</label>
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker normal-shift-disable" id="out_grace" name="out_grace" disabled />
	                        <label class="control-label">End Time</label>
	                    </div>
	                    <!-- /.col-3 -->
	                </div>
	                <!-- /.form-group -->
	                <div class="form-group">
	                    <div class="col-offset-3 col-6">
	                        <label> 
	                            <input type="checkbox" id="status_night_shift" name="status_night_shift" />
	                            <strong><small class="text-blue">Shift Cutoff Time</small></strong>
	                        </label> 
	                        <input type="text" class="form-control timepicker night-shift-disable" id="max_shift_end_cut_off_time" name="max_shift_end_cut_off_time" disabled/>
	                    </div>
	                    <!-- /.col-6 -->
	                </div>
	                <!-- /.form-group -->
	            </div>
	            <!-- /.card-body -->
	        </div>
	        <!-- /.card -->
	        <div class="card no-padding">
	            <div class="card-header">
	                <p>GRACE TIME IN MINUTES</p>
	            </div>
	            <!-- /.card-header -->
	            <div class="card-body">
	            	<br />
	                <div class="form-group">
	                    <div class="col-offset-3 col-6">
	                        <div class="form-group">
	                            <div class="col-6">
	                                <label class="control-label">In</label>
	                                <input type="text" class="form-control" id="grace_in" name="grace_in" />
	                            </div>
	                            <!-- /.col-6 -->
	                            <div class="col-6">
	                                <label class="control-label">Break Out</label>
	                                <input type="text" class="form-control normal-shift-disable" id="grace_break_out" name="grace_break_out" disabled/>
	                            </div>
	                            <!-- /.col-6 -->
	                        </div>
	                        <!-- /.form-group -->
	                        <div class="form-group">
	                            <div class="col-6">
	                                <label class="control-label">Out</label>
	                                <input type="text" class="form-control" id="grace_out" name="grace_out" />
	                            </div>
	                            <!-- /.col-6 -->
	                            <div class="col-6">
	                                <label class="control-label">Break In</label>
	                                <input type="text" class="form-control normal-shift-disable" id="grace_break_in" name="grace_break_in" disabled/>
	                            </div>
	                            <!--/. col-6 -->
	                        </div>
	                        <!-- /.form-group -->
	                    </div>
	                    <!-- /.col-6 -->
	                </div>
	                <!-- /.form-group -->
	           	</div>
	           	<!-- /.card-body -->
	       	</div>
	       	<!-- /.card -->
	       	<div class="card no-padding">
	            <div class="card-header">
	                <label>
	                    <input type="checkbox" id="overtime" name="overtime" />
	                    IS OVERTIME APPLICABLE?
	                </label>
	            </div>
	            <!-- /.card-header -->
	            <div class="card-body">
	            	<br />
	                <div class="form-group">
	                    <div class="col-6">
	                        <input type="text" class="form-control timepicker overtime-disable" id="min_overtime" name="min_overtime" disabled/>
	                        <label class="control-label">Min Overtime</label>
	                    </div>
	                    <!-- /.col-6 -->
	                    <div class="col-6">
	                        <input type="text" class="form-control timepicker overtime-disable" id="max_overtime" name="max_overtime" disabled/>
	                        <label class="control-label">Max Overtime</label>
	                    </div>
	                    <!-- /.col-6 -->
	                </div>
	                <!-- /.form-group -->
	           	</div>
	           	<!-- /.card-body -->
	       	</div>
	       	<!-- /.card -->
	       	<div class="card no-padding">
	            <div class="card-header">
	                <label>
	                    <input type="checkbox" id="status_half_day" name="status_half_day" />
	                    IS HALF DAY APPLICABLE?
	                </label>
	            </div>
	            <!-- /.card-header -->
	            <div class="card-body">
	            	<br />
	                <div class="form-group">
	                    <div class="col-4">
	                        <select class="form-control half-day-disable" name="half_day" id="half_day" disabled>
	                            <option selected="selected" value="select">--Select--</option>
	                            <option value="Sunday">Sunday</option>
	                            <option value="Monday">Monday</option>
	                            <option value="Tuesday">Tuesday</option>
	                            <option value="Wednesday">Wednesday</option>
	                            <option value="Thursday">Thursday</option>
	                            <option value="Friday">Friday</option>
	                            <option value="Saturday">Saturday</option>
	                        </select>
	                        <label class="control-label">Half Day</label>
	                    </div>
	                    <!-- /.col-4 -->
	                    <div class="col-4">
	                        <input type="text" class="form-control timepicker half-day-disable" id="start_time_half_day" name="start_time_half_day" disabled/>
	                        <label class="control-label">Start Time</label>
	                    </div>
	                    <!-- /.col-4 -->
	                    <div class="col-4">
	                        <input type="text" class="form-control timepicker half-day-disable" id="end_time_half_day" name="end_time_half_day" disabled/>
	                        <label class="control-label">End Time</label>
	                    </div>
	                    <!-- /col-4 -->
	                </div>
	                <!-- /.form-group -->
	            </div>
	            <!-- /.card-body -->
	        </div>
	        <!-- /.card -->
	        <div class="card no-padding">
	            <div class="card-header">
	                <label>SELECT WEEKLY OFF</label>
	            </div>
	            <!-- /.card-header -->
	            <div class="card-body">
	            	<br />
	                <div class="form-group">
	                    <div class="col-6">
	                        <select class="form-control" id="weekly_off1" name="weekly_off1">
	                            <option value="select">Select Weekly Off 1</option>
	                            <option value="Sunday">Sunday</option>
	                            <option value="Monday">Monday</option>
	                            <option value="Tuesday">Tuesday</option>
	                            <option value="Wednesday">Wednesday</option>
	                            <option value="Thursday">Thursday</option>
	                            <option value="Friday">Friday</option>
	                            <option value="Saturday">Saturday</option>
	                        </select>
	                        <label class="control-label">Week Off 1</label>
	                    </div>
	                    <!-- /.col-6 -->
	                    <div class="col-6">
	                        <select class="form-control" id="weekly_off2" name="weekly_off2">
	                            <option value="select">Select Weekly Off 2</option>
	                            <option value="Sunday">Sunday</option>
	                            <option value="Monday">Monday</option>
	                            <option value="Tuesday">Tuesday</option>
	                            <option value="Wednesday">Wednesday</option>
	                            <option value="Thursday">Thursday</option>
	                            <option value="Friday">Friday</option>
	                            <option value="Saturday">Saturday</option>
	                        </select>
	                        <label class="control-label">Week Off 2</label>
	                    </div>
	                    <!-- /.col-6 -->
	               	</div>
	               	<!-- /.form-group -->
	           	</div>
	           	<!-- /.card-body -->
	       	</div>
	       	<!-- /.card -->
	       	<div class="card no-padding">
	            <div class="card-header">
	                <p>RAMDAN TIMINGS</p>
	            </div>
	            <!-- /.card-header -->
	            <div class="card-body">
	            	<br />
	                <div class="form-group">
	                    <div class="col-offset-3 col-3">
	                        <input type="text" class="form-control timepicker" id="ramadan_in_time" name="ramadan_in_time">
	                        <label class="control-label">Start Time</label>
	                    </div>
	                	<!-- /.col-3 -->
	                    <div class="col-3">
	                        <input type="text" class="form-control timepicker" id="ramadan_out_time" name="ramadan_out_time">
	                        <label class="control-label">End Time</label>
	                    </div>
	                    <!-- /.col-3 -->
	                </div>
	                <!-- /.form-group -->
				</div>
				<!-- /.card-body -->
				<div class="card-footer text-center">
					<button type="button"  class="btn btn-blue" id="saveButton" data-control="button" data-role="shift/save" data-loading-text="Saving ...">
						Save Shift
					</button>
				</div>
				<!-- /.card-footer -->
			</div>
			<!-- /.card -->
		</form>
		<!-- /.form -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<script type="text/javascript" src="../../../resources/js/company/manage_shift.js"></script>
    <script type="text/javascript" src="../../../resources/lib/timepicki.js"></script>
    <link rel="stylesheet" href="../../../resources/css/timepicki.css" >
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
</asp:Content>