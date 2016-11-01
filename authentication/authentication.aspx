<%@ Page Language="C#" AutoEventWireup="true" CodeFile="authentication.aspx.cs" Inherits="application_authentication" Title="Authentication | SecurTime"%>

<!DOCTYPE html>
<html>
	<head runat="server">
		 <link rel="ICON" href="../resources/images/favicon.png">
        <!-- page title -->
        <title>Authentication | SecurTime</title>
        <!-- normalize.css -->
        <link rel="stylesheet" href="../resources/css/normalize.css" >
        <!-- critical CSS -->
        <style type="text/css">
            * {
                box-sizing: border-box;
            }
            html,
            body {
                height: 100%;
            }
            body {
            	background: #eee;
                color: #4d4d4d;
                font-size: 13px;
                font-family: Helvetica, sans-serif;
                letter-spacing: 0.5px;
                padding: 0;
                margin: 0;
            }
            small {
                font-size: 11px !important;
            }
            a {
                color: #2980b9;
                text-decoration: none;
            }
            a:hover {
                text-decoration: underline;
            }
            h1, h2, h3, h4, h5, h6 {
                padding: 0;
                margin: 0;
                font-weight: 300;
            }
            p {
                margin: 0;
                padding: 0;
            }
            ul {
            	list-style-type: none;
            	margin: 0;
            	padding: 0;
            }

            .pull-left {
            	float: left;
            }
            .pull-right {
            	float: right;
            }

            .container {

            }
            .container::before,
            .container::after {
            	display: table;
            	content: '';
            }
            .container::after {
            	clear: both;
            }

            /* header */
            #saxPageHeader {
            	padding: 5px 20px 3px;
            	border-bottom: 1px solid #2980b9;
            	background: #3498db;
            	height: 40px;
            }
            	.brand {
            		height: 30px;
            	}
            		.logo {
            			width: 80px;
            		}

        		.header-menu li {
        			padding-left: 25px;
        			color: #fff;
        			float: right;
        			line-height: 30px;
        		}
        			.header-menu li a {
        				color: #fff;
        			}

			/* title bar */
			#saxPageTitleBar {
				background: #f9f9f9;
				height: 50px;
				border-bottom: 1px solid #ccc;
			}

			#saxPageContentOuter {
				position: absolute;
				top: 91px;
				bottom: 0px;
				left: 0px;
				right: 0px;
				overflow-x: hidden;
				overflow-y: auto;
			}
				#saxPageContent {
					position: absolute;
					top: 0px;
					bottom: 0px;
					right: 0px;
					left: 0px;
					overflow-x: hidden;
					overflow-y: auto;
				}
					#saxPageContent .content {
						padding: 20px;
					}

			/* card */
			.card {
				position: absolute;
				top: 20%;
				left: 30%;
				width: 500px;
				background: #fff;
				border: 1px solid #ccc;
				border-radius: 2px;
				padding: 30px;
			}

			/* table */
			.table {
				margin-top: 30px;
				width: 100%;
			}
				.table tbody td {
					padding: 15px 0;
				}
					.table tbody td:nth-child(2) {
						text-align: right;
						font-weight: bold;
					}

			.welcome {
				font-size: 24px;
				padding-bottom: 10px;
				border-bottom: 1px solid #ccc;
			}
				.status {
					font-size: 12px;
				}
				.text-green {
					color: #27ae60;
				}
				.text-red {
					color: #c0392b;
				}
        </style>
        <!-- other stylesheets -->
        <link rel="stylesheet" href="../resources/font-awesome/css/font-awesome.min.css">
	</head>

	<body>

		<section id="saxPage">
			<header id="saxPageHeader">
				<div class="container">
					<div class="pull-left">
						<div class="brand">
							<img src="../resources/images/logo100.png" alt="SecurAX" class="logo">
						</div>
					</div>
					<!-- /.pull-left -->
					<div class="pull-right">
					</div>
					<!-- /.pull-right -->
				</div>
				<!-- /.container -->
			</header>
			<!-- /#saxPageHeader -->
			<section class="page-title-bar" id="saxPageTitleBar">
			</section>
			<!-- /#saxPageTitleBar -->
			<section id="saxPageContentOuter">
				<section id="saxPageContent">
					<div class="content">
						<div class="card">
							<div class="card-body text-center">
								<h2 class="welcome">Welcome Back!</h2>

								<table class="table" cellspacing="0" cellpadding="0">
									<colgroup>
										<col width="80%">
										<col width="20%">
									</colgroup> 
									<tbody>
										<tr id="license">
											<td>Validating license ...</td>
											<td><p id="licenseStatus" class="status"></p></td>
										</tr>
										<tr id="session">
											<td>Loading session data ...</td>
											<td><p id="sessionStatus" class="status"></p></td>
										</tr>
										<tr id="menu">
											<td>Loading menu ...</td>
											<td><p id="menuStatus" class="status"></p></td>
										</tr>
									</tbody>
								</table>
								<!-- /.table -->
							</div>
						</div>
					</div>
					<!-- /.content -->
				</section>
				<!-- /#saxPageContent -->
			</section>
			<!--- /#saxPageContentOuter -->
		</section>

		<!-- loading scripts -->
		<script type="text/javascript" src="../resources/lib/sessvars.js"></script>
		<script type="text/javascript" src="../resources/lib/jquery.min.js"></script>
		<script type="text/javascript" src='../resources/lib/underscore.min.js?v=<%= Session["version"].ToString() %>'></script>
		<script type="text/javascript" src="../resources/js/authentication/authentication.js?v=002"></script>
		<script type="text/javascript" src="../resources/js/common/permissions_map.js?v=001"></script>
	</body>
</html>     