
<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="roster.aspx.cs" Inherits="shift_roster" Title="Shift Roster | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
		<style type="text/css">
		/*#shiftRosterTable {
			table-layout: fixed;
		}
		#shiftRosterTable td,
		#shiftRosterTable th {
			width: 200px;
		}*/
        #table-listing{
  width: 100%; 
           }
          .table-scrollable {
    width:auto;
    overflow-x:scroll;
    overflow-y: hidden;
    border: 1px solid #dddddd;
    margin: 10px 0 ;
          }  

   .table th{
       width:60px;
           }
        .table td{
  vertical-align:top;
  border-bottom: 1px solid #ddd;
  width:100px;
   
                  }
	</style>
   <%--  th- white-space:nowrap; --%>
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">SHIFT ROSTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="filters/toggle">
                	<span class="fa fa-filter"></span> Filters
                </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div id="employeeTab">
		<div class="container">
			<div class="card no-padding" id="filters">
				<div class="card-body">
					<form method="" id="filterForm" name="filterForm">
		                <div class="form-group">
		                    <div class="col-2">
		                        <label><strong><small>Company</small></strong></label>
		                        <select class="form-control" id="filter_company" name="filter_company">
		                            <option value="select">Select Company</option>
		                        </select>
		                    </div>

			                <div class="col-2">
			                    <label><strong><small>Department</small></strong></label>
			                    <select class="form-control" id="filter_department" name="filter_department">
			                        <option value="select">Select Department</option>
			                    </select>
			                </div>

			                <div class="col-2">
			                    <label><strong><small>Designation</small></strong></label>
			                    <select class="form-control" id="filter_designation" name="filter_designation">
			                        <option value="select">Select Designation</option>
			                    </select>
			                </div>
		                    
		                    <div class="col-2">
		                        <label><strong><small>Employee Category</small></strong></label>
		                        <select class="form-control" id="filter_employee_category" name="filter_employee_category">
		                        	<option value="select">Select Employee Category</option>
		                        </select>
		                    </div>
		                    
		                    <div class="col-2">
		                        <label><strong><small>Employee ID</small></strong></label>
		                        <input type="text" class="form-control" id="filter_employee_code" name="filter_employee_code" />
		                    </div>

		                    <div class="col-2">
		                    	<label><strong><small>Employee Name</small></strong></label>
		                        <input type="text" class="form-control" id="filter_employee_name" name="filter_employee_name" />
		                    </div>
		                    <!-- /.col-2 -->
		                </div>
		                <!-- /.form-group -->
		            </form>
		            <!-- /.form -->
				</div>
				<!-- /.card-body -->
				<div class="card-footer">
					<div class="container">
		                <div class="pull-right">
		                    <a class="btn" data-control="button" data-role="filters/reset">
		                        <span class="fa fa-refresh"></span> Reset Filters
		                    </a>
		                    <button class="btn btn-blue" id="filterButton" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
		                        <span class="fa fa-search"></span> Search
		                    </button>
		                </div>
		                <!-- /.pull-right -->
		            </div>
		            <!-- /.container -->
				</div>
				<!-- /.card-footer -->
			</div>
			<!-- /.card -->
		</div>
		<!-- /.container -->

		<div class="container text-center" id="noData">
		</div>
		<!-- /.container -->

		<div class="container" id="employeeListView">
			<div class="card no-padding">
				<div class="card-body" >
					<table class="table width-12" cellpadding="0" cellspacing="0" id="employeeTable">
	                    <thead>
	                        <tr> 
	                            <th><input type="checkbox" id="checkall" /></th>
	                            <th>Employee ID</th>
	                            <th>Employee Name</th>
	                        </tr>
	                    </thead>
	                    <tbody></tbody>
	                </table>
				</div>
				<!-- /.card-body -->
				<div class="card-footer text-center">
					<button type="button" class="btn btn-blue" data-control="button" data-role="next-page" data-loading-text="Processing ...">
                        Next
                         <span class="fa fa-long-arrow-right"></span>
                    </button>
				</div>
				<!-- /.card-footer -->
			</div>
			<!-- /.card -->
			<div class="text-center">
	            <button class="btn pagination" id="paginationButton" data-control="button" data-role="employee/more">
	                load more data
	            </button>
	        </div>
	        <!-- /.text-center -->
		</div>
		<!-- /.container -->
	</div>
	<!-- /#employeeTab -->
	<div class="hide" id="rosterTab">
		<div class="card">
			<div class="card-body">
				<div class="container">
					<div class="form-group">
						<div class="col-4">
							<input type="text" class="form-control date-picker" id="calendar" name="calendar">
						</div>
					</div>
				</div>

		<!--	<div class="container"   style="overflow-y: hidden; overflow-x: scroll;"> -->

               <div id="table-listing">
                   <div class="table-scrollable">
                        <table cellpadding="0" cellspacing="0" data-count-fixed-columns="3"  id="shiftRosterTable" class="table">
                            <thead></thead>
                            <tbody></tbody>
                        </table>
                    </div> 
                    </div> 
			<!--	</div> -->

				<!-- /.container -->
				<div class="clearfix"></div>
			</div>
			<!-- /.card-body -->
			<div class="card-footer">
				<div class="container text-center">
					<a href="javascript:void(0);" class="text-red" data-control="button" data-role="previous-page">
			            <span class="fa fa-long-arrow-left"></span>
			             Select Different Employee
			        </a>
			        <button class="btn btn-blue" id="saveShiftRosterButton" data-control="button" data-role="shift/save" data-loading-text="Processing ...">
			            <span class="fa fa-floppy-o"></span>
			             Save Shift Roster
			        </button>
				</div> 
			</div>
			<!-- /.card-footer -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /#shiftTab -->
    <!--Dialog box for check red lable in table-->
	
    <div class="modal fade" tabindex="-1" role="dialog" id="checkRedLabelDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Shift Roaster Save Confirmation</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#checkRedLabelDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#checkRedLabelDialog">No</a>
                    <button class="btn btn-blue" id="confirmshift" data-control="button" data-role="shift/saveDialog" data-loading-text="Processing ...">Yes</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<script type="text/javascript" src="../../../resources/js/transaction/shift/roster.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <link rel="stylesheet" href="../../../resources/css/datepicker.css">
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
</asp:Content>
