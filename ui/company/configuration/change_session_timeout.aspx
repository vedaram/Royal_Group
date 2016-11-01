<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="change_session_timeout.aspx.cs" Inherits="configuration_change_session_timeout" Title="Change Session Timeout | SecurTime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">CONFIGURATION</small> / <span class="current-page">CHANGE SESSION TIMEOUT</span>
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

<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
				</div>
		<!-- /.container -->
		<form class="form" id="#">
			
			<div class="card">
				<div class="card-header">
				    SESSION TIMEOUT
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<div class="form-group">
						<div class="col-4">
							<label class="text-blue"><strong><small>Session Timeout (in minutes)</small></strong></label>
							<input type="text" class="form-control" name="timeout" id="timeout" maxlength="3"  onkeypress="javascript:return isNumber(event)"/>
						</div>
						<div class="col-4" style="text-align:center;">
						<button type="button" class="btn btn-blue" id="Button1" data-control="button" data-role="#" data-loading-text="Saving ...">
							Save
						</button>
						</div>
						
					<!-- /.form-group -->
				</div>
				<!-- /.card-body -->
				
			</div>
			<!-- /.card -->
		</form>
		<!-- /.form -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" runat="server">  
<script type="text/javascript" src="../../../resources/js/company/change_session_timeout.js" ></script>
</asp:Content>

