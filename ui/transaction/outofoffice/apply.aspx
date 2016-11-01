<%@ Page Title="OOO Application | SecurTime" Language="C#" MasterPageFile="~/layout/base.master"
    AutoEventWireup="true" CodeFile="apply.aspx.cs" Inherits="outofoffice_apply" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" runat="Server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">OUT
                    OF OFFICE APPLICATION</span>
            </div>
        </div>
        <!-- /.pull-left -->
    </div>
</asp:Content>
<asp:Content ID="Content2"  ContentPlaceHolderID="saxMasterPageContent" runat="Server">
    <div class="container" >
        <div class="card">
            <div class="card-header">
                <h4>
                    APPLY OUT OF OFFICE</h4>
            </div>
            <div class="card-body">
                <div class="container">
                    <div class="col-12">
                        <form id="saveoutofofficeForm" method="post">
                            <div class="form-group">
                                <div class="col-3">
                                    <label class="control-label">
                                        Employee ID</label>
                                    <input type="text" class="form-control" id="employee_id" name="employee_id" />
                                </div>
                                <div class="col-3">
                                    <label class="control-label">
                                        Employee Name</label>
                                    <input type="text" class="form-control" id="employee_name" name="employee_name" readonly='true' />
                                </div>
                            </div>
                            <!-- /.form-group -->
                            <div class="form-group">
                                <div class="col-3">
                                    <label class="control-label">
                                        Out Of Office Type</label>
                                    <select class="form-control" id="outofoffice" name="outofoffice">
                                        <option value="select">-- Select --</option>
                                        <option value="1">Personal</option>
                                        <option value="2">Local Business</option>
                                        <option value="3">Business Trip / Training</option>
                                    </select>
                                </div>
                            </div>
                            <!-- /.form-group -->
                            <div class="form-group">
                                <div class="col-3">
                                    <input type="text" class="form-control" id="in_date" name="in_date" />
                                    <label class="control-label">
                                        From Date</label>
                                </div>
                                <div class="col-3">
                                    <input type="text" class="form-control timepicker" id="in_time" name="in_time" />
                                    <label class="control-label">
                                        From Time</label>
                                </div>
                                <div class="col-3">
                                    <input type="text" class="form-control datepicker" id="out_date" name="out_date" />
                                    <label class="control-label">
                                        To Date</label>
                                </div>
                                <div class="col-3">
                                    <input type="text" class="form-control timepicker" id="out_time" name="out_time" />
                                    <label class="control-label">
                                        To Time</label>
                                </div>
                            </div>
                            <!-- /.form-group -->
                            <div class="form-group">
                                <div class="col-3">
                                    <input type="text" class="form-control" readonly="true" id="total_days" name="total_days" />
                                    <label class="control-label">
                                        Total Days</label>
                                </div>
                                <div class="col-3">
                                    <input type="text" class="form-control" readonly="true" id="total_hour" name="total_hour" />
                                    <label class="control-label">
                                        Total Time (in minutes)</label>
                                </div>
                            </div>
                            <!-- /.form-group -->
                            <div class="form-group">
                                <div class="col-12">
                                    <label class="control-label">
                                        Reason</label>
                                    <textarea class="form-control" id="reason" name="reason"></textarea>
                                </div>
                                <!-- /.col-offset-2 col-8 -->
                            </div>
                            <!-- /.form-group -->
                        </form>
                        <!-- /.form -->
                    </div>
                    <!-- /.col-8 -->
                </div>
                <!-- /.container -->
            </div>
            <!-- /.card-body -->
            <div class="card-footer">
                <div class="text-center">
                    <button type="button" class="btn btn-blue" id="saveoutofofficeButton" data-control="button"
                        data-role="ooo/validate" data-loading-text="Saving ...">
                        <span class="fa fa-floppy-o"></span>Save
                    </button>
                </div>
                <!-- /.text-center -->
            </div>
            <!-- /.card-footer -->
        </div>
        <!-- /.card -->
    </div>
    <div class="modal fade" tabindex="-1" role="dialog" id="checkOOODialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>OUT OFF OFFICE Application</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#checkOOODialog">X</a>
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
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#checkOOODialog">No</a>
                    <button class="btn btn-blue" id="confirmOOOButton" data-control="button" data-role="ooo/submit" data-loading-text="Processing ...">Yes</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" runat="Server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css">
    <link rel="stylesheet" href="../../../resources/css/timepicki.css">

    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>

    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>

    <script type="text/javascript" src="../../../resources/lib/timepicki.js"></script>

    <script type="text/javascript" src="../../../resources/js/transaction/outofoffice/apply.js"></script>

</asp:Content>
