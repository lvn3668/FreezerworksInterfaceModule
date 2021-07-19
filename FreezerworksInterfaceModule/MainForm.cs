using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

namespace FreezerworksInterfaceModule {
	public partial class MainForm : Form {

		private NameValueCollection appSettings = ConfigurationManager.AppSettings;
		private Color okColor = Color.Green;
		private Color errorColor = Color.Red;
		private Color black = Color.Black;
		private string rackID = String.Empty;
		private ArrayList scannedRacks = new ArrayList();
		private TwoDScanner scanner;
		private TwoDScan twoDscan;
		private MavericDatabaseInterface mdb;
		private int startingAliquotCount;
		private string unexpectedAliquotLocationStr = String.Empty;
		private TextWriter error_log;

		//private System.Drawing.Font boldLabel = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
		//private System.Drawing.Font regularLobel = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));

		/// <summary>
		/// Constructor for the main form
		/// </summary>
		public MainForm() {
			InitializeComponent();

			TextWriterTraceListener tr2 = new TextWriterTraceListener(System.IO.File.CreateText("Debug.txt"));
			Debug.Listeners.Add(tr2);

			if (File.Exists(appSettings["ErrorLog"])) {
				error_log = new StreamWriter(appSettings["ErrorLog"], true);
			} else {
				error_log = new StreamWriter(appSettings["ErrorLog"], true);
				error_log.WriteLine(appSettings["ErrorLogHeader"]);
				error_log.Flush();
			}

			lbl_Status.Text = String.Empty;


			// instantiate a new 2D scanner
			scanner = new TwoDScanner();

			// connect to the scanner server
			bool connected = scanner.Connect(appSettings["2dScannerHostname"], Int32.Parse(appSettings["2dScannerPort"]));
			if (!connected) {
				btn_Import.Visible = false;
				btn_ProcessBacklog.Visible = false;
				MessageBox.Show("Visionmate Server is not running!");
			}

			// create new database interface
			mdb = new MavericDatabaseInterface();

			// since we're using CSV files for now, import state file
			mdb.readtablefromdisk();

			startingAliquotCount = mdb.totalNumberOfAliquotsleft;

			if (startingAliquotCount > 0) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = okColor;
				lbl_Status.Text = String.Format("Loaded {0} aliquot records from backlog", startingAliquotCount);
			}



			lbl_Instructions.Text = "Please do one of the following:\n";
			lbl_Instructions.Text += "A.  Press Process Backlog to only process backlogged data\n";
			lbl_Instructions.Text += "B.  Insert thumb drive with data files and press Import to \n     process new data and any backlogs\n";
			lbl_Instructions.Text += "C. Press Export to export the current processed data to a \n     thumb drive\n";
			lbl_Instructions.Text += "D. Press Exit to exit the tool\n";
		}

		#region Events

		/// <summary>
		/// Exits the application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Exit_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		/// <summary>
		/// Event handler for Process Backlog button
		/// Starts the rack scanning process
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ProcessBacklog_Click(object sender, EventArgs e) {
			if (mdb.totalNumberOfAliquotsleft > 0) {
				// set gui to rack scan state
				lbl_Instructions.ForeColor = black;
				//lbl_Instructions.Font = boldLabel;
				lbl_Instructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
				lbl_Instructions.Text = "Ready to scan next aliquot rack ID...";
				btn_ProcessBacklog.Visible = false;
				lbl_Status.Visible = false;
				txt_RackID.Visible = true;
				txt_RackID.Focus();
			} else {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = "No backlogged data found";
			}
		}

		/// <summary>
		/// When the Export button is clicked, the contents of the Export folder are
		/// copied to a special folder on another drive, most likely a thumb drive.
		/// They are then copied to a backup folder in the application space, and deleted
		/// from the current export folder.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Export_Click(object sender, EventArgs e) {
			// Write contents of CVS file to thumb drive
			try {
				string[] cvsList = Directory.GetFiles(appSettings["ExportFolder"]);
				if (cvsList.Length < 1) {
					lbl_Status.Visible = true;
					lbl_Status.ForeColor = errorColor;
					lbl_Status.Text = "No data to export";
					return;
				}

				string destPath = Path.Combine(appSettings["ExportDrive"], appSettings["ExportFolder"]);

				// Check for thumb drive
				if (!Directory.Exists(appSettings["ExportDrive"])) {
					lbl_Status.Visible = true;
					lbl_Status.ForeColor = errorColor;
					lbl_Status.Text = String.Format("Export folder not found at {0}\\", appSettings["ExportDrive"]);
					return;

				}

				// Check to see if destination path exists, create the folder if it doesn't.
				if (!Directory.Exists(destPath)) {
					try {
						Directory.CreateDirectory(destPath);
					} catch (IOException error) {
						Debug.WriteLine(error.Message);
						lbl_Status.Visible = true;
						lbl_Status.ForeColor = errorColor;
						lbl_Status.Text = String.Format("Could not create export folder");
						return;
					}
				}

				// Check for backup folder, create one if missing
				if (!Directory.Exists(appSettings["BackupFolder"])) {
					try {
						Directory.CreateDirectory(appSettings["BackupFolder"]);
					} catch (IOException error) {
						Debug.WriteLine(error.Message);
						lbl_Status.Visible = true;
						lbl_Status.ForeColor = errorColor;
						lbl_Status.Text = "Could not create backup folder";
						return;
					}

				}

				// move each file to thumbdrive and backup folder
				// then delete
				foreach (string f in cvsList) {
					string fName = f.Substring(appSettings["ExportFolder"].Length + 1);
					File.Copy(f, Path.Combine(destPath, fName), true);
					File.Copy(f, appSettings["BackupFolder"] + "\\" + fName, true);
					File.Delete(f);
				}

				lbl_Status.Visible = true;
				lbl_Status.ForeColor = okColor;
				lbl_Status.Text = "Data exported successfully.";

			} catch (IOException copyError) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = copyError.Message;
			}

		}

		/// <summary>
		/// Starts the process to import CSV files from a thumbdrive
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_Import_Click(object sender, EventArgs e) {
			string importPath = Path.Combine(appSettings["ImportDrive"], appSettings["ImportFolder"]);

			// check for thumb drive
			if (!Directory.Exists(appSettings["ImportDrive"])) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = String.Format("Import folder not found at {0}\\", appSettings["ImportDrive"]);
				return;
			}

			// check for data folder on the thumbdrive
			if (!Directory.Exists(importPath) && mdb.totalNumberOfAliquotsleft < 1) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = String.Format("Import folder not found at {0}\\", appSettings["ImportDrive"]);
				return;
			}

			string[] csvList = Directory.GetFiles(importPath);

			// check for data files in data folder
			if (csvList.Length < 1 && mdb.totalNumberOfAliquotsleft < 1) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = "No backlogged or new data found";
				return;
			}

			if (txt_RackID.Visible && csvList.Length < 1) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = "No backlogged or new data found";
				return;
			}

			// Check for backup folder, create one if missing
			string processedPath = Path.Combine(appSettings["ImportDrive"], appSettings["ProccessedFolder"]);
			if (!Directory.Exists(processedPath)) {
				try {
					Directory.CreateDirectory(processedPath);
				} catch (IOException error) {
					Debug.WriteLine(error.Message);
					lbl_Status.Visible = true;
					lbl_Status.ForeColor = errorColor;
					lbl_Status.Text = "Could not create backup folder";
					return;
				}

			}

			lbl_Status.Visible = true;
			lbl_Status.ForeColor = okColor;
			lbl_Status.Text = "Reading input files...";

			// read in each file, add it to database
			// backup and delete each file from data folder
			try {
				foreach (string file in csvList) {
					mdb.AddCSVFile(file);
					string fName = file.Substring(importPath.Length + 1);
					File.Copy(file, Path.Combine(processedPath, fName), true);
					File.Delete(file);
				}
			} catch (Exception error) {
				Debug.WriteLine(error.Message);
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = "Error reading input files";
				return;
			}

			// follwing an import, check the total number of aliquots we're waiting to scan in
			startingAliquotCount = mdb.totalNumberOfAliquotsleft;

			// set gui to rack scan state
			lbl_Instructions.ForeColor = black;
			lbl_Instructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			//lbl_Instructions.Font = boldLabel;
			lbl_Instructions.Text = "Ready to scan next aliquot rack ID...";
			btn_ProcessBacklog.Visible = false;
			if (txt_RackID.Visible) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = okColor;
				lbl_Status.Text = "Data imported successfully";
			} else {
				lbl_Status.Visible = false;
			}
			txt_RackID.Text = String.Empty;
			txt_RackID.Enabled = true;
			txt_RackID.Visible = true;
			txt_RackID.Focus();

		}

		/// <summary>
		/// Catches either upper right X close, or the exit button and saves state
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			string msg = "Exit Freezerworks Interface Module?";
			if (mdb.totalNumberOfAliquotsleft > 0) {
				lbl_Status.Visible = true;
				lbl_Status.Text = String.Format("Remaining {0} aliquot records will remain in backlog", mdb.totalNumberOfAliquotsleft.ToString());
				lbl_Instructions.Visible = false;
			}

			var result = MessageBox.Show(msg, "Close Application",
															MessageBoxButtons.YesNo,
															MessageBoxIcon.Question);
			if (result == DialogResult.No) {
				e.Cancel = true;
			} else {
				// save state to disk
				mdb.saveimporttabletodisk();

				// close error log
				error_log.Close();
				error_log.Dispose();

			}
		}

		/// <summary>
		/// Waits for the EOL sequence from the 1D Scanner and evaluates the input,
		/// kicking off the 2D scan if the rackID is valid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txt_RackID_KeyDown(object sender, KeyEventArgs e) {
			//if (e.Control && e.KeyCode == Keys.M) {
			if (e.KeyCode == Keys.Enter) {
				if (validateRackID(txt_RackID.Text)) {
					rackID = txt_RackID.Text;
					scannedRacks.Add(rackID);

					// set gui to aliquot scan state
					lbl_Status.ForeColor = okColor;
					lbl_Status.Text = tubesProcessedMsg();
					lbl_Status.Visible = true;
					lbl_Instructions.ForeColor = black;
					lbl_Instructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
					//lbl_Instructions.Font = boldLabel;
					lbl_Instructions.Text = "Ready to scan aliquot rack...";
					txt_RackID.Enabled = false;

					start2Dscan();

				}
			}
		}

		#endregion


		#region Background worker

		/// <summary>
		/// Asynchornous thread for polling the 2d scanner while allowing the UI to function
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TwoDScannerBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			BackgroundWorker worker = sender as BackgroundWorker;
			try {

				worker.ReportProgress(0, appSettings["PrepareScan"]);

				// Set a unique rack ID
				scanner.setRackIDonBioServ(System.DateTime.Now.ToString().Replace('/', '1').Replace(':', '1').Replace(' ', '1'));
				// Sets the expected number of tubes, more efficient scanning
				scanner.setNumberOfTubes(appSettings["ExpectedTubes"]);

				while (!scanner.checkIfRackPresent()) {
					Debug.WriteLine("Rack is not present");
					worker.ReportProgress(-1, appSettings["RackNotPresent"]);
					Thread.Sleep(2000);
				}

				scanner.issueStartScancommandToBioServ();
				// Poor scanner needs some time to reset itself
				// this seems to be the key to getting accurate scans.
				Thread.Sleep(Int32.Parse(appSettings["ScannerSleep"]));

				worker.ReportProgress(1, appSettings["Scanning"]);

				ScannerStatus status = scanner.GetStatus();

				while (!status.FinishedScan) {
					while (!scanner.checkIfRackPresent()) {
						Debug.WriteLine("Rack is not present");
						worker.ReportProgress(-1, appSettings["RackNotPresent"]);
						Thread.Sleep(2000);
					}
		
					status = scanner.GetStatus();
					worker.ReportProgress(2, appSettings["Scanning"]);
					Debug.WriteLine("Scanning.");
					Thread.Sleep(500);
				}

				worker.ReportProgress(3, appSettings["WaitingForResults"]);
				// scan has finished
				twoDscan = new TwoDScan(scanner);


			} catch (Exception error) {
				Debug.WriteLine(error.Message);
				Debug.WriteLine(error.StackTrace);
				worker.ReportProgress(0, appSettings["FatalError"]);
			}

		}

		/// <summary>
		/// Runs when the polling is complete for a specific rack
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TwoDScannerBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			Debug.WriteLine("TwoDScannerBackgroundWorker_RunWorkerCompleted");

			// check for duplicate aliquots in recently completed scan
			if (duplicateAliquots(twoDscan)) {
				string loc = duplicateAliquotLocation(twoDscan);
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = String.Format("The aliquot tube ID at {0} has already been scanned", loc);
				add_error("duplicate aliquot ID", loc);
				// User will need to fix the rack, we can afford to sleep this much
				Thread.Sleep(2000);
				start2Dscan();
			} else if (!validate2dScan(twoDscan)) {
				// check to see if there's an unexpected aliquot
				if (unexpectedAliquotLocationStr != String.Empty) {
					lbl_Status.Visible = true;
					lbl_Status.ForeColor = errorColor;
					lbl_Status.Text = String.Format("The aliquot tube ID at {0} does not exist in the records", unexpectedAliquotLocationStr);
					add_error("unexpected aliquot", unexpectedAliquotLocationStr);
				}
				unexpectedAliquotLocationStr = String.Empty;
				// User will need to fix the rack, we can afford to sleep this much
				Thread.Sleep(2000);

				start2Dscan();

			} else {

				// Merge results into master data table

				mdb.updateAliquots(twoDscan, rackID);

				// TODO this should really be handled by the database.
				scannedRacks.Add(rackID);

				// Setup gui for new rack scan
				btn_Exit.Visible = true;
				btn_Export.Visible = true;
				btn_Import.Visible = true;
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = okColor;
				lbl_Status.Text = tubesProcessedMsg();
				if (mdb.totalNumberOfAliquotsleft.Equals(0)) {
					txt_RackID.Visible = false;
					lbl_Instructions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
					//lbl_Instructions.Font = regularLobel;
					lbl_Instructions.Text = "Please do one of the following:\n";
					lbl_Instructions.Text += "A. Insert thumb drive with data files and press Import to \n    process new data and any backlogs\n";
					lbl_Instructions.Text += "B. Press Export to export the current processed data to a \n    thumb drive\n";
					lbl_Instructions.Text += "C. Press Exit to exit the tool\n";
				} else {
					txt_RackID.Text = String.Empty;
					txt_RackID.Enabled = true;
					txt_RackID.Focus();
					//lbl_Instructions.Font = boldLabel;
					lbl_Instructions.Text = "Ready to scan next aliquot rack ID...";
				}
				lbl_Instructions.Visible = true;
			}
		}

		private void TwoDScannerBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (e.ProgressPercentage < 0) {
				lbl_Instructions.ForeColor = errorColor;
			} else {
				lbl_Instructions.ForeColor = black;
			}
			lbl_Instructions.Text = e.UserState.ToString();
		}


		#endregion

		/// <summary>
		/// Starts the 2D Scan thread
		/// </summary>
		private void start2Dscan() {
			if (!TwoDScannerBackgroundWorker.IsBusy) {
				btn_Exit.Visible = false;
				btn_Export.Visible = false;
				btn_Import.Visible = false;
				TwoDScannerBackgroundWorker.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Validates a 2D Scan resultset
		/// </summary>
		/// <param name="twoDscan">Scan to be validated</param>
		/// <returns>True if no errors found</returns>
		private bool validate2dScan(TwoDScan twoDscan) {
			unexpectedAliquotLocationStr = mdb.unexpectedAliquotLocation(twoDscan);
			if (unexpectedAliquotLocationStr != String.Empty) {
				return false;
			} else {
				return true;
			}
		}

		/// <summary>
		/// Calculates tubes processed
		/// </summary>
		/// <returns>Formatted string of tubes processed</returns>
		private string tubesProcessedMsg() {
			return String.Format("{0} of {1} aliquot tubes have been processed",
											startingAliquotCount - mdb.totalNumberOfAliquotsleft,
											startingAliquotCount);
		}

		/// <summary>
		/// Validates whether or not a rack ID is in the correct format
		/// </summary>
		/// <param name="candidate"></param>
		/// <returns>True if the rack ID is in the correct format</returns>
		private bool validateRackID(string candidate) {
			// 1D barcode should contain only 13 alpha-numerics
			Regex r = new Regex(@"^[A-z0-9]{13}$");
			if (!r.IsMatch(candidate)) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = "This aliquot rack ID is invalid";
				add_error("invalid box ID", "This aliquot rack ID is invalid");
				txt_RackID.Text = String.Empty;
				txt_RackID.Focus();
				return false;
			}

			if (scannedRacks.Contains(candidate)) {
				lbl_Status.Visible = true;
				lbl_Status.ForeColor = errorColor;
				lbl_Status.Text = "This aliquot rack ID has already been scanned";
				add_error("duplicate box ID", rackID);
				txt_RackID.Text = String.Empty;
				txt_RackID.Focus();
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns a duplicate aliquot location if one exists
		/// </summary>
		/// <param name="twoDscan"></param>
		/// <returns>The first duplicate location</returns>
		private string duplicateAliquotLocation(TwoDScan twoDscan) {
			return mdb.checkfoduplicateAliquotLocation(twoDscan);
		}

		/// <summary>
		/// Determines if a duplicate aliquot exists in a scan
		/// </summary>
		/// <param name="twoDscan"></param>
		/// <returns>True if at least one duplicate is found</returns>
		private bool duplicateAliquots(TwoDScan twoDscan) {
			return mdb.checkduplicatealiquots(twoDscan);
		}

		/// <summary>
		/// Writes an error message to the error file
		/// </summary>
		/// <param name="error_msg">Description of the error</param>
		/// <param name="details">Additional information about the error</param>
		private void add_error(string error_msg, string details) {
			string error = (rackID != null ? rackID : String.Empty)
				+ "," + System.DateTime.Now.ToString("MM/dd/yyyy")
				+ "," + System.DateTime.Now.ToString("t")
				+ "," + System.Environment.UserName
				+ "," + System.Environment.MachineName
				+ "," + error_msg
				+ "," + details;
			Debug.WriteLine(error);
			error_log.WriteLine(error);
			error_log.Flush();

			//BoxID,StudyID,Date,Time,OperatorID,Hostname,Error,Details
		}

		/// <summary>
		/// Writes the message to the error log.
		/// </summary>
		/// <param name="error_msg"></param>
		private void add_error(string error_msg) {
			add_error(error_msg, String.Empty);
		}

	}
}