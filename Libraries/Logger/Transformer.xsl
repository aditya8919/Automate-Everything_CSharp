<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/TestExecutionStatus">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"></xsl:text>
		<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
			<head>
				<link href='http://fonts.googleapis.com/css?family=Open+Sans:400,700' rel='stylesheet' type='text/css' />
				<meta http-equiv="content-type" content="text/html; charset=utf-8" />
				<style type="text/css">
					body {
					font-family: 'Open Sans', sans-serif;
					margin: 0;
					padding: 0;
					background: #e5e5e5;
					color: #3f3f3f;
					font-size: 14px;
					line-height: 24px;
					}

					a { color: #000088; }
					a:hover { color: #880000; }

					#top {
					background: #2E627C;
					padding: 0 10px;
					height: 90px;
					line-height: 90px;
					border-bottom: 5px solid #7599a9;
					}

					#header {
					float: none;
					}
					#header h1 {
					margin: 0;
					padding: 0;
					font-weight: bold;
					color: #fff;
					font-size: 33px;
					letter-spacing: -1px;
					}

					#topmenu {
					float: right;
					}

					#contentwrap {
					width: auto;
					background: #f9f9f9;
					margin: 20px ;
					border: 1px solid #c9c9c9;
					-webkit-border-radius: 7px;
					-moz-border-radius: 7px;
					border-radius: 7px;
					}

					.cleft { float: none; padding: 20px 0 20px 20px; width: 200px; }
					.cleft h3 {
					-webkit-border-radius: 7px;
					-moz-border-radius: 7px;
					border-radius: 7px;
					font-size: 12px;
					font-weight: bold;
					color: #fff;
					background: #6a9d84;
					padding: 3px 10px;
					}

					.cright {
					-webkit-border-top-right-radius: 7px;
					-webkit-border-bottom-right-radius: 7px;
					-moz-border-radius-topright: 7px;
					-moz-border-radius-bottomright: 7px;
					border-top-right-radius: 7px;
					border-bottom-right-radius: 7px;
					border-left: 1px solid #e9e9e9;
					background: #fff;
					float: right;
					padding: 0 20px 20px 20px;
					width: 750px; }
					.cright h2 {
					-webkit-border-radius: 7px;
					-moz-border-radius: 7px;
					border-radius: 7px;
					background: #fffaae;
					padding: 10px;
					font-weight: normal;
					color: #585858;
					letter-spacing: -1px;
					font-weight: bold;
					margin: 20px -10px 10px -10px;

					}

					.cCenter {
					font-size: 11.4px;
					-webkit-border-top-right-radius: 7px;
					-webkit-border-bottom-right-radius: 7px;
					-moz-border-radius-topright: 7px;
					-moz-border-radius-bottomright: 7px;
					border-top-right-radius: 7px;
					border-bottom-right-radius: 7px;
					border-left: 1px solid #e9e9e9;
					background: #fff;
					float: none;
					padding: 0 20px 20px 20px;
					width: 1160px; }

					.cCenter h2 {
					-webkit-border-radius: 7px;
					-moz-border-radius: 7px;
					border-radius: 7px;
					background: #fffaae;
					padding: 10px;
					font-weight: normal;
					color: #585858;
					letter-spacing: -1px;
					font-weight: bold;
					margin: 20px -10px 10px -10px;
					width: 100%;
					}

					#footer {
					background: #4D606E;
					color: #fff;
					padding: 10px;
					position: relative;
					bottom: 0;
					left: 0;
					width: 100%;

					}

					#footer a { color: #fff; text-decoration: none; border-bottom: 3px  #fff; }
					#footer a:hover { border-bottom: 3px  #ccc; }

					.left { float: left; }
					.right { float: right; }
					.clear { clear: both; padding: 0 0 10px 0; }

					table, td{
					font:100% Arial, Helvetica, sans-serif;
					}
					table{width:100%;border-collapse:collapse;margin:1em 0;}
					th, td{text-align:left;padding:.5em;border:1px solid #fff;}
					th{background:#328aa4  repeat-x;color:#fff;}
					td{background:#e5f1f4;}
				</style>
				<title>Test Execution Report</title>
			</head>
			<body>
				<div id="top">
					<div id="header">
						<center>
							<h1>Test Execution Report</h1>
						</center>
					</div>
				</div>
				<div id="contentwrap">
					<div class="cright">
						<h2>Execution Summary</h2>

						<!--Below Table Store the Test Environment Details-->
						<table>
							<tr>
								<td>Operating System</td>
								<td>
									<xsl:value-of select="/TestExecutionStatus/TCLogger/OS" />
								</td>
							</tr>
							<tr>
								<td>Build Version</td>
								<td>
									<xsl:value-of select="/TestExecutionStatus/TCLogger/BuildVersion" />
								</td>
							</tr>
							<tr>
								<td>Environment</td>
								<td>
									<xsl:value-of select="/TestExecutionStatus/TCLogger/Env" />
								</td>
							</tr>
						</table>
						<!--Below Table Store the Class Level Results Pass/Fail and the Count of total tests executed-->
						<table>
							<tr>
								<th>Modules</th>
								<th># TC Pass</th>
								<th># TC Fail</th>
								<th># TC Other</th>
								<th># TC Inconclusive</th>
								<th># TC Executed</th>
								<th>ExecutionTime</th>
							</tr>
							<!--Identify the Unique Suite Names from the output xml-->
							<xsl:variable name="unique-list" select="//SuiteName[not(.=following::SuiteName)]" />
							<!--Identify the Class Level Pass Fail and total test executed count-->
							<xsl:for-each select="$unique-list">
								<xsl:variable name="Suite_Name" select="."/>
								<xsl:variable name="TC" select="count(/TestExecutionStatus/TCLogger[SuiteName=$Suite_Name])"/>
								<xsl:variable name="PC" select="count(/TestExecutionStatus/TCLogger[TestCaseResult='Passed'][SuiteName=$Suite_Name])"/>
								<xsl:variable name="FC" select="count(/TestExecutionStatus/TCLogger[TestCaseResult='Failed'][SuiteName=$Suite_Name])"/>
								<xsl:variable name="OC" select="count(/TestExecutionStatus/TCLogger[TestCaseResult='Other'][SuiteName=$Suite_Name])"/>
								<xsl:variable name="IC" select="count(/TestExecutionStatus/TCLogger[TestCaseResult='Inconclusive'][SuiteName=$Suite_Name])"/>
								<xsl:variable name="ET" select="sum(/TestExecutionStatus/TCLogger[SuiteName=$Suite_Name]/ExecutionTime)"/>
								<!--Display the Suite Level Pass/Fail/ Total Test Executed Count along Total Execution Time-->
								<tr>
									<td align='Center'>
										<xsl:value-of select="$Suite_Name" />
									</td>
									<td align='Center'>
										<xsl:value-of select='$PC' />
									</td>
									<td align='Center'>
										<xsl:value-of select='$FC' />
									</td>
									<td align='Center'>
										<xsl:value-of select='$OC'/>
									</td>
									<td align='Center'>
										<xsl:value-of select='$IC'/>
									</td>
									<td align='Center'>
										<xsl:value-of select='$TC'/>
									</td>
									<td>
										<xsl:value-of select="floor($ET div 60)" />m<xsl:value-of select="$ET mod 60" />s
									</td>
								</tr>
							</xsl:for-each>
							<!--Display the Total Pass/Fail/ Total Test Executed Count along Total Execution Time for All the Suite executed-->
							<tr bgcolor="#C3FDB8">
								<td align='Center'>
									<b>Total</b>
								</td>
								<td id="Pass" align='Center'>
									<b>
										<xsl:value-of select='count(TCLogger[TestCaseResult="Passed"])' />
									</b>
								</td>
								<td id="Fail" align='Center'>
									<b>
										<xsl:value-of select='count(TCLogger[TestCaseResult="Failed"])' />
									</b>
								</td>
								<td id="Oth" align='Center'>
									<b>
										<xsl:value-of select='count(TCLogger[TestCaseResult="Other"])'/>
									</b>
								</td>
								<td id="Inc" align='Center'>
									<b>
										<xsl:value-of select='count(TCLogger[TestCaseResult="Inconclusive"])'/>
									</b>
								</td>
								<td id="Total" align='Center'>
									<b>
										<xsl:value-of select='count(TCLogger[TestCaseResult="Passed"]) + count(TCLogger[TestCaseResult="Failed"]) + count(TCLogger[TestCaseResult="Other"]) + count(TCLogger[TestCaseResult="Inconclusive"])'/>
									</b>
								</td>
								<td id="ExecutionTime">
									<xsl:variable name="GET" select='sum(/TestExecutionStatus/TCLogger/ExecutionTime)' />
									<b>
										<xsl:value-of select="floor($GET div 60)" />m<xsl:value-of select="$GET mod 60" />s
									</b>
								</td>
							</tr>
							<!--Display the Result in terms of Pass/Fail Percentage-->
							<tr bgcolor="#C3FDB8">
								<td colspan='1' align='Center'>
									<b>% Status </b>
								</td>
								<td colspan='1' align='Center'>
									<xsl:variable name="PassVal" select='count(TCLogger[TestCaseResult="Passed"])'/>
									<xsl:variable name="TotalVal" select='count(TCLogger[TestCaseResult="Passed"]) + count(TCLogger[TestCaseResult="Failed"]) + count(TCLogger[TestCaseResult="Other"]) + count(TCLogger[TestCaseResult="Inconclusive"])'/>
									<b>
										<xsl:value-of select='format-number((100 * $PassVal div $TotalVal),"0.#")' /> %
									</b>
								</td>
								<td colspan='1' align='Center'>
									<xsl:variable name="FailVal" select='count(TCLogger[TestCaseResult="Failed"])'/>
									<xsl:variable name="TotalVal" select='count(TCLogger[TestCaseResult="Passed"]) + count(TCLogger[TestCaseResult="Failed"]) + count(TCLogger[TestCaseResult="Other"]) + count(TCLogger[TestCaseResult="Inconclusive"])'/>
									<b>
										<xsl:value-of select='format-number((100 * $FailVal div $TotalVal),"0.#")' /> %
									</b>
								</td>
								<td colspan='1' align='Center'>
									<xsl:variable name="OthVal" select='count(TCLogger[TestCaseResult="Other"])'/>
									<xsl:variable name="TotalVal" select='count(TCLogger[TestCaseResult="Passed"]) + count(TCLogger[TestCaseResult="Failed"]) + count(TCLogger[TestCaseResult="Other"]) + count(TCLogger[TestCaseResult="Inconclusive"])'/>
									<b>
										<xsl:value-of select='format-number((100 * $OthVal div $TotalVal),"0.#")'/> %
									</b>
								</td>
								<td colspan='1' align='Center'>
									<xsl:variable name="IncVal" select='count(TCLogger[TestCaseResult="Inconclusive"])'/>
									<xsl:variable name="TotalVal" select='count(TCLogger[TestCaseResult="Passed"]) + count(TCLogger[TestCaseResult="Failed"]) + count(TCLogger[TestCaseResult="Other"]) + count(TCLogger[TestCaseResult="Inconclusive"])'/>
									<b>
										<xsl:value-of select='format-number((100 * $IncVal div $TotalVal),"0.#")'/> %
									</b>
								</td>
							</tr>
						</table>
					</div>
					<!--Displaying a PieChart for the Pass/Fail Result-->
					<div class="cleft">
						<div id="piechart_3d" style="width: 400px; height: 300px;"></div>
					</div>
					<div class="cCenter">
						<!--Below Table display the executed Module with corresponding test cases along with Result and Execution time in tabular format -->
						<table>
							<tr  bgcolor="#9acd32">
								<th>Sr.no.</th>
								<th>Module</th>
								<th>Test Method</th>
								<th>Test Result</th>
								<th>Execution Time</th>
							</tr>
							<xsl:for-each select="TCLogger">
								<tr>
									<td  align="Center">
										<xsl:value-of select="1 + count(preceding-sibling::TCLogger)" />
									</td>
									<td  align="center">
										<xsl:value-of select="SuiteName" />
									</td>
									<td>
										<a href="{concat(LogPath,'')}" target='_blank'>
											<b>
												<xsl:choose>
													<xsl:when test='TestCaseResult="Failed"'>
														<font color="#FF0000">
															<xsl:value-of select="TestMethodName" />
														</font>
													</xsl:when>
													<xsl:when test='TestCaseResult="Inconclusive"'>
														<font color="#996633">
															<xsl:value-of select="TestMethodName" />
														</font>
													</xsl:when>
													<xsl:otherwise>
														<xsl:value-of select="TestMethodName" />
													</xsl:otherwise>
												</xsl:choose>
											</b>
										</a>
									</td>
									<xsl:if test='TestCaseResult="Passed"'>
										<td>
											<b>
												<font color="#006400">
													<xsl:value-of select="TestCaseResult" />
												</font>
											</b>
										</td>
									</xsl:if>
									<xsl:if test='TestCaseResult="Failed"'>
										<td>
											<b>
												<font color="#FF0000">
													<xsl:value-of select="TestCaseResult" />
												</font>
											</b>
										</td>
									</xsl:if>
									<xsl:if test='TestCaseResult="Other"'>
										<td>
											<b>
												<font color="#FFD700">
													<xsl:value-of select="TestCaseResult"/>
												</font>
											</b>
										</td>
									</xsl:if>
									<xsl:if test='TestCaseResult="Inconclusive"'>
										<td>
											<b>
												<font color="#996633">
													<xsl:value-of select="TestCaseResult" />
												</font>
											</b>
										</td>
									</xsl:if>

									<td>
										<xsl:value-of select="floor(ExecutionTime div 60)" />m<xsl:value-of select="ExecutionTime mod 60" />s
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</div>
					<script type="text/javascript" src="https://www.google.com/jsapi">
						<xsl:text>document.write("hello")</xsl:text>
					</script>
					<!--Using Google Charts to Display the PieChart-->
					<script type="text/javascript">
						google.load("visualization", "1", {packages:["corechart"]});
						google.setOnLoadCallback(drawChart);
						function drawChart() {

						var pass=document.getElementById('Pass').innerText;
						var fail=document.getElementById('Fail').innerText;
						var other=document.getElementById('Oth').innerText;
						var Inc=document.getElementById('Inc').innerText;
						var data = new google.visualization.DataTable();
						data.addColumn('string', 'Status');
						data.addColumn('number', 'Count');
						data.addRows([
						['Passed', parseInt(pass)],
						['Failed', parseInt(fail)],
						['Other', parseInt(other)],
						['Inconclusive', parseInt(Inc)]
						]);

						var options = {
						chartArea:{left:10,top:20, width:'95%',height:'80%'},
						legend:{position: 'bottom'},
						is3D: true,
						colors: ['#006400', '#FF0000', '#FFD700', '#996633'],
						pieSliceTextStyle: {color: 'black', bold: true}
						};

						var chart = new google.visualization.PieChart(document.getElementById('piechart_3d'));
						chart.draw(data, options);
						}

						function showHideDiv(divId)
						{
						var divstyle;divstyle = document.getElementById(divId).style.display;
						if(divstyle.toLowerCase()=='none')
						{document.getElementById(divId).style.display = 'block';}
						else{document.getElementById(divId).style.display = 'none';}
						}
					</script>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>