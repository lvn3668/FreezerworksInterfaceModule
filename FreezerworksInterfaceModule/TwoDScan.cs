using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;


namespace FreezerworksInterfaceModule {
	internal class TwoDScan {
		private string currentProduct { get; set; } = null;
		private int numRowsOnRack { get; set; } = 0;
		private int numColsOnRack { get; set; } = 0;
		private int rackSize { get; set; } = 0;
		private int totalNumberOfTubesRead { get; set; } = 0;
		private int numberOfNoReads { get; set; } = 0; 
		private int tubesRead { get; set; } = 0;
		private string decodeTime { get; set; } = null;
		private bool rackOrientation { get; set; } = false;
		private bool rackPresent { get; set; } = false;
		private Dictionary<String, String> barcodes { get; set; } = new Dictionary<string, string>();
		
		// Rack size is not more than 12 x 8
		private int[,] rackMatrix = new int[20,20];
		private string rackId = null;
		private static NameValueCollection appSettings = ConfigurationManager.AppSettings;
		private TwoDScanner scanner = new TwoDScanner();

		public TwoDScan() {
			barcodes = new Dictionary<String, String>();
		}

		public TwoDScan(TwoDScanner scanner) {
			try {
				this.scanner = scanner;
				Debug.WriteLine("Get current product");
				currentProduct = scanner.getCurrentProductVersionFromBioServ();
				Debug.WriteLine("Get RackID");
				rackId = scanner.getRackIDFromBioServ();
				numRowsOnRack = Int32.Parse(currentProduct.Substring(1, 2));
				numColsOnRack = Int32.Parse(currentProduct.Substring(3, 2));
				rackSize = numRowsOnRack * numColsOnRack;
				Debug.WriteLine("Get number of No reads");
				numberOfNoReads = scanner.getNumberOfNoReadsFromBioServ();
				Debug.WriteLine("Get number of tubes");
				totalNumberOfTubesRead = scanner.getNumberOfTubesOnRackFromBioServ();
				Debug.WriteLine("Get number of reads");
				tubesRead = scanner.getNumberOfReadsFromBioServ();
				Debug.WriteLine("Get decode time");
				decodeTime = scanner.getDecodeTimeFromBioServ();
				rackOrientation = true;
				rackPresent = true;
				Debug.WriteLine("Get bar code data");
				barcodes = scanner.getBarcodeData();
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
			}
		}

		/// <summary>
		/// return Rack Matrix
		/// </summary>
		public int[,] returnRackMatrix {
			get { return rackMatrix; }
			set {
				rackMatrix = new int[NumRows, NumCols];
			}
		}

		/// <summary>
		/// List of 2D barcodes 
		/// </summary>
		public Dictionary<String, String> Barcodes {
			get { return barcodes; }
			set {
				barcodes = new Dictionary<String, String>();
				barcodes = value;
			}
		}

		/// <summary>
		/// Return rack Id
		/// </summary>
		public string RackId {
			get { return rackId; }
			set { rackId = value; }
		}

		/// <summary>
		/// Return tubes read
		/// </summary>
		public int TubesRead {
			get { return tubesRead; }
			set { tubesRead = value; }
		}
		/// <summary>
		/// return Current Product
		/// </summary>
		public string CurrentProduct {
			get { return currentProduct; }
			set { currentProduct = value; }
		}

		/// <summary>
		/// Return rack size
		/// </summary>
		public int RackSize {
			get { return rackSize; }
			set { rackSize = value; }
		}

		/// <summary>
		/// Return number of rows on rack
		/// </summary>
		public int NumRows {
			get { return numRowsOnRack; }
			set { numRowsOnRack = value; }
		}

		/// <summary>
		/// Return number of cols on rack
		/// </summary>
		public int NumCols {
			get { return numColsOnRack; }
			set { numColsOnRack = value; }
		}

		/// <summary>
		/// Return number of tubes read
		/// </summary>
		public int returnnumberoftubesread {
			get { return totalNumberOfTubesRead; }
			set { totalNumberOfTubesRead = value; }
		}

		/// <summary>
		/// Return number of no reads
		/// </summary>
		public int numberofnoreads {
			get { return numberOfNoReads; }
			set { numberOfNoReads = value; }
		}

		/// <summary>
		/// Return decode time
		/// </summary>
		public string returndecodettime {
			get { return decodeTime; }
			set { decodeTime = value; }
		}

		/// <summary>
		/// Return if rack present on scanner
		/// </summary>
		public bool returnifrackpresentonscanner {
			get { return rackPresent; }
			set { rackPresent = value; }
		}

		/// <summary>
		/// Return rack orientation 
		/// </summary>
		public bool rackorientation {
			get { return rackOrientation; }
			set { rackOrientation = value; }
		}
	}
}
