<%@ Page Language="C#" AutoEventWireup="true" CodeFile="home.aspx.cs" Inherits="profile_home" Title="Home | SecurTime" MasterPageFile="~/layout/base.master"%>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
		<div class="pull-left">
			<small class="current-section">PROFILE</small>
			<p class="current-page">DASHBOARD</p>
		</div>
		<!-- /.pull-left -->
		<div class="pull-right">
		</div>
		<!-- /.pul-right -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<section id="employeeDashboard" class="hide">
		<div class="container">
			<div class="col-6">
	            <div class="card">
	                <div class="card-body">
	                    <div id="calendar"></div>
	                    <div class="loading"></div>
	                </div>
	                <!-- /.card-body-->
	            </div>
	            <!-- /.card -->
			</div>
			<!-- /.col-6 -->
			<div class="col-6">
				<div class="card widget" id="employeeDetails">
					<div class="card-body">
		                <div class="container">
		                	<div class="pull-left">
			                    <img src='<%= Session["display_picture"].ToString() %>' class="display-picture" id="homePageDP">
			                </div>
			                <!-- /.pull-left -->
			                <div class="pull-left">
			                    <table cellpadding="0" cellspacing="0">
			                        <tbody>
			                            <tr>
			                                <td><h3 id="homePageUsername"></h3></td>
			                            </tr>
			                            <tr>
			                                <td><p id="homePageDepartment"></p></td>
			                            </tr>
			                            <tr>
			                                <td><p id="homePageDesignation"></p></td>
			                            </tr>
			                        </tbody>
			                    </table>
			                </div>
			                <!-- /.pull-left -->
		                </div>
		                <!-- /.container -->
	                </div>
	                <!-- /.card-body -->
	            </div>
	            <!-- /.card -->
	            <div class="card no-padding widget" style="height: 307px; overflow-y:scroll;">
                   <%-- <div class="card-header">
                        <p>Leave* Count: <span id="leaveStarCount"></span></p>
                    </div>--%>
                    <div class="card-header">
                        Leaves Available
                    </div>
                    <div class="card-body">
                        <div class="table-container" id="leavesAvailableTable">
                            <table class="table width-12" cellpadding="0" cellspacing="0">
                                <thead>
                                    <th>Leave Name</th>
                                    <th>Used</th>
                                    <th>Balance</th>
                                    <th>CarryForwardLeave</th>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                        <!-- /.table-container -->
                        <div class="no-data text-center hide">
                            <p><span class="text-orange fa fa-frown-o"></span> <strong>No data to display for this widget.</strong></p>
                        </div>
                        <div class="loading"></div>
                    </div>
                    <!-- /.card-body -->
                </div>
                <!-- /.card -->
			</div>
			<!-- /.col-6 -->
		</div>
		<!-- /.container -->
		<div class="container">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <p class="card-header-text">Number of hours worked</p>
                    </div>
                    <div class="card-body">
                    	<div class="no-data text-center hide">
                            <p><span class="text-orange fa fa-frown-o"></span> <strong>No data to display for this widget</strong></p>
                        </div>
                        <!-- /.no-data -->
                        <div class="graph-container" id="hoursWorkedGraph">
                            <canvas></canvas>
                        </div>
                        <!-- /.graph-container -->
                        <div class="loading"></div>
                    </div>
                    <!-- /.card-body -->
                </div>
                <!-- /.card -->
            </div>
            <!-- /.col-12 -->
		</div>
		<!-- /.container -->
	</section>
	<!-- /#employeeDashboard -->
    <section id="adminDashboard" class="hide">
    	<div class="container">
    		<div class="card">
    			<div class="card-header">
    				Employee Strength Today
    			</div>
    			<!-- /.card-header -->
    			<div class="card-body">
    				<div class="no-data text-center hide">
                        <p><span class="text-orange fa fa-frown-o"></span> <strong>No data to display for today</strong></p>
                        <small>Please try again later</small>
                    </div>
                    <!-- /.no-data -->
    				<div class="graph-container container" id="employeeStrengthGraph">
    					<div class="col-offset-2 col-4">
	                        <canvas></canvas>
                        </div>
                        <!-- /.col-6 -->
                        <div class="col-3">
                        	<div class="form-group">
								<h2>Welcome back!</h2>
								<br /><br />
							</div>
	                        <table class="table center-block" cellpadding="0" cellspacing="0">
	                            <tbody>
	                                <tr>
	                                	<td class="text-left"><p><span class="fa fa-circle text-green"></span> <small>PRESENT<small></p></td>
	                                    <td><h2 id="presentEmployeeCount" class="text-green"></h2></td>
                                    </tr>
                                    <tr>
                                    	<td class="text-left"><p><span class="fa fa-circle text-red"></span> <small>ABSENT</small></p></td>
	                                    <td><h2 id="absentEmployeeCount" class="text-red"></h2></td>
	                                </tr>
	                            </tbody>
	                        </table>
                       	</div>
                       	<!-- /.col-6 -->
                    </div>
                    <!-- /.graph-container -->
                    <div class="loading"></div>
    			</div>
    			<!-- /.card-body -->
    		</div>
    		<!-- /.card -->
    	</div>
    	<!-- /.container -->
	</section>
    <!-- /#adminDashboard -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<link rel="stylesheet" href="../../resources/css/fullcalendar.min.css">
	<link rel="stylesheet" href="css/home.css">
	<script type="text/javascript" src="../../resources/lib/moment.min.js"></script>
	<script type="text/javascript" src="../../resources/lib/fullcalendar.min.js"></script>
    <script type="text/javascript" src="../../resources/lib/Chart.min.js"></script>
	<script type="text/javascript" src="../../resources/js/profile/home.js"></script>
</asp:Content>