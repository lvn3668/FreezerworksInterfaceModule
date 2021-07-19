// Author Lalitha Viswanathan
// FIM V2.0 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GenericParsing;
using System.Diagnostics;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using System.Collections;
using System.Xml;

namespace FreezerworksInterfaceModule {
	/// <summary>
	/// This is an abstract interface so that the UI can pretend it is talking to a real
	/// database.  This class will be swapped out for a class that talks to a real database
	/// eventually
	/// </summary>
	class MavericDatabaseInterface {

		/// <summary>
		/// 
		/// </summary>
		private DataTable importTable { get; set; } = null;
		private TextWriter scannedDataWriter { get; set; } = null;
		private NameValueCollection appSettings { get; set; } = ConfigurationManager.AppSettings;
		string fileName { get; set; }  = String.Empty;
		string PE1fileName { get; set;  } = String.Empty;
		string PE2fileName { get; set; }  = String.Empty;
		string BUF1fileName { get; set; } = String.Empty;
		string BUF2fileName { get; set; }  = String.Empty;
		private ArrayList aliquotScans = new ArrayList();
		private Boolean splitBufBox
		{ get; set; } = true;


		/// <summary>
		/// Constructor for the database interface
		/// </summary>
		public MavericDatabaseInterface() {
			createCSVFiles();
		}

		/// <summary>
		/// Creates the stub of a CSV file with the header row
		/// </summary>
		internal void createCSVFiles() {
			if (!Directory.Exists(appSettings["ExportFolder"])) {
				Directory.CreateDirectory(appSettings["ExportFolder"]);
			}

			fileName = appSettings["ExportFolder"] + @"\" + System.Environment.MachineName + System.DateTime.Now.Date.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "") + "_" + System.DateTime.Now.TimeOfDay.ToString().Replace(".", "_").Replace(":", "_") + ".txt";
			// Open file and write header
			scannedDataWriter = new StreamWriter(fileName);
			if (appSettings["Include_CVSHeader"].Equals("true")) {
				scannedDataWriter.WriteLine(appSettings["Header_CVSFile"]);
			}
			scannedDataWriter.Flush();
			scannedDataWriter.Close();


			PE1fileName = appSettings["ExportFolder"] + @"\PE1" + System.Environment.MachineName + System.DateTime.Now.Date.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "") + "_" + System.DateTime.Now.TimeOfDay.ToString().Replace(".", "_").Replace(":", "_") + ".txt";
			// Open file and write header
			scannedDataWriter = new StreamWriter(PE1fileName);
			if (appSettings["Include_CVSHeader"].Equals("true")) {
				scannedDataWriter.WriteLine(appSettings["Header_CVSFile"]);
			}
			scannedDataWriter.Flush();
			scannedDataWriter.Close();

			PE2fileName = appSettings["ExportFolder"] + @"\PE2" + System.Environment.MachineName + System.DateTime.Now.Date.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "") + "_" + System.DateTime.Now.TimeOfDay.ToString().Replace(".", "_").Replace(":", "_") + ".txt";
			// Open file and write header
			scannedDataWriter = new StreamWriter(PE2fileName);
			if (appSettings["Include_CVSHeader"].Equals("true")) {
				scannedDataWriter.WriteLine(appSettings["Header_CVSFile"]);
			}
			scannedDataWriter.Flush();
			scannedDataWriter.Close();


			BUF1fileName = appSettings["ExportFolder"] + @"\BUF1" + System.Environment.MachineName + System.DateTime.Now.Date.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "") + "_" + System.DateTime.Now.TimeOfDay.ToString().Replace(".", "_").Replace(":", "_") + ".txt";
			// Open file and write header
			scannedDataWriter = new StreamWriter(BUF1fileName);
			if (appSettings["Include_CVSHeader"].Equals("true")) {
				scannedDataWriter.WriteLine(appSettings["Header_CVSFile"]);
			}
			scannedDataWriter.Flush();
			scannedDataWriter.Close();


			BUF2fileName = appSettings["ExportFolder"] + @"\BUF2" + System.Environment.MachineName + System.DateTime.Now.Date.ToString().Replace(" ", "").Replace("/", "_").Replace(":", "") + "_" + System.DateTime.Now.TimeOfDay.ToString().Replace(".", "_").Replace(":", "_") + ".txt";
			// Open file and write header
			scannedDataWriter = new StreamWriter(BUF2fileName);
			if (appSettings["Include_CVSHeader"].Equals("true")) {
				scannedDataWriter.WriteLine(appSettings["Header_CVSFile"]);
				scannedDataWriter.Flush();
				scannedDataWriter.Close();
			}


		}

		/// <summary>
		/// The number of aliquots that have been imported, but not yet
		/// in a logged rack
		/// </summary>
		public int totalNumberOfAliquotsleft {
			get {
				if (importTable != null) {
					return importTable.Rows.Count;
				} else {
					return 0;
				}
			}
		}

		/// <summary>
		/// Adds a CSV file to the internal database/table
		/// </summary>
		/// <param name="csvfile">A LFHM CSV file</param>
		internal void AddCSVFile(string csvfile) {
			using (GenericParserAdapter parser = new GenericParserAdapter(csvfile)) {
				parser.ColumnDelimiter = ',';
				parser.FirstRowHasHeader = true;
				if (importTable == null) {
					importTable = parser.GetDataTable();
					importTable.TableName = "Maveric";
					DataColumn[] colums = new DataColumn[1];
					colums[0] = importTable.Columns["AliquotID"];
					importTable.PrimaryKey = colums;
				} else {
					importTable.Merge(parser.GetDataTable());
				}
				printValues("Merged new file");
			}

		}

		/// <summary>
		/// A debugging method for insuring the CSV files made it into the data table
		/// </summary>
		/// <param name="label">A label to display in the debug window</param>
		private void printValues(string label) {
			// Display the values in the DataTable:
			Debug.WriteLine(label);
			foreach (DataRow row in importTable.Rows) {
				foreach (DataColumn col in importTable.Columns) {
					Debug.Write("," + row[col].ToString());
				}
				Debug.WriteLine("");
			}
		}

		/// <summary>
		/// Writes out a rack scan to disk and removes
		/// them from the import table.  The name is slightly deceptive
		/// since in the future it will update the aliquot row in the real db.
		/// </summary>
		/// <param name="twoDscan">The rack scan to add</param>
		/// <returns>True if successful</returns>
		internal bool updateAliquots(TwoDScan twoDscan, string rackID) {
			// We should have a rack with say 96 aliquots
			// Find each aliquot in our master table and write out data to disk

			if (rackID.Contains("PE1")) {
				scannedDataWriter = new StreamWriter(PE1fileName, true);
			} else if (rackID.Contains("PE2")) {
				scannedDataWriter = new StreamWriter(PE2fileName, true);
			} else if (rackID.Contains("BUF")) {
				if (splitBufBox) {
					splitBufBox = false;
					scannedDataWriter = new StreamWriter(BUF1fileName, true);
				} else {
					splitBufBox = true;
					scannedDataWriter = new StreamWriter(BUF2fileName, true);
				}
			} else {
				scannedDataWriter = new StreamWriter(fileName, true);
			}


			foreach (KeyValuePair<string, string> kv in twoDscan.Barcodes) {
				string sql = String.Format("AliquotID={0}", kv.Value);
				DataRow[] rows = importTable.Select(sql);
				writeAliquotToFile(rows[0], kv.Key, rackID);
				aliquotScans.Add(kv.Value);
				importTable.Rows.Remove(rows[0]);

			}
			scannedDataWriter.Flush();
			scannedDataWriter.Close();
			return true;
		}

		/// <summary>
		/// Writes an aliquot row to disk
		/// </summary>
		/// <param name="dataRow">The row from the data table of the aliquot</param>
		/// <param name="location">The location in the rack</param>
		/// <param name="rackID">The ID of the rack the aliquot is in</param>
		private void writeAliquotToFile(DataRow dataRow, string location, string rackID) {
			string row = location.Substring(0, 1);
			string column = location.Substring(1, location.Length - 1);

			String record = dataRow["SampleDate"].ToString()			// Sample Date
						+ "," + dataRow["BankID"].ToString()				// Bank ID
						+ "," + dataRow["StudyID"].ToString()				// StudyID
						+ "," + dataRow["AliquotType"].ToString()			// Aliquot Type
						+ "," + dataRow["AliquotID"].ToString()				// AliquotID
						+ "," + dataRow["InitialAmount"].ToString()		// Initial Amount
						+ "," + dataRow["CurrentAmount"].ToString()		// Current Amount
						+ "," + dataRow["NumberOfThaws"].ToString()	// Number of Thaws
						+ "," + dataRow["LFHMDate"].ToString()			// LFHM Date
						+ "," + dataRow["LFHMTime"].ToString()			// LFHM Time
						+ "," + dataRow["LFHMOperatorID"].ToString()	// LFHM operator
						+ "," + dataRow["LFHMHostname"].ToString()	// LFHMHostname
						+ "," + rackID												// Box ID
						+ "," + row													// Aliquot location row
						+ "," + column												// Aliquot location column
						+ "," + System.DateTime.Now.ToString("MM/dd/yyyy") // FIM Date
						+ "," + System.DateTime.Now.ToString("t")		// FIM Time
						+ "," + System.Environment.UserName				// FIM operator
						+ "," + System.Environment.MachineName;			// FIM Hostname

			scannedDataWriter.WriteLine(record);

		}

		/// <summary>
		/// Checks to see if the aliquots in a 2D scan are in our import table
		/// </summary>
		/// <param name="twoDscan">A 2D Scan</param>
		/// <returns>True if all are accounted for</returns>
		internal bool AliquotsExist(TwoDScan twoDscan) {
			foreach (KeyValuePair<string, string> kv in twoDscan.Barcodes) {
				string sql = String.Format("AliquotID={0}", kv.Value);
				DataRow[] rows = importTable.Select(sql);
				if (rows.Length < 1) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Provides the rack location of an aliquot found in a 2D scan, that can't
		/// be found in the import table
		/// </summary>
		/// <param name="twoDscan">A 2D Scan</param>
		/// <returns>Rack location coordinates</returns>
		internal string unexpectedAliquotLocation(TwoDScan twoDscan) {
			foreach (KeyValuePair<string, string> kv in twoDscan.Barcodes) {
				string sql = String.Format("AliquotID='{0}'", kv.Value);
				DataRow[] rows = importTable.Select(sql);
				if (rows.Length < 1) {
					return kv.Key;
				}
			}
			return String.Empty;
		}

		/// <summary>
		/// Checks if an aliquot has already been seen in a 2D Scan this session
		/// </summary>
		/// <param name="twoDscan"></param>
		/// <returns>True if a duplicate is found</returns>
		internal bool checkduplicatealiquots(TwoDScan twoDscan) {
			foreach (KeyValuePair<string, string> kv in twoDscan.Barcodes) {
				if (aliquotScans.Contains(kv.Value)) {
					return true;
				}
			}
			return false;

		}

		/// <summary>
		/// Returns the rack location of the duplicate aliquot
		/// </summary>
		/// <param name="twoDscan"></param>
		/// <returns>Rack location coordinate</returns>
		internal string checkfoduplicateAliquotLocation(TwoDScan twoDscan) {
			foreach (KeyValuePair<string, string> kv in twoDscan.Barcodes) {
				if (aliquotScans.Contains(kv.Value)) {
					//add_error("duplicate aliquot ID", kv.Value);
					return kv.Key;
				}
			}
			return String.Empty;
		}

		/// <summary>
		/// Saves the import table to disk for reading back in at a later time
		/// </summary>
		internal void saveimporttabletodisk() {
			if (importTable != null) {
				if (!Directory.Exists(appSettings["StateFolder"])) {
					Directory.CreateDirectory(appSettings["StateFolder"]);
				}
				string path = Path.Combine(appSettings["StateFolder"], appSettings["StateFile"]);
				if (File.Exists(path)) {
					File.Delete(path);
				}

				try {
					TextWriter tWriter = new StreamWriter(path);
					importTable.WriteXml(tWriter, XmlWriteMode.WriteSchema);
					tWriter.Flush();
					tWriter.Close();
				} catch (Exception e) {
					Debug.WriteLine(e.Message);
				}
			}
		}

		/// <summary>
		/// Reads in the import table from disk and deletes the file
		/// </summary>
		internal void readtablefromdisk() {
			if (!Directory.Exists(appSettings["StateFolder"])) {
				return;
			}
			string path = Path.Combine(appSettings["StateFolder"], appSettings["StateFile"]);
			if (!File.Exists(path)) {
				return;
			} else {
				importTable = new DataTable();
				importTable.TableName = "Maveric";
				TextReader txtReader = new StreamReader(path);
				XmlReader reader = XmlReader.Create(txtReader);
				try {
					importTable.ReadXml(reader);
				} catch (Exception e) {
					Debug.WriteLine(e.Message);
					Debug.WriteLine(e.StackTrace);
					importTable = null;
				}

				reader.Close();
				txtReader.Close();
				File.Delete(path);
			}
		}
	}
}
