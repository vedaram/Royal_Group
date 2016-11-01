<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="import.aspx.cs" Inherits="shift_import" Title="Shift Roster Import | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">SHIFT ROSTER IMPORT</span>
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
		<div class="card">
			<div class="card-header">
				<div class="container">
					<div class="pull-left" style="line-height: 30px;">
                        <h4>IMPORT SHIFT MASTER</h4>
                    </div>
                    <!-- /.pull-left -->
                    <div class="pull-right">
                    	<button type="button" id="shiftExportButton" class="btn btn-blue" data-control="button" data-role="shift/export" data-loading-text="Exporting ...">
                    		<span class="fa fa-file-excel-o"></span> Shift Export
                    	</button>
                        <a href="../../../exports/templates/shift_roster.xlsx" class="btn btn-blue" target="_blank">
                            <span class="fa fa-file-excel-o"></span> Shift Roster Template
                        </a>
                    </div>
                    <!-- /.pull-right -->
				</div>
				<!-- /.container -->
			</div>
			<div class="card-body">
			 	<div class="form-group">
			 		<div class="container">
	                    <div class="col-4">
	                        <label class="control-label">Choose file to import</label>
	                        <input type="file" class="form-control" id="file_upload" name="file_upload" />
	                    </div>
	                    <!-- /.col-4 -->
	                    <div class="col-4">
	                        <button class="btn btn-blue" id="importShiftButton" data-control="button" data-role="shift/import" data-control-loading="Importing ...">
	                            <span class="fa fa-upload"></span>
	                             Import Shift Roster
	                        </button>
	                    </div>
	                    <!-- /.col-4 -->
                    </div>
                    <!-- /.container -->
                </div>
                	
                <!-- /.form-group -->
                <div class="form-group">
                	<div class="container">
                		<div class="col-12">
		                	<label class="control-label">Employee Master Import Result</label><br />
		                    <textarea class="form-control" id="importShiftResult" style="height: 200px; border-bottom: 1px solid #ddd;" disabled></textarea>
	                    </div>
	                    <!-- /.col-12 -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.form-group -->
			</div>
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<script type="text/javascript" src="../../../resources/js/transaction/shift/import.js"></script>
</asp:Content>