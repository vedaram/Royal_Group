<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="shift_setting.aspx.cs" Inherits="configuration_shift_setting" Title="Shift Settings | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">CONFIGURATION</small> / <span class="current-page">SHIFT SETTINGS</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<div class="container">
			<div class="col-offset-2 col-6">
				<div class="form-group">
					<label class="text-blue"><strong><small>Company</small></strong></label>
					<select class="form-control" id="company" name="company">
						<option value="select">Select Company</option>
					</select>
				</div>
				<!-- /.form-group -->
			</div>
			<!-- /.col-offset-2 .col-6 -->
		</div>
		<!-- /.container -->
		<form class="form" id="saveForm">
			<div class="card">
				<div class="card-header">
					BREAK DETAILS
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-offset-2 col-4">
							<label class="text-blue"><strong><small>Break Type</small></strong></label>
							<select class="form-control" id="break_type" name="break_type">
								<option value="1">Actual Break</option>
								<option value="0">Fixed Break</option>
							</select>
						</div>
						<!-- /.col-offset-2 col-4 -->
						<div class="col-4">
							<label class="text-blue"><strong><small>Break Hours</small></strong></label>
							<input type="text" class="timepicker form-control" id="break_hours" name="break_hours" disabled/>
						</div>
						<!-- /.col-4 -->
					</div>
					<!-- /.form-group -->
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
			<div class="card">
				<div class="card-header">
					AUTO SHIFT?
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-offset-4 col-4">
							<label class="text-blue"><strong><small>Need Auto Shift?</small></strong></label>
							<select class="form-control" name="is_auto_shift" id="is_auto_shift">
								<option value="0">No</option>
								<option value="1">Yes</option>
							</select>
						</div>
						<!-- /.col-offset-4 col-4 -->
					</div>
					<!-- /.form-group -->
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
			<div class="card">
				<div class="card-header">
					HOLIDAY COUNT
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-offset-4 col-4">
							<label class="text-blue"><strong><small>Max Holiday Count</small></strong></label>
							<input type="text" class="form-control" id="max_holiday_count" name="max_holiday_count" />
						</div>
						<!-- /.col-offset-4 col-4 -->
					</div>
					<!-- /.form-group -->
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
			<div class="card">
				<div class="card-header">
					<label>
						<input type="checkbox" id="is_ramadan" name="is_ramadan"/>
						RAMDAN SHIFT DATES
					</label>
					
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-6">
							<label class="text-blue"><strong><small>From</small></strong></label>
							<input type="text" class="form-control datepicker" name="ramadan_from_date" id="ramadan_from_date" disabled/>
						</div>
						<!-- /.col-6 -->
						<div class="col-6">
							<label class="text-blue"><strong><small>To</small></strong></label>
							<input type="text" class="form-control datepicker" name="ramadan_to_date" id="ramadan_to_date" disabled/>
						</div>
						<!-- /.col-6 -->
					</div>
					<!-- /.form-group -->
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
			<div class="card">
				<div class="card-header">
					<label>
						<input type="checkbox" id="break_deduction_required" name="break_deduction_required"/>
						BREAK DEDUCTION FOR WEEK OFF OR HOLIDAYS
					</label>
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-6">
							<label class="text-blue"><strong><small>Min Working Hours required</small></strong></label>
							<input type="text" class="form-control timepicker" name="min_work_hours_for_deduction" id="min_work_hours_for_deduction" disabled/>
						</div>
						<!-- /.col-6 -->
						<div class="col-6">
							<label class="text-blue"><strong><small>Break Deduction (in Minutes)</small></strong></label>
							<input type="text" class="form-control" name="break_deduction" id="break_deduction" disabled/>
						</div>
						<!-- /.col-6 -->
					</div>
					<!-- /.form-group -->
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
			<div class="card">
				<div class="card-header">
					WORK HOURS FOR PAYROLL
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-6">
							<label class="text-blue"><strong><small>Min Work Hours</small></strong></label>
							<input type="text" class="form-control timepicker" name="min_work_hours" id="min_work_hours"/>
						</div>
						<!-- /.col-6 -->
						<div class="col-6">
							<label class="text-blue"><strong><small>Max Work Hours</small></strong></label>
							<input type="text" class="form-control timepicker" name="max_work_hours" id="max_work_hours"/>
						</div>
						<!-- /.col-6 -->
					</div>
					<!-- /.form-group -->
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
			<div class="card">
				<div class="card-header">
					PAYROLL LEAVE DATES
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-4">
							<label class="text-blue"><strong><small>From Day</small></strong></label>
							<input type="text" class="form-control" name="from_day" id="from_day"/>
						</div>
						<!-- /.col-4 -->
						<div class="col-4">
							<label class="text-blue"><strong><small>To Day</small></strong></label>
							<input type="text" class="form-control" name="to_day" id="to_day"/>
						</div>
						<!-- /.col-4 -->
						<div class="col-4">
							<label class="text-blue"><strong><small>Block Day</small></strong></label>
							<input type="text" class="form-control" name="block_day" id="block_day"/>
						</div>
						<!-- /.col-4 -->
					</div>
					<!-- /.form-group -->
				</div>
				</div>
				<div class="card">
					<div class="card-header">
						WEEKOFF/HOLIDAY CUTOFF
					</div>
					<div class="card-body">
						<div class="form-group">
							<div class="col-6">
								<label class="text-blue"><strong><small>CUTOFF TIME</small></strong></label>
								<input type="text" class="form-control timepicker" name="cutoff_time" id="cutoff_time" />
							</div>
						</div>
					</div>
				 </div>
				</div>
				<!-- /.card-body -->
				<div class="card-footer">
					<div class="text-center">
						<button type="button" class="btn btn-blue" id="saveButton" data-control="button" data-role="shift-setting/save" data-loading-text="Saving ...">
							Save
						</button>
					</div>
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
	<script type="text/javascript" src="../../../resources/js/company/shift_setting.js"></script>
    <script type="text/javascript" src="../../../resources/lib/timepicki.js"></script>
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>

    <link rel="stylesheet" href="../../../resources/css/datepicker.css" >
    <link rel="stylesheet" href="../../../resources/css/timepicki.css" >
</asp:Content>