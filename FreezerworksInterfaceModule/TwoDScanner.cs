// Author Lalitha Viswanathan
// FIM Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;

namespace FreezerworksInterfaceModule {
	class TwoDScanner {

		private NameValueCollection appSettings = ConfigurationManager.AppSettings;

		private static TcpClient tcpClient = new TcpClient();
		private static NetworkStream clientSockStream { get; set; } = null;
		private StreamWriter streamWriter { get; set; } = null;
		private StreamReader streamReader { get; set; } = null;

		private string acknowledgeText = "OK";
		private string changeProduct = "P";
		private string getCurrentProduct = "C";
		private string getDecodeTime = "K";
		private string getExpectedNumberOfTubes = "A";
		private string getHardwareStatus = "Q";
		private string getStringStatus = "J";
		private string getRackOrientation = "O";
		private string getNumberOfNoReads = "G";
		private string getNumberOfTubes = "H";
		private string getNumberOfReads = "F";
		private string getRackID = "B";
		private string getRackPresentStatus = "R";
		private string getScanData = "D";
		private string getScannerStatus = "L";
		private string setExpectedNumberOfTubes = "A";
		private string setRackID = "I";
		private string startScanflag = "S";
		private string enabledExpectedTubes = "N";
		private string reset = "Z";
		private string noTubeText = "No Tube";
		private string noReadText = "No Read";

		private string previousResult = String.Empty;
		//private bool ignorePreviousResultOnce = false;


		public TwoDScanner() {

		}

		/// <summary>
		/// connect to the server at given port
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		internal bool Connect(string hostname, int port) {

			try {
				Debug.WriteLine("Before connecting, status of TCP Connection:" + tcpClient.Connected);

				bool isListening = false;
				if (!tcpClient.Connected) {
					List<IPEndPoint> listenersList = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().ToList();
					foreach (IPEndPoint listener in listenersList) {
						if (listener.Port.Equals(port)) {
							isListening = true;
							break;
						}
					}
					if (!isListening) {
						return false;
					}

					tcpClient = new TcpClient(hostname, port);
					//get a network stream from server   
					clientSockStream = tcpClient.GetStream();
					streamWriter = new StreamWriter(clientSockStream, System.Text.Encoding.ASCII);
					streamWriter.AutoFlush = true;
					streamWriter.NewLine = "\r";
					streamReader = new StreamReader(clientSockStream, System.Text.Encoding.ASCII);
					Debug.WriteLine("Connected to VisionMate server");
					return true;
				}

			} catch (Exception e) {
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
			}

			return false;
		}

		/// <summary>
		/// Get the status of the scanner
		/// </summary>
		/// <returns></returns>
		internal ScannerStatus GetStatus() {
			try {

				Debug.WriteLine("Getting scanner status");
				string response = communicateWithServer(getScannerStatus);
				Debug.WriteLine("Response from GetStatus: " + response);
				BitArray scannerStatusArray = new BitArray(System.BitConverter.GetBytes(Int32.Parse(response, CultureInfo.InvariantCulture)));
				ScannerStatus status = new ScannerStatus(scannerStatusArray);
				return status;
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
				throw e;
			}
		}

		/// <summary>
		/// Function to write BioServ to server (logs of bioserv commands)
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		private string communicateWithServer(string command) {
			try {
				writeToServer(command);
				return readFromServer();
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
				throw e;
			}

		}

		/// <summary>
		/// read from server
		/// </summary>
		/// <returns></returns>
		private string readFromServer() {
			try {
				StringBuilder response = new StringBuilder();
				streamReader = new StreamReader(clientSockStream, System.Text.Encoding.ASCII);

				while (streamReader.Peek() != -1) {
					string chunk = Char.ConvertFromUtf32(streamReader.Read());
					response.Append(chunk);
				}

				Debug.WriteLine("Response from server " + response);
				response.Remove(0, acknowledgeText.Length);
				return response.ToString();
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
				throw e;
			}
		}

		/// <summary>
		/// Write command to BioServ server
		/// </summary>
		/// <param name="command"></param>
		private void writeToServer(string command) {
			Debug.WriteLine("Command to write to server is " + command);
			try {
				// send command to visionmate
				streamWriter.WriteLine(command);
				streamWriter.Flush();
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
				throw e;
			}
		}


		/// <summary>
		/// issue start scan command to BioServ
		/// </summary>
		internal void issueStartScancommandToBioServ() {
			Debug.WriteLine("Issuing Start Scan command");
			communicateWithServer(startScanflag);
		}

		/// <summary>
		/// get BioServ Product Version 
		/// </summary>
		/// <returns></returns>
		internal string getCurrentProductVersionFromBioServ() {
			return communicateWithServer(getCurrentProduct);
		}

		/// <summary>
		/// get Number of aliquots read - 2D (aliquots on rack) from BioServ and number of tubes scanned (1d) 
		/// </summary>
		/// <returns></returns>
		internal string getNumberOfReadsFromBioServ()
        {
			return communicateWithServer(getNumberOfReads); 
        }
		internal string getRackIDFromBioServ() {
			return communicateWithServer(getRackID);
		}

		/// <summary>
		/// Get No of aliquots not read by BioServ
		/// </summary>
		/// <returns></returns>
		internal int getNumberOfNoReadsFromBioServ() {
			string reads = communicateWithServer(getNumberOfNoReads);
			return Int32.Parse(reads);
		}

		internal int getNumberOfTubesOnRackFromBioServ() {
			string tubes = communicateWithServer(getNumberOfTubes);
			return Int32.Parse(tubes);
		}

		/// <summary>
		/// Get no of 2d scans
		/// </summary>
		/// <returns></returns>
		internal int GetNumberOfAliquotsScanned() {
			string reads = communicateWithServer(getNumberOfReads);
			return Int32.Parse(reads);
		}

		/// <summary>
		/// Get time for run (decode time for barcodes) 
		/// </summary>
		/// <returns></returns>
		internal string getDecodeTimeFromBioServ() {
			return communicateWithServer(getDecodeTime);
		}

		/// <summary>
		/// Get barcode data (1d and 2d barcodes scanned) in run
		/// </summary>
		/// <returns></returns>
		internal Dictionary<string, string> getBarcodeData() {
			Dictionary<String, String> scanData = new Dictionary<String, String>();
			Debug.WriteLine("Get scan data");

			string responseFromServer = communicateWithServer(getScanData);
	
			string[] responses = Regex.Split(responseFromServer, @"\,?([A-Z]\d\d)\,(No Tube|\d+)\,?");
			int i = 1;

			while (i < responses.Length) {
				if (!(responses[i + 1].Trim().StartsWith(noTubeText, StringComparison.InvariantCultureIgnoreCase))) {
					scanData.Add(responses[i].Trim(), responses[i + 1].Trim());
				}
				i += 3;
			}
			return scanData;
		}

		/// <summary>
		/// Setter for number of tubes / 1d scans
		/// </summary>
		/// <param name="numberOfTubes"></param>
		internal void setNumberOfTubes(string numberOfTubes) {
			communicateWithServer(setExpectedNumberOfTubes + numberOfTubes);
		}

		/// <summary>
		/// Set rack Id on BioServ
		/// </summary>
		/// <param name="id"></param>
		internal void setRackIDonBioServ(string id) {
			communicateWithServer(setRackID + id);
		}

		/// <summary>
		/// Determines if a rack is present.
		/// Will always return true after a Start Scan has been issued and the
		/// scan is not complete.
		/// </summary>
		/// <returns></returns>
		internal bool checkIfRackPresent() {
			string response = communicateWithServer(getRackPresentStatus);
			if (response == "True") {
				Debug.WriteLine("Rack Found");
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Do not use, server support isn't good
		/// </summary>
		/// <param name="rackType"></param>
		internal void setRackTypeOnBioServ(string rackType) {
			writeToServer(changeProduct + rackType);
		}

		/// <summary>
		/// Do not use, server support isn't good
		/// </summary>
		internal void enableNumberOfExpectedTubesOnBioServ() {
			Debug.WriteLine("Enable Expected Tubes");
			communicateWithServer(enabledExpectedTubes);
		}

		/// <summary>
		/// Do not use, server support isn't good
		/// </summary>
		/// <returns></returns>
		internal string getHardwareStatusFromBioServ() {
			return communicateWithServer(getHardwareStatus);
		}
	}
}
